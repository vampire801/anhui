using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using MahjongGame_AH;
using MahjongGame_AH.Data;
using MahjongGame_AH.Network.Message;
using XLua;

namespace MahjongGame_AH.Data
{
    //public struct RedPage_Game
    //{
    //    public int num;
    //    public int Gold;
    //    public string moneytype;
    //    public string moneytypetype;
    //    public string name;
    //}

    internal enum CardType : byte
    {
        WAN1 = 0x11, WAN2 = 0x12, WAN3 = 0x13, WAN4 = 0x14, WAN5 = 0x15, WAN6 = 0x16, WAN7 = 0x17, WAN8 = 0x18, WAN9 = 0x19,
        TON1 = 0x21, TON2 = 0x22, TON3 = 0x23, TON4 = 0x24, TON5 = 0x25, TON6 = 0x26, TON7 = 0x27, TON8 = 0x28, TON9 = 0x29,
        TIO1 = 0x31, TIO2 = 0x32, TIO3 = 0x33, TIO4 = 0x34, TIO5 = 0x35, TIO6 = 0x36, TIO7 = 0x37, TIO8 = 0x38, TIO9 = 0x39,
        FE = 0x41, FS = 0x42, FW = 0x43, FN = 0x44,
        JZ = 0x51, JF = 0x52, JB = 0x53,
        HSP = 0x61, HSU = 0x62, HAU = 0x63, HWI = 0x64, HM = 0x65, HL = 0x66, HZ = 0x67, HJ = 0x68
    }
    [Hotfix]
    [LuaCallCSharp]
    public class PlayerPlayingPanelData
    {
        //是否在选择能跑能下界面
        public bool isWatingPlayerDownOrUp;
        //是否显示游戏相关面板
        public bool isPanelShow_Playing;
        //是否显示等待界面相关面板
        public bool isPanelShow_Wating;
        public bool isShowSettingGroup;
        #region 玩家信息
        //存储同桌其他玩家的信息
        public Dictionary<int, NetMsg.UserInfoDef> usersInfo = new Dictionary<int, NetMsg.UserInfoDef>();
        //玩家的座位编号
        public int playerPos;
        //玩家是否准备好的标志位        
        public bool IsReady;
        //是否是庄家 
        public bool isBanker;
        //房间的id
        public int iRoomId;
        //玩家的id
        public int iUserId;
        //玩家的离线信息，离线为1，在线为0
        public int[] DisConnectStatus = new int[4];
        //保存玩家花牌玩法手中花牌的数量
        public int[] PlayerFlowerCount = new int[4] { 0, 0, 0, 0 };
        #endregion

        #region 游戏相关信息        
        public bool isSpwanSpecialCard;  //产生特殊牌的时候，只包括吃碰
        public bool isGangAndPengEnable; //可杠可碰的同时出现
        public bool isGangToPeng; //可杠可碰但是点击碰的标志位  
        public bool isGangToPeng_Sort;
        public bool isGangToPeng_Put;
        public bool isGangToPeng_Later;
        public NetMsg.ClientDiscardTileNotice DiscardTileNotice = new NetMsg.ClientDiscardTileNotice(); //保存出牌通知的信息
        public bool isBreakConnectControlInterval; //控制牌的间隙
        public bool isBreakConnect;  //断线重入的标志位
        public bool isRecordScore_Break; //用于记录玩家的重入的分数的标志

        public bool isLastPongOrKong;  //最后一次操作是否是吃碰杠抢

        public int iBeginRoundGame = 0;  //是否开始一局的标志位
        public bool isBeginGame;  //是否开始游戏的标志位
        public int FinshedGameNum = 0;  //已经完成的游戏局数    
        public bool isFirstZero;  //玩家获取的游戏玩法，是否是0，如果是0在玩法配置中添加局数
        public int AllGameNum = 8; //总局数        
        public int FinshedQuanNum = 1; //完成的圈数
        public int GameLowScore;  //游戏的底分

