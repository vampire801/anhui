using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using MahjongLobby_AH.Data;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class ScoreStatistics : MonoBehaviour
    {
        public Text AllGet; //总收入
        public Text AllPay; //总消耗
        public Image MyCoinPay; //我的金币消耗
        public Image MemberCoinPay; //成员的金币消耗
        public Image MyFreeCoinPay; //我的赠币消耗
        public Image MemberFreeCoinPay; //成员的赠币消耗
        public Image MyScoreGet; //我的业绩收入
        public Image MemberScoreGet; //成员的业绩收入
        public Text[] MyCoin;  //我的金币 0表示赠币  1表示金币  3表示总数
        public Text[] MemCoin;  //成员金币 0表示赠币  1表示金币  3表示总数

        public Text[] ScoreGet; //0表示老板业绩收入  1表示会员业绩收入

        public void ChangeMonthMessage(int type)
        {
            UpdateShow(type);
        }


        /// <summary>
        /// 更新业绩统计面板信息
        /// </summary>
        /// <param name="status">1表示本月 2表示上月</param>
        public void UpdateShow(int status)
        {
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;

            #region 老板金币消耗
            float Sum_1 = pspd.ParlorBossScoreStatMessage[status - 1].bossCoin + pspd.ParlorBossScoreStatMessage[status - 1].bossCoin3; //当前月的消耗总数
            float Sum_2 = pspd.ParlorBossScoreStatMessage[2 - status].bossCoin + pspd.ParlorBossScoreStatMessage[2 - status].bossCoin3; //另一个月的消耗总数
            float rate_my = pspd.ParlorBossScoreStatMessage[status - 1].bossCoin / Sum_1;
            float Width_MyCoin_My = 0;  //我的金币消耗显示长度
            float Width_MyFreeCoin_My = 0;  //我的金币消耗显示长度
            if (Sum_1 > Sum_2)
            {
                Width_MyCoin_My = pspd.CoinPayLength;
                Width_MyFreeCoin_My = pspd.CoinPayLength * (1 - rate_my);
                //Debug.LogError(",rate_my:" + rate_my);
            }
            else
            {
                float rate_min = Sum_1 / Sum_2;
                float width_all = pspd.CoinPayLength * rate_min;
                Width_MyCoin_My = width_all;
                Width_MyFreeCoin_My = width_all * (1 - rate_my);
                //Debug.LogError("rate_min:" + rate_min + ",rate_my:" + rate_my);
            }


            //老板金币显示

            //MyCoinPay.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0.5f, Width_MyCoin_My);
            MyCoinPay.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width_MyCoin_My);
            //老板赠币显示
            //MyFreeCoinPay.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0.5f, Width_MyFreeCoin_My);
            MyFreeCoinPay.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width_MyFreeCoin_My);
            #endregion

            #region 会员金币消耗
            float Sum_3 = pspd.ParlorBossScoreStatMessage[status - 1].memCoin + pspd.ParlorBossScoreStatMessage[status - 1].memCoin3; //本月金币消耗总数
            float Sum_4 = pspd.ParlorBossScoreStatMessage[2 - status].memCoin3 + pspd.ParlorBossScoreStatMessage[2 - status].memCoin; //上月金币消耗总数
            float rate_my_ = (float)pspd.ParlorBossScoreStatMessage[status - 1].memCoin / Sum_3;
            float Width_MemCoin_My = 0;  //会员金币消耗显示长度
            float Width_MemFreeCoin_My = 0;  //会员赠币消耗显示长度
            if (Sum_3 > Sum_4)
            {
                Width_MemCoin_My = pspd.CoinPayLength;
                Width_MemFreeCoin_My = pspd.CoinPayLength * (1 - rate_my_);
                //Debug.LogError(",rate_my:" + rate_my_);
            }
            else
            {
                float rate_min = Sum_3 / Sum_4;
                float width_all = pspd.CoinPayLength * rate_min;
                Width_MemCoin_My = width_all;
                Width_MemFreeCoin_My = width_all * (1 - rate_my_);
                //Debug.LogError("rate_min:" + rate_min + ",rate_my:" + rate_my_);
            }

            //Debug.LogError("Width_MemCoin_My:" + Width_MemCoin_My + ",Width_MemFreeCoin_My:" + Width_MemFreeCoin_My);
            //会员金币显示
            MemberCoinPay.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width_MemCoin_My);
            //会员赠币显示
            MemberFreeCoinPay.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width_MemFreeCoin_My);
            #endregion

            #region 业绩收入
            //显示 老板业绩收入
            if (pspd.ParlorBossScoreStatMessage[status - 1].score1 > pspd.ParlorBossScoreStatMessage[2 - status].score1)
            {
                MyScoreGet.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pspd.ScoreGetLength);
            }
            else
            {
                float rate = 0;

                if (pspd.ParlorBossScoreStatMessage[2 - status].score1 != 0)
                {
                    rate = (float)(pspd.ParlorBossScoreStatMessage[status - 1].score1 / pspd.ParlorBossScoreStatMessage[2 - status].score1);
                }

                MyScoreGet.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pspd.ScoreGetLength * rate);
            }

            //显示 会员业绩收入
            if (pspd.ParlorBossScoreStatMessage[status - 1].score2 > pspd.ParlorBossScoreStatMessage[2 - status].score2)
            {
                MemberScoreGet.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pspd.ScoreGetLength);
            }
            else
            {
                float rate = 0;
                if (pspd.ParlorBossScoreStatMessage[2 - status].score2 != 0)
                {
                    rate = (float)(pspd.ParlorBossScoreStatMessage[status - 1].score2 / pspd.ParlorBossScoreStatMessage[2 - status].score2);
                }

                MemberScoreGet.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pspd.ScoreGetLength * rate);
            }
            #endregion

            #region 显示数据信息
            AllGet.text = "本月总收入: " + (pspd.ParlorBossScoreStatMessage[status - 1].score1 + pspd.ParlorBossScoreStatMessage[status - 1].score2).ToString("0.00");
            AllPay.text = "本月兑换: " + pspd.ParlorBossScoreStatMessage[status - 1].consume.ToString("0.00");

            MyCoin[0].text = pspd.ParlorBossScoreStatMessage[status - 1].bossCoin3.ToString();
            MyCoin[1].text = pspd.ParlorBossScoreStatMessage[status - 1].bossCoin.ToString();
            MyCoin[2].text = Sum_1.ToString();

            MemCoin[0].text = pspd.ParlorBossScoreStatMessage[status - 1].memCoin3.ToString();
            MemCoin[1].text = pspd.ParlorBossScoreStatMessage[status - 1].memCoin.ToString();
            MemCoin[2].text = (pspd.ParlorBossScoreStatMessage[status - 1].memCoin3 + pspd.ParlorBossScoreStatMessage[status - 1].memCoin).ToString();

            ScoreGet[0].text = pspd.ParlorBossScoreStatMessage[status - 1].score1.ToString("0.00");
            ScoreGet[1].text = pspd.ParlorBossScoreStatMessage[status - 1].score2.ToString("0.00");
            #endregion
        }


        /// <summary>
        /// 切换统计数据
        /// </summary>
        /// <param name="drop"></param>
        public void BtnChangeMonth(Toggle toggle)
        {
            if (toggle.isOn)
            {
                int index = Convert.ToInt16(toggle.name.Split('_')[1]);
                if (index == 1)
                {
                    UpdateShow(1);
                }
                else
                {
                    UpdateShow(2);
                }
            }
        }

    }
}