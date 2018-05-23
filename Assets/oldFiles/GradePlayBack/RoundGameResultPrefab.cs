using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using XLua;
using anhui;
namespace PlayBack_1
{
    [Hotfix]
    [LuaCallCSharp]
    public class RoundGameResultPrefab : MonoBehaviour
    {
        public int winSeat = 0;  //赢家座位号
        int iDrawSeatNum = 0; //自摸座位号
        public RawImage headImage;  //头像
        public Text NickName;  //昵称
        public GameObject playerCard;  //手牌的父物体
        public Image winMessage; //明暗杠的具体信息
        public Text winMessage_text;  //明暗杠的具体显示文字
        public Text PlayerScore;  //玩家的分数 
        public Image Banker; //庄家的标志图片
        public GameObject initPos;  //单张麻将初始位置
        public GameObject initPos_special;  //特殊麻将的初始位置
        public Font[] font;  //0表示金色字体，1表示绿色字体
        //单局结算
        public Image winMessage_Image;  //赢牌信息
        public Sprite[] image_round;  //0胡1自摸2放炮
        [HideInInspector]
        public int iseatNum;  //玩家座位号
        public int iMingGongNum;  //明杠的数量
        public int iAnGangGongNum; //暗杠的数量
        bool bSelfWin;  //是否自摸的标志位



        /// <summary>
        /// 更新界面的玩家信息
        /// </summary>
        /// <param name="index"></param>
        public void MessageVlaue()
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;

            anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(headImage, pbd.sHeadUrl[iseatNum - 1]);
            NickName.text = pbd.sName[iseatNum - 1];
            if (pbd.bResultPoint[iseatNum - 1] > 0)
            {
                PlayerScore.font = font[0];
                PlayerScore.text = "+" + pbd.bResultPoint[iseatNum - 1].ToString();
            }
            else if (pbd.bResultPoint[iseatNum - 1] < 0)
            {
                PlayerScore.font = font[1];
                PlayerScore.text = pbd.bResultPoint[iseatNum - 1].ToString();
            }
            else
            {
                PlayerScore.font = font[1];
                PlayerScore.text = pbd.bResultPoint[iseatNum - 1].ToString();
            }
            StringBuilder str = new StringBuilder();

            //确定是否显示庄家标志图片
            if (iseatNum == pbd.byDealerSeat)
            {
                Banker.gameObject.SetActive(true);
            }
            else
            {
                Banker.gameObject.SetActive(false);
            }


            #region 处理明暗杠
            //获取明杠暗杠数量
            for (int i = 0; i < pbd.resultType[iseatNum - 1].byTripletNum; i++)
            {
                if (pbd.resultType[iseatNum - 1].tripletType[i].byPongKongType == 2)
                {
                    iMingGongNum++;
                }
                else if (pbd.resultType[iseatNum - 1].tripletType[i].byPongKongType == 3)
                {
                    iAnGangGongNum++;
                }

            }

            //添加明杠
            if (pbd.resultType[iseatNum - 1].cExposedKongPoint != 0)
            {
                str.Append("明杠");
                int point = pbd.resultType[iseatNum - 1].cExposedKongPoint;
                if (point > 0)
                {
                    str.Append("+");
                }

                //添加被杠，额外扣分
                if (pbd.playingMethodConf.byFanExtraExposedExtra > 0)
                {
                    str.Append(point);
                }
                else
                {
                    str.Append(point);
                }

                str.Append("    ");
            }

            //添加暗杠
            if (pbd.resultType[iseatNum - 1].cConcealedKongPoint != 0)
            {
                str.Append("暗杠");
                if (pbd.resultType[iseatNum - 1].cConcealedKongPoint > 0)
                {
                    str.Append("+");
                }
                str.Append(pbd.resultType[iseatNum - 1].cConcealedKongPoint);
                str.Append("    ");
            }
            #endregion

            int winCount_ = 0; //赢家的数量

