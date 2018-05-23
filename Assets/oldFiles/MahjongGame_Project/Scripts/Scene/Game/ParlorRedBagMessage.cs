using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Text;
using System;
using MahjongGame_AH.Network.Message;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class ParlorRedBagMessage : MonoBehaviour
    {
        public GameObject Bulletin; //公告
        public Image RobRedBagBtn; //抢红包按钮  只显示倒计时，倒计时结束直接显示抢红包面板 
        public GameObject RobPanel; //抢红包面板 
        public Image RobStatus; //显示抢 或者抢光了
        public Button RPMessage; //查看红包详细信息
        public Button Ok; //关闭红包界面
        public Text ShowUnOpenMessage; //显示未达到开启条件
        public Sprite[] RobStatus_image; //抢红包状态的显示信息 0表示抢 1表示抢光了 
        public GameObject RbMessageParent; //麻将馆红包详细信息的父物体
        public GameObject NoRbRecord; //没有红包记录信息  
        public GameObject RbMessagePanel; //红包详细信息面板
        public Text RedBagDownTimer; //红包倒计时        
        public int LeftRobRedBagTimer; //剩余开始抢红包的时间，用于显示30s倒计时   
        [HideInInspector]
        public MahjongLobby_AH.Data.ParlorShowPanelData.ParlorRedBag_ redBagMessage = new MahjongLobby_AH.Data.ParlorShowPanelData.ParlorRedBag_();
        Vector3 InitPos_Bul = Vector3.zero;
        [HideInInspector]
        public int status; ////0表示没有红包  1表示有红包且未开始抢  2表示有红包已经开始抢 3表示开启条件不足 4表示红包已抢完

        public int ReqStatus; //请求状态 0表示正常 1表示抢光了之后自己的状态


        void Start()
        {
            InitPos_Bul = new Vector3(Screen.width / 2 + 500f, Screen.height / 4, 0);
        }

        //void OnGUI()
        //{
        //    if (GUI.Button(new Rect(10, 10, 100, 40), "1111"))
        //    {
        //        NetMsg.ClientOpenRp17Res msg = new NetMsg.ClientOpenRp17Res();
        //        msg.srp17Info.iAssetNum = 100;
        //        msg.srp17Info.byAssetType = 1;
        //        UIMainView.Instance.ParlorRedBagMessage.GetAwardShow(msg.srp17Info);
        //    }
        //}



        /// <summary>
        /// 移动公告
        /// </summary>
        public void MoveBulletin()
        {
            Bulletin.transform.GetChild(0).GetComponent<Text>().text = redBagMessage.context;
            //开始移动            
            float width = Bulletin.transform.GetChild(0).GetComponent<Text>().preferredWidth + Screen.width + 50;
            float x = -InitPos_Bul.x - Bulletin.transform.GetChild(0).GetComponent<Text>().preferredWidth;
            float speed = 150f;
            float timer = width / speed;
            Bulletin.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Bulletin.transform.GetChild(0).GetComponent<Text>().preferredWidth + 140f);

            Tweener tweener = Bulletin.transform.DOLocalMoveX(x, timer);
            tweener.SetEase(Ease.Linear);
            if ((int)anhui.MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(System.DateTime.Now) < redBagMessage.endTim - redBagMessage.interval)
            {
                Bulletin.transform.localPosition = InitPos_Bul + new Vector3(Bulletin.transform.GetChild(0).GetComponent<Text>().preferredWidth, 0, 0);
                if (gameObject.activeInHierarchy)
                {
                    tweener.OnComplete(() => StartCoroutine(InterTimerMoveBulletin(redBagMessage.interval)));
                }
            }
        }

        IEnumerator InterTimerMoveBulletin(float timer)
        {
            yield return new WaitForSeconds(timer);
            MoveBulletin();
        }



        /// <summary>
        /// 更新抢红包的界面信息
        /// </summary>
        /// <param name="status_">0表示隐藏红包按钮 1表示关闭倒计时打开抢字</param>
        public void UpdateRob(int status_)
        {
            if (status_ == 0)
            {
                gameObject.SetActive(false);
                if (temp != null)
                {
                    StopCoroutine(temp);
                }
            }
            else
            {
                status = 2;
                //打开抢的面板
                UpdateRobStatus(1);
                //关闭红包倒计时按钮
                RobRedBagBtn.gameObject.SetActive(false);
                if (temp != null)
                {
                    StopCoroutine(temp);
                }
            }
        }

        public int BeginTimer = 0;
        /// <summary>
        /// 更新红包界面
        /// </summary>
        /// <param name="status">//0表示没有红包  1表示有红包且未开始抢  2表示有红包已经开始抢 3表示开启条件不足 4表示红包已抢完</param>
        public void UpdateShow(int status_, MahjongLobby_AH.Data.ParlorShowPanelData.ParlorRedBag_ info)
        {
            InitPos_Bul = new Vector3(Screen.width / 2 + 500f, Screen.height / 4, 0);
            if (anhui.MahjongCommonMethod.Instance.isGetReaBag)
            {
                return;
            }
            redBagMessage = info;
            gameObject.SetActive(true);
            status = status_;
            if (status == 0)
            {
                gameObject.SetActive(false);
            }
            else if (status == 1)
            {
                MoveBulletin();
                BeginTimer = info.beginTim;
                anhui.MahjongCommonMethod.Instance.GetNowTimer(0, GetNowTimer);
                RobRedBagBtn.gameObject.SetActive(false);
                UpdateRobStatus(0);
                ShowDowntimer();
            }
            else if (status == 2)
            {
                MoveBulletin();
                RobRedBagBtn.gameObject.SetActive(false);
                UpdateRobStatus(1);
            }
            else if (status == 4)
            {
                MoveBulletin();
                RobRedBagBtn.gameObject.SetActive(false);
                UpdateRobStatus(2);
            }
            //else if (status == 3)
            //{
            //    MoveBulletin();
            //    RobRedBagBtn.gameObject.SetActive(false);
            //    UpdateRobStatus(3);
            //}
        }


        /// <summary>
        /// 获取时间
        /// </summary>
        /// <returns></returns>
        public void GetNowTimer(int id, int timer)
        {
            LeftRobRedBagTimer = BeginTimer - timer;
        }


        /// <summary>
        /// 更新面板
        /// </summary>
        /// <param name="msg"></param>
        public void GetAwardShow(NetMsg.ClientSrp17Info msg)
        {
            if (temp != null)
            {
                StopCoroutine(temp);
            }

            //抢光了，显示红包详细信息
            if (msg == null)
            {
                UpdateRobStatus(2);
            }
            //抢到红包，播放红包领取动画
            else
            {
                UpdateRobStatus(0);
                UIMgr.GetInstance().ShowRedPagePanel.OnSetValueAndNotOpenPanel(16, 1, 2, null, RedPageShowPanel.NowState.Game);
                int RpType = 0;
                switch (msg.byAssetType)
                {
                    case 1:
                        {
                            RpType = 3;
                        }
                        break;
                    case 2:
                        {
                            RpType = 4;
                        }
                        break;
                    case 3:
                        {
                            RpType = 1;
                        }
                        break;
                    case 4:
                        {
                            RpType = 2;
                        }
                        break;
                    case 5:
                        {
                            RpType = 5;
                        }
                        break;
                    case 6:
                        {
                            RpType = 0;
                        }
                        break;
                }

                Debug.LogError("RpType:" + RpType);
                UIMgr.GetInstance().ShowRedPagePanel.DirectOpenRedPagePanel(msg.iAssetNum.ToString(), RpType, "麻将馆福利，抢到就是赚到！");
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="status_rb">0关闭面板 1表示显示抢  2表示显示抢光了，查看详情  3表示未达到开启条件，确定按钮</param>
        public void UpdateRobStatus(int status_rb)
        {
            if (status_rb == 0)
            {
                RobPanel.SetActive(false);
                return;
            }
            RobPanel.SetActive(true);
            RobStatus.gameObject.SetActive(true);
            if (status_rb == 1)
            {
                RobStatus.sprite = RobStatus_image[0];
                RPMessage.gameObject.SetActive(false);
                Ok.gameObject.SetActive(false);
                ShowUnOpenMessage.gameObject.SetActive(false);
            }
            else if (status_rb == 2)
            {
                RobStatus.sprite = RobStatus_image[1];
                RPMessage.gameObject.SetActive(true);
                Ok.gameObject.SetActive(false);
                ShowUnOpenMessage.gameObject.SetActive(false);
            }
            else if (status_rb == 3)
            {
                RobStatus.gameObject.SetActive(false);
                RPMessage.gameObject.SetActive(false);
                Ok.gameObject.SetActive(true);
                ShowUnOpenMessage.text = "由于牌桌人数未达到条件，无法领取";
            }
            RobStatus.SetNativeSize();
        }

        IEnumerator temp;

        /// <summary>
        /// 显示倒计时
        /// </summary>
        void ShowDowntimer()
        {
            temp = OneSecond();
            StartCoroutine(temp);

            if (LeftRobRedBagTimer > 0 && LeftRobRedBagTimer <= 30)
            {
                status = 1;
                RobRedBagBtn.gameObject.SetActive(true);
                RobRedBagBtn.transform.GetChild(0).gameObject.SetActive(true);
                RobRedBagBtn.transform.GetChild(1).gameObject.SetActive(true);
                RobRedBagBtn.transform.GetChild(2).gameObject.SetActive(false);
            }

            if (LeftRobRedBagTimer <= 0)
            {
                if (temp != null)
                {
                    StopCoroutine(OneSecond());
                }
                //RedBagDownTimer.text = "等待服务器统计在线人数";
                status = 2;
                RobRedBagBtn.transform.GetChild(0).gameObject.SetActive(false);
                RobRedBagBtn.transform.GetChild(1).gameObject.SetActive(false);
                RobRedBagBtn.transform.GetChild(2).gameObject.SetActive(true);
            }
        }


        IEnumerator OneSecond()
        {
            yield return new WaitForSeconds(1f);
            LeftRobRedBagTimer--;
            //显示倒计时
            RedBagDownTimer.text = LeftRobRedBagTimer.ToString("00");
            ShowDowntimer();
        }

        /// <summary>
        /// 点击抢红包按钮
        /// </summary>
        public void BtnRobRedBag()
        {
            Debug.LogError("status:" + status);

            if (status == 2)
            {
                NetMsg.ClientOpenRp17Req msg = new NetMsg.ClientOpenRp17Req();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg.iRp17Id = redBagMessage.rpId;
                Network.NetworkMgr.Instance.GameServer.SendClientOpenRp17Req(msg);
            }
        }


        /// <summary>
        /// 处理分享到微信消息的按钮事件
        /// </summary>
        /// </summary>
        /// <param name="type"> type=0 分享朋友 1->朋友圈 2->收藏</param>
        public void HandleShareWX()
        {
            //MahjongLobby_AH.SDKManager.Instance.iWitchShare = 0;

            //MahjongLobby_AH.SDKManager.Instance.BtnShare( 1, 10,0, 0);
            //gameObject.SetActive(false);
        }

        /// <summary>
        /// 获得红包的详细信息
        /// </summary>
        public void BtnGetRbMessage()
        {
            UpdateRobStatus(0);
            FromWebGetRedBag17();
        }


        public int iRedId; //麻将馆红包id
        /// <summary>
        /// 获取玩家当前红包状态
        /// </summary>
        /// <param name="iparlorId"></param>
        /// <param name="timer"></param>
        public void GetParlorRedBagStatus(int iparlorId, int timer)
        {
            anhui.MahjongCommonMethod mcm = anhui.MahjongCommonMethod.Instance;
            int nowTimer = timer;
            //处理玩家从某个麻将馆进入游戏，检查该玩家是否有红包信息
            if (iparlorId != 0)
            {
                for (int i = 0; i < mcm.parlorRedBagInfo.Length; i++)
                {
                    //找到指定的麻将馆
                    if (mcm.parlorRedBagInfo[i].parlorId == iparlorId)
                    {
                        iRedId = mcm.parlorRedBagInfo[i].rpId;

                        int status = 0;  //0表示没有红包  1表示有红包且未开始抢  2表示有红包已经开始抢 3表示开启条件不足 4表示红包已抢完
                        if (nowTimer < mcm.parlorRedBagInfo[i].endTim)
                        {
                            //红包还未结束
                            if (mcm.parlorRedBagInfo[i].state == 0)
                            {
                                if (nowTimer < mcm.parlorRedBagInfo[i].beginTim)
                                {
                                    status = 1;
                                }
                                else
                                {
                                    status = 2;
                                }
                            }
                            //红包不达开启条件
                            else
                            {
                                status = 0;
                            }
                            ////红包已经抢完
                            //else if (mcm.parlorRedBagInfo[i].state == 1)
                            //{
                            //    status = 0;
                            //}
                        }
                        else
                        {
                            status = 0;
                        }

                        Debug.LogError("status:" + status);

                        UpdateShow(status, mcm.parlorRedBagInfo[i]);
                        break;
                    }
                }
            }
        }

        #region 获取某个麻将馆红包的详细信息
        public List<MahjongLobby_AH.Data.ParlorShowPanelData.ParlorRedBagMessage> SmallRedBagMessage =
            new List<MahjongLobby_AH.Data.ParlorShowPanelData.ParlorRedBagMessage>();
        public int SmallRedBagCount; //小红包的总数       

        public void FromWebGetRedBag17()
        {
            string url = " ";
            if (MahjongLobby_AH.SDKManager.Instance.IOSCheckStaus == 1)
            {
                url = MahjongLobby_AH.LobbyContants.MAJONG_PORT_URL_T + "rp17.x";
            }
            else
            {
                url = MahjongLobby_AH.LobbyContants.MAJONG_PORT_URL + "rp17.x";
            }

            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("rpId", iRedId.ToString());
            anhui.MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, GetRedBag17Data, "12", 5);
        }

        /// <summary>
        /// 获取指定麻将馆红包的领取信息
        /// </summary>
        /// <param name="json"></param>
        /// <param name="status"></param>
        public void GetRedBag17Data(string json, int status)
        {
            MahjongLobby_AH.Data.ParlorShowPanelData.ParlorRedBagMessageData parlor =
                new MahjongLobby_AH.Data.ParlorShowPanelData.ParlorRedBagMessageData();
            parlor = JsonUtility.FromJson<MahjongLobby_AH.Data.ParlorShowPanelData.ParlorRedBagMessageData>(json);
            //产生麻将馆红包详细信息预置体
            RbMessagePanel.SetActive(true);
            if (parlor.status != 1 || parlor.data.Length == 0)
            {
                RbMessageParent.SetActive(false);
                NoRbRecord.SetActive(true);
                return;
            }

            RbMessageParent.SetActive(true);
            NoRbRecord.SetActive(false);
            SmallRedBagCount = parlor.srpNum;

            for (int i = 0; i < parlor.data.Length; i++)
            {
                SmallRedBagMessage.Add(parlor.data[i]);
            }

            //产生预置体
            SpwanSmallRedBag();
        }

        InfinityGridLayoutGroup infinityGridLayoutGroup;

        /// <summary>
        /// 产生所有小红包
        /// </summary>
        public void SpwanSmallRedBag()
        {
            MahjongLobby_AH.ParlorSmallRedBag[] parlor = RbMessageParent.GetComponentsInChildren<MahjongLobby_AH.ParlorSmallRedBag>();
            int count = 0;  //红包数量
            count = (SmallRedBagMessage.Count > 6) ? 6 : SmallRedBagMessage.Count;
            int count_ = count > parlor.Length ? count : parlor.Length;

            for (int i = 0; i < count_; i++)
            {
                MahjongLobby_AH.ParlorSmallRedBag temp = null;
                if (i > parlor.Length - 1)
                {
                    GameObject go = Instantiate(parlor[0].gameObject);
                    go.transform.SetParent(RbMessageParent.transform);
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.localScale = Vector3.one;
                    go.transform.localEulerAngles = Vector3.zero;
                    temp = go.GetComponent<MahjongLobby_AH.ParlorSmallRedBag>();
                    //更新界面
                    temp.UpdateShow(SmallRedBagMessage[i]);
                }
                else
                {
                    if (i > SmallRedBagMessage.Count - 1)
                    {
                        parlor[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        parlor[i].gameObject.SetActive(true);
                        temp = parlor[i];
                        //更新界面
                        temp.UpdateShow(SmallRedBagMessage[i]);
                    }
                }
            }
            RbMessageParent.GetComponent<ContentSizeFitter>().enabled = true;
            RbMessageParent.GetComponent<GridLayoutGroup>().enabled = true;

            if (SmallRedBagMessage.Count > 6)
            {
                infinityGridLayoutGroup = RbMessageParent.GetComponent<InfinityGridLayoutGroup>();
                infinityGridLayoutGroup.RemoveListener_Rect();
                infinityGridLayoutGroup.Init();
                infinityGridLayoutGroup.SetAmount(SmallRedBagMessage.Count);
                infinityGridLayoutGroup.updateChildrenCallback = UpdateParlorSamllRedBag;
            }
        }

        void UpdateParlorSamllRedBag(int index, Transform trans)
        {
            if (trans != null)
            {
                trans.GetComponent<MahjongLobby_AH.ParlorSmallRedBag>().UpdateShow(SmallRedBagMessage[index]);
            }
        }
        #endregion

    }
}
