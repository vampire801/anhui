using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongGame_AH.GameSystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class ServerSystem : GameSystemBase
    {
        #region 事件处理
        /// <summary>
        /// 当进入场景
        /// </summary>
        /// <param name="sender">场景管理器</param>
        protected override void HandleOnEnterScene(SceneManager_anhui sender)
        {
            switch (sender.EnteringScene)
            {
              
                case ESCENE.MAHJONG_GAME_MAIN_SCENE:
                    GameSceneInit();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 当离开场景
        /// </summary>
        /// <param name="sender">场景管理器</param>
        protected override void HandleOnLeaveScene(SceneManager_anhui sender)
        {
            switch (sender.EnteringScene)
            {

               
                default:
                    break;
            }
        }

        /// <summary>
        /// 处理接收到的网络数据
        /// </summary>
        /// <param name="cMsgType">消息类型</param>
        protected override void HandleOnRecvData(ushort cMsgType)
        {
            switch (cMsgType)
            {                                
                default:
                    break;
            }
        }

        #endregion 事件处理

        /// <summary>
        /// 进入游戏场景之后，直接连接游戏服务器
        /// </summary>
        void GameSceneInit()
        {
            //Debug.LogError("开始直接连接游戏服务器");
            Network.NetworkMgr.Instance.GameServer.Connect();
        }
    }

}
