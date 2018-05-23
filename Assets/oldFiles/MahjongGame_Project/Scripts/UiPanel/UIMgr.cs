using UnityEngine;
using System.Collections;
using MahjongGame_AH.UISystem;
using Common;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class UIMgr : MonoBehaviour
    {
        public float DesignWidth = 1280;
        public float DesignHeight = 720;
        


        #region 实例

        public const string MESSAGE_OPEN_PANEL = "UIMgr.MESSAGE_OPEN_PANEL";
        public const string MESSAGE_CLOSE_PANEL = "UIMgr.MESSAGE_CLOSE_PANEL";

        static UIMgr instance;
        public static UIMgr GetInstance()
        {
            return instance;
        }

        #endregion 实例

        #region public interface

        public GameObject RootAnchor;
        public GameObject[] UIPanelPrefabs;

        RedPageShowPanel showPanel;
        public RedPageShowPanel ShowRedPagePanel
        {
            get
            {
                if (showPanel == null)
                {
                    GameObject panel = Instantiate(UIPanelPrefabs[2]) as GameObject;
                    //Debug.LogError("panel" + panel.name);
                    //Debug.LogError("camera" + transform.GetChild(0).gameObject.name);
                    //Debug.LogError("RootAnchor" + RootAnchor.name);
                    //panel.transform.SetParent(RootAnchor.transform);
                    panel.transform.localPosition = Vector3.zero;
                    panel.transform.localRotation = Quaternion.identity;//RootAnchor.transform.localRotation;
                    panel.name = UIPanelPrefabs[2].name;
                    panel.SetActive(false);
                    showPanel = panel.GetComponent<RedPageShowPanel>();
                }
                return showPanel;
            }
        }

        /// <summary>
        /// UI面板的哈希表，键是UIID, 值是视图的处理类。
        /// </summary>
        Hashtable UIPanels = new Hashtable();

        /// <summary>
        /// 将要被删除的面板
        /// </summary>
        Hashtable delPanels = new Hashtable();
        int delDelayFrame;
        const int DEL_PANEL_FRAME = 5;

        ushort panelIDToOpen = UIConstant.UIID_WRONG_ID;
        ushort panelIDToClose = UIConstant.UIID_WRONG_ID;

        public delegate void LoadingTimeOutEventHandler();
        public event LoadingTimeOutEventHandler OnLoadingTimeOut;

        bool loading;
        float loadingTimeOutTimer;
        float timeOutTime;

        void Awake()
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
            UIPanels.Clear();
        }

        void Start()
        {          
            SceneManager_anhui.Instance.OnEnterScene += HandleOnEnterScene;
            SceneManager_anhui.Instance.OnLeaveScene += HandleOnLeaveScene;

            Load_GeneralPanels();
        }

        void LateUpdate()
        {
            --delDelayFrame;
            if (delDelayFrame < 0)
            {
                foreach (DictionaryEntry panel in delPanels)
                {
                    if ((GameObject)panel.Value != null)
                    {
                        GameObject.DestroyImmediate(panel.Value as GameObject);
                    }
                }

                delPanels.Clear();
            }

            if (panelIDToOpen != UIConstant.UIID_WRONG_ID)
            {
                DoOpenPanel(panelIDToOpen);
                panelIDToOpen = UIConstant.UIID_WRONG_ID;
            }
            else if (panelIDToClose != UIConstant.UIID_WRONG_ID)
            {
                DoClosePanel(panelIDToClose);
                panelIDToClose = UIConstant.UIID_WRONG_ID;
            }

            if (loading)
            {
                if (timeOutTime > 0.1f)
                {
                    if (Time.realtimeSinceStartup - loadingTimeOutTimer > timeOutTime)
                    {
                        if (OnLoadingTimeOut != null)
                        {
                            OnLoadingTimeOut();
                        }

                        UIMgr.HideLoading();
                    }
                }
            }
        }

        void OnEnable()
        {
            Messenger_anhui<ushort>.AddListener(MESSAGE_OPEN_PANEL, OpenPanel);
            Messenger_anhui<ushort>.AddListener(MESSAGE_CLOSE_PANEL, ClosePanel);
        }

        void OnDisable()
        {            
            Messenger_anhui<ushort>.RemoveListener(MESSAGE_OPEN_PANEL, OpenPanel);
            Messenger_anhui<ushort>.RemoveListener(MESSAGE_CLOSE_PANEL, ClosePanel);
            SceneManager_anhui.Instance.OnEnterScene -= HandleOnEnterScene;
            SceneManager_anhui.Instance.OnLeaveScene -= HandleOnLeaveScene;
        }


        /// <summary>
        /// 打开UI面板
        /// </summary>
        /// <param name='uiid'>
        /// UI面板的ID
        /// </param>
        public static void OpenUIPanel(ushort uiid)
        {
            Messenger_anhui<ushort>.Broadcast(UIMgr.MESSAGE_OPEN_PANEL, uiid);
        }

        /// <summary>
        /// 关闭UI面板
        /// </summary>
        /// <param name='uiid'>
        /// UI面板的ID
        /// </param>
        public static void CloseUIPanel(ushort uiid)
        {
            Messenger_anhui<ushort>.Broadcast(UIMgr.MESSAGE_CLOSE_PANEL, uiid);
        }

        /// <summary>
        /// 显示加载进度
        /// </summary>
        public static void ShowLoading()
        {
            ShowLoading(0.0f);
        }

        /// <summary>
        /// 显示加载进度
        /// </summary>
        /// <param name='timeoutSec'>
        /// 超时的秒数
        /// </param>
        public static void ShowLoading(float timeoutSec)
        {
            GetInstance().loading = true;
            GetInstance().timeOutTime = timeoutSec;
            GetInstance().loadingTimeOutTimer = Time.realtimeSinceStartup;

            GetInstance().OpenPanel(UIConstant.UIID_GENERAL_LOADING);
        }

        /// <summary>
        /// 隐藏加载中
        /// </summary>
        public static void HideLoading()
        {
            UIMgr.GetInstance().loading = false;
            UIMgr.GetInstance().timeOutTime = 0.0f;
            UIMgr.GetInstance().loadingTimeOutTimer = 0.0f;

            UIMgr.GetInstance().ClosePanel(UIConstant.UIID_GENERAL_LOADING);
        }

        //显示等待窗口
        public void ShowWaiting()
        {
            OpenPanel(UIConstant.UIID_GENERAL_WAITING);
        }

        //隐藏等待窗口
        public void HideWaiting()
        {
            ClosePanel(UIConstant.UIID_GENERAL_WAITING);
        }

        /// <summary>
        /// 获取消息框
        /// </summary>
        /// <returns>
        /// 消息框
        /// </returns>
        public UIMessageView GetUIMessageView()
        {
            return (UIMessageView)GetUIPanel(UIConstant.UIID_MAHJONG_GAME_GENERAL_MESSAGE);
        }

        /// <summary>
        /// 加载通用面板
        /// </summary>
        void Load_GeneralPanels()
        {            
            RegistUIPanel(UIConstant.UIID_GENERAL_LOADING);
            ClosePanel(UIConstant.UIID_GENERAL_LOADING);

            RegistUIPanel(UIConstant.UIID_MAHJONG_GAME_GENERAL_MESSAGE);
            ClosePanel(UIConstant.UIID_MAHJONG_GAME_GENERAL_MESSAGE);

            RegistUIPanel(UIConstant.UIID_GENERAL_WAITING);
            ClosePanel(UIConstant.UIID_GENERAL_WAITING);
        
        }

        /// <summary>
        /// 卸载通用面板
        /// 这是公用的一些面板，不要卸载
        /// </summary>
        void Unload_GeneralPanels()
        {
            // TODO
            RemoveUIPanel(UIConstant.UIID_MAHJONG_GAME_GENERAL_MESSAGE);
        }

        /// <summary>
        /// 加载游戏场景的面板
        /// </summary>
        void Load_LobbyPanels()
        {
            RegistUIPanel(UIConstant.UIID_MAHJONG_GAME_MAIN_PANEL);
            ////		RegistUIPanel(UIConstant.UIID_LOBBY_SOCIAL_PANEL);

            ////		CloseUIPanel(UIConstant.UIID_LOBBY_SOCIAL_PANEL);
        }

        /// <summary>
        /// 卸载游戏场景的面板
        /// </summary>
        void Unload_LobbyPanels()
        {
            RemoveUIPanel(UIConstant.UIID_MAHJONG_GAME_MAIN_PANEL);

            ////		RemoveUIPanel(UIConstant.UIID_LOBBY_SOCIAL_PANEL);
        }

        /// <summary>
        /// 当离开场景的处理器
        /// </summary>
        /// <param name='sender'>
        /// 场景管理器
        /// </param>
        void HandleOnLeaveScene(SceneManager_anhui sender)
        {            
            ShowLoading();

            switch (sender.LeavingScene)
            {
                //				case ESCENE.LOBBY_SCENE:
                case ESCENE.MAHJONG_GAME_MAIN_SCENE:

                    Unload_LobbyPanels();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 当进入场景的处理器
        /// </summary>
        /// <param name='sender'>
        /// 场景管理器
        /// </param>
        void HandleOnEnterScene(SceneManager_anhui sender)
        {            
            UIMgr.HideLoading();
            switch (sender.EnteringScene)
            {
                case ESCENE.MAHJONG_GAME_MAIN_SCENE:                    
                    Load_LobbyPanels();
                    //ScaleAnchor();
                    OpenUIPanel(UIConstant.UIID_MAHJONG_GAME_MAIN_PANEL);
                    break;

                default:
                    break;
            }
        }


        #region implement

        /// <summary>
        /// Regists the user interface panel.
        /// </summary>
        /// <returns>
        /// success or not
        /// </returns>
        /// <param name='uiid'>
        /// Panel's ID
        /// </param>
        public bool RegistUIPanel(ushort uiid)
        {            
            GameObject panel;
            for (int i = 0; i < UIPanelPrefabs.Length; ++i)
            {
                if (UIPanelPrefabs[i] == null)
                {
                    DEBUG.Gui(UIConstant.UIIdToString(uiid) + " is NULL", LogType.Error);
                    continue;
                }

                BaseUI prefabUI = UIPanelPrefabs[i].GetComponent<BaseUI>();
                if (prefabUI == null)
                {
                    DEBUG.Gui(UIConstant.UIIdToString(uiid) + " does NOT have BaseUI component", LogType.Error);
                    continue;
                }

                if (prefabUI.GetID() == uiid)
                {
                    if (RootAnchor == null)
                    {
                        continue;
                    }

                    panel = (GameObject)Instantiate(UIPanelPrefabs[i]);
                    panel.transform.SetParent(RootAnchor.transform);
                    panel.transform.localPosition = Vector3.zero;
                    panel.transform.localRotation = RootAnchor.transform.localRotation;
                    panel.transform.localScale = Vector3.one;
                    panel.name = UIPanelPrefabs[i].name;

                    BaseUI ui = panel.GetComponent<BaseUI>();
                    if (ui != null)
                    {
                        if (!UIPanels.ContainsKey(ui.GetID()))
                        {
                            UIPanels.Add(ui.GetID(), panel);

                            return true;
                        }
                        DEBUG.Gui(UIConstant.UIIdToString(ui.GetID()) + " is already exist!", LogType.Error);
                        return false;
                    }
                    DEBUG.Gui("There's no BaseUI module in " + UIPanelPrefabs[i].name, LogType.Error);
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Remove the user interface panel. 
        /// it will be remove from the m_UIPanels, and add to m_DelPanels. 
        /// it will delete server frames later.
        /// </summary>
        /// <returns>
        /// success or not
        /// </returns>
        /// <param name='uiid'>
        /// Panel's ID
        /// </param>
        public bool RemoveUIPanel(ushort uiid)
        {
            DEBUG.Gui("Remove UI Panel: " + UIConstant.UIIdToString(uiid));

            //Debug.LogError("移除面板:" + uiid);

            if (UIPanels.ContainsKey(uiid))
            {
                if (IsActive(uiid))
                {
                    if ((GameObject)UIPanels[uiid] != null)
                    {
                        DoClosePanel(uiid);
                    }                        
                }

                if((GameObject)UIPanels[uiid]!=null)
                {
                    ((GameObject)UIPanels[uiid]).SetActive(false);
                    delPanels.Add(uiid, (UIPanels[uiid] as GameObject));
                }                                               
                UIPanels.Remove(uiid);
                delDelayFrame = DEL_PANEL_FRAME;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the UI panel.
        /// </summary>
        /// <returns>
        /// The UI panel.
        /// </returns>
        /// <param name='uiid'>
        /// uiid.
        /// </param>
        public BaseUI GetUIPanel(ushort uiid)
        {
            if (UIPanels.ContainsKey(uiid))
            {
                BaseUI ui = ((GameObject)UIPanels[uiid]).GetComponent<BaseUI>();
                return ui;
            }

            return null;
        }

        public bool IsActive(ushort uiid)
        {
            if (UIPanels.ContainsKey(uiid))
            {
                if((GameObject)UIPanels[uiid]==null)
                {
                    return false;
                }
                return ((GameObject)UIPanels[uiid]).activeInHierarchy;
            }

            return false;
        }


        #endregion

        #endregion

        void OpenPanel(ushort uiid)
        {
            DEBUG.Gui("Open UI Panel: " + UIConstant.UIIdToString(uiid));

            panelIDToOpen = uiid;
        }

        void ClosePanel(ushort uiid)
        {
            DEBUG.Gui("Close UI Panel: " + UIConstant.UIIdToString(uiid));

            panelIDToClose = uiid;
        }

        void DoOpenPanel(ushort uiid)
        {
            if (UIPanels.ContainsKey(uiid))
            {
                ((GameObject)UIPanels[uiid]).SetActive(true);

                if (((GameObject)UIPanels[uiid]).GetComponent<BaseUI>() != null)
                {
                    ((GameObject)UIPanels[uiid]).GetComponent<BaseUI>().FireActiveEvent();
                }
            }
            else
            {
                DEBUG.Gui(UIConstant.UIIdToString(uiid) + " is not exist!", LogType.Error);
            }
        }

        void DoClosePanel(ushort uiid)
        {
            if (UIPanels.ContainsKey(uiid))
            {
                if (((GameObject)UIPanels[uiid]).GetComponent<BaseUI>() != null)
                {
                    ((GameObject)UIPanels[uiid]).GetComponent<BaseUI>().FireDeactiveEvent();
                }

                ((GameObject)UIPanels[uiid]).SetActive(false);
            }
            else
            {
                DEBUG.Gui(UIConstant.UIIdToString(uiid) + " is not exist!", LogType.Error);
            }
        }      
    }

}


