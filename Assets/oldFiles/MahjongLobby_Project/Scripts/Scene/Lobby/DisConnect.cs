using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;
using anhui;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class DisConnect : MonoBehaviour
    {
        public Text content;
        public bool isShowDisConnect; //是否显示断开连接的弹出框
        public GameObject ReTry; //重试              
        public GameObject Custom;//客服界面

        //面板更新显示0关闭 1显示客服界面 2显示重试界面
        public void UpdateShow(int status, string str = TextConstant.NET_MSG_CONNECT_LOBBY_SERVER_FAILED)
        {
            content.text = str;

            if (status == 0)
            {
                isShowDisConnect = false;
                gameObject.SetActive(false);
            }
            else
            {
                if (status == 1)
                {
                    Custom.SetActive(true);
                    ReTry.SetActive(false);
                }
                else if (status == 2)
                {
                    Custom.SetActive(false);
                    ReTry.SetActive(true);
                }
                isShowDisConnect = true;
                gameObject.SetActive(true);
            }
        }


        //确定按钮
        public void Ok()
        {
            Application.Quit();
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
            //如果玩家没有网络连接，点击按钮没有提示网络没有连接            
            if (MahjongCommonMethod.Instance.NetWorkStatus() <= 0)
            {
                MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
            }
            else
            {
                UpdateShow(0);
                Debug.LogError("切换后天台重连=================================5");
                MahjongCommonMethod.Instance.InitScene();
            }
        }


        /// <summary>
        /// 检查设置按钮
        /// </summary>
        public void CheckSet()
        {
            MahjongCommonMethod.Instance.SettingPanel();
        }

    }

}
