using UnityEngine;
using System.Collections.Generic;
using MahjongGame_AH.Network.Message;
using XLua;

namespace MahjongGame_AH.Network
{
    [Hotfix]
    [LuaCallCSharp]
    ///// <summary>
    /// 代理信息类
    /// </summary>
    public class ProxyInfo
    {
        public string ip;
        public ushort port;
    }

    /// <summary>
    /// 消息数据
    /// </summary>
    public class MsgData
    {
        public ushort cType;
        public bool bEncrypt;
        public object obj;
    }

    /// <summary>
    /// 网络管理器类
    /// </summary>
    public class NetworkMgr : MonoBehaviour
    {

        #region 实例

        static NetworkMgr instance;
        public static NetworkMgr Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion 实例

        #region 委托事件

        /// <summary>
        /// 接收消息
        /// </summary>
        public delegate void RecvDataEventHandler(ushort cMsgType);
        public event RecvDataEventHandler OnRecvData;

        #endregion 委托事件

        #region 代理服务器

        /// <summary>
        /// 最多设置5个代理
        /// </summary>
        public const int MAX_PROXY_NUM = 5;
        /// <summary>
        /// 是否使用代理，0不用代理，1使用代理
        /// </summary>
        [HideInInspector]
        public int
            ProxyEnable = 0;
        /// <summary>
        /// 代理信息列表
        /// </summary>
        public List<ProxyInfo> ProxyInfoList = new List<ProxyInfo>();

        #endregion 代理服务器

        #region 各个网络客户端的实例

        /// <summary>
        /// 大厅网关
        /// </summary>
        MahjongGameGateway gameGateway;
        public MahjongGameGateway GameGateway
        {
            get
            {
                return gameGateway;
            }
        }


        /// <summary>
        /// 游戏服务器
        /// </summary>
        MahjongGameServer gameServer;
        public MahjongGameServer GameServer
        {
            get
            {
                return gameServer;
            }
        }


        #endregion 各个网络客户端的实例

        #region 父类的方法

        /// <summary>
        /// 唤醒
        /// </summary>
        void Awake()
        {
          
            instance = this;
            //DontDestroyOnLoad(gameObject);
            gameGateway = new MahjongGameGateway();
            gameServer = new MahjongGameServer();
            if (anhui.MahjongCommonMethod.Instance)
            {
                //Debug.LogError("SeverIp：" + MahjongCommonMethod.Instance.SeverIp + ",SeverPort：" + MahjongCommonMethod.Instance.SeverPort);
                gameServer.Init(anhui.MahjongCommonMethod.Instance.SeverIp, anhui.MahjongCommonMethod.Instance.SeverPort);
            }
        }


        /// <summary>
        /// 更新
        /// </summary>
        void Update()
        {


            if (gameGateway != null)
            {
                gameGateway.Update();
            }

            if (gameServer != null)
            {
                gameServer.Update();
            }
        }

        #endregion 父类的方法

        #region 公共的成员方法

        /// <summary>
        /// 当收到消息
        /// </summary>
        /// <param name="cMsgType">消息类型</param>
        public void OnNetMessage(ushort cMsgType)
        {
            if (OnRecvData != null)
            {
                OnRecvData(cMsgType);
            }
        }


        /// <summary>
        /// 给游戏网络保持连接
        /// </summary>
        public void KeepGameServerAlive()
        {
            InvokeRepeating("SendKeepGameNetAlive", 30, 30);
        }

        /// <summary>
        /// 给游戏网络发送保持连接消息
        /// </summary>
        public void SendKeepGameNetAlive()
        {
            SendGameMessage(NetMsg.KEEP_ALIVE);
        }
        /// <summary>
        /// 发送消息给游戏服务器
        /// </summary>
        /// <param name="cMsgType">消息类型</param>
        public void SendGameMessage(byte cMsgType)
        {
            NetworkMgr.instance.GameServer.SendKeepAlive();
        }


        public void DisconnectAll()
        {
            if (gameServer != null)
            {
                gameServer.Disconnect();
            }

            if (gameGateway != null)
            {
                gameGateway.Disconnect();
            }
        }

        #endregion 公共的成员方法
    }
}


