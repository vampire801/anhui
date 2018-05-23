using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class Waring : MonoBehaviour
    {
        public Text text;  //字体显示
        public GameObject tf; //三个及以上的父物体
        public GameObject two;  //存放两个的父物体

        /// <summary>
        /// 玩家内容
        /// </summary>
        /// <param name="ip">玩家的信息</param>
        /// <param name="status">1表示ip，2表示mac，3表示距离</param>
        public void Ip(Dictionary<string, List<int>> ip, int status)
        {
            //Debug.LogError("==================================0");
            if (ip.Count <= 0)
            {
                // Debug.LogError("==================================1");
                return;
            }

            switch (status)
            {
                case 1:
                    text.text = "IP地址相同";
                    break;
                case 2:
                    text.text = "MAC地址相同";
                    break;
                case 3:
                    text.text = "距离在100米之内";
                    break;
            }

            if (ip.Count == 1)
            {
                //Debug.LogError("==================================3");
                foreach (string key in ip.Keys)
                {
                    //Debug.LogError("key：" + key + ",value：" + ip[key].Count);
                    if (ip[key].Count < 2)
                    {
                        //Debug.LogError("===============1");
                        continue;
                    }
                    for (int i = 0; i < ip[key].Count; i++)
                    {
                        //Debug.LogError("===============key:"+ key+",count:"+ip[key].Count);
                        GameObject go = Instantiate(Resources.Load<GameObject>("Game/AntiCheck/HeadIamge"));
                        go.transform.SetParent(tf.transform);
                        go.transform.localScale = Vector3.one;
                        go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                        anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(go.transform.Find("RawImage").GetComponent<RawImage>(),
                            GameData.Instance.PlayerPlayingPanelData.usersInfo[ip[key][i] + 1].szHeadimgurl);
                    }
                }
            }
            else if (ip.Count >= 2)
            {
                //Debug.LogError("==================================2");
                int index = 0;
                foreach (string key in ip.Keys)
                {
                    if (ip[key].Count < 2)
                    {
                        continue;
                    }
                    for (int i = 0; i < ip[key].Count; i++)
                    {
                        //      Debug.LogError("key：" + key + "，value：" + ip[key].Count);

                        GameObject go = Instantiate(Resources.Load<GameObject>("Game/AntiCheck/HeadIamge"));
                        if (index == 0)
                        {
                            go.transform.SetParent(tf.transform);
                        }
                        else
                        {
                            go.transform.SetParent(two.transform);
                        }

                        go.transform.localScale = Vector3.one;
                        go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                        anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(go.transform.Find("RawImage").GetComponent<RawImage>(),
                            GameData.Instance.PlayerPlayingPanelData.usersInfo[ip[key][i] + 1].szHeadimgurl);
                    }
                    index++;
                }
            }
        }

    }
}