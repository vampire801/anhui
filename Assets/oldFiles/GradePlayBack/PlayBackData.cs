using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using XLua;

namespace PlayBack_1
{
    [Hotfix]
    [LuaCallCSharp]
    public class PlayBackData
    {
        public static bool isComePlayBack; //是否进入到回放场景
        public int iPbVersion_Old = 0;  //老版本号
        public int iPbVersion_New = 1;  //新的版本

        public int iPlayBackVersion;  //播放版本号        
        public int iLeftCardNum = 136;  //当局所剩的牌的数量
        public int iCurrentCardNum;  //当前自己的手牌数量        
        public string iCurrentGameNum;  //当前的游戏局数
        public string sRoomId;  //当前房间的房间码
        public int iAllDealNum;  //总操作步骤的步数
        public int iDoneDealNum; //已经操作的步数
        public string sPlayBackNum; //回放码

        public int PutCardSeatNum; //保存上次出牌玩家的座位号
        public string[] sName = new string[4]; //四个玩家的昵称  按座位顺序保存
        public string[] sHeadUrl = new string[4]; //四个玩家的头像地址
        public int[] iUserId = new int[4];  //四个玩家的userid
        public int[] sex = new int[4];  //四个玩家的性别 
        public int[] iPoint = new int[4];  //四个玩家的得分
        public int[] iPointInit = new int[4]; //玩家的初始分数
        public int byDealerSeat; //庄家座位号
        public int byIfNormalOrDragon; ////武乡玩法平胡还是龙胡  0没操作1表示平胡2表示大胡
        public int byShootSeatReadHand;
        public uint[] iOpenRoomParam = new uint[MahjongLobby_AH.Network.Message.NetMsg.OpenRoomParamRow ];  //开房参数 
        public string[] sPlayBackMessageData; //玩家的详细步骤数据
        public uint[] iPlayMethodParam = new uint[MahjongLobby_AH.Network.Message.NetMsg.OpenRoomParamRow]; //该玩法的配置参数
        public int iMethodId;   //该玩法的玩法编号
        public byte[] SpecialNoticeType = new byte[7];  //吃碰杠胡的通知问题
        public int iChoiceTing = 0; //是否选择听的标志
        public bool isAlreadyShowTing;  //是否已经显示过听的标志位
        public int[] FlowerCpount; //花牌数量
        #region 玩家结算的数据保存
        //玩家的房间的玩法信息
        public MahjongGame_AH.Network.Message.NetMsg.PlayingMethodConfDef playingMethodConf = new MahjongGame_AH.Network.Message.NetMsg.PlayingMethodConfDef();
        public MahjongGame_AH.Network.Message.NetMsg.PlayingMethodConfDef_2 playingMethodConf_2 = new MahjongGame_AH.Network.Message.NetMsg.PlayingMethodConfDef_2();

        //保存用户手上的牌
        public byte[,] bHandleTiles = new byte[4, 14];

        //四个玩家的分数
        public int[] bResultPoint = new int[4];

        //四个玩家的结果信息
        public MahjongGame_AH.Network.Message.NetMsg.ResultTypeDef[] resultType = new MahjongGame_AH.Network.Message.NetMsg.ResultTypeDef[4];
        public MahjongGame_AH.Network.Message.NetMsg.ResultTypeDef_2[] resultType_2 = new MahjongGame_AH.Network.Message.NetMsg.ResultTypeDef_2[4];

        // 放炮用户座位号
        public byte byShootSeat;

        //所有用户胡牌的标志位
        public byte[] byaWinSrat = new byte[4];

        //玩家的番种类
        public sbyte[,] caFanResult = new sbyte[4, 6];

        #endregion



        public int ShowSeatNum;  //第一视角的玩家座位号 

        //玩家的特殊牌
        public class SpecialType
        {
            public byte byValue; //麻将的花色值
            public int type; //类型  1吃2碰3杠4胡5十三幺吃 6十三幺抢
        }

