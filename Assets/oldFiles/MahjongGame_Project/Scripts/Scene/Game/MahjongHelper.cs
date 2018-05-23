using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MahjongGame_AH.Data;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MahjongHelper : MonoBehaviour
    {

        #region 单例
        static MahjongHelper instace;
        public static MahjongHelper Instance
        {
            get
            {
                return instace;
            }
        }

        void Awake()
        {
            instace = this;
        }
        #endregion


        public const int SUIT_CHARACTER = 1;  //万牌
        public const int SUIT_DOT = 2;	// 筒牌
        public const int SUIT_BAMBOO = 3;	// 索牌
        public const int SUIT_WIND = 4;	// 风牌
        public const int SUIT_DRAGON = 5;	// 箭牌
        public const int SUIT_FLOWER = 6;   // 花牌

        public int byFanThirteenIndepend = 2;  //十三不靠的番数
        public int byFanThirteenOrphans = 2;  //十三幺的番数
        public int byFanLuxurySevenPairs = 3;  //豪华七对
        public int byFanSevenPairs = 2;  //普通七对

        public int iFanThirteenIndepend;  //十三幺的番数  小于0表示不糊十三幺  
        public byte iFanWin;  //胡牌番数

        public Dictionary<byte, TingMessage[]> mahjongTing = new Dictionary<byte, TingMessage[]>();  //打出可听的牌对应的胡牌信息
        public List<byte> ByteListTing = new List<byte>();   //打出可听的牌的信息
        public List<byte> Ting = new List<byte>();   //打出可听的牌的信息
        public List<TingMessage> TingCount = new List<TingMessage>();  //保存玩家可胡牌的信息
        /// <summary>
        /// 麻将的胡牌的信息
        /// </summary>
        public class TingMessage
        {
            public byte value;   //可以胡的牌的值
            public int count;  //胡牌之后的牌的番数
        }
        public int SortTingMessage(TingMessage b, TingMessage a)
        {
            int res = 0;
            if (anhui.MahjongCommonMethod.Instance.iPlayingMethod == 20001)//如果是红中麻将
            {
                if (a.value == 0x51 && b.value != 0x51)
                {
                    return 1;
                }
                else if (b.value == 0x51 && a.value != 0x51)
                {
                    return -1;
                }
            }
            if (a.value < b.value)
            {
                return 1;
            }
            else if (a.value > b.value)
            {
                return -1;
            }
            return res;
        }
        public int sortbyteTingCardslist(byte a, byte b)
        {
            int res = 0;
            if (anhui.MahjongCommonMethod.Instance.iPlayingMethod == 20001)//如果是红中麻将
            {
                if (a == 0x51 && b != 0x51)
                {
                    return 1;
                }
                else if (b == 0x51 && a != 0x51)
                {
                    return -1;
                }
            }
            if (a < b)
            {
                return 1;
            }
            else if (a > b)
            {
                return -1;
            }
            return res;
        }
        //保存玩家特殊牌的显示信息
        public int[] specialValue_ = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };


        /// <summary>
        /// 获取特殊胡牌番数
        /// </summary>
        public void GetSpecialTypeMultiple(MahjongGame_AH.Network.Message.NetMsg.PlayingMethodConfDef info)
        {
            byFanThirteenIndepend = info.byFanThirteenIndepend;
            byFanThirteenOrphans = info.byFanThirteenOrphans;
            byFanLuxurySevenPairs = info.byFanLuxurySevenPairs;
            byFanSevenPairs = info.byFanSevenPairs;
        }

        #region 杠的提示
        /// <summary>
        /// 在玩家起手牌之后，判断玩家是否有杠
        /// </summary>
        /// <param name="mahjongValue">表示玩家手里的牌，一维表示花色1万牌，2表示筒牌，3表示索牌，4表示风牌，5表示箭牌；二维表示1--9表示对应花色的牌的数量，0表示该花色的牌的总张数</param>
        /// <param name="mahjongValue_Hand">表示玩家摸得手牌</param>
        /// <param name="specialCard">表示特殊牌</param>
        /// <returns>0表示没有杠，1表示暗杠，2表示碰杠</returns>
        public int JudgeGang(byte[,] mahjongValue, List<SpecialCard> specialCard)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            pppd.bkongValue_ = new List<PlayerPlayingPanelData.bkong>();
            //for (int i = 0; i < 5; i++)
            //{
            //    Debug.LogError("value：" + mahjongValue[i, 0] + "," + mahjongValue[i, 1] + "," + mahjongValue[i, 2] + "," + mahjongValue[i, 3] + "," + mahjongValue[i, 4]
            //        + "," + mahjongValue[i, 5] + "," + mahjongValue[i, 6] + "," + mahjongValue[i, 7] + "," + mahjongValue[i, 8] + "," + mahjongValue[i, 9]);
            //}
           // Debug.LogError("pppd.isChoiceTing" + pppd.isChoiceTing + "!pppd.isAlreadyChangeStatus" + !pppd.isAlreadyChangeStatus);
            if (pppd.isChoiceTing >= 1 && !pppd.isAlreadyChangeStatus)
            {
                return 0;
            }

            int[] bySuit = new int[3];  //保存玩家的手里的特殊牌的花色
            int valueType = 0;  //花色的个数
            //如果是缺门逮pao，则优先检查  玩家的手里特殊牌的 是不是已经两门
            if (pppd.playingMethodConf.byWinLimitLack > 0)
            {
                for (int i = 0; i < pppd.usersCardsInfo[0].listSpecialCards.Count; i++)
                {
                    if (pppd.usersCardsInfo[0].listSpecialCards[i].mahValue[0] > 0)
                    {
                        int value = pppd.usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16;
                        if (value > 3)
                        {
                            continue;
                        }
                        bySuit[value - 1] = value;
                    }
                }
            }

            for (int i = 0; i < bySuit.Length; i++)
            {
                if (bySuit[i] > 0)
                {
                    valueType++;
                }
            }
           // Debug.LogError("pppd.isLastPongOrKong" + pppd.isLastPongOrKong);
            //如果刚才玩家有过特殊牌处理之后，不会进行碰杠的判断
            if (pppd.isLastPongOrKong)
            {
                return 0;
            }

            //可杠的特殊牌的数量
            int canSpecial = 0;
           // Debug.LogError("pppd.isChoicTir " + pppd.isChoicTir);
            //如果玩家成了十三幺之后，不会杠
            if (pppd.isChoicTir == 1)
            {
                return 0;
            }
            //判断碰杠
            for (int i = 0; i < specialCard.Count; i++)
            {
                if (specialCard[i].type == 2)
                {
                    for (int k = 1; k < 6; k++)
                    {
                        for (int j = 1; j < 10; j++)
                        {
                            //  Debug.LogError("判断碰杠:" + pppd.isChoiceTing + "," + pppd.bLastCard + "," + specialCard[i].mahValue[0] + "," + (k * 16 + j));
                            //Debug.LogError("重入测试" + MahjongCommonMethod.Instance.iPlayingMethod);
                            if (anhui.MahjongCommonMethod.Instance.iPlayingMethod ==20001)//红中麻将过杠不提示
                            {
                                if (pppd.bLastCard== specialCard[i].mahValue[0]&& mahjongValue[k - 1, j] > 0&& specialCard[i].mahValue[0] == k * 16 + j)//手牌有值并且牌值等于特殊牌
                                {
                                    pppd.bkongValue = specialCard[i].mahValue[0];
                                    PlayerPlayingPanelData.bkong kong = new PlayerPlayingPanelData.bkong();
                                    kong.kongType = 1;
                                    kong.kongValue = specialCard[i].mahValue[0];
                                    pppd.bkongValue_.Add(kong);
                                    canSpecial++;
                                }
                            }
                            else if(anhui.MahjongCommonMethod.Instance.iPlayingMethod!=0)
                            {
                                if (mahjongValue[k - 1, j] > 0 && specialCard[i].mahValue[0] == k * 16 + j)//手牌有值并且牌值等于特殊牌
                                {
                                    if (pppd.isChoiceTing != 0 && pppd.bLastCard != specialCard[i].mahValue[0])
                                    {
                                        continue;
                                    }
                                    pppd.bLastCard = specialCard[i].mahValue[0];
                                    pppd.bkongValue = specialCard[i].mahValue[0];

                                    PlayerPlayingPanelData.bkong kong = new PlayerPlayingPanelData.bkong();
                                    kong.kongType = 1;
                                    kong.kongValue = specialCard[i].mahValue[0];

                                    pppd.bkongValue_.Add(kong);
                                    canSpecial++;
                                }
                            }
                           // Debug.LogError(mahjongValue[k - 1, j]);
                           
                        }
                    }
                }
            }

            //判断暗杠  
            for (int i = 1; i < 6; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    if (mahjongValue[i - 1, j] == 4)
                    {
                        //Debug.LogError("判断暗杠:" + pppd.isChoiceTing);
                        if (pppd.isChoiceTing == 2)
                        {
                            //判断听牌之后，暗杠是否改变听口
                            if ((i * 16 + j) != MahjongManger.Instance.GetDealCard().bMahjongValue)
                            {
                                continue;
                            }
                            else
                            {
                                bool iscanGang = true;

                                for (int indrx = 0; indrx < Ting.Count; indrx++)
                                {
                                    //Debug.LogError("听的牌：" + Ting[indrx] + "，手牌：" + ((i * 16 + j)));
                                    if ((i * 16 + j) == Ting[indrx])
                                    {
                                        iscanGang = false;
                                        break;
                                    }
                                }

                                //for (int k = 0; k < resultType.sequenceType.Count; k++)
                                //{
                                //    if (i == resultType.sequenceType[k].bySuit && (j == resultType.sequenceType[k].byFirstValue ||
                                //        j == resultType.sequenceType[k].byFirstValue + 1 ||
                                //        j == resultType.sequenceType[k].byFirstValue + 2))
                                //    {
                                //        iscanGang = false;
                                //        break;
                                //    }
                                //}

                                if (!iscanGang)
                                {
                                    continue;
                                }
                            }
                        }
                        pppd.bkongValue = (byte)(i * 16 + j);
                        pppd.bAnkongStatus = 1;
                        PlayerPlayingPanelData.bkong kong = new PlayerPlayingPanelData.bkong();
                        kong.kongType = 0;
                        kong.kongValue = (byte)(i * 16 + j);
                        pppd.bkongValue_.Add(kong);
                        canSpecial++;
                    }
                }
            }

            if (valueType >= 2)
            {
                for (int i = 0; i < pppd.bkongValue_.Count; i++)
                {
                    if (pppd.bkongValue_[i].kongValue / 16 > 3)
                    {
                        continue;
                    }
                    else
                    {
                        if (pppd.bkongValue_[i].kongValue / 16 != bySuit[0] && pppd.bkongValue_[i].kongValue / 16 != bySuit[1]
                       && pppd.bkongValue_[i].kongValue / 16 != bySuit[2])
                        {
                            pppd.bkongValue_.RemoveAt(i);
                        }
                    }
                }
            }

            if (pppd.bkongValue_.Count <= 0)
            {
                return 0;
            }
            else
            {
                pppd.isNeedSendPassMessage = false;
            }

            return canSpecial;
        }

        #endregion


        #region 听牌提示      

        /// <summary>
        /// 获取哪些牌打出之后，可以听牌
        /// </summary>
        /// <param name="status">1表示不考虑听牌，2表示考虑听牌</param>
        /// <returns></returns>
        public Dictionary<byte, TingMessage[]> GetEnableTingCard(int status)
        {
            Ting = new List<byte>();
            Dictionary<byte, TingMessage[]> value = new Dictionary<byte, TingMessage[]>();
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            //如果玩家已经选择了听牌模式，则不在检查听牌
            if (pppd.isChoiceTing == 2 && status == 2)
            {
                return value;
            }

            byte[,] mahjongValue = new byte[5, 10];
            mahjongValue = pppd.GetPlayerAllHandCard(1);


            //for (int i = 0; i < 5; i++)
            //{
            //    Debug.LogError("=============" + mahjongValue[i, 0] + "," + mahjongValue[i, 1] + "," + mahjongValue[i, 2] + "," + mahjongValue[i, 3] + "," + mahjongValue[i, 4]
            //        + "," + mahjongValue[i, 5] + "," + mahjongValue[i, 6] + "," + mahjongValue[i, 7] + "," + mahjongValue[i, 8] + "," + mahjongValue[i, 9]);
            //}

            //如果玩家的牌不满足33332，直接return
            int count = 0;
            for (int i = 0; i < 5; i++)
            {
                count += mahjongValue[i, 0];
            }

            //Debug.LogError("count:" + count);

            if (count % 3 != 2)
            {
                return value;
            }

            //遍历所有手牌，打出之后是否可以听牌
            for (int i = 0; i < 5; i++)
            {
                if (mahjongValue[i, 0] == 0)
                {
                    continue;
                }
                for (int j = 1; j < 10; j++)
                {
                    if (mahjongValue[i, j] > 0)
                    {
                        List<byte> mahjong_ = new List<byte>();
                        byte mah = (byte)((i + 1) * 16 + j);

                        for (int k = 0; k < pppd.usersCardsInfo[0].listCurrentCards.Count; k++)
                        {
                            mahjong_.Add(pppd.usersCardsInfo[0].listCurrentCards[k].cardNum);
                        }

                        if (pppd.isChoicTir == 1)
                        {
                            for (int k = 0; k < pppd.usersCardsInfo[0].listSpecialCards.Count; k++)
                            {
                                mahjong_.Add(pppd.usersCardsInfo[0].listSpecialCards[k].mahValue[0]);
                            }
                        }

                        for (int l = 0; l < mahjong_.Count; l++)
                        {
                            if (mahjong_[l] == mah)
                            {
                                mahjong_.RemoveAt(l);
                                break;
                            }
                        }

                        mahjong_.Add(0);
                        TingMessage[] mah_me = GetTingRemind(mahjong_.ToArray());

                        if (mah_me.Length <= 0)
                        {
                            continue;
                        }

                        if (mah_me.Length > 0)
                        {
                            if (value.ContainsKey(mah))
                            {
                                value[mah] = mah_me;
                            }
                            else
                            {
                                value.Add(mah, mah_me);
                                Ting.Add(mah);
                            }
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// 判断玩家打出这张牌之后，是否可以听牌,以及所有能胡牌的花色
        /// </summary>
        /// <param name="mahjongValue"></param>
        /// <returns></returns>
        public TingMessage[] GetTingRemind_Add(byte[,] mahjongValue)
        {
            byte[] value = new byte[34] { 17, 18, 19, 20, 21, 22, 23, 24, 25, 33, 34, 35, 36, 37, 38, 39, 40, 41, 49, 50, 51, 52, 53, 54, 55, 56, 57, 65, 66, 67, 68, 81, 82, 83 };
            List<TingMessage> card = new List<TingMessage>();
            // string valueType = "";
            //遍历所有牌型，添加进去之后，判断是否符合胡牌标准
            for (int i = 0; i < 34; i++)
            {
                byte bysuit = (byte)(value[i] / 16);
                byte byvalue = (byte)(value[i] % 16);
                mahjongValue[bysuit - 1, byvalue]++;
                mahjongValue[bysuit - 1, 0]++;
                //Debug.LogError("=====================");
                //for (int k = 0; k < 5; k++)
                //{
                //    Debug.LogError("value：" + mahjongValue[k, 0] + "," + mahjongValue[k, 1] + "," + mahjongValue[k, 2] + "," + mahjongValue[k, 3] + "," + mahjongValue[k, 4]
                //        + "," + mahjongValue[k, 5] + "," + mahjongValue[k, 6] + "," + mahjongValue[k, 7] + "," + mahjongValue[k, 8] + "," + mahjongValue[k, 9]);
                //}
                TingMessage message = new TingMessage();
                if (JudgeWin(mahjongValue) > 0)
                {
                    // Debug.Log("++" + value[i].ToString("X2"));
                    message.value = value[i];
                    message.count = iFanWin;
                    card.Add(message);
                    // valueType += "_" + value[i].ToString("X2");
                }
                mahjongValue[bysuit - 1, byvalue]--;
                mahjongValue[bysuit - 1, 0]--;

            }
            //Debug.LogWarning("===========\n" + valueType);
            return card.ToArray();
        }


        /// <summary>
        /// 判断玩家打出这张牌之后，是否可以听牌,以及所有能胡牌的花色
        /// </summary>
        /// <param name="mahjongValue"></param>
        /// <returns></returns>
        public TingMessage[] GetTingRemind(byte[] mahjongValue)
        {
            byte[] value = new byte[34] { 17, 18, 19, 20, 21, 22, 23, 24, 25, 33, 34, 35, 36, 37, 38, 39, 40, 41, 49, 50, 51, 52, 53, 54, 55, 56, 57, 65, 66, 67, 68, 81, 82, 83 };

            List<TingMessage> card = new List<TingMessage>();

            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            //遍历所有牌型，添加进去之后，判断是否符合胡牌标准
            for (int i = 0; i < 34; i++)
            {
                mahjongValue[mahjongValue.Length - 1] = value[i];

                byte[,] mahjong = new byte[5, 10];

                byte bysuit = 0;
                byte byvalue = 0;
                for (int k = 0; k < mahjongValue.Length; k++)
                {
                    bysuit = (byte)(mahjongValue[k] / 16);
                    byvalue = (byte)(mahjongValue[k] % 16);
                    if (bysuit > 5)
                    {
                        continue;
                    }
                    mahjong[bysuit - 1, 0]++;
                    mahjong[bysuit - 1, byvalue]++;
                }

                //for (int k = 0; k < 5; k++)
                //{
                //    Debug.LogError("=============" + mahjong[k, 0] + "," + mahjong[k, 1] + "," + mahjong[k, 2] + "," + mahjong[k, 3] + "," + mahjong[k, 4]
                //        + "," + mahjong[k, 5] + "," + mahjong[k, 6] + "," + mahjong[k, 7] + "," + mahjong[k, 8] + "," + mahjong[k, 9]);
                //}

                //Debug.LogError("=======================================================");

                TingMessage message = new TingMessage();
                if (JudgeWin(mahjong) > 0)
                {
                    message.value = value[i];
                    message.count = iFanWin;
                    card.Add(message);
                }
            }

            return card.ToArray();
        }

        #endregion
        //顺子信息
        public class SequenceTypeDef
        {
            public byte bySuit; // 牌类型：1万牌，2筒牌，3索牌
            public byte byFirstValue; // 顺子的第一个值                        
        }

        //刻子信息
        public class TripletTypeDef
        {
            public byte bySuit; // 牌类型：1万牌，2筒牌，3索牌，4风牌，5箭牌            
            public byte byValue; // 牌值                                               
        }

        //玩家手里的牌的信息
        public class ResultType
        {
            public List<SequenceTypeDef> sequenceType = new List<SequenceTypeDef>(); // 顺子牌
            public List<TripletTypeDef> tripletType = new List<TripletTypeDef>(); // 刻子牌
        }

        //保存玩家的手牌信息
        public ResultType resultType = new ResultType();

        /// <summary>
        /// 分析一种花色的牌，分解为刻子和顺子的组合
        /// </summary>
        /// <param name="byaSuitTileNum">一种花色的牌的各个值的张数</param>
        /// <param name="bHonorTile">是否是字牌（风牌和箭牌）</param>
        /// <param name="bySuit">花色：1万牌、2筒牌、3索牌、4风牌、5箭牌</param>
        /// <param name="isSaveCardMessage">是否保存玩家的顺子刻子信息</param>
        /// <returns></returns>
        bool AnalyzeSuit(byte[] byaSuitTileNum, bool bHonorTile, byte bySuit, bool isSaveCardMessage = false)
        {
            //if (bySuit == 1)
            //{
            //    Debug.LogError("wwwvalue:" + byaSuitTileNum[0] + "," + byaSuitTileNum[1] + "," + byaSuitTileNum[2] + "," + byaSuitTileNum[3] + "," + byaSuitTileNum[4] + "," + byaSuitTileNum[5] + "," + byaSuitTileNum[6]
            //        + "," + byaSuitTileNum[7] + "," + byaSuitTileNum[8] + "," + byaSuitTileNum[9]);
            //}
            if (bySuit > 3)
            {
                bHonorTile = true;
            }


            if (byaSuitTileNum[0] == 0)
            {
                return true;
            }

            //寻找最小的值
            byte byValue = 0;

            for (byValue = 1; byValue < 10; byValue++)
            {
                if (byaSuitTileNum[byValue] > 0)
                {
                    break;
                }
            }

            //能否分析成功
            bool bSuccess = false;

            //分析刻子
            if (byaSuitTileNum[byValue] > 2)
            {
                //出去刻子的3张牌
                byaSuitTileNum[byValue] -= 3;
                byaSuitTileNum[0] -= 3;

                if (isSaveCardMessage)
                {
                    //添加玩家的刻子信息
                    TripletTypeDef seq = new TripletTypeDef();
                    seq.bySuit = bySuit;
                    seq.byValue = byValue;
                    resultType.tripletType.Add(seq);
                }

                //分析剩余的牌
                bSuccess = AnalyzeSuit(byaSuitTileNum, bHonorTile, bySuit, isSaveCardMessage);

                //还原刻子的三张牌
                byaSuitTileNum[byValue] += 3;
                byaSuitTileNum[0] += 3;

                return bSuccess;
            }

            //分析顺子
            if (!bHonorTile && byValue < 8 && (byaSuitTileNum[byValue + 1] > 0) && (byaSuitTileNum[byValue + 2] > 0))
            {
                //是否限制3以下序数牌成牌,0不限值，1限制
                //if (GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byUnderThreeLimit == 1)
                //{
                //    if (byValue < 4)
                //    {
                //        return false;
                //    }
                //}

                //出去三张顺子的3张牌
                byaSuitTileNum[byValue]--;
                byaSuitTileNum[byValue + 1]--;
                byaSuitTileNum[byValue + 2]--;
                byaSuitTileNum[0] -= 3;


                //分析剩余的牌
                bSuccess = AnalyzeSuit(byaSuitTileNum, bHonorTile, bySuit, isSaveCardMessage);

                if (isSaveCardMessage)
                {
                    //添加玩家的顺子信息,如果列表已经包含该顺子信息，则不会继续添加   
                    SequenceTypeDef seq = new SequenceTypeDef();
                    seq.bySuit = bySuit;
                    seq.byFirstValue = byValue;
                    resultType.sequenceType.Add(seq);
                }

                //还原顺子的3张牌
                byaSuitTileNum[byValue]++;
                byaSuitTileNum[byValue + 1]++;
                byaSuitTileNum[byValue + 2]++;
                byaSuitTileNum[0] += 3;
                return bSuccess;
            }

            return false;
        }


        /// <summary>
        /// 判断是否普通胡牌
        /// </summary>
        /// <param name="byaTileNum">各种花色的牌各个值的张数，一维的5代表花色；二维的10的0位置保存这个花色的总张数，1～9位置保存对应值的牌张数</param>
        /// <returns></returns>
        public bool JudgeNormalWin(byte[,] byaTileNum)
        {
            //Debug.LogError("GameData.Instance.PlayerPlayingPanelData.isChoicTir：" + GameData.Instance.PlayerPlayingPanelData.isChoicTir
            //    + ",GameData.Instance.PlayerPlayingPanelData.iMethodId：" + GameData.Instance.PlayerPlayingPanelData.iMethodId);

            //for (int i = 0; i < 5; i++)
            //{
            //    Debug.LogError(byaTileNum[i, 0] + "," + byaTileNum[i, 1] + "," + byaTileNum[i, 2] + "," + byaTileNum[i, 3] + "," + byaTileNum[i, 4]
            //        + "," + byaTileNum[i, 5] + "," + byaTileNum[i, 6] + "," + byaTileNum[i, 7] + "," + byaTileNum[i, 8] + "," + byaTileNum[i, 9]);
            //}
            //Debug.LogError("==========================================================================");
            if (GameData.Instance.PlayerPlayingPanelData.isChoicTir == 1 || GameData.Instance.PlayerPlayingPanelData.iMethodId == 0)
            {
                return false;
            }

            resultType = new ResultType();

            byte byJongSuit = 0;   //将牌的花色
            bool bJongExist = false;  //将是否有了
            byte byRemainder = 0;  //余数
            byte bySuit = 0;

            //是否满足33332的模型
            for (bySuit = SUIT_CHARACTER; bySuit < SUIT_FLOWER; bySuit++)  //5中花色
            {
                byRemainder = (byte)(byaTileNum[bySuit - 1, 0] % 3);
                if (byRemainder == 1)
                {
                    return false;
                }
                else if (byRemainder == 2)
                {
                    if (bJongExist)
                    {
                        return false;
                    }
                    byJongSuit = bySuit;
                    bJongExist = true;
                }
            }

            //分析不包含将牌的花色
            for (bySuit = 1; bySuit < 6; bySuit++)
            {
                byte[] byaTileNum_1 = new byte[10];
                for (int i = 0; i < 10; i++)
                {
                    byaTileNum_1[i] = byaTileNum[bySuit - 1, i];
                }

                if (bySuit != byJongSuit)
                {
                    if (!AnalyzeSuit(byaTileNum_1, bySuit == SUIT_WIND || bySuit == SUIT_DRAGON, bySuit))
                    {
                        return false;
                    }
                }
            }

            //分析包含将牌的花色
            bool bSuccess = false;  //除去将牌后分析是否成功
            for (byte byValue = 1; byValue < 10; byValue++)
            {
                if (byaTileNum[byJongSuit - 1, byValue] >= 2)
                {
                    //除去2张将牌                                     
                    byaTileNum[byJongSuit - 1, byValue] -= 2;
                    byaTileNum[byJongSuit - 1, 0] -= 2;

                    byte[] byaTileNum_2 = new byte[10];
                    for (int i = 0; i < 10; i++)
                    {
                        byaTileNum_2[i] = byaTileNum[byJongSuit - 1, i];
                    }

                    //分析剩下的牌
                    if (AnalyzeSuit(byaTileNum_2, bySuit == SUIT_WIND || bySuit == SUIT_DRAGON, byJongSuit))
                    {
                        bSuccess = true;
                    }

                    //还原2张将牌
                    byaTileNum[byJongSuit - 1, byValue] += 2;
                    byaTileNum[byJongSuit - 1, 0] += 2;
                }
            }


            //判断沁源硬三百是否满足胡牌条件
            if (bSuccess)
            {
                iFanWin = 1;
                //解析玩家所有的手牌  获取所有的顺子 刻子信息
                for (int i = 0; i < 5; i++)
                {
                    byte[] temp = new byte[10];
                    for (int j = 0; j < 10; j++)
                    {
                        temp[j] = byaTileNum[i, j];
                    }
                    AnalyzeSuit(temp, (i + 1) == SUIT_WIND || (i + 1) == SUIT_DRAGON, (byte)(i + 1), true);
                }

                //Debug.LogError("顺子数量:" + resultType.sequenceType.Count);
                ////打印刻子信息
                //for (int i = 0; i < resultType.sequenceType.Count; i++)
                //{
                //    Debug.LogError("顺子信息" + resultType.sequenceType[i].bySuit + ",:" + resultType.sequenceType[i].byFirstValue);
                //}

                //Debug.LogError("刻子数量:" + resultType.tripletType.Count);
                //for (int i = 0; i < resultType.tripletType.Count; i++)
                //{
                //    Debug.LogError("刻子信息" + resultType.tripletType[i].bySuit + ",:" + resultType.tripletType[i].byValue);
                //}

                if (GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byWinLimitBeginFan > 1)
                {
                    if (JudgeWinMultiple(byaTileNum) >= 300)
                    {
                        return bSuccess;
                    }
                    else
                    {
                        return false;
                    }
                }

                //如果是黎城翻二  玩法id13
                if (GameData.Instance.PlayerPlayingPanelData.iMethodId == 13)
                {
                    for (int i = 0; i < resultType.sequenceType.Count; i++)
                    {
                        if (resultType.sequenceType[i].byFirstValue < 4)
                        {
                            return false;
                        }
                    }
                }
            }
            return bSuccess;
        }

        /// <summary>
        /// 玩家对应牌的胡牌番数
        /// </summary>
        /// <param name="byaTileNum"></param>
        /// <returns></returns>
        public int JudgeWinMultiple(byte[,] byaTileNum)
        {
            int Multiple = 0;  //胡牌番数
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            bool isMenqing = true;
            int count_ddh = 0;  //对对胡的数量
            //门清100番（不吃不碰不明杠）
            for (int i = 0; i < pppd.usersCardsInfo[0].listSpecialCards.Count; i++)
            {
                if (pppd.usersCardsInfo[0].listSpecialCards[i].type < 7)
                {
                    if (pppd.usersCardsInfo[0].listSpecialCards[i].type == 2 || pppd.usersCardsInfo[0].listSpecialCards[i].type == 3)
                    {
                        count_ddh++;
                    }
                    isMenqing = false;
                }

                if (pppd.usersCardsInfo[0].listSpecialCards[i].type == 7)
                {
                    count_ddh++;
                }
            }

            if (isMenqing)
            {
                //Debug.LogError("门清100番");
                Multiple += 100;
            }


            if (resultType.sequenceType.Count >= 2)
            {
                //姐妹100番（或叫亲三即一色牌中如二三四二三四组成西塔牌）
                bool isSister = false;
                for (int i = 0; i < resultType.sequenceType.Count; i++)
                {
                    //Debug.LogError("顺子的信息:" + resultType.sequenceType[i].bySuit + ",值：" + resultType.sequenceType[i].byFirstValue);
                    for (int j = i + 1; j < resultType.sequenceType.Count; j++)
                    {
                        if (resultType.sequenceType[i].bySuit == resultType.sequenceType[j].bySuit && resultType.sequenceType[i].byFirstValue == resultType.sequenceType[j].byFirstValue)
                        {
                            Multiple += 100;
                            isSister = true;
                            //Debug.LogError("姐妹100番:");
                            break;
                        }
                    }
                    if (isSister)
                    {
                        break;
                    }
                }


                bool Laosho = false;
                //老少                
                for (int i = 0; i < resultType.sequenceType.Count; i++)
                {
                    byte temp = 0;
                    byte value = 0;
                    if (resultType.sequenceType[i].byFirstValue == 1 || resultType.sequenceType[i].byFirstValue == 7)
                    {
                        value = resultType.sequenceType[i].byFirstValue;
                        temp = resultType.sequenceType[i].bySuit;
                        for (int j = 0; j < resultType.sequenceType.Count; j++)
                        {
                            if (resultType.sequenceType[j].bySuit == temp && resultType.sequenceType[j].byFirstValue == 8 - value)
                            {
                                Multiple += 100;
                                //Debug.LogError("老少 100番");
                                Laosho = true;
                                break;
                            }
                        }
                    }
                    if (Laosho)
                    {
                        break;
                    }
                }
            }

            //断玄（没有一、九牌、风牌、红中白板发财）
            int iCardNum = 0;  //牌的数量
            for (int i = 0; i < 4; i++)
            {
                if (i == 3)
                {
                    iCardNum += byaTileNum[i, 0];
                }
                else
                {
                    iCardNum += byaTileNum[i, 1];
                    iCardNum += byaTileNum[i, 9];
                }
            }

            //判断特殊牌
            for (int i = 0; i < pppd.usersCardsInfo[0].listSpecialCards.Count; i++)
            {
                if (pppd.usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16 < 4)
                {
                    if (pppd.usersCardsInfo[0].listSpecialCards[i].mahValue[0] % 16 == 1 ||
                        pppd.usersCardsInfo[0].listSpecialCards[i].mahValue[0] % 16 == 9)
                    {
                        iCardNum++;
                    }
                }
                else
                {
                    iCardNum++;
                }
            }

            if (iCardNum <= 0)
            {
                Multiple += 100;
                //Debug.LogError("断玄 100番");
            }

            //缺一门
            if (JudgeLackSuit(byaTileNum, 2))
            {
                Multiple += 100;
                //Debug.LogError("缺一门 100番");
            }

            //一条龙（一色牌中一二三四五六七八九组成三搭牌）
            bool isFlag = true;
            for (int i = 0; i < 3; i++)
            {
                if (byaTileNum[i, 0] >= 9)
                {
                    for (int j = 1; j < 10; j++)
                    {
                        if (byaTileNum[i, j] <= 0)
                        {
                            isFlag = false;
                        }
                    }
                    if (isFlag)
                    {
                        Multiple += 300;
                        //Debug.LogError("一条龙 300番");
                        break;
                    }
                }
            }


            //七小对
            if (JudgeSevenPairs(byaTileNum) > 0)
            {
                Multiple += 700;
                //Debug.LogError("七小对 700番");
            }

            //对对胡  
            if (count_ddh + resultType.tripletType.Count >= 4 && resultType.sequenceType.Count == 0)
            {
                Multiple += 100;
                //Debug.LogError("对对胡 100番");
            }


            //清一色            
            int[] iCardNum_ = new int[3];
            //添加特殊牌的数量            
            for (int i = 0; i < pppd.usersCardsInfo[0].listSpecialCards.Count; i++)
            {
                if (pppd.usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16 < 4)
                {
                    int count_ = 0;
                    if (pppd.usersCardsInfo[0].listSpecialCards[i].type == 1 || pppd.usersCardsInfo[0].listSpecialCards[i].type == 2)
                    {
                        count_ = 3;
                    }
                    else if (pppd.usersCardsInfo[0].listSpecialCards[i].type == 3)
                    {
                        count_ = 4;
                    }
                    else
                    {
                        count_ = 1;
                    }
                    iCardNum_[pppd.usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16 - 1] += count_;
                }
            }

            //花色的数量
            int valueCount = 0;
            //添加手牌的牌的张数
            for (int i = 0; i < 3; i++)
            {
                if (byaTileNum[i, 0] > 0 || iCardNum_[i] > 0)
                {
                    valueCount++;
                }
            }

            if (valueCount == 1)
            {
                Multiple += 700;
                //Debug.LogError("清一色 700番");
            }


            //混一色
            int[] num = new int[3];
            //手牌
            for (int i = 0; i < 3; i++)
            {
                if (byaTileNum[i, 0] > 0)
                {
                    num[i] = 1;
                }
            }

            //检查玩家特殊得牌
            if (pppd.usersCardsInfo[0].listSpecialCards.Count > 0)
            {
                for (int i = 0; i < pppd.usersCardsInfo[0].listSpecialCards.Count; i++)
                {
                    if ((pppd.usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16) < 4)
                    {
                        num[pppd.usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16 - 1] = 1;
                    }
                }
            }

            int count = 0;
            for (int i = 0; i < num.Length; i++)
            {
                if (num[i] == 1)
                {
                    count++;
                }
            }

            if (count == 1 && byaTileNum[3, 0] > 0)
            {
                Multiple += 200;
                //Debug.LogError("混一色 200番");
            }

            //添加令风番
            Multiple += JudgeWindMul();

            //Debug.LogError("番数:" + Multiple);
            iFanWin = (byte)(Multiple / 100);
            return Multiple;
        }

        /// <summary>
        /// 判断玩家的令风番
        /// </summary>
        int JudgeWindMul()
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int Mul_Wind = 0;
            //获取玩家所有手牌的花色
            byte[,] mah_hand = pppd.GetPlayerAllHandCard(3);
            //如果自己是庄家
            if (pppd.bySeatNum == pppd.byDealerSeat)
            {
                //如果没有对应的风牌，直接为0
                if (mah_hand[3, pppd.bySeatNum] >= 3)
                {
                    Mul_Wind += 200;
                }
            }
            else
            {
                //添加庄家对应风牌的番数
                if (mah_hand[3, pppd.byDealerSeat] >= 3)
                {
                    Mul_Wind += 100;
                }

                if (mah_hand[3, pppd.bySeatNum] >= 3)
                {
                    Mul_Wind += 100;
                }
            }


            //添加令牌番数
            if (mah_hand[4, 0] > 0)
            {
                for (int i = 1; i < 4; i++)
                {
                    if (mah_hand[4, i] >= 3)
                    {
                        Mul_Wind += 100;
                    }
                }
            }

            return Mul_Wind;
        }



        /// <summary>
        /// 判断十三不靠（147,258,369不同花色的2句，字牌7种都有，其中1种是对子，其他是单张）
        /// </summary>
        /// <param name="byaTileNum">各种花色的牌各个值的张数，一维的5代表花色；二维的10的0位置保存这个花色的总张数，1～9位置保存对应值的牌张数</param>
        /// <returns></returns>
        public int JudgeThirteenIndepend(byte[,] byaTileNum)
        {
            //如果已经听牌不检查
            if (GameData.Instance.PlayerPlayingPanelData.isChoicTir == 1)
            {
                return 0;
            }

            byte bySuit = 0;  //花色
            byte byValue = 0;  //值
            byte[] bytempSuit = new byte[10];  //[0]:147,[1]:148,[2]=:149,[3]:158,[4]:159,[5]:169,[6]:258,[7]:259,[8]:269,[9]:369
            byte[] bytempValue = new byte[10]; //[0]:147,[1]:148,[2]=:149,[3]:158,[4]:159,[5]:169,[6]:258,[7]:259,[8]:269,[9]:369

            byte bySuit147 = 0, bySuit258 = 0, bySuit369 = 0;
            //bool b147 = false, b258 = false, b369 = false;
            byte[,] byaTempTileNum = new byte[5, 10];

            for (int i = 0; i < 5; i++)
            {
                for (int k = 0; k < 10; k++)
                {
                    byaTempTileNum[i, k] = byaTileNum[i, k];
                }
            }

            //序数牌
            for (bySuit = SUIT_CHARACTER; bySuit < SUIT_WIND; bySuit++)
            {
                if (byaTempTileNum[bySuit - 1, 1] > 0 && byaTempTileNum[bySuit - 1, 4] > 0 && byaTempTileNum[bySuit - 1, 7] > 0)
                {
                    //b147 = true;
                    bytempSuit[0] = bySuit;
                    bytempValue[0]++;
                    bySuit147 = bySuit;
                    byaTempTileNum[bySuit - 1, 1]--;
                    byaTempTileNum[bySuit - 1, 4]--;
                    byaTempTileNum[bySuit - 1, 7]--;
                    byaTempTileNum[bySuit - 1, 0] -= 3;
                }

                if (byaTempTileNum[bySuit - 1, 1] > 0 && byaTempTileNum[bySuit - 1, 4] > 0 && byaTempTileNum[bySuit - 1, 8] > 0)
                {
                    //b258 = true;
                    bytempSuit[1] = bySuit;
                    bytempValue[1]++;
                    bySuit258 = bySuit;
                    byaTempTileNum[bySuit - 1, 1]--;
                    byaTempTileNum[bySuit - 1, 4]--;
                    byaTempTileNum[bySuit - 1, 8]--;
                    byaTempTileNum[bySuit - 1, 0] -= 3;
                }

                if (byaTempTileNum[bySuit - 1, 1] > 0 && byaTempTileNum[bySuit - 1, 4] > 0 && byaTempTileNum[bySuit - 1, 9] > 0)
                {
                    bytempSuit[2] = bySuit;
                    bytempValue[2]++;
                    bySuit369 = bySuit;
                    byaTempTileNum[bySuit - 1, 1]--;
                    byaTempTileNum[bySuit - 1, 4]--;
                    byaTempTileNum[bySuit - 1, 9]--;
                    byaTempTileNum[bySuit - 1, 0] -= 3;
                }

                if (byaTempTileNum[bySuit - 1, 1] > 0 && byaTempTileNum[bySuit - 1, 5] > 0 && byaTempTileNum[bySuit - 1, 8] > 0)
                {
                    bytempSuit[3] = bySuit;
                    bytempValue[3]++;
                    byaTempTileNum[bySuit - 1, 1]--;
                    byaTempTileNum[bySuit - 1, 5]--;
                    byaTempTileNum[bySuit - 1, 8]--;
                    byaTempTileNum[bySuit - 1, 0] -= 3;
                }
                if (byaTempTileNum[bySuit - 1, 1] > 0 && byaTempTileNum[bySuit - 1, 5] > 0 && byaTempTileNum[bySuit - 1, 9] > 0)
                {
                    bytempSuit[4] = bySuit;
                    bytempValue[4]++;
                    byaTempTileNum[bySuit - 1, 1]--;
                    byaTempTileNum[bySuit - 1, 5]--;
                    byaTempTileNum[bySuit - 1, 9]--;
                    byaTempTileNum[bySuit - 1, 0] -= 3;
                }
                if (byaTempTileNum[bySuit - 1, 1] > 0 && byaTempTileNum[bySuit - 1, 6] > 0 && byaTempTileNum[bySuit - 1, 9] > 0)
                {
                    bytempSuit[5] = bySuit;
                    bytempValue[5]++;
                    byaTempTileNum[bySuit - 1, 1]--;
                    byaTempTileNum[bySuit - 1, 6]--;
                    byaTempTileNum[bySuit - 1, 9]--;
                    byaTempTileNum[bySuit - 1, 0] -= 3;
                }
                if (byaTempTileNum[bySuit - 1, 2] > 0 && byaTempTileNum[bySuit - 1, 5] > 0 && byaTempTileNum[bySuit - 1, 8] > 0)
                {
                    bytempSuit[6] = bySuit;
                    bytempValue[6]++;
                    byaTempTileNum[bySuit - 1, 2]--;
                    byaTempTileNum[bySuit - 1, 5]--;
                    byaTempTileNum[bySuit - 1, 8]--;
                    byaTempTileNum[bySuit - 1, 0] -= 3;
                }
                if (byaTempTileNum[bySuit - 1, 2] > 0 && byaTempTileNum[bySuit - 1, 5] > 0 && byaTempTileNum[bySuit - 1, 9] > 0)
                {
                    bytempSuit[7] = bySuit;
                    bytempValue[7]++;
                    byaTempTileNum[bySuit - 1, 2]--;
                    byaTempTileNum[bySuit - 1, 5]--;
                    byaTempTileNum[bySuit - 1, 9]--;
                    byaTempTileNum[bySuit - 1, 0] -= 3;
                }
                if (byaTempTileNum[bySuit - 1, 2] > 0 && byaTempTileNum[bySuit - 1, 6] > 0 && byaTempTileNum[bySuit - 1, 9] > 0)
                {
                    bytempSuit[8] = bySuit;
                    bytempValue[8]++;
                    byaTempTileNum[bySuit - 1, 2]--;
                    byaTempTileNum[bySuit - 1, 6]--;
                    byaTempTileNum[bySuit - 1, 9]--;
                    byaTempTileNum[bySuit - 1, 0] -= 3;
                }
                if (byaTempTileNum[bySuit - 1, 3] > 0 && byaTempTileNum[bySuit - 1, 6] > 0 && byaTempTileNum[bySuit - 1, 9] > 0)
                {
                    bytempSuit[9] = bySuit;
                    bytempValue[9]++;
                    byaTempTileNum[bySuit - 1, 3]--;
                    byaTempTileNum[bySuit - 1, 6]--;
                    byaTempTileNum[bySuit - 1, 9]--;
                    byaTempTileNum[bySuit - 1, 0] -= 3;
                }
            }

            bool byIndepend1 = false; //是否能胡 十三不靠(硬靠)
            bool byIndepend2 = false; //是否能胡 十三不靠(软靠)
            if ((bytempSuit[0] != 0 && bytempSuit[6] != 0 && bytempSuit[9] == 0 && bytempSuit[0] != bytempSuit[6])
            || (bytempSuit[0] != 0 && bytempSuit[6] == 0 && bytempSuit[9] != 0 && bytempSuit[0] != bytempSuit[9])
            || (bytempSuit[0] == 0 && bytempSuit[6] != 0 && bytempSuit[9] != 0 && bytempSuit[6] != bytempSuit[9]))
            {
                byIndepend1 = true;
            }
            else
            {
                int iIndependNum = 0; //十三不靠数量
                byte[] byIndependSuit = new byte[2]; //那两门牌                
                for (int i = 0; i < 10; i++)
                {
                    if (bytempValue[i] > 0)
                    {
                        iIndependNum += bytempValue[i];
                    }
                }
                if (iIndependNum == 2)
                {
                    byIndepend2 = true;
                }
                else
                {
                    return -1;
                }
            }

            if (byIndepend1 || byIndepend2)
            {
                // 上面的147，258，369六张牌已经固定了，剩下8张是否6个单张和1个对子的字牌
                byte bySingleCount = 0;
                byte byPairCount = 0;
                for (byValue = 1; byValue <= 4; byValue++) // 风牌
                {
                    if (byaTempTileNum[SUIT_WIND - 1, byValue] == 1)
                    {
                        bySingleCount++;
                    }
                    else if (byaTempTileNum[SUIT_WIND - 1, byValue] == 2)
                    {
                        byPairCount++;
                    }
                }
                for (byValue = 1; byValue <= 3; byValue++) // 箭牌
                {
                    if (byaTempTileNum[SUIT_DRAGON - 1, byValue] == 1)
                    {
                        bySingleCount++;
                    }
                    else if (byaTempTileNum[SUIT_DRAGON - 1, byValue] == 2)
                    {
                        byPairCount++;
                    }
                }

                if (bySingleCount == 6 && byPairCount == 1 && byaTempTileNum[SUIT_WIND - 1, 0] + byaTempTileNum[SUIT_DRAGON - 1, 0] == 8)
                {
                    return byFanThirteenIndepend;
                }
            }

            return -1;
        }

        //注意要把吃抢的牌加入数组中
        /// <summary>
        /// 判断十三幺
        /// </summary>
        /// <param name="byaTileNum">各种花色的牌各个值的张数，一维的5代表花色；二维的10的0位置保存这个花色的总张数，1～9位置保存对应值的牌张数</param>
        int JudgeThirteenOrphans(byte[,] byaTileNum)
        {
            //把吃抢的牌加进来
            byte[,] byaTempTileNum = new byte[5, 10];
            byaTempTileNum = byaTileNum;

            bool bPaire = false;  //是否有对子
            byte bySuit = 0, byValue = 0;
            //检查四种风牌
            for (byValue = 1; byValue < 5; byValue++)
            {
                if (byaTempTileNum[SUIT_WIND - 1, byValue] < 1 || byaTempTileNum[SUIT_WIND - 1, byValue] > 2)
                {
                    return -1;
                }
                else if (byaTempTileNum[SUIT_WIND - 1, byValue] == 2)
                {
                    if (bPaire)
                    {
                        return -1;
                    }
                    bPaire = true;
                }
            }

            //检查三种箭牌
            for (byValue = 1; byValue < 4; byValue++)
            {
                if (byaTempTileNum[SUIT_DRAGON - 1, byValue] < 1 || byaTempTileNum[SUIT_DRAGON - 1, byValue] > 2)
                {
                    return -1;
                }
                else if (byaTempTileNum[SUIT_DRAGON - 1, byValue] == 2)
                {
                    if (bPaire)
                    {
                        return -1;
                    }
                    bPaire = true;
                }
            }

            //检查序数牌
            for (bySuit = SUIT_CHARACTER; bySuit < SUIT_WIND; bySuit++)
            {
                if (byaTempTileNum[bySuit - 1, 1] < 1 || byaTempTileNum[bySuit - 1, 1] > 2 || byaTempTileNum[bySuit - 1, 9] < 1 || byaTempTileNum[bySuit - 1, 9] > 2)
                {
                    return -1;
                }
                if (byaTempTileNum[bySuit - 1, 1] == 2 || byaTempTileNum[bySuit - 1, 9] == 2)
                {
                    if (bPaire)
                    {
                        return -1;
                    }
                    bPaire = true;
                }
                for (byValue = 2; byValue <= 8; byValue++)
                {
                    if (byaTempTileNum[bySuit - 1, byValue] > 0)
                    {
                        return -1;
                    }
                }
            }

            return byFanThirteenOrphans;
        }

        /// <summary>
        /// 判断七对
        /// </summary>
        /// <param name="byaTileNum"></param>
        /// <param name="byLaiziTile">< 癞子牌 (0 没有癞子 则癞子牌值)>
        /// <returns></returns>
        int JudgeSevenPairs(byte[,] byaTileNum, byte byLaiziTile = 0)
        {
            if (GameData.Instance.PlayerPlayingPanelData.isChoicTir == 1)
            {
                return 0;
            }
            byte byTotalNum = 0;  //牌的总数
            byte bySuit = 0, byValue = 0;  //花色值

            for (bySuit = SUIT_CHARACTER; bySuit < SUIT_FLOWER; bySuit++)
            {
                byTotalNum += byaTileNum[bySuit - 1, 0];
            }
            if (byTotalNum != 14)
            {
                return -1;
            }


            // 检查是否有顺子，长治麻将有顺子不算七对

            if (GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byWinSpecialSevenPairsFlag == 1)
            {
                for (bySuit = SUIT_CHARACTER; bySuit < SUIT_WIND; bySuit++) // 序数牌
                {
                    for (byValue = 1; byValue < 7; byValue++)
                    {
                        if (byaTileNum[bySuit - 1, byValue] > 0 && byaTileNum[bySuit - 1, byValue + 1] > 0 && byaTileNum[bySuit - 1, byValue + 2] > 0)
                        {
                            return -1;
                        }
                    }
                }
            }

            byte byLaiziSuit = 0; // 癞子的花色
            byte byLaiziValue = 0; // 癞子的牌值
            byte byLaiziNum = 0; //癞子牌数量
            if (byLaiziTile > 0)// 是否玩癞子
            {
                byLaiziSuit = (byte)(byLaiziTile >> 4);
                byLaiziValue = (byte)(byLaiziTile & 0x0F);
                byLaiziNum = byaTileNum[byLaiziSuit - 1, byLaiziValue];
                byaTileNum[byLaiziSuit - 1, byLaiziValue] = 0;
                byaTileNum[byLaiziSuit - 1, 0] -= byLaiziNum;
            }

            byte[] byaPairNum = new byte[5];  //5种花色对子的数量
            byte[] byaQuadrupletNum = new byte[5];  //5种花色四刻子的数量
            byte bySolaNum = 0; //单张数量
            for (bySuit = SUIT_CHARACTER; bySuit < SUIT_FLOWER; bySuit++) // 5种花色
            {
                for (byValue = 1; byValue < 10; byValue++)
                {
                    if (byaTileNum[bySuit - 1, byValue] == 2)
                    {
                        byaPairNum[bySuit - 1]++;
                    }
                    else if (byaTileNum[bySuit - 1, byValue] == 4)
                    {
                        byaPairNum[bySuit - 1] += 2;
                        byaQuadrupletNum[bySuit - 1]++;
                    }
                    else if (byaTileNum[bySuit - 1, byValue] == 1 || byaTileNum[bySuit - 1, byValue] == 3)
                    {
                        bySolaNum++;
                    }
                }
            }

            bool bSolaNumNoLaiZiNum = false;//单张数量是否与癞子数量一样 或者  单张数量与癞子减2相同 都能胡牌
            if (byLaiziNum > 0)
            {
                if (bySolaNum == byLaiziNum || byLaiziNum - 2 == bySolaNum || (bySolaNum == 0 && byLaiziNum % 2 == 0) || (byLaiziNum - bySolaNum) > 0 && (byLaiziNum - bySolaNum) % 2 == 0) // 判断用癞子牌去补单张
                {
                    bSolaNumNoLaiZiNum = true;
                }
            }

            if (byLaiziTile > 0)//还原癞子牌
            {
                byaTileNum[byLaiziSuit - 1, byLaiziValue] += byLaiziNum;
                byaTileNum[byLaiziSuit - 1, 0] += byLaiziNum;
            }

            if (bSolaNumNoLaiZiNum)
            {
                //TODO  癞子七对符合条件 可以胡牌

            }
            else if (byaPairNum[0] + byaPairNum[1] + byaPairNum[2] + byaPairNum[3] + byaPairNum[4] != 7) // 不是七对
            {
                return -1;
            }
            int iTotalQuadrupletNum = byaQuadrupletNum[0] + byaQuadrupletNum[1] + byaQuadrupletNum[2] + byaQuadrupletNum[3] + byaQuadrupletNum[4];
            //TODO: 要添加另外一个判断是否开启豪华七对
            if (iTotalQuadrupletNum > 0 && GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byWinSpecialLuxurySevenPairs == 1)  //豪华七对
            {
                return byFanLuxurySevenPairs + iTotalQuadrupletNum - 1;
            }
            //普通七对
            else
            {
                return byFanSevenPairs;
            }
        }


        /// <summary>
        /// 判断是否缺一门
        /// </summary>
        /// <param name="byaTileNum">各种花色的牌各个值的张数，一维的5代表花色；二维的10的0位置保存这个花色的总张数，1～9位置保存对应值的牌张数</param>
        /// <param name="status">1表示手里不可以有三种牌，2表示手里只能缺一门牌，不可以缺两门</param>
        /// <returns></returns>
        bool JudgeLackSuit(byte[,] byaTileNum, int status)

        {
            if (GameData.Instance.PlayerPlayingPanelData.isChoicTir == 1)
            {
                return false;
            }
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            bool[] baSuit = new bool[] { false, false, false };
            int baSuitNum = 0;  //保留几门
            byte bySuit = 0;
            //序数牌
            for (bySuit = SUIT_CHARACTER; bySuit < SUIT_WIND; bySuit++)
            {
                if (byaTileNum[bySuit - 1, 0] > 0)
                {
                    baSuit[bySuit - 1] = true;
                }
            }


            //检查玩家特殊得牌
            if (pppd.usersCardsInfo[0].listSpecialCards.Count > 0)
            {
                for (int i = 0; i < pppd.usersCardsInfo[0].listSpecialCards.Count; i++)
                {
                    if ((pppd.usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16) < 4)
                    {
                        baSuit[(pppd.usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16) - 1] = true;
                    }
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (baSuit[i])
                {
                    baSuitNum++;
                }
            }

            if (pppd.playingMethodConf.byWinLimitLack > 0)
            {
                //只能缺一门
                if (pppd.playingMethodConf.byWinLimitLack == 1)
                {
                    if (baSuitNum == 2)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                //最少缺一门
                else if (pppd.playingMethodConf.byWinLimitLack == 2)
                {
                    if (baSuitNum < 3)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                //三种花色都有
                if (baSuit[0] && baSuit[1] && baSuit[2])
                {
                    return false;
                }
                else
                {
                    //只可以缺一门
                    if (status == 2)
                    {
                        if (baSuitNum == 2)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (status == 1)
                    {
                        return true;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 判断靠八张
        /// </summary>
        /// <param name="byaTileNum">各种花色的牌各个值的张数，一维的5代表花色；二维的10的0位置保存这个花色的总张数，1～9位置保存对应值的牌张数</param>
        /// <returns>靠八张的最大数量</returns>
        int JudgeDependEight(byte[,] byaTileNum)
        {
            if (GameData.Instance.PlayerPlayingPanelData.isChoicTir == 1)
            {
                return 0;
            }
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            byte[] byaSuitNum = new byte[] { 0, 0, 0 };  //保存三种花色牌的数量
            byte bySuit = 0;

            //序数牌
            for (bySuit = SUIT_CHARACTER; bySuit < SUIT_WIND; bySuit++)
            {
                byaSuitNum[bySuit - 1] = byaTileNum[bySuit - 1, 0];
            }


            //添加吃牌的数量
            for (int i = 0; i < pppd.usersCardsInfo[0].listSpecialCards.Count; i++)
            {
                bySuit = (byte)(pppd.usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16);
                if (bySuit > 3)
                {
                    continue;
                }

                if (pppd.usersCardsInfo[0].listSpecialCards[i].type == 1 || pppd.usersCardsInfo[0].listSpecialCards[i].type == 2)
                {
                    byaSuitNum[bySuit - 1] += 3;
                }
                //明暗杠
                else if (pppd.usersCardsInfo[0].listSpecialCards[i].type == 3 || pppd.usersCardsInfo[0].listSpecialCards[i].type == 7)
                {
                    byaSuitNum[bySuit - 1] += 4;
                }
            }

            //获取最大数量
            byte byMax = 0;

            for (int i = 0; i < 3; i++)
            {
                if (byaSuitNum[i] > byMax)
                {
                    byMax = byaSuitNum[i];
                }
            }

            //Debug.LogError("玩家的靠八张数量:" + byMax);

            return byMax;
        }

        /// <summary>
        /// 判断玩家胡牌。如果返回值大于0就是可以胡牌
        /// </summary>
        /// <param name="byaTileNum"></param>
        /// <returns></returns>
        public int JudgeWin(byte[,] byaTileNum)
        {
            //for (int i = 0; i < 5; i++)
            //{
            //    Debug.LogError("=============" + byaTileNum[i, 0] + "," + byaTileNum[i, 1] + "," + byaTileNum[i, 2] + "," + byaTileNum[i, 3] + "," + byaTileNum[i, 4]
            //        + "," + byaTileNum[i, 5] + "," + byaTileNum[i, 6] + "," + byaTileNum[i, 7] + "," + byaTileNum[i, 8] + "," + byaTileNum[i, 9]);
            //}
            //Debug.LogError("==========================================================================");
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            //检查麻将牌数量
            byte byTotalTileNum = 0;
            for (byte bySuit = SUIT_CHARACTER; bySuit < SUIT_FLOWER; bySuit++)
            {
                byte bySuitTileNum = 0;
                for (byte byValue = 1; byValue < 10; byValue++)
                {
                    bySuitTileNum += byaTileNum[bySuit - 1, byValue];
                    byTotalTileNum += byaTileNum[bySuit - 1, byValue];
                }
                if (bySuitTileNum != byaTileNum[bySuit - 1, 0])
                {
                    return -1;
                }
            }

            int byThirteenOrphansNum = 0;  //十三幺的吃抢的数量
            for (int i = 0; i < pppd.usersCardsInfo[0].listSpecialCards.Count; i++)
            {
                //判断玩家是否有吃抢的牌
                if (pppd.usersCardsInfo[0].listSpecialCards[i].type == 5 || pppd.usersCardsInfo[0].listSpecialCards[i].type == 6)
                {
                    byThirteenOrphansNum++;
                }
            }


            if (byThirteenOrphansNum == 0 && byTotalTileNum % 3 != 2)
            {
                return -1;
            }

            //是否有风牌
            if (GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byCreateModeHaveWind == 0 && byaTileNum[SUIT_WIND - 1, 0] > 0)
            {
                //  Debug.Log("有风牌 不能胡牌\n");
                return -1;
            }
            //是否有箭牌
            if (GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byCreateModeHaveDragon == 0 && byaTileNum[SUIT_DRAGON - 1, 0] > 0)
            {
                //Debug.Log("有箭牌 不能胡牌\n");
                return -1;
            }

            // 只有红中
            if (GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byCreateModeHaveDragon == 2 && (byaTileNum[SUIT_DRAGON - 1, 0] - byaTileNum[SUIT_DRAGON - 1, 1]) > 0)
            {
                //Debug.Log("除了红中还有其他牌 不能胡牌\n");
                return -1;
            }

            //检查缺一门
            if (GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byWinLimitLack > 0)
            {
                if (!JudgeLackSuit(byaTileNum, 1))
                {
                    return -1;
                }
            }

            //检查靠八张
            bool bCheckDependEightOk = true;
            int byDependEight = 0;
            if (GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byWinLimitDependEight > 0)
            {
                //Debug.LogError("===================================byWinLimitDependEight");
                byDependEight = JudgeDependEight(byaTileNum);
                if (byDependEight < 8)
                {
                    //  Debug.LogError("================================byDependEight:" + byDependEight);
                    //十三幺不需要靠八张，这里不直接返回，等后面在检查十三幺
                    bCheckDependEightOk = false;
                }
            }


            //番数
            int iTotalFan = 0;
            byte byLaiziTile = 0;//癞子牌
           // Debug.LogError("是否有癞子："+GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byDiscardLaizi);
          //  Debug.LogError(GameData.Instance.PlayerNodeDef.byLaiziSuit + "--" + GameData.Instance.PlayerNodeDef.byLaiziValue);
            if (GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byDiscardLaizi != 0)
            {
                byLaiziTile = (byte)(GameData.Instance.PlayerNodeDef.byLaiziSuit << 4 | GameData.Instance.PlayerNodeDef.byLaiziValue);
            }
            //检查特殊胡牌
            iTotalFan = JudgeSpecialWin(byaTileNum, byLaiziTile);

            //  Debug.LogError("iTotalFan" + iTotalFan);
            iFanWin = (byte)iTotalFan;

            if (!bCheckDependEightOk && iFanThirteenIndepend <= 0)
            {
                // Debug.LogWarning ("不是靠八张，也不是十三幺");
                return -1;
            }
            if (iTotalFan == -1)
            {
                // Debug.LogWarning("癞子byDiscardLaizi：" + GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byDiscardLaizi);
                //检查普通胡牌////是否癞子
                if (GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byDiscardLaizi != 0)
                {

                    // Debug.LogWarningFormat("byLaiziSuit:{0},byLaiziValue:{1}", GameData.Instance.PlayerNodeDef.byLaiziSuit, GameData.Instance.PlayerNodeDef.byLaiziValue);
                    if (JudgeLaiZiWin(byaTileNum, GameData.Instance.PlayerNodeDef.byLaiziSuit, GameData.Instance.PlayerNodeDef.byLaiziValue))
                    {
                        iTotalFan = 1;
                    }
                }
                else //没有癞子
                {
                    if (JudgeNormalWin(byaTileNum))
                    {
                        iTotalFan = 1;
                    }
                }
                //Debug.LogError("玩家普通胡牌:" + iTotalFan);
            }

            if (iTotalFan == -1)
            {
                return -1;
            }

            if (iTotalFan > 0)
            {
                pppd.isNeedSendPassMessage = false;
            }

            //检查特殊胡牌
            return iTotalFan;
        }


        /// <summary>
        /// 判断特殊胡牌
        /// </summary>
        /// <param name="byaTileNum"></param>
        /// <param name="byLaiziTile"></癞子牌  （0 没有癞子 则癞子牌值）>
        /// <returns></returns>
        int JudgeSpecialWin(byte[,] byaTileNum, byte byLaiziTile = 0)
        {
            int iFan = 0;
            //判断四癞子
          //  Debug.LogWarning("癞子牌值" + byLaiziTile.ToString("X2"));
            //Debug.LogError(byaTileNum[(byLaiziTile >> 4) - 1, 0]);
            if (byLaiziTile >>4>0)
            {
                if (byaTileNum[(byLaiziTile >> 4) - 1, 0] > 0)
                {
                    if (byaTileNum[(byLaiziTile >> 4) - 1, byLaiziTile & 0x0f] >= 4)
                    {
                        return 1;
                    }
                }
            }
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            //番数


            //十三不靠
            if (pppd.playingMethodConf.byWinSpecialThirteenIndepend > 0 && pppd.playingMethodConf.byFanThirteenIndepend > 0)
            {
                iFan = JudgeThirteenIndepend(byaTileNum);
                if (iFan > -1)
                {
                    return iFan;
                }
            }
            //十三幺
            if (pppd.playingMethodConf.byWinSpecialThirteenOrphans > 0 && pppd.playingMethodConf.byFanThirteenOrphans > 0)
            {
                iFan = JudgeThirteenOrphans(byaTileNum);

                //Debug.LogError("iFan:" + iFan);

                if (iFan > 0)
                {
                    iFanThirteenIndepend = iFan;
                }
                if (iFan > -1)
                {
                    return iFan;
                }
            }

            //7对/豪华七对
            if (pppd.playingMethodConf.byWinSpecialSevenPairs > 0 && (pppd.playingMethodConf.byFanSevenPairs > 0 || pppd.playingMethodConf.byFanLuxurySevenPairs > 0))
            {
                iFan = JudgeSevenPairs(byaTileNum, byLaiziTile);
                if (iFan > -1)
                {
                    return iFan;
                }
            }

            return -1;
        }


        /// <summary>
        /// 将指定的手牌转换为5*10的数组
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public byte[,] SwitchArray(byte[] temp)
        {
            byte[,] value = new byte[5, 10];

            byte bySuit = 0;
            byte byValue = 0;
            //先把手牌添加到数组中
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == 0)
                {
                    continue;
                }
                bySuit = (byte)(temp[i] / 16);  //花色
                byValue = (byte)(temp[i] % 16);  //值
                //确定该花色的牌的总数
                value[bySuit - 1, 0]++;
                //添加对应的牌到数组中
                value[bySuit - 1, byValue]++;
            }
            return value;
        }

        //======================================  红中麻将======================================================
        /**
         * 判断是否癞子胡牌
         * @param byaTileNum 各种花色的牌各个值的张数，一维的5代表花色；二维的10的0位置保存这个花色的总张数，1～9位置保存对应值的牌张数
         * @param resultType 结果信息
         *  癞子的花色
         *   癞子的牌值@return 是否胡牌
         */
        public bool JudgeLaiZiWin(byte[,] byaTileNum, byte byLaiziSuit = 0, byte byLaiziValue = 0)
        {
            byte[] byCanHuTile = new byte[34];// 保存能胡的牌
            byte byCanHuTileNum = 0; // 能胡的牌数量
            byte byLaiziNum = 0; //癞子牌数量
            bool bCanHu = false;
            //计算癞子的数量
            if (byLaiziSuit > 0 && byLaiziValue > 0)
            {
                byLaiziNum = byaTileNum[byLaiziSuit - 1,byLaiziValue];
                byaTileNum[byLaiziSuit - 1,byLaiziValue] -= byLaiziNum;
                byaTileNum[byLaiziSuit - 1,0] -= byLaiziNum;
            }
            //Debug.LogError("==================22222 ");
            byte[] byaTestTileWin = new byte[34] { 17, 18, 19, 20, 21, 22, 23, 24, 25, 33, 34, 35, 36, 37, 38, 39, 40, 41, 49, 50, 51, 52, 53, 54, 55, 56, 57, 65, 66, 67, 68, 81, 82, 83 };
            if (byLaiziNum == 0)
            {
                if (JudgeNormalWin(byaTileNum))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (byLaiziNum == 1)
            {
                for (int k = 0; k < 27; k++)
                {
                    byaTileNum[(byaTestTileWin[k] >> 4) - 1,0]++;
                    byaTileNum[(byaTestTileWin[k] >> 4) - 1,(byaTestTileWin[k] & 0x0F)]++;
                    if (JudgeNormalWin(byaTileNum))
                    {
                        byCanHuTileNum++;
                        bCanHu = true;
                    }
                    byaTileNum[(byaTestTileWin[k] >> 4) - 1,0]--;
                    byaTileNum[(byaTestTileWin[k] >> 4) - 1,(byaTestTileWin[k] & 0x0F)]--;
                    if(bCanHu)
                    {
                        break;
                    }
                }
            }
            else if (byLaiziNum == 2)
            {
                for (int i = 0; i < 27; i++)
                {
                    byaTileNum[(byaTestTileWin[i] >> 4) - 1,0]++;
                    byaTileNum[(byaTestTileWin[i] >> 4) - 1,(byaTestTileWin[i] & 0x0F)]++;
                    for (int j = 0; j < 27; j++)
                    {
                        byaTileNum[(byaTestTileWin[j] >> 4) - 1,0]++;
                        byaTileNum[(byaTestTileWin[j] >> 4) - 1,(byaTestTileWin[j] & 0x0F)]++;
                        
                        if (JudgeNormalWin(byaTileNum))
                        {
                            byCanHuTileNum++;
                            bCanHu = true;
                        }
                        byaTileNum[(byaTestTileWin[j] >> 4) - 1,0]--;
                        byaTileNum[(byaTestTileWin[j] >> 4) - 1,(byaTestTileWin[j] & 0x0F)]--;
                        if (bCanHu)
                        {
                            break;
                        }
                    }
                    byaTileNum[(byaTestTileWin[i] >> 4) - 1,0]--;
                    byaTileNum[(byaTestTileWin[i] >> 4) - 1,(byaTestTileWin[i] & 0x0F)]--;
                    if (bCanHu)
                    {
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 27; i++)
                {
                    byaTileNum[(byaTestTileWin[i] >> 4) - 1,0]++;
                    byaTileNum[(byaTestTileWin[i] >> 4) - 1,(byaTestTileWin[i] & 0x0F)]++;
                    for (int j = 0; j < 27; j++)
                    {
                        byaTileNum[(byaTestTileWin[j] >> 4) - 1,0]++;
                        byaTileNum[(byaTestTileWin[j] >> 4) - 1,(byaTestTileWin[j] & 0x0F)]++;
                        for (int k = 0; k < 27; k++)
                        {
                            byaTileNum[(byaTestTileWin[k] >> 4) - 1,0]++;
                            byaTileNum[(byaTestTileWin[k] >> 4) - 1,(byaTestTileWin[k] & 0x0F)]++;
                            
                            if (JudgeNormalWin(byaTileNum))
                            {
                                byCanHuTileNum++;
                                bCanHu = true;
                            }
                            byaTileNum[(byaTestTileWin[k] >> 4) - 1,0]--;
                            byaTileNum[(byaTestTileWin[k] >> 4) - 1,(byaTestTileWin[k] & 0x0F)]--;
                            if (bCanHu)
                            {
                                break;
                            }
                        }
                        byaTileNum[(byaTestTileWin[j] >> 4) - 1,0]--;
                        byaTileNum[(byaTestTileWin[j] >> 4) - 1,(byaTestTileWin[j] & 0x0F)]--;
                        if (bCanHu)
                        {
                            break;
                        }
                    }
                    byaTileNum[(byaTestTileWin[i] >> 4) - 1,0]--;
                    byaTileNum[(byaTestTileWin[i] >> 4) - 1,(byaTestTileWin[i] & 0x0F)]--;
                    if (bCanHu)
                    {
                        break;
                    }
                }
            }
          
            if (byCanHuTileNum == 0)
            {
                return false;
            }
            return true;
        }





            ////byaTileNum=new byte [5, 10];
            //byte bySuit = 0;
            //byte byValue = 0;
            //byte byChangeValue = 0; // 变化的牌值


            //byte byLaiziNum = 0; //癞子牌数量
            //byte byEmployLaiZiNum = 0; //使用癞子的数量
            //byte byIndex = 0; // 当前牌的下标
            //byte byRemainingNum = 0; // 去除正常顺子刻子后剩余的牌
            //byte[] byaTiles = new byte[14]; // 用于保存需要癞子的牌,包括将牌
            //byte[,] byaSaveTileNum = new byte[byaTileNum.GetLength(0), byaTileNum.GetLength(1)];
            //for (int i = 0; i < byaTileNum.GetLength(0); i++) // 用于保存手牌
            //{
            //    for (int y = 0; y < byaTileNum.GetLength(1); y++)
            //    {
            //        byaSaveTileNum[i, y] = byaTileNum[i, y];
            //    }
            //}

            //bool bSuccess = false; // 能否胡牌
            //bool bJong = false; // 将是否存在
            //bool bJongTile = false;//奖牌是否找到
            //byte byRemainder = 0; // 余数
            //byte byJongSuit = 0; // 将牌的花色
            //byte byJongValue = 0; // 将牌的牌值
            //byte byJongValueTemp = 0;//
            //                         //memset(byaTiles, 0, byaTiles.Length );
            //                         //保存手牌
            //                         //memset(byaSaveTileNum, 0, byaSaveTileNum.Length );

            //// memcpy(byaSaveTileNum, byaTileNum, byaSaveTileNum.Length );
            ////计算癞子的数量
            //// 是否满足33332模型 // 这里考虑没有癞子时候 要先找出将牌
            ////if (byLaiziSuit == 0)
            ////{
            ////    //  Debug.LogError("byLaiziSuit:" + byLaiziSuit);
            ////    return false;
            ////}
            //byLaiziNum = byaSaveTileNum[byLaiziSuit - 1, byLaiziValue];
            //byaSaveTileNum[byLaiziSuit - 1, byLaiziValue] = 0;
            //byaSaveTileNum[byLaiziSuit - 1, 0] -= byLaiziNum;

            //if (byLaiziNum == 0)//  没有癞子 (3*n+2模型)
            //{
            //    if (JudgeNormalWin(byaSaveTileNum))
            //    {
            //        Debug.LogError("jugeNormalWin");
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //for (bySuit = SUIT_CHARACTER; bySuit < SUIT_FLOWER; bySuit++) // 5种花色
            //{
            //    byRemainder = (byte)(byaSaveTileNum[bySuit - 1, 0] % 3);
            //    if (byLaiziNum > 0)
            //    {
            //        if (byRemainder == 1)
            //        {
            //            if (bySuit < SUIT_WIND)
            //            {
            //                for (byValue = 1; byValue < 10; byValue++)
            //                {
            //                    if (byValue < 2 && byaSaveTileNum[bySuit - 1,byValue] == 2 && byaSaveTileNum[bySuit - 1,byValue + 1] == 1 && byaSaveTileNum[bySuit - 1,byValue + 2] == 2 && byaSaveTileNum[bySuit - 1,byValue + 3] == 2 && byaSaveTileNum[bySuit - 1,byValue + 4] == 1 &&
            //            ((byaSaveTileNum[bySuit - 1,byValue + 5] == 2 && byaSaveTileNum[bySuit - 1,byValue + 6] == 0 && byaSaveTileNum[bySuit - 1,byValue + 7] == 0 && byaSaveTileNum[bySuit - 1,byValue + 8] == 0) ||
            //            (byaSaveTileNum[bySuit - 1,byValue + 5] == 0 && byaSaveTileNum[bySuit - 1,byValue + 6] == 2 && byaSaveTileNum[bySuit - 1,byValue + 7] == 0 && byaSaveTileNum[bySuit - 1,byValue + 8] == 0) ||
            //            (byaSaveTileNum[bySuit - 1,byValue + 5] == 0 && byaSaveTileNum[bySuit - 1,byValue + 6] == 0 && byaSaveTileNum[bySuit - 1,byValue + 7] == 2 && byaSaveTileNum[bySuit - 1,byValue + 8] == 0) ||
            //            (byaSaveTileNum[bySuit - 1,byValue + 5] == 0 && byaSaveTileNum[bySuit - 1,byValue + 6] == 0 && byaSaveTileNum[bySuit - 1,byValue + 7] == 0 && byaSaveTileNum[bySuit - 1,byValue + 8] == 2)))
            //                    {
            //                        if (byaSaveTileNum[bySuit - 1,byValue + 5] == 2 && byaSaveTileNum[bySuit - 1,byValue + 6] == 0 && byaSaveTileNum[bySuit - 1,byValue + 7] == 0 && byaSaveTileNum[bySuit - 1,byValue + 8] == 0)
            //                        {
            //                            byJongValueTemp = (byte)(byValue + 5);
            //                        }
            //                        else if (byaSaveTileNum[bySuit - 1,byValue + 5] == 0 && byaSaveTileNum[bySuit - 1,byValue + 6] == 2 && byaSaveTileNum[bySuit - 1,byValue + 7] == 0 && byaSaveTileNum[bySuit - 1,byValue + 8] == 0)
            //                        {
            //                            byJongValueTemp = (byte)(byValue + 6);
            //                        }
            //                        else if (byaSaveTileNum[bySuit - 1,byValue + 5] == 0 && byaSaveTileNum[bySuit - 1,byValue + 6] == 0 && byaSaveTileNum[bySuit - 1,byValue + 7] == 2 && byaSaveTileNum[bySuit - 1,byValue + 8] == 0)
            //                        {
            //                            byJongValueTemp = (byte)(byValue + 7);
            //                        }
            //                        else
            //                        {
            //                            byJongValueTemp = (byte)(byValue + 8);
            //                        }

            //                        byJongSuit = bySuit;
            //                        bJong = true;
            //                        break;
            //                    }
            //                    else if (byValue < 2 && byaSaveTileNum[bySuit - 1,byValue] == 2 && byaSaveTileNum[bySuit - 1,byValue + 1] == 1 && byaSaveTileNum[bySuit - 1,byValue + 2] == 2 && byaSaveTileNum[bySuit - 1,byValue + 3] == 2 && byaSaveTileNum[bySuit - 1,byValue + 4] == 1 &&
            //                             ((byaSaveTileNum[bySuit - 1,byValue + 5] == 1 && byaSaveTileNum[bySuit - 1,byValue + 6] == 1 && byaSaveTileNum[bySuit - 1,byValue + 7] == 0 && byaSaveTileNum[bySuit - 1,byValue + 8] == 0) ||
            //                             (byaSaveTileNum[bySuit - 1,byValue + 5] == 0 && byaSaveTileNum[bySuit - 1,byValue + 6] == 0 && byaSaveTileNum[bySuit - 1,byValue + 7] == 1 && byaSaveTileNum[bySuit - 1,byValue + 8] == 1) ||
            //                                 (byaSaveTileNum[bySuit - 1,byValue + 5] == 0 && byaSaveTileNum[bySuit - 1,byValue + 6] == 1 && byaSaveTileNum[bySuit - 1,byValue + 7] == 1 && byaSaveTileNum[bySuit - 1,byValue + 8] == 0)))
            //                    {
            //                        if (byaSaveTileNum[bySuit - 1,byValue + 5] == 1 && byaSaveTileNum[bySuit - 1,byValue + 6] == 1 && byaSaveTileNum[bySuit - 1,byValue + 7] == 0 && byaSaveTileNum[bySuit - 1,byValue + 8] == 0)
            //                        {
            //                            byJongValueTemp = (byte)(byValue + 7);
            //                        }
            //                        else if (byaSaveTileNum[bySuit - 1,byValue + 5] == 0 && byaSaveTileNum[bySuit - 1,byValue + 6] == 0 && byaSaveTileNum[bySuit - 1,byValue + 7] == 1 && byaSaveTileNum[bySuit - 1,byValue + 8] == 1)
            //                        {
            //                            byJongValueTemp = (byte)(byValue + 6);
            //                        }
            //                        else
            //                        {
            //                            byJongValueTemp = (byte)(byValue + 8);
            //                        }
            //                        byJongSuit = bySuit;
            //                        bJong = true;
            //                        break;
            //                    }
            //                    else if (byValue < 2 && byaSaveTileNum[bySuit - 1,byValue] == 2 && byaSaveTileNum[bySuit - 1,byValue + 1] == 1 && byaSaveTileNum[bySuit - 1,byValue + 2] == 2
            //            && byaSaveTileNum[bySuit - 1,byValue + 3] == 2 && byaSaveTileNum[bySuit - 1,byValue + 4] == 1 && byaSaveTileNum[bySuit - 1,byValue + 5] == 0 && byaSaveTileNum[bySuit - 1,byValue + 6] == 1 && byaSaveTileNum[bySuit - 1,byValue + 7] == 1 && byaSaveTileNum[bySuit - 1,byValue + 8] == 0)
            //                    {
            //                        byJongSuit = bySuit;
            //                        bJong = true;
            //                        byJongValueTemp = (byte)(byValue + 8);
            //                        break;
            //                    }
            //                    else if (byValue < 2 && byaSaveTileNum[bySuit - 1,byValue] == 1 && byaSaveTileNum[bySuit - 1,byValue + 1] == 1 && byaSaveTileNum[bySuit - 1,byValue + 2] == 1
            //            && byaSaveTileNum[bySuit - 1,byValue + 3] == 2 && byaSaveTileNum[bySuit - 1,byValue + 4] == 0 && byaSaveTileNum[bySuit - 1,byValue + 5] == 0 && byaSaveTileNum[bySuit - 1,byValue + 6] == 0 && byaSaveTileNum[bySuit - 1,byValue + 7] == 2 && byaSaveTileNum[bySuit - 1,byValue + 8] == 3)
            //                    {
            //                        byJongSuit = bySuit;
            //                        bJong = true;
            //                        byJongValueTemp = (byte)(byValue + 7);
            //                        break;
            //                    }
            //                    else if (byValue < 2 && byaSaveTileNum[bySuit - 1,byValue] == 2 && byaSaveTileNum[bySuit - 1,byValue + 1] == 1 && byaSaveTileNum[bySuit - 1,byValue + 2] == 2
            //                        && byaSaveTileNum[bySuit - 1,byValue + 3] == 2 && byaSaveTileNum[bySuit - 1,byValue + 4] == 1 && byaSaveTileNum[bySuit - 1,byValue + 5] == 0 && byaSaveTileNum[bySuit - 1,byValue + 6] == 1 && byaSaveTileNum[bySuit - 1,byValue + 7] == 0 && byaSaveTileNum[bySuit - 1,byValue + 8] == 1)
            //                    {
            //                        byJongSuit = bySuit;
            //                        bJong = true;
            //                        byJongValueTemp = (byte)(byValue + 7);
            //                        break;
            //                    }
            //                    else if (byValue < 2 && byaSaveTileNum[bySuit - 1,byValue] == 2 && byaSaveTileNum[bySuit - 1,byValue + 1] == 1 && byaSaveTileNum[bySuit - 1,byValue + 2] == 2
            //                        && byaSaveTileNum[bySuit - 1,byValue + 3] == 2 && byaSaveTileNum[bySuit - 1,byValue + 4] == 1 && byaSaveTileNum[bySuit - 1,byValue + 5] == 0 && byaSaveTileNum[bySuit - 1,byValue + 6] == 0 && byaSaveTileNum[bySuit - 1,byValue + 7] == 1 && byaSaveTileNum[bySuit - 1,byValue + 8] == 1)
            //                    {
            //                        byJongSuit = bySuit;
            //                        bJong = true;
            //                        byJongValueTemp = (byte)(byValue + 6);
            //                        break;
            //                    }
            //                    else if (byValue < 2 && byaSaveTileNum[bySuit - 1,byValue] == 1 && byaSaveTileNum[bySuit - 1,byValue + 1] == 1 && byaSaveTileNum[bySuit - 1,byValue + 2] == 1
            //                        && byaSaveTileNum[bySuit - 1,byValue + 3] == 2 && byaSaveTileNum[bySuit - 1,byValue + 4] == 0 && byaSaveTileNum[bySuit - 1,byValue + 5] == 0 && byaSaveTileNum[bySuit - 1,byValue + 6] == 1 && byaSaveTileNum[bySuit - 1,byValue + 7] == 2 && byaSaveTileNum[bySuit - 1,byValue + 8] == 2)
            //                    {
            //                        byJongSuit = bySuit;
            //                        bJong = true;
            //                        byJongValueTemp = (byte)(byValue + 6);
            //                        break;
            //                    }
            //                    else if (byValue < 3 && byaSaveTileNum[bySuit - 1,byValue] == 1 && byaSaveTileNum[bySuit - 1,byValue + 1] == 1 && byaSaveTileNum[bySuit - 1,byValue + 2] == 2
            //           && byaSaveTileNum[bySuit - 1,byValue + 3] == 0 && byaSaveTileNum[bySuit - 1,byValue + 4] == 0 && byaSaveTileNum[bySuit - 1,byValue + 5] == 2 && byaSaveTileNum[bySuit - 1,byValue + 6] == 1 && byaSaveTileNum[bySuit - 1,byValue + 7] == 2)
            //                    {
            //                        byJongSuit = bySuit;
            //                        byJongValueTemp = (byte)(byValue + 6);
            //                        bJong = true;
            //                        break;
            //                    }
            //                    else if (byValue < 4 && byaSaveTileNum[bySuit - 1, byValue] == 2 && byaSaveTileNum[bySuit - 1, byValue + 1] == 1
            //                    && byaSaveTileNum[bySuit - 1, byValue + 2] == 2 && byaSaveTileNum[bySuit - 1, byValue + 3] == 2 && byaSaveTileNum[bySuit - 1, byValue + 4] == 2
            //                    && byaSaveTileNum[bySuit - 1, byValue + 5] == 0 && byaSaveTileNum[bySuit - 1, byValue + 6] == 1)
            //                    {
            //                        byJongSuit = bySuit;
            //                        bJong = true;
            //                        byJongValueTemp = (byte)(byValue + 5);
            //                        break;
            //                    }
            //                    else if (byValue < 7 && ((byaSaveTileNum[bySuit - 1, byValue] == 1 && byaSaveTileNum[bySuit - 1, byValue + 1] == 1 && byaSaveTileNum[bySuit - 1, byValue + 3] == 2)
            //                        || (byaSaveTileNum[bySuit - 1, byValue] == 2 && byaSaveTileNum[bySuit - 1, byValue + 1] == 1 && byaSaveTileNum[bySuit - 1, byValue + 3] == 1)))
            //                    {
            //                        if (byaSaveTileNum[bySuit - 1,byValue + 2]  == 1 || byaSaveTileNum[bySuit - 1,byValue + 2] == 2 || (byValue > 0 && byaSaveTileNum[bySuit - 1,byValue - 1] == 2))
            //                        {
            //                            continue;
            //                        }
            //                        byJongSuit = bySuit;
            //                        bJong = true;
            //                        byJongValueTemp = (byte)(byValue + 2);
            //                        break;
            //                    }
            //                    else if (byValue < 8
            //                    && ((byaSaveTileNum[bySuit - 1,byValue] == 1 && byaSaveTileNum[bySuit - 1,byValue + 1] == 1 && byaSaveTileNum[bySuit - 1,byValue + 2] == 3)
            //                            || (byaSaveTileNum[bySuit - 1,byValue] == 3 && byaSaveTileNum[bySuit - 1,byValue + 1] == 1 && byaSaveTileNum[bySuit - 1,byValue + 2] == 1)))
            //                    {

            //                        if ((byaSaveTileNum[bySuit - 1,byValue] == 1 && byaSaveTileNum[bySuit - 1,byValue + 1] == 1 && byaSaveTileNum[bySuit - 1,byValue + 2] == 3))
            //                        {
            //                            byJongValueTemp = (byte)(byValue - 1);
            //                        }
            //                        else
            //                        {
            //                            byJongValueTemp = (byte)(byValue + 3);
            //                        }
            //                        byJongSuit = bySuit;
            //                        bJong = true;
            //                        break;
            //                    }

            //                }
            //            }
            //        }
            //        else if (byRemainder == 2)
            //        {
            //            if (bySuit < SUIT_WIND)
            //            {
            //                for (byValue = 1; byValue < 10; byValue++)
            //                {
            //                    if (byValue < 8 && ((byaSaveTileNum[bySuit - 1, byValue] == 3 && byaSaveTileNum[bySuit - 1, byValue + 1] == 1 && byaSaveTileNum[bySuit - 1, byValue + 2] == 1)
            //                        || (byaSaveTileNum[bySuit - 1, byValue] == 1 && byaSaveTileNum[bySuit - 1, byValue + 1] == 1 && byaSaveTileNum[bySuit - 1, byValue + 2] == 3)))
            //                    {
            //                        byJongSuit = bySuit;
            //                        bJong = true;
            //                        break;
            //                    }
            //                    else if (byValue < 7 && ((byaSaveTileNum[bySuit - 1, byValue] == 1 && byaSaveTileNum[bySuit - 1, byValue + 1] == 1 && byaSaveTileNum[bySuit - 1, byValue + 2] == 1 && byaSaveTileNum[bySuit - 1, byValue + 3] == 2)
            //                        || (byaSaveTileNum[bySuit - 1, byValue] == 2 && byaSaveTileNum[bySuit - 1, byValue + 1] == 1 && byaSaveTileNum[bySuit - 1, byValue + 2] == 1 && byaSaveTileNum[bySuit - 1, byValue + 3] == 1)))
            //                    {
            //                        byJongSuit = bySuit;
            //                        bJong = true;
            //                        break;
            //                    }
            //                    else if (bySuit < SUIT_WIND && byValue < 6 && byaSaveTileNum[bySuit - 1,byValue] == 2 && byaSaveTileNum[bySuit - 1,byValue + 1] == 1
            //                    && byaSaveTileNum[bySuit - 1,byValue + 2] == 2 && byaSaveTileNum[bySuit - 1,byValue + 3] == 2 && byaSaveTileNum[bySuit - 1,byValue + 4] == 1)
            //                    {
            //                        byJongSuit = bySuit;
            //                        bJong = true;
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //        if (bJong)
            //        {
            //            break;
            //        }
            //    }
            //}
            //// Debug.LogWarningFormat("MahjongHelper::JudgeNormalWin,癞子牌{0},{1}数量{2}", byLaiziSuit, byLaiziValue, byLaiziNum);
            ////去除不许要癞子成扑的牌
            //for (bySuit = SUIT_CHARACTER; bySuit < SUIT_FLOWER; bySuit++)
            //{
            //    if (bJong && byJongSuit == bySuit)
            //    {
            //        for (byValue = 1; byValue < 10; byValue++)
            //        {
            //            if (!bJongTile && byaSaveTileNum[bySuit - 1, byValue] >= 2)
            //            {
            //                //memset(&tempResult, 0, sizeof(tempResult));
            //                // memcpy(&tempResult, &resultType, sizeof(ResultTypeDef));
            //                // 除去2张将牌
            //                byaSaveTileNum[byJongSuit - 1, byValue] -= 2;
            //                byaSaveTileNum[byJongSuit - 1, 0] -= 2;
            //                if (byJongValueTemp > 0)
            //                {
            //                    byaSaveTileNum[byJongSuit - 1, byJongValueTemp] += 1;
            //                    byaSaveTileNum[byJongSuit - 1, 0] += 1;
            //                }
            //                byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);

            //                if (AnalyzeSuit(bySuit > SUIT_BAMBOO ? true : false, byJongSuit))
            //                {
            //                    SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //                    byaTiles[byRemainingNum] = (byte)(byJongSuit << 4 | byValue);
            //                    byRemainingNum++;
            //                    byaTiles[byRemainingNum] = (byte)(byJongSuit << 4 | byValue);
            //                    byRemainingNum++;
            //                    byJongValue = byValue;
            //                    bJongTile = true;
            //                }
            //                else
            //                {
            //                    // 还原2张将牌
            //                    byaSaveTileNum[byJongSuit - 1, byValue] += 2;
            //                    byaSaveTileNum[byJongSuit - 1, 0] += 2;
            //                }
            //                if (byJongValueTemp > 0)
            //                {
            //                    byaSaveTileNum[byJongSuit - 1, byJongValueTemp] -= 1;
            //                    byaSaveTileNum[byJongSuit - 1, 0] -= 1;
            //                }
            //            }
            //        }
            //    }
            //    //   Debug.Log("==================22222 bySuit：" + bySuit);
            //    byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //    AnalyzeSuit(bySuit, byLaiziSuit, byLaiziValue, 20001);
            //    SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //}

            //// Debug.LogWarningFormat("MahjongHelper::JudgeNormalWin 万总张数{0}:{1}{2}{3}{4}{5}{6}{7}{8}{9}",byaSaveTileNum[0,0],byaSaveTileNum[0,1] ,byaSaveTileNum[0,2],byaSaveTileNum[0,3],byaSaveTileNum[0,4],byaSaveTileNum[0,5],byaSaveTileNum[0,6],byaSaveTileNum[0,7],byaSaveTileNum[0,8],byaSaveTileNum[0,9]);
            ////Debug.LogWarningFormat("MahjongHelper::JudgeNormalWin 筒总张数[%d]：" + byaSaveTileNum[1,0] + " " + byaSaveTileNum[1,1] + " " + byaSaveTileNum[1,2] + " " + byaSaveTileNum[1,3] + " " + byaSaveTileNum[1,4] + " " +
            ////    byaSaveTileNum[1,5] + " " + byaSaveTileNum[1,6] + " " + byaSaveTileNum[1,7] + " " + byaSaveTileNum[1,8], byaSaveTileNum[1,9]);
            ////Debug.LogWarningFormat("MahjongHelper::JudgeNormalWin 索总张数[%d]：" + byaSaveTileNum[2,0] + " " + byaSaveTileNum[2,1] + " " + byaSaveTileNum[2,2] + " " + byaSaveTileNum[2,3] + " " + byaSaveTileNum[2,4] + " " +
            //////   byaSaveTileNum[2,5] + " " + byaSaveTileNum[2,6] + " " + byaSaveTileNum[2,7] + " " + byaSaveTileNum[2,8] + " " + byaSaveTileNum[2,9]);
            ////Debug.LogWarningFormat("MahjongHelper::JudgeNormalWin 风总张数[%d]：" + byaSaveTileNum[3,0] + " " + byaSaveTileNum[3,1] + " " + byaSaveTileNum[3,2] + " " + byaSaveTileNum[3,3] + " " + byaSaveTileNum[3,4] + " " +
            ////   byaSaveTileNum[3,5] + " " + byaSaveTileNum[3,6] + " " + byaSaveTileNum[3,7] + " " + byaSaveTileNum[3,8] + " " + byaSaveTileNum[3,9]);
            ////Debug.LogWarningFormat("MahjongHelper::JudgeNormalWin 箭总张数[%d]：" + byaSaveTileNum[4,0] + " " + byaSaveTileNum[4,1] + " " + byaSaveTileNum[4,2] + " " + byaSaveTileNum[4,3] + " " + byaSaveTileNum[4,4] + " " +
            ////   byaSaveTileNum[4,5] + " " + byaSaveTileNum[4,6] + " " + byaSaveTileNum[4,7] + " " + byaSaveTileNum[4,8] + " " + byaSaveTileNum[4,9]);


            //// 将剩余的牌加入数组中
            //for (bySuit = SUIT_CHARACTER; bySuit < SUIT_FLOWER; bySuit++)
            //{
            //    if (byaSaveTileNum[bySuit - 1, 0] == 0)
            //    {
            //        continue;
            //    }
            //    for (byValue = 1; byValue < 10; byValue++)
            //    {
            //        if (byaSaveTileNum[bySuit - 1, byValue] > 0)
            //        {
            //            byaSaveTileNum[bySuit - 1, byValue]--;
            //            byaSaveTileNum[bySuit - 1, 0]--;
            //            byaTiles[byRemainingNum] = (byte)(bySuit << 4 | byValue);
            //            byRemainingNum++;
            //            byValue--;
            //        }
            //    }
            //}
            ////分析需要癞子的牌
            //for (int i = 0; i < byLaiziNum + 1; i++)
            //{
            //    bySuit = (byte)(byaTiles[byIndex] >> 4);
            //    byValue = (byte)(byaTiles[byIndex] & 0x0F);
            //    if (byRemainingNum == 0)// 剩下的牌为 0
            //    {
            //        // 癞子总数减去已使用后数为 3 并且 将牌已确定  （这里就组成 红中刻子）(可以胡牌 直接结束)
            //        if (byLaiziNum - byEmployLaiZiNum == 3 && bJong)
            //        {
            //            bSuccess = true;
            //            byaSaveTileNum[byLaiziSuit - 1, 0] += byLaiziNum;
            //            byaSaveTileNum[byLaiziSuit - 1, byLaiziValue] += byLaiziNum;
            //            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, byLaiziSuit - 1);
            //            AddResult(byLaiziSuit, byLaiziValue, byLaiziSuit, byLaiziValue, 1, false);
            //            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, byLaiziSuit - 1);
            //            break;
            //        }
            //        // 癞子总数减去已使用后数为2 并且 将牌没有确定 （这里就组成 将牌 （红中））（可以胡牌 直接结束）
            //        else if (byLaiziNum - byEmployLaiZiNum == 2 && !bJong)
            //        {
            //            bJong = true;
            //            bSuccess = true;
            //            byaSaveTileNum[byLaiziSuit - 1, 0] += 2;
            //            byaSaveTileNum[byLaiziSuit - 1, byLaiziValue] += 2;
            //            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, byLaiziSuit - 1);
            //            AddResult(byLaiziSuit, byLaiziValue, byLaiziSuit, byLaiziValue, 3, false);
            //            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, byLaiziSuit - 1);
            //            break;
            //        }
            //        else if (byLaiziNum - byEmployLaiZiNum != 0) //  癞子总数减去已使用后数不为 0 直接退出 没有胡牌
            //        {
            //            Debug.LogError("MahjongHelper::JudgeNormalWin C");
            //            return false;
            //        }
            //    }
            //    else if (byRemainingNum == 1)
            //    {
            //        // 癞子总数减去已使用后数为 1 并且将牌没有确定 （这里就组成将牌） （可以胡牌 直接结束）
            //        if (byLaiziNum - byEmployLaiZiNum == 1 && !bJong)
            //        {
            //            bJong = true;
            //            bSuccess = true;
            //            byChangeValue = byValue;
            //            byaSaveTileNum[bySuit - 1, 0] += 1;
            //            byaSaveTileNum[bySuit - 1, byValue] += 1;
            //            //增加结果信息
            //            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, true, bySuit, byChangeValue, 1);
            //            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //            byEmployLaiZiNum += 1;
            //            byRemainingNum -= 1;
            //            break;
            //        }
            //        // 癞子总数减去已使用后数为 2 并且将牌已确定 （这里就组成刻子） （可以胡牌 直接结束）
            //        else if (byLaiziNum - byEmployLaiZiNum == 2 && bJong)
            //        {
            //            bSuccess = true;
            //            byChangeValue = byValue;
            //            byaSaveTileNum[bySuit - 1, 0] += 1;
            //            byaSaveTileNum[bySuit - 1, byValue] += 1;
            //            //增加结果信息
            //            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 1, true, bySuit, byChangeValue, 2);
            //            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //            byEmployLaiZiNum += 2;
            //            byRemainingNum -= 1;
            //            break;
            //        }
            //        else
            //        {
            //          //  Debug.LogError("MahjongHelper::JudgeNormalWin D");
            //            return false;
            //        }
            //    }
            //    // 剩下的牌数为 2
            //    else if (byRemainingNum == 2)
            //    {
            //        if (byaTiles[byIndex] == byaTiles[byIndex + 1] && !bJong)
            //        {
            //            bJong = true;
            //            bSuccess = true;
            //            byaSaveTileNum[bySuit - 1, 0] += 2;
            //            byaSaveTileNum[bySuit - 1, byValue] += 2;
            //            //增加结果信息
            //            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, false);
            //            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //            byRemainingNum -= 2;
            //            byIndex += 2;
            //        }
            //        // 癞子总数减去已使用后数大于0 如果两张相同 并且有将牌 （这里就组成刻字牌）
            //        else if (byLaiziNum - byEmployLaiZiNum > 0 && byaTiles[byIndex] == byaTiles[byIndex + 1] && bJong)
            //        {
            //            bSuccess = true;
            //            byChangeValue = byValue;
            //            byaSaveTileNum[bySuit - 1, 0] += 2;
            //            byaSaveTileNum[bySuit - 1, byValue] += 2;
            //            //增加结果信息
            //            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 1, true, bySuit, byChangeValue, 1);
            //            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //            byRemainingNum -= 2;
            //            byIndex += 2;
            //            byEmployLaiZiNum += 1;
            //        }
            //        /*else if (byaTiles[byIndex] == byaTiles[byIndex + 1] && !bJong)
            //            {
            //            bJong = true;
            //            bSuccess = true;
            //            byaSaveTileNum[bySuit - 1,0] += 2;
            //            byaSaveTileNum[bySuit - 1,byValue] += 2;
            //            AddResult(resultType, byaSaveTileNum[bySuit - 1], bySuit,byValue, 3, false);
            //            byRemainingNum -= 2;
            //            byIndex += 2;
            //            break;
            //            }*/
            //        else if (byaTiles[byIndex + 1] - byaTiles[byIndex] < 0x03 && bySuit < SUIT_WIND)
            //        {
            //            bool bHu = false;
            //            if (byLaiziNum - byEmployLaiZiNum == 1 && bJong) // 癞子总数减去已使用后数为 2 并且将牌已确定 可以胡牌
            //            {
            //                bSuccess = true;
            //                bHu = true;
            //            }
            //            //增加结果信息
            //            byChangeValue = (byte)(byValue + 0x03 - (byaTiles[byIndex + 1] - byaTiles[byIndex]));
            //            byaSaveTileNum[bySuit - 1, 0] += 2;
            //            byaSaveTileNum[bySuit - 1, byValue] += 1;
            //            byaSaveTileNum[bySuit - 1, byValue + 1] += 1;
            //            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 2, true, bySuit, byChangeValue, 1);
            //            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //            byRemainingNum -= 2;
            //            byIndex += 2;
            //            byEmployLaiZiNum += 1;
            //            if (bHu) // 可以胡牌直接結束
            //            {
            //                break;
            //            }
            //        }
            //        else if (byLaiziNum - byEmployLaiZiNum == 3 && !bJong) // 癞子总数减去已使用后数为 3  并且将牌没有确定 （这里就组成顺子）
            //        {
            //            byChangeValue = byValue;
            //            byRemainingNum -= 1;
            //            byaSaveTileNum[bySuit - 1, 0] += 1;
            //            byaSaveTileNum[bySuit - 1, byValue] += 1;
            //            byIndex++;
            //            //增加结果信息
            //            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //            AddResult(byLaiziSuit, byLaiziValue, bySuit, (byte)(byValue == 8 ? byValue - 1 : byValue), 2, true, bySuit, byChangeValue, 2);
            //            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //            byEmployLaiZiNum += 2;
            //        }
            //        else
            //        {
            //            // Debug.LogError("MahjongHelper::JudgeNormalWin E");
            //            return false;
            //        }
            //    }
            //    else if (byRemainingNum > 2)
            //    {
            //        if (bJong && byJongSuit == bySuit && byJongValue == byValue)
            //        {
            //            //增加结果信息
            //            byaSaveTileNum[bySuit - 1, 0] += 2;
            //            byaSaveTileNum[bySuit - 1, byValue] += 2;
            //            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, false);
            //            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //            byRemainingNum -= 2;
            //            byIndex += 2;
            //        }
            //        //  剩下的牌减去2张将加上剩余的癞子数大于2  并且两张牌相同 将牌没有确定 （这里就组成将牌）
            //        else if (((byRemainingNum - 2) + (byLaiziNum - byEmployLaiZiNum)) > 2 && byaTiles[byIndex] == byaTiles[byIndex + 1] && !bJong)
            //        {
            //            bJong = true;
            //            //增加结果信息
            //            byaSaveTileNum[bySuit - 1, 0] += 2;
            //            byaSaveTileNum[bySuit - 1, byValue] += 2;
            //            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, false);
            //            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);

            //            byRemainingNum -= 2;
            //            byIndex += 2;
            //        }
            //        // 癞子总数减去已使用后数大于 0 并且将牌已确定 （这里就组成刻子）
            //        else if (byLaiziNum - (byEmployLaiZiNum + 1) > 0 && byaTiles[byIndex] == byaTiles[byIndex + 1] && bJong)
            //        {
            //            //增加结果信息
            //            byChangeValue = byValue;
            //            byaSaveTileNum[bySuit - 1, 0] += 2;
            //            byaSaveTileNum[bySuit - 1, byValue] += 2;
            //            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 1, true, bySuit, byChangeValue, 1);
            //            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //            byRemainingNum -= 2;
            //            byIndex += 2;
            //            byEmployLaiZiNum += 1;
            //        }
            //        // 癞子总数减去已使用后数大于 0 后面的牌值减前面的牌值 小于 3 并且是序数牌  (这里组成顺子)
            //        else if (byLaiziNum - (byEmployLaiZiNum + 1) > 0 && byaTiles[byIndex + 1] - byaTiles[byIndex] < 0x03 && bySuit < SUIT_WIND)
            //        {
            //            //增加结果信息
            //            byChangeValue = (byte)(byValue + 0x03 - (byaTiles[byIndex + 1] - byaTiles[byIndex]));
            //            byaSaveTileNum[bySuit - 1, 0] += 2;
            //            byaSaveTileNum[bySuit - 1, byValue] += 1;
            //            byaSaveTileNum[bySuit - 1, byValue + 1] += 1;
            //            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 2, true, bySuit, byChangeValue, 1);
            //            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //            byRemainingNum -= 2;
            //            byIndex += 2;
            //            byEmployLaiZiNum += 1;
            //        }
            //        else if (byRemainingNum - 2 == 1 && byLaiziNum - byEmployLaiZiNum == 2 && !bJong) // 剩下的牌加剩下的癞子牌一共5张 ，剩下的牌减去2张将牌等于 1 癞子总数减去已使用后数等于2 并且将牌没有确定
            //        {
            //            bool flag = false;
            //            for (int k = 0; k < 13; k++)
            //            {
            //                if (byaTiles[k] > 0 && byaTiles[k] == byaTiles[k + 1]) // 找到两张相同的牌
            //                {
            //                    bJong = true;
            //                    //增加结果信息
            //                    bySuit = (byte)(byaTiles[k] >> 4);
            //                    byValue = (byte)(byaTiles[k] & 0x0F);
            //                    byaSaveTileNum[bySuit - 1, 0] += 2;
            //                    byaSaveTileNum[bySuit - 1, byValue] += 2;
            //                    byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //                    AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, false);
            //                    SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //                    byRemainingNum -= 2;
            //                    flag = true;
            //                    break;
            //                }
            //            }
            //            if (!flag && ((byRemainingNum - 1) + (byLaiziNum - (byEmployLaiZiNum + 1))) > 2 && !bJong) // 剩下的牌减去1张将加上剩余的癞子数大于2 并且将牌没有确定 （这里组成将牌）
            //            {
            //                bJong = true;
            //                byChangeValue = byValue;
            //                byaSaveTileNum[bySuit - 1, 0] += 1;
            //                byaSaveTileNum[bySuit - 1, byValue] += 1;
            //                byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //                AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, true, bySuit, byChangeValue, 1);
            //                SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //                byRemainingNum--;
            //                byIndex++;
            //                byEmployLaiZiNum += 1;
            //            }
            //            else if (flag)
            //            {
            //                //TODO 这里不能直接结束  还需要继续组合
            //            }
            //            else
            //            {
            //                // Debug.LogError("MahjongHelper::JudgeNormalWin  F111");
            //                return false;
            //            }
            //        }
            //        else
            //        {
            //            if ((byRemainingNum - 2) == 1 && (byLaiziNum - byEmployLaiZiNum == 2) && !bJong) // 剩下的牌加剩下的癞子牌一共5张 ，剩下的牌减去2张将牌等于 1 癞子总数减去已使用后数等于2 并且将牌没有确定
            //            {
            //                for (int k = 0; k < 13; k++)
            //                {
            //                    if (byaTiles[k] > 0 && byaTiles[k] == byaTiles[k + 1]) // 找到两张相同的牌
            //                    {
            //                        bJong = true;
            //                        //增加结果信息
            //                        bySuit = (byte)(byaTiles[k] >> 4);
            //                        byValue = (byte)(byaTiles[k] & 0x0F);
            //                        byaSaveTileNum[bySuit - 1, 0] += 2;
            //                        byaSaveTileNum[bySuit - 1, byValue] += 2;
            //                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //                        AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, false);
            //                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //                        byRemainingNum -= 2;
            //                        i--;
            //                        break;
            //                    }
            //                }
            //            }// 剩下的牌减去1张将加上剩余的癞子数大于2 并且将牌没有确定 （这里组成将牌）
            //            else if (((byRemainingNum - 1) + (byLaiziNum - (byEmployLaiZiNum + 1))) > 2 && !bJong)
            //            {
            //                if ((byaTiles[byIndex + 1] - byaTiles[byIndex]) < 0x03 && bySuit < SUIT_WIND)
            //                {
            //                    //增加结果信息
            //                    byChangeValue = (byte)(byValue + 0x03 - (byaTiles[byIndex + 1] - byaTiles[byIndex]));
            //                    byaSaveTileNum[bySuit - 1, 0] += 2;
            //                    byaSaveTileNum[bySuit - 1, byValue] += 1;
            //                    byaSaveTileNum[bySuit - 1, byValue + 1] += 1;
            //                    byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //                    AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 2, true, bySuit, byChangeValue, 1);
            //                    SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //                    byRemainingNum -= 2;
            //                    byIndex += 2;
            //                    byEmployLaiZiNum += 1;
            //                }
            //                else
            //                {
            //                    bJong = true;
            //                    byChangeValue = byValue;
            //                    byaSaveTileNum[bySuit - 1, 0] += 1;
            //                    byaSaveTileNum[bySuit - 1, byValue] += 1;
            //                    byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //                    AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, true, bySuit, byChangeValue, 1);
            //                    SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //                    byRemainingNum--;
            //                    byIndex++;
            //                    byEmployLaiZiNum += 1;
            //                }
            //            }
            //            else if (byRemainingNum == 3 && (byLaiziNum - byEmployLaiZiNum) > 1 && (byaTiles[byIndex + 1] - byaTiles[byIndex] < 0x03 || byaTiles[byIndex + 2] - byaTiles[byIndex + 1] < 0x03))
            //            {
            //                if (byaTiles[byIndex + 2] - byaTiles[byIndex + 1] < 0x03)
            //                {
            //                    bySuit = (byte)(byaTiles[byIndex + 1] >> 4);
            //                    byValue = (byte)(byaTiles[byIndex + 1] & 0x0F);
            //                    byChangeValue = (byte)((byValue + 0x03) - (byaTiles[byIndex + 2] - byaTiles[byIndex + 1]));
            //                }
            //                else
            //                {
            //                    byChangeValue = (byte)((byValue + 0x03) - (byaTiles[byIndex + 1] - byaTiles[byIndex]));
            //                    byIndex++;
            //                }
            //                byaSaveTileNum[bySuit - 1, 0] += 2;
            //                byaSaveTileNum[bySuit - 1, byValue] += 1;
            //                byaSaveTileNum[bySuit - 1, byValue + 1] += 1;
            //                byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //                AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 2, true, bySuit, byChangeValue, 1);
            //                SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //                byRemainingNum -= 2;
            //                byEmployLaiZiNum += 1;
            //            }
            //            else if (byRemainingNum == 4 && byIndex < 10 && ((byaTiles[byIndex] == byaTiles[byIndex + 1] && byaTiles[byIndex + 2] == byaTiles[byIndex + 3]) || (byaTiles[byIndex] == byaTiles[byIndex + 1] || byaTiles[byIndex + 2] == byaTiles[byIndex + 3])) && bJong)
            //            {
            //                if (byaTiles[byIndex] == byaTiles[byIndex + 1] && byaTiles[byIndex + 2] == byaTiles[byIndex + 3])
            //                {                    
            //                    //增加结果信息
            //                    byaSaveTileNum[bySuit - 1, 0] += 2;
            //                    byaSaveTileNum[bySuit - 1, byValue] += 2;
            //                    byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //                    AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, false);
            //                    SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //                    byRemainingNum -= 2;
            //                    byIndex += 2;
            //                }
            //                else
            //                {
            //                    if (byaTiles[byIndex] == byaTiles[byIndex + 1])
            //                    {                             
            //                        //增加结果信息
            //                        byaSaveTileNum[bySuit - 1, 0] += 2;
            //                        byaSaveTileNum[bySuit - 1, byValue] += 2;
            //                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //                        AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, false);
            //                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //                        byRemainingNum -= 2;
            //                        byIndex += 2;
            //                    }
            //                    else
            //                    {
            //                        bySuit = (byte)(byaTiles[byIndex + 2] >> 4);
            //                        byValue = (byte)(byaTiles[byIndex + 2] & 0x0F);
            //                        //增加结果信息
            //                        byaSaveTileNum[bySuit - 1, 0] += 2;
            //                        byaSaveTileNum[bySuit - 1, byValue] += 2;
            //                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            //                        AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, false);
            //                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            //                        byRemainingNum -= 2;
            //                    }
            //                }   
            //            }
            //            else
            //            {
            //                //Debug.Log("MahjongHelper::JudgeNormalWin G");
            //                return false;
            //            }
            //        }
            //    }
            //}

            //if (bJong == false)
            //{
            //    //Debug.LogError("MahjongHelper::JudgeNormalWin 没有找到将牌");
            //    bSuccess = false;
            //}
            //for (bySuit = SUIT_CHARACTER; bySuit < SUIT_FLOWER; bySuit++)
            //{
            //    if (byaSaveTileNum[bySuit - 1, 0] > 0)
            //    {
            //        bSuccess = false;
            //        //Debug.LogError("MahjongHelper::JudgeNormalWin 分析牌失败");
            //        break;
            //    }
            //}
            //// Debug.LogWarning("MahjongHelper::JudgeNormalWinb Success=" + bSuccess + "bJong= " + bJong);
            //return bSuccess;
       // }
        bool AnalyzeSuit(bool bHonorTile, byte bySuit, bool isSaveCardMessage = false)
        {
            //if (bySuit == 1)
            //{
            //    Debug.LogError("wwwvalue:" + byaSuitTileNum[0] + "," + byaSuitTileNum[1] + "," + byaSuitTileNum[2] + "," + byaSuitTileNum[3] + "," + byaSuitTileNum[4] + "," + byaSuitTileNum[5] + "," + byaSuitTileNum[6]
            //        + "," + byaSuitTileNum[7] + "," + byaSuitTileNum[8] + "," + byaSuitTileNum[9]);
            //}
            if (bySuit > 3)
            {
                bHonorTile = true;
            }

            if (byaSuitTileNum[0] == 0)
            {
                return true;
            }

            //寻找最小的值
            byte byValue = 0;

            for (byValue = 1; byValue < 10; byValue++)
            {
                if (byaSuitTileNum[byValue] > 0)
                {
                    break;
                }
            }

            //能否分析成功
            bool bSuccess = false;

            //分析刻子
            if (byaSuitTileNum[byValue] > 2)
            {
                //出去刻子的3张牌
                byaSuitTileNum[byValue] -= 3;
                byaSuitTileNum[0] -= 3;

                if (isSaveCardMessage)
                {
                    //添加玩家的刻子信息
                    MahjongGame_AH.MahjongHelper.TripletTypeDef seq = new MahjongGame_AH.MahjongHelper.TripletTypeDef();
                    seq.bySuit = bySuit;
                    seq.byValue = byValue;
                    resultType.tripletType.Add(seq);
                }

                //分析剩余的牌
                bSuccess = AnalyzeSuit(bHonorTile, bySuit, isSaveCardMessage);

                //还原刻子的三张牌
                byaSuitTileNum[byValue] += 3;
                byaSuitTileNum[0] += 3;

                return bSuccess;
            }

            //分析顺子
            if (!bHonorTile && byValue < 8 && (byaSuitTileNum[byValue + 1] > 0) && (byaSuitTileNum[byValue + 2] > 0))
            {
                //是否限制3以下序数牌成牌,0不限值，1限制
                //if (GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byUnderThreeLimit == 1)
                //{
                //    if (byValue < 4)
                //    {
                //        return false;
                //    }
                //}

                //出去三张顺子的3张牌
                byaSuitTileNum[byValue]--;
                byaSuitTileNum[byValue + 1]--;
                byaSuitTileNum[byValue + 2]--;
                byaSuitTileNum[0] -= 3;


                //分析剩余的牌
                bSuccess = AnalyzeSuit(bHonorTile, bySuit, isSaveCardMessage);

                if (isSaveCardMessage)
                {
                    //添加玩家的顺子信息,如果列表已经包含该顺子信息，则不会继续添加   
                    MahjongGame_AH.MahjongHelper.SequenceTypeDef seq = new MahjongGame_AH.MahjongHelper.SequenceTypeDef();
                    seq.bySuit = bySuit;
                    seq.byFirstValue = byValue;
                    resultType.sequenceType.Add(seq);
                }

                //还原顺子的3张牌
                byaSuitTileNum[byValue]++;
                byaSuitTileNum[byValue + 1]++;
                byaSuitTileNum[byValue + 2]++;
                byaSuitTileNum[0] += 3;
                return bSuccess;
            }

            return false;
        }
        /**
     * 分析一种花色的牌，分解为刻和顺的组合
     * @param byaSuitTileNum 一种花色的牌各个值的张数
     * @param bySuit 花色：1万牌、2筒牌、3索牌、4风牌、5箭牌
     * @param byLaiziSuit 癞子花色
     * @param byLaiziValue 癞子牌值
     */
        byte[] byaSuitTileNum = new byte[10];
        void AnalyzeSuit(byte bySuit, byte byLaiziSuit, byte byLaiziValue, int Methord)
        {

            //  Debug.Log("==================11byaSuitTileNum：" + byaSuitTileNum[0] + "," + byaSuitTileNum[1]);
            if (byaSuitTileNum[0] == 0) // 这种花色总数为0
            {
                return;
            }
            // 寻找最小的值
            byte byValue = 0;
            for (byValue = 1; byValue < 10; byValue++) // 各个值
            {
                if (byaSuitTileNum[byValue] > 0)
                {
                    //     Debug.Log("==================11111 bySuit：" + bySuit);
                    break;
                }
            }

            // 分析刻子
            if (byaSuitTileNum[byValue] > 2)
            {
                //增加结果信息
                AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 1, false);

                // 分析剩余的牌
                AnalyzeSuit(bySuit, byLaiziSuit, byLaiziValue, 20001);
                //  Debug.Log("==================22byaSuitTileNum：" + byaSuitTileNum[0] + "," + byaSuitTileNum[1]);
                return;
            }
            else if (bySuit < SUIT_WIND && byValue < 8 && ((byaSuitTileNum[byValue] == 2 && byaSuitTileNum[byValue + 1] == 1 && byaSuitTileNum[byValue + 2] == 2) || (byaSuitTileNum[byValue] == 1 && byaSuitTileNum[byValue + 1] == 2 && byaSuitTileNum[byValue + 2] == 2) || (byaSuitTileNum[byValue] == 2 && byaSuitTileNum[byValue + 1] == 2 && byaSuitTileNum[byValue + 2] == 1)))
            {
                //增加结果信息
                AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 2, false);

                // 分析剩余的牌
                AnalyzeSuit(bySuit, byLaiziSuit, byLaiziValue, 20001);

                return;
            }
            else if (bySuit < SUIT_WIND && byValue < 8 && byaSuitTileNum[byValue] == 2 && byaSuitTileNum[byValue + 1] > 1 && byaSuitTileNum[byValue + 2] > 1)
            {
                //增加结果信息
                AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 2, false);
                //增加结果信息
                AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 2, false);
                //   Debug.Log("==================33byaSuitTileNum：" + byaSuitTileNum[0] + "," + byaSuitTileNum[1]);
                // 分析剩余的牌
                AnalyzeSuit(bySuit, byLaiziSuit, byLaiziValue, 20001);

                return;
            }
            else if (bySuit < SUIT_WIND && byValue < 5 && byaSuitTileNum[byValue] > 0 && byaSuitTileNum[byValue + 1] == 1 && byaSuitTileNum[byValue + 2] > 0 && byaSuitTileNum[byValue + 3] > 0 && byaSuitTileNum[byValue + 4] > 0 && byaSuitTileNum[byValue + 5] > 0)
            {
                //增加结果信息
                AddResult(byLaiziSuit, byLaiziValue, bySuit, (byte)(byValue + 3), 2, false);

                // 分析剩余的牌
                AnalyzeSuit(bySuit, byLaiziSuit, byLaiziValue, 20001);
                return;
            }
            // 分析顺子
            else if (bySuit < SUIT_WIND && byValue < 8 && byaSuitTileNum[byValue] == 1 && byaSuitTileNum[byValue + 1] > 0 && byaSuitTileNum[byValue + 2] > 0)
            {
                if (bySuit < SUIT_WIND && byValue < 5 && byaSuitTileNum[byValue] == 1 && byaSuitTileNum[byValue + 1] == 1 && byaSuitTileNum[byValue + 2] == 1 && byaSuitTileNum[byValue + 3] == 1 && byaSuitTileNum[byValue + 4] == 1)
                {
                    //增加结果信息
                    AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 2, false);
                    //     Debug.Log("==================44byaSuitTileNum：" + byaSuitTileNum[0] + "," + byaSuitTileNum[1]);
                    // 分析剩余的牌
                    AnalyzeSuit(bySuit, byLaiziSuit, byLaiziValue, 20001);

                    return;
                }
                else

                if (byValue < 7 && byaSuitTileNum[byValue + 1] == 1 && byaSuitTileNum[byValue + 2] > 0 && byaSuitTileNum[byValue + 3] > 0 && byaSuitTileNum[byValue + 2] != 2)
                {
                    //增加结果信息
                    AddResult(byLaiziSuit, byLaiziValue, bySuit, (byte)(byValue + 1), 2, false);

                    // 分析剩余的牌
                    AnalyzeSuit(bySuit, byLaiziSuit, byLaiziValue, 20001);

                    return;
                }
                else
                {
                    //增加结果信息
                    AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 2, false);
                    //     Debug.Log("==================44byaSuitTileNum：" + byaSuitTileNum[0] + "," + byaSuitTileNum[1]);
                    // 分析剩余的牌
                    AnalyzeSuit(bySuit, byLaiziSuit, byLaiziValue, 20001);

                    return;
                }   
            }
            else if (byValue < 10)
            {
                byte byTileNum = 0;
                byTileNum = byaSuitTileNum[byValue];
                byaSuitTileNum[byValue] = 0;
                byaSuitTileNum[0] -= byTileNum;

                //  Debug.Log("==================55byaSuitTileNum：" + byaSuitTileNum[0] + "," + byaSuitTileNum[1]);
                // 分析剩余的牌
                AnalyzeSuit(bySuit, byLaiziSuit, byLaiziValue, 20001);
                byaSuitTileNum[byValue] += byTileNum;
                byaSuitTileNum[0] += byTileNum;
            }
            // Debug.Log("==================66byaSuitTileNum：" + byaSuitTileNum[0] + "," + byaSuitTileNum[1]);
            return;
        }
        /**
         * 去除手牌并增加结果信息
         * @param byaSuitTileNum 一种花色的牌各个值的张数
         * @param byLaiziSuit 癞子花色
         * @param byLaiziValue 癞子牌值
         * @param bySuit 添加的花色
         * @param byValue 添加的牌值：刻子为牌值，顺子为第一个的牌值
         * @param byChangeType 变化状态：1刻子，2顺子，3将牌
         * @param byLaizi 是否有癞子
         * @param byChangeSuit 变化花色
         * @param byChangeValue 变化牌值
         * @param byUseLaiziNum 使用癞子的数量
         * @return 无
         */
        void AddResult(byte byLaiziSuit, byte byLaiziValue, byte bySuit, byte byValue, byte byChangeType, bool byLaizi, byte byChangeSuit = 0, byte byChangeValue = 0, byte byUseLaiziNum = 0)
        {
            // Debug.LogError(byChangeValue);
            if (byChangeValue == 10 && byValue == 8)
            {
                byChangeValue = 7;
                byValue = 7;
            }
            if (byLaizi)
            {
                for (byte i = 0; i < byUseLaiziNum; i++)
                {
                    byaSuitTileNum[byChangeValue]++;
                    byaSuitTileNum[0]++;
                }
            }
            switch (byChangeType)
            {
                case 0:
                    break;
                case 1:
                    // 除去刻子的3张牌
                    byaSuitTileNum[byValue] -= 3;
                    byaSuitTileNum[0] -= 3;
                    break;
                case 2:
                    // 除去顺子的3张牌
                    byaSuitTileNum[byValue]--;
                    byaSuitTileNum[byValue + 1]--;
                    byaSuitTileNum[byValue + 2]--;
                    byaSuitTileNum[0] -= 3;
                    break;
                case 3:
                    // 除去将牌的2张牌
                    byaSuitTileNum[byValue] -= 2;
                    byaSuitTileNum[0] -= 2;
                    break;
                default:
                    break;
            }
        }

        byte[] DobouleByte2Single(byte[,] value, int n)
        {
            byte[] single = new byte[value.GetLength(1)];
            //  single = value.GetLongLength(n );
            for (int i = 0; i < single.Length; i++)
            {
                single[i] = value[n, i];
            }
            return single;
        }
        void SingleByte2Doboule(ref byte[,] value, byte[] single, int row)
        {
            for (int i = 0; i < value.GetLength(1); i++)
            {
                value[row, i] = single[i];
            }
        }
    }
}
