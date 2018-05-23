using UnityEngine;
using System.Collections;
using MahjongLobby_AH.Data;
using UnityEngine.UI;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewInsteadOpenRoomPanel : MonoBehaviour
    {
        public GameObject InsteadRulePanel;  //代开规则的面板
        public GameObject CurrentRoomStatus;  //没有开始游戏的开放信息父物体
        public GameObject RacingRoomStatus;  //已经开始的房间信息父物体
        public GameObject InstaedOPenRoomStatus;  //房间状态的显示
        public GameObject InsteadOpenRecord;   //代开房间的历史消息
        public GameObject NoRecord; //无代开记录的物体
        public GameObject Content;   //滑动条的content
        public float ContentSize = 0;  //content的初始长度
        
        public GameObject[] OpenRoomStatuBtn;  //点击状态按钮显示的图片
        public GameObject[] InsteadRecordBtn; //点击代开按钮的显示的图片

        public const string MESSAGE_INSTEADOPEN = "MainViewInsteadOpenRoomPanel.MESSAGE_INSTEADOPEN";  //代开房间按钮
        public const string MESSAGE_INSTAEDOPENRECORD = "MainViewInsteadOpenRoomPanel.MESSAGE_INSTAEDOPENRECORD";  //代开记录按钮
        public const string MESSAGE_INSTEADRULE = "MainViewInsteadOpenRoomPanel.MESSAGE_INSTEADRULE";  //代开规则按钮
        public const string MESSAGE_OPENCREATROOM = "MainViewInsteadOpenRoomPanel.MESSAGE_OPENCREATROOM";  //创建房间按钮
        public const string MESSAGE_BTNCLOSE = "MainViewInsteadOpenRoomPanel.MESSAGE_BTNCLOSE";  //代开房间的关闭按钮
        public const string MESSAGE_INSTEADRULECLOSE = "MainViewInsteadOpenRoomPanel.MESSAGE_INSTEADRULECLOSE";  //代开规则的关闭按钮

        InfinityGridLayoutGroup infinityGridLayoutGroup;



        void Start()
        {
            ContentSize = Content.GetComponent<RectTransform>().rect.size.y;
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
        /// 面板更新
        /// </summary>
        public void UpdateShow()
        {
            InsteadOpenRoomPanelData iorpd = GameData.Instance.InsteadOpenRoomPanelData;
            if (iorpd.PanelShow)
            {
                gameObject.SetActive(true);
                GameData.Instance.isShowQuitPanel = false;
                if (iorpd.iBtnStatus == 1)
                {
                    OpenRoomStatuBtn[0].SetActive(true);
                    OpenRoomStatuBtn[1].SetActive(true);
                    InsteadRecordBtn[0].SetActive(false);
                    InsteadRecordBtn[1].SetActive(false);
                }
                else
                {
                    OpenRoomStatuBtn[0].SetActive(false);
                    OpenRoomStatuBtn[1].SetActive(false);
                    InsteadRecordBtn[0].SetActive(true);
                    InsteadRecordBtn[1].SetActive(true);
                }
                if (iorpd.InsteadRulePanelShow)
                {
                    InsteadRulePanel.SetActive(true);
                }
                else
                {
                    InsteadRulePanel.SetActive(false);
                }
            }
            else
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);                
            }
        }

      

        /// <summary>
        /// 点击查看代开房间状态
        /// </summary>
        public void BtnInsteadOPen()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_INSTEADOPEN);
        }

        /// <summary>
        /// 点击查看代开记录按钮
        /// </summary>
        public void BtnInsteadOpenRecord()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_INSTAEDOPENRECORD);
        }

        /// <summary>
        /// 点击代开规则按钮
        /// </summary>
        public void BtnInsteadRule()
        {
            //SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_INSTEADRULE);
        }

        /// <summary>
        /// 点击创建房间按钮
        /// </summary>
        public void BtnOpenRoom()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_OPENCREATROOM);
        }

        /// <summary>
        /// 代开房间的关闭按钮
        /// </summary>
        public void BtnClose()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_BTNCLOSE);
        }

        /// <summary>
        /// 代开规则面板的规则按钮
        /// </summary>
        public void BtnInsteadRuleClose()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_INSTEADRULECLOSE);
        }


        /// <summary>
        /// 产生玩家所有的代开房间，如果
        /// </summary>
        /// <param name="roomInfo">该房间的信息</param>
        public void SpwanAllRoomStatus(InsteadOpenRoomPanelData.RoomInfo roomInfo)
        {
            int status = -2;    //-1表示没开，0没使用，1已经预订，2等待开始游戏，3已开始游戏
            if (roomInfo == null)
            {
                status = -1;
            }
            else
            {
                status = roomInfo.cOpenRoomStatus;
            }

            //产生的预置体
            GameObject go = null;

            switch (status)
            {
                case -1:
                    go = Instantiate(Resources.Load<GameObject>("Lobby/InsteadCreatRoomPanel/UnOpenRoom"));
                    break;
                case 0:
                case 1:
                case 2:
                    go = Instantiate(Resources.Load<GameObject>("Lobby/InsteadCreatRoomPanel/RoomStatus"));
                    break;
                case 3:
                    go = Instantiate(Resources.Load<GameObject>("Lobby/InsteadCreatRoomPanel/RacingRoom"));
                    break;
                default:
                    break;
            }

            //设定父物体
            if (status < 3)
            {
                go.transform.SetParent(CurrentRoomStatus.transform);
            }
            else
            {
                go.transform.SetParent(RacingRoomStatus.transform);
                //设定顺序
                if (status == -1)
                {
                    go.transform.SetSiblingIndex(CurrentRoomStatus.transform.childCount - 1);
                }
            }
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
            go.transform.localScale = Vector3.one;

            InsteadOpenMessage roomMessage = go.GetComponent<InsteadOpenMessage>();
            if (roomMessage == null)
            {
                return;
            }

            //更新房间状态的界面
            roomMessage.roomInfo = roomInfo;
        }


        /// <summary>
        /// 初始化代开房间的预置体
        /// </summary>       
        public void Deleteprefab()
        {            
            InsteadOpenRoomPanelData iorpd = GameData.Instance.InsteadOpenRoomPanelData;            
            //删除上面的五个预置体
            InsteadOpenMessage[] message_0 = CurrentRoomStatus.GetComponentsInChildren<InsteadOpenMessage>();
            for (int i = 0; i < message_0.Length; i++)
            {                
                Destroy(message_0[i].gameObject);
            }                   

            //更新玩家已经开始游戏的预置体
            InsteadOpenMessage[] message_1 = RacingRoomStatus.GetComponentsInChildren<InsteadOpenMessage>();
            //先检测玩家的预置体和数据的数量
            if (message_1.Length != iorpd.OpenRoomInfo_Started.Count)
            {
                //如果数量大于之前的，添加
                if (message_1.Length < iorpd.OpenRoomInfo_Started.Count)
                {
                    //更新已有的界面
                    for (int i = 0; i < message_1.Length; i++)
                    {
                        message_1[i].roomInfo = iorpd.OpenRoomInfo_Started[i];
                    }

                    //添加新的界面
                    for (int i = message_1.Length; i < iorpd.OpenRoomInfo_Started.Count; i++)
                    {
                        SpwanAllRoomStatus(iorpd.OpenRoomInfo_Started[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < iorpd.OpenRoomInfo_Started.Count; i++)
                    {
                        message_1[i].roomInfo = iorpd.OpenRoomInfo_Started[i];                   
                    }

                    for (int i = iorpd.OpenRoomInfo_Started.Count; i < message_1.Length; i++)
                    {
                        Destroy(message_1[i].gameObject);
                    }
                }
            }
        }


        public void Delay()
        {
            Invoke("SpwanInsteadOpenRecord", 0.15f);
        }

        /// <summary>
        /// 产生代开房间的预置体，如果玩家关闭之后
        /// </summary>
        void SpwanInsteadOpenRecord()
        {            
            InsteadOpenRoomPanelData iorpd = GameData.Instance.InsteadOpenRoomPanelData;
            if(iorpd.isNoRecord)
            {
                NoRecord.gameObject.SetActive(true);
            }
            else
            {
                NoRecord.gameObject.SetActive(false);
            }
            if(!iorpd.isFirstSpwanInsteadRecord)
            {
                return;
            }
            iorpd.isFirstSpwanInsteadRecord = false;
            //删除玩家的之前的面板
            InsteadOpenRecordMessage[] rooms = InsteadOpenRecord.transform.Find("Content").GetComponentsInChildren<InsteadOpenRecordMessage>();

            if (rooms.Length > 0)
            {
                for (int i = 0; i < rooms.Length; i++)
                {
                    Destroy(rooms[i].gameObject);
                }

  
            }
            InsteadOpenRecord.transform.Find("Content").GetComponent<GridLayoutGroup>().enabled = true;
            InsteadOpenRecord.transform.Find("Content").GetComponent<ContentSizeFitter>().enabled = true;
            
            int count = iorpd.RoomMessage.Count;
            Debug.LogError("count:" + count);

            if (count <= 0)
            {
                return;
            }
            
            if (count > 5)
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/InsteadCreatRoomPanel/InsteadOpenRecord"));
                    go.transform.SetParent(InsteadOpenRecord.transform.Find("Content"));
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.localScale = Vector3.one;
                    go.name = "InsteadOpenRecord" + i;
                }
                //初始化数据列表;
                infinityGridLayoutGroup = InsteadOpenRecord.transform.Find("Content").GetComponent<InfinityGridLayoutGroup>();
                infinityGridLayoutGroup.Init();
                //Invoke("SetAmount", 0.1f);
                infinityGridLayoutGroup.SetAmount(iorpd.RoomMessage.Count);
                infinityGridLayoutGroup.updateChildrenCallback = UpdateChildrenCallback;

            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/InsteadCreatRoomPanel/InsteadOpenRecord"));
                    go.transform.SetParent(InsteadOpenRecord.transform.Find("Content"));
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.localScale = Vector3.one;
                    go.name = "InsteadOpenRecord" + i;
                    InsteadOpenRecordMessage room = go.GetComponent<InsteadOpenRecordMessage>();                    
                    room.RoomMessage = iorpd.RoomMessage[i];
                    room.index = i;
                    room.ShowInsteadRecord();
                }
            }
        }

        void UpdateChildrenCallback(int index, Transform trans)
        {
            InsteadOpenRoomPanelData iorpd = GameData.Instance.InsteadOpenRoomPanelData;
            InsteadOpenRecordMessage room = trans.GetComponent<InsteadOpenRecordMessage>();
            room.RoomMessage = iorpd.RoomMessage[index];
            room.index = index;
            room.ShowInsteadRecord();
        }

    }

}
