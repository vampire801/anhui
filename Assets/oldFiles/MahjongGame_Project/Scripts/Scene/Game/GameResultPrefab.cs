using UnityEngine;
using System.Text;
using UnityEngine.UI;
using MahjongGame_AH.Data;
using System.Collections.Generic;
using MahjongGame_AH.Network.Message;
using XLua;
using anhui;
namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class GameResultPrefab : MonoBehaviour
    {
        public RawImage headImage;  //头像
        public Text NickName;  //昵称
        public GameObject playerCard;  //手牌的父物体
        public Image winMessage; //明暗杠的具体信息
        public Text winMessage_text;  //明暗杠的具体显示文字
        public Text PlayerScore;  //玩家的分数 
        public Image Banker; //庄家的标志图片
        public Image Bg;  //背景图
        public Image Score_Bg; //分数背景图
        public GameObject initPos;  //单张麻将初始位置
        public GameObject initPos_special;  //特殊麻将的初始位置
        public int winSeat = 0;  //赢家座位号
        //单局结算
        public Image winMessage_Image;  //赢牌信息
        public Sprite[] image_round;  //0胡1自摸2放炮



        [HideInInspector]
        public int iseatNum = 1;  //玩家座位号
        public int iMingGongNum;  //明杠的数量
        public int iAnGangGongNum; //暗杠的数量
        int iDrawSeatNum = 0; //自摸座位号

        bool bSelfWin;  //是否自摸的标志位

        #region 处理玩家一局结算        
        /// <summary>
        /// 更新界面的玩家信息
        /// </summary>
        /// <param name="index"></param>
        public void MessageVlaue()
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            GameResultPanelData grpd = GameData.Instance.GameResultPanelData;

            MahjongCommonMethod.Instance.GetPlayerAvatar(headImage, pppd.usersInfo[iseatNum].szHeadimgurl);
            NickName.text = pppd.usersInfo[iseatNum].szNickname;
            if (grpd.bResultPoint[iseatNum - 1] > 0)
            {
                PlayerScore.font = font[0];
                PlayerScore.text = "+" + grpd.bResultPoint[iseatNum - 1].ToString();
            }
            else if (grpd.bResultPoint[iseatNum - 1] < 0)
            {
                PlayerScore.font = font[1];
                PlayerScore.text = grpd.bResultPoint[iseatNum - 1].ToString();
            }
            else
            {
                PlayerScore.font = font[1];
                PlayerScore.text = grpd.bResultPoint[iseatNum - 1].ToString();
            }
            StringBuilder str = new StringBuilder();

            //确定是否显示庄家标志图片
            if (iseatNum == pppd.byDealerSeat)
            {
                Banker.gameObject.SetActive(true);
            }
            else
            {
                Banker.gameObject.SetActive(false);
            }


            #region 处理明暗杠
            //获取明杠暗杠数量
            for (int i = 0; i < grpd.resultType[iseatNum - 1].byTripletNum; i++)
            {
                if (grpd.resultType[iseatNum - 1].tripletType[i].byPongKongType == 2)
                {
                    iMingGongNum++;
                }
                else if (grpd.resultType[iseatNum - 1].tripletType[i].byPongKongType == 3)
                {
                    iAnGangGongNum++;
                }

            }

            //添加明杠
            if (grpd.resultType[iseatNum - 1].cExposedKongPoint != 0)
            {
                str.Append("明杠");
                int point = grpd.resultType[iseatNum - 1].cExposedKongPoint;
                if (point > 0)
                {
                    str.Append("+");
                }

                //添加被杠，额外扣分
                if (pppd.playingMethodConf.byFanExtraExposedExtra > 0)
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
            if (grpd.resultType[iseatNum - 1].cConcealedKongPoint != 0)
            {
                str.Append("暗杠");
                if (grpd.resultType[iseatNum - 1].cConcealedKongPoint > 0)
                {
                    str.Append("+");
                }
                str.Append(grpd.resultType[iseatNum - 1].cConcealedKongPoint);
                str.Append("    ");
            }
            #endregion

            int winCount_ = 0; //赢家的数量

            //添加自摸
            for (int i = 0; i < 4; i++)
            {
               // Debug.LogWarning(i + "自摸检测byaWinSrat[i]" + grpd.byaWinSrat[i]);
                if (grpd.byaWinSrat[i] > 0)
                {
                    winCount_++;
                    winSeat = i + 1;
                    iDrawSeatNum = i + 1;
                }
            }
            //判断一局结束是否是自摸  没有放炮  有赢家
            if (winCount_ > 0 && grpd.byShootSeat <= 0)
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
            if (grpd.byaWinSrat[iseatNum - 1] > 0 && !bSelfWin)
            {
                winMessage_Image.gameObject.SetActive(true);
                winMessage_Image.sprite = image_round[0];
            }

            //如果玩家点炮显示点炮信息
            if (grpd.byShootSeat == iseatNum)
            {
                winMessage_Image.gameObject.SetActive(true);
                winMessage_Image.sprite = image_round[2];
            }


            int iKongWin = 0;
            //判断是否荒庄
            for (int i = 0; i < 4; i++)
            {
                // Debug.LogError(i + "grpd.byaWinSrat[i]" + grpd.byaWinSrat[i]);
                if (grpd.byaWinSrat[i] > 0)
                {
                    iKongWin++;
                }
            }


            //判断抢杠胡收几家
            bool isRobbingWin = false;
            if (iDrawSeatNum == 0 && pppd.playingMethodConf.byFanRobbingKongWin > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (grpd.byaWinSrat[i] > 0 && grpd.resultType[i].lastTile.byRobbingKong == 1)
                    {
                        isRobbingWin = true;
                    }
                }
            }

            //如果不是荒庄 显示底分倍数
            if (iKongWin > 0)
            {

                str.Append(ShowSpecialWinType(iseatNum - 1, grpd.caFanResult));

                if (iDrawSeatNum > 0)//自摸
                {

                }
                else//放炮
                {

                    #region 放炮
                    //一炮多响  放炮收三家
                    //if (pppd.playingMethodConf.byWinPointMultiPay > 0 || pppd.playingMethodConf.byWinPointMultiShoot > 0)
                    //{
                    //    if (iseatNum == grpd.byShootSeat)
                    //    {
                    //        str.Append(ShowSpecialWinType(iseatNum - 1, grpd.caFanResult));
                    //        for (int i = 0; i < 4; i++)
                    //        {
                    //            if (grpd.byaWinSrat[i] > 0)
                    //            {
                    //                str.Append(ShowSpecialWinType(i, grpd.caFanResult));
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (grpd.byaWinSrat[iseatNum - 1] > 0)
                    //        {
                    //            str.Append(ShowSpecialWinType(iseatNum - 1, grpd.caFanResult));
                    //        }
                    //        else
                    //        {
                    //            for (int i = 0; i < 4; i++)
                    //            {
                    //                if (grpd.byaWinSrat[i] > 0)
                    //                {
                    //                    str.Append(ShowSpecialWinType(i, grpd.caFanResult));
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}//放炮收一家
                    //else
                    //{
                    //    if (grpd.byaWinSrat[iseatNum - 1] > 0)
                    //    {
                    //        str.Append(ShowSpecialWinType(iseatNum - 1, grpd.caFanResult));
                    //    }

                    //    if (iseatNum == grpd.byShootSeat)
                    //    {
                    //        for (int i = 0; i < 4; i++)
                    //        {
                    //            if (grpd.byaWinSrat[i] > 0)
                    //            {
                    //                str.Append(ShowSpecialWinType(i, grpd.caFanResult));
                    //                break;
                    //            }
                    //        }
                    //    }

                    //}
                    #endregion 放炮

                    #region 点

                    if (pppd.playingMethodConf.byWinPoint > 0 && grpd.byShootSeat == 0)
                    {
                        int iValue = grpd.resultType[iseatNum - 1].lastTile.bySuit;
                        int point = 0;
                        if (iValue != 0 && grpd.byaWinSrat[iseatNum - 1] > 0)
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
                    //if (pppd.playingMethodConf.byWinKong > 0 && grpd.byaWinSrat[iseatNum - 1] > 0)
                    //{
                    //    byte[] temp = new byte[14];
                    //    for (int i = 0; i < 14; i++)
                    //    {
                    //        temp[i] = grpd.bHandleTiles[winSeat - 1, i];
                    //    }
                    //    byte[,] value = MahjongHelper.Instance.SwitchArray(temp);
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
                    //    //if (pppd.playingMethodConf.byWinKongFlagNum > 0)
                    //    //{
                    //    //    for (int i = 0; i < pppd.usersCardsInfo[0].listSpecialCards.Count; i++)
                    //    //    {
                    //    //        if (pppd.usersCardsInfo[0].listSpecialCards[i].type == 2 && pppd.usersCardsInfo[0].listSpecialCards[i].mahValue[0] / 16 > 3)
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
                    if (pppd.playingMethodConf.byWinPointDealerMultiple > 1 && pppd.playingMethodConf.byWinPointPlayerMultiple > 1)
                    {
                        string sZhuangContent = "";
                        if (pppd.lcDealerMultiple == 1)
                        {
                            sZhuangContent = "庄*3";
                        }
                        else
                        {
                            sZhuangContent = "庄*" + pppd.playingMethodConf.byWinPointDealerMultiple;
                        }
                        string sXianContent = "闲*1";

                        //添加庄闲倍数
                        if (iDrawSeatNum > 0)
                        {
                            if (iDrawSeatNum == pppd.byDealerSeat)
                            {
                                str.Append(sZhuangContent);
                                str.Append("    ");
                            }
                            else
                            {
                                if (iseatNum == pppd.byDealerSeat)
                                {
                                    str.Append(sZhuangContent);
                                    str.Append("    ");
                                }
                                else
                                {
                                    str.Append(sXianContent);
                                    str.Append("    ");
                                }
                            }
                        }
                        else
                        {
                            //如果接炮收三家 或者 抢杠胡收三家
                            if (pppd.playingMethodConf.byWinPointMultiPay == 1 || isRobbingWin)
                            {
                                if (iseatNum == pppd.byDealerSeat)
                                {
                                    str.Append(sZhuangContent);
                                }
                                else
                                {
                                    //如果庄家胡牌
                                    if (grpd.byaWinSrat[pppd.byDealerSeat - 1] > 0)
                                    {
                                        str.Append(sZhuangContent);
                                    }
                                    else
                                    {
                                        str.Append(sXianContent);
                                    }
                                }
                                str.Append("    ");
                            }
                            else
                            {
                                if (grpd.byaWinSrat[iseatNum - 1] > 0 || iseatNum == grpd.byShootSeat)
                                {
                                    if (iseatNum == pppd.byDealerSeat)
                                    {
                                        str.Append(sZhuangContent);
                                    }
                                    else
                                    {
                                        if (winSeat == pppd.byDealerSeat || grpd.byShootSeat == pppd.byDealerSeat)
                                        {
                                            str.Append(sZhuangContent);
                                        }
                                        else
                                        {
                                            str.Append(sXianContent);
                                        }
                                    }
                                    str.Append("    ");
                                }
                            }
                        }
                    }
                    #endregion

                    #region 自摸
                    if (iDrawSeatNum > 0 && MahjongCommonMethod.Instance.iPlayingMethod != 20001)
                    {
                        //平顺自摸不翻倍
                        if (pppd.playingMethodConf.byWinPointSelfDrawnMultiple == 1)
                        {
                            str.Append("自摸*" + pppd.playingMethodConf.byWinPointSelfDrawnMultiple);
                        }
                        else
                        {
                            str.Append("自摸*" + pppd.playingMethodConf.byWinPointSelfDrawnMultiple);
                        }
                        str.Append("    ");
                    }
                    #endregion

                   
                    #endregion
                    #region 添加额外接炮放炮信息
                    Debug.Log("添加额外接炮放炮信息" + pppd.playingMethodConf.byWinPointMultiPay + "__" + pppd.playingMethodConf.byWinPointMultiPayExtraMode);
                    //添加接炮的信息
                    if (pppd.playingMethodConf.byWinPointMultiPay > 0 && pppd.playingMethodConf.byWinPointMultiPayExtraMode > 0)
                    {
                        if (pppd.playingMethodConf.byWinPointMultiPayExtra > 0)
                        {
                            if (grpd.byaWinSrat[iseatNum - 1] > 0 && iDrawSeatNum == 0 && grpd.resultType[iseatNum - 1].cRobbingKongPoint == 0)
                            {
                                str.Append("接炮+" + pppd.playingMethodConf.byWinPointMultiPayExtra);
                                str.Append("    ");
                            }

                            if (grpd.byShootSeat == iseatNum && grpd.resultType[iseatNum - 1].cRobbingKongPoint == 0)
                            {
                                str.Append("放炮-" + pppd.playingMethodConf.byWinPointMultiPayExtra);
                                str.Append("    ");
                            }
                        }
                    }
                    #endregion
                    #region 抢杠胡的额外杠分
                    ////添加抢杠胡的额外杠分
                    //if (isContainRob)
                    //{
                    //    if (pppd.playingMethodConf.byFanRobbingKongWin > 0 && grpd.byShootSeat != 0)
                    //    {
                    //        if (iseatNum == grpd.byShootSeat)
                    //        {
                    //            str.Append("抢杠胡-" + pppd.playingMethodConf.byFanRobbingKongWin * basePoint);
                    //            str.Append("    ");
                    //        }

                    //        if (iseatNum == winSeat)
                    //        {
                    //            str.Append("抢杠胡+" + pppd.playingMethodConf.byFanRobbingKongWin * basePoint);
                    //            str.Append("    ");
                    //        }
                    //    }
                    //}
                    #endregion
                }
                //  Debug.LogError("补杠" + grpd.resultType[iseatNum - 1].cTripletToKongPoint);
                if (grpd.resultType[iseatNum - 1].cTripletToKongPoint != 0)
                {
                    str.Append("补杠");
                    if (grpd.resultType[iseatNum - 1].cTripletToKongPoint > 0)
                    {
                        str.Append("+");
                    }
                    str.Append(grpd.resultType[iseatNum - 1].cTripletToKongPoint);
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

        /*
            F_COMMON_HAND = 0,					// 平胡
        	F_THIRTEEN_INDEPEND,					// 十三不靠
        	F_THIRTEEN_ORPHANS,					// 十三幺
        	F_SEVEN_PAIRS,							// 七对
        	F_LUXURY_SEVEN_PAIRS,				// 豪华七对

        	F_ALL_CONCEALED_HAND=5,				// 门清
        	F_SISTER_PAVE,							//	姐妹
        	F_DOUBLE_SISTER_PAVE,					//双姐妹铺
        	F_LACK_SUIT,							// 缺一门
        	F_CONNECTED_SEQUENCE,				//	一条龙

        	F_COUPLES_HAND=10,						// 对对胡
        	F_SAME_COLOUR,							//	清一色
        	F_SAME_COUPLES,						//清对对
        	F_MIXED_ONE_SUIT,						//	混一色
        	F_HONOR_TILES,						//字一色

        	F_GANG_KAI=15,								// 杠上开花
        	F_FOUR_LAIZI,							// 四癞子胡牌
        	F_COMMON_HAND_FOUR_LAIZI,			// 天胡/地胡四癞子
        	F_SOFT_LACK,						//软缺
        	F_FORCED_LACK,						//硬缺

        	F_FOUR_IN_HAND=20,						//四遇子
        	F_VALUABLE_WIND,					//值钱风
        	F_ROBBING_KONG_WIN,					//抢杠胡
        	F_PEN_IN_WIND,						//圈风
        	F_ONSELF_WIND_JONG,					//风圈将

        	F_DI_READ_HAND=25,						//地听
        	F_THREE_WIND,						//三季风
        	F_BROKEN_ORPHAS,					//断幺
        	F_ONLY_ORPHS,							//独幺
        	F_SEA_BOTTOM_DRAW,					//海底捞月

        	F_WHOLE_BEG=30,						//全求人
        	F_ALL_ONE_NINE,						//幺幺胡(全一九)
        	F_BIG_THREE,						//大三元
        	F_WHITE_BORADS_AS_JONG,				//配子吃
        	F_ONLY_TILE_WIN,					//独一

        	F_TIAN_HU=35,							//天胡
        	F_DI_HU,							//地胡
        	F_FLOWER,								//花牌
        	F_CONTINUOUS_SIX,						//六连
        	F_DOUBLE_CONTINUOUS_SIX,				//双六连

        	F_MEET=40,									//见面
        	F_ONE_NINE_JONG,						//幺九将
        	F_KE,									//刻牌
        	F_ONESELF_DOOR_WIND,					//本门风
        	F_DERLEAST_WIND,						//本圈风

        	F_NO_ONE_NINE=45,							//断一九
        	F_FOUR_KONG,							//四杠/十八罗汉
        	F_FOUR_WIND_KONG,						//大四喜
            F_SINGLE_HOIST,                         //单吊
        	F_TOTAL_NUM								// 计番种类总数

       */
        /// <summary>
        /// 显示玩家的特殊胡牌牌型
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        string ShowSpecialWinType(int index, sbyte[,] value)
        {
            GameResultPanelData grpd = GameData.Instance.GameResultPanelData;
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < NetMsg.F_TOTAL_NUM; i++)
            {
                // Debug.LogWarning("用户："+index +"胡牌牌型：" + i + "value：" + value[index, i]);
                if (MahjongCommonMethod.Instance._cfgSpecialTypeCardName.data[i].index  == 49&& MahjongCommonMethod.Instance.iPlayingMethod == 20001)
                {
                    // if (value[index, i] > 0)                       
                    str.AppendFormat(MahjongCommonMethod.Instance._cfgSpecialTypeCardName.data[i].name + (value[index, i] > 0 ? "+" : ""), grpd.resultType[0].byFanTiles[0]);
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
            GameResultPanelData grpd = GameData.Instance.GameResultPanelData;
            Vector3 initpos_0 = initPos.transform.localPosition;  //普通麻将的初始位置

            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            int index = pppd.GetOtherPlayerShowPos(iseatNum + 1) - 1;

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
                if (grpd.bHandleTiles[iseatNum - 1, i] != 0)
                {
                    mahvalue.Add(grpd.bHandleTiles[iseatNum - 1, i]);
                }
            }


            //添加玩家最后获取的一张牌，会添加一个胡牌的标志图片
            if (grpd.resultType[iseatNum - 1].lastTile.bySuit != 0 && grpd.byaWinSrat[iseatNum - 1] > 0)
            {
                bLastTile = (byte)(grpd.resultType[iseatNum - 1].lastTile.bySuit * 16 + grpd.resultType[iseatNum - 1].lastTile.byValue);

                if (!bSelfWin)
                {
                    mahvalue.Add(bLastTile);
                }
            }

            mahvalue.Sort(iCompareList);

            //产生手牌
            for (int i = 0; i < mahvalue.Count; i++)
            {
                if (mahvalue[i] != 0)
                {
                    GameObject go = null;
                    go = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.showVCardPre);
                    go.transform.SetParent(playerCard.transform);
                    go.transform.SetAsFirstSibling();
                    go.transform.localScale = Vector3.one;
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.Find("Image/num").transform.localEulerAngles = Vector3.zero;
                    go.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
                    go.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                    go.transform.localPosition = initpos_0 + new Vector3(mahjongNum_0 * 42f, 0, 0);
                    mahjongNum_0++;
                    if (bLastTile == mahvalue[i] && winNum == 0 && grpd.byaWinSrat[iseatNum - 1] > 0)
                    {
                        go.transform.Find("win").gameObject.SetActive(true);

                        winNum++;
                    }
                    go.transform.Find("point").gameObject.SetActive(false);
                    UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(go.transform.Find("Image/num").GetComponent<Image>(), mahvalue[i]);
                }
                else
                {
                    break;
                }
            }

            Vector3 initpos_1 = initPos_special.transform.localPosition;  //特殊麻将的初始位置
            int mahjongNum_1 = 0;
            //产生顺子牌
            //Debug.LogError("产生顺子牌:" + grpd.resultType[iseatNum - 1].bySequenceNum);
            for (int i = 0; i < grpd.resultType[iseatNum - 1].bySequenceNum; i++)
            {
                GameObject go = null;
                byte value = 0;   //顺子的第一个值
                go = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.pegaUCardsPre);
                go.transform.SetParent(playerCard.transform);
                go.transform.SetAsFirstSibling();
                go.transform.localScale = Vector3.one;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localPosition = initpos_1 + new Vector3(mahjongNum_0 * 42f + 20f + mahjongNum_1 * 145f, 0, 0);
                //更新牌面
                value = (byte)(grpd.resultType[iseatNum - 1].sequenceType[i].bySuit * 16 + grpd.resultType[iseatNum - 1].sequenceType[i].byFirstValue);
                UpdateCard(go, value, 0);
                mahjongNum_1++;
            }
           // Debug.LogError("grpd.resultType[iseatNum - 1].byTripletNum" + grpd.resultType[iseatNum - 1].byTripletNum);
            //产生刻子牌
            for (int i = 0; i < grpd.resultType[iseatNum - 1].byTripletNum; i++)
            {
                GameObject go = null;
                byte value = 0;  //对应牌的花色值
                go = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.pegaUCardsPre);
                go.transform.SetParent(playerCard.transform);
                go.transform.SetAsFirstSibling();
                go.transform.localScale = Vector3.one;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localPosition = initpos_1 + new Vector3(mahjongNum_0 * 42f + 20f + mahjongNum_1 * 145f, 0, 0);
                //更新牌面的花色值
                value = (byte)(grpd.resultType[iseatNum - 1].tripletType[i].bySuit * 16 + grpd.resultType[iseatNum - 1].tripletType[i].byValue);
                // Debug.LogError("刻子牌的花色值:" + value.ToString("X2"));
                UpdateCard(go, value, grpd.resultType[iseatNum - 1].tripletType[i].byPongKongType);
                mahjongNum_1++;
            }

            //Debug.LogError("吃抢的牌的数量:" + grpd.resultType[iseatNum - 1].byThirteenOrphansNum);
            //产生吃抢的牌
            for (int i = 0; i < grpd.resultType[iseatNum - 1].byThirteenOrphansNum; i++)
            {
                byte value = 0;  //对应牌的花色值
                                 //更新牌面的花色值
                value = (byte)(grpd.resultType[iseatNum - 1].thirteenOrphansType[i].bySuit * 16 + grpd.resultType[iseatNum - 1].thirteenOrphansType[i].byValue);
                GameObject go = null;
                go = PoolManager.Spawn("Game/Ma/", PlayerPlayingPanelData.showVCardPre);
                go.transform.SetParent(playerCard.transform);
                go.transform.SetAsFirstSibling();
                go.transform.localScale = Vector3.one;
                go.transform.localEulerAngles = Vector3.zero;
                go.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
                go.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
                go.transform.localPosition = initpos_0 + new Vector3(mahjongNum_0 * 42f + 20f, 0, 0);
                UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(go.transform.Find("Image/num").GetComponent<Image>(), value, index);
                mahjongNum_0++;
            }
        }

        //手牌排序规则
        int iCompareList(byte b, byte a)
        {
            int res = 0;
            //获取麻将牌的花色和大小
            if (a < b)
            {
                return 1;
            }
            if (a > b)
            {
                return -1;
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
            // Debug.LogError("UpdateCard" + status);
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
            //碰
            if (status == 1)
            {
                UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(num_L.GetComponent<Image>(), value);
                UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(num_M.GetComponent<Image>(), value);
                UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(num_R.GetComponent<Image>(), value);

                //Sprite sprite = UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(value);
                //num_L.GetComponent<Image>().sprite = sprite;
                //num_M.GetComponent<Image>().sprite = sprite;
                //num_R.GetComponent<Image>().sprite = sprite;
                return;
            }

            //顺子
            if (status == 0)
            {
                UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(num_L.GetComponent<Image>(), value);
                UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(num_M.GetComponent<Image>(), (byte)(value + 1));
                UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(num_R.GetComponent<Image>(), (byte)(value + 2));
                //num_L.GetComponent<Image>().sprite = sprite_0;
                //num_M.GetComponent<Image>().sprite = sprite_1;
                //num_R.GetComponent<Image>().sprite = sprite_2;
                return;
            }

            //明杠
            if (status == 2 || status == 4)
            {
                //Sprite sprite = UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(value);
                UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(num_L.GetComponent<Image>(), value);
                UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(num_M.GetComponent<Image>(), value);
                UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(num_R.GetComponent<Image>(), value);
                //num_L.GetComponent<Image>().sprite = sprite;
                //num_M.GetComponent<Image>().sprite = sprite;
                //num_R.GetComponent<Image>().sprite = sprite;
                tran_U.GetComponent<Image>().enabled = true;
                num_U.GetComponent<Image>().enabled = true;
                UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(num_U.GetComponent<Image>(), value);
                return;
                //num_U.GetComponent<Image>().sprite = sprite;
            }

            //暗杠
            if (status == 3)
            {
                Sprite sprite_0 = UIMainView.Instance.PlayerPlayingPanel._myAnGangBK;
                Sprite sprite_2 = UIMainView.Instance.PlayerPlayingPanel._myMingGangBK;
                //Sprite sprite_1 = UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(value);

                tran_U.GetComponent<Image>().enabled = true;
                tran_U.GetComponent<Image>().sprite = sprite_2;
                num_U.GetComponent<Image>().enabled = true;
                //num_U.GetComponent<Image>().sprite = sprite_1;
                UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(num_U.GetComponent<Image>(), value);
                tran_L.GetComponent<Image>().enabled = true;
                tran_L.GetComponent<Image>().sprite = sprite_0;
                num_L.gameObject.SetActive(false);
                tran_R.GetComponent<Image>().enabled = true;
                tran_R.GetComponent<Image>().sprite = sprite_0;
                num_R.gameObject.SetActive(false);

            }

        }
    

        #region 处理一圈结束的结算
        public Text[] RoomResult;  //玩家打完游戏的玩家的详细数据
        public Image winBg;  //赢家的显示图标
        public Text iuserid;  //玩家id
        public Sprite[] RoomResultBg_sprite;  //玩家房间结束显示的背景颜色，1表示输家的背景，0表示赢家的背景    
        public Font[] font;  //0表示金色字体，1表示绿色字体
        public Image roomWinMessage; //房间结算信息
        public Image[] multiply; //结算的乘法符号 0表示赢家X  1表示输家X
        public Text Count; //称号数量
        public Sprite[] image_room;  //房间结算图片，0大赢家  1最佳炮手  2雀王 3雀神 4炮王 5炮神        
        string color_win = "539821FF";    //赢家的显示颜色
        string color_lose = "9C806AFF";   //输家的显示颜色
        string nickNameColor = "dbfdf3";  //失败玩家的昵称颜色
        string nickNameColor_outline = "206774";  //失败玩家的昵称颜色描边

        /// <summary>
        /// 更新玩家的信息
        /// </summary>
        public void UpdatePlayerMessage()
        {
            StringBuilder str = new StringBuilder();
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            GameResultPanelData grpd = GameData.Instance.GameResultPanelData;
            //更新玩家头像
            MahjongCommonMethod.Instance.GetPlayerAvatar(headImage, pppd.usersInfo[iseatNum].szHeadimgurl);

            int Score = 0;
            if (pppd.playingMethodConf.byBillingMode == 3)
            {
                Score = pppd.playingMethodConf.byBillingNumber;
            }

            int multiple = 1; //分数是否要放大100倍
           // if (pppd.playingMethodConf.byWinLimitBeginFan > 0)
           // {
           //     multiple = 100;
            //}

            if (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].sTotalPoint * multiple > Score)
            {
                //更新玩家的分数
                PlayerScore.font = font[0];
                PlayerScore.text = "+" + (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].sTotalPoint * multiple).ToString();
                winBg.sprite = RoomResultBg_sprite[0];
                //更新玩家的昵称
                NickName.text = pppd.usersInfo[iseatNum].szNickname;
                //更新玩家的id
                iuserid.text = "ID:" + pppd.usersInfo[iseatNum].iUserId.ToString();
            }
            else
            {
                PlayerScore.font = font[1];
                PlayerScore.text = (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].sTotalPoint * multiple).ToString();
                winBg.sprite = RoomResultBg_sprite[1];

                //更新玩家的昵称
                NickName.text = pppd.usersInfo[iseatNum].szNickname;
                NickName.color = new Color(0.86f, 1, 0.95f, 1);
                NickName.GetComponent<Outline>().effectColor = new Color(0.125f, 0.404f, 0.455f, 1f);
                //更新玩家的id
                iuserid.text = "<color=#206774>" + "ID:" + pppd.usersInfo[iseatNum].iUserId + "</color>";
            }


            //更新玩家的具体数据            
            RoomResult[0].text = grpd.roomResultNotice.aRoomResultType[iseatNum - 1].wSelfDrawn.ToString();
            RoomResult[1].text = grpd.roomResultNotice.aRoomResultType[iseatNum - 1].wExposedKong.ToString();
            RoomResult[2].text = grpd.roomResultNotice.aRoomResultType[iseatNum - 1].wConcealedKong.ToString();
            RoomResult[3].text = grpd.roomResultNotice.aRoomResultType[iseatNum - 1].wShoot.ToString();
            RoomResult[4].text = grpd.roomResultNotice.aRoomResultType[iseatNum - 1].wTakeShoot.ToString();

            //根据玩家的分数确定显示玩家的赢牌信息
            bool score = true;  //玩家分数
            bool NormalWinCount = true;  //接炮次数

            for (int i = 0; i < 4; i++)
            {
                //如果玩家选择的是打锅玩法，玩法要大于最低的分数
                if (pppd.playingMethodConf.byBillingMode == 2 || pppd.playingMethodConf.byBillingMode == 3)
                {
                    if (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].sTotalPoint * multiple <= pppd.playingMethodConf.byBillingNumber)
                    {
                        score = false;
                    }
                }

                //判断得分是不是最高
                if (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].sTotalPoint * multiple <= 0 || grpd.roomResultNotice.aRoomResultType[iseatNum - 1].sTotalPoint * multiple < grpd.roomResultNotice.aRoomResultType[i].sTotalPoint * multiple)
                {
                    score = false;
                }

                //判断点炮次数最多
                if (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].wShoot < grpd.roomResultNotice.aRoomResultType[i].wShoot || grpd.roomResultNotice.aRoomResultType[iseatNum - 1].wShoot < 2)
                {
                    NormalWinCount = false;
                }
            }

            if (score)
            {
                multiply[1].gameObject.SetActive(false);
                roomWinMessage.gameObject.SetActive(true);
                if (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBigWinnerCount < 4)
                {
                    //大赢家
                    if (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBigWinnerCount == 1)
                    {
                        multiply[0].gameObject.SetActive(false);
                        Count.gameObject.SetActive(false);
                    }
                    else
                    {
                        multiply[0].gameObject.SetActive(true);
                        Count.gameObject.SetActive(true);
                        Count.text = grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBigWinnerCount.ToString();
                    }
                    roomWinMessage.sprite = image_room[0];
                }
                else if (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBigWinnerCount < 7)
                {
                    //雀王
                    if (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBigWinnerCount == 4)
                    {
                        multiply[0].gameObject.SetActive(false);
                        Count.gameObject.SetActive(false);
                    }
                    else
                    {
                        multiply[0].gameObject.SetActive(true);
                        Count.gameObject.SetActive(true);
                        Count.text = (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBigWinnerCount - 3).ToString();
                    }
                    roomWinMessage.sprite = image_room[2];
                }
                else
                {
                    //雀神
                    if (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBigWinnerCount == 7)
                    {
                        multiply[0].gameObject.SetActive(false);
                        Count.gameObject.SetActive(false);
                    }
                    else
                    {
                        multiply[0].gameObject.SetActive(true);
                        Count.gameObject.SetActive(true);
                        Count.text = (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBigWinnerCount - 6).ToString();
                    }
                    roomWinMessage.sprite = image_room[3];
                }

                if (iseatNum == MahjongCommonMethod.Instance.mySeatid)
                {
                    GameData.Instance.GameResultPanelData.isWinner = true;
                }
            }

            if (!score && NormalWinCount)
            {
                roomWinMessage.gameObject.SetActive(true);
                multiply[0].gameObject.SetActive(false);
                Count.text = grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBestGunnerCount.ToString();
                if (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBestGunnerCount < 4)
                {
                    //最佳炮手
                    if (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBestGunnerCount == 1)
                    {
                        multiply[1].gameObject.SetActive(false);
                        Count.gameObject.SetActive(false);
                    }
                    else
                    {
                        multiply[1].gameObject.SetActive(true);
                        Count.gameObject.SetActive(true);
                        Count.text = (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBestGunnerCount).ToString();
                    }
                    roomWinMessage.sprite = image_room[1];
                }
                else if (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBestGunnerCount < 7)
                {
                    //炮王
                    if (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBestGunnerCount == 4)
                    {
                        multiply[1].gameObject.SetActive(false);
                        Count.gameObject.SetActive(false);
                    }
                    else
                    {
                        multiply[1].gameObject.SetActive(true);
                        Count.gameObject.SetActive(true);
                        Count.text = (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBestGunnerCount - 3).ToString();
                    }
                    roomWinMessage.sprite = image_room[4];
                }
                else
                {
                    //炮神
                    if (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBestGunnerCount == 7)
                    {
                        multiply[1].gameObject.SetActive(false);
                        Count.gameObject.SetActive(false);
                    }
                    else
                    {
                        multiply[1].gameObject.SetActive(true);
                        Count.gameObject.SetActive(true);
                        Count.text = (grpd.roomResultNotice.aRoomResultType[iseatNum - 1].byBestGunnerCount - 6).ToString();
                    }
                    roomWinMessage.sprite = image_room[5];
                }

                if (iseatNum == MahjongCommonMethod.Instance.mySeatid)
                {
                    GameData.Instance.GameResultPanelData.isWinner = true;
                }
            }

            roomWinMessage.GetComponent<Image>().SetNativeSize();

        }


        #endregion

    }
}