            //添加自摸
            for (int i = 0; i < 4; i++)
            {
                // Debug.LogWarning(i + "自摸检测byaWinSrat[i]" + pbd.byaWinSrat[i]);
                if (pbd.byaWinSrat[i] > 0)
                {
                    winCount_++;
                    winSeat = i + 1;
                    iDrawSeatNum = i + 1;
                }
            }
            //判断一局结束是否是自摸  没有放炮  有赢家
            if (winCount_ > 0 && pbd.byShootSeat <= 0)
            {
                bSelfWin = true;
            }
            else
            {
                iDrawSeatNum = 0;
            }

            if (iDrawSeatNum == iseatNum)
            {
                winMessage_Image.gameObject.SetActive(true);
                winMessage_Image.sprite = image_round[1];
            }

            //判断自己接炮胡
            if (pbd.byaWinSrat[iseatNum - 1] > 0 && !bSelfWin)
            {
                winMessage_Image.gameObject.SetActive(true);
                winMessage_Image.sprite = image_round[0];
            }

            //如果玩家点炮显示点炮信息
            if (pbd.byShootSeat == iseatNum)
            {
                winMessage_Image.gameObject.SetActive(true);
                winMessage_Image.sprite = image_round[2];
            }


            int iKongWin = 0;
            //判断是否荒庄
            for (int i = 0; i < 4; i++)
            {
                // Debug.LogError(i + "pbd.byaWinSrat[i]" + pbd.byaWinSrat[i]);
                if (pbd.byaWinSrat[i] > 0)
                {
                    iKongWin++;
                }
            }


            //判断抢杠胡收几家
            bool isRobbingWin = false;
            if (iDrawSeatNum == 0 && pbd.playingMethodConf.byFanRobbingKongWin > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (pbd.byaWinSrat[i] > 0 && pbd.resultType[i].lastTile.byRobbingKong == 1)
                    {
                        isRobbingWin = true;
                    }
                }
            }