        public bool isFirstDealCard = true;  //是否是第一次发牌  TODO:处理在断线重入的情况下置为false
        public bool isSendPlayerPass = true;  //玩家点击过，是否发送请求消息
        public bool isNeedSendPassMessage = false;  //是否需要发送弃的消息
        public byte FirstDealCard; //第一次发的牌的花色值  
        public int iSpwanCardNum = 0;  //已经产生牌的数量
        public int iSpecialCardNum = 0;  //已经产生特殊牌型的数量，主要是包括吃碰杠胡
        public bool isCanHandCard = false; //玩家自己是否可以出牌的标志位     
        public byte bLastCard;  //上一次出的牌，或者是自己摸到的牌  
        public byte bkongValue;  //玩家要进行杠的牌的值
        public byte byCanDownRu; //开启能下/能跑：0不开启，1能下 2能跑（如果自己是庄为能下/则能跑）
        public bool isGameEnd; //游戏是否结束的标志位，用于判断玩家的从后台切回来之后是否要断线重连
        public byte LaiZi = 0;//癞子牌值  0-无癞子
        //可以杠的花色值
        public class bkong
        {
            public byte kongValue;  //杠的值
            public byte kongType;  //杠的类型，1表示暗杠，2表示碰杠
        }
        public List<bkong> bkongValue_ = new List<bkong>();  //玩家可以杠的牌        

        public byte bAnkongStatus;  //是否是暗杠的标志位0表示没有暗杠，1表示有暗杠
        public byte byJustGetCard;  //玩家自己刚摸到的手牌          
        public int[] sScoreShow = new int[4];  //存储四个玩家的总分数                    
        public float DownTime; //倒计时时间      
        public float DownTimer_Limit = 999f; //倒计时上限时间  
        public int LeftCardCount = 144;  //剩余牌的数量
                                         // public int LeftCardCount_DisConnect; //断开连接之后的剩余牌的数量
        public int iGangNum = 0;  //杠的次数
        public int iMethodId; //玩法id
        public NetMsg.PlayingMethodConfDef playingMethodConf = new NetMsg.PlayingMethodConfDef();//玩家的房间的玩法信息
        public int lcDealerMultiple; //潞城是否下分的状态
        internal byte byDealerSeat;//庄家座位号
        internal int bySeatNum;//自己座位号
        internal int iOpenRoomUserId;//房主的id

        internal string szGameNum;//一局游戏编号
        internal byte byType;//计分通知类型.类型：1前台，2明杠，3暗杠，4后和
        internal byte byLastCard;//第十四张牌
        public int isChoiceTing;  //是否已经选择了听牌的标志位
        public byte[] isChoiceTing_ALL = new byte[4];  //所有玩家的听牌标志位
        public bool isAlreadyChangeStatus;  //是否已经改变过了玩家的手牌状态
        public byte[] SeatID = new byte[4];// 0没人 1有人 2有人并且占座 3有人占座不在房间内
        public int[] TablePlayerUserID = new int[4] { 0, 0, 0, 0 };//桌上四个玩家的userID

        ///// <summary>
        ///// 红包数量
        ///// </summary>
        //public List<RedPage_Game> m_lRP = new List<RedPage_Game>();
        /// <summary>
        /// 0-没碰杠，没吃抢 1-碰杠过 2-吃抢过
        /// </summary>
        internal int iTirState;
        /// <summary>
        /// 是否做牌;
        /// </summary>
        internal byte isChoicTir;
        internal byte isNeedSort;
        #endregion

        #region 托管相关消息
        public int iPlayerHostStatus;  //玩家的托管状态 0表示不托管  1表示到时间被动托管  2表示主动托管
        public byte[] iAllPlayerHostStatus = new byte[4]; //所有玩家的托管状态 0--3  座位号-1
        public bool isAllAutoStatus;  //是否是四个玩家托管状态
        public int iDissolveFlag; //玩家的托管状态
        public int iDissolveStatus;
        internal const int GLOBLE_BUTTONTIME=5;
        public float time_t = GLOBLE_BUTTONTIME; //道具倒计时
        public IEnumerator DelayToolBtnClick= null ;
       
        public NetMsg.ClientDismissRoomRes DismissRoomRes = new NetMsg.ClientDismissRoomRes();  //保存玩家的托管发起的解散信息
        #endregion

