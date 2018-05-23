using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using XLua;

namespace PlayBack_1
{
    [Hotfix]
    [LuaCallCSharp]
    public class PlayBackGameRulePanel_2 : MonoBehaviour
    {
        public Transform PayNumParent;   //玩法的局数的父物体
        public GameObject[] RuleParent;
        public GameObject RuleParent_;

        //具体玩法的信息     
        public class CardType
        {
            public int ID;   //玩法id
            public string card_type;  //玩法的名字
            public string notes;  //该玩法的注释       
            public int status; //0表示没选中，2表示选中  
        }

        public class PayCount
        {
            public int Num; //开放数量
            public int type; //1表示局，2表示圈，3表示分
            public int Pay; //支付房卡数量
            public int status;  //0表示没选中，1表示选中
        }

        //开放的支付参数
        public List<PayCount> pay = new List<PayCount>();
        //玩法的具体信息
        public List<CardType> type = new List<CardType>();


        public void AnslyGameRuleParam(int MethodId)
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            switch (MethodId)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
            gameObject.SetActive(true);
            SpwanPayNum(MethodId);
            SpwanMethod(MethodId);
        }


        /// <summary>
        /// 产生玩法数量的预置体
        /// </summary>
        /// <param name="MethodId">玩法id</param>        
        void SpwanPayNum(int MethodId)
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            int ChoiceIndex = -1;
            anhui.MahjongCommonMethod mcm = anhui.MahjongCommonMethod.Instance;

            //玩法局或圈的数量
            int[] sum = new int[5];
            //玩法数量对应支付的房卡数量
            int[] pay = new int[5];

            //获取对应的游戏局或者圈的数量
            if (mcm._dicMethodConfig[MethodId].sum.Contains("|"))
            {
                string[] str_0 = mcm._dicMethodConfig[MethodId].sum.Split('|')[0].Split('_');
                string[] str_1 = mcm._dicMethodConfig[MethodId].sum.Split('|')[1].Split('_');
                for (int i = 0; i < 3; i++)
                {
                    sum[i] = Convert.ToInt16(str_0[i]);
                }

                for (int i = 0; i < 2; i++)
                {
                    sum[i + 3] = Convert.ToInt16(str_1[i]);
                }
            }
            else
            {
                string[] str_0 = mcm._dicMethodConfig[MethodId].sum.Split('_');
                for (int i = 0; i < 3; i++)
                {
                    if (i < str_0.Length)
                    {
                        sum[i] = Convert.ToInt16(str_0[i]);
                    }
                    else
                    {
                        sum[i] = 0;
                    }

                }
                sum[3] = 0;
                sum[4] = 0;
            }

            //获取玩家对应的支付数量
            if (mcm._dicMethodConfig[MethodId].pay.Contains("|"))
            {
                string[] str_0 = mcm._dicMethodConfig[MethodId].pay.Split('|')[0].Split('_');
                string[] str_1 = mcm._dicMethodConfig[MethodId].pay.Split('|')[1].Split('_');
                for (int i = 0; i < 3; i++)
                {
                    pay[i] = Convert.ToInt16(str_0[i]);
                }

                for (int i = 0; i < 2; i++)
                {
                    pay[i + 3] = Convert.ToInt16(str_1[i]);
                }
            }
            else
            {
                string[] str_0 = mcm._dicMethodConfig[MethodId].pay.Split('_');
                for (int i = 0; i < 3; i++)
                {
                    if (i < str_0.Length)
                    {
                        pay[i] = Convert.ToInt16(str_0[i]);
                    }
                    else
                    {
                        pay[i] = 0;
                    }

                }
                pay[3] = 0;
                pay[4] = 0;
            }

            int count = 0;

            //根据付费数量，判断出玩家的信息
            for (int i = 0; i < 5; i++)
            {
                if (pbd.playingMethodConf_2.byBillingNumber == sum[i] && pbd.playingMethodConf_2.byBillingPrice == pay[i])
                {
                    ChoiceIndex = i;
                }
            }

