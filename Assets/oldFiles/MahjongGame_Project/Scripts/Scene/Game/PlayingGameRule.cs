using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using MahjongGame_AH.Data;
using MahjongGame_AH.GameSystem.SubSystem;
using System.Collections.Generic;
using DG.Tweening;
using XLua;
using anhui;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    //展示房间规则
    public class PlayingGameRule : MonoBehaviour
    {
        public Transform PayNumParent;   //玩法的局数的父物体
        public GameObject[] RuleParent;//4是aa支付
        public GameObject RuleParent_;

        public ScrollRect m_GameRule;//scrollview 的

        public ScrollRect Rect; //所有玩法的那个rect
        public LayoutElement[] LayoutMent_All; //所有玩法面板的父物体

        public LayoutElement SpecialSetting; //高级设置的父物体
        public GameObject[] AdvanceSetting;  //高级设置选项
        public Text SettingText;  //高级设置字体的信息
        public Image SettingImage;  //高级设置的图片
        public GameObject _gPlayingGameRulePanel;

        public GameObject TuoGuan;//托管按钮
        public Text CarouselContent;//轮播条的内容

        public Text ReginPlayName;//区域玩法名称

        public GameObject PlayGoldModth;//自选玩法
        public GameObject PlayGoldModthParent;//父物体

        int AnvanceSettingStatus = 1; //1表示可以点击展开，2表示可以点击收起

        void Start()
        {
            UpdateShowMethod();
        }

        /// <summary>
        /// 产生玩法数量的信息
        /// </summary>
        /// <param name="index">该玩法的id</param>
        public void UpdateShowMethod()
        {
            int index = MahjongCommonMethod.Instance.iPlayingMethod;
            if (index <= 0)
            {
                return;
            }
            SpwanPayNum(index);
            SpwanMethod(index);

            ReginPlayName.text = MahjongCommonMethod.Instance._dicMethodConfig[index].METHOD_NAME;
        }


        /// <summary>
        /// 产生玩法数量的预置体
        /// </summary>
        /// <param name="index">玩法id</param>
        /// <param name="ChoiceIndex">玩家选择的下标</param>
        void SpwanPayNum(int index)
        {
            int ChoiceIndex = -1;

            MahjongCommonMethod mcm = MahjongCommonMethod.Instance;
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;


            //玩法局或圈的数量
            int[] sum = new int[5];
            //玩法数量对应支付的房卡数量
            int[] pay = new int[5];

            //获取对应的游戏局或者圈的数量
            if (mcm._dicMethodConfig[index].sum.Contains("|"))
            {
                string[] str_0 = mcm._dicMethodConfig[index].sum.Split('|')[0].Split('_');
                string[] str_1 = mcm._dicMethodConfig[index].sum.Split('|')[1].Split('_');
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
                string[] str_0 = mcm._dicMethodConfig[index].sum.Split('_');
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
            if (mcm._dicMethodConfig[index].pay.Contains("|"))
            {
                string[] str_0 = mcm._dicMethodConfig[index].pay.Split('|')[0].Split('_');
                string[] str_1 = mcm._dicMethodConfig[index].pay.Split('|')[1].Split('_');
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
                string[] str_0 = mcm._dicMethodConfig[index].pay.Split('_');
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

            //if (index == 17)
            //{
            //    sum[3] = 4; pay[3] = pay[0];
            //    sum[4] = 8; pay[4] = pay[1];
            //    sum[5] = 12; pay[5] = pay[2];
            //}

            //int addChoiceIndex = 0;
            if (index == 17)
            {
                if (pppd.playingMethodConf.byBillingMode == 2)
                {
                    sum[0] = 4;
                    sum[1] = 8;
                    sum[2] = 12;
                    //addChoiceIndex = 3;
                }
            }
            int count = 0;

            //根据付费数量，判断出玩家的信息
            for (int i = 0; i < 5; i++)
            {
                if (index == 2 && pppd.playingMethodConf.byBillingPrice == pay[i] && pppd.playingMethodConf.byBillingMode == 2)
                {
                    ChoiceIndex = i;
                    //pppd.playingMethodConf.byBillingMode = 1;
                }
                else if (pppd.playingMethodConf.byBillingNumber == sum[i] && pppd.playingMethodConf.byBillingPrice == pay[i])
                {
                    ChoiceIndex = i;
                }
            }

            MahjongLobby_AH.Data.CreatRoomMessagePanelData cd = MahjongLobby_AH.GameData.Instance.CreatRoomMessagePanelData;
            if (MahjongCommonMethod .Instance.ReadColumnValue( cd.roomMessage_,2,39) <= 1)
            {
                for (int pay_index = 0; pay_index < pay.Length; pay_index++)
                {
                    if (pay[pay_index] != 0)
                    {
                        pay[pay_index] = pay[pay_index] / 4;
                        Debug.LogError(pay[pay_index]);
                    }
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

                crcm.type = pppd.playingMethodConf.byBillingMode;
                crcm.num = sum[i];
                crcm.pay = pay[i];
                crcm.UpdateShow();
                count++;
            }

            if (pppd.playingMethodConf.byBillingMode == 1)
            {
                PayNumParent.GetChild(1).GetComponent<Text>().text = "圈数：";
            }
            else if (pppd.playingMethodConf.byBillingMode == 2)
            {
                PayNumParent.GetChild(1).GetComponent<Text>().text = "局数：";
            }
            else if (pppd.playingMethodConf.byBillingMode == 3)
            {
                PayNumParent.GetChild(1).GetComponent<Text>().text = "分数：";
            }

            PayNumParent.GetComponent<LayoutElement>().minHeight = 60f;
            //PayNumParent.GetComponent<LayoutElement>().minHeight = 60f + ((int)(count / 3f + 0.5f) - 1f) * 45;
        }


        /// <summary>
        /// 产生规则的信息
        /// </summary>
        /// <param name="index"></param>
        void SpwanMethod(int index)
        {
            int num_MultiChoiceBtn = 0;  //表示隐藏多选按钮数量 
            //int num_OnlyOneChoice = 0;  //表示隐藏多选按钮数量            
            MahjongCommonMethod mcm = MahjongCommonMethod.Instance;
            MahjongLobby_AH.Data.CreatRoomMessagePanelData cd = MahjongLobby_AH.GameData.Instance.CreatRoomMessagePanelData;

            //删除规则
            for (int i = 0; i < RuleParent.Length - 1; i++)
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
            #region 托管条显示不显示
            ///////////////////////////////////////////////////////////
            if ( cd.roomMessage_[5] > 0)
            {
                TuoGuan.SetActive(true);
                GameObject gotuoguan = null;
                m_GameRule.enabled = true;

                m_GameRule.transform.GetChild(0).GetChild(0).transform.SetParent(m_GameRule.transform.GetChild(0));
                m_GameRule.transform.GetChild(0).GetChild(0).transform.localPosition = new Vector3(0, 0, 0);
                for (int i = 0; i < 4; i++)
                {
                    gotuoguan = Instantiate(Resources.Load<GameObject>("Lobby/PlayerMethodPanel/OnlyOneChoice"));
                    gotuoguan.GetComponent<Toggle>().group = RuleParent[3].GetComponent<ToggleGroup>();
                    gotuoguan.name = "OnlyOneChoice";

                    gotuoguan.transform.SetParent(RuleParent[3].transform);
                    gotuoguan.transform.localPosition = new Vector3(gotuoguan.transform.localPosition.x, gotuoguan.transform.localPosition.y, 0);
                    gotuoguan.transform.localScale = Vector3.one;
                    gotuoguan.transform.Find("Label").GetComponent<Text>().text = MahjongLobby_AH.UIMainView.Instance.CreatRoomMessagePanel.lTuoGuan[i];
                    gotuoguan.GetComponent<Toggle>().interactable = false;
                    int LateTime = 0;
                    if (i != 0)
                    {
                        string str = MahjongLobby_AH.UIMainView.Instance.CreatRoomMessagePanel.lTuoGuan[i];
                        str = str.Substring(0, MahjongLobby_AH.UIMainView.Instance.CreatRoomMessagePanel.lTuoGuan[i].Length - 1);
                        LateTime = (Convert.ToInt32(str) / 60);
                    }

                    if (cd.roomMessage_[5] == LateTime)
                    {
                        gotuoguan.GetComponent<Toggle>().isOn = true;
                    }
                    else
                    {
                        gotuoguan.GetComponent<Toggle>().isOn = false;
                    }
                }
            }
            else
            {
                TuoGuan.SetActive(false);
                m_GameRule.enabled = false;
            }
            if (MahjongCommonMethod.Instance.ReadColumnValue ( cd.roomMessage_,2,39) ==2)
            {
                RuleParent[4].transform.GetChild(0).GetComponent<Toggle>().isOn = true;
                RuleParent[4].transform.GetChild(1).GetComponent<Toggle>().isOn = false;
            }
            else if(MahjongCommonMethod.Instance.ReadColumnValue(cd.roomMessage_, 2, 39)<=1)
            {
                RuleParent[4].transform.GetChild(0).GetComponent<Toggle>().isOn = false;
                RuleParent[4].transform.GetChild(1).GetComponent<Toggle>().isOn = true;
            }
            if (RuleParent[0].GetComponentsInChildren<Transform>().Length < 1)
            {
                RuleParent[0].transform.parent.gameObject.SetActive(false);
            }
            else
            {
                RuleParent[0].transform.parent.gameObject.SetActive(true);
            }

            if (PlayGoldModth.activeSelf)
            {
                m_GameRule.enabled = true;
            }
            #endregion

            if (!mcm._dicMethodCardType.ContainsKey(index))
            {
                RuleParent_.SetActive(false);
                return;
            }
            RuleParent_.SetActive(true);
            //该玩法有多个规则可以实现
            for (int i = 0; i < mcm._dicMethodCardType[index].Count; i++)
            {
                GameObject go = null;

                //如果该玩法为多选
                if (mcm._dicmethodToCardType[index][i].Choice == 2)
                {
                    go = Instantiate(Resources.Load<GameObject>("Lobby/PlayerMethodPanel/MultiChoiceBtn"));
                    go.name = "MultiChoiceBtn";
                    if (mcm._dicmethodToCardType[index][i].Hierarchy == 1)
                    {
                        num_MultiChoiceBtn++;
                    }
                    else
                    {
                        RuleParent[mcm._dicmethodToCardType[index][i].Hierarchy - 1].GetComponent<GridLayoutGroup>().padding.left = 65;
                        go.transform.Find("Label").transform.localPosition += new Vector3(20, 0, 0);
                    }
                }
                else
                {
                    go = Instantiate(Resources.Load<GameObject>("Lobby/PlayerMethodPanel/OnlyOneChoice"));
                    go.GetComponent<Toggle>().group = RuleParent[mcm._dicmethodToCardType[index][i].Hierarchy - 1].GetComponent<ToggleGroup>();
                    go.name = "OnlyOneChoice";
                }
                bool status =mcm. JudgeIsShow(mcm._dicmethodToCardType[index][i].RuleId);
                if (status)
                {
                    for (int k = 0; k < mcm._dicMethodCardType[index].Count; k++)
                    {
                        if (mcm._dicMethodCardType[index][i].ID == mcm._dicmethodToCardType[index][i].RuleId)
                        {
                            SelectContent.Add(mcm._dicMethodCardType[index][i].notes);
                            break;
                        }
                    }
                }
                go.transform.SetParent(RuleParent[mcm._dicmethodToCardType[index][i].Hierarchy - 1].transform);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                go.transform.localScale = Vector3.one;
                go.transform.Find("Label").GetComponent<Text>().text = mcm._dicMethodCardType[index][i].card_type;
                go.GetComponent<Toggle>().interactable = false;
                if (status)
                {
                    go.transform.Find("Label").GetComponent<Text>().color = new Color(1, 0, 0, 1);//红色 选中为红色
                    go.GetComponent<Toggle>().isOn = true;
                }
            }

            if (num_MultiChoiceBtn <= 4)
            {
                RuleParent[0].GetComponentInParent<LayoutElement>().minHeight = 70f;
            }
            else
            {
                RuleParent[0].GetComponentInParent<LayoutElement>().minHeight = 120f;
            }


            //if (num_OnlyOneChoice == 0)
            //{
            //    RuleParent[1].SetActive(false);
            //    RuleParent[2].SetActive(false);
            //}
            //else
            //{
            //    RuleParent[1].SetActive(true);
            //    RuleParent[2].SetActive(true);
            //}

            RuleParent_.GetComponent<LayoutElement>().minHeight = 170f + (int)((num_MultiChoiceBtn) / 4f - 0.5f) * 50f;
        }

        public void BtnClose()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            _gPlayingGameRulePanel.SetActive(false);
        }

        /// <summary>
        /// 点击高级设置
        /// </summary>
        public void BtnAdvanceSetting()
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

            if (AnvanceSettingStatus == 1)
            {
                SettingText.text = "收起";
                SettingImage.transform.localEulerAngles = new Vector3(0, 0, 180f);
                AdvanceSetting[0].SetActive(true);
                AdvanceSetting[1].SetActive(true);
                SpecialSetting.minHeight = 145f;
                AnvanceSettingStatus = 2;
            }
            else
            {
                SettingText.text = "展开";
                SettingImage.transform.localEulerAngles = Vector3.zero;
                AdvanceSetting[0].SetActive(false);
                AdvanceSetting[1].SetActive(false);
                SpecialSetting.minHeight = 55f;
                AnvanceSettingStatus = 1;
            }

            float height = 0;
            for (int i = 0; i < LayoutMent_All.Length; i++)
            {
                height += LayoutMent_All[i].minHeight;
            }

            if (height > 375f)
            {
                Rect.GetComponent<ScrollRect>().enabled = true;
            }
            else
            {
                Rect.GetComponent<ScrollRect>().enabled = false;
            }
        }

        void OnEnable()
        {
            StartCarouselIndex = 0;
            isClockSelectButton = true;
            timeCarouselStart = 0;
        }

        void Update()
        {
            //UpdataRunToCarouselRoll();
        }

        [HideInInspector]
        public List<string> SelectContent = new List<string>();
        private float timeCarouselStart = 0;
        private float timeCarouselOver = 5.0f;
        private int StartCarouselIndex = 0;
        private bool isClockSelectButton = false;
        /// <summary>
        /// 循环调用轮播条
        /// </summary>
        /// <returns></returns>
        void UpdataRunToCarouselRoll()
        {
            if (isClockSelectButton)
            {
                while (timeCarouselStart >= timeCarouselOver)
                {
                    if (StartCarouselIndex < SelectContent.Count)
                        StartCarouselIndex++;
                    else
                        StartCarouselIndex = 0;
                    DnCarouselRoll(SelectContent[StartCarouselIndex >= SelectContent.Count ? 0 : StartCarouselIndex]);

                    timeCarouselStart -= timeCarouselOver;
                }
                timeCarouselStart += Time.deltaTime;
            }
        }

        /// <summary>
        /// 轮播条播放选中的内容
        /// </summary>
        /// <param name="contentText"></param>
        public void DnCarouselRoll(string contentText)
        {
            CarouselContent.text = contentText;

            //先置为空的
            CarouselContent.transform.localPosition = new Vector3(0, -20, 0);
            CarouselContent.color = new Color(1, 1, 1, 1);
            float speed = 1f;
            //之后的操作
            CarouselContent.transform.DOLocalMoveY(0, speed);
            CarouselContent.DOColor(new Color(113 / 255.0f, 85 / 255.0f, 72 / 255.0f, 1), speed);
        }

        public void BtnRoomRule()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            int index = MahjongCommonMethod.Instance.iPlayingMethod;
            if (index <= 0)
            {
                return;
            }
            Messenger_anhui<int>.Broadcast(MainViewGameRulePanel.MESSAGE_OPEN, index);

            _gPlayingGameRulePanel.SetActive(false);
        }
    }

}
