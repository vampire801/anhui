using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using XLua;

namespace MahjongGame_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class AntiCheatingPanelData
    {
        public bool isPanelShow;  //面板显示的标志位     
        public int iReadyShowAnti;  //已经显示过防作弊面板的次数
        public string[] Ip = new string[4];  //四个玩家的ip地址
        public string[] MacId = new string[4];  //四个玩家的mac地址
        public float[,] PlayerPos = new float[4, 2];  //四个玩家的位置经纬度
        public bool isSpwanEnd;  //是否已经产生动画结束的预置体
        public bool isFinishPutCard; //是否完成发牌的动画
        /// <summary>
        /// 获取四个玩家之间IP地址的检测结果
        /// </summary>
        /// <returns>每个ip地址有多少个玩家</returns>
        public Dictionary<string, List<int>> GetPlayerIpSame()
        {
            Dictionary<string, List<int>> Result = new Dictionary<string, List<int>>();

            for (int i = 0; i < 4; i++)
            {
                //Debug.Log ("ip:" + Ip[i] + ",length:" + Ip[i].Length);
                if (Result.ContainsKey(Ip[i]))
                {
                    Result[Ip[i]].Add(i);
                }
                else
                {
                    List<int> temp = new List<int>();
                    temp.Add(i);
                    Result.Add(Ip[i], temp);
                }
            }

            foreach (var t in Result)
            {
                Debug.Log ("相同ip的数量:" + t.Key + ",:" + t.Value.Count);
            }

            return Result;
        }


        /// <summary>
        /// 获取四个玩家之间Mac地址的检测结果
        /// </summary>
        /// <returns>每个Mac地址有多少个玩家</returns>
        public Dictionary<string, List<int>> GetPlayerMacIdSame()
        {
            Dictionary<string, List<int>> Result = new Dictionary<string, List<int>>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (j != i)
                    {
                        if (string.Compare(MacId[i], MacId[j]) == 1)
                        {
                            if (Result.ContainsKey(MacId[i]))
                            {
                                Result[MacId[i]].Add(j);
                            }
                            else
                            {
                                List<int> temp = new List<int>();
                                temp.Add(j);
                                Result.Add(MacId[i], temp);
                            }
                        }
                    }
                }
            }
            return Result;
        }


        [DllImport("__Internal")]
        private static extern double _CalcDistanceBetweenCoor(double l1, double l2, double l3, double l4);

        /// <summary>
        /// 获取玩家与玩家之间小于100的玩家index
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<int>> GetPlayerDistance()
        {
            Dictionary<string, List<int>> value = new Dictionary<string, List<int>>();

            //玩家之间的距离          
            for (int i = 0; i < 3; i++)
            {
                //如果玩家定位失败，就不会参与防作弊判断
                if (PlayerPos[i, 0] == 0 || PlayerPos[i, 1] == 0)
                {
                    continue;
                }
                for (int j = i + 1; j < 4; j++)
                {
                    if (PlayerPos[j, 0] == 0 || PlayerPos[j, 1] == 0)
                    {
                        continue;
                    }

#if UNITY_ANDROID
                    AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity");
                    AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
                    //调用安卓的测距方法
                    float distance = jo.Call<float>("GetDistance", PlayerPos[i, 0], PlayerPos[i, 1], PlayerPos[j, 0], PlayerPos[j, 1]);

#elif UNITY_IOS
                    //调用ios的测距方法
                    float distance = (float)_CalcDistanceBetweenCoor(PlayerPos[i, 0], PlayerPos[i, 1], PlayerPos[j, 0], PlayerPos[j, 1]);            
#endif

                    Debug.LogWarning("distance:" + distance);
                    if (distance < 100 && distance >= 0)
                    {
                        if (value.ContainsKey(i.ToString()))
                        {
                            value[i.ToString()].Add(j);
                        }
                        else
                        {
                            List<int> temp = new List<int>();
                            temp.Add(j);
                            value.Add(i.ToString(), temp);
                        }
                    }
                }
            }


            Dictionary<string, List<int>> temp_value = new Dictionary<string, List<int>>();

            //判断玩家之间距离是否是在100米范围内
            foreach (string key in value.Keys)
            {
                Debug.LogWarning("数量:" + value[key].Count);

                //四个人在一个区域
                if (value[key].Count == 3)
                {
                    //所有人都在一个区域
                    List<int> temp = new List<int>();
                    for (int i = 0; i < value[key].Count; i++)
                    {
                        Debug.LogWarning("玩家座位号:" + value[key][i]);
                        temp.Add(value[key][i]);
                    }
                    temp.Add(System.Convert.ToInt16(key));
                    temp_value.Add(key, temp);
                    return temp_value;
                }

                //三个人在一个区域
                if (value[key].Count == 2)
                {
                    List<int> temp = new List<int>();
                    for (int i = 0; i < value[key].Count; i++)
                    {
                        Debug.LogWarning("玩家座位号:" + value[key][i]);
                        temp.Add(value[key][i]);
                    }
                    temp.Add(System.Convert.ToInt16(key));
                    temp_value.Add(key, temp);
                    return temp_value;
                }

                //俩个人在一个区域
                if (value[key].Count == 1)
                {
                    List<int> temp = new List<int>();
                    for (int i = 0; i < value[key].Count; i++)
                    {
                        Debug.LogError("玩家座位号:" + value[key][i]);
                        temp.Add(value[key][i]);
                    }
                    temp.Add(System.Convert.ToInt16(key));
                    temp_value.Add(key, temp);
                }
            }


            return temp_value;
        }

    }

}