        internal const string szThirteenChoiceDis = "确认将只可成十三幺，不可成其他牌型。开启吃、抢功能，关闭碰、杠功能。";
        internal const string szThirteenChoiceDis_LC = "确认将只可成十三幺，不可成其他牌型。开启抢功能，关闭碰、杠功能。";
        internal const string szThirteenUnableChoice = "已操作过碰、杠功能，不可成十三幺";
        internal const string szThirteenUnableCancal = "已操作过吃、抢功能，不可取消";
        internal const string currentDCardPre = "currentDCardPre";
        internal const string currentMoCardPre = "currentMoCardPre";
        internal const string currentHCardPre = "currentHCardPre";
        internal const string currentUCardPre = "currentUCardPre";
        internal const string pegaDCardsPre = "pegaDCardsPre";
        internal const string pegaHCardsPre = "pegaHCardsPre";
        internal const string pegaUCardsPre = "pegaUCardsPre";
        internal const string showHCardPre = "showHCardPre";
        internal const string showVCardPre = "showVCardPre";
        internal const string TabelBigCard = "TabelBigCard";
        internal NetMsg.AgainLoginGameData againLoginData = new NetMsg.AgainLoginGameData();
        /// <summary>
        /// 0-自己 
        /// </summary>
        public UserCardsInfo[] usersCardsInfo = new UserCardsInfo[4];
        /// <summary>
        /// 开房模式：1普通 2馆主 3馆内成员
        /// </summary>
        internal byte byOpenRoomMode;
        internal bool isVoiceLogin;

        public PlayerPlayingPanelData()
        {
            for (int i = 0; i < 4; i++)
            {
                usersCardsInfo[i] = new UserCardsInfo();
            }

            playingMethodConf.byWinSpecialThirteenOrphans = (byte)MahjongLobby_AH.GameData.Instance.CreatRoomMessagePanelData.iThirteenOrphans;
        }


        /// <summary>
        /// 根据其他玩家的ID获取其他玩家在屏幕上的位置 返回的是从零开始的下标
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int GetOtherPlayerShowPosForOtherPlayerID(int userID)
        {
            int actualPos = GetOtherPlayerPos(userID);
            return (GetOtherPlayerShowPos(actualPos) - 1);
        }

        /// <summary>
        /// 获取其他玩家应该显示的位置
        /// </summary>
        /// <param name="otherPos">其他玩家的位置,1----4</param>
        /// <returns></returns>
        public int GetOtherPlayerShowPos(int otherPos)
        {
            if (playerPos == 0)
            {
                return 0;
            }
            int index = 0;
            if (otherPos == playerPos)
            {
                index = 1;
            }
            else
            {
                int pos = 0;
                if ((otherPos - (playerPos - 1)) > 0)
                {
                    pos = otherPos - (playerPos - 1);
                }
                else
                {
                    pos = otherPos - (playerPos - 1) + 4;
                }
                index = pos;
            }
            return index;
        }
        /// <summary>
        /// 根据其他玩家的id，获取该玩家的座位号
        /// </summary>
        /// <param name="iuserId"></param>
        /// <returns></returns>
        public int GetOtherPlayerPos(int iuserId)
        {
            int pos = 0;
            if (iuserId == iUserId)
            {
                pos = playerPos;
                // Debug.LogError("pos:" + pos + ",playerPos:" + playerPos);
                return pos;
            }
            for (int i = 1; i < usersInfo.Count + 1; i++)
            {
                if (usersInfo.ContainsKey(i) && iuserId == usersInfo[i].iUserId)
                {
                    pos = usersInfo[i].cSeatNum;
                    break;
                }
            }

            return pos;
        }


        //对当前玩家的手牌进行排序，规则：明暗杠、吃、碰牌优先排序，普通牌:万，筒，条，东西南北中发白，花
        public void CurrentCradSort(int status)
        {
            usersCardsInfo[0].listCurrentCards.Sort(iCompareList);
            if (status == 1)
            {
                UIMainView.Instance.PlayerPlayingPanel.CardsUpdateShow();
            }
            else if (status == 2)
            {
                UIMainView.Instance.PlayerPlayingPanel.CardUpdateShow_Self();
            }
        }

        //当前麻将的值
        class Crad
        {
            public Mahjong mah;
            public float x;
        }

        //初始化牌的排序
        public void Sort_AllCard()
        {
            List<Crad> jong = new List<Crad>();
            Mahjong[] mahjong = MahjongManger.Instance.GetSelfCard();
            for (int i = 0; i < mahjong.Length; i++)
            {
                Crad card = new Crad();
                card.mah = mahjong[i];
                card.x = mahjong[i].transform.localPosition.x;
                jong.Add(card);
            }

            jong.Sort(Compare_Card);


            //先确定第一张牌的位置
            float firstPos = 0f;
            if (jong[0].x == MahjongManger.Instance.FirstPos.x)
            {
                firstPos = MahjongManger.Instance.FirstPos.x;
            }
            else
            {
                if (MahjongManger.Instance.FirstPos.x < jong[0].x)
                {
                    firstPos = jong[0].x;
                }
                else
                {
                    firstPos = MahjongManger.Instance.FirstPos.x;
                }
            }

            //遍历如果所有牌的位置，判断是否正确
            for (int i = 0; i < jong.Count - 1; i++)
            {
                Debug.LogWarning("pos:" + jong[i].x + ",value:" + jong[i].mah.bMahjongValue + ",id:" + jong[i].mah.iMahId);
                if (jong[i].mah.transform.localPosition != new Vector3(firstPos + 85f * i, jong[i].mah.transform.localPosition.y, 0))
                {
                    jong[i].mah.transform.localPosition = new Vector3(firstPos + 85f * i, jong[i].mah.transform.localPosition.y, 0);
                    Debug.LogWarning("pos:" + firstPos);
                }
            }
        }


