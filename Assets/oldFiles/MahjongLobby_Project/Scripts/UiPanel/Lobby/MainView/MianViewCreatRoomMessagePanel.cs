using UnityEngine;
using System.Collections.Generic;
using MahjongLobby_AH.Data;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System;
using MahjongLobby_AH.LobbySystem.SubSystem;
using DG.Tweening;
using System.Collections;
using XLua;
using anhui;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MianViewCreatRoomMessagePanel : MonoBehaviour
    {
        public static sbyte bool2sbyte(bool status)
        {
            return (sbyte)(status ? 1 : 0);
        }
        public const string MESSAGE_OKBTN = "MianViewCreatRoomMessagePanel.MESSAGE_OKBTN";  //确定按钮
        public const string MESSAGE_CANELBTN = "MianViewCreatRoomMessagePanel.MESSAGE_CANELBTN";  //取消按钮       
        public const string MESSAGE_SWITCHCITY = "MianViewCreatRoomMessagePanel.MESSAGE_SWITCHCITY"; //点击切换城市
        public const string MESSAGE_PLAYMETHOD = "MianViewCreatRoomMessagePanel.MESSAGE_PLAYMETHOD";  //点击选择玩法按钮
        public const string MESSAGE_SPECIALSETTING = "MianViewCreatRoomMessagePanel.MESSAGE_SPECIALSETTING";  //点击高级设置按钮           


        public GameObject Grid_MethodBtn;  //玩法按钮
        public GameObject MethodNum;  //显示该玩法可选择玩法的数量的父物体
        public GameObject RuleParent_; //规则的父物体                           

        public ScrollRect Rect; //所有玩法的那个rect
        public LayoutElement[] LayoutMent_All; //所有玩法面板的父物体

        public LayoutElement SpecialSetting; //高级设置的父物体
        public GridLayoutGroup SpecialSetting_;

        public GameObject[] AdvanceSetting;  //高级设置选项
        public Text SettingText; //高级设置选项的字体显示
        public Image SettingImage;  //高级设置的选项

        public Text Compliment; //显示赞

        public GameObject RoomCost;
        public GameObject[] RuleParent;  //玩法规则的父物体  0表示第一层，1表示第二层，3表示第三层  

        public GameObject SwitchLocation; //切换位置的按钮
        public Text SelectArea; //显示选择的区域
        public Text GameDiscrite;  //玩法描述

        #region 新版的

        public Text CarouselContent;//轮播条内容
        public Text CreatRoomHour;//创建房间小时
        public Text CreatRoomMin;//创建房间分钟

        public GameObject BossClockCreateRoom;//老板点击创建开放之后弹出的界面
        public GameObject[] m_showNew;//新功能提示

        //        private Vector3 ContentInitPos = Vector3.zero;  //保存content的初始位置
        private Color NormalColor = new Color(113 / 255.0f, 85 / 255.0f, 72 / 255.0f, 1);//灰色  不选中为灰色
                                                                                         // private Color NormalColor = Color.white ;
        private Color ChangeClolor = new Color(1, 0, 0, 1);//红色 选中为红色
        //private Color ChangeClolor = Color.white;
        [HideInInspector]
        public string[] lTuoGuan = new string[4];//保存托管剩余时间使用的
        [HideInInspector]
        public int CanUserHous = 0;//时间的跨度最大为两小时
        private bool isCreateRoomClock = false;//判断是不是可以点击时间  是不是预约房

        // "超过1分钟将由笨笨的机器人代打，机器人代打战绩会很惨哟！"
        // "超过2分钟将由笨笨的机器人代打，机器人代打战绩会很惨哟！"
        // "超过5分钟将由笨笨的机器人代打，机器人代打战绩会很惨哟！"
        private string tuoguantishi = "将由笨笨的机器人代打，机器人代打战绩会很惨哟！";

        [HideInInspector]
        public int m_CreateRoomSource;//创建房间的来源 默认创建房间为1 其他为创建预约房

        public GameObject m_PlayModth;//特殊玩法
        public GameObject m_PlayModthParent;//特殊玩法的父物体

        /// <summary>
        /// 圈为true
        /// </summary>
        public bool QuanBukexuan = false;//选圈抢庄不可选
        //public bool isweiqianzhuang = false;//选了抢庄

        Vector3 IninPos_CrBtn = new Vector3(-424f, -60f, 0);
        #endregion

        void Start()
        {

            // ContentInitPos = Rect.transform.Find("Viewport/Content").transform.localPosition;

            //初始化面板的显示
            UpdateShow();
            //Rect.enabled = false;
            // UpdateShow();
            //如果创建房间的新手引导在，则关闭新手引导
            NewPlayerGuide.Instance.HideIndexGuide(NewPlayerGuide.Guide.CreatRoom);
        }


        void OnEnable()
        {
            GameData.Instance.CreatRoomMessagePanelData.MethodId = MahjongCommonMethod.Instance.lsMethodId[0];
            // UpdateShowMethod(GameData.Instance.CreatRoomMessagePanelData.MethodId);
            SpwanPlayMethodBtn();
            StartSetValue();

            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            MahjongCommonMethod.Instance.WriteColumnValue(ref crmpd.roomMessage_, 39, 2, 2);
            crmpd.roomMessage_[4] = 0;
            crmpd.roomMessage_[5] = 0;
            AdvanceSetting[2].transform.GetChild(0).GetChild(0).GetComponent<Toggle>().isOn = true;
            AdvanceSetting[2].transform.GetChild(0).GetChild(1).GetComponent<Toggle>().isOn = false;
            AdvanceSetting[2].transform.GetChild(0).GetChild(2).GetComponent<Toggle>().isOn = false;
            AdvanceSetting[2].transform.GetChild(0).GetChild(3).GetComponent<Toggle>().isOn = false;
            AdvanceSetting[3].transform.GetChild(0).GetChild(0).GetComponent<Toggle>().isOn = true;
            AdvanceSetting[3].transform.GetChild(0).GetChild(1).GetComponent<Toggle>().isOn = false;
            StartSetValue();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }

            UpdataRunToCarouselRoll();
            UpdateTime();
        }

        /// <summary>
        /// 面板更新显示
        /// </summary>
        public void UpdateShow()
        {
            GameData gd = GameData.Instance;
            CreatRoomMessagePanelData crmpd = gd.CreatRoomMessagePanelData;
            if (crmpd.PanelShow)
            {
                gameObject.SetActive(true);
                GameData.Instance.isShowQuitPanel = false;
                Compliment.text = crmpd.iCompliment.ToString();

                if (MahjongCommonMethod.Instance._dicDisConfig.ContainsKey(GameData.Instance.SelectAreaPanelData.iCountyId))
                {
                    SelectArea.text = MahjongCommonMethod.Instance._dicDisConfig[GameData.Instance.SelectAreaPanelData.iCountyId].COUNTY_NAME;
                }
                if (SDKManager.Instance.IOSCheckStaus == 0 && SDKManager.Instance.CheckStatus == 0)
                {
                    SwitchLocation.SetActive(true);
                    transform.Find("BlackBg/BtnMethod").transform.localPosition = IninPos_CrBtn;
                }
                else
                {
                    SwitchLocation.SetActive(false);
                    transform.Find("BlackBg/BtnMethod").transform.localPosition = IninPos_CrBtn + new Vector3(0, 60, 0);
                }
            }
            else
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
                if (crmpd.AnvanceSettingStatus == 2)
                {
                    BtnAdvanceSetting();
                }
            }
        }



        //玩法局或圈的数量
        [HideInInspector]
        public int[] Method_sum = new int[5];
        //玩法数量对应支付的房卡数量
        [HideInInspector]
        public int[] Method_pay = new int[5];
        //玩法类型
        [HideInInspector]
        public int[] Method_type = new int[2];

        GameObject[] method_num = new GameObject[6];

        /// <summary>
        /// 更新玩法面板
        /// </summary>
        /// <param name="index">表示玩法id</param>
        public void UpdateShowMethod(int index, bool chaxun = true)
        {
            Debug.LogWarning("玩法ID：" + GameData.Instance.CreatRoomMessagePanelData.MethodId);

            if (index <= 0)
            {
                return;
            }

            MethodNum.transform.parent.transform.parent.transform.localPosition = Vector3.zero;
            MahjongCommonMethod mcm = MahjongCommonMethod.Instance;
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            for (int allM = 0; allM < crmpd.allRoomMethor.Count; allM++)
            {
                if (crmpd.allRoomMethor[allM].methord == index)
                {
                    crmpd.isSavedParamMethid = true;
                    for (int i = 0; i < crmpd.roomMessage_.Length; i++)
                    {
                        crmpd.roomMessage_[i] = crmpd.allRoomMethor[allM].param[i];
                    }
                }
            }
            //Debug.LogError("====" + mcm._dicMethodConfig[index].type);
            //产生玩法的对应选择按钮
            if (mcm._dicMethodConfig[index].type.Contains("_"))
            {
                string[] str = mcm._dicMethodConfig[index].type.Split('_');
                for (int i = 0; i < str.Length; i++)
                {
                    Method_type[i] = Convert.ToInt32(str[i]);
                }
            }
            else
            {
                Method_type[0] = Convert.ToInt32(mcm._dicMethodConfig[index].type);
                Method_type[1] = 0;
            }

            if (Method_type[0] == 1)
            {
                GameDiscrite.text = "圈数：";
            }
            else
            {
                GameDiscrite.text = "局数：";
            }

            // Debug.LogError("玩法局：" + mcm._dicMethodConfig[index].sum);
            //获取对应的游戏局或者圈的数量
            if (mcm._dicMethodConfig[index].sum.Contains("|"))
            {
                string[] str_0 = mcm._dicMethodConfig[index].sum.Split('|')[0].Split('_');
                string[] str_1 = mcm._dicMethodConfig[index].sum.Split('|')[1].Split('_');
                for (int i = 0; i < 3; i++)
                {
                    Method_sum[i] = Convert.ToInt16(str_0[i]);
                }

                for (int i = 0; i < 2; i++)
                {
                    Method_sum[i + 3] = Convert.ToInt16(str_1[i]);
                }
            }
            else
            {
                string[] str_0 = mcm._dicMethodConfig[index].sum.Split('_');
                for (int i = 0; i < 3; i++)
                {
                    if (i < str_0.Length)
                    {
                        Method_sum[i] = Convert.ToInt16(str_0[i]);
                    }
                    else
                    {
                        Method_sum[i] = 0;
                    }

                }
                Method_sum[3] = 0;
                Method_sum[4] = 0;
            }

            //  Debug.LogError("Pay" + mcm._dicMethodConfig[index].pay);
            //获取玩家对应的支付数量
            if (mcm._dicMethodConfig[index].pay.Contains("|"))
            {
                string[] str_0 = mcm._dicMethodConfig[index].pay.Split('|')[0].Split('_');
                string[] str_1 = mcm._dicMethodConfig[index].pay.Split('|')[1].Split('_');
                for (int i = 0; i < 3; i++)
                {
                    Method_pay[i] = Convert.ToInt16(str_0[i]);
                }

                for (int i = 0; i < 2; i++)
                {
                    Method_pay[i + 3] = Convert.ToInt16(str_1[i]);
                }
            }
            else
            {
                string[] str_0 = mcm._dicMethodConfig[index].pay.Split('_');
                for (int i = 0; i < 3; i++)
                {
                    if (i < str_0.Length)
                    {
                        Method_pay[i] = Convert.ToInt16(str_0[i]);
                    }
                    else
                    {
                        Method_pay[i] = 0;
                    }
                }
                Method_pay[3] = 0;
                Method_pay[4] = 0;
            }


            //产生之前删除之前的数据
            if (MethodNum.transform.Find("GameNumbers").childCount > 0)
            {
                CreatRoomChoiceMethod[] temp = MethodNum.transform.Find("GameNumbers").GetComponentsInChildren<CreatRoomChoiceMethod>();
                for (int i = 0; i < temp.Length; i++)
                {
                    Destroy(temp[i].gameObject);
                }
            }

            int method_Index = 0;
            //产生对应的玩法数据
            for (int i = 0; i < Method_sum.Length; i++)
            {
                // Debug.LogError("Method_sum" + Method_sum[i]);
                if (Method_sum[i] == 0)
                {
                    continue;
                }

                GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/PlayerMethodPanel/CreatRoomChoiceMethod"));
                go.name = "CreatRoomChoiceMethod";
                go.transform.SetParent(MethodNum.transform.Find("GameNumbers"));
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                go.transform.localScale = Vector3.one;
                go.GetComponent<Toggle>().group = MethodNum.transform.Find("GameNumbers").GetComponent<ToggleGroup>();
                CreatRoomChoiceMethod crcm = go.GetComponent<CreatRoomChoiceMethod>();
                method_num[method_Index++] = go;
                if (i > 2)
                {
                    crcm.type = Method_type[1];
                }
                else
                {
                    crcm.type = Method_type[0];
                }
                crcm.num = Method_sum[i];
                crcm.pay = Method_pay[i];
                if (crmpd.isSavedParamMethid)
                {
                    uint id = mcm.ReadInt32toInt4(crmpd.roomMessage_[1], 16);
                    //Debug.LogError("id " + id + "count " + count);

                    if (i == id)
                    {
                        go.GetComponent<Toggle>().isOn = true;
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        go.GetComponent<Toggle>().isOn = true;
                    }
                }
                if (chaxun)
                    crcm.UpdateShow();
            }
            MethodNum.transform.GetComponent<LayoutElement>().minHeight = 60f + ((int)0.5f) * 45;
            //Debug.LogError("id;"+mcm.ReadInt32toInt4(crmpd.roomMessage_[1], 16));
            if (index == 20015)
            {
                m_PlayModth.SetActive(true);
                // Rect.GetComponent<ScrollRect>().enabled = true;
                CreatRoomChoiceMethod[] temp = m_PlayModthParent.GetComponentsInChildren<CreatRoomChoiceMethod>();
                if (temp.Length > 0)
                {
                    for (int i = 0; i < temp.Length; i++)
                    {
                        Destroy(temp[i].gameObject);
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/PlayerMethodPanel/CreatRoomChoiceMethod"));
                    go.name = "CreatRoomChoiceMethod";
                    go.transform.SetParent(m_PlayModthParent.transform);
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.localScale = Vector3.one;
                    go.GetComponent<Toggle>().group = MethodNum.transform.Find("GameNumbers").GetComponent<ToggleGroup>();
                    CreatRoomChoiceMethod crcm = go.GetComponent<CreatRoomChoiceMethod>();
                    method_num[method_Index++] = go;

                    crcm.type = 2;
                    crcm.num = (i + 1) * 4;
                    crcm.pay = Method_pay[i];

                    if (crmpd.isSavedParamMethid)
                    {
                        uint id = mcm.ReadInt32toInt4(crmpd.roomMessage_[1], 16);

                        if ((i + 3) == id)
                        {
                            go.GetComponent<Toggle>().isOn = true;
                        }
                    }
                    if (chaxun)
                        crcm.UpdateShow();
                }

            }
            else
            {
                if (m_PlayModth.activeInHierarchy)
                {
                    m_PlayModth.SetActive(false);
                }
            }

            //玩法更新
            if (chaxun)
                UpdateMethodMessage(index);
        }

        /// <summary>
        /// 更新玩法界面
        /// </summary>
        /// <param name="index">玩法id</param>
        void UpdateMethodMessage(int index)
        {
            // GameData.Instance.CreatRoomMessagePanelData.iParamOpenRoomMessage = new int[GameData.Instance.CreatRoomMessagePanelData.iParamOpenRoomMessage.Length];
            for (int i = 0; i < RuleParent.Length - 1; i++)
            {
                Toggle[] tog = new Toggle[RuleParent[i].GetComponentsInChildren<Toggle>().Length];
                tog = RuleParent[i].GetComponentsInChildren<Toggle>();
                if (tog.Length > 0)
                {
                    for (int k = 0; k < tog.Length; k++)
                    {
                        DestroyImmediate(tog[k].gameObject);
                    }
                }
            }
            int num_only = 0;  //表示产生单选按钮的数量
            int num_mul = 0;  //表示产生多选按钮的数量     
            int[] num_row = new int[5]; //保存玩法的行数       
            GameData gd = GameData.Instance;
            CreatRoomMessagePanelData crmpd = gd.CreatRoomMessagePanelData;
            MahjongCommonMethod mcm = MahjongCommonMethod.Instance;

            //表示该玩法需要选择玩法
            if (mcm._dicmethodToCardType.ContainsKey(index))
            {
                //重置位置的标志位
                bool[] isInitPos = new bool[] { false, false };
                //该玩法有多个规则可以实现
                for (int i = 0; i < mcm._dicmethodToCardType[index].Count; i++)
                {
                    GameObject go = null;
                    //如果该玩法为多选
                    if (mcm._dicmethodToCardType[index][i].Choice == 2)
                    {
                        go = Instantiate(Resources.Load<GameObject>("Lobby/PlayerMethodPanel/MultiChoiceBtn"));
                        go.name = "MultiChoiceBtn";
                        go.GetComponent<Toggle>().isOn = false;
                        if (mcm._dicmethodToCardType[index][i].Hierarchy == 1)
                        {
                            num_mul++;
                        }

                        if (go.GetComponent<CheckLongPressClick>() != null)
                        {
                            go.GetComponent<CheckLongPressClick>().RuleId = mcm._dicmethodToCardType[index][i].RuleId;
                        }

                        //如果第二层存放多选按钮，则调整父物体位置
                        if (mcm._dicmethodToCardType[index][i].Hierarchy == 2)
                        {
                            isInitPos[0] = true;
                        }
                        else if (mcm._dicmethodToCardType[index][i].Hierarchy == 3)
                        {
                            isInitPos[1] = true;
                        }
                    }
                    //如果单选
                    else
                    {
                        num_only++;
                        go = Instantiate(Resources.Load<GameObject>("Lobby/PlayerMethodPanel/OnlyOneChoice"));
                        if (RuleParent[mcm._dicmethodToCardType[index][i].Hierarchy - 1].GetComponent<ToggleGroup>() == null)
                        {
                            RuleParent[mcm._dicmethodToCardType[index][i].Hierarchy - 1].AddComponent<ToggleGroup>();
                        }
                        go.GetComponent<Toggle>().group = RuleParent[mcm._dicmethodToCardType[index][i].Hierarchy - 1].GetComponent<ToggleGroup>();
                        go.name = "OnlyOneChoice";
                    }

                    go.transform.SetParent(RuleParent[mcm._dicmethodToCardType[index][i].Hierarchy - 1].transform);
                    //  Debug.LogError(index);
                    //   Debug.LogError(mcm._dicmethodToCardType[index][i].Hierarchy - 1);

                    num_row[mcm._dicmethodToCardType[index][i].Hierarchy - 1] = 1;

                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.localScale = Vector3.one;
                    go.transform.Find("Label").GetComponent<Text>().text = mcm._dicMethodCardType[index][i].card_type;
                    //go.transform.GetComponent<CheckLongPressClick>().Content = mcm._dicMethodCardType[index][i].notes;
                    //为规则添加点击事件
                    AddClickDelgete_Multichoice(go.GetComponent<Toggle>(), mcm._dicMethodCardType[index][i].ID);
                    int options = 0;
                    int ruleID = mcm._dicmethodToCardType[index][i].RuleId;

                    if (!crmpd.isSavedParamMethid)
                    {
                        options = mcm._dicmethodToCardType[index][i].Options;
                    }
                    else
                    {
                        options = mcm.GetPlayerParamRuleId(ruleID);
                    }
                    //Debug.LogError("index = " + mcm._dicmethodToCardType[index][i].Options);
                    Debug.Log("crmpd.iParamMethid：" + crmpd.isSavedParamMethid + ",index:" + index +
                      "mcm._dicMethodCardType[index][i].ID:" + mcm._dicMethodCardType[index][i].ID
                          + "," + options);
                    if (options == 0)
                    {
                        go.GetComponent<Toggle>().isOn = false;
                        go.transform.Find("Label").GetComponent<Text>().color = NormalColor;

                    }
                    else
                    {
                        go.GetComponent<Toggle>().isOn = true;
                        go.transform.Find("Label").GetComponent<Text>().color = ChangeClolor;
                    }

                    if (go.transform.Find("Label").GetComponent<Text>().text == "抢庄" && UIMainView.Instance.CreatRoomMessagePanel.QuanBukexuan)
                    {
                        go.transform.GetChild(0).gameObject.SetActive(false);
                        go.transform.GetChild(2).gameObject.SetActive(true);
                        go.transform.Find("Label").GetComponent<Text>().color = new Color(143 / 255.0f, 143 / 255.0f, 143 / 255.0f, 1);//银色
                    }
                }
            }
            /////////////////////////////////////////////////////////
            if (AdvanceSetting[2] != null && AdvanceSetting[3] != null)
            {
                //托管
                AdvanceSetting[2].transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = lTuoGuan[0];
                AddClickDelgete_Multichoice(AdvanceSetting[2].transform.GetChild(0).GetChild(0).GetComponent<Toggle>(), 100);

                AdvanceSetting[2].transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = lTuoGuan[1];
                AddClickDelgete_Multichoice(AdvanceSetting[2].transform.GetChild(0).GetChild(1).GetComponent<Toggle>(), 101);

                AdvanceSetting[2].transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Text>().text = lTuoGuan[2];
                AddClickDelgete_Multichoice(AdvanceSetting[2].transform.GetChild(0).GetChild(2).GetComponent<Toggle>(), 102);

                AdvanceSetting[2].transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<Text>().text = lTuoGuan[3];
                AddClickDelgete_Multichoice(AdvanceSetting[2].transform.GetChild(0).GetChild(3).GetComponent<Toggle>(), 103);

                //预约
                AddClickDelgete_Multichoice(AdvanceSetting[3].transform.GetChild(0).GetChild(0).GetComponent<Toggle>(), 200);
                AddClickDelgete_Multichoice(AdvanceSetting[3].transform.GetChild(0).GetChild(1).GetComponent<Toggle>(), 201);
            }
            if (m_CreateRoomSource == 1 || GameData.Instance.PlayerNodeDef.iMyParlorId <= 0)
            {
                RoomCost.transform.GetChild(0).GetComponent<Toggle>().interactable = true;
                RoomCost.transform.GetChild(1).GetComponent<Toggle>().interactable = true;
            }
            else
            {
                RoomCost.transform.GetChild(0).GetComponent<Toggle>().interactable = false;
                RoomCost.transform.GetChild(1).GetComponent<Toggle>().interactable = false;
            }
            AddClickDelgete_Multichoice(RoomCost.transform.GetChild(0).GetComponent<Toggle>(), 400);//房主支付
            AddClickDelgete_Multichoice(RoomCost.transform.GetChild(1).GetComponent<Toggle>(), 401);
            if (mcm.ReadColumnValue(crmpd.roomMessage_, 2, 39) == 0)
            {
                mcm.WriteColumnValue(ref crmpd.roomMessage_, 39, 2, 2);
            }
            if (mcm.ReadColumnValue(crmpd.roomMessage_, 2, 39) <= 1)
            {
                RoomCost.transform.GetChild(1).GetComponent<Toggle>().isOn = true;
                RoomCost.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
            }
            if (mcm.ReadColumnValue(crmpd.roomMessage_, 2, 39) == 2)
            {
                RoomCost.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
                RoomCost.transform.GetChild(1).GetComponent<Toggle>().isOn = false;
            }
            /////////////////////////////////////////////////////////
            //Debug.LogError("num_mul = "+ num_mul+ ",num_only" + num_only+",index"+index);
            //如果单选和多选为零，则关闭游戏玩法选择
            if (num_mul + num_only == 0 )
            {
                LayoutMent_All[1].gameObject.SetActive(false);
            }
            else
            {
                LayoutMent_All[1].gameObject.SetActive(true);
            }

            //if (RuleParent[0].GetComponentsInChildren<Toggle>().Length >= 1 && RuleParent[0].GetComponentsInChildren<Toggle>()[0].name.Contains("OnlyOneChoice") && num_only > 0)
            //{
            //    RuleParent[0].GetComponent<GridLayoutGroup>().padding.left = 30;
            //}
            //else
            //{
            //    RuleParent[0].GetComponent<GridLayoutGroup>().padding.left = 55;
            //}

            if (num_mul + num_only == 0)
            {
                RuleParent[0].transform.parent.gameObject.SetActive(false);
                //RuleParent_.transform.Find("PlayerMothed/Method/Method_1").gameObject.SetActive(false);
            }
            else
            {
                RuleParent[0].transform.parent.gameObject.SetActive(true);
                //RuleParent_.transform.Find("PlayerMothed/Method/Method_1").gameObject.SetActive(true);
                if (num_mul <= 4)
                {
                    RuleParent[0].GetComponentInParent<LayoutElement>().minHeight = 60f;
                }
                else
                {
                    RuleParent[0].GetComponentInParent<LayoutElement>().minHeight = 143;
                }
            }


            int count = 0;

            for (int i = 0; i < 3; i++)
            {
                if (num_row[i] > 0)
                {
                    RuleParent[i].SetActive(true);
                    count++;
                }
                else
                {
                    RuleParent[i].SetActive(false);
                }
            }

            if (!RuleParent_.gameObject.activeInHierarchy)
            {
                return;
            }

            //if (count == 1)
            //{
            //    RuleParent_.GetComponentInParent<LayoutElement>().minHeight = 65f;
            //}
            //else
            //{
            //    RuleParent_.GetComponentInParent<LayoutElement>().minHeight = 50f * count + 45f + (int)(num_mul / 4f - 0.5f) * 50f;
            //}
        }

        /// <summary>
        /// 产生对应的玩法按钮
        /// </summary>
        public void SpwanPlayMethodBtn()
        {
           // Debug.LogError("产生玩法");
            MahjongCommonMethod mcm = MahjongCommonMethod.Instance;
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;

            CityConnectCounty[] prefab = Grid_MethodBtn.GetComponentsInChildren<CityConnectCounty>();
            if (prefab.Length > 0)
            {
                for (int i = 0; i < prefab.Length; i++)
                {
                    Destroy(prefab[i].gameObject);
                }
            }

            if (crmpd.CreatRoomType > 1)
            {
                mcm.lsMethodId = new List<int>();
                for (int k = 0; k < mcm._dicPlayNameConfig[GameData.Instance.ParlorShowPanelData.iCountyId[2]].Count; k++)
                {
                    Debug.LogWarning("产生对应的玩法按钮"+ mcm._dicPlayNameConfig[GameData.Instance.ParlorShowPanelData.iCountyId[2]][k].METHOD);
                    mcm.lsMethodId.Add(mcm._dicPlayNameConfig[GameData.Instance.ParlorShowPanelData.iCountyId[2]][k].METHOD);
                }
            }

            // 玩法：
            int iCountyId_ = 0;
            int iPlayingMethod_ = 0;
            if (PlayerPrefs.HasKey("iPlayingMethod" + GameData.Instance.PlayerNodeDef.iUserId))
            {
                iPlayingMethod_ = PlayerPrefs.GetInt("iPlayingMethod" + GameData.Instance.PlayerNodeDef.iUserId);
            }
            GameObject objTemp = null;
            // 开房所属县id
            //int iCountyId_ = 0;
            if (PlayerPrefs.HasKey("iPlayingMethod" + GameData.Instance.PlayerNodeDef.iUserId))
            {
                iCountyId_ = PlayerPrefs.GetInt("iCountyId" + GameData.Instance.PlayerNodeDef.iUserId);
            }
            for (int i = 0; i < mcm.lsMethodId.Count; i++)
            {
                if (SDKManager.Instance.CheckStatus == 1)
                {
                    if (i > 1)
                    {
                        break;
                    }
                }
                GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/PlayerMethodPanel/Method"));
                go.SetActive(true);
                go.name = "btn_" + mcm.lsMethodId[i];
                Debug.LogWarning("iPlayingMethod_" + iPlayingMethod_);
                Debug.LogWarning("mcm.lsMethodId[i]" + mcm.lsMethodId[i]);
                go.transform.SetParent(Grid_MethodBtn.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                CityConnectCounty message = go.transform.GetComponent<CityConnectCounty>();
                //Debug.LogError("iCountyId_:" + iCountyId_ + "," + sapd.iCountyId+","+ iPlayingMethod_);
                if (iCountyId_ == sapd.iCountyId && iPlayingMethod_ != 0)
                {
                    if (iPlayingMethod_ == mcm.lsMethodId[i])//if (i == 0)
                    {
                        message.status[0].SetActive(true);
                        if (message.status[1].activeInHierarchy)
                        {
                            message.status[1].SetActive(false);
                        }
                        crmpd.MethodId = iPlayingMethod_;
                        //Debug.LogError("===============================3");

                        SystemMgr.Instance.CreatRoomMessageSystem.UpdateMethodShow(crmpd.MethodId);
                    }
                    else
                    {
                        if (message.status[0].activeInHierarchy)
                        {
                            message.status[0].SetActive(false);

                        }
                        message.status[1].SetActive(true);
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        message.status[0].SetActive(true);
                        if (message.status[1].activeInHierarchy)
                        {
                            message.status[1].SetActive(false);
                        }
                    }
                    else
                    {
                        if (message.status[0].activeInHierarchy)
                        {
                            message.status[0].SetActive(false);
                        }
                        message.status[1].SetActive(true);
                    }
                }                
                int temp = 0;
                if (crmpd.CreatRoomType > 1)
                {
                    temp = GameData.Instance.ParlorShowPanelData.iCountyId[2];
                }
                else
                {
                    temp = sapd.iCountyId;
                }
                for (int l = 0; l < mcm._dicPlayNameConfig[temp].Count; l++)
                {
                    if (mcm._dicPlayNameConfig[temp][l].METHOD == mcm.lsMethodId[i])
                    {
                        //如果是审核版本，则删除长治两个字
                        message.status[2].GetComponent<Text>().text = mcm._dicPlayNameConfig[temp][l].METHOD_NAME;
                        message.status[3].GetComponent<Text>().text = mcm._dicPlayNameConfig[temp][l].METHOD_NAME;
                    }
                }
                //为按钮动态添加点击事件
                AddClickDelegate(go.GetComponent<Button>());
                if (objTemp==null)
                {
                    objTemp = go;
                }
            }
            BtnGameMethod(objTemp);
        }
        void OnNoChoice(int index)
        {

            Data.CreatRoomMessagePanelData cd = GameData.Instance.CreatRoomMessagePanelData;
             //Debug.Log ("初始化:"+index);
            //int index = Convert.ToInt32(name.Split('_')[1]);
            bool isContainMethord = false;
            for (int i = 0; i < cd.allRoomMethor.Count; i++)
            {
                // Debug.LogError("++"+cd.allRoomMethor[i].methord  + "_" + cd.allRoomMethor[i].param [0].ToString("X8") + "_" + cd.allRoomMethor[i].param[1].ToString("X8") + "_" + cd.allRoomMethor[i].param[2].ToString("X8") + "_" + cd.allRoomMethor[i].param[3].ToString("X8") + "_" + cd.allRoomMethor[i].param[4].ToString("X8") + "_" + cd.allRoomMethor[i].param[5].ToString("X8"));
                if (index == cd.allRoomMethor[i].methord)
                {
                    isContainMethord = true;
                    cd.allRoomMethor[i] = new Data.CreatRoomMessagePanelData.MethordRuleClass(index, cd.roomMessage_);
                    // Debug.LogError("保存：" + index + "_" + cd.roomMessage_[0].ToString("X8") + "_" + cd.roomMessage_[1].ToString("X8") + "_" + cd.roomMessage_[2].ToString("X8") + "_" + cd.roomMessage_[3].ToString("X8") + "_" + cd.roomMessage_[4].ToString("X8") + "_" + cd.roomMessage_[5].ToString("X8"));
                }
            }
            if (!isContainMethord)
            {
                cd.allRoomMethor.Add(new Data.CreatRoomMessagePanelData.MethordRuleClass(index, cd.roomMessage_));
                //  Debug.LogError("保存：" + index + "_" + cd.roomMessage_[0].ToString("X8") + "_" + cd.roomMessage_[1].ToString("X8") + "_" + cd.roomMessage_[2].ToString("X8") + "_" + cd.roomMessage_[3].ToString("X8") + "_" + cd.roomMessage_[4].ToString("X8") + "_" + cd.roomMessage_[5].ToString("X8"));
            }
        }

        void AddClickDelegate(Button go)
        {
            go.onClick.AddListener(delegate () { BtnGameMethod(go.gameObject); });
        }

        /// <summary>
        /// 点击选择房间玩法
        /// </summary>
        /// <param name="g0"></param>
        public void BtnGameMethod(GameObject go)
        {
            Debug.LogWarning("模拟点击选择玩法页签"+go.name);
            if (m_showNew[0] != null)
            {
                if (m_showNew[0].activeInHierarchy)
                {
                    m_showNew[0].SetActive(false);
                }
            }

            //修改自己本身的图片
            if (go == null)
            {
                return;
            }
            //显示选中按钮状态
            Button[] button = Grid_MethodBtn.GetComponentsInChildren<Button>();
            for (int i = 0; i < button.Length; i++)
            {
                if (string.Equals(button[i].name, go.name))
                {
                    go.transform.GetComponent<CityConnectCounty>().status[0].SetActive(true);
                    go.transform.GetComponent<CityConnectCounty>().status[1].SetActive(false);
                }
                else
                {
                    if (button[i].transform.GetComponent<CityConnectCounty>().status[0].activeInHierarchy)
                    {
                        button[i].transform.GetComponent<CityConnectCounty>().status[0].SetActive(false);
                        OnNoChoice(Convert.ToInt32(button[i].name.Split('_')[1]));
                        button[i].transform.GetComponent<CityConnectCounty>().status[1].SetActive(true);
                    }
                }
            }

            Messenger_anhui<GameObject>.Broadcast(MESSAGE_PLAYMETHOD, go);

            //float height = 0;
            //for (int i = 0; i < LayoutMent_All.Length; i++)
            //{
            //    height += LayoutMent_All[i].minHeight;
            //}

            //if (height > 375f || m_PlayModth.activeSelf)
            //{
            //    Rect.GetComponent<ScrollRect>().enabled = true;
            //}
            //else
            //{
            //    Rect.GetComponent<ScrollRect>().enabled = false;
            //}
        }
        /// <summary>
        /// 点击确定按钮
        /// </summary>
        public void BtnOk()
        {
            //这个判断有错，这个判断需要写的是老板和普通成员的区别
            OnNoChoice(GameData.Instance.CreatRoomMessagePanelData.MethodId );
            if (m_CreateRoomSource != 1 && isCreateRoomClock)
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                int time = OnSubWaitForTimeStartGame();
                if (time < 0)
                    UIMgr.GetInstance().GetUIMessageView().Show("预约创建房间时间不的低于5分钟！");
                else
                {
                    CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
                    crmpd.roomMessage_[4] = (uint)time;
                    Debug.LogError("预约房：" + crmpd.roomMessage_[4] + "," + time);
                    BtnOk_ok();
                }
            }
            else
            {
                BtnOk_ok();
            }
            if (m_showNew[0] != null) m_showNew[0].SetActive(false);

            OnSetInitCreatRoomInfo();
        }

        public void BtnOk_ok()
        {
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            if (m_showNew[0] != null) m_showNew[0].SetActive(false);

            if (((int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now) - crmpd.LastCreatTime) < 1)
            {
                return;
            }

            Messenger_anhui.Broadcast(MESSAGE_OKBTN);
        }

        public void BtnOk_Close()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            BossClockCreateRoom.SetActive(false);
            if (m_showNew[0] != null) m_showNew[0].SetActive(false);
        }

        /// <summary>
        /// 点击取消按钮
        /// </summary>
        public void BtnCanel()
        {
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            for (int i = 0; i < crmpd.iParamOpenRoomMessage.Length; i++)
            {
                crmpd.iParamOpenRoomMessage[i] = 0;
            }
            OnSetInitCreatRoomInfo();
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_CANELBTN);
        }

        //为规则按钮添加点击事件
        void AddClickDelgete_Multichoice(Toggle go, int index)
        {
            go.onValueChanged.AddListener((bool isOn) => { BtnChoiceRule(index, isOn, go); });
        }
        List<string> SelectContent = new List<string>();
        int[] iChoiceQTHH = new int[2];  //玩家选择前台后和的标志位
        /// <summary>
        /// 规则的点击事件
        /// </summary>
        /// <param name="index"></param>
        public void BtnChoiceRule(int index, bool status, Toggle go)
        {
            Debug.LogWarningFormat("++++++++++++index:{0},status:{1}", index, status);
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            //bool isNotSetNextSelect = true;
            //switch (index)
            //{
            //    case 100: isNotSetNextSelect = false; break;
            //    case 101: isNotSetNextSelect = false; break;
            //    case 102: isNotSetNextSelect = false; break;
            //    case 103: isNotSetNextSelect = false; break;
            //    case 200: isNotSetNextSelect = false; break;
            //    case 201: isNotSetNextSelect = false; break;
            //    case 400: isNotSetNextSelect = false; break;
            //    case 401: isNotSetNextSelect = false; break;
            //}
            //if (isNotSetNextSelect)
            //{
            //    //保存选择规则的数组下标
            //    int id = 0;
            //    if (index / 1000 == 1)
            //    {
            //        id = index % 1000;
            //    }
            //    else if (index / 1000 == 2)
            //    {
            //        id = index % 1000 + 7;
            //    }
            //    else if (index / 1000 == 3)
            //    {
            //        id = index % 1000 + 20;
            //    }
            //    //Debug.LogError("玩家选择的信息:" + index + ",规则id：" + id + ",status：" + status);
            //    //保存玩家
            //    crmpd.iParamOpenRoomMessage[id - 1] = bool2sbyte(status);
            // }
            MahjongCommonMethod mcm = MahjongCommonMethod.Instance;
            switch (index)
            {
                #region 特殊牌型  安徽暂时不考虑
                //十三幺
                case 1001:
                    #region 十三幺

                    break;
                #endregion
                //七对
                case 1002:
                    #region 七对
                    //  Debug.LogError("七对有选择");
                    //如果玩家没选择可接炮 ，默认选择 
                    mcm.WriteColumnValue(ref crmpd.roomMessage_, 6, bool2sbyte(status), 2);
                    break;
                #endregion
                //豪华七对
                case 1003:
                    #region 豪华七对                   
                    break;
                #endregion
                //十三不靠
                case 1004:
                    #region 十三不靠

                    break;
                #endregion
                //明杠收三家
                case 1005:
                    #region 明杠收三家

                    break;
                #endregion
                //四杠荒庄
                case 1006:
                    #region 四杠荒庄

                    break;
                #endregion
                //自摸翻倍
                case 1007:
                    #region 自摸翻倍                  
                    break;
                #endregion
                //一炮一响
                case 2002:
                    #region 一炮一响                    
                    break;
                #endregion
                //无前抬后和
                case 2004:
                    #region 无前抬后和

                    break;
                #endregion
                //前抬
                case 2005:
                    #region 前抬

                    break;
                #endregion
                //后和
                case 2006:
                //接炮收三家
                case 2007:
                    #region 接炮收三家    
                    if (status)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 15, 1, 2);
                    }
                    break;
                #endregion
                //接炮一家
                case 2008:
                    #region 接炮一家
                    if (status)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 15,0, 2);
                    }
                    break;
                #endregion
                //红中算码
                case 2009:
                    #region 红中算码
                    mcm.WriteColumnValue(ref crmpd.roomMessage_, 28, bool2sbyte(status), 2);
                    break;
                    #endregion 红中算码

                    #region 后和

                    break;
                #endregion
                //多人付
                case 2010:
                    #region 多人付                   
                    break;
                #endregion                          
                //带字牌
                case 2018:
                    #region 带字牌

                    break;
                #endregion
                //点炮三分
                case 2019:
                    #region 点炮三分
                    break;
                #endregion
                //轮庄
                case 2020:
                    #region 抢庄                   
                    break;
                #endregion
                //放炮玩法
                case 2021:
                    #region 放炮玩法                  
                    break;
                #endregion
                //带庄
                case 2022:
                    #region 带庄
                    break;
                #endregion
                //自摸胡
                case 2023:
                    #region 自摸胡                   
                    break;
                #endregion
                //可吃牌
                case 3003:
                    #region 可吃牌                  
                    #endregion
                    break;
                //无码
                case 3004:
                    #region 无码    
                    if (crmpd.MethodId == 20001 && status)
                    {
                        mcm.WriteInt32toInt4(ref crmpd.roomMessage_[0], 0, 16);
                    }
                    #endregion
                    break;
                //扎二码
                case 3005:
                    #region 扎二码   
                    if (crmpd.MethodId == 20001 && status)
                    {
                        mcm.WriteInt32toInt4(ref crmpd.roomMessage_[0], 2, 16);
                    }
                    #endregion
                    break;
                //扎四码
                case 3006:
                    #region 扎四码   
                    if (crmpd.MethodId == 20001 && status)
                    {
                        mcm.WriteInt32toInt4(ref crmpd.roomMessage_[0], 4, 16);
                    }
                    #endregion
                    break;
                //扎六码
                case 3007:
                    #region 扎六码 
                    if (crmpd.MethodId == 20001 && status)
                    {
                        mcm.WriteInt32toInt4(ref crmpd.roomMessage_[0], 6, 16);
                    }
                    #endregion
                    break;
                #endregion
                //一炮多响
                case 2003:
                    //如果玩家没选择可接炮 ，默认选择                        
                    if (crmpd.MethodId == 20016)
                    {
                        ChangeReleaveStatus(3001, 0);
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 12, bool2sbyte(status), 2);
                    }
                    break;
                //可接炮
                case 3001:
                    mcm.WriteColumnValue(ref crmpd.roomMessage_, 27, bool2sbyte(status), 2);
                    break;
                //1嘴
                case 3014:
                    if (crmpd.MethodId == 20016 && status)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 25, 1, 2);
                    }
                    break;
                //5嘴
                case 3015:
                    if (crmpd.MethodId == 20016 && status)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 25, 2, 2);
                    }
                    break;
                //10嘴
                case 3016:
                    if (crmpd.MethodId == 20016 && status)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 25, 3, 2);
                    }
                    break;
                //平胡
                case 3017:
                    if (crmpd.MethodId == 20016 && status)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 41, 1, 2);
                    }
                    break;
                //大胡
                case 3018:
                    if (crmpd.MethodId == 20016 && status)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 41, 2, 2);
                    }
                    break;
                //坎
                case 3024:
                    if (crmpd.MethodId == 20015)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 31, bool2sbyte(status), 2);
                    }
                    break;
                //缺门
                case 3025:
                    if (crmpd.MethodId == 20015)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 30, bool2sbyte(status), 2);
                    }
                    break;
                //点数
                case 3027:
                    if (crmpd.MethodId == 20015)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 42, bool2sbyte(status), 2);
                    }
                    break;
                //见面
                case 3028:
                    if (crmpd.MethodId == 20015)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 32, bool2sbyte(status), 2);
                    }
                    break;
                //幺九将
                case 3029:
                    if (crmpd.MethodId == 20015)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 33, bool2sbyte(status), 2);
                    }
                    break;
                //六连
                case 3030:
                    if (crmpd.MethodId == 20015)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 34, bool2sbyte(status), 2);
                    }
                    break;
                //断独幺
                case 3031:
                    if (crmpd.MethodId == 20015)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 35, bool2sbyte(status), 2);
                    }
                    break;
                //抢杠全包
                case 3032:

                    if (crmpd.MethodId == 20001)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 40, bool2sbyte(status), 2);
                    }
                    break;
                ///抢杠无红中
                case 3033:
                    if (crmpd.MethodId == 20001)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 36, bool2sbyte(status), 2);
                    }
                    break;
                //荒庄荒杠
                case 3034:
                    if (crmpd.MethodId == 20001)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 37, bool2sbyte(status), 2);
                    }
                    break;
                //扎一码
                case 3035:
                    if (crmpd.MethodId == 20001 && status)
                    {
                        mcm.WriteInt32toInt4(ref crmpd.roomMessage_[0], 1, 17);
                    }
                    break;
                //原板记分
                case 3036:
                    if (crmpd.MethodId == 20001 && status)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 38, 1, 2);
                    }
                    break;
                //新版记分
                case 3037:
                    if (crmpd.MethodId == 20001 && status)
                    {
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 38, 2, 2);
                    }
                    break;
                //一番起胡
                case 3038:
                    if (status)
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 45, 0, 2);
                    break;
                //俩翻起胡
                case 3039:
                    if (status)
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 45, 1, 2);
                    break;
                case 3040:
                    //带配子
                    if (status)
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 43, 0, 2);
                    break;
                //不带配子
                case 3041:
                    if (status)
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 43, 1, 2);
                    break;
                //白皮配子
                case 3042:
                    if (status)
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 43, 2, 2);
                    break;
                //字牌30番
                case 3043:
                    if (status)
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 44, 0, 2);
                    break;
                //字牌60番
                case 3044:
                    if (status)
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 44, 1, 2);
                    break;
                //字牌100番
                case 3045:
                    if (status)
                        mcm.WriteColumnValue(ref crmpd.roomMessage_, 44, 2, 2);
                    break;
                //托管
                case 100:
                    #region 托管100
                    if (status)
                    {
                        crmpd.roomMessage_[5] = 0;
                        if (m_showNew[1] != null) m_showNew[1].SetActive(false);
                    }
                    break;
                #endregion
                case 101:
                    #region 托管101
                    if (status)
                    {
                        string str = lTuoGuan[1];
                        str = str.Substring(0, lTuoGuan[1].Length - 1);
                        crmpd.roomMessage_[5] = (uint)(Convert.ToInt32(str) / 60);//以分显示
                        if (m_showNew[1] != null) m_showNew[1].SetActive(false);
                    }
                    break;
                #endregion
                case 102:
                    #region 托管102
                    if (status)
                    {
                        string str = lTuoGuan[2];
                        str = str.Substring(0, lTuoGuan[2].Length - 1);
                        crmpd.roomMessage_[5] = (uint)(Convert.ToInt32(str) / 60);//以分显示
                        if (m_showNew[1] != null) m_showNew[1].SetActive(false);
                    }
                    break;
                #endregion
                case 103:
                    #region 托管103
                    if (status)
                    {
                        string str = lTuoGuan[3];
                        str = str.Substring(0, lTuoGuan[3].Length - 1);
                        crmpd.roomMessage_[5] = (uint)(Convert.ToInt32(str) / 60);//以分显示
                        if (m_showNew[1] != null) m_showNew[1].SetActive(false);
                    }
                    break;
                #endregion
                //预约
                case 200: if (status) { crmpd.roomMessage_[4] = 0; if (m_showNew[2] != null) m_showNew[2].SetActive(false); StartSetValue(); } break;
                case 201: if (status) { crmpd.roomMessage_[4] = 0; if (m_showNew[2] != null) m_showNew[2].SetActive(false); ClockUseValue(); } break;
                //支付方式
                case 400:
                    #region 支付方式400
                    if (status)
                    {
                        MahjongCommonMethod.Instance.WriteColumnValue(ref crmpd.roomMessage_, 39, 2, 2);
                        for (int j = 0; j < MahjongCommonMethod.Instance.methiod.Count; j++)
                        {
                            if (MahjongCommonMethod.Instance._dicMethodConfig[crmpd.MethodId].METHOD == MahjongCommonMethod.Instance.methiod[j].id)
                            {
                                Text[] text0 = method_num[0].GetComponentsInChildren<Text>();
                                for (int method_num_text = 0; method_num_text < text0.Length; method_num_text++)
                                {
                                    if (text0[method_num_text].name == "CardNum")
                                    {
                                        text0[method_num_text].text = "*" + (MahjongCommonMethod.Instance.methiod[j].price1).ToString();
                                    }
                                }
                                Text[] text1 = method_num[1].GetComponentsInChildren<Text>();
                                for (int method_num_text = 0; method_num_text < text1.Length; method_num_text++)
                                {
                                    if (text1[method_num_text].name == "CardNum")
                                    {
                                        text1[method_num_text].text = "*" + (MahjongCommonMethod.Instance.methiod[j].price2).ToString();
                                    }
                                }
                                Text[] text2 = method_num[2].GetComponentsInChildren<Text>();
                                for (int method_num_text = 0; method_num_text < text2.Length; method_num_text++)
                                {
                                    if (text2[method_num_text].name == "CardNum")
                                    {
                                        text2[method_num_text].text = "*" + (MahjongCommonMethod.Instance.methiod[j].price3).ToString();
                                    }
                                }
                                //2
                                if (m_PlayModth.activeSelf)
                                {
                                    Text[] text3 = method_num[3].GetComponentsInChildren<Text>();
                                    for (int method_num_text = 0; method_num_text < text3.Length; method_num_text++)
                                    {
                                        if (text3[method_num_text].name == "CardNum")
                                        {
                                            text3[method_num_text].text = "*" + (MahjongCommonMethod.Instance.methiod[j].price1).ToString();
                                        }
                                    }
                                    Text[] text4 = method_num[4].GetComponentsInChildren<Text>();
                                    for (int method_num_text = 0; method_num_text < text4.Length; method_num_text++)
                                    {
                                        if (text4[method_num_text].name == "CardNum")
                                        {
                                            text4[method_num_text].text = "*" + (MahjongCommonMethod.Instance.methiod[j].price2).ToString();
                                        }
                                    }
                                    Text[] text5 = method_num[5].GetComponentsInChildren<Text>();
                                    for (int method_num_text = 0; method_num_text < text5.Length; method_num_text++)
                                    {
                                        if (text5[method_num_text].name == "CardNum")
                                        {
                                            text5[method_num_text].text = "*" + (MahjongCommonMethod.Instance.methiod[j].price3).ToString();
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    break;//1表示是房主一人支付                    
                #endregion
                case 401:
                    #region 支付方式401
                    if (status)
                    {
                        MahjongCommonMethod.Instance.WriteColumnValue(ref crmpd.roomMessage_, 39, 1, 2);
                        for (int j = 0; j < MahjongCommonMethod.Instance.methiod.Count; j++)
                        {
                            if (MahjongCommonMethod.Instance._dicMethodConfig[crmpd.MethodId].METHOD == MahjongCommonMethod.Instance.methiod[j].id)
                            {
                                Text[] text0 = method_num[0].GetComponentsInChildren<Text>();
                                for (int method_num_text = 0; method_num_text < text0.Length; method_num_text++)
                                {
                                    if (text0[method_num_text].name == "CardNum")
                                    {
                                        text0[method_num_text].text = "*" + (MahjongCommonMethod.Instance.methiod[j].price1 / 4).ToString();
                                    }
                                }
                                Text[] text1 = method_num[1].GetComponentsInChildren<Text>();
                                for (int method_num_text = 0; method_num_text < text1.Length; method_num_text++)
                                {
                                    if (text1[method_num_text].name == "CardNum")
                                    {
                                        text1[method_num_text].text = "*" + (MahjongCommonMethod.Instance.methiod[j].price2 / 4).ToString();
                                    }
                                }
                                Text[] text2 = method_num[2].GetComponentsInChildren<Text>();
                                for (int method_num_text = 0; method_num_text < text2.Length; method_num_text++)
                                {
                                    if (text2[method_num_text].name == "CardNum")
                                    {
                                        text2[method_num_text].text = "*" + (MahjongCommonMethod.Instance.methiod[j].price3 / 4).ToString();
                                    }
                                }
                                //2
                                if (m_PlayModth.activeSelf)
                                {
                                    Text[] text3 = method_num[3].GetComponentsInChildren<Text>();
                                    for (int method_num_text = 0; method_num_text < text3.Length; method_num_text++)
                                    {
                                        if (text3[method_num_text].name == "CardNum")
                                        {
                                            text3[method_num_text].text = "*" + (MahjongCommonMethod.Instance.methiod[j].price1 / 4).ToString();
                                        }
                                    }
                                    Text[] text4 = method_num[4].GetComponentsInChildren<Text>();
                                    for (int method_num_text = 0; method_num_text < text4.Length; method_num_text++)
                                    {
                                        if (text4[method_num_text].name == "CardNum")
                                        {
                                            text4[method_num_text].text = "*" + (MahjongCommonMethod.Instance.methiod[j].price2 / 4).ToString();
                                        }
                                    }
                                    Text[] text5 = method_num[5].GetComponentsInChildren<Text>();
                                    for (int method_num_text = 0; method_num_text < text5.Length; method_num_text++)
                                    {
                                        if (text5[method_num_text].name == "CardNum")
                                        {
                                            text5[method_num_text].text = "*" + (MahjongCommonMethod.Instance.methiod[j].price3 / 4).ToString();
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    break;
                #endregion
                default:
                    Debug.LogError("玩家选择玩法错误：" + index);
                    break;
            }
            if (!mcm._dicMethodCardType.ContainsKey(GameData.Instance.CreatRoomMessagePanelData.MethodId))
            {
                return;
            }

            //添加滚动条提示
            string CarouselRollContent = "";
            for (int i = 0; i < mcm._dicMethodCardType[GameData.Instance.CreatRoomMessagePanelData.MethodId].Count; i++)
            {
                //如果是相同的ID的话                
                if (mcm._dicMethodCardType[GameData.Instance.CreatRoomMessagePanelData.MethodId][i].ID == index)
                {
                    if (status)
                    {
                        CarouselRollContent = mcm._dicMethodCardType[GameData.Instance.CreatRoomMessagePanelData.MethodId][i].notes;
                    }
                    else
                    {
                        CarouselRollContent = mcm._dicMethodCardType[GameData.Instance.CreatRoomMessagePanelData.MethodId][i].notNotes;
                    }
                    DnCarouselRoll(CarouselRollContent);
                    break;
                }
            }

            isClockSelectButton = false;
            OnShowSelect(index, go, status);
            string str1 = "BtnChoiceRule：";
            for (int i = 0; i < crmpd.roomMessage_.Length; i++)
            {
                str1 += " " + crmpd.roomMessage_[i].ToString("X8");
            }
            Debug.LogWarning(str1);
        }

        /// <summary>
        /// 选择的玩法说明
        /// </summary>
        void OnShowSelect(int index, Toggle go, bool status)
        {
            SelectContent.Clear();
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            MahjongCommonMethod mcm = MahjongCommonMethod.Instance;
            //Debug.LogWarning("选择ID" + index + "__" + mcm.GetPlayerParamRuleId(index));
            if (status)
            {
                go.transform.GetChild(1).GetComponent<Text>().color = ChangeClolor;
            }
            else
            {
                go.transform.GetChild(1).GetComponent<Text>().color = NormalColor;
            }
            //for (int i = 0; i < mcm._dicMethodCardType[GameData.Instance.CreatRoomMessagePanelData.MethodId].Count; i++)
            //{
            //    //int id = 0;
            //    //if (mcm._dicMethodCardType[GameData.Instance.CreatRoomMessagePanelData.MethodId][i].ID / 1000 == 1)
            //    //{
            //    //    id = mcm._dicMethodCardType[GameData.Instance.CreatRoomMessagePanelData.MethodId][i].ID % 1000;
            //    //}
            //    //else if (mcm._dicMethodCardType[GameData.Instance.CreatRoomMessagePanelData.MethodId][i].ID / 1000 == 2)
            //    //{
            //    //    id = mcm._dicMethodCardType[GameData.Instance.CreatRoomMessagePanelData.MethodId][i].ID % 1000 + 7;
            //    //}
            //    //else if (mcm._dicMethodCardType[GameData.Instance.CreatRoomMessagePanelData.MethodId][i].ID / 1000 == 3)
            //    //{
            //    //    id = mcm._dicMethodCardType[GameData.Instance.CreatRoomMessagePanelData.MethodId][i].ID % 1000 + 20;
            //    //}
            //    ////////////////////////////////////////////////////////// 判断选择那个了，然后变为红色
            //    Text changeLabel2 = null;
            //    Text[] label2 = RuleParent_.GetComponentsInChildren<Text>();
            //    for (int j = 0; j < label2.Length; j++)
            //    {
            //        if (label2[j].text == mcm._dicMethodCardType[GameData.Instance.CreatRoomMessagePanelData.MethodId][i].card_type)
            //        {
            //            {
            //                changeLabel2 = label2[j];
            //                break;
            //            }
            //        }
            //    }
            //    if (mcm.GetPlayerParamRuleId(index) == 1)
            //    {
            //        if (changeLabel2 != null)
            //        {
            //            if (changeLabel2.transform.parent.GetComponent<Toggle>().isOn)
            //                changeLabel2.color = ChangeClolor;
            //        }

            //        SelectContent.Add(mcm._dicMethodCardType[GameData.Instance.CreatRoomMessagePanelData.MethodId][i].notes);
            //    }
            //    else
            //    {
            //        if (changeLabel2 != null)
            //            changeLabel2.color = NormalColor;
            //    }
            //}
            StartCarouselIndex = 0;
            isClockSelectButton = true;
            timeCarouselStart = 0;
        }

        /// <summary>
        /// 使创建放界面上那个按钮变为红色而使用的
        /// </summary>
        public void UpdataToShowForSelectUseGoldCreatRoom()
        {
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            //Toggle[] toggle_cricle = MethodNum.GetComponentsInChildren<Toggle>();
            //for (int i = 0; i < toggle_cricle.Length; i++)
            //{
            //    if (toggle_cricle[i].isOn)
            //    {
            //     toggle_cricle[i].transform.FindChild("Label").GetComponent<Text>().color = Changelolor;
            //    }
            //    else
            //    {
            //        toggle_cricle[i].transform.FindChild("Label").GetComponent<Text>().color = NormalColor;
            //    }
            //}
        }

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
                    if (SelectContent.Count <= 0) return;
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
            if (GameData.Instance.CreatRoomMessagePanelData.MethodId == 15 || GameData.Instance.CreatRoomMessagePanelData.MethodId == 12)
            {
                CarouselContent.text = "";
                return;
            }

            CarouselContent.text = contentText;

            //先置为空的
            CarouselContent.transform.localPosition = new Vector3(0, -20, 0);
            CarouselContent.color = new Color(1, 1, 1, 1);
            float speed = 1f;
            //之后的操作
            CarouselContent.transform.DOLocalMoveY(0, speed);
            CarouselContent.DOColor(new Color(113 / 255.0f, 85 / 255.0f, 72 / 255.0f, 1), speed);
        }


        //选择断线率
        public void BtnDisConnectRate(int index)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            crmpd.iDisconnectRate = index;
        }

        //选择出牌需求
        public void BtnDiscardTime(int index)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            crmpd.iDiscardTime = index;
        }

        //改变获赞数量
        public void ChangePariseNum(int status)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;

            //处理减赞的数量
            if (status == 1)
            {
                if (crmpd.iCompliment <= 0)
                {
                    return;
                }
                crmpd.iCompliment -= 1;
            }
            //处理加赞数量
            else
            {
                if (crmpd.iCompliment >= 999)
                {
                    return;
                }
                crmpd.iCompliment += 1;
            }

            Compliment.text = crmpd.iCompliment.ToString();

            if (crmpd.iCompliment > GameData.Instance.PlayerNodeDef.iCompliment)
            {
                MahjongCommonMethod.Instance.ShowRemindFrame("不能大于自己当前最大赞数");
                crmpd.iCompliment = GameData.Instance.PlayerNodeDef.iCompliment;
                Compliment.text = crmpd.iCompliment.ToString();
                return;
            }


        }

        /// <summary>
        /// 点击高级设置
        /// </summary>
        public void BtnAdvanceSetting()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;

            if (crmpd.AnvanceSettingStatus == 1)
            {
                if (m_showNew[0] != null) m_showNew[0].SetActive(false);

                if (m_showNew[1] != null)
                {
                    if (!PlayerPrefs.HasKey("NormalShowNew"))
                    {
                        PlayerPrefs.SetInt("NormalShowNew", 0);
                    }
                    if (!PlayerPrefs.HasKey("MahjongShowNew"))
                    {
                        PlayerPrefs.SetInt("MahjongShowNew", 0);
                    }

                    int NormalShowNew_num = PlayerPrefs.GetInt("NormalShowNew");//不在麻将馆内
                    int MahjongShowNew_num = PlayerPrefs.GetInt("MahjongShowNew");//
                    //if (NormalShowNew_num <= 3 || MahjongShowNew_num <= 3)
                    {
                        if (m_CreateRoomSource == 1 && NormalShowNew_num <= 3)//不在麻将馆内
                        {
                            NormalShowNew_num++;
                            if (m_showNew[0] != null) m_showNew[0].SetActive(false);
                            if (m_showNew[1] != null) m_showNew[1].SetActive(true);
                            if (m_showNew[2] != null) m_showNew[2].SetActive(false);
                        }

                        if (m_CreateRoomSource != 1 && MahjongShowNew_num <= 3)//在麻将馆内
                        {
                            MahjongShowNew_num++;
                            if (m_showNew[0] != null) m_showNew[0].SetActive(false);
                            if (m_showNew[1] != null) m_showNew[1].SetActive(true);
                            if (m_showNew[2] != null) m_showNew[2].SetActive(true);
                        }
                        PlayerPrefs.SetInt("NormalShowNew", NormalShowNew_num);
                        PlayerPrefs.SetInt("MahjongShowNew", MahjongShowNew_num);
                    }

                    if (NormalShowNew_num >= 4 && MahjongShowNew_num >= 4)
                    {
                        PlayerPrefs.DeleteKey("NormalShowNew");
                        PlayerPrefs.DeleteKey("MahjongShowNew");
                        for (int i = 0; i < m_showNew.Length; i++)
                        {
                            if (m_showNew[i] != null)
                                Destroy(m_showNew[i].gameObject);
                        }
                    }
                }

                SettingText.text = "收起";
                SettingImage.transform.localEulerAngles = new Vector3(0, 0, 180f);
                //如果玩家是普通开房
                if (m_CreateRoomSource == 1)
                {
                    AdvanceSetting[0].SetActive(true);
                    AdvanceSetting[1].SetActive(true);
                    AdvanceSetting[2].SetActive(true);
                    AdvanceSetting[3].SetActive(false);
                    SpecialSetting.minHeight = 275f;
                    SpecialSetting_.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 275f);
                }
                //玩家是代理代开房间
                else
                {
                    AdvanceSetting[0].SetActive(true);
                    AdvanceSetting[1].SetActive(true);
                    AdvanceSetting[2].SetActive(true);
                    AdvanceSetting[3].SetActive(true);
                    SpecialSetting.minHeight = 275f;
                    SpecialSetting_.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 275f);
                }


                crmpd.AnvanceSettingStatus = 2;
            }
            else
            {
                SettingText.text = "展开";
                SettingImage.transform.localEulerAngles = Vector3.zero;
                AdvanceSetting[0].SetActive(false);
                AdvanceSetting[1].SetActive(false);
                AdvanceSetting[2].SetActive(false);
                AdvanceSetting[3].SetActive(false);
                SpecialSetting.minHeight = 60f;
                SpecialSetting_.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60f);
                //SpecialSetting.transform.localPosition -= new Vector3(0, 90f, 0);
                crmpd.AnvanceSettingStatus = 1;

                if (m_showNew[0] != null) m_showNew[0].SetActive(false);
                if (m_showNew[1] != null) m_showNew[1].SetActive(false);
                if (m_showNew[2] != null) m_showNew[2].SetActive(false);
            }

            //float height = 0;
            //for (int i = 0; i < LayoutMent_All.Length; i++)
            //{
            //    height += LayoutMent_All[i].minHeight;
            //}

            //if (height > 375f || m_PlayModth.activeSelf)
            //{
            //    Rect.GetComponent<ScrollRect>().enabled = true;
            //}
            //else
            //{
            //    //恢复位置，之后关闭                
            //    Rect.transform.Find("Viewport/Content").transform.localPosition = ContentInitPos;
            //   // Rect.GetComponent<ScrollRect>().enabled = false;
            //}
        }


        /// <summary>
        /// 代理代开房间时，选择的颜色，按示意图默认排序从1开始
        /// </summary>
        /// <param name="index"></param>
        public void BtnChoiceClolor(int index)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            crmpd.iColorFlag = index;
            Debug.LogWarning("index:" + index);
        }

        /// <summary>
        /// 点击修改城市玩法
        /// </summary>
        public void BtnSwitchCity()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_SWITCHCITY);

            OnSetInitCreatRoomInfo();

            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            for (int i = 0; i < crmpd.iParamOpenRoomMessage.Length; i++)
            {
                // Debug.LogError("这里BtnSwitchCity");
                crmpd.iParamOpenRoomMessage[i] = 0;
            }
        }


        /// <summary>
        /// 点击玩法的具体规则
        /// </summary>
        public void BtnGameRule()
        {
            Messenger_anhui.Broadcast(MainViewLobbyPanel.MESSAGE_PALYINGMETHOD);

            OnSetInitCreatRoomInfo();
        }


        int DataHour = 0;
        int DataMin = 0;

        //选择时间中
        void ClockUseValue()
        {
            string hour = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("HH");//当前时
            DataHour = Convert.ToInt32(hour);
            CreatRoomHour.text = DataHour.ToString();

            string min = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("mm");//当前分
            DataMin = Convert.ToInt32(min) + 5;
            if (DataMin >= 60)
            {
                DataMin = DataMin - 60;
                DataHour += 1;
                CreatRoomHour.text = DataHour.ToString();
                CreatRoomMin.text = DataMin.ToString();
            }
            else
            {
                CreatRoomMin.text = DataMin.ToString(); ;
            }

            isCreateRoomClock = true;
        }

        //不选择时间
        void StartSetValue()
        {
            string hour = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("HH");//当前时
            DataHour = Convert.ToInt32(hour);
            CreatRoomHour.text = hour;

            string min = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("mm");//当前分
            DataMin = Convert.ToInt32(min);
            CreatRoomMin.text = min;

            isCreateRoomClock = false;
        }

        void OnGetInto()
        {
            string hour = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("HH");//当前时
            string min = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("mm");//当前分
            if (Convert.ToInt32(hour) > (DataHour))
            {
                CreatRoomHour.text = hour;
                CreatRoomMin.text = min;
            }
            if (Convert.ToInt32(hour) >= (DataHour) && Convert.ToInt32(min) > (DataMin))
            {
                CreatRoomMin.text = min;
            }
        }

        public void BtnCreateRoomUpHour()//小时的上一页
        {
            if (isCreateRoomClock == false) return;
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            string hour = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("HH");//当前时
            string min = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("mm");//当前分
            if (Convert.ToInt32(hour) < DataHour)
            {
                DataHour--;
                CreatRoomHour.text = DataHour.ToString();
                if (Convert.ToInt32(min) > DataMin)
                {
                    DataMin = Convert.ToInt32(min) + 5;
                    CreatRoomMin.text = DataMin.ToString();
                }
            }
        }
        public void BtnCreateRoomDownHour()//小时的下一页
        {
            if (isCreateRoomClock == false) return;

            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            string hour = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("HH");//当前时
            if (DataHour < (Convert.ToInt32(hour) + CanUserHous))
            {
                DataHour++;
                CreatRoomHour.text = DataHour.ToString();
            }
        }
        public void BtnCreateRoomUpMin()////分钟的上一页
        {
            if (isCreateRoomClock == false) return;

            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            string min = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("mm");//当前分
            string hour = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("HH");//当前时
            DataMin -= 5;
            if (DataMin < 0) DataMin = 55;
            if (DataHour == Convert.ToInt32(hour))
            {
                if (DataMin < (Convert.ToInt32(min) + 5))
                {
                    DataMin = (Convert.ToInt32(min) + 5);
                }
            }
            CreatRoomMin.text = DataMin.ToString();
        }
        public void BtnCreateRoomDownMin()//分钟的下一页
        {
            if (isCreateRoomClock == false) return;
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            string min = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("mm");//当前分
            string hour = (MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("HH");//当前时

            int buqi = DataMin % 5;
            if (buqi == 0) buqi = 5;
            else buqi = 5 - buqi;

            DataMin += buqi;
            if (DataMin >= 60)
            {
                DataMin = 0;
                if ((Convert.ToInt32(hour) + CanUserHous) > DataHour)
                {
                    DataHour++;
                    CreatRoomHour.text = DataHour.ToString();
                }
            }
            CreatRoomMin.text = DataMin.ToString();
        }


        private float timeCreateRoomStart = 0;
        private float timeCreateRoomOver = 1.0f;
        void UpdateTime()
        {
            while (timeCreateRoomStart >= timeCreateRoomOver)
            {
                OnGetInto();//试试刷星时间
                timeCreateRoomStart -= timeCreateRoomOver;
            }
            timeCreateRoomStart += Time.deltaTime;
        }

        /// <summary>
        /// 结算一下离开时游戏还有多长时间  返回值的参数详情 如果是0说明没有选择预约开放 如果为-1说明开放时间不满足条件
        /// </summary>
        /// <returns></returns>
        public int OnSubWaitForTimeStartGame()
        {
            if (isCreateRoomClock == false) return 0;
            int StartMin = Convert.ToInt32((MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("mm"));//当前分
            int StartHour = Convert.ToInt32((MahjongCommonMethod.Instance.UnixTimeStampToDateTime(MahjongCommonMethod.Instance.GetCreatetime(), 0)).ToString("HH"));//当前时
            int SubTime = (DataMin - StartMin) + ((DataHour - StartHour) * 60);
            if (SubTime < 5) return -1;
            return SubTime;
        }


        /// <summary>
        /// 重置开放参数
        /// </summary>
        void OnSetInitCreatRoomInfo()
        {
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            MahjongCommonMethod.Instance.WriteColumnValue(ref crmpd.roomMessage_, 39, 2, 2);
            crmpd.roomMessage_[4] = 0;
            crmpd.roomMessage_[5] = 0;
            AdvanceSetting[2].transform.GetChild(0).GetChild(0).GetComponent<Toggle>().isOn = true;
            AdvanceSetting[2].transform.GetChild(0).GetChild(1).GetComponent<Toggle>().isOn = false;
            AdvanceSetting[2].transform.GetChild(0).GetChild(2).GetComponent<Toggle>().isOn = false;
            AdvanceSetting[2].transform.GetChild(0).GetChild(3).GetComponent<Toggle>().isOn = false;
            AdvanceSetting[3].transform.GetChild(0).GetChild(0).GetComponent<Toggle>().isOn = true;
            AdvanceSetting[3].transform.GetChild(0).GetChild(1).GetComponent<Toggle>().isOn = false;
            RoomCost.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
            RoomCost.transform.GetChild(1).GetComponent<Toggle>().isOn = false;
            if (m_showNew[0] != null) m_showNew[0].SetActive(false);
            if (m_showNew[1] != null) m_showNew[1].SetActive(false);
            if (m_showNew[2] != null) m_showNew[2].SetActive(false);
            StartSetValue();
            StringBuilder saveS = new StringBuilder();
            if (crmpd.allRoomMethor.Count == 0)
            {
                crmpd.allRoomMethor.Add(new CreatRoomMessagePanelData.MethordRuleClass(crmpd.MethodId, crmpd.roomMessage_));
            }
            else
            {
                for (int i = 0; i < crmpd.allRoomMethor.Count; i++)
                {
                    if (crmpd.allRoomMethor[i].methord == crmpd.MethodId)
                    {
                        crmpd.allRoomMethor[i] = new CreatRoomMessagePanelData.MethordRuleClass(crmpd.MethodId, crmpd.roomMessage_);
                    }
                }
            }
            // 20001:00000000_00000000_00000000|20015:
            for (int i = 0; i < crmpd.allRoomMethor.Count; i++)
            {
                if (i != 0)
                {
                    saveS.AppendFormat("|");
                }

                saveS.Append(crmpd.allRoomMethor[i].methord + ":");
                for (int j = 0; j < crmpd.allRoomMethor[i].param.Length; j++)
                {
                    if (j != 0)
                    {
                        saveS.Append("_");
                    }
                    saveS.Append(crmpd.allRoomMethor[i].param[j].ToString("X8"));
                }

            }
            PlayerPrefs.SetString(CreatRoomMessagePanelData.SaveRuleParamName, saveS.ToString());
          //  Debug.LogError(saveS.ToString());
        }


        //void OnGUI()
        //{
        //    if (GUI.Button(new UnityEngine.Rect(10, 10, 100, 40), "creat"))
        //    {
        //        CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
        //        Debug.Log("出牌参数：" + crmpd.roomMessage_[0] + "," + crmpd.roomMessage_[1] + "," + crmpd.roomMessage_[2]
        //        + "," + crmpd.roomMessage_[3] + "," + crmpd.roomMessage_[4] + "," + crmpd.roomMessage_[5] + "," + crmpd.roomMessage_[6] + "," + crmpd.roomMessage_[7]
        //        + "," + crmpd.roomMessage_[8] + "," + crmpd.roomMessage_[9] + "," + crmpd.roomMessage_[10] + "," + crmpd.roomMessage_[11] + "玩法id：" + crmpd.MethodId);
        //    }
        //}


        /// <summary>
        /// 改变关联状态
        /// </summary>        
        /// <param name="status">0表示高级改变基础  1表示基础改变高级</param>
        public void ChangeReleaveStatus(int ReleaveId, int status)
        {
            //  Debug.LogError("改变关联状态");
            if (ReleaveId == 0)
            {
                return;
            }

            int cengId = 0;
            for (int i = 0; i < MahjongCommonMethod.Instance._methodToCardType.Count; i++)
            {
                if (ReleaveId == MahjongCommonMethod.Instance._methodToCardType[i].RuleId)
                {
                    cengId = MahjongCommonMethod.Instance._methodToCardType[i].Hierarchy;
                    break;
                }
            }

            CheckLongPressClick[] temp = RuleParent[cengId - 1].transform.
                GetComponentsInChildren<CheckLongPressClick>();

            Toggle toggle = null;

            for (int i = 0; i < temp.Length; i++)
            {
                if (ReleaveId == temp[i].RuleId)
                {
                    toggle = temp[i].GetComponent<Toggle>();
                    break;
                }
            }

            if (toggle == null)
            {
                return;
            }

            if (status == 0)
            {
                toggle.isOn = true;
            }
            else
            {
                toggle.isOn = false;
            }
        }
    }

}
