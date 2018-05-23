using UnityEngine;
using UnityEngine.UI;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewGetGiftBagPanel : MonoBehaviour
    {
        public InputField ActivityKey;  //激活码的填入框
        public GameObject CodeContent;  //推广码解释弹框
        public GameObject[] GetBagPanel;  //推广礼包信息界面
        public RawImage HeadImage; //你的推广员的头像
        public Text nickName; //你的推广员的昵称


        #region 常量
        public const string MESSAGE_CLOSEBTN = "MainViewGetGiftBagPanel.MessageClosebtn";   //面板的关闭按钮    
        public const string MESSAGE_ACTIVITYBTN = "MainViewGetGiftBagPanel.MessageActivitybtn";  //面板的激活按钮
        public const string MESSAGE_BTNWHATCODE = "MainViewGetGiftBagPanel.MESSAGE_BTNWHATCODE";  //点击什么是推广码按钮
        public const string MESSAGE_BTNCLOSECODE = "MainViewGetGiftBagPanel.MESSAGE_BTNCLOSECODE";  //点击关闭推广码按钮
        #endregion 常量


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// 面板的显示更新
        /// </summary>
        public void UpdateShow()
        {
            GameData gd = GameData.Instance;
            GetGiftSpreadBagPanelData ggbpd = gd.GetGiftSpreadBagPanelData;
            if (ggbpd.PanelShow)
            {
                gameObject.SetActive(true);

                GameData.Instance.isShowQuitPanel = false;
                //如果玩家已经被推广
                if (GameData.Instance.PlayerNodeDef.iSpreaderId > 0 && GameData.Instance.PlayerNodeDef.iSpreadGiftTime == 0)
                {
                    GetBagPanel[1].SetActive(true);
                    GetBagPanel[0].SetActive(false);//未被推广的
                    nickName.text = string.Format("请领取你的好友【{0}】赠送的推广礼包", ggbpd.szNickName);
                    if (ggbpd.szUrlHead != null )
                    {
                        anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(HeadImage, ggbpd.szUrlHead);
                    }
                }
                else if(GameData.Instance.PlayerNodeDef.iSpreaderId == 0 && GameData.Instance.PlayerNodeDef.iSpreadGiftTime == 0)
                {
                    GetBagPanel[1].SetActive(false);
                    GetBagPanel[0].SetActive(true);
                }

                ActivityKey.text = ggbpd.GiftSpreadCode;
                if (ggbpd.isShowCode)
                {
                    CodeContent.SetActive(true);
                }
                else
                {
                    CodeContent.SetActive(false);
                }
            }
            else
            {
                ActivityKey.text = "";
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }



        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        public void BtnClose()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_CLOSEBTN);
        }

        /// <summary>
        /// 点击激活推广码按钮
        /// </summary>
        public void BtnActivity()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            if (ActivityKey.text.Length != 6)
            {
                anhui.MahjongCommonMethod.Instance.ShowRemindFrame("您输入的推广码不正确，请重新输入");
                return;
            }


            Messenger_anhui<string>.Broadcast(MESSAGE_ACTIVITYBTN, ActivityKey.text);
        }

        /// <summary>
        /// 点击什么是推广码按钮
        /// </summary>
        public void BtnWhatCode()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_BTNWHATCODE);
        }

        /// <summary>
        /// 点击关闭推广码解释按钮
        /// </summary>
        public void BtnCloseWhatCode()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_BTNCLOSECODE);
        }
    }

}