        //玩家手上所有的牌
        public class PlayerCardMessage
        {
            //玩家的所有手牌
            public List<byte> HandCard;
            //桌面上的牌
            public List<byte> TableCard;
            //玩家的特殊手牌
            public List<SpecialType> SpecialCard;

            public PlayerCardMessage()
            {
                HandCard = new List<byte>();
                TableCard = new List<byte>();
                SpecialCard = new List<SpecialType>();
            }

        }

        //四个玩家的所有的牌的信息   0--3对应1---4
        public PlayerCardMessage[] playerCardMessage = new PlayerCardMessage[4];

        /// <summary>
        /// 对当前玩家的手牌进行排序
        /// </summary>
        /// <param name="byvalue"></param>
        /// <returns></returns>
        public List<byte> SortToHandCard(List<byte> byvalue)
        {
            byvalue.Sort(iCompareList);
            return byvalue;
        }

        /// <summary>
        /// 玩家手牌的排序规则
        /// </summary>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        int iCompareList(byte b, byte a)
        {
            int res = 0;
            //获取麻将牌的花色和大小
            int a_0 = a / 16;
            int a_1 = a % 16;
            int b_0 = b / 16;
            int b_1 = b % 16;

            if (a_0 < b_0)
            {
                res = 1;
            }
            else if (a_0 == b_0)
            {
                if (a_1 <= b_1)
                {
                    res = 1;
                }
                else
                {
                    res = -1;
                }
            }
            else
            {
                res = -1;
            }

            return res;
        }

        /// <summary>
        /// 解析玩家的回放数据
        /// </summary>
        /// <param name="content"></param>
        public void DealPlayerNotice(string content)
        {
            string sTtile = content.Substring(0, 4);
            //去掉消息的编号
            string message = content.Substring(4);

            switch (sTtile)
            {
                case "0912":
                    //Debug.LogError("补花通知");
                    HandleClientAppliqueNotice(message);
                    break;
                case "1112":
                    //Debug.LogError("补花回应");
                    HandleClientAppliqueRes(message);
                    break;
                case "0412":
                    Debug.Log("能跑能下通知消息");
                    HandleCanDownOrEscapeNotice(message);
                    break;
                case "0612":
                    Debug.Log("能跑能下回应消息");
                    HandleCanDownOrEscapeRes(message);
                    break;
                case "1212":
                    PlayBackMahjongPanel.Instance.DelayOrderByPlayBack_Ie(0, iDoneDealNum);
                    break;
                case "0812":
                    Debug.Log("听牌回应消息");
                    HandleTingRes(message);
                    break;
                case "0011":
                    Debug.Log("发牌通知消息");
                    HandleDealTileNotice(message);
                    break;
                case "0111":
                    //Debug.Log("出牌通知消息");
                    HandleDiscardTileNotice(message);
                    break;
                case "0311":
                    //Debug.Log("出牌回应消息");
                    HandleDiscardTileRes(message);
                    break;
                case "0411":
                    //Debug.Log("处理玩家的吃碰杠胡的通知消息");
                    HandleSpecialTileNotice(message);
                    break;
                case "0611":
                    //Debug.Log("吃碰刚胡抢回应消息");
                    HandleSpecialTileRes_Delay(message);
                    break;
                case "0012":
                    //Debug.Log("一局结束通知消息");
                    HandleGameReultNotice(message);
                    break;
            }
        }

        /// <summary>
        /// 处理玩家补花通知
        /// </summary>
        /// <param name="message"></param>
        void HandleClientAppliqueNotice(string message)
        {
            PlayBackMahjongPanel.Instance.DelayOrderByPlayBack_Ie(0, iDoneDealNum);
        }


