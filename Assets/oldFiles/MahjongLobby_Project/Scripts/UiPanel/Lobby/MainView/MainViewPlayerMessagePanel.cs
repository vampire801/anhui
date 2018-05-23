using UnityEngine;
using System.Collections;
using MahjongLobby_AH.Data;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewPlayerMessagePanel : MonoBehaviour
    {
        public GameObject Grid;  //存放所有消息的grid
        public Scrollbar bar;  //滑动条的bar，用于判断滑动条是否拉到底部
        public GameObject NoneMessage; //暂无消息显示记录显示界面


        public const string MESSAGE_CLOSEBTN = "MainViewPlayerMessagePanel.MESSAGE_CLOSEBTN"; //点击关闭面板的按钮
        public const string MESSAGE_JOINAGENCY = "MainViewPlayerMessagePanel.MESSAGE_JOINAGENCY";  //点击加入代理的加入按钮
        public const string MESSAGE_DISJOINAGENCY = "MainViewPlayerMessagePanel.MESSAGE_NOTJOINAGENCY";  //点击拒绝加入代理的按钮
        public const string MESSAGE_AGREECANELRELATION = "MainViewPlayerMessagePanel.MESSAGE_AGREECANELRELATION";  //同意解除代理关系
        public const string MESSAGE_DISAGREECANELRELATION = "MainViewPlayerMessagePanel.MESSAGE_DISAGREECANELRELATION";  //拒绝解除代理关系


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 面板更新显示
        /// </summary>
        public void UpdateShow()
        {
            GameData gd = GameData.Instance;
            PlayerMessagePanelData pmpd = gd.PlayerMessagePanelData;
            if(pmpd.PanelShow)
            {                
                gameObject.SetActive(true);
                GameData.Instance.isShowQuitPanel = false;
            }
            else
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }


     
        /// <summary>
        /// 产生对应的消息面板
        /// </summary>
        public void SpwanAgencyMessagePanel()
        {
            GameData gd = GameData.Instance;
            PlayerMessagePanelData pmpd = gd.PlayerMessagePanelData;
            pmpd.PanelShow = true;
            UpdateShow();
            //初始化之前的数据
            PlayerMessagePrefabCommon[] message = Grid.GetComponentsInChildren<PlayerMessagePrefabCommon>();                
            if(message.Length>0)
            {
                for(int i=0;i<message.Length;i++)
                {
                    Destroy(message[i].gameObject);
                }
            }

           
            //如果玩家没有消息，直接return
            if(pmpd.PlayerMessageData[0].data.Count<=0)
            {
                NoneMessage.SetActive(true);
                return;
            }
            else
            {
                NoneMessage.SetActive(false);
            }
            
            ////修改content的值
            //float height = (pmpd.PlayerMessageData[0].data.Count-1) * 170f;           
            //Grid.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            //Grid.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);

            //先清除玩家的之前的消息,后期在进行优化
            PlayerMessagePrefabCommon[] prefab = Grid.GetComponentsInChildren<PlayerMessagePrefabCommon>();
            if(prefab.Length>0)
            {
                for(int i=0;i<prefab.Length;i++)
                {
                    Destroy(prefab[i].gameObject);
                }
            }
            //产生新的消息
            for(int i=0;i<pmpd.PlayerMessageData[0].data.Count;i++)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/PlayerMessagePanel/RelationMessage"));
                go.transform.SetParent(Grid.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                go.transform.localEulerAngles = Vector3.zero;
                //为消息预置体赋值
                go.GetComponent<PlayerMessagePrefabCommon>().messageContent = pmpd.PlayerMessageData[0].data[i];
            }
        }

        /// <summary>
        /// 对玩家的消息数据按时间进行排序
        /// </summary>
        void MessageSort()
        {
            //GameData gd = GameData.Instance;
            //PlayerMessagePanelData pmpd = gd.PlayerMessagePanelData;                      
        }

        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        public void BtnClose()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_CLOSEBTN);
        }

        /// <summary>
        /// 同意加入代理
        /// </summary>
        public void BtnJoinAgency()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_JOINAGENCY);
        }

        /// <summary>
        /// 不同意加入代理
        /// </summary>
        public void BtnDisJoinAgency()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_DISJOINAGENCY);
        }

        /// <summary>
        /// 同意解除代理关系
        /// </summary>
        public void BtnAgreeCanelRelation()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_AGREECANELRELATION);
        }

        /// <summary>
        /// 不同意解除代理关系
        /// </summary>
        public void BtnDisAgreeCanelRelation()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_DISAGREECANELRELATION);
        }

        
    }

}
