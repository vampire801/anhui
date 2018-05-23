using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using anhui;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class LobbyBulletin : MonoBehaviour
    {
        #region 单例
        static LobbyBulletin instance;
        public static LobbyBulletin Instance
        {
            get
            {
                return instance;
            }
        }

        void Awake()
        {
            instance = this;
        }
        #endregion

        public bool NoGetBulletinData; //是否要获取登录公告的数据
        public int Status; //登录状态，1表示正常登录 2表示登录服务器失败，显示公告信息
        public GameObject bg;  //大厅公告背景
        public Text title; //公告的标题
        public Text content;  //公告的内容
        public ScrollRect Rect;
        public GameObject Grid_C;

        //保存大厅公告
        public List<BulletinContent> BulletinContentData = new List<BulletinContent>();

        public class BulletinContent
        {
            public string title;  //公告标题
            public string content;  //公告内容
        }

        public class LoginBulletin
        {
            public int status;  //1成功  0参数错误  9系统错误
            public List<BulletinContent> data;
        }


        public class lsBulletinContent
        {
            public List<LoginBulletin> LoginBulletinData = new List<LoginBulletin>();
        }

        /// <summary>
        /// 获取大厅登录公告内容
        /// </summary>
        /// <param name="json"></param>
        public void GetBulletinData(string json, int status)
        {
            //Debug.LogError("登录公告信息内容:" + json);

            BulletinContentData.Clear();
            lsBulletinContent data = new lsBulletinContent();
            data = JsonBase.DeserializeFromJson<lsBulletinContent>(json.ToString());
            if (data.LoginBulletinData[0].status != 1)
            {
                Debug.LogError("获取网页json数据状态错误,status:" + data.LoginBulletinData[0].status);
                return;
            }

            //表示没有公告，直接显示登陆界面
            if (data.LoginBulletinData[0].data.Count == 0 || SDKManager.Instance.IOSCheckStaus == 1)
            {
                bg.SetActive(false);
                GameData gd = GameData.Instance;
                gd.WXLoginPanelData.isPanelShow = true;
                SystemMgr.Instance.WXLoginSystem.UpdateShow();
                // SDKManager.Instance.iUserId_iAuthType_ServerType = MahjongCommonMethod .Instance .iUserid + LobbyContants.SeverType;//赋值
                if (PlayerPrefs.HasKey(SDKManager.Instance.iUserId_iAuthType_ServerType) && Network.NetworkMgr.Instance.LobbyServer.Connected)
                {
                    SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading");//加载loading画面---认证回应
                    GameData.Instance.PlayerNodeDef.isSencondTime = 2;
                    //判断是否过期
                    Debug.LogError("玩家认证类型:===================1");
                    SDKManager.Instance.GetRefreshToken(PlayerPrefs.GetString(SDKManager.Instance.szrefresh_token));
                }
                return;
            }

            for (int i = 0; i < data.LoginBulletinData.Count; i++)
            {
                for (int j = 0; j < data.LoginBulletinData[i].data.Count; j++)
                {
                    BulletinContent content = new BulletinContent();
                    content.title = data.LoginBulletinData[i].data[j].title;
                    content.content = "\t" + data.LoginBulletinData[i].data[j].content;
                    BulletinContentData.Add(content);
                }
            }

            ShowBulletin();
        }

        /// <summary>
        /// 显示公告界面
        /// </summary>
        public void ShowBulletin()
        {
            if (BulletinContentData.Count <= 0)
            {
                //显示登录界面
                bg.SetActive(false);
                GameData gd = GameData.Instance;
                gd.WXLoginPanelData.isPanelShow = true;
                SystemMgr.Instance.WXLoginSystem.UpdateShow();
                return;
            }
            bg.SetActive(true);
            title.text = BulletinContentData[0].title;
            content.text = BulletinContentData[0].content;

            if (content.preferredHeight > 300f)
            {
                Rect.enabled = true;
                Grid_C.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, content.preferredHeight + 20f);
            }
            else
            {
                Rect.enabled = false;
            }

        }

        /// <summary>
        /// 点击确定按钮
        /// </summary>
        public void BtnOk()
        {
            if (Status == 1)
            {
                //显示登陆界面
                GameData gd = GameData.Instance;
                gd.WXLoginPanelData.isPanelShow = true;
                SystemMgr.Instance.WXLoginSystem.UpdateShow();
                // SDKManager.Instance.iUserId_iAuthType_ServerType = MahjongCommonMethod.Instance.iUserid + LobbyContants.SeverType;//赋值
                if (PlayerPrefs.HasKey(SDKManager.Instance.iUserId_iAuthType_ServerType))//自动跳转
                {
                    SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("loading");//加载loading画面---认证回应
                    GameData.Instance.PlayerNodeDef.isSencondTime = 2;
                    Debug.LogError("玩家认证类型:===================2");
                    //判断是否过期
                    SDKManager.Instance.GetRefreshToken(PlayerPrefs.GetString(SDKManager.Instance.szrefresh_token));
                }
            }
            //关闭自己
            gameObject.SetActive(false);
            //删除显示更新界面的预置体
            if (CheckUpdateManager.Instance)
            {
                CheckUpdateManager.Instance.OnCloseHouKenTral();
            }
        }
    }

}