            //删除自己的之前的房间规则信息
            CreatRoomChoiceMethod[] method = PayNumParent.Find("GameNumbers").GetComponentsInChildren<CreatRoomChoiceMethod>();
            if (method.Length > 0)
            {
                for (int i = 0; i < method.Length; i++)
                {
                    Destroy(method[i].gameObject);
                }
            }

            //产生对应的玩法数据
            for (int i = 0; i < sum.Length; i++)
            {
                if (sum[i] == 0)
                {
                    continue;
                }

                GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/PlayerMethodPanel/CreatRoomChoiceMethod"));
                go.name = "CreatRoomChoiceMethod";
                go.transform.SetParent(PayNumParent.Find("GameNumbers"));
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                go.transform.localScale = Vector3.one;
                if (i == ChoiceIndex)
                {
                    go.transform.GetComponent<Toggle>().isOn = true;
                }
                else
                {
                    go.transform.GetComponent<Toggle>().isOn = false;
                }

                go.transform.GetComponent<Toggle>().interactable = false;
                CreatRoomChoiceMethod crcm = go.GetComponent<CreatRoomChoiceMethod>();

                crcm.type = pbd.playingMethodConf_2.byBillingMode;
                crcm.num = sum[i];
                crcm.pay = pay[i];
                crcm.UpdateShow();
                count++;
            }

