using UnityEngine;
using System.Collections.Generic;
using System;
using MahjongLobby_AH.Network.Message;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class LobbyMainPanelData
    {
        #region Json_Voucher
        [Serializable]
        public class Voucher
        {
            public int status; //1成功0参数错误  9系统错误
            public VoucherData[] data;
        }
        [Serializable]
        public struct VoucherData
        {
            public string voucherId;//代金券编号
            public string amount;//代金券金额
            public string getTime;//：获得时间
            public string useTime;//使用时间，如果未使用值为0
            public string exTime;//过期时间
            public string limit;//使用限制
            public string orderId;//使用的充值订单号，如果未使用值为0
            /// <summary>
            /// //代金券状态，0未使用 1使用未完成 2已使用
            /// </summary>
            public string state;
        }
        public class  IVoucherData
        {
            public int isFirst;
            public int iCanUse;
            public int voucherId;
            public int amount;
            public int getTime;
            public int useTime;//使用时间，如果未使用值为0
            public int exTime;//过期时间
            public int limit;//使用限制
            public int  state;//使用状态 0未使用 
            public string orderId;//使用的充值订单号，如果未使用值为0
        }
        #endregion Json_Voucher

        #region Json_Ad
        [Serializable]
        public class Json_Ad
        {
            public int status;
            public string title;
            public int num;
            public Ad_Data[] data;

        }
        [Serializable]
        public class Ad_Data
        {
            public string title;
            public string context;
        }
        #endregion


        /// <summary>
        /// 预约开房参数
        /// </summary>
        [Serializable]
        public class ReservationParameters
        {
            public int status;// 1成功  9系统错误 0没有配置参数
            public int maxReserveTim;// 最大预约时间（单位：秒）-
            public int playTimeOut1;// 超时托管时间选项1-
            public int playTimeOut2;// 超时托管时间选项2-
            public int playTimeOut3;// 超时托管时间选项3-
        }
        public bool isPanelShow;//是否显示面板      
        public string Uri_Gene = "http://www.9game.cn/byqb/"; //推广链接             
        public bool IsShowCreatRoomPanel;  //是否显示创建面板       
        public string url_Avatar;  //头像地址，前期测试使用，后期删除
        public bool isShowBuyRoomCard; //是否显示购买房卡的面板
        public bool isShowChoiceChannelPanel;//是否显示渠道面板
        public Voucher vc = new Voucher();
        public List< IVoucherData> _dicVoucher = new List<IVoucherData>();
        public Json_Ad ja = new Json_Ad();
        //存储大厅的公告通知消息
        public List<string> lsBulletinNotice = new List<string>();
        public bool isPlayingBulletin;  //是否正在播放公告的标志位
        public bool isJoinedRoom;  //是否已经有加入其他房间的标志位      

        public bool isDown;//充值按钮标志位  
        public int iChargeId;//当前充值编号
        public string szOrderId;//当前订单编号

        public int CardNumstatus;  //礼包获取的状态，0表示不加入自己背包，1表示加入
        public int NewPlayerCardNum = 30;  //新手房卡数量
        public string UrlVoucher = "userVoucher.x?uid={0}";
        public string UrlAd = "ad.x";

        public ReservationParameters m_lReservationParameters = new ReservationParameters();
        internal bool isShowDoubleIcon;
        internal bool isShowSendCoin;
        internal float oriPos;
        internal float downPos;
        /// <summary>
        /// 获取预约时间
        /// </summary>
        public void OnGetAppointmentURL()
        {
            string url = "  ";
            if (SDKManager.Instance.IOSCheckStaus == 0)
            {
                url = LobbyContants.MAJONG_PORT_URL + "getParam.x";
            }
            else
            {
                url = LobbyContants.MAJONG_PORT_URL_T + "getParam.x";
            }
            anhui.MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, null, OnGetValue, " ", 5);
        }

        void OnGetValue(string json, int status)
        {
            ReservationParameters data = new ReservationParameters();
            data = JsonUtility.FromJson<ReservationParameters>(json);
            if (data.status != 1)
            {
                Debug.LogWarning ("没有预约" + data.status);
                return;
            }
            UIMainView.Instance.CreatRoomMessagePanel.CanUserHous = (data.maxReserveTim/3600);//转化为小时
            UIMainView.Instance.CreatRoomMessagePanel.lTuoGuan[0] = "无";
            UIMainView.Instance.CreatRoomMessagePanel.lTuoGuan[1] = data.playTimeOut1+"秒";
            UIMainView.Instance.CreatRoomMessagePanel.lTuoGuan[2] = data.playTimeOut2+"秒";
            UIMainView.Instance.CreatRoomMessagePanel.lTuoGuan[3] = data.playTimeOut3+"秒";
        }
    }
   

    public class CountyMessageDataReservationParameters
    {
        public List<LobbyMainPanelData.ReservationParameters> CountyData = new List<LobbyMainPanelData.ReservationParameters>();
    }
}