        //通过麻将的位置进行排序
        int Compare_Card(Crad p1, Crad p2)
        {
            int res = 0;
            if (p1.x >= p2.x)
            {
                res = 1;
            }
            else
            {
                res = -1;
            }
            return res;
        }


        /// <summary>
        /// 获取玩家的所有麻将子物体
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public Mahjong[] GetTransfromChild(Transform trans)
        {
            if (trans == null)
            {
                return null;
            }

            List<Mahjong> list = new List<Mahjong>();
            Mahjong[] go_ = trans.GetComponentsInChildren<Mahjong>(false);
            for (int i = 0; i < go_.Length; i++)
            {
                if (go_[i].enabled)
                {
                    list.Add(go_[i]);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 确定玩家是不是碰杠
        /// </summary>
        public int IsPongKong()
        {
            int status = 0;
            //Debug.LogError("bLastCard:" + bLastCard);
            for (int i = 0; i < usersCardsInfo[0].listSpecialCards.Count; i++)
            {
                if (usersCardsInfo[0].listSpecialCards[i].type == 2)
                {
                    if (bLastCard == usersCardsInfo[0].listSpecialCards[i].mahValue[0])
                    {
                        status = 1;
                    }
                }
            }
            return status;
        }


        //手牌排序规则
        int iCompareList(CurrentCard b, CurrentCard a)
        {
            int res = 0;
            //获取麻将牌的花色和大小

            //Debug.LogError ("红中"+MahjongCommonMethod.Instance.iPlayingMethod);
            if (anhui.MahjongCommonMethod.Instance.iPlayingMethod == 20001)//如果是红中麻将
            {
                if (a.cardNum == 0x51 && b.cardNum != 0x51)
                {
                    return 1;
                }
                else if (b.cardNum == 0x51 && a.cardNum != 0x51)
                {
                    return -1;
                }
            }
            // Debug.LogWarning("a:" + a.cardNum.ToString("x2") +"b_"+b.cardNum .ToString("x2"));

            if (a.cardNum < b.cardNum)
            {
                return 1;
            }
            else if (a.cardNum > b.cardNum)
            {
                return -1;
            }
            //if (a_0 == b_0)
            //{
            //    if (a_1 <= b_1)
            //    {
            //        return 1;
            //    }
            //}
            return res;
        }


        //对物体根据位置排序
        public int Compare_Pos(MainViewPlayerPlayingPanel.TempMahjong p1, MainViewPlayerPlayingPanel.TempMahjong p2)
        {
            int res = 0;
            if (p1.x >= p2.x)
            {
                res = 1;
            }
            else
            {
                res = -1;
            }
            return res;
        }




        /// <summary>
        /// 获取玩家所有的牌，然后转到数组
        /// </summary>
        /// <param name="status">1，2表示只把玩家手里的牌加入到数组中，3表示所有牌添加入数组</param>
        /// <returns></returns>
        public byte[,] GetPlayerAllHandCard(int status)
        {
            byte[,] mahjongValue = new byte[5, 10];
            byte bySuit = 0;
            byte byValue = 0;
            //先把手牌添加到数组中
            for (int i = 0; i < usersCardsInfo[0].listCurrentCards.Count; i++)
            {
                if (usersCardsInfo[0].listCurrentCards[i].cardNum == 0 || usersCardsInfo[0].listCurrentCards[i].cardNum > 96)
                {
                    Debug.LogWarning("=======================添加手牌错误");
                    continue;
                }
                bySuit = (byte)(usersCardsInfo[0].listCurrentCards[i].cardNum / 16);  //花色
                byValue = (byte)(usersCardsInfo[0].listCurrentCards[i].cardNum % 16);  //值
                //确定该花色的牌的总数
                mahjongValue[bySuit - 1, 0]++;
                //添加对应的牌到数组中
                mahjongValue[bySuit - 1, byValue]++;
            }


            //如果玩家选择成十三幺  添加特殊牌
            if (GameData.Instance.PlayerPlayingPanelData.isChoicTir == 1 || status == 3)
            {
                //再把玩家的特殊牌添加到数组中
                for (int i = 0; i < usersCardsInfo[0].listSpecialCards.Count; i++)
                {
                    if (usersCardsInfo[0].listSpecialCards[i].mahValue[0] == 0)
                    {
                        continue;
                    }

                    //碰
                    if (usersCardsInfo[0].listSpecialCards[i].type == 2)
                    {
                        bySuit = (byte)(usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16);
                        byValue = (byte)(usersCardsInfo[0].listSpecialCards[i].mahValue[0] % 16);
                        //添加花色牌的总数
                        mahjongValue[bySuit - 1, 0] += 3;
                        //添加某个花色值
                        mahjongValue[bySuit - 1, byValue] += 3;
                    }

                    //吃
                    if (usersCardsInfo[0].listSpecialCards[i].type == 1)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            bySuit = (byte)(usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16);
                            byValue = (byte)(usersCardsInfo[0].listSpecialCards[i].mahValue[0] % 16);
                            //添加某个花色值
                            mahjongValue[bySuit - 1, byValue] += 1;
                        }

                        //添加花色牌的总数
                        mahjongValue[bySuit - 1, 0] += 3;
                    }

                    //杠
                    if (usersCardsInfo[0].listSpecialCards[i].type == 3)
                    {
                        bySuit = (byte)(usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16);
                        byValue = (byte)(usersCardsInfo[0].listSpecialCards[i].mahValue[0] % 16);
                        //添加花色牌的总数
                        mahjongValue[bySuit - 1, 0] += 4;
                        //添加某个花色值
                        mahjongValue[bySuit - 1, byValue] += 4;
                    }

                    //十三幺 吃 抢
                    if (usersCardsInfo[0].listSpecialCards[i].type == 5 || usersCardsInfo[0].listSpecialCards[i].type == 6)
                    {
                        bySuit = (byte)(usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16);
                        byValue = (byte)(usersCardsInfo[0].listSpecialCards[i].mahValue[0] % 16);
                        //添加花色牌的总数
                        mahjongValue[bySuit - 1, 0] += 1;
                        //添加某个花色值
                        mahjongValue[bySuit - 1, byValue] += 1;
                    }
                }
            }

            return mahjongValue;
        }


        /// <summary>
        /// 获取要吃碰杠的相关牌的信息
        /// </summary>
        /// <param name="value">麻将的花色值</param>
        /// <param name="type">0表示吃，1表示碰，2表示杠</param>
        /// <returns></returns>
        public byte[] GetPlayerSpecialCardTtile(byte value, int type)
        {
            byte[] titles = new byte[2];
            if (type > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    titles[i] = value;
                }

                return titles;
            }

            List<byte> mah = new List<byte>();
            //遍历所有的牌型，获取要处理的牌型
            for (int i = 0; i < usersCardsInfo[0].listCurrentCards.Count; i++)
            {
                if ((usersCardsInfo[0].listCurrentCards[i].cardNum / 16) == (value / 16))
                {
                    mah.Add(usersCardsInfo[0].listCurrentCards[i].cardNum);
                }
            }

            //TODO:十三幺
            return titles;
        }
        /// <summary>
        /// 断线重入数据传递
        /// </summary>
        /// <param name="seatNum">摸牌座位号</param>
        internal void TransCardsData(int seatNum)
        {
            isBreakConnect = true;
            isRecordScore_Break = true;

            Debug.LogWarning("断线重入数据传递");
            MainViewPlayerPlayingPanel ppp = UIMainView.Instance.PlayerPlayingPanel;



            //处理特殊拍
            for (int i = 0; i < NetMsg.MAX_USER_PER_TABLE; i++)
            {

                //碰、杠
                for (int j = 0; j < againLoginData.resultType[i].byTripletNum; j++)
                {
                    List<byte> lisb = new List<byte>();
                    byte value = (byte)(againLoginData.resultType[i].tripletType[j].byValue + againLoginData.resultType[i].tripletType[j].bySuit * 16);
                    lisb.Add(value);
                    lisb.Add(value);
                    lisb.Add(value);
                    if (againLoginData.resultType[i].tripletType[j].byPongKongType == 1)
                    {
                        lisb.Add(0);
                        UIMainView.Instance.PlayerPlayingPanel.SpwanSpecialCard(lisb.ToArray(), i + 1, 2, true, againLoginData.resultType[i].tripletType[j].bySeatNum);
                    }
                    else
                    {
                        lisb.Add(value);
                        UIMainView.Instance.PlayerPlayingPanel.SpwanSpecialCard(lisb.ToArray(), i + 1, 3, true, againLoginData.resultType[i].tripletType[j].bySeatNum, againLoginData.resultType[i].tripletType[j].byPongKongType - 1);//明暗12
                    }
                }
                //吃  顺子
                for (int j = 0; j < againLoginData.resultType[i].bySequenceNum; j++)
                {
                    List<byte> lisb = new List<byte>();

                    byte value1 = 0;
                    byte value2 = 0;
                    byte value3 = 0;

                    if (againLoginData.resultType[i].sequenceType[j].byFirstValue == againLoginData.resultType[i].sequenceType[j].byChowValue)
                    {
                        value1 = (byte)(againLoginData.resultType[i].sequenceType[j].byFirstValue + 1 + againLoginData.resultType[i].sequenceType[j].bySuit * 16);
                        value2 = (byte)(againLoginData.resultType[i].sequenceType[j].byFirstValue + 2 + againLoginData.resultType[i].sequenceType[j].bySuit * 16);
                        value3 = (byte)(againLoginData.resultType[i].sequenceType[j].byFirstValue + againLoginData.resultType[i].sequenceType[j].bySuit * 16);
                    }
                    else if ((againLoginData.resultType[i].sequenceType[j].byFirstValue + 1) == againLoginData.resultType[i].sequenceType[j].byChowValue)
                    {
                        value1 = (byte)(againLoginData.resultType[i].sequenceType[j].byFirstValue + againLoginData.resultType[i].sequenceType[j].bySuit * 16);
                        value2 = (byte)(againLoginData.resultType[i].sequenceType[j].byFirstValue + 2 + againLoginData.resultType[i].sequenceType[j].bySuit * 16);
                        value3 = (byte)(againLoginData.resultType[i].sequenceType[j].byFirstValue + 1 + againLoginData.resultType[i].sequenceType[j].bySuit * 16);
                    }
                    else if ((againLoginData.resultType[i].sequenceType[j].byFirstValue + 2) == againLoginData.resultType[i].sequenceType[j].byChowValue)
                    {
                        value1 = (byte)(againLoginData.resultType[i].sequenceType[j].byFirstValue + againLoginData.resultType[i].sequenceType[j].bySuit * 16);
                        value2 = (byte)(againLoginData.resultType[i].sequenceType[j].byFirstValue + 1 + againLoginData.resultType[i].sequenceType[j].bySuit * 16);
                        value3 = (byte)(againLoginData.resultType[i].sequenceType[j].byFirstValue + 2 + againLoginData.resultType[i].sequenceType[j].bySuit * 16);
                    }
                    lisb.Add(value1);
                    lisb.Add(value2);
                    lisb.Add(value3);
                    int pos = i;
                    if (i == 0)
                        pos = 4;
                    UIMainView.Instance.PlayerPlayingPanel.SpwanSpecialCard(lisb.ToArray(), i + 1, 1, true, pos);
                }
                //Debug.LogError("产生十三幺的牌型：" + againLoginData.resultType[i].byThirteenOrphansNum);
                for (int j = 0; j < againLoginData.resultType[i].byThirteenOrphansNum; j++)
                {
                    byte value = (byte)(againLoginData.resultType[i].thirteenOrphansType[j].byValue + againLoginData.resultType[i].thirteenOrphansType[j].bySuit * 16);
                    List<byte> lisb = new List<byte>();
                    lisb.Add(value); lisb.Add(0);
                    lisb.Add(0); lisb.Add(0);
                    UIMainView.Instance.PlayerPlayingPanel.SpwanSpecialCard(lisb.ToArray(), i + 1, (byte)(againLoginData.resultType[i].thirteenOrphansType[j].byType + 4), true, 5);
                }
            }
            for (int i = 0; i < NetMsg.MAX_USER_PER_TABLE; i++)
            {
                int index = GetOtherPlayerShowPos(i + 1) - 1;
                //赋值手牌
                int sumNum = againLoginData.resultType[i].byTripletNum * 3 + againLoginData.resultType[i].bySequenceNum * 3 + againLoginData.resultType[i].byThirteenOrphansNum + againLoginData.byaHandTileNum[i];
                //Debug.LogError("玩家手牌数量:" + againLoginData.byaHandTileNum[i]);
                for (int j = 0; j < againLoginData.byaHandTileNum[i]; j++)
                {
                    if (index == 0)//自己的
                    {
                        if ((j == againLoginData.byaHandTileNum[i] - 1) && sumNum == 14)
                        {
                            continue;
                        }
                        byte[] mah = new byte[1] { againLoginData.byaHandTiles[j] };
                        ppp.SpwanMahjong(mah, 0);//克隆自己手牌
                    }
                }

                //克隆tableCard
                for (int j = 0; j < againLoginData.byaDiscardTileNum[i]; j++)
                {
                    //Debug.LogWarning("玩家座位号++" + seatNum + " 第几张牌：" + j);
                    GameObject obj = ppp.SpwanTableCrad(i + 1, againLoginData.bya2DiscardTiles[i, j]);
                }
                //克隆其他玩家手牌
                if (index != 0)
                {

                    //Debug.LogWarning("克隆玩家" + (index + 1) + "手牌数量：" + againLoginData.byaHandTileNum[i]);
                    if ((i + 1) == seatNum)
                    {
                        ppp.SpwanOtherPlayerCrad(againLoginData.byaHandTileNum[i] - 1, index + 1);
                    }
                    else
                    {
                        ppp.SpwanOtherPlayerCrad(againLoginData.byaHandTileNum[i], index + 1);
                    }

                }
            }

            //CurrentCradSort(1);//排序
            ////处理最后一张摸牌
            //for (int i = 0; i < NetMsg.MAX_USER_PER_TABLE; i++)
            //{
            //    int index = GetOtherPlayerShowPos(i + 1) - 1;
            //    int sumNum = againLoginData.resultType[i].byTripletNum * 3 + againLoginData.resultType[i].bySequenceNum * 3 + againLoginData.resultType[i].byThirteenOrphansNum + againLoginData.byaHandTileNum[i];

            //    if (sumNum == 14 && index == 0)
            //    {
            //        byLastCard = againLoginData.byaHandTiles[againLoginData.byaHandTileNum[i] - 1];
            //        UIMainView.Instance.PlayerPlayingPanel.SpwanSelfPutCard_IE(byLastCard, 0);
            //        LeftCardCount_DisConnect += 1;
            //    }
            //}

            //if (GameData.Instance.PlayerPlayingPanelData.isChoiceTing == 2)
            //{
            //    UIMainView.Instance.PlayerPlayingPanel.ChangeHandCardStatus(0);
            //}

            //LeftCardCount = LeftCardCount_DisConnect;
            SystemMgr.Instance.PlayerPlayingSystem.UpdateShow();
        }

