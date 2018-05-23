using UnityEngine;
using MahjongLobby_AH.Data;
using UnityEngine.UI;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;
using anhui;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewUserInfoPanel : MonoBehaviour
    {
        public GameObject SettingPanel;  //设置面板
        public GameObject ParlorMessage; //玩家麻将馆信息，在审核状态下 隐藏
        public RawImage headImage;  //头像   
        public Button[] _btnFlagM;//音乐
        public Button[] _btnFlagE;//音效
        public Slider _sliderM;
        public Slider _sliderE;

        #region 麻将馆相关
        public Text ParlorIdentity;  //麻将馆身份
        public Text JoinParlorTitle; //创建或加入麻将馆的标题
        public GameObject[] Parlor;  //麻将馆的详细信息
        public Text[] ParlorCount;  //操作麻将馆的次数
        public GameObject NoneParlor; //没有麻将馆提示
        #endregion

        //存储玩家节点0表示出牌速度，1表示逃跑率，2表示赞的数量，3表示当前ip，4表示玩家id 5表示玩家id
        public Text[] PlayerNode;
        public const string MESSAGE_BTNSETTING = "MainViewUserInfoPanel.BTNNOTEOPEN";   //点击系统设置按钮
        public const string MESSAGE_BTNSETTINGCLOSE = "MainViewUserInfoPanel.MESSAGE_BTNSETTINGCLOSE";  //点击系统设置的关闭按钮
        public const string MESSAGE_BTNCLOSE = "MainViewUserInfoPanel.BTNNOTECLOSE";    //点击玩家信息的关闭按钮
        public const string MESSAGE_MUSICVOLUME = "MainViewUserInfoPanel.MESSAGE_MUSICVOLUME";  //改变音量大小
        public const string MESSAGE_MUSICEFFECTVOLUME = "MainViewUserInfoPanel.MESSAGE_MUSICEFFECTVOLUME";  //改变音效的大小
        public const string MESSAGE_SETMUSIC = "MainViewUserInfoPanel.MESSAGE_CLOSEMUSIC";  //直接设置开关音乐        
        public const string MESSAGE_SETMUSICEFFECT = "MainViewUserInfoPanel.MESSAGE_CLOSEMUSICEFFECT";  //直接设置开关音效        
        public const string MESSAGE_LOGOUT = "MainViewUserInfoPanel.MESSAGE_LOGOUT";//登出
        public const string MESSAGE_BACKTOLOGINPANEL = "MainViewUserInfoPanel.MESSAGE_BACKTOLOGINPANEL";//返回登陆面板        
        public GameObject _Music;
        public GameObject _Effect;


        void Start()
        {
            //如果玩家第一次打开头像面板，则弹出新手引导
            if (PlayerPrefs.GetFloat(NewPlayerGuide.Guide.PlayerGiftCode.ToString()) == 0)
            {
                NewPlayerGuide.Instance.OpenIndexGuide(NewPlayerGuide.Guide.PlayerGiftCode);
                NewPlayerGuide.Instance.SetTimeHideGuide_Ie(5f, NewPlayerGuide.Guide.PlayerGiftCode);
            }

        }

        void OnEnable()
        {
            //关闭新手引导
            if (PlayerPrefs.GetFloat(NewPlayerGuide.Guide.PlayerGiftCode.ToString()) == 0)
            {
                NewPlayerGuide.Instance.HideIndexGuide(NewPlayerGuide.Guide.PlayerGiftCode);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 更新面板
        /// </summary>
        public void UpdateShow()
        {
            GameData gd = GameData.Instance;
            UserInfoPanelData uipd = gd.UserInfoPanelData;
            if (uipd.isPanelShow)
            {
                gameObject.SetActive(true);

                if (SDKManager.Instance.IOSCheckStaus == 1 || SDKManager.Instance.CheckStatus == 1)
                {
                    ParlorMessage.SetActive(false);
                }
                else
                {
                    ParlorMessage.SetActive(true);
                }

                if (gd.PlayerNodeDef.iPlayCardAcc != 0)
                {
                    PlayerNode[0].text = (gd.PlayerNodeDef.iPlayCardTimeAcc / gd.PlayerNodeDef.iPlayCardAcc).ToString("0") + "秒/平均";
                }
                else
                {
                    PlayerNode[0].text = "0秒/平均";
                }

                // Debug.LogError("游戏局数：" + gd.PlayerNodeDef.iGameNumAcc + "，掉线次数：" + gd.PlayerNodeDef.iDisconnectAcc);

                if (gd.PlayerNodeDef.iGameNumAcc != 0)
                {
                    PlayerNode[1].text = ((gd.PlayerNodeDef.iDisconnectAcc / (float)gd.PlayerNodeDef.iGameNumAcc) * 100f).ToString("0.00") + "%";
                }
                else
                {
                    PlayerNode[1].text = "0%";
                }
                //声音关闭按钮显示
                Button[] gomusic = _Music.GetComponentsInChildren<Button>(true);
                gomusic[0].gameObject.SetActive(MahjongCommonMethod.Instance.isMusicShut);
                gomusic[1].gameObject.SetActive(!MahjongCommonMethod.Instance.isMusicShut);
                Button[] goeffect = _Effect.GetComponentsInChildren<Button>(true);
                goeffect[0].gameObject.SetActive(MahjongCommonMethod.Instance.isEfectShut);
                goeffect[1].gameObject.SetActive(!MahjongCommonMethod.Instance.isEfectShut);
                _sliderM.value = MahjongCommonMethod.Instance.MusicVolume * 0.01f;
                _sliderE.value = MahjongCommonMethod.Instance.EffectValume * 0.01f;
                PlayerNode[2].text = gd.PlayerNodeDef.iCompliment.ToString();
                PlayerNode[3].text = MahjongCommonMethod.PlayerIp;
                PlayerNode[4].text = gd.PlayerNodeDef.iUserId.ToString();
                PlayerNode[5].text = gd.PlayerNodeDef.szNickname;
                MahjongCommonMethod com = MahjongCommonMethod.Instance;
                com.GetPlayerAvatar(headImage, gd.PlayerNodeDef.szHeadimgurl);
                if (uipd.SettingPanelShow)
                {
                    SettingPanel.SetActive(true);
                }
                else
                {
                    SettingPanel.SetActive(false);
                }

                //处理麻将馆信息
                PlayerNodeDef pnd = GameData.Instance.PlayerNodeDef;
                if (pnd.iMyParlorId > 0)
                {
                    ParlorIdentity.text = "麻将馆老板";
                    JoinParlorTitle.text = "创建的麻将馆：";
                }
                else
                {
                    ParlorIdentity.text = "麻将馆会员";
                    JoinParlorTitle.text = "加入的麻将馆：";
                }
                ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                int parlorCount = 0;
                for (int i = 0; i < pspd.parlorInfoDef.Length; i++)
                {
                    if (pspd.parlorInfoDef[i] != null && pspd.parlorInfoDef[i].iParlorId > 0)
                    {
                        parlorCount++;
                        Parlor[i].SetActive(true);
                        Parlor[i].transform.GetChild(1).GetComponent<Text>().text = pspd.parlorInfoDef[i].szParlorName;
                    }
                    else
                    {
                        Parlor[i].SetActive(false);
                    }
                }

                if (parlorCount == 0)
                {
                    NoneParlor.SetActive(true);
                }
                else
                {
                    NoneParlor.SetActive(false);
                }


                ParlorCount[0].text = pnd.iLeaveParlorAcc + "次";

                if (pnd.iMyParlorId > 0)
                {
                    ParlorCount[1].text = (pnd.iCreatParlorAcc + 1) + "次";
                }
                else
                {
                    ParlorCount[1].text = (pnd.iCreatParlorAcc) + "次";
                }
                ParlorCount[2].text = pnd.iKickParlorAcc + "次";
                ParlorCount[3].text = pnd.iDismissParlorAcc + "次";

            }
            else
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 打开设置面板
        /// </summary>
        public void BtnSettingOpen()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);

            Messenger_anhui.Broadcast(MESSAGE_BTNSETTING);
        }

        /// <summary>
        /// 关闭玩家信息面板的关闭按钮
        /// </summary>
        public void BtnClose()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_BTNCLOSE);
        }

        /// <summary>
        /// 点击设置面板的关闭按钮
        /// </summary>
        public void BtnSettingClose()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_BTNSETTINGCLOSE);
        }


        /// <summary>
        /// 改变音量大小
        /// </summary>
        public void BtnMusicVolume(Slider sd)
        {
            Messenger_anhui<float>.Broadcast(MESSAGE_MUSICVOLUME, sd.value);
        }

        /// <summary>
        /// 改变音效的大小
        /// </summary>
        public void BtnMusicEffectVolume(Slider sd)
        {
            Messenger_anhui<float>.Broadcast(MESSAGE_MUSICEFFECTVOLUME, sd.value);
        }

        /// <summary>
        /// 音乐静音
        /// </summary>
        public void BtnIsMusicMute(Slider obj1)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            //开启
            if (_btnFlagM[0].gameObject.activeInHierarchy)//off按钮显示  
            {
                obj1.enabled = true;
                _btnFlagM[0].gameObject.SetActive(false);//off
                _btnFlagM[1].gameObject.SetActive(true);//on
                obj1.value = MahjongCommonMethod.Instance.LastMusicVolume * 0.01f;
            }
            //静音
            else
            {

                _btnFlagM[0].gameObject.SetActive(true);
                _btnFlagM[1].gameObject.SetActive(false);
                MahjongCommonMethod.Instance.LastMusicVolume = (int)(obj1.value * 100);
                obj1.value = 0;
                obj1.enabled = false;
            }
            Messenger_anhui.Broadcast(MESSAGE_SETMUSIC);
        }
        /// <summary>
        /// 音效静音
        /// </summary>
        public void BtnIsEffecMute(Slider obj1)
        {
            if (_btnFlagE[0].gameObject.activeInHierarchy)
            //开启
            {
                obj1.enabled = true;
                _btnFlagE[0].gameObject.SetActive(false);
                _btnFlagE[1].gameObject.SetActive(true);
                obj1.value = MahjongCommonMethod.Instance.LastEffectValume * 0.01f;
            }
            else//静音
            {
                _btnFlagE[0].gameObject.SetActive(true);
                _btnFlagE[1].gameObject.SetActive(false);
                MahjongCommonMethod.Instance.LastEffectValume = (int)(obj1.value * 100);
                obj1.value = 0;
                obj1.enabled = false;
            }
            Messenger_anhui.Broadcast(MESSAGE_SETMUSICEFFECT);
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
        }

        /// <summary>
        /// 点击返回大厅
        /// </summary>
        public void BtnReturnLobby()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_BTNSETTINGCLOSE);
            Messenger_anhui.Broadcast(MESSAGE_BTNCLOSE);
        }

        /// <summary>
        /// 点击退出登录
        /// </summary>
        public void ExitGame()
        {
            MahjongCommonMethod.isLoginOut = true;
            MahjongCommonMethod.isGameToLobby = false;
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);

            Messenger_anhui.Broadcast(MESSAGE_LOGOUT);
            Messenger_anhui.Broadcast(MESSAGE_BACKTOLOGINPANEL);

        }

        /// <summary>
        /// 点击复制按钮
        /// </summary>
        public void BtnCopy(Text text)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            MahjongCommonMethod.Instance.CopyString(text.text);
            //处理玩家复制成功之后提示文字
            MahjongCommonMethod.Instance.ShowRemindFrame("复制成功", false);
        }

        public void LongPress()
        {
            Debug.LogError("长按该按钮");
        }


        public void LevelPress()
        {
            Debug.LogError("离开按钮");
        }
        public void PlayClickVoice()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
        }
    }
}