            PayNumParent.GetComponent<LayoutElement>().minHeight = 60f + ((int)(count / 3f + 0.5f) - 1f) * 45;
        }


        /// <summary>
        /// 产生规则的信息
        /// </summary>
        /// <param name="MethodId"></param>
        void SpwanMethod(int MethodId)
        {
            int num = 0;  //表示隐藏多选按钮数量            
            anhui.MahjongCommonMethod mcm = anhui.MahjongCommonMethod.Instance;

            //删除规则
            for (int i = 0; i < RuleParent.Length; i++)
            {
                Toggle[] tog = new Toggle[RuleParent[i].GetComponentsInChildren<Toggle>().Length];
                tog = RuleParent[i].GetComponentsInChildren<Toggle>();
                if (tog.Length > 0)
                {
                    for (int k = 0; k < tog.Length; k++)
                    {
                        Destroy(tog[k].gameObject);
                    }
                }
            }

            //该玩法有多个规则可以实现
            for (int i = 0; i < mcm._dicMethodCardType[MethodId].Count; i++)
            {
                GameObject go = null;

                //如果该玩法为多选
                if (mcm._dicmethodToCardType[MethodId][i].Choice == 2)
                {
                    num++;
                    go = Instantiate(Resources.Load<GameObject>("Lobby/PlayerMethodPanel/MultiChoiceBtn"));
                    go.name = "MultiChoiceBtn";
                }
                else
                {
                    go = Instantiate(Resources.Load<GameObject>("Lobby/PlayerMethodPanel/OnlyOneChoice"));
                    go.GetComponent<Toggle>().group = RuleParent[mcm._dicmethodToCardType[MethodId][i].Hierarchy - 1].GetComponent<ToggleGroup>();
                    go.name = "OnlyOneChoice";
                }

                bool status = JudgeIsShow(mcm._dicmethodToCardType[MethodId][i].RuleId);
                go.transform.SetParent(RuleParent[mcm._dicmethodToCardType[MethodId][i].Hierarchy - 1].transform);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                go.transform.localScale = Vector3.one;
                go.transform.Find("Label").GetComponent<Text>().text = mcm._dicMethodCardType[MethodId][i].card_type;
                go.GetComponent<Toggle>().interactable = false;
                if (status)
                {
                    go.GetComponent<Toggle>().isOn = true;
                }
            }

            if (num <= 4)
            {
                RuleParent[0].GetComponentInParent<LayoutElement>().minHeight = 70f;
            }
            else
            {
                RuleParent[0].GetComponentInParent<LayoutElement>().minHeight = 120f;
            }
            RuleParent_.GetComponent<LayoutElement>().minHeight = 170f + (int)(num / 4f - 0.5f) * 50f;
        }

        /// <summary>
        /// 规则id
        /// </summary>
        /// <param name="MethodId"></param>
        bool JudgeIsShow(int MethodId)
        {
            bool isShow = false;
            PlayBackData pppd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            switch (MethodId)
            {
                //十三幺
                case 1001:
                    if (pppd.playingMethodConf.byWinSpecialThirteenOrphans == 1)
                    {
                        isShow = true;
                    }
                    break;
                //七对
                case 1002:
                    Debug.Log("七对可胡:  "+ pppd.playingMethodConf.byWinSpecialSevenPairs);
                    if (pppd.playingMethodConf.byWinSpecialSevenPairs == 1)
                    {
                        isShow = true;
                    }
                    break;
                //十三不靠                   
                case 1004:
                    if (pppd.playingMethodConf.byWinSpecialThirteenIndepend == 1)
                    {
                        isShow = true;
                    }
                    break;
                //明杠收三家
                case 1005:
                    if (pppd.playingMethodConf.byFanExtraExposedMode == 2)
                    {
                        isShow = true;
                    }
                    break;
                //四杠荒庄
                case 1006:
                    if (pppd.playingMethodConf.byDrawFourKong == 1)
                    {
                        isShow = true;
                    }
                    break;
                //自摸翻倍
                case 1007:
                    if (pppd.playingMethodConf.byWinPointSelfDrawnMultiple == 2 && pppd.iMethodId == 7)
                    {
                        isShow = true;
                    }
                    break;
                //一炮单响
                case 2002:
                    if (pppd.playingMethodConf.byWinPointMultiShoot == 0)
                    {
                        isShow = true;
                    }
                    break;
                //一炮多响
                case 2003:
                    if (pppd.playingMethodConf.byWinPointMultiShoot == 1)
                    {
                        isShow = true;
                    }
                    break;

                //接炮收一家
                case 2007:
                    if (pppd.playingMethodConf.byWinPointMultiPay == 0)
                    {
                        isShow = true;
                    }
                    break;
                //接炮收三家
                case 2008:
                    if (pppd.playingMethodConf.byWinPointMultiPay == 1)
                    {
                        isShow = true;
                    }
                    break;
                //一人付
                case 2009:
                    if (pppd.playingMethodConf.byFanExtraExposedBase == 0)
                    {
                        isShow = true;
                    }
                    break;
                //多人付
                case 2010:
                    if (pppd.playingMethodConf.byFanExtraExposedBase == 1)
                    {
                        isShow = true;
                    }
                    break;
                //坐庄
                case 2011:
                    if (pppd.playingMethodConf.byWinPointMultiPay == 1)
                    {
                        isShow = true;
                    }
                    break;

                //带字牌
                case 2018:
                    if (pppd.playingMethodConf.byCreateModeHaveWind > 0 && pppd.playingMethodConf.byCreateModeHaveDragon > 0)
                    {
                        isShow = true;
                    }
                    break;
                //点炮三分
                case 2019:

                    break;
                //轮庄
                case 2020:
                    if (pppd.playingMethodConf.byDealerDraw == 2 || pppd.playingMethodConf.byDealerDealer == 2)
                    {
                        isShow = true;
                    }
                    break;
                //放炮玩法
                case 2021:
                    if (pppd.playingMethodConf.byDealerPlayer == 2)
                    {
                        isShow = true;
                    }
                    break;
                //带庄
                case 2022:
                    if (pppd.playingMethodConf.byWinPointDealerMultiple == 2)
                    {
                        isShow = true;
                    }
                    break;
                //自摸胡
                case 2023:
                    if (pppd.playingMethodConf.byWinLimitSelfDrawn == 1)
                    {
                        isShow = true;
                    }
                    break;
                //可接炮
                case 3001:
                    if (pppd.playingMethodConf.byWinLimitSelfDrawn == 0)
                    {
                        isShow = true;
                    }
                    break;
                //杠随胡走
                case 2024:
                    if (pppd.playingMethodConf.byFanExtraMode == 2)
                    {
                        isShow = true;
                    }
                    break;

                //可吃牌
                case 3003:

                    if (pppd.playingMethodConf.byDiscardChow == 1)
                    {
                        isShow = true;
                    }
                    break;
                //逼头金
                case 3004:
                    //if (pppd.playingMethodConf.byDiscardOutGold == 1)
                    //{
                    //    isShow = true;
                    //}
                    break;
                //逼二金
                case 3005:
                    //if (pppd.playingMethodConf.byDiscardOutGold == 2)
                    //{
                    //    isShow = true;
                    //}
                    break;
                //不逼金
                case 3006:
                    //if (pppd.playingMethodConf.byDiscardOutGold == 0)
                    //{
                    //    isShow = true;
                    //}
                    break;
                //暗杠可见
                case 3007:
                    //if (pppd.playingMethodConf.byDiscardSeeConcealedKong == 1)
                    //{
                    //    isShow = true;
                    //}
                    break;
                //1嘴
                case 3014:
                    if (pppd.iMethodId == 20016 && pppd.playingMethodConf.byWinPointBasePoint == 1)
                    {
                        isShow = true;
                    }
                    break;
                //5嘴
                case 3015:
                    if (pppd.iMethodId == 20016 && pppd.playingMethodConf.byWinPointBasePoint == 5)
                    {
                        isShow = true;
                    }
                    break;
                //10嘴
                case 3016:
                    if (pppd.iMethodId == 20016 && pppd.playingMethodConf.byWinPointBasePoint == 10)
                    {
                        isShow = true;
                    }
                    break;
                //平胡
                case 3017:
                    if (pppd.iMethodId == 20016 && pppd.playingMethodConf.byFanSameColor == 0 &&
                        pppd.playingMethodConf.byFanMixedOneSuit == 0 && pppd.playingMethodConf.byFanLackSuit == 0
                        && pppd.playingMethodConf.byFanCouplesHand == 0)
                    {
                        isShow = true;
                    }
                    break;
                //大胡
                case 3018:
                    if (pppd.iMethodId == 20016 && pppd.playingMethodConf.byFanSameColor > 0 &&
                       pppd.playingMethodConf.byFanMixedOneSuit > 0 && pppd.playingMethodConf.byFanLackSuit > 0
                       && pppd.playingMethodConf.byFanCouplesHand > 0)
                    {
                        isShow = true;
                    }
                    break;
                //坎
                case 3024:
                    if (pppd.playingMethodConf.byWinKong == 1)
                    {
                        isShow = true;
                    }
                    break;
                //缺门
                case 3025:
                    if (pppd.playingMethodConf.byWinLack == 1)
                    {
                        isShow = true;
                    }
                    break;
                //点数
                case 3027:
                    if (pppd.playingMethodConf.byWinPoint == 1)
                    {
                        isShow = true;
                    }
                    break;
                //见面
                case 3028:
                    if (pppd.playingMethodConf.byWinMeet == 1)
                    {
                        isShow = true;
                    }
                    break;
                //幺九将
                case 3029:
                    if (pppd.playingMethodConf.byOneNineJong == 1)
                    {
                        isShow = true;
                    }
                    break;
                //六连
                case 3030:
                    if (pppd.playingMethodConf.byContinuousSix == 1)
                    {
                        isShow = true;
                    }
                    break;
                //断独幺
                case 3031:
                    if (pppd.playingMethodConf.byFanBrokenOrphas == 1)
                    {
                        isShow = true;
                    }
                    break;
            }

            return isShow;
        }

        /// <summary>
        /// 关闭规则面板
        /// </summary>
        public void BtnClose()
        {
            gameObject.SetActive(false);
        }


    }

}