        public void DelayDiscardTileNotice(NetMsg.ClientDiscardTileNotice msg)
        {
            if (msg.bySeatNum <= 0)
            {
                return;
            }

            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            UIMainView.Instance.PlayerPlayingPanel.ShowWitchPlayerPlay(msg.bySeatNum - 1);
            //处理重新开始倒计时时间
            GameData.Instance.PlayerPlayingPanelData.DownTime = 0f;

            //处理自己摸得牌
            if (msg.bySeatNum == pppd.bySeatNum)
            {
                //把自己的手牌置为空
                MahjongManger.Instance.PlayerDealHandCrad = null;

                //对玩家手牌重新排序
                pppd.CurrentCradSort(2);

                //表示自己可出牌
                Debug.LogWarning("延迟====出牌通知消息,座位号：" + msg.bySeatNum + ",摸得牌：" + msg.byDrawTile + ",标志位：" + pppd.isFirstDealCard);
                pppd.bLastCard = msg.byDrawTile;

                if (msg.byDrawTile != 0)
                {

                    if (!pppd.isFirstDealCard)
                    {
                        //pppd.CurrentCradSort(true);//排序
                        Debug.LogWarning("出牌通知消息");
                        //处理自己出牌通知，延迟显示摸得手牌
                        UIMainView.Instance.PlayerPlayingPanel.SpwanSelfPutCard_IE(msg.byDrawTile, 0.5f);
                    }
                    else
                    {
                        pppd.FirstDealCard = msg.byDrawTile;
                    }

                }
                else
                {
                    if (!pppd.isRecordScore_Break)
                    {
                        //玩家碰杠之后，把最后一张牌分出间隙
                        UIMainView.Instance.PlayerPlayingPanel.MoveLastCard();
                    }

                    pppd.isCanHandCard = true;
                    if (!pppd.isGangToPeng)
                    {
                        Debug.LogWarning("判断碰杠");
                        //处理玩家摸到手牌之后，判断玩家的杠
                        if (MahjongHelper.Instance.JudgeGang(pppd.GetPlayerAllHandCard(2), pppd.usersCardsInfo[0].listSpecialCards) > 0)
                        {
                            pppd.isSendPlayerPass = false;
                            Debug.LogWarning("玩家有杠========================================");
                            //int[] specialValue = new int[] { 0, 0, 1, 0, 0, 0, 0, 1 };
                            MahjongHelper.Instance.specialValue_[2] = 1;
                            MahjongHelper.Instance.specialValue_[7] = 1;
                            UIMainView.Instance.PlayerPlayingPanel.ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);
                        }
                    }
                    else
                    {
                        pppd.isGangToPeng = false;
                    }

                    //在碰之后也要检测是否可以听牌                                        
                    MahjongHelper.Instance.mahjongTing = new Dictionary<byte, MahjongHelper.TingMessage[]>();
                    MahjongHelper.Instance.mahjongTing = MahjongHelper.Instance.GetEnableTingCard(2);
                    //Debug.LogError("听牌检查==========================延迟出牌:"+ MahjongHelper.Instance.mahjongTing.Count);
                    if (MahjongHelper.Instance.mahjongTing.Count > 0)
                    {
                        //显示所有可以听牌的花色值
                        UIMainView.Instance.PlayerPlayingPanel.UpdateTingCard(MahjongHelper.Instance.Ting.ToArray());
                    }
                }

                return;
            }
            //处理别人摸得牌
            else
            {

                if (msg.byDrawTile != 0)
                {
                    int index = pppd.GetOtherPlayerShowPos(msg.bySeatNum);
                    //产生别的玩家的手牌
                    UIMainView.Instance.PlayerPlayingPanel.SpwanOtherPlayerCrad(1, index);
                }
            }

