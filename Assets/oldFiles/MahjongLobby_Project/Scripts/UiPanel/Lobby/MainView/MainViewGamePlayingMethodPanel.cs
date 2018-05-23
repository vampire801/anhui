using UnityEngine;
using System.Collections.Generic;
using System;
using MahjongLobby_AH.Data;
using UnityEngine.UI;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewGamePlayingMethodPanel : MonoBehaviour
    {
        public GameObject SwitchLocation; //城市切换按钮
        public GameObject BtnParent;  //按钮的父物体
        public GameObject PmParent;  //具体玩法的父物体
        public ScrollRect rect;  //滑动条
        public Text content; //显示内容
        public Toggle[] toggle; //所有的toggle按钮
        public Text Area; //地区显示
        public GameObject[] IosHideBtn; //ios审核隐藏按钮  0表示切换地区按钮  1表示手机玩法按钮
        public Vector3 PmParentPos = Vector3.zero;
        public float height = 0;

        //是否在游戏内调用代码
        bool isGame = false;

        //区域选择
        public GameObject m_SelectRegion;

        //红包的按钮
        public GameObject m_RPbutton;

        #region 常量
        public const string MESSAGE_CLOSEBTN = "MainViewGamePlayingMethodPanel.MSEEAGE_CLOSEBTN";  //关闭按钮
        public const string MESSAGE_MAHJONGMETHOD = "MainViewGamePlayingMethodPanel.MESSAGE_MAHJONGMETHOD";  //选择不同种类按钮
        #endregion 常量    

        void Awake()
        {
            //DontDestroyOnLoad(gameObject);
            PmParentPos = PmParent.transform.localPosition;
            height = PmParent.GetComponent<RectTransform>().rect.height;
            PointIndexBtn(false, 0, null, true);
        }

        /// <summary>
        /// 指定点击哪个按钮
        /// </summary>
        /// <param name="index"></param>
        /// <param name="isAddMethod"></param>
        public void PointIndexBtn(bool GameIn, int index, string name = null, bool isAddMethod = false)
        {
            isGame = GameIn;
            OnGame(isGame, name);
            for (int i = 0; i < toggle.Length; i++)
            {
                //添加方法
                if (isAddMethod)
                {
                    BtnPointGameShow(toggle[i], i + 1);
                }
                if (i == 0)
                {
                    toggle[i].isOn = true;
                }
                else
                {
                    toggle[i].isOn = false;
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 面板的更新显示
        /// </summary>
        public void UpdateShow(int status)
        {
            GameData gd = GameData.Instance;
            GamePlayingMethodPanelData gpmpd = gd.GamePlayingMethodPanelData;
            CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
            if (gpmpd.PanelShow)
            {                
                if (MahjongLobby_AH.GameData.Instance.CreatRoomMessagePanelData != null)
                {
                    //if (crmpd.CreatRoomType == 1 && SDKManager.Instance.IOSCheckStaus == 0 && SDKManager.Instance.CheckStatus == 0)
                    if (UIMainView.Instance.ParlorShowPanel.ParlorMessagePanel.gameObject.activeSelf == false && SDKManager.Instance.IOSCheckStaus == 0 && SDKManager.Instance.CheckStatus == 0)
                    {
                        SwitchLocation.SetActive(true);
                        BtnParent.GetComponent<GridLayoutGroup>().padding.top = 5;
                    }
                    else
                    {
                        SwitchLocation.SetActive(false);
                        BtnParent.GetComponent<GridLayoutGroup>().padding.top = -50;
                    }
                }

                GameData.Instance.isShowQuitPanel = false;
                gameObject.SetActive(true);
                SpwanBtn(status);


                if (SDKManager.Instance.IOSCheckStaus == 1 || SDKManager.Instance.CheckStatus == 1)
                {
                    for (int i = 0; i < IosHideBtn.Length; i++)
                    {
                        IosHideBtn[i].SetActive(false);
                    }
                    //BtnParent.GetComponent<GridLayoutGroup>().padding.top = -50;
                }
                else
                {
                    for (int i = 0; i < IosHideBtn.Length; i++)
                    {
                        IosHideBtn[i].SetActive(true);
                    }                    
                    //BtnParent.GetComponent<GridLayoutGroup>().padding.top = 5;
                }
            }
            else
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        public void BtnClose()
        {
            if (isGame)
            {
                gameObject.SetActive(false);
            }
            else
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Messenger_anhui.Broadcast(MESSAGE_CLOSEBTN);
            }
        }

        /// <summary>
        /// 点击查看方法按钮
        /// </summary>
        /// <param name="go"></param>
        public void BtnMahjong(GameObject go)
        {
            if (go == null)
            {
                return;
            }
            //显示选中按钮状态
            Button[] button = BtnParent.GetComponentsInChildren<Button>();
            for (int i = 0; i < button.Length; i++)
            {
                if (string.Equals(button[i].name, go.name))
                {
                    button[i].transform.GetComponent<CityConnectCounty>().status[0].SetActive(true);
                    button[i].transform.GetComponent<CityConnectCounty>().status[1].SetActive(false);
                }
                else
                {
                    button[i].transform.GetComponent<CityConnectCounty>().status[0].SetActive(false);
                    button[i].transform.GetComponent<CityConnectCounty>().status[1].SetActive(true);
                }
            }

            int index = Convert.ToInt32(go.name.Split('_')[1]);
            if (index <= 0)
            {
                return;
            }
            GameData.Instance.GamePlayingMethodPanelData.GameIndex = index;
            AddMethod(1);
            Messenger_anhui<int>.Broadcast(MESSAGE_MAHJONGMETHOD, index);
        }

        /// <summary>
        /// 指定显示玩法信息
        /// </summary>
        /// <param name="index">1表示基本规则 2基本牌型 3特殊规则 4游戏结算</param>
        public void BtnPointGameShow(Toggle toggle, int index)
        {
            toggle.onValueChanged.AddListener(delegate (bool isOn) { AddMethod_Tog(isOn, index); });
        }

        void AddMethod_Tog(bool isOn, int index)
        {
            GameData gd = GameData.Instance;
            GamePlayingMethodPanelData gpmpd = gd.GamePlayingMethodPanelData;
            string name = gpmpd.GameIndex + "_" + index;
            StartCoroutine(gpmpd.ReadPlayingMethond_(name, isGame));
        }

        void AddMethod(int index)
        {
            GameData gd = GameData.Instance;
            GamePlayingMethodPanelData gpmpd = gd.GamePlayingMethodPanelData;
            string name = gpmpd.GameIndex + "_" + index;
            StartCoroutine(gpmpd.ReadPlayingMethond_(name, isGame));
        }

        /// <summary>
        /// 产生对应玩法的按钮
        /// </summary>
        public void SpwanBtn(int status)
        {
            anhui.MahjongCommonMethod mcm = anhui.MahjongCommonMethod.Instance;
            GameData gd = GameData.Instance;
            GamePlayingMethodPanelData gpmpd = gd.GamePlayingMethodPanelData;
            //SelectAreaPanelData sapd = gd.SelectAreaPanelData;
            //删除之前的按钮
            Button[] btn = BtnParent.transform.GetComponentsInChildren<Button>();

            for (int i = 0; i < btn.Length; i++)
            {
                Destroy(btn[i].gameObject);
            }

            int index_ = 0;
            List<int> temp = new List<int>();

            //Debug.LogError("status:" + status + ",:" + mcm.lsMethodId.Count + "," + GameData.Instance.CreatRoomMessagePanelData.MethodId);

            if (status == 0)
            {
                temp = mcm.lsMethodId;
                if (GameData.Instance.CreatRoomMessagePanelData.MethodId > 0)
                {
                    index_ = GameData.Instance.CreatRoomMessagePanelData.MethodId;
                    PointIndexBtn(false, 0);
                    Area.text = anhui.MahjongCommonMethod.Instance._dicDisConfig[gpmpd.CountyId].COUNTY_NAME;
                }
                else
                {
                    temp = new List<int>();
                    if (SDKManager.Instance.IOSCheckStaus == 1 || SDKManager.Instance.CheckStatus == 1)
                    {
                        temp.Add(2);
                    }
                    else
                    {
                        for (int i = 0; i < mcm._dicPlayNameConfig[gpmpd.CountyId].Count; i++)
                        {
                            temp.Add(mcm._dicPlayNameConfig[gpmpd.CountyId][i].METHOD);
                        }
                    }
                    index_ = mcm._dicPlayNameConfig[gpmpd.CountyId][0].METHOD;
                    Area.text = anhui.MahjongCommonMethod.Instance._dicDisConfig[gpmpd.CountyId].COUNTY_NAME;
                }
                //Area.text = MahjongCommonMethod.Instance._dicDisConfig[GameData.Instance.SelectAreaPanelData.iCountyId].COUNTY_NAME;
            }
            else
            {
                Area.text = anhui.MahjongCommonMethod.Instance._dicDisConfig[gpmpd.CountyId].COUNTY_NAME;
                string[] id = anhui.MahjongCommonMethod.Instance._dicDisConfig[gpmpd.CountyId].METHOD.Split('_');
                for (int k = 0; k < id.Length; k++)
                {
                    int ID = Convert.ToInt16(id[k]);
                    if (ID != 0)
                    {
                        temp.Add(ID);
                    }
                }
                PointIndexBtn(false, 0);
            }
            gpmpd.GameIndex = temp[0];
            AddMethod(1);
            for (int i = 0; i < temp.Count; i++)
            {
                if (SDKManager.Instance.CheckStatus == 1)
                {
                    if (i > 1)
                    {
                        break;
                    }
                }

                GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/PlayerMethodPanel/Method"));

                go.name = "Method_" + temp[i];
                go.transform.SetParent(BtnParent.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);


                if (index_ > 0)
                {
                    if (temp[i] == index_)
                    {
                        go.transform.GetComponent<CityConnectCounty>().status[0].SetActive(true);
                        go.transform.GetComponent<CityConnectCounty>().status[1].SetActive(false);
                    }
                    else
                    {
                        go.transform.GetComponent<CityConnectCounty>().status[1].SetActive(true);
                        go.transform.GetComponent<CityConnectCounty>().status[0].SetActive(false);
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        go.transform.GetComponent<CityConnectCounty>().status[0].SetActive(true);
                        go.transform.GetComponent<CityConnectCounty>().status[1].SetActive(false);
                    }
                    else
                    {
                        go.transform.GetComponent<CityConnectCounty>().status[1].SetActive(true);
                        go.transform.GetComponent<CityConnectCounty>().status[0].SetActive(false);
                    }
                }

                if (SDKManager.Instance.CheckStatus == 0)
                {
                    //Debug.LogError("gpmpd.CountyId：" + gpmpd.CountyId + ".count：" + mcm._dicPlayNameConfig[gpmpd.CountyId].Count);
                    for (int k = 0; k < mcm._dicPlayNameConfig[gpmpd.CountyId].Count; k++)
                    {
                        if (temp[i] == mcm._dicPlayNameConfig[gpmpd.CountyId][k].METHOD)
                        {
                            go.transform.GetComponent<CityConnectCounty>().status[2].GetComponent<Text>().text = mcm._dicPlayNameConfig[gpmpd.CountyId][k].METHOD_NAME;
                            go.transform.GetComponent<CityConnectCounty>().status[3].GetComponent<Text>().text = mcm._dicPlayNameConfig[gpmpd.CountyId][k].METHOD_NAME;
                        }
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        go.transform.GetComponent<CityConnectCounty>().status[2].GetComponent<Text>().text = "推倒胡";
                        go.transform.GetComponent<CityConnectCounty>().status[3].GetComponent<Text>().text = "推倒胡";
                    }
                    else
                    {
                        go.transform.GetComponent<CityConnectCounty>().status[2].GetComponent<Text>().text = "推倒胡打锅";
                        go.transform.GetComponent<CityConnectCounty>().status[3].GetComponent<Text>().text = "推倒胡打锅";
                    }

                }

                AddClickDelegate(go.GetComponent<Button>());
            }
        }

        /// <summary>
        /// 动态为按钮添加方法
        /// </summary>
        /// <param name="method"></param>
        public void AddClickDelegate(Button button)
        {
            button.onClick.AddListener(
                delegate ()
                {
                    BtnMahjong(button.gameObject);
                });
        }

        /// <summary>
        /// 跳转客服的玩法收集界面
        /// </summary>
        public void SkipCustomPanel()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MainViewLobbyPanel.MESSAGE_CUSTOMSEVERBTN);
            UIMainView.Instance.CustomSeverPanel.PointIndex = 2;
        }

        /// <summary>
        /// 点击修改城市玩法
        /// </summary>
        public void BtnSwitchCity()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            GameData.Instance.SelectAreaPanelData.iOpenStatus = 6;
            GameData.Instance.SelectAreaPanelData.pos_index = 1;
            GameData.Instance.SelectAreaPanelData.isPanelShow = true;
            SystemMgr.Instance.SelectAreaSystem.UpdateShow();
        }

        /// <summary>
        /// 是否在游戏内
        /// </summary>
        /// <param name="GameIn"> 游戏内为true </param>
        void OnGame(bool GameIn, string name)
        {
            if (GameIn)
            {
                gameObject.SetActive(true);
                m_SelectRegion.GetComponent<Button>().enabled = false;
                m_SelectRegion.transform.GetChild(1).gameObject.SetActive(false);
                m_SelectRegion.transform.GetChild(2).gameObject.SetActive(true);
                m_SelectRegion.transform.GetChild(2).GetComponent<Text>().text = name;
                m_RPbutton.SetActive(false);
                isGame = true;
                AddMethod(1);
            }
            else
            {
                m_SelectRegion.GetComponent<Button>().enabled = true;
                m_SelectRegion.transform.GetChild(1).gameObject.SetActive(true);
                //m_SelectRegion.transform.GetChild(2).gameObject.SetActive(false);
                m_RPbutton.SetActive(true);
                isGame = false;
            }
        }
    }

}