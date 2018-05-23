using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class HistroyGradeSystem:GameSystemBase
    {

        #region 事件处理
        /// <summary>
        /// 处理进入场景
        /// </summary>
        /// <param name="sender"></param>
        protected override void HandleOnEnterScene(SceneManager_anhui sender)
        {
            switch (sender.EnteringScene)
            {
                case ESCENE.MAHJONG_LOBBY_MAIN_SCENE:
                   
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
                
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理

        public delegate void HistroyGradeSystemUpdateShow();
        public HistroyGradeSystemUpdateShow HistroyGradeSystemUpdate;


        public void UpdateShow()
        {
            if(HistroyGradeSystemUpdate!=null)
            {
                HistroyGradeSystemUpdate();
            }
        }
      
    }

}
