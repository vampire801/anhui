using UnityEngine;
using System.Collections.Generic;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class SelectAreaPanelData
    {
        public bool isPanelShow;  //面板是否显示的标志位
        public bool isShowCityPanel;  //是否显示城市的面板

        public int iOpenStatus; //打开状态，1表示在开始界面打开，2表示在创建房间界面打开 3表示在创建麻将馆选择地区显示  4表示在雀神广场中切换地区显示
                                // 5表示在大厅打开地区面板 6表示在玩法规则面板打开 7表示在申请开馆界面改变地区面板 8表示第一次进入麻将馆显示地图

        public int iCityId = 550;  //选择的城市编号
        public int iCountyId = 341125;  //选择的农村编号


        public bool isGetCitySuccess; //获取城市信息成功
        public bool isGetCountySuccess;  //获取县的信息成功

        public bool isMove;  //地图是否移动的标志位
        public bool isStartMove;  //地图是否开始移动

        public const string url_city = "getCity.x";  //网页的后缀
        public const string url_county = "getCounty.x";  //网页的后缀
        public bool IsGetCityMessage;  //是否已经读取过城市信息的标志位
        //public bool IsGetCountyMessage; //是否已经读取县城的标志位
        public bool[] IsGetCountyMessage = new bool[2] { false, false }; //是否已经读取县城的标志位
        /// <summary>
        /// 0-大厅 1-规则 2麻将馆创建房间 3开麻将馆 4广场 5申请开馆
        /// </summary>
        public int pos_index;

        #region 城市的配置信息
        //保存城市开启的信息，是市的id
        public List<CityMessage> listCityMessage = new List<CityMessage>();
        //城市的配置信息
        public class CityMessage
        {
            public string cityId;  //城市id   
            public string cityName; //城市名字         
            public string flag;  //是否激活的标志位，0表示未激活，1表示待激活,2表示已经激活
        }

        public class CityMessage_
        {
            public int status; //1成功，9系统错误
            public List<CityMessage> data; //城市数据
        }

        public class CityMessageData
        {
            public List<CityMessage_> CityData = new List<CityMessage_>();
        }


        /// <summary>
        /// 获取已开放城市的信息
        /// </summary>
        /// <param name="json"></param>
        public void GetCityMessage(string json, int status)
        {
            listCityMessage.Clear();
            CityMessageData data = new CityMessageData();
            data = JsonBase.DeserializeFromJson<CityMessageData>(json.ToString());
            if (data.CityData[0].status != 1)
            {
                Debug.LogError("获取网页json数据状态错误,status:" + data.CityData[0].status);
                return;
            }


            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            //保存获取的城市信息
            for (int i = 0; i < data.CityData.Count; i++)
            {
                for (int j = 0; j < data.CityData[i].data.Count; j++)
                {
                    CityMessage message = new CityMessage();
                    message.cityId = data.CityData[i].data[j].cityId;
                    message.cityName = data.CityData[i].data[j].cityName;
                    message.flag = data.CityData[i].data[j].flag;
                    listCityMessage.Add(message);
                    if (System.Convert.ToInt16(message.flag) == 2)
                    {
                        pspd.listCityMessage.Add(message);
                    }
                }
            }
            //获取城市信息成功
            isGetCitySuccess = true;

            if (isGetCountySuccess)
            {
                //改变地图大小
                UIMainView.Instance.SelectAreaPanel.ChangeScale();
            }
        }
        #endregion


        #region 县城的配置信息
        //县的配置信息
        public class CountyMessage
        {
            public string countyId;  //县的id
            public string countyName;  //县城的名字            
            public string cityId;  //该县对应的城市的id
            public string flag; //是否激活的标志位，0表示未激活，1表示待激活,2表示已经激活
            public string num; //麻将馆数量
        }

        public class CountyMessage_
        {
            public int status;  //1成功，9系统错误
            public List<CountyMessage> data;  //县城的数据
        }

        public class CountyMessageData
        {
            public List<CountyMessage_> CountyData = new List<CountyMessage_>();
        }

        //保存城市对应的县城的开启状况int 是市的id
        public Dictionary<int, List<CountyMessage>> dicCountyMessage = new Dictionary<int, List<CountyMessage>>();

        /// <summary>
        /// 获取已开放县城的信息
        /// </summary>
        /// <param name="json"></param>
        public void GetCountyMessage(string json, int status)
        {

            dicCountyMessage.Clear();
            CountyMessageData data = new CountyMessageData();
            data = JsonBase.DeserializeFromJson<CountyMessageData>(json.ToString());
            if (data.CountyData[0].status != 1)
            {
                Debug.LogError("获取网页json数据状态错误,status:" + data.CountyData[0].status);
                return;
            }

            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            //保存获取的城市信息
            for (int i = 0; i < data.CountyData.Count; i++)
            {
                for (int j = 0; j < data.CountyData[i].data.Count; j++)
                {
                    CountyMessage message = new CountyMessage();
                    message.cityId = data.CountyData[i].data[j].cityId;
                    message.countyId = data.CountyData[i].data[j].countyId;
                    message.countyName = data.CountyData[i].data[j].countyName;
                    message.flag = data.CountyData[i].data[j].flag;
                    message.num= data.CountyData[i].data[j].num;
                    bool isAdd = false;
                    if (System.Convert.ToInt16(message.flag) == 2)
                    {
                        isAdd = true;
                    }

                    //如果字典中已经有这个键，直接添加县
                    if (dicCountyMessage.ContainsKey(System.Convert.ToInt32(message.cityId)))
                    {
                        dicCountyMessage[System.Convert.ToInt32(message.cityId)].Add(message);
                    }
                    else
                    {
                        List<CountyMessage> county = new List<CountyMessage>();
                        county.Add(message);
                        dicCountyMessage.Add(System.Convert.ToInt32(message.cityId), county);
                    }


                    if (isAdd)
                    {
                        if (pspd.dicCountyMessage.ContainsKey(System.Convert.ToInt32(message.cityId)))
                        {
                            pspd.dicCountyMessage[System.Convert.ToInt32(message.cityId)].Add(message);
                        }
                        else
                        {
                            List<CountyMessage> county = new List<CountyMessage>();
                            county.Add(message);
                            pspd.dicCountyMessage.Add(System.Convert.ToInt32(message.cityId), county);
                        }
                    }
                }
            }

            ////添加县城的测试
            //CountyMessage message_ = new CountyMessage();
            //message_.countyId = "140428";
            //message_.cityId = "355";
            //message_.countyName = "长子县";
            //message_.flag = "2";
            //List<CountyMessage> county_ = new List<CountyMessage>();
            //county_.Add(message_);
            //if (dicCountyMessage.ContainsKey(System.Convert.ToInt32(message_.cityId)))
            //{
            //    dicCountyMessage[System.Convert.ToInt32(message_.cityId)].Add(message_);
            //}

            //获取县的信息成功
            isGetCountySuccess = true;

            if (isGetCitySuccess)
            {
                //改变地图大小
                UIMainView.Instance.SelectAreaPanel.ChangeScale();
            }
        }
        #endregion



    }

}
