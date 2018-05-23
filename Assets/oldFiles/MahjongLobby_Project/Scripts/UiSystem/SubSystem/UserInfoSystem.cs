using MahjongLobby_AH.Data;
using UnityEngine;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class UserInfoSystem:GameSystemBase 
    {
        #region 事件处理
        protected override void HandleOnEnterScene(SceneManager_anhui sender)
        {
            switch (sender.EnteringScene )
            {
                case ESCENE.MAHJONG_LOBBY_MAIN_SCENE:
                    Messenger_anhui.AddListener(MainViewUserInfoPanel.MESSAGE_BTNCLOSE, HandleBtnClose);    //点击面板的关闭按钮
                    Messenger_anhui.AddListener(MainViewUserInfoPanel.MESSAGE_BTNSETTING, HandleBtnSettingOpen);  //点击系统设置按钮
                    Messenger_anhui.AddListener(MainViewUserInfoPanel.MESSAGE_BTNSETTINGCLOSE, HandleBtnSettingClose);
                    Messenger_anhui<float>.AddListener(MainViewUserInfoPanel.MESSAGE_MUSICVOLUME, HandleChangeMusic);
                    Messenger_anhui<float>.AddListener(MainViewUserInfoPanel.MESSAGE_MUSICEFFECTVOLUME, HandleChangeEffect);
                    Messenger_anhui.AddListener(MainViewUserInfoPanel.MESSAGE_SETMUSIC, HandleSetMusic);                 
                    Messenger_anhui.AddListener(MainViewUserInfoPanel.MESSAGE_SETMUSICEFFECT, HandleSetMusicEffect);
                    Messenger_anhui.AddListener(MainViewUserInfoPanel.MESSAGE_LOGOUT, HandleLogOut);
                    Messenger_anhui.AddListener(MainViewUserInfoPanel.MESSAGE_BACKTOLOGINPANEL, HandleBackToLoginPanel);
                    break;
                default:
                    break;
            }
        }
        protected override void HandleOnLeaveScene(SceneManager_anhui sender)
        {
            switch (sender.LeavingScene)
            {
                case ESCENE.MAHJONG_LOBBY_MAIN_SCENE:
                    Messenger_anhui.RemoveListener(MainViewUserInfoPanel.MESSAGE_BTNCLOSE, HandleBtnClose);
                    Messenger_anhui.RemoveListener(MainViewUserInfoPanel.MESSAGE_BTNSETTING, HandleBtnSettingOpen);
                    Messenger_anhui.RemoveListener(MainViewUserInfoPanel.MESSAGE_BTNSETTINGCLOSE, HandleBtnSettingClose);
                    Messenger_anhui<float>.RemoveListener(MainViewUserInfoPanel.MESSAGE_MUSICVOLUME, HandleChangeMusic);
                    Messenger_anhui<float>.RemoveListener(MainViewUserInfoPanel.MESSAGE_MUSICEFFECTVOLUME, HandleChangeEffect);
                    Messenger_anhui.RemoveListener(MainViewUserInfoPanel.MESSAGE_SETMUSIC, HandleSetMusic);
                    Messenger_anhui.RemoveListener(MainViewUserInfoPanel.MESSAGE_SETMUSICEFFECT, HandleSetMusicEffect);                    
                    Messenger_anhui.RemoveListener(MainViewUserInfoPanel.MESSAGE_LOGOUT, HandleLogOut);
                    Messenger_anhui.RemoveListener(MainViewUserInfoPanel.MESSAGE_BACKTOLOGINPANEL, HandleBackToLoginPanel);
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理

        public delegate void UserInfoUpdateEventHandler();
        public UserInfoUpdateEventHandler OnUserInfoUpdate;

        public void UpdateShow()
        {
            if (OnUserInfoUpdate !=null)
            {
                OnUserInfoUpdate();
            }
        }

        /// <summary>
        /// 关闭玩家信息按钮
        /// </summary>
        void HandleBtnClose()
        {            
            GameData gd = GameData.Instance;
            UserInfoPanelData uipd = gd.UserInfoPanelData;
            uipd.isPanelShow = false;
            GameData.Instance.LobbyMainPanelData.isShowBuyRoomCard = false;
            UpdateShow();
            SystemMgr.Instance.LobbyMainSystem.UpdateShow();
        }
        /// <summary>
        /// 打开系统设置按钮
        /// </summary>
        void HandleBtnSettingOpen()
        {
            GameData gd = GameData.Instance;
            UserInfoPanelData uipd = gd.UserInfoPanelData;
            uipd.SettingPanelShow = true;
            UpdateShow();
        }

        /// <summary>
        /// 关闭系统设置按钮
        /// </summary>
        void HandleBtnSettingClose()
        {
            GameData gd = GameData.Instance;
            UserInfoPanelData uipd = gd.UserInfoPanelData;
            uipd.SettingPanelShow = false;
            UpdateShow();
        }

        /// <summary>
        /// 该变音乐大小
        /// </summary>
        /// <param name="volume"></param>
        void HandleChangeMusic(float volume)
        {
            GameData gd = GameData.Instance;
            anhui.MahjongCommonMethod.Instance.MusicVolume  = (int)(volume * 100f);
            SystemMgr.Instance.BgmSystem.UpdateVolume();
        }

        /// <summary>
        /// 改变音效的大小
        /// </summary>
        /// <param name="volume"></param>
        void HandleChangeEffect(float volume)
        {
            GameData gd = GameData.Instance;
            anhui.MahjongCommonMethod.Instance.EffectValume  = (int)(volume * 100f);
            SystemMgr.Instance.AudioSystem.UpdateVolume();
        }

        /// <summary>
        /// 处理设置音乐
        /// </summary>
        void HandleSetMusic()
        {
            anhui.MahjongCommonMethod  com = anhui.MahjongCommonMethod.Instance ;
            if (com.isMusicShut )
            {
                com.isMusicShut = false  ;
            }
            else
            {
                com.isMusicShut = true;
               // MahjongCommonMethod.Instance.EffectValume = 0;
            }
            SystemMgr.Instance.UserInfoSystem.UpdateShow();
            SystemMgr.Instance.BgmSystem.UpdateVolume();            
        }

        /// <summary>
        /// 处理设置音效
        /// </summary>
        void HandleSetMusicEffect()
        {
            anhui.MahjongCommonMethod  uipd = anhui.MahjongCommonMethod.Instance ;
            if(uipd.isEfectShut )
            {
                uipd.isEfectShut  = false;
            }
            else
            {
                uipd.isEfectShut  = true;
            }
            SystemMgr.Instance.UserInfoSystem.UpdateShow();
            SystemMgr.Instance.AudioSystem.UpdateVolume();            
        }

       

        /// <summary>
        /// 退出登录
        /// </summary>
        void HandleLogOut()
        {
         //   SDKManager.Instance.iUserId_iAuthType_ServerType = MahjongCommonMethod.Instance.iUserid + LobbyContants.SeverType;//赋值
            PlayerPrefs.DeleteKey(SDKManager.Instance.iUserId_iAuthType_ServerType);           
        }
        /// <summary>
        /// 返回登陆面板
        /// </summary>
        void HandleBackToLoginPanel()
        {
            anhui.MahjongCommonMethod.isAuthenSuccessInLobby = false;

            anhui.MahjongCommonMethod.Instance.InitScene(2);
            GameData.Instance.UserInfoPanelData.isPanelShow = false;
            UpdateShow();
            ////断开游戏/大厅服务器连接
            //if (MahjongLobby_AH.Network.NetworkMgr.Instance)
            //{                
            //    MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyServer.Disconnect();
            //    //清空gc垃圾
            //    System.GC.Collect();
            //    MahjongLobby_AH.Scene.SceneManager.Instance.LoadScene(MahjongLobby_AH.Scene.ESCENE.MAHJONG_LOBBY_GENERAL);           
            //}          
        }
    }

}