        /// <summary>
        ///  补花回应
        /// </summary>
        /// <param name="message"></param>
        void HandleClientAppliqueRes(string message)
        {
            //解析数据
            int seatNum = System.Convert.ToByte(message[0].ToString()) * 16 +
                System.Convert.ToByte(message[1].ToString());  //保存玩家座位号

            //保存玩家的补牌数量
            int byTileNum = (System.Convert.ToByte(message[2].ToString()) * 16 +
                System.Convert.ToByte(message[3].ToString()));

            List<byte> byaFlowerTile = new List<byte>();
            //保存玩家的花牌
            for (int i = 0; i < byTileNum * 2; i += 2)
            {
                byte temp = (byte)(System.Convert.ToByte(message[i + 4].ToString()) * 16 +
                    System.Convert.ToByte(message[i + 5].ToString()));
                byaFlowerTile.Add(temp);
            }



            //保存玩家的补牌
            List<byte> byaTileNum = new List<byte>();
            for (int i = 0; i < byTileNum * 2; i += 2)
            {
                byte temp = (byte)(System.Convert.ToByte(message[i + 4 + byTileNum * 2].ToString()) * 16 +
                    System.Convert.ToByte(message[i + 5 + byTileNum * 2].ToString()));
                byaTileNum.Add(temp);
            }

            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            int index_ = PlayBackMahjongPanel.Instance.GetPlayerIndex(seatNum);    //获取玩家所在位置的下标
            PlayBackMahjongPanel.Instance.SpwanSpeaiclTypeRemind(index_, seatNum, 16);  //产生吃碰杠胡 补花等 特殊提示

            int index = PlayBackMahjongPanel.Instance.GetPlayerIndex(seatNum);
            //更新花牌数量
            PlayBackMahjongPanel.Instance.UpdateFlower(index, byTileNum);

            //删除花牌，同时产生玩家新的补牌
            for (int i = 0; i < byaFlowerTile.Count; i++)
            {
                //删除玩家刚摸的那张手牌
                PlayBackMahjongPanel.Instance.DelPlayerHandleCard(byaFlowerTile[i], seatNum);
            }

            PlayBackMahjongPanel.Instance.SpwanHandCard(byaTileNum.ToArray(), seatNum, false);

            //重新排序
            PlayBackMahjongPanel.Instance.UpdatePlayerAllCard(seatNum);

            PlayBackMahjongPanel.Instance.DelayOrderByPlayBack_Ie(0.5f, iDoneDealNum);
        }

        /// <summary>
        /// 处理玩家能跑能下的通知消息
        /// </summary>
        /// <param name="message"></param>
        void HandleCanDownOrEscapeNotice(string message)
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            int iSeatNum; //座位号
            int iDownOrEscape; //庄闲跑或下的类型
            iSeatNum = System.Convert.ToByte(message[0].ToString()) * 16 + System.Convert.ToByte(message[1].ToString());
            iDownOrEscape = System.Convert.ToByte(message[2].ToString()) * 16 + System.Convert.ToByte(message[3].ToString());

            //如果等于展示的座位号
            if (iSeatNum == pbd.ShowSeatNum)
            {
                PlayBackMahjongPanel.Instance.ShowCanDownRu(iDownOrEscape);
            }

            PlayBackMahjongPanel.Instance.DelayOrderByPlayBack_Ie(0, iDoneDealNum);
        }

        /// <summary>
        /// 处理玩家能跑能下的回应消息
        /// </summary>
        /// <param name="message"></param>
        void HandleCanDownOrEscapeRes(string message)
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            int iSeatNum; //座位号
            int iDownOrEscape; //庄闲跑或下的类型
            iSeatNum = System.Convert.ToByte(message[0].ToString()) * 16 + System.Convert.ToByte(message[1].ToString());
            iDownOrEscape = System.Convert.ToByte(message[2].ToString()) * 16 + System.Convert.ToByte(message[3].ToString());

            Debug.LogError("座位号：" + iSeatNum + ",pbd.ShowSeatNum：" + pbd.ShowSeatNum);

            //如果等于展示的座位号
            if (iSeatNum == pbd.ShowSeatNum)
            {
                PlayBackMahjongPanel.Instance.ShowChoiceCanDownRu(iDownOrEscape);
                return;
            }

