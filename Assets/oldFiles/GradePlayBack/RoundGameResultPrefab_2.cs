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
    public class RoundGameResultPrefab_2 : MonoBehaviour
    {
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
            MahjongCommonMethod.Instance.GetPlayerAvatar(headImage, pbd.sHeadUrl[iseatNum - 1]);
            NickName.text = pbd.sName[iseatNum - 1];

            int mul = 1;
            if (pbd.iMethodId == 14)
            {
                mul = 100;
            }

            if (pbd.iPoint[iseatNum - 1] > 0)
            {
                PlayerScore.font = font[0];
                PlayerScore.text = "+" + (pbd.iPoint[iseatNum - 1] * mul).ToString();
            }
            else if (pbd.iPoint[iseatNum - 1] < 0)
            {
                PlayerScore.font = font[1];
                PlayerScore.text = (pbd.iPoint[iseatNum - 1] * mul).ToString();
            }
            else
            {
                PlayerScore.font = font[1];
                PlayerScore.text = (pbd.iPoint[iseatNum - 1] * mul).ToString();
            }

            //处理明暗杠，自摸信息,调整位置长度
            int num = 0;   //用来表示要显示几个特殊牌型
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

            //获取明杠暗杠数量
            for (int i = 0; i < pbd.resultType_2[iseatNum - 1].byTripletNum; i++)
            {
                if (pbd.resultType_2[iseatNum - 1].tripletType[i].byPongKongType == 2)
                {
                    iMingGongNum++;
                }
                else if (pbd.resultType_2[iseatNum - 1].tripletType[i].byPongKongType == 3)
                {
                    iAnGangGongNum++;
                }

            }

            //添加明杠
            if (pbd.resultType_2[iseatNum - 1].cExposedKongPoint != 0)
            {
                num++;
                str.Append("明杠");
                if (pbd.resultType_2[iseatNum - 1].cExposedKongPoint > 0)
                {
                    str.Append("+");
                }
                str.Append(pbd.resultType_2[iseatNum - 1].cExposedKongPoint);
                str.Append("    ");
            }

            //添加暗杠
            if (pbd.resultType_2[iseatNum - 1].cConcealedKongPoint != 0)
            {
                num++;
                str.Append("暗杠");
                if (pbd.resultType_2[iseatNum - 1].cConcealedKongPoint > 0)
                {
                    str.Append("+");
                }
                str.Append(pbd.resultType_2[iseatNum - 1].cConcealedKongPoint);
                str.Append("    ");
            }

            int iDrawSeatNum = 0; //自摸座位号
            int winCount_ = 0; //赢家的数量
            //添加自摸
            for (int i = 0; i < 4; i++)
            {
                if (pbd.byaWinSrat[i] > 0)
                {
                    winCount_++;
                    iDrawSeatNum = i + 1;
                }
            }

            if (winCount_ > 0 && pbd.byShootSeat <= 0)
            {
                bSelfWin = true;
                if (iDrawSeatNum == iseatNum)
                {
                    winMessage_Image.gameObject.SetActive(true);
                    winMessage_Image.sprite = image_round[1];
                }
            }
            else
            {
                iDrawSeatNum = 0;
            }

            //判断自己接炮胡
            if (pbd.byaWinSrat[iseatNum - 1] == 1 && !bSelfWin)
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
                if (pbd.byaWinSrat[i] == 1)
                {
                    iKongWin++;
                }
            }

            //本玩法的底分，默认为1分，沁县的底分为5分
            int iBaseScore = 1;

            //添加前抬后和
            //for (int i = 0; i < 2; i++)
            //{
            //    if (pbd.resultType_2[iseatNum - 1].cRaisePoint[i] != 0)
            //    {
            //        if (i == 0)
            //        {
            //            str.Append("前抬");
            //        }
            //        else
            //        {
            //            str.Append("后和");
            //        }

            //        if (pbd.resultType_2[iseatNum - 1].cRaisePoint[i] > 0)
            //        {
            //            str.Append("+");
            //        }
            //        str.Append(pbd.resultType_2[iseatNum - 1].cRaisePoint[i]);
            //        str.Append("    ");
            //        num++;
            //    }
            //}


            //如果是荒庄不显示底分倍数
            if (iKongWin > 0)
            {
                //表示有人自摸
                if (iDrawSeatNum > 0)
                {
                    //如果自摸的人是自己
                    if (iseatNum == iDrawSeatNum)
                    {
                        str.Append("底分+" + iBaseScore * 3f);
                        str.Append("    ");
                    }
                    else
                    {
                        str.Append("底分-" + iBaseScore * 1f);
                        str.Append("    ");
                    }

                    //如果是庄自摸
                    if (iDrawSeatNum == pbd.byDealerSeat)
                    {
                        str.Append("庄*2");
                        str.Append("    ");
                    }
                    else
                    {
                        //如果自己是庄家
                        if (iseatNum == pbd.byDealerSeat)
                        {
                            str.Append("庄*2");
                            str.Append("    ");
                        }
                        else
                        {
                            str.Append("闲*1");
                            str.Append("    ");
                        }
                    }
                    num++;
                }
                //无人自摸
                else
                {
                    if (pbd.byaWinSrat[iseatNum - 1] == 1)
                    {
                        if (pbd.playingMethodConf.byWinPointMultiPay == 1)
                        {
                            str.Append("底分+" + iBaseScore * 3f);
                            str.Append("    ");
                        }
                        else
                        {
                            str.Append("底分+" + iBaseScore * 1f);
                            str.Append("    ");
                        }
                        //如果自己是庄家
                        if (iseatNum == pbd.byDealerSeat)
                        {
                            str.Append("庄*2");
                            str.Append("    ");
                        }
                        else
                        {
                            str.Append("闲*1");
                            str.Append("    ");
                        }
                        num++;
                    }
                    else
                    {
                        int winSeat = -1;
                        for (int i = 0; i < 4; i++)
                        {
                            if (pbd.byaWinSrat[i] == 1)
                            {
                                winSeat = i;
                            }
                        }

                        int winCount = 0; //赢得人的数量
                        if (pbd.byShootSeat == iseatNum)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (pbd.byaWinSrat[i] > 0)
                                {
                                    winCount++;
                                }
                            }
                            if (winCount > 0)
                            {
                                Debug.LogWarning("=========================seatNum:" + iseatNum);
                                str.Append("底分-");
                                str.Append(winCount * iBaseScore);
                                str.Append("    ");

                                if (iseatNum == pbd.byDealerSeat)
                                {
                                    str.Append("庄*2");
                                    str.Append("    ");
                                }
                                else
                                {
                                    if (winSeat == pbd.byDealerSeat - 1)
                                    {
                                        str.Append("庄*2");
                                        str.Append("    ");
                                    }
                                    else
                                    {
                                        str.Append("闲*1");
                                        str.Append("    ");
                                    }
                                }
                                num++;
                            }

                            //如果是自己放炮，同时有特殊胡牌类型
                            for (int l = 0; l < 4; l++)
                            {
                                if (pbd.byaWinSrat[l] > 0)
                                {
                                    //特殊牌型，加减分数
                                    for (int i = 0; i < 5; i++)
                                    {
                                        if (pbd.caFanResult[l, i] != 0)
                                        {
                                            switch (i)
                                            {
                                                //十三不靠
                                                case 1:
                                                    str.Append("十三不靠*");
                                                    str.Append(pbd.caFanResult[l, i]);
                                                    str.Append("    ");
                                                    num++;
                                                    break;
                                                //十三幺
                                                case 2:
                                                    str.Append("十三幺*");
                                                    str.Append(pbd.caFanResult[l, i]);
                                                    str.Append("    ");
                                                    num++;
                                                    break;
                                                //七对
                                                case 3:
                                                    str.Append("七对*");
                                                    str.Append(pbd.caFanResult[l, i]);
                                                    str.Append("    ");
                                                    num++;
                                                    break;
                                                case 4:
                                                    str.Append("七对*");
                                                    str.Append(pbd.caFanResult[l, i]);
                                                    str.Append("    ");
                                                    num++;
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                            num++;



                        }
                        else if (pbd.playingMethodConf.byWinPointMultiPay == 1)
                        {
                            winCount += 1;
                            str.Append("底分-");
                            str.Append(winCount * iBaseScore);
                            str.Append("    ");

                            if (iseatNum == pbd.byDealerSeat)
                            {
                                str.Append("庄*2");
                                str.Append("    ");
                            }
                            else
                            {
                                str.Append("闲*1");
                                str.Append("    ");
                            }

                            num++;
                        }
                    }
                }

                //如果有玩家自摸，添加特殊牌型
                if (iDrawSeatNum > 0)
                {
                    //特殊牌型，加减分数
                    for (int i = 0; i < 5; i++)
                    {
                        if (pbd.caFanResult[iDrawSeatNum - 1, i] != 0)
                        {
                            switch (i)
                            {
                                //十三不靠
                                case 1:
                                    str.Append("十三不靠*");
                                    str.Append(pbd.caFanResult[iDrawSeatNum - 1, i]);
                                    str.Append("    ");
                                    num++;
                                    break;
                                //十三幺
                                case 2:
                                    str.Append("十三幺*");
                                    str.Append(pbd.caFanResult[iDrawSeatNum - 1, i]);
                                    str.Append("    ");
                                    num++;
                                    break;
                                //七对
                                case 3:
                                    str.Append("七对*");
                                    str.Append(pbd.caFanResult[iDrawSeatNum - 1, i]);
                                    str.Append("    ");
                                    break;
                                case 4:
                                    str.Append("七对*");
                                    str.Append(pbd.caFanResult[iDrawSeatNum - 1, i]);
                                    str.Append("    ");
                                    break;
                            }
                            num++;
                        }
                    }
                }
                else
                {
                    //特殊牌型，加减分数
                    for (int i = 0; i < 5; i++)
                    {
                        if (pbd.caFanResult[iseatNum - 1, i] != 0)
                        {
                            switch (i)
                            {
                                //十三不靠
                                case 1:
                                    str.Append("十三不靠*");
                                    str.Append(pbd.caFanResult[iseatNum - 1, i]);
                                    str.Append("    ");
                                    num++;
                                    break;
                                //十三幺
                                case 2:
                                    str.Append("十三幺*");
                                    str.Append(pbd.caFanResult[iseatNum - 1, i]);
                                    str.Append("    ");
                                    num++;
                                    break;
                                //七对
                                case 3:
                                case 4:
                                    str.Append("七对*");
                                    str.Append(pbd.caFanResult[iseatNum - 1, i]);
                                    str.Append("    ");
                                    num++;
                                    break;
                            }
                            num++;
                        }
                    }


                }

                if (iDrawSeatNum > 0)
                {
                    str.Append("自摸*2");
                    str.Append("    ");
                    num++;
                }
            }



            if (num == -1)
            {
                winMessage.gameObject.SetActive(false);
                winMessage_text.gameObject.SetActive(false);
            }

            //改变显示特殊牌型文字说明的长度
            if (num > 0)
            {
                winMessage.gameObject.SetActive(true);
                winMessage_text.gameObject.SetActive(true);
                winMessage_text.text = str.ToString();
                winMessage.rectTransform.sizeDelta = new Vector2(winMessage_text.preferredWidth + 20f, 31f);
                winMessage.transform.localPosition = new Vector3(winMessage.transform.localPosition.x + (winMessage.rectTransform.rect.width - 60f) / 2f, 8f, 0);
            }

        }


        /// <summary>
        /// 产生对应的手牌
        /// </summary>
        public void SpwanPlayerCard()
        {
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
            if (pbd.resultType_2[iseatNum - 1].lastTile.bySuit != 0 && pbd.byaWinSrat[iseatNum - 1] == 1)
            {
                bLastTile = (byte)(pbd.resultType_2[iseatNum - 1].lastTile.bySuit * 16 + pbd.resultType_2[iseatNum - 1].lastTile.byValue);

                if (!bSelfWin)
                {
                    mahvalue.Add(bLastTile);
                }
            }


            //移除对应的顺子牌的麻将值
            for (int i = 0; i < pbd.resultType_2[iseatNum - 1].bySequenceNum; i++)
            {
                byte value = (byte)(pbd.resultType_2[iseatNum - 1].sequenceType[i].bySuit * 16
                     + pbd.resultType_2[iseatNum - 1].sequenceType[i].byFirstValue);
                mahvalue.Remove(value);
                mahvalue.Remove((byte)(value + 1));
                mahvalue.Remove((byte)(value + 2));
            }

            //移除手牌中的刻子牌的花色值
            for (int i = 0; i < pbd.resultType_2[iseatNum - 1].byTripletNum; i++)
            {
                byte value = (byte)(pbd.resultType_2[iseatNum - 1].tripletType[i].bySuit * 16
                     + pbd.resultType_2[iseatNum - 1].tripletType[i].byValue);
                //如果是碰，删除两张手牌
                if (pbd.resultType_2[iseatNum - 1].tripletType[i].byPongKongType == 1)
                {
                    mahvalue.Remove(value);
                    mahvalue.Remove(value);
                }

                //如果是杠，删除3张手牌
                if (pbd.resultType_2[iseatNum - 1].tripletType[i].byPongKongType == 2 || pbd.resultType_2[iseatNum - 1].tripletType[i].byPongKongType == 3)
                {
                    mahvalue.Remove(value);
                    mahvalue.Remove(value);
                    mahvalue.Remove(value);
                }
            }

            //移除玩家十三幺的牌的花色值
            for (int i = 0; i < pbd.resultType_2[iseatNum - 1].byThirteenOrphansNum; i++)
            {
                byte value = (byte)(pbd.resultType_2[iseatNum - 1].thirteenOrphansType[i].bySuit * 16
                    + pbd.resultType_2[iseatNum - 1].thirteenOrphansType[i].byValue);

                mahvalue.Remove(value);
            }

            mahvalue.Sort(iCompareList);

            Debug.LogWarning("玩家座位号:" + iseatNum + ",手牌的数量:" + mahvalue.Count);

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
                        //Debug.LogWarning("牌的值:" + mahvalue[i]);
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

                        //Debug.LogWarning("位置:" + go.transform.localPosition);

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
            for (int i = 0; i < pbd.resultType_2[iseatNum - 1].bySequenceNum; i++)
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
                value = (byte)(pbd.resultType_2[iseatNum - 1].sequenceType[i].bySuit * 16 + pbd.resultType_2[iseatNum - 1].sequenceType[i].byFirstValue);
                UpdateCard(go, value, 0);
                mahjongNum_1++;
            }

            //产生刻子牌
            for (int i = 0; i < pbd.resultType_2[iseatNum - 1].byTripletNum; i++)
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
                value = (byte)(pbd.resultType_2[iseatNum - 1].tripletType[i].bySuit * 16 + pbd.resultType_2[iseatNum - 1].tripletType[i].byValue);
                Debug.LogWarning("刻子牌的花色值:" + value);
                UpdateCard(go, value, pbd.resultType_2[iseatNum - 1].tripletType[i].byPongKongType);
                mahjongNum_1++;
            }

            //产生吃抢的牌
            for (int i = 0; i < pbd.resultType_2[iseatNum - 1].byThirteenOrphansNum; i++)
            {
                byte value = 0;  //对应牌的花色值
                                 //更新牌面的花色值
                value = (byte)(pbd.resultType_2[iseatNum - 1].thirteenOrphansType[i].bySuit * 16 + pbd.resultType_2[iseatNum - 1].thirteenOrphansType[i].byValue);
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


        /// <summary>
        /// 更新牌面
        /// </summary>
        /// <param name="go">对应的预置体</param>
        /// <param name="value">预置体对应的花色值</param>
        /// <param name="status">0表示顺子，1表示碰，2表示明杠，3表示暗杠</param>
        void UpdateCard(GameObject go, byte value, int status)
        {
            Debug.LogWarning("value:" + value + ",status:" + status);
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
