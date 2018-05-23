using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
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
                //			case ESCENE.LOBBY_SCENE:
                //				GameSceneInit();
                //				break;
                case ESCENE.MAHJONG_LOBBY_MAIN_SCENE:

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
            switch (sender.LeavingScene)
            {
                case ESCENE.MAHJONG_LOBBY_MAIN_SCENE:
                    //GameSceneInit();
                    break;

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
                //TODO:  处理认证回应消息
                //消息种类
                //case NetMsg.CLIENT_AUTHEN_RES: //0x02
                //    HandleAuthenRes();
                //    break;


                default:
                    break;
            }
        }

        #endregion 事件处理

        #region 网络消息处理

        //TODO: 处理玩家报名回应消息

        //TODO: 处理玩家认证回应消息

        #endregion 网络消息处理

        #region 公共成员方法

        /// <summary>
        /// 选择游戏服务器来连接
        /// </summary>
        public void ChooseGameServerToConnect()
        {

        }

        #endregion 公共成员方法


        #region 私有成员方法
        void GameSceneInit()
        {
            //Debug.Log("Lobby ServerSystem Start Connect Network.");
            //进去主场景后先连接报名服务器进入游戏服务器，再检查版本
            MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyGateway.Connect();
        }

        /// <summary>
        /// 加载设置
        /// </summary>
        void LoadSettings()
        {
            //游戏数据加载
            DEBUG.Log("加载设置");
        }
        #endregion 私有成员方法

    }
}

