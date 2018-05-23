using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class CreatRoomMessagePanelData
    {
        public bool PanelShow;  //面板是否显示的标志位

        public int AnvanceSettingStatus = 1; //高级按钮的点击状态，1表示可点展开，2表示可点收起
        public int[] ChoiceIndex = new int[] { 1, 1, 1 };  //三个面板分别选择的下标
        public int GameNum;  //选择的局数
        public int MulLimit;  //番数的上限
                              // public int PlayMethod;  //玩法

        public int iCityId_Parlor;
        public int iCountyId_Parlor;
        public int LastCreatTime; //记录上次创建的房间的时间 

        #region 玩家发送选择房间参数
        public int MethodId;  //玩法的id 
        public int CreatRoomType = 1;  //创建房间的类型，1表示自己创建，2表示代理代开
        public int iColorFlag = 1;  //颜色标志,
        public int iCompliment = 0;  //赞的数量
        public int iDisconnectRate = 0;  //掉线率要求
        public int iDiscardTime = 0; //出牌速度要求
        public int iRoomCard = 1;  //开房要消耗的房卡数量
        public const string SaveRuleParamName = "SaveRuleParam";
        public uint iPrice = 1;  //付费
        public int iMultiple = 2; //庄闲倍数，现在默认2
        public int iBasePoint = 1;  //低分，默认1
        public int iExposedMode = 0;  //明杠收三家  0表示没选 1表示选中
        public int iSevenPairs = 0; //七对 
        public int iLuxurySevenPairs = 0; //豪华七对
        public int iThirteenIndepend = 0;  //十三不靠
        public int iFourKong = 0;  //四杠荒庄
        public int iThirteenOrphans = 0;  //十三幺
        public int iRaiseMode = 0;  //前抬后和，0表示不用，1表示前抬，2表示后和
        public int iDraw = 0;  //必须自摸 0表示可接炮  1表示仅可自摸
        public int iMultiShoot = 0;  //仅当可接炮选择时才会出现一炮多响
        public int iMultiPay = 0;  //0只收放炮的，1收三家【自摸设置必须是可接炮才有效】
        /// <summary>
        /// 房间规则保存
        /// </summary>
        public uint[] roomMessage_ = new uint[MahjongLobby_AH.Network.Message.NetMsg.OpenRoomParamRow];  //保存玩家的选择房间信息
        public List<MethordRuleClass> allRoomMethor = new List<MethordRuleClass>();
        // public const int infoLengh =60;
        public int[] iParamOpenRoomMessage = new int[MahjongLobby_AH.Network.Message.NetMsg.OpenRoomParamRow];  //保存玩家的默认选择的创建房间的信息
        public bool isSavedParamMethid;  //玩家上次玩家创建的玩法id
        //  public int[] iParamOpenRoomMessage_put = new int[infoLengh]; //读取玩家的默认创建房间的信息
        public class MethordRuleClass
        {
            public int methord;
            public uint[] param = new uint[MahjongLobby_AH.Network.Message.NetMsg.OpenRoomParamRow];
            public MethordRuleClass()
            {

            }
            public MethordRuleClass(int meth, uint[] par)
            {
                methord = meth;
                for (int i = 0; i < par.Length; i++)
                {
                    param[i] = par[i];
                }
            }
        }
      
                                         // public int iPayId; //记录玩家上次选择的付费方式

        #endregion

        //赋值
        public uint[] GetValue()
        {
            // Debug.Log("第十六位："+iPrice);
            anhui.MahjongCommonMethod.Instance.WriteInt32toInt4(ref roomMessage_[1], iPrice, 16);
            //   roomMessage_[0] = (sbyte)iPrice;

            //保存玩家的付费方式
            return roomMessage_;
        }

    }
}
