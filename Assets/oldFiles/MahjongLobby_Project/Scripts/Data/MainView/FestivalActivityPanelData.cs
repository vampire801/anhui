using UnityEngine;
using System.Collections;
using MahjongLobby_AH.LobbySystem.SubSystem;
using UnityEngine.UI;
using DG.Tweening;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.Network;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.InteropServices;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class FestivalActivityPanelData : MonoBehaviour
    {
        public const string MESSAGE_LUCKNUM = "FestivalActivityPanelData.MESSAGE_LUCKNUM";
        public const string MESSAGE_HISTORY = "FestivalActivityPanelData.MESSAGE_HISTORY";

        //主面板
        public GameObject m_ligthYellow;//灯光闪烁
        public Image m_imTurntable;//转盘     图片  
        public Text m_txLotteryNumber;//抽奖次数
        public Image m_imFourPages;//四个叶片
        public GameObject m_gOverheadLantern;//中将彩灯
        public GameObject m_gFenXiangOpenBtn;//分享按钮可点
        public GameObject m_gFenXiangClose;//关闭按钮

        //中奖面板
        public GameObject m_gWinningPanel;//中奖面板
        public Text m_txEarnedAwards;//奖品数量
        public Image m_imWinPicture;//中奖图片
        public Text m_txWinPictureJiFen;//是积分的时候打开一句话
        public Sprite m_imJiFen;//积分
        public Sprite m_imJinBi;//金币

        //记录面板
        public GameObject m_gHistory;//领取的历史记录界面
        public WinningRecord m_WinRecode;//预制体
        public GameObject m_gPrefabParent;//预制体的父级




        Tween t1, t2, t3, t4;//转盘的载体
        int shangyicijishu = 0;//上一次转盘所剩的格子数
        bool hasDownLoadQRcode = false;//为了不频繁的点击使用
        float angleData = 1.5f;//旋转时间
        int SaveHistory = 0;//有多少个历史数据
        List<GameObject> m_lDestHistory = new List<GameObject>();//每条历史数据的保存
        bool m_bDrawLottery = false;//转盘开奖中
        bool m_bZuanzhuan = false;//旋转的四个叶片的
        int m_iLotteryNumber = 0;//可抽奖次数
        [HideInInspector]
        public int m_iWin;//中奖的位置


        //保存大厅公告
        public List<AvtivityHistoreContent> AvtivityHistore_ = new List<AvtivityHistoreContent>();

        public class AvtivityHistoreContent
        {
            public string tim;  //日志发生的时间戳
            public string awardType;  //奖励类型：0无，1绑定金币，2金币，3积
            public string awardNum;  //奖励数量
        }

        public class AvtivityHistore
        {
            public int status;  //1成功  0参数错误  9系统错误
            public List<AvtivityHistoreContent> data;
        }

        public class FestivalAvtivityHistoreContent
        {
            public List<AvtivityHistore> avtivityHistoreContent = new List<AvtivityHistore>();
        }


        void Start()
        {
            m_gOverheadLantern.SetActive(false);

            StartCoroutine(DownLoadWWW(LobbyContants.FestivalShareActivityUrl));
        }

        void Update()
        {
            //灯闪烁
            DoLightShader();
        }

        void OnEnable()
        {
            m_txLotteryNumber.text = m_iLotteryNumber + "次抽奖机会";
        }

        /// <summary>
        /// 分享按钮是否可点
        /// </summary>
        /// <param name="isfenxiangstate"> true 是可以点击分享按钮 </param>
        public void OnShowFenXiangBtn(bool isfenxiangstate)
        {
            if (isfenxiangstate)
            {
                m_gFenXiangOpenBtn.SetActive(true);
                m_gFenXiangClose.SetActive(false);
            }
            else
            {
                m_gFenXiangOpenBtn.SetActive(false);
                m_gFenXiangClose.SetActive(true);
            }
        }

        /// <summary>
        /// 显示可抽奖次数
        /// </summary>
        public void OnShowLotteryNumber(int num)
        {
            Debug.LogError("----------显示可抽奖次数------------" + num);
            m_iLotteryNumber += num;
            m_txLotteryNumber.text = m_iLotteryNumber + "次抽奖机会";// string.Format("{0}次抽奖机会" + m_iLotteryNumber);
        }


        float m_fLightShaderTimer = 0;
        float m_fLightShaderTimerOver = 0.4f;
        //闪烁灯光的代码
        private void DoLightShader()
        {
            while (m_fLightShaderTimer >= m_fLightShaderTimerOver)
            {
                m_ligthYellow.SetActive(!m_ligthYellow.activeSelf);
                m_fLightShaderTimer -= m_fLightShaderTimerOver;
            }
            m_fLightShaderTimer += Time.deltaTime;
        }

        /// <summary>
        /// 转盘按钮
        /// </summary>
        public void OnBtn_Turntable()
        {
            if (m_iLotteryNumber <= 0)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("抽奖次数不足");
            }

            if (m_bDrawLottery/*如果在旋转中*/ || m_iLotteryNumber <= 0/*抽奖次数没有了*/) return;

            Debug.LogError("转盘按钮 调用" + m_iLotteryNumber);
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            NetMsg.ClientLotteryReqDef msg = new NetMsg.ClientLotteryReqDef();
            msg.iUserId = anhui.MahjongCommonMethod.Instance.iUserid;
            NetworkMgr.Instance.LobbyServer.SendLotteryReq(msg);
        }

        /// <summary>
        /// 旋转
        /// </summary>
        public void UpdataHistoryShow()
        {
            Debug.LogError("抽奖次数 = " + m_iLotteryNumber + ",m_iWin = " + m_iWin);

            if (m_iWin < 0)
            {
                return;
            }

            if (m_iLotteryNumber > 0/*抽奖次数没有了*/)
            {
                int qunashu = m_iWin;

                switch (qunashu)
                {
                    case 0: { qunashu = m_iWin + 10; angleData = 1.9f; } break;
                    case 1: { qunashu = m_iWin + 10; angleData = 3.5f; } break;
                    case 2: { qunashu = m_iWin + 10; angleData = 3.5f; } break;
                    case 3: { qunashu = m_iWin + 10; angleData = 3.5f; } break;
                    case 4: { qunashu = m_iWin; angleData = 1.5f; } break;
                    case 5: { qunashu = m_iWin; angleData = 1.5f; } break;
                    case 6: { qunashu = m_iWin; angleData = 1.9f; } break;
                    case 7: { qunashu = m_iWin; angleData = 1.9f; } break;
                    case 8: { qunashu = m_iWin; angleData = 1.9f; } break;
                    case 9: { qunashu = m_iWin; angleData = 1.9f; } break;
                }

                int quanshu = 0;
                quanshu = (shangyicijishu + qunashu - 1) * 36;
                if (quanshu < 0)
                {
                    quanshu = 360 + quanshu;
                }
                StartCoroutine(Turntable(3, 0.9f, quanshu));
                shangyicijishu = 10 - (qunashu - 1);

                AvtivityHistore_.Clear();
            }
        }


        /// <summary>
        /// 开始转盘
        /// </summary>
        /// <param name="times">总共转几圈</param>
        /// <param name="time0fOnce">一圈用时</param>
        /// <param name="angle">停止下来的角度</param>
        /// <returns></returns>
        IEnumerator Turntable(int times, float time0fOnce, int angle)
        {
            m_bDrawLottery = true;
            m_bZuanzhuan = true;
            OnShowLotteryNumber(-1);

            t1 = m_imTurntable.transform.DOLocalRotate(new Vector3(0, 0, 360), time0fOnce, (RotateMode)3);
            t1.SetEase(Ease.Linear);
            t1.SetLoops(times);
            t1.Play();
            t1.OnComplete(() =>
            {
                t2 = m_imTurntable.transform.DOLocalRotate(new Vector3(0, 0, angle), angleData, (RotateMode)3);
                t2.SetLoops(1);
                t2.Play();
            });
            yield return new WaitForSeconds(angleData);
            m_gOverheadLantern.SetActive(true);
            t3 = m_gOverheadLantern.transform.DOLocalRotate(new Vector3(0, 0, 360), time0fOnce, (RotateMode)3);
            t3.SetEase(Ease.Linear);
            t3.SetLoops(times);
            t3.Play();
            t3.OnComplete(() =>
            {
                t4 = m_gOverheadLantern.transform.DOLocalRotate(new Vector3(0, 0, 0), 1f, (RotateMode)3);
                t4.SetLoops(1);
                t4.Play();
                t4.OnComplete(() => { StartCoroutine(OnLaterShowFenxiang()); });
            });
        }

        IEnumerator OnLaterShowFenxiang()
        {
            for (int i = 0; i < 4; i++)
            {
                m_gOverheadLantern.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                m_gOverheadLantern.SetActive(false);
                yield return new WaitForSeconds(0.1f);
            }

            m_bDrawLottery = false;
            OnShowWinningPanel();
            m_gOverheadLantern.SetActive(false);
        }

        /// <summary>
        /// 分享按钮 分享文字
        /// </summary>
        public void OnBtn_Share()
        {
            if (m_bDrawLottery) return;
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            string url = SDKManager.WXInviteUrl + 0;
            string title = "【双喜麻将】最地道推倒胡玩法与现实中的好友一起组局线上游戏吧！";
            string discritption = "";
            if (GameData.Instance.PlayerNodeDef.userDef.byUserSource == 2)//如果是微信
                SDKManager.Instance.HandleShareWX(url, title, discritption, 1, 20, 0, "");
        }

        /// <summary>
        /// 中奖之后面板出现
        /// </summary>
        /// <param name="WinType">奖品类型</param>
        /// <param name="WinNum">奖品数量</param>
        void OnShowWinningPanel()
        {
            Debug.LogError("抽奖" + m_iWin);
            string win = "";
            m_gWinningPanel.SetActive(true);
            m_txWinPictureJiFen.gameObject.SetActive(false);
            switch (m_iWin)
            {
                case 0:
                    {
                        GameData.Instance.PlayerNodeDef.iCoin += 20;
                        win = "金币x20"; m_imWinPicture.sprite = m_imJinBi;
                    }
                    break;
                case 1:
                    {
                        win = "88积分"; m_imWinPicture.sprite = m_imJiFen;
                        m_txWinPictureJiFen.gameObject.SetActive(true);
                    }
                    break;
                case 2:
                    {
                        GameData.Instance.PlayerNodeDef.iCoin += 50;
                        win = "金币x50"; m_imWinPicture.sprite = m_imJinBi;
                    }
                    break;
                case 3:
                    {
                        GameData.Instance.PlayerNodeDef.iCoin += 80;
                        win = "金币x80"; m_imWinPicture.sprite = m_imJinBi;
                    }
                    break;
                case 4:
                    {
                        GameData.Instance.PlayerNodeDef.iCoin += 100;
                        win = "金币x100"; m_imWinPicture.sprite = m_imJinBi;
                    }
                    break;
                case 5:
                    {
                        win = "100积分"; m_imWinPicture.sprite = m_imJiFen;
                        m_txWinPictureJiFen.gameObject.SetActive(true);
                    }
                    break;
                case 6:
                    {
                        GameData.Instance.PlayerNodeDef.iCoin += 300;
                        win = "金币x300"; m_imWinPicture.sprite = m_imJinBi;
                    }
                    break;
                case 7:
                    {
                        win = "300积分"; m_imWinPicture.sprite = m_imJiFen;
                        m_txWinPictureJiFen.gameObject.SetActive(true);
                    }
                    break;
                case 8:
                    {
                        win = "888积分"; m_imWinPicture.sprite = m_imJiFen;
                        m_txWinPictureJiFen.gameObject.SetActive(true);
                    }
                    break;
                case 9:
                    {
                        GameData.Instance.PlayerNodeDef.iCoin += 1001;
                        win = "金币x1001"; m_imWinPicture.sprite = m_imJinBi;
                    }
                    break;
            }
            UIMainView.Instance.LobbyPanel.UpdateShow();

            m_txEarnedAwards.text = win;
        }

        public void OnFenXiangLaterSend()
        {
            NetMsg.ClientLotteryCountReqDef msgLottery = new NetMsg.ClientLotteryCountReqDef();
            msgLottery.iUserId = anhui.MahjongCommonMethod.Instance.iUserid;
            msgLottery.byAddLotteryCount = 1;//分享成功只会增加一次抽奖次数，所以是1
            NetworkMgr.Instance.LobbyServer.SendLotteryCountReq(msgLottery);

            OnShowFenXiangBtn(false);//关闭
        }

        /// <summary>
        /// 关闭中奖的界面  在微信分享回来之后关闭
        /// </summary>
        public void OnCloseWinningPanel()
        {
            m_gWinningPanel.SetActive(false);
        }

        WWW m_wGetUrlImage;//网络消息
        int Counter = 0;

        IEnumerator DownLoadWWW(string DownLoadurl)
        {
            m_wGetUrlImage = new WWW(DownLoadurl);
            if (m_wGetUrlImage.error != null)
            {
                Debug.LogError("www.error:" + m_wGetUrlImage.error);//如果没有下载下来处理方法

                if (Counter >= 3)
                {
                    StopCoroutine("DownLoadWWW");
                }
                else
                {
                    StartCoroutine(DownLoadWWW(DownLoadurl));
                }
                Counter++;
            }
            while (!m_wGetUrlImage.isDone)
            {
                yield return 0;
            }
            yield return m_wGetUrlImage;
            if (m_wGetUrlImage.text.Length == 0)
            {
                Debug.LogError("下载长度为：" + m_wGetUrlImage.text.Length);//同上
                if (Counter >= 3)
                {
                    StopCoroutine("DownLoadWWW");
                }
                else
                {
                    StartCoroutine(DownLoadWWW(DownLoadurl));
                }
                Counter++;
            }

            Debug.LogError("网页数据加载完成" + m_wGetUrlImage);
        }

        //下载网页的文件s
        IEnumerator DownLoadActivityImg(string DownLoadurl)
        {
            string datapath = Application.persistentDataPath + "/PIC_0091.jpg";

            if (m_wGetUrlImage.isDone)
            {
                Debug.LogError("加载成功的图片");
            }
            else
            {
                Debug.LogError("加载不成功的图片");
            }

            Debug.LogError("分享");

            if (m_wGetUrlImage.error != null)
            {
                StopCoroutine("DownLoadActivityImg");
            }

            if (hasDownLoadQRcode == false)
            {
                //为图片赋值
                if (m_wGetUrlImage.texture)
                {
                    Debug.LogError("为图片赋值" + datapath);
                    hasDownLoadQRcode = true;

                    byte[] bt = m_wGetUrlImage.texture.EncodeToJPG();
                    StartCoroutine(DoSave(datapath, bt));
                }
                else
                {
                    Debug.LogError("没有图片");
                    yield return new WaitForSeconds(2);
                    StartCoroutine(DownLoadActivityImg(DownLoadurl));
                }

                #region 加载网页上的图片

                //WWW www = new WWW(DownLoadurl);
                //Debug.LogError("DownLoadurl = " + DownLoadurl + "www = " + www);
                //if (www.error != null)
                //{
                //    Debug.LogError("www.error:" + www.error);//如果没有下载下来处理方法
                //                                             //   raw.texture = Resources.Load<Texture>("icon");
                //    Counter++;
                //    if (Counter >= 3)
                //    {
                //        StopCoroutine("DownLoadActivityImg");
                //    }
                //    else
                //    {
                //        StartCoroutine(DownLoadActivityImg(DownLoadurl));
                //    }
                //}
                //while (!www.isDone)
                //{
                //    yield return 0;
                //}
                //Debug.LogError("--网址数据" + www);
                //yield return www;

                //if (www.text.Length == 0)
                //{
                //    Debug.LogError("下载长度为：" + www.text.Length);//同上
                //    Counter++;
                //    if (Counter >= 3)
                //    {
                //        StopCoroutine("DownLoadActivityImg");
                //    }
                //    else
                //    {
                //        StartCoroutine(DownLoadActivityImg(DownLoadurl));
                //    }
                //}
                //else
                //{
                //    //为图片赋值
                //    if (www.texture)
                //    {
                //        Debug.LogError("为图片赋值" + datapath);
                //        hasDownLoadQRcode = true;

                //        byte[] bt = www.texture.EncodeToJPG();
                //        StartCoroutine(DoSave(datapath, bt));
                //    }
                //    else
                //    {
                //        Debug.LogError("没有图片");
                //        yield return new WaitForSeconds(2);
                //        StartCoroutine(DownLoadActivityImg(DownLoadurl));
                //    }
                //}

                #endregion
            }
            else
            {
                ShareImageForImage(null, datapath, 1, 1, 11);
                //分享的方法
            }
        }

        //保存到本地
        IEnumerator DoSave(string Savepath, byte[] data)
        {
            yield return new WaitUntil(() =>
            {
                File.WriteAllBytes(Savepath, data);
                return File.Exists(Savepath);
            });

            Debug.LogError("进入最终分享函数" + Savepath);
            SDKManager.Instance.ShareImage(null, Savepath, 1, 1, 11);
        }

        /// <summary>
        /// 分享图片  从网页获取图片分享到朋友圈
        /// </summary>
        /// <param name="url">本地图片地址</param>
        /// <param name="type">type=0 分享朋友 1->朋友圈 2->收藏</param>
        public void ShareImageForImage(byte[] bytes, string url, int type, int mode, int avtivity = 0)
        {
            if (GameData.Instance.PlayerNodeDef.userDef.byUserSource == 1)//如果是游客
            {
                if (GameData.Instance)
                {
                    anhui.MahjongCommonMethod.Instance.ShowRemindFrame("请使用微信登录", false);
                }
                else
                {
                    anhui.MahjongCommonMethod.Instance.ShowRemindFrame("请使用微信登录", true);
                }

            }
            else
            {
                GameData.Instance.m_iFestivalActivity = avtivity;

#if UNITY_EDITOR
                anhui.MahjongCommonMethod.Instance.ShowRemindFrame("分享给朋友", false);
#elif UNITY_ANDROID

            AndroidJavaClass jc = new AndroidJavaClass("com.ibluejoy.anhuishuangxi.wxapi.WXEntryActivity");
            AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("GetActivity");
            // swpd.szInNeedSendNumber = "110";//赋值网址传参
            jo.Call("ImageShare", bytes, url, type, mode);//type=0 分享朋友 1->朋友圈 2->收藏
#endif
#if UNITY_IOS
                Debug.LogError("传参到xcode ");
				_WXOpenShareImageReq(bytes, url, type, mode);
#endif
            }
        }
        [DllImport("__Internal")]
        private static extern void _WXOpenShareImageReq(byte[] data, string url, int type, int mode);

        /// <summary>
        /// 分享图片
        /// </summary>
        public void OnBtn_ShareToImage()
        {
            //如果是编辑器的状态下直接关闭界面
            Debug.LogError("分享图片");
            m_bZuanzhuan = false;
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Debug.LogError("开始分享");
            OnCloseWinningPanel();
#if UNITY_EDITOR
#else
                StartCoroutine(DownLoadActivityImg(LobbyContants.FestivalShareActivityUrl));
#endif

            //StartCoroutine(DownLoadActivityImg(LobbyContants.FestivalShareActivityUrl));
        }

        /// <summary>
        /// 关闭房间按钮
        /// </summary>
        public void OnBtn_Close()
        {
            if (m_bDrawLottery) return;

            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            gameObject.SetActive(false);
        }

        #region 中奖记录
        /// <summary>
        /// 中奖纪录
        /// </summary>
        public void OnBtn_WinningRecord()
        {
            if (m_bDrawLottery) return;

            m_gHistory.SetActive(true);

            if (AvtivityHistore_.Count <= 0 || SaveHistory != AvtivityHistore_.Count)
            {
                SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
                Dictionary<string, string> value = new Dictionary<string, string>();
                value.Add("uid", GameData.Instance.PlayerNodeDef.iUserId.ToString());
                string url = LobbyContants.FestivalActivityUrl + "userLotteryLog.x";
                anhui.MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url, value, OnGetValue, "avtivityHistoreContent");
            }
            else
            {
                OnShowWinPanel();
            }

        }
        public void BtnWXShareZhongjiang()
        {
            StartCoroutine(DownLoadActivityImg(LobbyContants.FestivalShareActivityUrl));
        }

        void OnGetValue(string json, int status)
        {
            AvtivityHistore_.Clear();
            FestivalAvtivityHistoreContent data = new FestivalAvtivityHistoreContent();
            data = JsonBase.DeserializeFromJson<FestivalAvtivityHistoreContent>(json.ToString());
            if (data.avtivityHistoreContent[0].status != 1)
            {
                Debug.LogError("获取网页json数据状态错误,status:" + data.avtivityHistoreContent[0].status);
                return;
            }
            for (int i = 0; i < data.avtivityHistoreContent.Count; i++)
            {
                for (int j = 0; j < data.avtivityHistoreContent[i].data.Count; j++)
                {
                    AvtivityHistoreContent content = new AvtivityHistoreContent();
                    content.tim = data.avtivityHistoreContent[i].data[j].tim;
                    content.awardType = data.avtivityHistoreContent[i].data[j].awardType;
                    content.awardNum = data.avtivityHistoreContent[i].data[j].awardNum;
                    AvtivityHistore_.Add(content);
                }
            }
            SaveHistory = AvtivityHistore_.Count;
            OnShowWinPanel();
        }
        /// <summary>
        /// 显示中奖纪录界面
        /// </summary>
        public void OnShowWinPanel()
        {
            Debug.LogError("开始显示：" + AvtivityHistore_.Count);
            for (int i = 0; i < AvtivityHistore_.Count; i++)
            {
                GameObject game = Instantiate(m_WinRecode.gameObject) as GameObject;
                game.transform.SetParent(m_gPrefabParent.transform);
                game.transform.localPosition = new Vector3(game.transform.localPosition.x, game.transform.localPosition.y, 0);
                game.transform.localScale = new Vector3(1, 1, 1);
                game.GetComponent<WinningRecord>().m_txTime.text =
                   anhui.MahjongCommonMethod.Instance.UnixTimeStampToDateTime(Convert.ToInt32(AvtivityHistore_[i].tim), 0).ToString("yyyy年-MM月-dd日 HH:mm");
                switch (Convert.ToInt32(AvtivityHistore_[i].awardType))
                {
                    case 1: game.GetComponent<WinningRecord>().m_txHobby.text = "金币" + AvtivityHistore_[i].awardNum; break;
                    case 2: game.GetComponent<WinningRecord>().m_txHobby.text = "金币" + AvtivityHistore_[i].awardNum; break;
                    case 3: game.GetComponent<WinningRecord>().m_txHobby.text = "积分" + AvtivityHistore_[i].awardNum; break;
                }
                m_lDestHistory.Add(game);
            }
        }

        /// <summary>
        /// 关闭房间中奖按钮
        /// </summary>
        public void OnBtn_History()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            for (int i = 0; i < m_lDestHistory.Count; i++)
            {
                Destroy(m_lDestHistory[i].gameObject);
            }

            m_gHistory.SetActive(false);
        }

        #endregion
    }
}