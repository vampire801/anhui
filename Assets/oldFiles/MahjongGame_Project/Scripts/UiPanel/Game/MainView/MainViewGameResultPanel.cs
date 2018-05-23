using UnityEngine;
using System.Collections;
using MahjongGame_AH.Data;
using UnityEngine.UI;
using MahjongGame_AH.GameSystem.SubSystem;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewGameResultPanel : MonoBehaviour
    {
        //一局结束的结算的父物体
        public GameObject RoundGameReult;
        //一圈结束的结算的父物体
        public GameObject AllGameResult;
        public GameObject[] ShowContent;
        
        public GameObject[] GameBtn;  //结算界面的按钮，0表示一局的按钮，1表示游戏结束结算的按钮
        public GameObject[] Title; //一级弹框背景

        //一局结束结算的按钮
        public const string MESSAGER_NEXTROUND_ROUND = "MainViewGameResultPanel.MESSAGER_NEXTROUND_ROUND";  //点击继续下一局的按钮
        public const string MESSAGE_SHARE_ROUND = "MainViewGameResultPanel.MESSAGE_SHARE_ROUND";  //分享按钮
        //房间游戏结束的按钮
        public const string MESSAGER_NEXTROUND_ROOM = "MainViewGameResultPanel.MESSAGER_NEXTROUND_ROOM";  //点击继续下一局的按钮
        public const string MESSAGE_SHARE_ROOM = "MainViewGameResultPanel.MESSAGE_SHARE_ROOM";  //分享按钮
        public GameObject[] _btnShare;
        public GameObject[] _btnRround;
        public GameObject[] _btnRoom;
        //分享按钮
        public GameObject RPButton;
        void Start()
        {
            transform.GetComponent<Canvas>().worldCamera = Camera.main;
            RPButton.SetActive(false);
        }


        /// <summary>
        /// 更新面板数据
        /// </summary>
        public void UpdateShow()
        {
            GameResultPanelData grpd = GameData.Instance.GameResultPanelData;
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            if (grpd.isPanelShow)
            {
                gameObject.SetActive(true);
                if (MahjongLobby_AH.SDKManager.Instance.IOSCheckStaus == 1)
                {
#if UNITY_IOS || UNITY_IPONE
                    for (int i = 0; i < _btnShare.Length ; i++)
                    {
                        _btnShare[i].SetActive(false);
                    }
                    
#endif
                }
                else
                {
                    for (int i = 0; i < _btnShare.Length; i++)
                    {
                        _btnShare[i].SetActive(true);
                    }

                }
                //修改确定按钮的图片
                //if(grpd.HandDissolve==0)
                //{
                //    okImage.sprite = ok_sprite[0];
                //}
                //else
                //{
                //    okImage.sprite = ok_sprite[1];
                //}


                //产生一局结算的结果
                if (grpd.isShowRoundGameResult)
                {
                    RoundGameReult.SetActive(true);
                    GameBtn[0].SetActive(true);
                }
                else
                {
                    RoundGameReult.SetActive(false);
                    GameBtn[0].SetActive(false);
                }

                //产生房间游戏结算的结果
                if (grpd.isShowRoomGameResult)
                {
                    AllGameResult.SetActive(true);
                    Title[0].SetActive(false);
                    Title[1].SetActive(false);
                    GameBtn[1].SetActive(true);
                    RPButton.SetActive(true);
                }
                else
                {
                    Title[0].SetActive(true);
                    Title[1].SetActive(true);
                    AllGameResult.SetActive(false);
                    GameBtn[1].SetActive(false);
                    RPButton.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 产生四家的结算的预置体
        /// </summary>
        public void SpwanGameReult_Round()
        {
            GameResultPanelData grpd = GameData.Instance.GameResultPanelData;
            //在每次产生之前先删除之前的预置体
            GameResultPrefab[] prefab = RoundGameReult.transform.GetComponentsInChildren<GameResultPrefab>();
            if (prefab.Length > 0)
            {
                for (int i = 0; i < prefab.Length; i++)
                {
                    Destroy(prefab[i].gameObject);
                }
            }

            for (int i = 1; i < 5; i++)
            {
                GameObject go = null;
                //通过座位号判断玩家信息是否是赢家
                if (grpd.byaWinSrat[i - 1] > 0)
                {
                    go = Instantiate(Resources.Load<GameObject>("Game/GameResult/RoundPlayerResult_Win"));
                    go.transform.SetParent(RoundGameReult.transform);
                    go.transform.SetAsFirstSibling();
                }
                else
                {
                    go = Instantiate(Resources.Load<GameObject>("Game/GameResult/RoundPlayerResult_Normal"));
                    go.transform.SetParent(RoundGameReult.transform);
                }
                go.transform.localScale = Vector3.one;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                GameResultPrefab gameRes = go.GetComponent<GameResultPrefab>();
                gameRes.iseatNum = i;
                gameRes.MessageVlaue();
                gameRes.SpwanPlayerCard();
            }

            bool isHuangZhuang = true;
            //如果本次是荒庄，产生荒庄的图标
            for (int i = 0; i < 4; i++)
            {
                if (grpd.byaWinSrat[i] > 0)
                {
                    isHuangZhuang = false;
                }
            }

            //在这里处理如果玩家处于托管状态，30s后自动准备            
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            pppd.isAllAutoStatus = true; //是否是四个人的托管状态
            for (int i = 0; i < pppd.iAllPlayerHostStatus.Length; i++)
            {
                if (pppd.iAllPlayerHostStatus[i] == 0)
                {
                    pppd.isAllAutoStatus = false;
                    break;
                }
            }

            if (!pppd.isAllAutoStatus)
            {
                AutoReady();
            }

            if (isHuangZhuang)
            {
                UIMainView.Instance.PlayerPlayingPanel.SpwanSpeaiclTypeRemind(4, 9, false);
            }


        }


        /// <summary>
        /// 产生一圈比赛结算的预置体
        /// </summary>
        public void SpwanAllGameResult()
        {
            //在每次产生之前先删除之前的预置体
            GameResultPrefab[] prefab = AllGameResult.transform.GetComponentsInChildren<GameResultPrefab>();
            if (prefab.Length > 0)
            {
                for (int i = 0; i < prefab.Length; i++)
                {
                    Destroy(prefab[i].gameObject);
                }
            }

            for (int i = 1; i < 5; i++)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Game/GameResult/AllGameReult"));
                go.transform.SetParent(AllGameResult.transform);
                go.transform.SetSiblingIndex(i);
                go.transform.localScale = Vector3.one;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                GameResultPrefab gameRes = go.GetComponent<GameResultPrefab>();
                gameRes.iseatNum = i;
                gameRes.UpdatePlayerMessage();
            }
            if (GameData.Instance.GameResultPanelData.isWinner)
            {
                SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Win, false, false);
            }
            else
            {
                SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Lose, false, false);

            }

        }

        //如果处于托管状态，30s后自动处于准备状态
        void AutoReady()
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            if (pppd.iPlayerHostStatus > 0)
            {
                //初始化面板           
                UIMainView.Instance.PlayerPlayingPanel.InitPanel();
                StartCoroutine(ThirSecondDelay());
            }
        }

        //延迟30s
        IEnumerator ThirSecondDelay()
        {
            yield return new WaitForSeconds(30f);
            BtnNext(2);
        }

        /// <summary>
        /// 点击继续按钮
        /// </summary>
        /// <param name="status">1表示手动点击状态，2表示自动点击</param>
        public void BtnNext(int status)
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            Messenger_anhui<int>.Broadcast(MESSAGER_NEXTROUND_ROUND, status);
            PlayerPrefs.SetInt("TingOneCard", 0);
        }

        /// <summary>
        /// 比赛总结算的确定按钮
        /// </summary>
        public void BtnoK_End()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            Messenger_anhui.Broadcast(MESSAGER_NEXTROUND_ROOM);
            PlayerPrefs.SetString(MahjongLobby_AH.LobbyContants.SetSeatIDAgo, "0000");
            PlayerPrefs.SetInt("GameActivie", 0);
            PlayerPrefs.SetInt("TingOneCard", 0);
        }
        bool down1;
        bool down2;

        public RawImage _rawImage;
        /// <summary>
        /// 单局点击分享按钮
        /// </summary>
        public void BtnShare()
        {
            if (!down1)
            {
                down1 = true;
                Invoke("deleat1", 2f);
                GameData.Instance.m_active = 0;
                PlayerPrefs.SetInt("GameActivie", 0);
                SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
                Messenger_anhui< GameObject[]>.Broadcast(MESSAGE_SHARE_ROUND,  _btnRround);
            }
        }

        /// <summary>
        /// 分享得红包
        /// </summary>
        public void BtnShareRP()
        {
            if (!down2)
            {
                down2 = true;
                Invoke("deleat2", 2);
                PlayerPrefs.SetInt("GameActivie", GameData.Instance.m_active);
                SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
                ShowContent[2].gameObject.SetActive(false);
                //Messenger<RawImage, GameObject[]>.Broadcast(MESSAGE_SHARE_ROOM, _rawImage, _btnRoom);
                MahjongLobby_AH.SDKManager.Instance.BtnShare( 1, GameData.Instance.m_active,"");
            }
        }

        void deleat1()
        {
            down1 = false;
        }
        void deleat2()
        {
            down2 = false;
        }
        /// <summary>
        /// 比赛总结算的分享按钮
        /// </summary>
        public void BtnShare_End()
        {
            if (!down2)
            {
                down2 = true;
                Invoke("deleat2", 2);
                PlayerPrefs.SetInt("GameActivie", 0);
                ShowContent[0].transform.GetChild(0).GetComponent<Text>().text = anhui.MahjongCommonMethod.Instance.RoomId;
                ShowContent[1].transform.GetChild(0).GetComponent<Text>().text = System.DateTime.Now.ToString("yyyy_MM_dd HH:mm");
                ShowContent[2].gameObject.SetActive(true);
                SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
                Messenger_anhui< GameObject[]>.Broadcast(MESSAGE_SHARE_ROOM,  _btnRoom);
            }
        }

    }

}