            //如果不是荒庄 显示底分倍数
            if (iKongWin > 0)
            {

                str.Append(ShowSpecialWinType(iseatNum - 1, pbd.caFanResult));

                if (iDrawSeatNum > 0)//自摸
                {

                }
                else//放炮
                {
                    #region 放炮
                    //一炮多响  放炮收三家
                    //if (pbd.playingMethodConf.byWinPointMultiPay > 0 || pbd.playingMethodConf.byWinPointMultiShoot > 0)
                    //{
                    //    if (iseatNum == pbd.byShootSeat)
                    //    {
                    //        str.Append(ShowSpecialWinType(iseatNum - 1, pbd.caFanResult));
                    //        for (int i = 0; i < 4; i++)
                    //        {
                    //            if (pbd.byaWinSrat[i] > 0)
                    //            {
                    //                str.Append(ShowSpecialWinType(i, pbd.caFanResult));
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (pbd.byaWinSrat[iseatNum - 1] > 0)
                    //        {
                    //            str.Append(ShowSpecialWinType(iseatNum - 1, pbd.caFanResult));
                    //        }
                    //        else
                    //        {
                    //            for (int i = 0; i < 4; i++)
                    //            {
                    //                if (pbd.byaWinSrat[i] > 0)
                    //                {
                    //                    str.Append(ShowSpecialWinType(i, pbd.caFanResult));
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}//放炮收一家
                    //else
                    //{
                    //    if (pbd.byaWinSrat[iseatNum - 1] > 0)
                    //    {
                    //        str.Append(ShowSpecialWinType(iseatNum - 1, pbd.caFanResult));
                    //    }

                    //    if (iseatNum == pbd.byShootSeat)
                    //    {
                    //        for (int i = 0; i < 4; i++)
                    //        {
                    //            if (pbd.byaWinSrat[i] > 0)
                    //            {
                    //                str.Append(ShowSpecialWinType(i, pbd.caFanResult));
                    //                break;
                    //            }
                    //        }
                    //    }

                    //}
                    #endregion 放炮

                    #region 点

                    if (pbd.playingMethodConf.byWinPoint > 0 && pbd.byShootSeat == 0)
                    {
                        int iValue = pbd.resultType[iseatNum - 1].lastTile.bySuit;
                        int point = 0;
                        if (iValue != 0 && pbd.byaWinSrat[iseatNum - 1] > 0)
                        {
                            int suit = iValue / 16;
                            int value = iValue % 16;
                            if (suit < 4)
                            {
                                if (value < 4)
                                {
                                    point = 1;
                                }
                                else if (value > 3 && value < 7)
                                {
                                    point = 2;
                                }
                                else
                                {
                                    point = 3;
                                }
                            }
                            else
                            {
                                point = 3;
                            }
                        }
                        str.Append("点+");
                        str.Append(point);
                        str.Append("    ");
                    }



                    #endregion

                    #region 坎（三张一样的算一坎）
                    //int kong_count = 0;  //坎的数量
                    ////如果设置了坎，则计算坎的分数
                    //if (pbd.playingMethodConf.byWinKong > 0 && pbd.byaWinSrat[iseatNum - 1] > 0)
                    //{
                    //    byte[] temp = new byte[14];
                    //    for (int i = 0; i < 14; i++)
                    //    {
                    //        temp[i] = pbd.bHandleTiles[winSeat - 1, i];
                    //    }
                    //    byte[,] value =SwitchArray(temp);
                    //    for (int i = 0; i < 5; i++)
                    //    {
                    //        for (int j = 1; j < 10; j++)
                    //        {
                    //            //如果玩家手牌有三张相同的记为1坎
                    //            if (value[i, j] > 2)
                    //            {
                    //                kong_count++;
                    //            }
                    //        }
                    //    }

                    //    //如果下地的三张是不是也要算入坎中
                    //    //if (pbd.playingMethodConf.byWinKongFlagNum > 0)
                    //    //{
                    //    //    for (int i = 0; i < pbd.usersCardsInfo[0].listSpecialCards.Count; i++)
                    //    //    {
                    //    //        if (pbd.usersCardsInfo[0].listSpecialCards[i].type == 2 && pbd.usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16 > 3)
                    //    //        {
                    //    //            kong_count++;
                    //    //        }
                    //    //    }
                    //    //}

                    //    if (kong_count > 0)
                    //    {

                    //        str.Append("坎+" + kong_count);
                    //        str.Append("    ");
                    //    }
                    //}

                    #endregion

                    #region 庄闲倍数                    
                    //if (pbd.playingMethodConf.byWinPointDealerMultiple > 1 && pbd.playingMethodConf.byWinPointPlayerMultiple > 1)
                    //{
                    //    string sZhuangContent = "";
                    //    if (pbd.lcDealerMultiple == 1)
                    //    {
                    //        sZhuangContent = "庄*3";
                    //    }
                    //    else
                    //    {
                    //        sZhuangContent = "庄*" + pbd.playingMethodConf.byWinPointDealerMultiple;
                    //    }
                    //    string sXianContent = "闲*1";

                    //    //添加庄闲倍数
                    //    if (iDrawSeatNum > 0)
                    //    {
                    //        if (iDrawSeatNum == pbd.byDealerSeat)
                    //        {
                    //            str.Append(sZhuangContent);
                    //            str.Append("    ");
                    //        }
                    //        else
                    //        {
                    //            if (iseatNum == pbd.byDealerSeat)
                    //            {
                    //                str.Append(sZhuangContent);
                    //                str.Append("    ");
                    //            }
                    //            else
                    //            {
                    //                str.Append(sXianContent);
                    //                str.Append("    ");
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        //如果接炮收三家 或者 抢杠胡收三家
                    //        if (pbd.playingMethodConf.byWinPointMultiPay == 1 || isRobbingWin)
                    //        {
                    //            if (iseatNum == pbd.byDealerSeat)
                    //            {
                    //                str.Append(sZhuangContent);
                    //            }
                    //            else
                    //            {
                    //                //如果庄家胡牌
                    //                if (pbd.byaWinSrat[pbd.byDealerSeat - 1] > 0)
                    //                {
                    //                    str.Append(sZhuangContent);
                    //                }
                    //                else
                    //                {
                    //                    str.Append(sXianContent);
                    //                }
                    //            }
                    //            str.Append("    ");
                    //        }
                    //        else
                    //        {
                    //            if (pbd.byaWinSrat[iseatNum - 1] > 0 || iseatNum == pbd.byShootSeat)
                    //            {
                    //                if (iseatNum == pbd.byDealerSeat)
                    //                {
                    //                    str.Append(sZhuangContent);
                    //                }
                    //                else
                    //                {
                    //                    if (winSeat == pbd.byDealerSeat || pbd.byShootSeat == pbd.byDealerSeat)
                    //                    {
                    //                        str.Append(sZhuangContent);
                    //                    }
                    //                    else
                    //                    {
                    //                        str.Append(sXianContent);
                    //                    }
                    //                }
                    //                str.Append("    ");
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    #region 自摸
                    if (iDrawSeatNum > 0 && anhui.MahjongCommonMethod.Instance.iPlayingMethod != 20001)
                    {
                        //平顺自摸不翻倍
                        if (pbd.playingMethodConf.byWinPointSelfDrawnMultiple == 1)
                        {
                            str.Append("自摸*" + pbd.playingMethodConf.byWinPointSelfDrawnMultiple);
                        }
                        else
                        {
                            str.Append("自摸*" + pbd.playingMethodConf.byWinPointSelfDrawnMultiple);
                        }
                        str.Append("    ");
                    }
                    #endregion

                    #region 底分
                    //int basePoint = pbd.playingMethodConf.byWinPointBasePoint < 1 ? 1 : pbd.playingMethodConf.byWinPointBasePoint;  //底分
                    //string sPointContent = "";
                    //// Debug.LogError("是否自摸："+bSelfWin);
                    //if (MahjongCommonMethod.Instance.iPlayingMethod == 20001 && bSelfWin)//如果是红中麻将
                    //{
                    //    sPointContent = "自摸";
                    //}
                    //else
                    //{
                    //    sPointContent = "底分";
                    //}
                    ////如果自摸
                    //if (iDrawSeatNum > 0 || pbd.playingMethodConf.byWinPointMultiPay == 1 || isRobbingWin)
                    //{
                    //    str.Append(sPointContent);
                    //    if (iseatNum == iDrawSeatNum || pbd.byaWinSrat[iseatNum - 1] > 0)
                    //    {
                    //        str.Append("+" + basePoint * 3);
                    //    }
                    //    else
                    //    {
                    //        str.Append("-" + basePoint);
                    //    }
                    //    str.Append("    ");
                    //}
                    //else
                    //{
                    //    if (iseatNum == pbd.byShootSeat)
                    //    {
                    //        //如果支持一炮多响
                    //        if (pbd.playingMethodConf.byWinPointMultiShoot > 0)
                    //        {
                    //            for (int i = 0; i < 4; i++)
                    //            {
                    //                if (pbd.byaWinSrat[i] > 0)
                    //                {
                    //                    basePoint += pbd.playingMethodConf.byWinPointBasePoint;
                    //                }
                    //            }
                    //            basePoint -= pbd.playingMethodConf.byWinPointBasePoint;
                    //        }
                    //        str.Append(sPointContent);
                    //        str.Append("-" + basePoint);
                    //        str.Append("    ");
                    //    }
                    //    else
                    //    {
                    //        if (pbd.byaWinSrat[iseatNum - 1] > 0)
                    //        {
                    //            str.Append(sPointContent);
                    //            str.Append("+" + basePoint);
                    //            str.Append("    ");
                    //        }
                    //    }
                    //}
                    #endregion

                    #region 添加额外接炮放炮信息
                    Debug.Log("添加额外接炮放炮信息" + pbd.playingMethodConf.byWinPointMultiPay + "__" + pbd.playingMethodConf.byWinPointMultiPayExtraMode);
                    //添加接炮的信息
                    if (pbd.playingMethodConf.byWinPointMultiPay > 0 && pbd.playingMethodConf.byWinPointMultiPayExtraMode > 0)
                    {
                        if (pbd.playingMethodConf.byWinPointMultiPayExtra > 0)
                        {
                            if (pbd.byaWinSrat[iseatNum - 1] > 0 && iDrawSeatNum == 0 && pbd.resultType[iseatNum - 1].cRobbingKongPoint == 0)
                            {
                                str.Append("接炮+" + pbd.playingMethodConf.byWinPointMultiPayExtra);
                                str.Append("    ");
                            }

                            if (pbd.byShootSeat == iseatNum && pbd.resultType[iseatNum - 1].cRobbingKongPoint == 0)
                            {
                                str.Append("放炮-" + pbd.playingMethodConf.byWinPointMultiPayExtra);
                                str.Append("    ");
                            }
                        }
                    }
                    #endregion
                   
                }
                //  Debug.LogError("补杠" + pbd.resultType[iseatNum - 1].cTripletToKongPoint);
                if (pbd.resultType[iseatNum - 1].cTripletToKongPoint != 0)
                {
                    str.Append("补杠");
                    if (pbd.resultType[iseatNum - 1].cTripletToKongPoint > 0)
                    {
                        str.Append("+");
                    }
                    str.Append(pbd.resultType[iseatNum - 1].cTripletToKongPoint);
                    str.Append("    ");
                }

  
            }

            //  Debug.Log("num" + num);
            //改变显示特殊牌型文字说明的长度
            if (str.Length > 0)
            {
                winMessage.gameObject.SetActive(true);
                winMessage_text.gameObject.SetActive(true);
                winMessage_text.text = str.ToString();
                if (winMessage_text.preferredWidth > 605f)
                {
                    winMessage.rectTransform.sizeDelta = new Vector2(625f, 31f);
                }
                else
                {
                    winMessage.rectTransform.sizeDelta = new Vector2(winMessage_text.preferredWidth + 20f, 31f);
                }
                winMessage.transform.localPosition = new Vector3(winMessage.transform.localPosition.x + (winMessage.rectTransform.rect.width - 60f) / 2f, 8f, 0);
            }
        }
        /// <summary>
        /// 显示玩家的特殊胡牌牌型
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        string ShowSpecialWinType(int index, sbyte[,] value)
        {
            PlayBackData  pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
           // Debug.LogError("_____"+MahjongCommonMethod.Instance.iPlayingMethod);
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < MahjongGame_AH.Network.Message.NetMsg.F_TOTAL_NUM; i++)
            {
                // Debug.LogWarning("用户："+index +"胡牌牌型：" + i + "value：" + value[index, i]);
                if (MahjongCommonMethod.Instance._cfgSpecialTypeCardName.data[i].index  == 49&& MahjongCommonMethod.Instance.iPlayingMethod==20001)
                {
                    // if (value[index, i] > 0)                       
                    str.AppendFormat(MahjongCommonMethod.Instance._cfgSpecialTypeCardName.data[i].name + (value[index, i] > 0 ? "+" : ""), pbd.resultType[0].byFanTiles[0]);
                    str.Append(value[index, i]);
                    str.Append("    ");
                }
                else if (value[index, i] != 0)
                {
                    if (MahjongCommonMethod.Instance.iPlayingMethod == 20001 && i == 0)
                    {
                        str.Append(MahjongCommonMethod.Instance._cfgSpecialTypeCardName.data[i].name_a + (value[index, i] > 0 ? "+" : ""));
                    }
                    else
                    {
                        str.Append(MahjongCommonMethod.Instance._cfgSpecialTypeCardName.data[i].name + (value[index, i] > 0 ? "+" : ""));
                    }
                    str.Append(value[index, i]);
                    str.Append("    ");
                }
            }
            return str.ToString();

        }

        /// <summary>
        /// 产生对应的手牌
        /// </summary>
        public void SpwanPlayerCard()
        {
            //Debug.LogError("产生玩家手牌");
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            Vector3 initpos_0 = initPos.transform.localPosition;  //普通麻将的初始位置
            if (initpos_0.y > 80)
            {
                initpos_0 = new Vector3(initpos_0.x, 80, 0);
            }
            int mahjongNum_0 = 0; //麻将产生数量
            byte bLastTile = 0;  //最后一张得到的牌
            int winNum = 0;  //显示胡牌的数量
            //存储玩家的手牌
            List<byte> mahvalue = new List<byte>();

            for (int i = 0; i < 14; i++)
            {
                if (pbd.bHandleTiles[iseatNum - 1, i] != 0)
                {
                    mahvalue.Add(pbd.bHandleTiles[iseatNum - 1, i]);
                }
            }

            //添加玩家最后获取的一张牌，会添加一个胡牌的标志图片
            if (pbd.resultType[iseatNum - 1].lastTile.bySuit != 0 && pbd.byaWinSrat[iseatNum - 1] == 1)
            {
                bLastTile = (byte)(pbd.resultType[iseatNum - 1].lastTile.bySuit * 16 + pbd.resultType[iseatNum - 1].lastTile.byValue);

                if (!bSelfWin)
                {
                    mahvalue.Add(bLastTile);
                }
            }


            //移除对应的顺子牌的麻将值
            for (int i = 0; i < pbd.resultType[iseatNum - 1].bySequenceNum; i++)
            {
                byte value = (byte)(pbd.resultType[iseatNum - 1].sequenceType[i].bySuit * 16
                     + pbd.resultType[iseatNum - 1].sequenceType[i].byFirstValue);
                mahvalue.Remove(value);
                mahvalue.Remove((byte)(value + 1));
                mahvalue.Remove((byte)(value + 2));
            }

            //移除手牌中的刻子牌的花色值
            for (int i = 0; i < pbd.resultType[iseatNum - 1].byTripletNum; i++)
            {
                byte value = (byte)(pbd.resultType[iseatNum - 1].tripletType[i].bySuit * 16
                     + pbd.resultType[iseatNum - 1].tripletType[i].byValue);
                //如果是碰，删除两张手牌
                if (pbd.resultType[iseatNum - 1].tripletType[i].byPongKongType == 1)
                {
                    mahvalue.Remove(value);
                    mahvalue.Remove(value);
                }

                //如果是杠，删除3张手牌
                if (pbd.resultType[iseatNum - 1].tripletType[i].byPongKongType == 2 || pbd.resultType[iseatNum - 1].tripletType[i].byPongKongType == 3)
                {
                    mahvalue.Remove(value);
                    mahvalue.Remove(value);
                    mahvalue.Remove(value);
                }
            }

            //移除玩家十三幺的牌的花色值
            for (int i = 0; i < pbd.resultType[iseatNum - 1].byThirteenOrphansNum; i++)
            {
                byte value = (byte)(pbd.resultType[iseatNum - 1].thirteenOrphansType[i].bySuit * 16
                    + pbd.resultType[iseatNum - 1].thirteenOrphansType[i].byValue);

                mahvalue.Remove(value);
            }

            mahvalue.Sort(iCompareList);

            //Debug.LogError("玩家座位号:" + iseatNum + ",手牌的数量:" + mahvalue.Count);

            //产生手牌
            for (int i = 0; i <= mahvalue.Count; i++)
            {
                if (i == mahvalue.Count)
                {
                    GameObject go = null;
                    go = PoolManager.Spawn("Game/Ma/", "showVCardPre");
                    go.transform.SetParent(playerCard.transform);
                    go.transform.SetAsFirstSibling();
                    go.transform.localScale = Vector3.one;
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.Find("Image/num").transform.localEulerAngles = Vector3.zero;
                    go.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
                    go.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                    go.transform.localPosition = initpos_0;

                    if (bLastTile == mahvalue[0] && winNum == 0 && pbd.byaWinSrat[iseatNum - 1] == 1)
                    {
                        go.transform.Find("win").gameObject.SetActive(true);
                        winNum++;
                    }
                    go.transform.Find("point").gameObject.SetActive(false);
                    go.transform.Find("Image/num").GetComponent<Image>().sprite = PlayBackMahjongPanel.Instance.UpdateCardValue(mahvalue[0]);
                }
                else
                {
                    if (mahvalue[i] != 0)
                    {
                        GameObject go = null;
                        go = PoolManager.Spawn("Game/Ma/", "showVCardPre");
                        go.transform.SetParent(playerCard.transform);
                        go.transform.SetAsFirstSibling();
                        go.transform.localScale = Vector3.one;
                        go.transform.localEulerAngles = Vector3.zero;
                        go.transform.Find("Image/num").transform.localEulerAngles = Vector3.zero;
                        go.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
                        go.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                        go.transform.localPosition = initpos_0 + new Vector3(mahjongNum_0 * 42f, 0, 0);

                        mahjongNum_0++;
                        if (bLastTile == mahvalue[i] && winNum == 0 && pbd.byaWinSrat[iseatNum - 1] == 1)
                        {
                            go.transform.Find("win").gameObject.SetActive(true);
                            winNum++;
                        }
                        go.transform.Find("point").gameObject.SetActive(false);
                        go.transform.Find("Image/num").GetComponent<Image>().sprite = PlayBackMahjongPanel.Instance.UpdateCardValue(mahvalue[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //为玩家的第一个牌重新产生
            Vector3 initpos_1 = initPos_special.transform.localPosition;  //特殊麻将的初始位置
            int mahjongNum_1 = 0;
            //产生顺子牌
            for (int i = 0; i < pbd.resultType[iseatNum - 1].bySequenceNum; i++)
            {
                GameObject go = null;
                byte value = 0;   //顺子的第一个值
                go = PoolManager.Spawn("Game/Ma/", "pegaUCardsPre");
                go.transform.SetParent(playerCard.transform);
                go.transform.SetAsFirstSibling();
                go.transform.localScale = Vector3.one;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localPosition = initpos_1 + new Vector3(mahjongNum_0 * 42f + 20f + mahjongNum_1 * 145f, 0, 0);
                //更新牌面
                value = (byte)(pbd.resultType[iseatNum - 1].sequenceType[i].bySuit * 16 + pbd.resultType[iseatNum - 1].sequenceType[i].byFirstValue);
                UpdateCard(go, value, 0);
                mahjongNum_1++;
            }

            //产生刻子牌
            for (int i = 0; i < pbd.resultType[iseatNum - 1].byTripletNum; i++)
            {
                GameObject go = null;
                byte value = 0;  //对应牌的花色值

                go = PoolManager.Spawn("Game/Ma/", "pegaUCardsPre");
                go.transform.SetParent(playerCard.transform);
                go.transform.SetAsFirstSibling();
                go.transform.localScale = Vector3.one;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localPosition = initpos_1 + new Vector3(mahjongNum_0 * 42f + 20f + mahjongNum_1 * 145f, 0, 0);
                //更新牌面的花色值
                value = (byte)(pbd.resultType[iseatNum - 1].tripletType[i].bySuit * 16 + pbd.resultType[iseatNum - 1].tripletType[i].byValue);
                Debug.Log ("刻子牌的花色值:" + value);
                UpdateCard(go, value, pbd.resultType[iseatNum - 1].tripletType[i].byPongKongType);
                mahjongNum_1++;
            }

            //产生吃抢的牌
            for (int i = 0; i < pbd.resultType[iseatNum - 1].byThirteenOrphansNum; i++)
            {
                byte value = 0;  //对应牌的花色值
                                 //更新牌面的花色值
                value = (byte)(pbd.resultType[iseatNum - 1].thirteenOrphansType[i].bySuit * 16 + pbd.resultType[iseatNum - 1].thirteenOrphansType[i].byValue);
                GameObject go = null;
                go = PoolManager.Spawn("Game/Ma/", "showVCardPre");
                go.transform.SetParent(playerCard.transform);
                go.transform.SetAsFirstSibling();
                go.transform.localScale = Vector3.one;
                go.transform.localEulerAngles = Vector3.zero;
                go.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
                go.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                go.transform.localPosition = initpos_0 + new Vector3(mahjongNum_0 * 42f + 20f, 0, 0);
                go.transform.Find("Image/num").GetComponent<Image>().sprite = PlayBackMahjongPanel.Instance.UpdateCardValue(value);
                mahjongNum_0++;
            }
        }

        //手牌排序规则
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


        /// <summary>
        /// 更新牌面
        /// </summary>
        /// <param name="go">对应的预置体</param>
        /// <param name="value">预置体对应的花色值</param>
        /// <param name="status">0表示顺子，1表示碰，2表示明杠，3表示暗杠</param>
        void UpdateCard(GameObject go, byte value, int status)
        {
            //Debug.LogError("value:" + value + ",status:" + status);

            if (go == null)
            {
                return;
            }
            //获取子物体下的具体物体
            Transform tran_L = go.transform.Find("belowCardL");
            Transform tran_M = go.transform.Find("belowCardM");
            Transform tran_R = go.transform.Find("belowCardR");
            Transform tran_U = go.transform.Find("topCard");
            Transform num_L = tran_L.transform.Find("num");
            Transform num_M = tran_M.transform.Find("num");
            Transform num_R = tran_R.transform.Find("num");
            Transform num_U = tran_U.transform.Find("num");

            //更改牌面的位置
            num_L.localEulerAngles = Vector3.zero;
            num_M.localEulerAngles = Vector3.zero;
            num_R.localEulerAngles = Vector3.zero;
            num_U.localEulerAngles = Vector3.zero;

            //麻将的值
            byte[] mahjongValue = new byte[3];

            //碰
            if (status == 1)
            {
                Sprite sprite = PlayBackMahjongPanel.Instance.UpdateCardValue(value);
                num_L.GetComponent<Image>().sprite = sprite;
                num_M.GetComponent<Image>().sprite = sprite;
                num_R.GetComponent<Image>().sprite = sprite;
                return;
            }



            //顺子
            if (status == 0)
            {
                Sprite sprite_0 = PlayBackMahjongPanel.Instance.UpdateCardValue(value);
                Sprite sprite_1 = PlayBackMahjongPanel.Instance.UpdateCardValue((byte)(value + 1));
                Sprite sprite_2 = PlayBackMahjongPanel.Instance.UpdateCardValue((byte)(value + 2));
                num_L.GetComponent<Image>().sprite = sprite_0;
                num_M.GetComponent<Image>().sprite = sprite_1;
                num_R.GetComponent<Image>().sprite = sprite_2;
                return;
            }

            //明杠
            if (status == 2)
            {
                Sprite sprite = PlayBackMahjongPanel.Instance.UpdateCardValue(value);
                num_L.GetComponent<Image>().sprite = sprite;
                num_M.GetComponent<Image>().sprite = sprite;
                num_R.GetComponent<Image>().sprite = sprite;
                tran_U.GetComponent<Image>().enabled = true;
                num_U.GetComponent<Image>().enabled = true;
                num_U.GetComponent<Image>().sprite = sprite;
            }

            //暗杠
            if (status == 3)
            {
                Sprite sprite_0 = PlayBackMahjongPanel.Instance.sKong_V[1];
                Sprite sprite_2 = PlayBackMahjongPanel.Instance.sKong_V[0];
                Sprite sprite_1 = PlayBackMahjongPanel.Instance.UpdateCardValue(value);
                tran_U.GetComponent<Image>().enabled = true;
                tran_U.GetComponent<Image>().sprite = sprite_2;
                num_U.GetComponent<Image>().enabled = true;
                num_U.GetComponent<Image>().sprite = sprite_1;
                tran_L.GetComponent<Image>().enabled = true;
                tran_L.GetComponent<Image>().sprite = sprite_0;
                num_L.gameObject.SetActive(false);
                tran_R.GetComponent<Image>().enabled = true;
                tran_R.GetComponent<Image>().sprite = sprite_0;
                num_R.gameObject.SetActive(false);

            }

        }
    }

}
