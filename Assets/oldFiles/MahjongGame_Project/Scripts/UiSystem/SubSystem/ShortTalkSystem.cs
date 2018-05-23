using UnityEngine;
using System.Collections;
using System.Text;
using MahjongGame_AH.Network.Message;
using MahjongGame_AH.Data;
using MahjongGame_AH.Network;
using System;
using XLua;

namespace MahjongGame_AH.GameSystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class ShortTalkSystem : GameSystemBase
    {

        protected override void HandleOnEnterScene(SceneManager_anhui sender)
        {
            switch (sender.EnteringScene)
            {
                case ESCENE.MAHJONG_GAME_MAIN_SCENE:
                    Messenger_anhui.AddListener(MainViewShortTalkPanel.MessageOpenChatPenal, HandleBtnOpenPenal);
                    Messenger_anhui.AddListener(MainViewShortTalkPanel.MessageCloseChatPenal, HandleBtnClosePenal);
                    Messenger_anhui<int>.AddListener(MainViewShortTalkPanel.MessageClickEmotion, HandleBtnClickFace);
                    break;
                default:
                    break;
            }
        }

        private void HandleBtnClickFace(int _mID)
        {
            NetMsg.ClientChatReq msg = new Network.Message.NetMsg.ClientChatReq();
            msg.iUserId[0] = anhui.MahjongCommonMethod.Instance.iUserid;
            msg.iUserId[1] = 0;
            msg.byChatType = 1;
            msg.byContentId = Convert.ToByte(_mID);
            NetworkMgr.Instance.GameServer.SendChatReq(msg);
        }

        protected override void HandleOnLeaveScene(SceneManager_anhui sender)
        {
            switch (sender.LeavingScene)
            {
                case ESCENE.MAHJONG_GAME_MAIN_SCENE:
                    Messenger_anhui.RemoveListener(MainViewShortTalkPanel.MessageOpenChatPenal, HandleBtnOpenPenal);
                    Messenger_anhui.RemoveListener(MainViewShortTalkPanel.MessageCloseChatPenal, HandleBtnClosePenal);
                    Messenger_anhui<int>.RemoveListener(MainViewShortTalkPanel.MessageClickEmotion, HandleBtnClickFace);
                    break;
                default:
                    break;
            }
        }

        private void HandleBtnClosePenal()
        {
            ShortTalkData std = GameData.Instance.ShortTalkData;
            std.isShowPanel = false;
            SystemMgr.Instance.ShortTalkSystem.UpdateShow();
        }

        private void HandleBtnOpenPenal()
        {
            ShortTalkData std = GameData.Instance.ShortTalkData;
            std.isShowPanel = true;
            SystemMgr.Instance.ShortTalkSystem.UpdateShow();

        }

        public delegate void ShortTalkUpdateEventHandler();
        public ShortTalkUpdateEventHandler OnShortTalkPanelUpdate;

        public void UpdateShow()
        {
            if (OnShortTalkPanelUpdate != null)
            {
                OnShortTalkPanelUpdate();
            }
        }

    }
}