            PlayBackMahjongPanel.Instance.DelayOrderByPlayBack_Ie(0, iDoneDealNum);
        }

        /// <summary>
        /// 处理玩家的发牌通知信息
        /// </summary>
        /// <param name="message"></param>
        void HandleDealTileNotice(string message)
        {
            Debug.LogWarning (" 处理玩家的发牌通知信息");
            int index = 0;
            byte[,] bTileNotice = new byte[4, 13];

            for (int i = 0; i < 4; i++)
            {
                byte[] temp = new byte[13];
                string sTileNotice = message.Substring(index, 26);
                index += 26;
                //具体解析玩家的数据
                for (int j = 0; j < 13; j++)
                {
                    bTileNotice[i, j] = (byte)(System.Convert.ToByte(sTileNotice[j * 2].ToString()) * 16 + System.Convert.ToByte(sTileNotice[j * 2 + 1].ToString()));
                    temp[j] = bTileNotice[i, j];
                }

                //产生玩家的手牌
                PlayBackMahjongPanel.Instance.SpwanHandCard(temp, i + 1, false);

                PlayBackMahjongPanel.Instance.UpdatePlayerAllCard(i + 1);
            }

            PlayBackMahjongPanel.Instance.DelayOrderByPlayBack_Ie(1.6f, iDoneDealNum);
        }


        /// <summary>
        /// 处理出牌通知
        /// </summary>
        /// <param name="message"></param>
        void HandleDiscardTileNotice(string message)
        {
            int iSeatNum; //座位号
            int iDrawTile; //摸得牌，如果吃碰后没摸牌为0，如果是是别人为0xff
            int iDrawFromEnd;  //是否从牌墙最后摸得牌
            int iDrawSeatButtom; //是否是摸海底
            iSeatNum = System.Convert.ToByte(message[0].ToString()) * 16 + System.Convert.ToByte(message[1].ToString());
            iDrawTile = System.Convert.ToByte(message[2].ToString()) * 16 + System.Convert.ToByte(message[3].ToString());
            iDrawFromEnd = System.Convert.ToByte(message[4].ToString()) * 16 + System.Convert.ToByte(message[5].ToString());
            iDrawSeatButtom = System.Convert.ToByte(message[6].ToString()) * 16 + System.Convert.ToByte(message[7].ToString());

            PutCardSeatNum = iSeatNum;
            int index = PlayBackMahjongPanel.Instance.GetPlayerIndex(iSeatNum);
            if (iDrawTile != 0)
            {
                byte[] temp = new byte[] { (byte)iDrawTile };

                //产生玩家的刚摸的手牌
                PlayBackMahjongPanel.Instance.SpwanHandCard(temp, iSeatNum, true);
                PlayBackMahjongPanel.Instance.SpwanHandCard(temp, iSeatNum, false);
            }

            //显示出牌玩家
            PlayBackMahjongPanel.Instance.ShowWitchPlayerPlay(index);


            //进行下一步动画
            PlayBackMahjongPanel.Instance.DelayOrderByPlayBack_Ie(1f, iDoneDealNum);
        }


        /// <summary>
        /// 处理玩家的吃碰杠胡的通知消息
        /// </summary>
        /// <param name="message"></param>
        void HandleSpecialTileNotice(string message)
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            //为吃碰杠胡通知消息添加补位字段
            string content = message + "00";
            //获取通知的数据
            byte[] notice = GetByteToString(content, 8);

            int ioffset = 0;
            MahjongGame_AH.Network.Message.NetMsg.ClientSpecialTileNotice msg = new MahjongGame_AH.Network.Message.NetMsg.ClientSpecialTileNotice();
            ioffset = msg.parseBytes(notice, ioffset);

            //获取数据
            SpecialNoticeType[0] = msg.byThirteenOrphansChow;
            SpecialNoticeType[1] = msg.byThirteenOrphansRob;
            SpecialNoticeType[2] = msg.byPong;
            SpecialNoticeType[3] = msg.byKong;
            SpecialNoticeType[4] = msg.byWin;
            SpecialNoticeType[5] = 0;
            SpecialNoticeType[6] = 1;

            //显示玩家的吃碰杠胡提示信息                        
            PlayBackMahjongPanel.Instance.ShowSpecialTileNotice(msg.cSeatNum, SpecialNoticeType, 1);

            //如果玩家下步操作是继续显示其他玩家的碰杠操作
            if (string.Equals(pbd.sPlayBackMessageData[iDoneDealNum].Substring(0, 4), "0411"))
            {
                PlayBackMahjongPanel.Instance.DelayOrderByPlayBack_Ie(0, iDoneDealNum);
            }
            else
            {
                Debug.LogError("=======================");
                PlayBackMahjongPanel.Instance.DelayOrderByPlayBack_Ie(1f, iDoneDealNum);
            }
        }

        /// <summary>
        /// 处理玩家出牌回应
        /// </summary>
        /// <param name="message"></param>
        void HandleDiscardTileRes(string message)
        {
            //隐藏所有的特殊牌提示
            PlayBackMahjongPanel.Instance.CloseAllPlayerSpecialRemind();
            //Debug.LogError(" 处理玩家出牌回应");
            int iSeatNum; //座位号
            int iDrawTile; //出的牌
            iSeatNum = System.Convert.ToByte(message[0].ToString()) * 16 + System.Convert.ToByte(message[1].ToString());
            iDrawTile = System.Convert.ToByte(message[2].ToString()) * 16 + System.Convert.ToByte(message[3].ToString());

            //删除玩家刚摸的那张手牌
            PlayBackMahjongPanel.Instance.DelPlayerHandleCard(iDrawTile, iSeatNum);

            //重新排序
            PlayBackMahjongPanel.Instance.UpdatePlayerAllCard(iSeatNum);

            //产生对应的牌进行出牌动画
            PlayBackMahjongPanel.Instance.PlayPutAnimation(iDrawTile, iSeatNum);

            PlayBackMahjongPanel.Instance.DelayOrderByPlayBack_Ie(1f, iDoneDealNum);

            //播放出牌语音
            PlayBackAudioMgr.Instance.OnPlayAuto(PlayBackAudioMgr.Instance.GetAudioSourceIndex((byte)iDrawTile), sex[iSeatNum - 1]);

            //如果玩家的已经处于停牌状态
            if (iChoiceTing == 1 && !isAlreadyShowTing)
            {
                ShowTingLogo(iSeatNum);
            }
        }


        /// <summary>
        /// 把所有的牌显示成为停牌状态
        /// </summary>
        public void ShowTingLogo(int seatNum)
        {
            Mahjong[] mahjong = PlayBackMahjongPanel.Instance.GetPlayerAllHandCard(seatNum);
            for (int i = 0; i < mahjong.Length; i++)
            {
                mahjong[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 0.5f);
            }

            isAlreadyShowTing = true;
        }

        /// <summary>
        /// 玩家的听的回应消息
        /// </summary>
        /// <param name="message"></param>
        void HandleTingRes(string message)
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            int seatNum = System.Convert.ToByte(message[0].ToString()) * 16 + System.Convert.ToByte(message[1].ToString());
            int type = System.Convert.ToByte(message[2].ToString()) * 16 + System.Convert.ToByte(message[3].ToString());
            pbd.iChoiceTing = 1 - type;
            SpecialNoticeType[0] = 0;
            SpecialNoticeType[1] = 0;
            SpecialNoticeType[2] = 0;
            SpecialNoticeType[3] = 0;
            SpecialNoticeType[4] = 0;
            SpecialNoticeType[5] = 1;
            SpecialNoticeType[6] = 1;
            //显示玩家的吃碰杠胡提示信息                        
            PlayBackMahjongPanel.Instance.ShowSpecialTileNotice((byte)seatNum, SpecialNoticeType, 1);

            //0.5s之后显示播放动画
            PlayBackMahjongPanel.Instance.DelayShowPlayerClickAnimation_(seatNum, 5, message);
        }




        /// <summary>
        /// 显示玩家的吃碰杠胡的信息
        /// </summary>
        /// <param name="message"></param>
        void HandleSpecialTileRes_Delay(string message)
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            int seatNum = System.Convert.ToByte(message[0].ToString()) * 16 + System.Convert.ToByte(message[1].ToString());
            int type = System.Convert.ToByte(message[8].ToString()) * 16 + System.Convert.ToByte(message[9].ToString());
            int index = -1;

            //如果是杠 直接减去手牌数量
            if (type == 3)
            {
                if (pbd.playingMethodConf_2.byReserveMode > 0 || pbd.playingMethodConf.byReserveMode > 0)
                {
                    if (pbd.playingMethodConf.byReserveMode != 5)
                    {
                        pbd.iLeftCardNum -= 2;
                        PlayBackMahjongPanel.Instance.UpdatePrograssing();
                    }
                 }
            }

            if (type == 0)
            {
                index = 6;
            }
            else if (type == 2)
            {
                index = 2;
            }
            else if (type == 3)
            {
                index = 3;
            }
            else if (type == 4)
            {
                index = 4;
            }
            else if (type == 5 || type == 1)
            {
                index = 0;
            }
            else if (type == 6)
            {
                index = 1;
            }
            else if (type == 7)
            {
                index = 5;
            }


            PlayBackMahjongPanel.Instance.ShowPlayerClickAnimation(seatNum, index, message);
        }

        /// <summary>
        /// 处理玩家吃碰杠胡回应消息
        /// </summary>
        /// <param name="message"></param>
        public void HandleSpecialTileRes(string message)
        {
            PlayBackMahjongPanel.Instance.CloseAllPlayerSpecialRemind();
            int iSeatNum; //座位号
            byte[] iDrawTile = new byte[4];  //特殊牌
            int iSpecialType;  //吃碰杠胡抢类型
            int iPongKong;  //是否是碰杠
            int iTileSeat; //出牌座位号
            iSeatNum = System.Convert.ToByte(message[0].ToString()) * 16 + System.Convert.ToByte(message[1].ToString());
            for (int i = 1; i < 4; i++)
            {
                iDrawTile[i - 1] = (byte)(System.Convert.ToByte(message[i * 2].ToString()) * 16 + System.Convert.ToByte(message[i * 2 + 1].ToString()));
            }
            iSpecialType = System.Convert.ToByte(message[8].ToString()) * 16 + System.Convert.ToByte(message[9].ToString());
            iPongKong = System.Convert.ToByte(message[10].ToString()) * 16 + System.Convert.ToByte(message[11].ToString());
            iTileSeat = System.Convert.ToByte(message[12].ToString()) * 16 + System.Convert.ToByte(message[13].ToString());

            int index = PlayBackMahjongPanel.Instance.GetPlayerIndex(iSeatNum);
            int kongstatus = 0;
            if (iSpecialType == 3)
            {
                if (iPongKong == 1)
                {
                    kongstatus = 3;
                }
                else
                {
                    if (iTileSeat == 0)
                    {
                        kongstatus = 2;
                    }
                    else
                    {
                        kongstatus = 1;
                    }
                }
            }

            if (iTileSeat != 0)
            {
                //删除桌面上的牌
                PlayBackMahjongPanel.Instance.DelTabelCard();
            }


            //处理玩家的吃碰杠胡通知
            PlayBackMahjongPanel.Instance.SpwanSpecialCard(iDrawTile, iSeatNum, iSpecialType, kongstatus, iTileSeat);

            //如果不是杠，会间隔一段时间处理下个任务，如果杠直接处理下一个即时积分
            if (iSpecialType == 3)
            {
                PlayBackMahjongPanel.Instance.DelayOrderByPlayBack_Ie(0, iDoneDealNum);
            }
            else
            {
                PlayBackMahjongPanel.Instance.DelayOrderByPlayBack_Ie(1.1f, iDoneDealNum);
            }
        }

        /// <summary>
        /// 处理玩家即时计分通知消息
        /// </summary>
        /// <param name="message"></param>
        void HandleRealtimePointNotice(string message)
        {
            //Debug.LogError(" 处理玩家即时计分通知消息"+(message.Length % 4)+","+ message.Length);
            string addLeng = "";
            switch (message.Length % 4)
            {
                case 1: addLeng = "00"; break;
                case 2: addLeng = "0000"; break;
                case 3: addLeng = "000000"; break;
            }
            string content = message + addLeng;
            Debug.LogError("length:" + content.Length);
            byte[] notice = GetByteToString(content, 8);

            int ioffset = 0;
            MahjongGame_AH.Network.Message.NetMsg.ClientRealtimePointNotice msg = new MahjongGame_AH.Network.Message.NetMsg.ClientRealtimePointNotice();
            ioffset = msg.parseBytes(notice, ioffset);
            int[] point = new int[4];

            for (int i = 0; i < 4; i++)
            {

                int index = PlayBackMahjongPanel.Instance.GetPlayerIndex(i + 1);
                if (msg.caPoints[i] > 200)
                {
                    point[i] = (msg.caPoints[i] - 256);
                    iPoint[index] += (msg.caPoints[i] - 256);
                }
                else
                {
                    point[i] = msg.caPoints[i];
                    iPoint[index] += msg.caPoints[i];
                }

                Debug.LogError("即时积分的分数:" + msg.caPoints[i]);

                //处理玩家即时积分界面
                PlayBackMahjongPanel.Instance.SpwanSpeaiclScoreRemind(index, point[i]);
                PlayBackMahjongPanel.Instance.UpdatePanel();
            }

            PlayBackMahjongPanel.Instance.DelayOrderByPlayBack_Ie(1f, iDoneDealNum);
        }


        //一局结果通知消息
        void HandleGameReultNotice(string message)
        {
            Debug.LogWarning ("PlayBackData 一局结果通知消息");
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;

            //string addLeng = "";
            //switch ((4 - message.Length % 4))
            //{
            //    case 1: addLeng = "0"; break;
            //    case 2: addLeng = "00"; break;
            //    case 3: addLeng = "000"; break;
            //}
            string content = message;

            //保存获取的通知数据
            byte[] notice = GetByteToString(content, 8);

            int ioffset = 0;

            if (pbd.iPlayBackVersion == pbd.iPbVersion_New)
            {
                MahjongGame_AH.Network.Message.NetMsg.ClientGameResultNotice msg = new MahjongGame_AH.Network.Message.NetMsg.ClientGameResultNotice();
                ioffset = msg.parseBytes(notice, ioffset);
                //处理结果消息
                bool isHuangZhuang = true;  //是否是荒庄的标志位                

                if (msg.byDismiss != 0)
                {
                    isHuangZhuang = false;
                }

                for (int i = 0; i < 4; i++)
                {
                    if (msg.byaWinSeat[i] != 0)
                    {
                        isHuangZhuang = false;
                    }
                }

                //处理玩家荒庄
                if (isHuangZhuang)
                {
                    //产生hu荒庄的特效结果
                    PlayBackMahjongPanel.Instance.SpwanSpeaiclTypeRemind(4, 0, 9);
                    //延迟2s之后显示结算结果                
                    PlayBackMahjongPanel.Instance.DelayShowHuangZhuang(msg);
                }
                //处理玩家的胡牌，显示结果通知            
                else
                {
                    //Debug.LogError("玩家胡牌信息：=================================");
                    //直接显示玩家的结算结果
                    pbd.bHandleTiles = msg.bya2HandTiles;
                    pbd.bResultPoint = msg.caResultPoint;
                    pbd.resultType = msg.aResultType;
                    pbd.byShootSeat = msg.byShootSeat;
                    //pbd.byIfNormalOrDragon = msg.byIfNormalOrDragon;
                    pbd.byShootSeatReadHand = msg.byShootSeatReadHand;
                    pbd.byaWinSrat = msg.byaWinSeat;
                    pbd.caFanResult = msg.caFanResult;
                    for (int i = 0; i < pbd.iPoint.Length; i++)
                    {
                        pbd.iPoint[i] = msg.caResultPoint[i];
                    }
                    if (PlayBackMahjongPanel.Instance.DealHongZhongZhongma(msg.aResultType[0].byFanTiles))
                    {
                        PlayBackMahjongPanel.Instance.DelaySpwanSpeaiclTypeRemind(3f, msg.byaWinSeat);
                    }
                    else
                    {
                        PlayBackMahjongPanel.Instance.DelaySpwanSpeaiclTypeRemind(0, msg.byaWinSeat);
                    }
                }
            }
            else
            {
                MahjongGame_AH.Network.Message.NetMsg.ClientGameResultNotice_2 msg = new MahjongGame_AH.Network.Message.NetMsg.ClientGameResultNotice_2();
                ioffset = msg.parseBytes(notice, ioffset);
                //处理结果消息
                bool isHuangZhuang = true;  //是否是荒庄的标志位

                for (int i = 0; i < 4; i++)
                {
                    if (msg.byaWinSeat[i] != 0)
                    {
                        isHuangZhuang = false;
                    }
                }

                Debug.LogError("解散标志位:" + msg.byDismiss);
                if (msg.byDismiss != 0)
                {
                    isHuangZhuang = false;
                }
                //处理玩家荒庄
                if (isHuangZhuang)
                {
                    //产生荒庄的特效结果
                    PlayBackMahjongPanel.Instance.SpwanSpeaiclTypeRemind(4, ShowSeatNum, 9);
                    //延迟2s之后显示结算结果                
                    PlayBackMahjongPanel.Instance.DelayShowHuangZhuang_2(msg);
                }
                //处理玩家的胡牌，显示结果通知            
                else
                {
                    //直接显示玩家的结算结果
                    pbd.bHandleTiles = msg.bya2HandTiles;
                    for (int i = 0; i < 4; i++)
                    {
                        pbd.bResultPoint[i] = (int)msg.caResultPoint[i];
                    }
                    pbd.resultType_2 = msg.aResultType;
                    pbd.byShootSeat = msg.byShootSeat;
                    pbd.byaWinSrat = msg.byaWinSeat;
                    pbd.caFanResult = msg.caFanResult;
                    for (int i = 0; i < pbd.iPoint.Length; i++)
                    {
                        pbd.iPoint[i] = msg.caResultPoint[i];
                    }
                    if (PlayBackMahjongPanel.Instance.DealHongZhongZhongma(msg.aResultType[0].byFanTiles))
                    {
                        PlayBackMahjongPanel.Instance.DelaySpwanSpeaiclTypeRemind(3f,msg.byaWinSeat);
                    }
                    else
                    {
                        PlayBackMahjongPanel.Instance.DelaySpwanSpeaiclTypeRemind(0, msg.byaWinSeat);
                    }
                }
            }
        }

        //将字符解析成byte数组
        public byte[] GetByteToString(string content, int count)
        {
            //保存临时的数据
            List<byte> data = new List<byte>();

            //先添加8个头文件
            for (int i = 0; i < count; i++)
            {
                data.Add(0);
            }

            //解析具体的字符串
            for (int i = 0; i < content.Length; i += 2)
            {
                byte temp = (byte)(stringToInt(content[i].ToString()) * 16 + stringToInt(content[i + 1].ToString()));
                data.Add(temp);
            }

            //Debug.LogError("消息长度：" + data.Count);

            return data.ToArray();
        }


        //将字符串转换为整形
        int stringToInt(string temp)
        {
            if (temp == null)
            {
                return -1;
            }
            switch (temp)
            {
                case "a":
                    return 10;
                case "b":
                    return 11;
                case "c":
                    return 12;
                case "d":
                    return 13;
                case "e":
                    return 14;
                case "f":
                    return 15;
                default:
                    return System.Convert.ToInt32(temp);
            }
        }


        public void OpenPlayBackScene()
        {
            //先关闭预加载的游戏场景
            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(ESCENE.MAHJONG_GAME_GENERAL.ToString());


            if (scene != null)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
                UnityEngine.SceneManagement.SceneManager.LoadScene("GradePlayBack", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("GradePlayBack", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            }
        }


    }
}