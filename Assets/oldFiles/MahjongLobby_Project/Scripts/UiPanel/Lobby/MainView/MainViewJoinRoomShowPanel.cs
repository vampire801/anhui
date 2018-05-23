using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewJoinRoomShowPanel : MonoBehaviour
    {
        public InputField RoomID;  //输入房间号的输入框
        public Text[] RoomId; //显示房间id

        public const string MESSAGE_NUMBTN = "MainViewJoinRoomShowPanel.MESSAGE_NUMBTN";   //数字键按钮
        public const string MESSAGE_CLEARBTN = "MainViewJoinRoomShowPanel.MESSAGE_CLEARBTN";  //清空键按钮
        public const string MESSAGE_DELNUM = "MainViewJoinRoomShowPanel.MESSAGE_DELNUM";  //删除数字键按钮
        public const string MESSAGE_BTNOK = "MainViewJoinRoomShowPanel.MESSAGE_BTNOK";  //确定按钮
        public const string MESSAGE_BTNCANEL = "MainViewJoinRoomShowPanel.MESSAGE_BTNCANEL";  //取消按钮


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Messenger_anhui<int>.Broadcast(MESSAGE_NUMBTN, 0);
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Messenger_anhui<int>.Broadcast(MESSAGE_NUMBTN, 1);
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Messenger_anhui<int>.Broadcast(MESSAGE_NUMBTN, 2);
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Messenger_anhui<int>.Broadcast(MESSAGE_NUMBTN, 3);
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Messenger_anhui<int>.Broadcast(MESSAGE_NUMBTN, 4);
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Messenger_anhui<int>.Broadcast(MESSAGE_NUMBTN, 5);
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Messenger_anhui<int>.Broadcast(MESSAGE_NUMBTN, 6);
            }
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Messenger_anhui<int>.Broadcast(MESSAGE_NUMBTN, 7);
            }
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Messenger_anhui<int>.Broadcast(MESSAGE_NUMBTN, 8);
            }
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Messenger_anhui<int>.Broadcast(MESSAGE_NUMBTN, 9);
            }
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Messenger_anhui.Broadcast(MESSAGE_DELNUM);
            }
        }

        /// <summary>
        /// 面板更新
        /// </summary>
        public void UpdateShow()
        {
            GameData gd = GameData.Instance;
            JoinRoomShowPanelData jrpd = gd.JoinRoomShowPanelData;
            if(jrpd.PanelShow)
            {
                gameObject.SetActive(true);
                GameData.Instance.isShowQuitPanel = false;
                //为房间码赋值
                for (int i=0;i<6;i++)
                {
                    if(jrpd.lsRoomId.Count>i)
                    {
                        RoomId[i].text = jrpd.lsRoomId[i].ToString();
                    }
                    else
                    {
                        RoomId[i].text = "";
                    }
                }                              
               
            }
            else
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// 点击数字按钮
        /// </summary>
        /// <param name="num"></param>
        public void BtnNum(GameObject num)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            string name = num.name;
            int NumIndex = Convert.ToInt32(name.Split('_')[1]);
            if(NumIndex<0||NumIndex>9)
            {
                return;
            }
            Messenger_anhui<int>.Broadcast(MESSAGE_NUMBTN, NumIndex);
        }

        /// <summary>
        /// 点击清空键
        /// </summary>
        public void BtnClaerNum()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_CLEARBTN);
        }


        /// <summary>
        /// 点击删除键
        /// </summary>
        public void BtnDelNum()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_DELNUM);
        }
             

        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        public void BtnClose()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_BTNCANEL);
        }
    }

}
