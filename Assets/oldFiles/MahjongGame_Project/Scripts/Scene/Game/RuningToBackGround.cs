using UnityEngine;
using System;
using System.Collections;
using MahjongGame_AH;
using MahjongGame_AH.Network;
using MahjongGame_AH.Network.Message;
using XLua;
using anhui;

[Hotfix]
[LuaCallCSharp]
public class RuningToBackGround : MonoBehaviour
{
    #region 单例

    static RuningToBackGround instance;
    public static RuningToBackGround Instance
    {
        get
        {
            return instance;
        }
    }

    #endregion
    public DateTime LevelTimer; //离开游戏的时间

    public bool isChangeFromTable;  //是否从后台切换回来

    public bool isPause = false;

    public bool isFocus = false;

    int ClickCount = 0;   //切换到后台次数

    void Awake()
    {
        instance = this;        
    }


    void OnEnable()
    {
        isPause = false;
        isFocus = false;
    }


    // 强制暂停时
    void OnApplicationPause(bool pauseStatus)
    {
#if UNITY_IPHONE || UNITY_ANDROID
        if (pauseStatus)
        {
            // 强制暂停时，事件
            pauseTime();
        }
#endif     
    }


    //“启动”手机时
    void OnApplicationFocus(bool hasFocus)
    {
#if UNITY_IPHONE || UNITY_ANDROID
        if (hasFocus)
        {
            // “启动”手机时，事件
            resumeList();
        }
        else
        {
            MahjongCommonMethod.NetWorkStatus_int = MahjongCommonMethod.Instance.NetWorkStatus();
        }
#endif
        if (!MahjongLobby_AH.GameData.Instance)
        {
            return;
        }
        StartCoroutine(WaitForOneSec());

    }

    private IEnumerator WaitForOneSec()
    {
        yield return new WaitForSeconds(1f);
        MahjongLobby_AH.Data.WXLoginPanelData ld = MahjongLobby_AH.GameData.Instance.WXLoginPanelData;
        if (ld.AuthonState == 1)
        {
            MahjongLobby_AH.SDKManager.Instance.OnGetmCodeCB("cancel");
        }
        Debug.Log("shareState" + MahjongLobby_AH.SDKManager.Instance.shareState);
        if (MahjongLobby_AH.SDKManager.Instance.shareState == 1)
        {
            Debug.LogWarning("启动手机");
            MahjongLobby_AH.SDKManager.Instance.SharedSuccess("UnShared");
        }
    }


    //强制暂停时，事件
    void pauseTime()
    {
        if (Application.isEditor)
        {
            return;
        }

        LevelTimer = DateTime.Now;

        if (MahjongGame_AH.GameData.Instance)
        {
            MahjongGame_AH.Network.Message.NetMsg.ClientEscapeReq msg = new MahjongGame_AH.Network.Message.NetMsg.ClientEscapeReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.byEscape = 1;
            Debug.LogError("强制暂停时发送离线消息");
            SendEscapeReq(msg);
        }
    }


    //“启动”手机时，事件
    void resumeList()
    {
        if (Application.isEditor || LevelTimer.Year == 1 || !MahjongCommonMethod.Instance.isFinshSceneLoad)
        {
            return;
        }

        //大厅
        if (MahjongLobby_AH.GameData.Instance)
        {
            //如果玩家断开连接
            if (!MahjongCommonMethod.Instance.isStartInit_Lobby && (MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now) - MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(LevelTimer) > 300 || !MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyServer.Connected))
            {
                MahjongCommonMethod.Instance.isStartInit_Lobby = true;
                MahjongCommonMethod.isAuthenSuccessInLobby = true;
                LevelTimer = DateTime.Now;
                if (MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now) - MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(LevelTimer) > 300 && !MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyServer.Connected)
                {
                    MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading", "正在加载大厅资源，请稍候...");
                }

                //检查网络情况，如果网络正常则关闭，并重连服务器，都则不会关闭该面板
                if (MahjongCommonMethod.Instance.NetWorkStatus() > 0)
                {
                    MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyGateway.tryCount = 0;
                    //重新连接                    
                    MahjongCommonMethod.Instance.InitScene();
                }
            }