            //关闭玩家的吃碰杠胡的界面显示 
            MahjongHelper.Instance.specialValue_ = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            UIMainView.Instance.PlayerPlayingPanel.ShowSpecialTileNoticeRemind(MahjongHelper.Instance.specialValue_);
        }


        /// <summary>
        /// 获取玩家的花牌信息
        /// </summary>
        /// <returns></returns>
        public byte[] GetPlayerFlowerCard()
        {
            List<byte> flower = new List<byte>();
            for (int i = 0; i < usersCardsInfo[0].listCurrentCards.Count; i++)
            {
                //Debug.LogError("玩家手中牌的花色值:" + usersCardsInfo[0].listCurrentCards[i].cardNum / 16 +
                //    ",value：" + usersCardsInfo[0].listCurrentCards[i].cardNum % 16);
                if (usersCardsInfo[0].listCurrentCards[i].cardNum > 96)
                {
                    flower.Add(usersCardsInfo[0].listCurrentCards[i].cardNum);
                }
            }
            return flower.ToArray();
        }

    }



    public class UserCardsInfo
    {

        public int inm;
        internal sbyte bySeatNum;
        internal List<ShowCard> listShowCards;
        internal List<CurrentCard> listCurrentCards;
        internal List<SpecialCard> listSpecialCards;
        internal List<byte> listTingCards;
        internal UserCardsInfo()
        {
            listCurrentCards = new List<CurrentCard>();
            listShowCards = new List<ShowCard>();
            listSpecialCards = new List<SpecialCard>();
        }
    }
    public class ShowCard
    {
        /// <summary>
        /// 牌种类
        /// </summary>
        public byte cardNum;
        public ShowCard()
        {

        }
        public ShowCard(byte num)
        {
            cardNum = num;
        }
    }
    public class CurrentCard
    {
        /// <summary>
        /// 麻将的唯一id
        /// </summary>
        public int MahId;
        /// <summary>
        /// 牌的花色值
        /// </summary>
        public byte cardNum;
        public CurrentCard()
        {

        }
        public CurrentCard(byte num)
        {
            cardNum = num;
        }
    }

    //特殊牌型的数据
    public class SpecialCard
    {
        //特殊牌的类型，1吃 2碰 3明杠 4胡 5吃（十三幺） 6抢（十三幺） 7暗杠
        public int type;
        //表示四种牌的值
        public byte[] mahValue = new byte[4];

        public SpecialCard(int type_, byte[] value)
        {
            type = type_;
            mahValue = value;
        }
    }


}
