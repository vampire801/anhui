using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using XLua;
using anhui;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class DisConnect : MonoBehaviour
    {
        public bool isShowDisConnect; //是否显示断开连接的面板
        public GameObject ReTry; //重试              
        public GameObject Custom;//客服界面
        public Text Content; //显示内容

        //显示0表示不可用，1表示显示重试界面  2表示显示客服界面 
        public void UpdateShow(int status, string text)
        {
            if (status == 0)
            {
                gameObject.SetActive(false);
                isShowDisConnect = false;
            }
            else
            {
                if (GameData.Instance && GameData.Instance.PlayerPlayingPanelData.isGameEnd || gameObject == null)
                {
                    return;
                }

                if (status == 1)
                {
                    ReTry.SetActive(true);
                    Custom.SetActive(false);
                }
                else if (status == 2)
                {
                    ReTry.SetActive(false);
                    Custom.SetActive(true);
                }
                gameObject.SetActive(true);
                Content.text = text;
                isShowDisConnect = true;
            }
        }

        /// <summary>
        /// 点击重试按钮
        /// </summary>
        public void BtnRetry()
        {
            //弹出提示，一秒后关闭
            MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading", "游戏重连中");
            StartCoroutine(OneSecond());
        }

        IEnumerator OneSecond()
        {
            yield return new WaitForSeconds(1f);
            if (MahjongCommonMethod.Instance.NetWorkStatus() > 0)
            {
                //进入到这里面，说明玩家游戏内断网
                MahjongCommonMethod.Instance.isGameDisConnect = true;

              //  Debug.LogError("GameData.Instance.PlayerPlayingPanelData.isBeginGame:" + GameData.Instance.PlayerPlayingPanelData.isBeginGame);

                //开始重新加载界面
                if (MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData.isBeginGame)
                {
                    //开始游戏之后，恢复加载界面
                    RecoverGameScene(2);
                }
                else
                {
                    //等待界面，恢复加载界面
                    RecoverGameScene(1);
                }
                gameObject.SetActive(false);
            }
            else
            {
                MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
            }
        }

        /// <summary>
        /// 检查设置按钮
        /// </summary>
        public void CheckSet()
        {
            MahjongCommonMethod.Instance.SettingPanel();
        }

        //点击确定按钮
        public void BtnOk()
        {
            //退出程序
            Application.Quit();
        }

        /// <summary>
        ///  游戏内恢复场景
        /// </summary>
        /// <param name="status">1表示等待状态，2表示游戏状态</param>
        public void RecoverGameScene(int status)
        {
            MahjongGame_AH.Data.PlayerPlayingPanelData pppd = MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData;
            //主动断开游戏服务器
            if (MahjongGame_AH.Network.NetworkMgr.Instance)
            {
                MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.Disconnect();
            }

            //打开加载界面
            MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading", "重新登录...");            
            //处理等待
            if (status == 1)
            {
                //初始化游戏面板数据
                pppd.usersInfo = new Dictionary<int, MahjongGame_AH.Network.Message.NetMsg.UserInfoDef>();
            }
            else if (status == 2)
            {
                //初始化
                UIMainView.Instance.PlayerPlayingPanel.InitPanel();
            }

            //重新连接游戏服务器            
            MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.Connect();
        }

    }
}