            //如果预约时间为0，请求开放回应，刷新界面
            if (MahjongLobby_AH.GameData.Instance.ParlorShowPanelData.isShowOrderTimePanel)
            {
                MahjongLobby_AH.GameData.Instance.ParlorShowPanelData.iUpdateOrderTimer = 1;
                MahjongLobby_AH.Network.Message.NetMsg.ClientMyRoomInfoReq myroominfo = new MahjongLobby_AH.Network.Message.NetMsg.ClientMyRoomInfoReq();
                myroominfo.iUserId = MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iUserId;
                MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyServer.SendMyRoomInfoReq(myroominfo);
            }
        }

        isChangeFromTable = true;
        //如果玩家在游戏，断开连接之后，从后台回来 直接重新连接
        if (MahjongGame_AH.GameData.Instance)
        {
            Debug.LogError("断开连接之后，从后台回来 直接重新连接======================0");
            if (GameData.Instance.PlayerPlayingPanelData.isGameEnd)
            {
                return;
            }

            //如果时间大于300s,或者已经断开连接
            if (!MahjongCommonMethod.Instance.isStartInit_Game && (MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now) - MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(LevelTimer) > 300 || !MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.Connected))
            {
                MahjongCommonMethod.Instance.isStartInit_Game = true;
                MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.tryCount = 0;
                ////清空消息队列
                //MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.NetClient.m_client.list = new ArrayList();

                Debug.LogError("断开连接之后，从后台回来 直接重新连接======================2");
                LevelTimer = DateTime.Now;
                MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading", "正在加载游戏资源，请稍候...");
                if (MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData.isPanelShow_Playing)
                {
                    UIMainView.Instance.disConnect.RecoverGameScene(2);
                }
                else
                {
                    UIMainView.Instance.disConnect.RecoverGameScene(1);
                }
            }
            else
            {
                MahjongGame_AH.Network.Message.NetMsg.ClientEscapeReq msg = new MahjongGame_AH.Network.Message.NetMsg.ClientEscapeReq();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg.byEscape = 0;
                Debug.LogError("“启动”手机时发送离线消息");
                if (GameData.Instance.PlayerPlayingPanelData.isPanelShow_Playing)
                {
                    JPush.JPushBinding.StopPush();
                }
                NetworkMgr.Instance.GameServer.SendEscapeReq(msg);
            }
        }

        if (MahjongGame_AH.UIMainView.Instance != null && MahjongGame_AH.UIMainView.Instance.PlayerPlayingPanel != null && MahjongGame_AH.UIMainView.Instance.PlayerPlayingPanel.m_isLost60Scend)
        {
            NetMsg.ClientReadyTimeReqDef msg = new NetMsg.ClientReadyTimeReqDef();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.wTableNum = (byte)MahjongCommonMethod.Instance.iTableNum;
            MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.SendClientReadyTimeReqDef(msg);
        }
    }


    //延迟3S之后检查玩家是否掉线
    IEnumerator DelayCheckPlayerNetwork()
    {
        yield return new WaitForSeconds(3f);
        //大厅
        if (MahjongLobby_AH.GameData.Instance)
        {
            //检查网络情况，如果网络正常则关闭，并重连服务器，都则不会关闭该面板
            if (MahjongCommonMethod.Instance.NetWorkStatus() == 0)
            {
                MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
                MahjongLobby_AH.UIMainView.Instance.disConnect.UpdateShow(2);
                yield break;
            }
            if (MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyServer.Connected)
            {
                MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
            }
        }

        //游戏
        if (MahjongGame_AH.GameData.Instance)
        {
            //检查网络情况，如果网络正常则关闭，并重连服务器，都则不会关闭该面板
            if (MahjongCommonMethod.Instance.NetWorkStatus() == 0)
            {
                MahjongGame_AH.UIMainView.Instance.disConnect.UpdateShow(1, "亲，您的网络不给力哦，请检查网络后重试");
                yield break;
            }

            if (MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.Connected)
            {
                MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
            }
        }
    }


    //发送请求消息
    void SendEscapeReq(NetMsg.ClientEscapeReq msg)
    {
        MsgData msgData;
        msgData = new MsgData();
        msgData.cType = NetMsg.CLIENT_ESCAPE_REQ;
        msgData.bEncrypt = false;
        NetMsg.ClientEscapeReq msg_ = (NetMsg.ClientEscapeReq)msg;
        msg_.msgHeadInfo.Version = NetMsg.MESSAGE_VERSION;
        msg_.msgHeadInfo.MsgType = NetMsg.CLIENT_ESCAPE_REQ;
        byte[] item = msg_.toBytes();
        msgData.obj = (object)item;
        NetworkMgr.Instance.GameServer.NetClient.SendCliMsg(msgData.obj, msgData.cType, msgData.bEncrypt);
    }
}
