using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using Spine.Unity;
using DG.Tweening;
using XLua;
using anhui;
namespace PlayBack_1
{
    [Hotfix]
    [LuaCallCSharp]
    public class PlayBackMahjongPanel : MonoBehaviour
    {
        #region 单例
        static PlayBackMahjongPanel instance;
        public static PlayBackMahjongPanel Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion
        /// <summary>
        /// 结果通知面板
        /// </summary>
        public GameResultNoticePanel gameResultNoticePanel;
        /// <summary>
        /// 房间规则的通知面板
        /// </summary>
        public PlayBackGameRulePanel playBackGameRulePanel;
        /// <summary>
        /// 房间规则的通知面板2
        /// </summary>
        public PlayBackGameRulePanel_2 playBackGameRulePanel_2;
        public GameObject _gHongzhongZhongma;

        #region 麻将的父物体        
        public Transform[] HandCardParent; //四个玩家手牌的父物体 0--3
        public Transform[] ShowCardParent_U; //四个玩家桌面牌的第一层的父物体
        public Transform[] ShowCardParent_D; //四个玩家桌面牌的第二层的父物体
        public Transform[] ShowCardParent_3; //四个玩家桌面牌的第三层的父物体
        public GameObject[] Flower; //花牌
        [HideInInspector]
        public GameObject LastTableCard; //玩家打到桌面的最后一张牌
        public Transform PutParent; //打出牌的父物体
        public Transform[] Big_Pos;  //玩家打出大牌的位置
        public Transform[] Table_Pos;  //玩家打到桌面第一张的位置
        //提示吃碰杠胡的位置
        public GameObject[] specialPos;
        //提示吃碰杠胡的位置
        public GameObject[] specialPos_effect;
        //自摸位置
        public GameObject[] specialPos_zimo;
        //胡牌位置
        public GameObject[] specialPos_hu;
        //吃的特效的位置
        public GameObject[] specialPos_Chi;
        //补花特效的位置
        public GameObject[] specialPos_bubua;
        //及时更新分数的位置
        public GameObject[] specialScorePos;
        //出牌动画的初始位置
        public GameObject[] PutAnimationInitPos;
        public GameObject CanDownRu;  //关于能跑能下的界面
        #endregion
        [HideInInspector]
        public GameObject EffectBuhua;
        #region 相关牌的图片        
        public Sprite[] sWan;   //万字牌
        public Sprite[] sTong;  //筒子牌
        public Sprite[] sTiao;  //索子牌
        public Sprite[] sFeng;  //风字牌
        public Sprite[] sJian;  //剪字牌      
        public Sprite[] sHua; //花牌
        public Sprite[] sKong_H; //1、3号位的杠的背景图  0明杠 1暗杠
        public Sprite[] sKong_V;  //2、4号位的杠的背景图  0明杠 1暗杠
        public Sprite[] playStatus;//播放状态图片
        #endregion       

        #region 关于游戏界面的变量
        public Text RoomId;  //房间号
        public Text GameNum;  //本局的局数
        public Text GamePrograssing; //本局游戏进度
        public Text PlaySpeed;  //播放速度
        public Text LeftCardNum; //剩余牌的数量
        public RawImage[] HeadImage;  //玩家头像
        public GameObject[] Banker; //庄家头像
        public Image[] OpenRoomer; //房主图片
        public Text[] PlayerScore;  //玩家分数
        public Text[] NameNick; //玩家昵称
        public GameObject PlayerRemindDirec;  //玩家出牌的指示方位
        public Animator[] _headAnim;  //头像光效动画
        public Image playBtn;//播放按钮
        public GameObject[] SpeciclNotice;  //玩家的吃碰杠胡通知界面
        #endregion

        public bool isPause; //是否暂停的标志位   
        public bool isFinishedAction; //是否完成动画的标志位   
        public int iPlaySpeed;  //播放速度，默认为1
        float timer = 2f;  //点击重播按钮，间隔1.5f才可以再次点击
        bool isClickClose; //是否可以点击关闭按钮

        string path = "Game/Ma/PlayBack/"; //保存预置体的路径

        public enum CardType
        {
            Empty_D,
            Empty_H,
            Empty_U,
            SpecialCard_D,
            SpecialCard_L,
            SpecialCard_R,
            SpecialCard_U,
            ThirteenCard_D,
            ThirteenCard_H,
            ThirteenCard_U,
            WinDCard,
            WinHCard,
            WinUCard
        }

        void Awake()
        {
            instance = this;
            timer = 2f;
        }

        void Update()
        {
            if (!isClickClose)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timer = 2f;
                    isClickClose = true;
                }
            }
        }

        //初始化玩家数据
        void InitPanel()
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            pbd.playerCardMessage = new PlayBackData.PlayerCardMessage[4];
            pbd.iDoneDealNum = 0;
            isFinishedAction = true;
            isClickClose = true;
            PoolManager.Clear();
            pbd.iLeftCardNum = 136;
            iPlaySpeed = 1;
            Time.timeScale = iPlaySpeed;
            isPause = false;
            pbd.iLeftCardNum = 136;

            //减去玩家12张底牌
            if (pbd.playingMethodConf.byReserveMode == 1 || pbd.playingMethodConf.byReserveMode == 3)
            {
                pbd.iLeftCardNum -= 12;
            }
            else if (pbd.playingMethodConf.byReserveMode == 2)
            {
                pbd.iLeftCardNum -= 14;
            }
            else if (pbd.playingMethodConf.byReserveMode == 5)
            {
                pbd.iLeftCardNum -= 6;
            }

            if (pbd.playingMethodConf_2.byReserveMode == 1)
            {
                pbd.iLeftCardNum -= 12;
            }

            //减去风牌
            if (pbd.playingMethodConf.byCreateModeHaveWind == 0)
            {
                pbd.iLeftCardNum -= 16;
            }

            //减去字牌
            if (pbd.playingMethodConf.byCreateModeHaveDragon == 0)
            {
                pbd.iLeftCardNum -= 12;
            }

            for (int i = 0; i < 4; i++)
            {
                pbd.iPoint[i] = pbd.iPointInit[i];
                // Flower[i].transform.GetChild(0).GetComponent<Text>().text = "X"+0;
                MahjongLobby_AH.GameData.Instance.PlayBackData.FlowerCpount[i] = 0;
            }
            DestroyAllCard();
            StopAllCoroutines();
        }

        //删除所有的玩家手牌
        void DestroyAllCard()
        {
            for (int j = 0; j < 4; j++)
            {
                PoolAgent[] agent = PutParent.GetChild(j).GetComponentsInChildren<PoolAgent>();
                for (int k = 0; k < agent.Length; k++)
                {
                    Destroy(agent[k].gameObject);
                }
            }
        }

        void Start()
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            //显示玩家的庄家信息
            int index = GetPlayerIndex(pbd.byDealerSeat);
            Banker[index].gameObject.SetActive(true);
            pbd.FlowerCpount = new int[4] { 0, 0, 0, 0 };
            //显示房主图片
            InitPanel();
            UpdateHeadImage();
            UpdatePanel();
            UpdatePrograssing();
            MahjongLobby_AH.SDKManager.Instance.gameObject.GetComponent<CameControler>().PostMsg("uloading");
            DelayOrderByPlayBack_Ie(0, 0);
        }

        /// <summary>
        /// 执行回放的第几个数据
        /// </summary>
        public void DelayOrderByPlayBack_Ie(float timer, int iArrayIndex)
        {
            if (isPause)
            {
                StartCoroutine(delayDo());
                return;
            }
            isFinishedAction = false;
            StartCoroutine(DelayOrderByPlayBack(timer, iArrayIndex));
        }


        IEnumerator delayDo()
        {
            yield return new WaitForSeconds(1f);
            DelayOrderByPlayBack_Ie(0, MahjongLobby_AH.GameData.Instance.PlayBackData.iDoneDealNum);
        }
        /// <summary>
        /// 执行回放的第几个数据
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="iArrayIndex"></param>
        /// <returns></returns>
        IEnumerator DelayOrderByPlayBack(float timer, int iArrayIndex)
        {
            yield return new WaitForSeconds(timer);
            OrderByPlayBack(iArrayIndex);
        }

        /// <summary>
        /// 执行回放的第几个数据
        /// </summary>
        /// <param name="iArrayIndex"></param>
        void OrderByPlayBack(int iArrayIndex)
        {
            //开始产生玩家摸得牌
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            //Debug.LogError("执行回放的第几个数据:" + pbd.sPlayBackMessageData[pbd.iDoneDealNum].Length+","+ pbd.iDoneDealNum);
            if (iArrayIndex <= pbd.sPlayBackMessageData.Length - 1)
            {
                pbd.DealPlayerNotice(pbd.sPlayBackMessageData[pbd.iDoneDealNum]);
                pbd.iDoneDealNum++;
                UpdatePrograssing();
            }
            isFinishedAction = true;
        }


        public void DelayShowHuangZhuang_2(MahjongGame_AH.Network.Message.NetMsg.ClientGameResultNotice_2 msg)
        {
            StartCoroutine(delayShow_ie_2(msg));
        }

        IEnumerator delayShow_ie_2(MahjongGame_AH.Network.Message.NetMsg.ClientGameResultNotice_2 msg)
        {
            yield return new WaitForSeconds(2f);
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            //直接显示玩家的结算结果
            pbd.bHandleTiles = msg.bya2HandTiles;
            for (int i = 0; i < 4; i++)
            {
                pbd.bResultPoint[i] = msg.caResultPoint[i];
            }
            pbd.resultType_2 = msg.aResultType;
            pbd.byShootSeat = msg.byShootSeat;
            pbd.byaWinSrat = msg.byaWinSeat;
            pbd.caFanResult = msg.caFanResult;
            for (int i = 0; i < pbd.iPoint.Length; i++)
            {
                pbd.iPoint[i] = msg.caResultPoint[i];
            }
            gameResultNoticePanel.SpwanGameReult_Round(msg.byaWinSeat);
        }

        public void DelayShowHuangZhuang(MahjongGame_AH.Network.Message.NetMsg.ClientGameResultNotice msg)
        {
            StartCoroutine(delayShow_ie(msg));
        }


        IEnumerator delayShow_ie(MahjongGame_AH.Network.Message.NetMsg.ClientGameResultNotice msg)
        {
            yield return new WaitForSeconds(2f);
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            //直接显示玩家的结算结果
            pbd.bHandleTiles = msg.bya2HandTiles;
            pbd.bResultPoint = msg.caResultPoint;
            pbd.resultType = msg.aResultType;
            pbd.byShootSeat = msg.byShootSeat;
            pbd.byaWinSrat = msg.byaWinSeat;
            pbd.caFanResult = msg.caFanResult;
            for (int i = 0; i < pbd.iPoint.Length; i++)
            {
                pbd.iPoint[i] = msg.caResultPoint[i];
            }
            gameResultNoticePanel.SpwanGameReult_Round(msg.byaWinSeat);
        }

        #region 麻将的回放逻辑


        /// <summary>
        /// 获取该玩家对应的数组下标  0----3
        /// </summary>
        /// <param name="seatNum">玩家座位号</param>
        /// <returns></returns>
        public int GetPlayerIndex(int seatNum)
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            if (seatNum == pbd.ShowSeatNum)
            {
                return 0;
            }
            else
            {
                if (seatNum > pbd.ShowSeatNum)
                {
                    return (seatNum - pbd.ShowSeatNum);
                }
                else
                {
                    return (seatNum - pbd.ShowSeatNum + 4);
                }
            }
            return -1;
        }


        /// <summary>
        /// 产生玩家手牌
        /// </summary>
        /// <param name="byValue">麻将的值的数组</param>
        /// <param name="seatNum">玩家座位号1---4</param>
        /// <param name="isByHand">是否是通知出牌</param>
        public void SpwanHandCard(byte[] byValue, int seatNum, bool isByHand)
        {
            //根据玩家座位号，决定他的数组下标
            int index = GetPlayerIndex(seatNum);

            int status = -1;  //显示父物体下的下标位置            
            string name = "";  //要产生预置体的名字
            Vector3 angle = Vector3.zero;
            if (isByHand)
            {
                switch (index)
                {
                    case 0:
                        name = CardType.Empty_D.ToString();
                        status = 1;
                        break;
                    case 1:
                        name = CardType.Empty_H.ToString();
                        status = 0;
                        break;
                    case 2:
                        name = CardType.Empty_U.ToString();
                        status = 0;
                        break;
                    case 3:
                        name = CardType.Empty_H.ToString();
                        status = 1;
                        break;
                }
            }
            else
            {
                switch (index)
                {
                    case 0:
                        name = CardType.WinDCard.ToString();
                        status = 1;
                        angle = Vector3.zero;
                        break;
                    case 1:
                        name = CardType.WinHCard.ToString();
                        status = 0;
                        angle = new Vector3(0, 0, 90f);
                        break;
                    case 2:
                        name = CardType.WinUCard.ToString();
                        status = 0;
                        angle = new Vector3(0, 0, 180f);
                        break;
                    case 3:
                        name = CardType.WinHCard.ToString();
                        angle = new Vector3(0, 0, -90f);
                        status = 1;
                        break;
                }
            }

            //产生所有的玩家的手牌
            for (int i = 0; i < byValue.Length; i++)
            {
                SpwanNormalCard(byValue[i], name, status, HandCardParent[index], angle, seatNum);
            }
        }

        /// <summary>
        /// 产生单个的普通的手牌
        /// </summary>
        /// <param name="ivalue">麻将的值</param>
        /// <param name="name">预置体的名字</param>
        /// <param name="status">状态，0表示新产生的物体放在第一个，1表示新产生的物体放在最后一个</param>
        /// <param name="parent">产生物体的父物体</param>
        /// <param name="angle_">麻将的花色的旋转值</param>
        public void SpwanNormalCard(int ivalue, string name, int status, Transform parent, Vector3 angle_, int seatNum)
        {
            PlayBack_1.PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            if (pbd.playerCardMessage[seatNum - 1] == null)
            {
                pbd.playerCardMessage[seatNum - 1] = new PlayBackData.PlayerCardMessage();
            }


            GameObject go = PoolManager.Spawn(path, name);
            go.transform.SetParent(parent);
            go.transform.localScale = Vector3.one;
            go.transform.localEulerAngles = Vector3.zero;
            if (status == 0)
            {
                go.transform.SetAsFirstSibling();
            }
            else if (status == 1)
            {
                go.transform.SetAsLastSibling();
            }
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0f);

            if (go.transform.GetComponent<Mahjong>())
            {
                go.transform.GetComponent<Mahjong>().bMahjongValue = (byte)ivalue;

                //更新界面
                go.transform.Find("Image/Num").transform.localEulerAngles = angle_;
                go.transform.Find("Image/Num").GetComponent<Image>().sprite = UpdateCardValue((byte)ivalue);

                //减去桌面牌的数量
                pbd.iLeftCardNum--;
                UpdatePrograssing();

                //保存玩家的手牌信息                        
                MahjongLobby_AH.GameData.Instance.PlayBackData.playerCardMessage[seatNum - 1].HandCard.Add((byte)ivalue);
            }

        }

        /// <summary>
        /// 产生玩家出牌动画
        /// </summary>
        /// <param name="byValue"></param>
        /// <param name="seatNum"></param>
        public void PlayPutAnimation(int byValue, int seatNum)
        {
            //产生玩家刚出的手牌
            int index = GetPlayerIndex(seatNum);
            GameObject go = PoolManager.Spawn(path, CardType.WinDCard.ToString());
            go.transform.SetParent(PutParent);
            go.transform.localScale = Vector3.one;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localPosition = PutAnimationInitPos[index].transform.localPosition;
            go.transform.Find("Image/Num").GetComponent<Image>().sprite = UpdateCardValue((byte)byValue);
            go.GetComponent<Mahjong>().bMahjongValue = (byte)byValue;
            go.GetComponent<Mahjong>().PutCardAnimator_pb(seatNum, byValue);
        }

        /// <summary>
        /// 更新麻将的界面
        /// </summary>
        /// <param name="value">麻将的十进制的值</param>
        /// <returns></returns>
        public Sprite UpdateCardValue(byte value)
        {
            Sprite spr = new Sprite();
            int iMahVlaue = value / 16;  //麻将的花色  1表示万，2表示筒，3表示条，4表示风，5表示箭牌
            int iValue = value % 16 - 1;  //麻将对应花色的值  1---9（风牌，箭牌除外）

            //根据花色不同显示牌面
            switch (iMahVlaue)
            {
                case 1:
                    spr = sWan[iValue];
                    break;
                case 2:
                    spr = sTong[iValue];
                    break;
                case 3:
                    spr = sTiao[iValue];
                    break;
                case 4:
                    spr = sFeng[iValue];
                    break;
                case 5:
                    spr = sJian[iValue];
                    break;
                case 6:
                    spr = sHua[iValue];
                    break;
            }
            return spr;
        }




        /// <summary>
        /// 删除玩家的手牌
        /// </summary>
        /// <param name="byValue">要删除的手牌的值</param>
        /// <param name="seatNum">玩家座位号</param>
        /// <param name="count">删除对应手牌的数量</param>
        public void DelPlayerHandleCard(int byValue, int seatNum)
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            Mahjong[] mah = GetPlayerAllHandCard(seatNum);

            for (int j = 0; j < mah.Length; j++)
            {
                if (mah[j].bMahjongValue == byValue)
                {
                    PoolManager.Unspawn(mah[j].gameObject);
                    //删除玩家的手牌信息                        
                    pbd.playerCardMessage[seatNum - 1].HandCard.Remove((byte)byValue);
                    break;
                }
            }

            GameObject[] go = GetPlayerEmptyCard(seatNum);
            //删除那张空牌
            if (go != null)
            {
                for (int i = 0; i < go.Length; i++)
                {
                    PoolManager.Unspawn(go[i]);
                }
            }
        }

        /// <summary>
        /// 产生特殊牌
        /// </summary>
        /// <param name="byValue">麻将的值</param>
        /// <param name="seatNum">玩家座位号1---4</param>
        /// <param name="type">特殊牌的类型，0弃1吃2碰3杠4胡5吃(长治花三门的十三幺)6抢(长治花三门的十三幺)</param>
        /// <param name="kongstatus">0没有杠，1明杠，2暗杠，3碰杠</param>  
        /// <param name="putCardSeatNum">出牌座位号 0表示暗杠</param>  
        public void SpwanSpecialCard(byte[] byValue, int seatNum, int type, int kongstatus, int putCardSeatNum)
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            if (byValue[0] <= 0)
            {
                return;
            }
            int index = GetPlayerIndex(seatNum);  //玩家的下标   0--3                    


            //处理胡牌
            if (type == 4)
            {
                //玩家自摸
                if (pbd.PutCardSeatNum == seatNum)
                {
                    //处理胡牌
                    SpwanSpeaiclTypeRemind(index, seatNum, 10);
                }
                else
                {
                    //处理胡牌
                    SpwanSpeaiclTypeRemind(index, seatNum, type);
                }

                //同时把玩家的手牌拿过来
                byte[] mah = new byte[] { byValue[0] };
                pbd.iLeftCardNum += 1;

                //如果是自摸不处理
                if (pbd.PutCardSeatNum != seatNum)
                {
                    //删除桌面上的牌
                    DelTabelCard();
                    SpwanHandCard(mah, seatNum, true);
                    SpwanHandCard(mah, seatNum, false);
                }
                return;
            }

            int count = 0;
            if (type == 1 || type == 2)
            {
                count = 2;
            }
            else if (type == 3)
            {
                if (kongstatus == 1)
                {
                    count = 3;
                }
                else if (kongstatus == 2)
                {
                    count = 4;
                }
                else if (kongstatus == 3)
                {
                    count = 1;
                }
            }
            else if (type == 5 || type == 6)
            {
                count = 1;
            }

            //删除手中的牌
            for (int i = 0; i < count; i++)
            {
                DelPlayerHandleCard(((i >= byValue.Length || byValue[i] == 0) ? byValue[0] : byValue[i]), seatNum);
            }


            //产生特殊预置体的特效
            SpwanSpeaiclTypeRemind(index, seatNum, type);

            int status = -1;  //排序的状态  0表示放在首位  1表示放在末尾
            string name = "";  //要产生的预置体的名字
            Vector3 angle = Vector3.zero; //预置体的旋转方向

            switch (index)
            {
                case 0:
                    status = 0;
                    if (type == 5 || type == 6)
                    {
                        name = CardType.ThirteenCard_D.ToString();
                        angle = Vector3.zero;
                    }
                    else
                    {
                        name = CardType.SpecialCard_D.ToString();
                        angle = Vector3.zero;
                    }
                    break;
                case 1:
                    status = 1;
                    if (type == 5 || type == 6)
                    {
                        name = CardType.ThirteenCard_H.ToString();
                        angle = new Vector3(0, 0, 90f);
                    }
                    else
                    {
                        name = CardType.SpecialCard_R.ToString();
                        angle = new Vector3(0, 0, 90f);
                    }

                    break;
                case 2:
                    status = 1;
                    if (type == 5 || type == 6)
                    {
                        name = CardType.ThirteenCard_U.ToString();
                        angle = new Vector3(0, 0, 180f);
                    }
                    else
                    {
                        name = CardType.SpecialCard_U.ToString();
                        angle = new Vector3(0, 0, 180f);
                    }

                    break;
                case 3:
                    status = 0;
                    if (type == 5 || type == 6)
                    {
                        name = CardType.ThirteenCard_H.ToString();
                        angle = new Vector3(0, 0, -90f);
                    }
                    else
                    {
                        name = CardType.SpecialCard_L.ToString();
                        angle = new Vector3(0, 0, -90f);
                    }
                    break;
            }



            //产生特殊预置体
            SpwanSingleSPecial(byValue, seatNum, type, kongstatus, status, name, angle, putCardSeatNum);
        }

        /// <summary>
        /// 产生特殊牌的预置体
        /// </summary>
        /// <param name="byValue"></param>
        /// <param name="type"></param>
        /// <param name="kongstatus"></param>
        /// <param name="status"></param>
        /// <param name="name"></param>
        /// <param name="angle"></param>
        /// <param name="putCardSeatNum"></param>
        void SpwanSingleSPecial(byte[] byValue, int seatNum, int type, int kongstatus, int status, string name, Vector3 angle, int putCardSeatNum)
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            PlayBackData.SpecialType temp = new PlayBackData.SpecialType();
            int index = GetPlayerIndex(seatNum);
            if (type == 3)
            {
                if (kongstatus == 3)
                {
                    for (int i = 0; i < MahjongLobby_AH.GameData.Instance.PlayBackData.playerCardMessage[seatNum - 1].SpecialCard.Count; i++)
                    {
                        if (byValue[0] == MahjongLobby_AH.GameData.Instance.PlayBackData.playerCardMessage[seatNum - 1].SpecialCard[i].byValue)
                        {
                            MahjongLobby_AH.GameData.Instance.PlayBackData.playerCardMessage[seatNum - 1].SpecialCard[i].type = 3;
                        }
                    }

                    //找到该玩家的对应的预置体
                    Mahjong[] mah = HandCardParent[index].GetComponentsInChildren<Mahjong>();
                    Mahjong tempgo = null;
                    for (int i = 0; i < mah.Length; i++)
                    {
                        if (mah[i].name.Contains("SpecialCard") && mah[i].bMahjongValue == byValue[0])
                        {
                            tempgo = mah[i];
                        }
                    }

                    //如果是碰杠直接更新预置体                    
                    UpdateSpecialCard(byValue, type, kongstatus, tempgo.transform, angle, false, index, putCardSeatNum);
                    return;
                }
                else
                {
                    temp.byValue = byValue[0];
                    temp.type = type;
                    //添加玩家的特殊预置体的信息
                    MahjongLobby_AH.GameData.Instance.PlayBackData.playerCardMessage[seatNum - 1].SpecialCard.Add(temp);
                }
            }
            else
            {
                temp.byValue = byValue[0];
                temp.type = type;
                //添加玩家的特殊预置体的信息
                MahjongLobby_AH.GameData.Instance.PlayBackData.playerCardMessage[seatNum - 1].SpecialCard.Add(temp);
            }


            GameObject go = PoolManager.Spawn(path, name);
            go.transform.SetParent(HandCardParent[index]);
            go.transform.localScale = Vector3.one;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0f);
            go.transform.GetComponent<Mahjong>().bMahjongValue = byValue[0];

            //获取玩家的已经产生特殊牌的数量
            int iSpecialCount = MahjongLobby_AH.GameData.Instance.PlayBackData.playerCardMessage[seatNum - 1].SpecialCard.Count;

            //从头开始排序
            if (status == 0)
            {
                go.transform.SetSiblingIndex(iSpecialCount - 1);
            }
            //从末尾开始排序
            else if (status == 1)
            {
                go.transform.SetSiblingIndex(HandCardParent[index].childCount - iSpecialCount);
            }

            //更新界面
            UpdateSpecialCard(byValue, type, kongstatus, go.transform, angle, false, index, putCardSeatNum);
        }

        /// <summary>
        /// 产生特殊牌型的提示
        /// </summary>
        /// <param name="index">0--3数组下标4表示放在屏幕正中央</param>
        /// <param name="isShowWinPanel"></param>
        /// <param name="type">1吃2碰3杠4胡5十三幺吃6十三幺抢7听8放炮9荒庄10自摸  11开局信息，12对局结束 13补花提示</param>
        public void SpwanSpeaiclTypeRemind(int index, int seatNum, int type_, bool isShowWinPanel = false)
        {
            // Debug.LogWarning ("特效SpwanSpeaiclTypeRemind");
            string path = "";
            switch (type_)
            {
                case 1:
                case 5:
                    path = "Game/Effect/Chi";
                    break;
                case 2:
                    path = "Game/Effect/Peng";
                    break;
                case 3:
                    path = "Game/Effect/Gang";
                    break;
                case 4:
                    path = "Game/Effect/Hu";
                    break;
                case 7:
                    path = "Game/Effect/Ting";
                    break;
                case 6:
                    path = "Game/Effect/qiang";
                    break;
                case 9:
                    path = "Game/Effect/huangzhuang";
                    break;
                case 10:
                    path = "Game/Effect/Zimo";
                    break;
                case 11:
                case 12:
                case 13:
                case 14:
                    path = "Game/Effect/qiantai";
                    break;
                case 15:
                    path = "Game/Effect/fangpao";
                    break;
                case 16:
                    path = "Game/Effect/buhua";
                    break;
                //抢杠胡
                case 17:
                //补杠
                case 18:
                    path = "Game/Effect/RobbingWin";
                    break;
            }
            if (type_ == 16 && EffectBuhua != null)
            {
                DestroyImmediate(EffectBuhua);
            }
            //Debug.LogError("不花" + type_);
            //产生胡牌的图标
            GameObject go = Instantiate(Resources.Load<GameObject>(path));
            go.transform.SetParent(transform.Find("Common"));

            if (index == 4)
            {
                //决定播放哪个动画
                if (type_ == 11)
                {
                    go.transform.localPosition = new Vector3(20f, 0f, 0f);
                    go.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().timeScale = 0.5f;
                    go.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().AnimationName = "eff_shmj_mgt";
                }
                else if (type_ == 12)
                {
                    go.transform.localPosition = Vector3.zero;
                    go.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().timeScale = 0.5f;
                    go.GetComponent<DeleteEffect>().Ani.GetComponent<SkeletonAnimation>().AnimationName = "eff_shmj_hfj";
                }
                else
                {
                    go.transform.localPosition = Vector3.zero;
                }
            }
            else
            {
                //根据类型不同，取不同位置，自摸胡特殊处理
                if (type_ == 10)
                {
                    go.transform.localPosition = specialPos_zimo[index].transform.localPosition;
                }
                else if (type_ == 4)
                {
                    go.transform.localPosition = specialPos_hu[index].transform.localPosition;
                }
                //荒庄
                else if (type_ == 9)
                {
                    go.transform.localPosition = Vector3.zero;
                }
                //吃
                else if (type_ == 5)
                {
                    go.transform.localPosition = specialPos_Chi[index].transform.localPosition;
                }
                //补花
                else if (type_ == 16)
                {
                    go.transform.localPosition = specialPos_bubua[index].transform.localPosition;
                }
                else
                {
                    go.transform.localPosition = specialPos_effect[index].transform.localPosition;
                }
            }

            if (type_ == 9)
            {
                go.transform.localScale = Vector3.one * 85;
            }
            else
            {
                go.transform.localScale = Vector3.one * 100;
            }

            go.transform.localEulerAngles = Vector3.zero;

            //播放吃碰杠胡提示音
            #region 吃碰杠胡抢语音
            PlayBackAudioMgr.AudioType type = PlayBackAudioMgr.AudioType.AUDIO_NONE;
            //吃
            if (type_ == 1 || type_ == 5)
            {
                type = PlayBackAudioMgr.AudioType.Chi;
            }

            //碰
            if (type_ == 2)
            {
                type = PlayBackAudioMgr.AudioType.Peng;
            }

            //杠
            if (type_ == 3)
            {
                type = PlayBackAudioMgr.AudioType.Gang;
            }

            //抢
            if (type_ == 6)
            {
                type = PlayBackAudioMgr.AudioType.Qiang;
            }

            //胡
            if (type_ == 4)
            {
                type = PlayBackAudioMgr.AudioType.Hu;
            }

            //自摸
            if (type_ == 10)
            {
                type = PlayBackAudioMgr.AudioType.Hu_Zimo;
            }

            //听
            if (type_ == 7)
            {
                type = PlayBackAudioMgr.AudioType.Ting;
            }
            PlayBackAudioMgr.Instance.OnPlayAuto(type, 1);
            #endregion 吃碰杠胡抢语音
        }

        /// <summary>
        /// 更新特殊牌的界面
        /// </summary>
        /// <param name="byValue">麻将的值</param>
        /// <param name="type">特殊牌的类型，0弃1吃2碰3杠4胡5吃(长治花三门的十三幺)6抢(长治花三门的十三幺)</param>
        /// <param name="kongstatus">0没有杠，1明杠，2暗杠，3碰杠</param>
        /// <param name="go">要更新的预置体</param>
        /// <param name="angle">牌面的角度</param>
        /// <param name="pengToGang">是否是碰变成杠</param>
        /// <param name="index">玩家的数组下标</param>
        /// <param name="putCardSeatNum">出牌座位号 0表示出牌玩家</param>
        void UpdateSpecialCard(byte[] byValue, int type, int kongstatus, Transform go, Vector3 angle, bool pengToGang, int index, int putCardSeatNum)
        {
            //处理玩家十三幺的吃碰
            if (type == 5 || type == 6)
            {
                go.transform.Find("Image/Num").GetComponent<Image>().sprite = UpdateCardValue((byte)byValue[0]);
                go.transform.Find("Image/Num").transform.localEulerAngles = angle;
                return;
            }

            Transform right_bg = go.transform.Find("belowCardR");
            Transform left_bg = go.transform.Find("belowCardL");
            Transform middle_bg = go.transform.Find("belowCardM");
            Transform top_bg = go.transform.Find("topCard");
            Transform right = go.transform.Find("belowCardR/num");
            Transform left = go.transform.Find("belowCardL/num");
            Transform middle = go.transform.Find("belowCardM/num");
            Transform top = go.transform.Find("topCard/num");
            //碰杠牌的指向图片
            Transform image_peng = go.transform.Find("belowCardM/Image");
            Transform image_gang = go.transform.Find("topCard/Image");

            int index_pg = 0;  //出牌人的下标
            if (kongstatus != 2)
            {
                index_pg = GetPlayerIndex(putCardSeatNum);
            }
            //Debug.LogError("type = " + type);

            if (type == 2 || type == 1)
            {
                image_gang.gameObject.SetActive(false);
                image_peng.gameObject.SetActive(true);
                //Debug.LogError("===" + index + "," + index_pg + "," + (index - index_pg) + "," + (Mathf.Abs(index - index_pg) % 2));
                //修改方向
                if (Mathf.Abs(index - index_pg) % 2 == 1)
                {
                    //Debug.LogError("碰 if ");
                    image_peng.transform.localEulerAngles += new Vector3(0, 0, 90 * Mathf.Abs(index - index_pg));
                }
                //else
                //{
                //    Debug.LogError("碰 else ");
                //}
            }

            if (type == 3)
            {
                //Debug.LogError("===" + index + "," + index_pg + "," + (index - index_pg) + "," + (Mathf.Abs(index - index_pg) % 2));
                image_peng.gameObject.SetActive(false);
                if (kongstatus != 2)
                {
                    image_gang.gameObject.SetActive(true);
                    //修改方向
                    if (Mathf.Abs(index - index_pg) % 2 == 1)
                    {
                        image_gang.transform.localEulerAngles += new Vector3(0, 0, -90 * Mathf.Abs(index - index_pg));
                    }
                    else
                    {
                        if ((index - index_pg) == 2 && index != 2)
                        {
                            image_gang.transform.localEulerAngles += new Vector3(0, 0, -90);
                        }
                    }
                }
                else
                {
                    image_gang.gameObject.SetActive(false);
                }
            }

            Sprite Kong_Bg_An = null;  //保存暗杠的背景图
            Sprite Kong_Bg_Ming = null;  //保存明杠的背景图
            if (index == 0 || index == 2)
            {
                Kong_Bg_Ming = sKong_V[0];
                Kong_Bg_An = sKong_V[1];
            }
            else
            {
                Kong_Bg_Ming = sKong_H[0];
                Kong_Bg_An = sKong_H[1];
            }

            //如果是碰杠，直接更新预置体
            if (pengToGang)
            {
                top_bg.GetComponent<Image>().enabled = true;
                top.GetComponent<Image>().enabled = true;
                top_bg.GetComponent<Image>().sprite = Kong_Bg_Ming;
                top.GetComponent<Image>().sprite = UpdateCardValue((byte)byValue[0]);
                return;
            }

            //如果是碰或吃，隐藏顶上的牌
            if (type == 2 || type == 1)
            {
                top_bg.GetComponent<Image>().enabled = false;
                top.GetComponent<Image>().enabled = false;
                right.localEulerAngles = angle;
                right.GetComponent<Image>().sprite = UpdateCardValue((byte)byValue[0]);

                left.localEulerAngles = angle;
                left.GetComponent<Image>().sprite = UpdateCardValue((byte)byValue[1] == 0 ? byValue[0] : byValue[1]);

                middle.localEulerAngles = angle;
                middle.GetComponent<Image>().sprite = UpdateCardValue((byte)byValue[2] == 0 ? byValue[0] : byValue[2]);
            }
            else if (type == 3)
            {
                if (kongstatus == 2 && MahjongLobby_AH.GameData.Instance.PlayBackData.iMethodId == 13)
                {
                    top_bg.GetComponent<Image>().sprite = Kong_Bg_An;
                    top.GetComponent<Image>().enabled = false;
                }
                else
                {
                    top_bg.GetComponent<Image>().enabled = true;
                    top.GetComponent<Image>().enabled = true;

                    top.localEulerAngles = angle;
                    top.GetComponent<Image>().sprite = UpdateCardValue((byte)byValue[0]);
                }


                if (kongstatus == 2)
                {
                    right_bg.GetComponent<Image>().sprite = Kong_Bg_An;
                    right.GetComponent<Image>().enabled = false;

                    left_bg.GetComponent<Image>().sprite = Kong_Bg_An;
                    left.GetComponent<Image>().enabled = false;

                    middle_bg.GetComponent<Image>().sprite = Kong_Bg_An;
                    middle.GetComponent<Image>().enabled = false;
                }
                else
                {
                    right_bg.GetComponent<Image>().sprite = Kong_Bg_Ming;
                    right.GetComponent<Image>().sprite = UpdateCardValue((byte)byValue[0]);

                    left_bg.GetComponent<Image>().sprite = Kong_Bg_Ming;
                    left.GetComponent<Image>().sprite = UpdateCardValue((byte)byValue[1] == 0 ? byValue[0] : byValue[1]);

                    middle_bg.GetComponent<Image>().sprite = Kong_Bg_Ming;
                    middle.GetComponent<Image>().sprite = UpdateCardValue((byte)byValue[2] == 0 ? byValue[0] : byValue[2]);
                }
            }
        }


        /// <summary>
        /// 获取对应玩家所有的手牌
        /// </summary>
        /// <param name="seatNum"></param>
        /// <returns></returns>
        public Mahjong[] GetPlayerAllHandCard(int seatNum)
        {
            int index = GetPlayerIndex(seatNum);

            Transform trans = HandCardParent[index];

            Mahjong[] mah = trans.GetComponentsInChildren<Mahjong>();
            List<Mahjong> lsMah = new List<Mahjong>();

            for (int i = 0; i < mah.Length; i++)
            {
                if (mah[i].name.Contains("Win") && mah[i].isActiveAndEnabled)
                {
                    lsMah.Add(mah[i]);
                }
            }

            return lsMah.ToArray();
        }

        /// <summary>
        /// 获取玩家手中的空牌
        /// </summary>
        /// <param name="seatNum"></param>
        /// <returns></returns>
        GameObject[] GetPlayerEmptyCard(int seatNum)
        {
            int index = GetPlayerIndex(seatNum);

            //Debug.LogError("index:" + index);
            Transform trans = HandCardParent[index];
            string name = "";

            switch (index)
            {
                case 0:
                    name = CardType.Empty_D.ToString();
                    break;
                case 1:
                case 3:
                    name = CardType.Empty_H.ToString();
                    break;
                case 2:
                    name = CardType.Empty_U.ToString();
                    break;
            }

            List<GameObject> empty = new List<GameObject>();

            for (int i = 0; i < trans.childCount; i++)
            {
                if (string.Equals(trans.GetChild(i).name, name))
                {
                    empty.Add(trans.GetChild(i).gameObject);
                }
            }

            return empty.ToArray();
        }

        /// <summary>
        /// 更新所有玩家的牌的界面
        /// </summary>
        /// <param name="seatNum"></param>
        public void UpdatePlayerAllCard(int seatNum)
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            Mahjong[] mah = GetPlayerAllHandCard(seatNum);
            int index = GetPlayerIndex(seatNum);
            List<byte> value = pbd.SortToHandCard(pbd.playerCardMessage[seatNum - 1].HandCard);

            int length = value.Count - 1;  //记录玩家手中牌的数量

            if (value.Count != mah.Length)
            {
                Debug.LogError("玩家手中牌的数据和数量不对应");
                return;
            }

            if (index == 0 || index == 3)
            {
                for (int i = 0; i < mah.Length; i++)
                {
                    mah[i].bMahjongValue = value[i];
                    mah[i].transform.Find("Image/Num").GetComponent<Image>().sprite = UpdateCardValue(value[i]);
                }
            }
            else if (index == 1 || index == 2)
            {
                for (int i = 0; i < mah.Length; i++)
                {
                    mah[i].bMahjongValue = value[length - i];
                    mah[i].transform.Find("Image/Num").GetComponent<Image>().sprite = UpdateCardValue(value[length - i]);
                }
            }

            GameObject[] go = GetPlayerEmptyCard(seatNum);
            //删除那张空牌
            if (go != null)
            {
                for (int i = 0; i < go.Length; i++)
                {
                    PoolManager.Unspawn(go[i]);
                }
            }
        }

        /// <summary>
        /// 产生桌面上牌
        /// </summary>
        /// <param name="byValue"></param>
        /// <param name="seatNum"></param>
        public void SpwanTableCard(byte byValue, int seatNum)
        {
            int index = GetPlayerIndex(seatNum);

            if (index <= -1)
            {
                return;
            }
            //获取对应的预置体的父物体
            Transform trans = null;
            if (ShowCardParent_D[index].childCount < 10)
            {
                trans = ShowCardParent_D[index];
            }
            else if (ShowCardParent_U[index].childCount < 10)
            {
                trans = ShowCardParent_U[index];
            }
            else
            {
                trans = ShowCardParent_3[index];
            }

            GameObject obj = null;
            if (index == 0 || index == 2)
            {
                obj = PoolManager.Spawn("Game/Ma/", "showVCardPre");
            }
            else
            {
                obj = PoolManager.Spawn("Game/Ma/", "showHCardPre");
            }

            obj.transform.SetParent(trans);

            //对数组第二个做特殊处理
            if (index == 1 || index == 2)
            {
                obj.transform.SetAsFirstSibling();
            }

            obj.transform.localPosition = new Vector3(obj.transform.position.x, obj.transform.position.y, Vector3.zero.z);
            obj.transform.localScale = new Vector3(1, 1, 1);

            obj.transform.localEulerAngles = Vector3.zero;
            if (index == 0)
            {
                obj.transform.Find("Image/num").transform.localEulerAngles = new Vector3(0, 0, 0f);
            }
            else if (index == 1)
            {
                obj.transform.Find("Image/num").transform.localEulerAngles = new Vector3(0, 0, 90f);
            }
            else if (index == 2)
            {
                obj.transform.Find("Image/num").transform.localEulerAngles = new Vector3(0, 0, 180f);
            }
            else if (index == 3)
            {
                obj.transform.Find("Image/num").transform.localEulerAngles = new Vector3(0, 0, -90f);
            }

            //对于预置体进行图片赋值
            obj.transform.Find("Image/num").GetComponent<Image>().sprite = UpdateCardValue(byValue);

            //对刚出的牌标示指示符
            if (LastTableCard != null)
            {
                LastTableCard.GetComponent<Mahjong>().point.gameObject.SetActive(false);
            }
            obj.GetComponent<Mahjong>().point.gameObject.SetActive(true);
            LastTableCard = obj;
        }

        //删除玩家桌面上的牌
        public void DelTabelCard()
        {
            if (LastTableCard != null)
            {
                PoolManager.Unspawn(LastTableCard);
            }
        }

        /// <summary>
        /// 及时分数更新
        /// </summary>
        /// <param name="index"></param>
        /// <param name="score"></param>
        public void SpwanSpeaiclScoreRemind(int index, int score)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Game/GameResult/SpecialTypeScoreNotice"));
            go.transform.SetParent(specialScorePos[index].transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            go.transform.localEulerAngles = new Vector2(0.5f, 1f);
            go.transform.GetComponent<CanvasGroup>().alpha = 1;
            SpecialTypeScoreNotice notice = go.GetComponent<SpecialTypeScoreNotice>();
            notice.GetValue(score);
        }
        #endregion

        #region 麻将界面的更新

        //更新头像
        public void UpdateHeadImage()
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            for (int i = 0; i < HeadImage.Length; i++)
            {
                int index = GetPlayerIndex(i + 1);
                if (i == 0)
                {
                    OpenRoomer[index].gameObject.SetActive(true);
                }
                else
                {
                    OpenRoomer[index].gameObject.SetActive(false);
                }
                anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(HeadImage[index], pbd.sHeadUrl[i]);
            }
        }

        /// <summary>
        /// 更新玩家的界面
        /// </summary>
        public void UpdatePanel()
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            for (int i = 0; i < NameNick.Length; i++)
            {
                //更新玩家昵称
                NameNick[i].text = pbd.sName[i];
                //更新玩家分数
                PlayerScore[i].text = pbd.iPoint[i].ToString();
                //更新玩家房间号，局数
                RoomId.text = pbd.sRoomId;
                GameNum.text = pbd.iCurrentGameNum;
            }
        }

        //更新玩家的进度
        public void UpdatePrograssing()
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            if (pbd.iDoneDealNum > pbd.iAllDealNum)
            {
                pbd.iDoneDealNum = pbd.iAllDealNum;
            }
            GamePrograssing.text = "进度：" + pbd.iDoneDealNum + "/" + pbd.iAllDealNum;
            PlaySpeed.text = iPlaySpeed + "倍速";
            LeftCardNum.text = "剩牌：" + pbd.iLeftCardNum.ToString();
        }

        /// <summary>
        /// 更新显示玩家的出牌方位信息
        /// </summary>
        /// <param name="index"></param>
        public void ShowWitchPlayerPlay(int index)
        {
            for (int i = 0; i < 4; i++)
            {
                PlayerRemindDirec.transform.GetChild(i).gameObject.SetActive(false);
                _headAnim[i].GetComponent<SpriteRenderer>().enabled = false;
            }
            _headAnim[index].GetComponent<SpriteRenderer>().enabled = true;
            PlayerRemindDirec.transform.GetChild(index).gameObject.SetActive(true);
        }

        /// <summary>
        /// 显示玩家吃碰杠胡通知消息
        /// </summary>
        /// <param name="seatNum">玩家座位号</param>
        /// <param name="SpecialType">吃碰杠胡的类型，1吃2碰3杠4胡5十三幺吃6十三幺抢</param>
        /// <param name="status">1表示打开通知界面，2表示关闭该玩家的通知界面</param>
        public void ShowSpecialTileNotice(byte seatNum, byte[] SpecialType, int status)
        {
            //获取玩家的数组下标
            int index = GetPlayerIndex(seatNum);

            //关闭通知界面
            if (status == 2)
            {
                SpeciclNotice[index].SetActive(false);
                return;
            }
            else
            {
                SpeciclNotice[index].SetActive(true);
            }



            //显示玩家的具体信息
            for (int i = 0; i < 7; i++)
            {
                if (SpecialType[i] == 1)
                {
                    SpeciclNotice[index].transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    SpeciclNotice[index].transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                }
            }
        }


        /// <summary>
        /// 显示玩家的点击动画
        /// </summary>
        /// <returns></returns>
        public void DelayShowPlayerClickAnimation_(int seatNum, int type, string message)
        {
            StartCoroutine(DelayShowPlayerClickAnimation(seatNum, type, message));
        }

        /// <summary>
        /// 显示玩家的点击动画
        /// </summary>
        /// <returns></returns>
        IEnumerator DelayShowPlayerClickAnimation(int seatNum, int type, string message)
        {
            yield return new WaitForSeconds(0.5f);

            Debug.LogError("DelayShowPlayerClickAnimation");
            //获取玩家的数组下标
            int index = GetPlayerIndex(seatNum);

            //显示关闭所有玩家手势
            for (int i = 0; i < SpeciclNotice[index].transform.childCount; i++)
            {
                SpeciclNotice[index].transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }

            Transform trans = SpeciclNotice[index].transform.GetChild(type).GetChild(1);
            trans.localScale = Vector3.one;
            trans.GetComponent<DOTweenAnimation>().DORestart(true);
            trans.gameObject.SetActive(true);

            //延迟显示碰杠的特殊操作
            StartCoroutine(DelayShowSpecial_ting(message));
        }


        /// <summary>
        /// 延迟显示碰杠的点击动画
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        IEnumerator DelayShowSpecial_ting(string message)
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            yield return new WaitForSeconds(1f);
            DelayOrderByPlayBack_Ie(1f, pbd.iDoneDealNum);
        }

        /// <summary>
        /// 显示玩家的点击手势
        /// </summary>
        /// <param name="seatNum"></param>
        /// <param name="type"></param>
        public void ShowPlayerClickAnimation(int seatNum, int type, string message)
        {
            //获取玩家的数组下标
            int index = GetPlayerIndex(seatNum);

            //显示关闭所有玩家手势
            for (int i = 0; i < SpeciclNotice[index].transform.childCount; i++)
            {
                SpeciclNotice[index].transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }

            Transform trans = SpeciclNotice[index].transform.GetChild(type).GetChild(1);
            //trans.GetComponent<DOTweenAnimation>().isRelative = true;
            trans.localScale = Vector3.one;
            trans.GetComponent<DOTweenAnimation>().DORestart(true);
            trans.gameObject.SetActive(true);

            //延迟显示碰杠的特殊操作
            StartCoroutine(DelayShowSpecial(message));
        }

        /// <summary>
        /// 延迟显示碰杠的点击动画
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        IEnumerator DelayShowSpecial(string message)
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            yield return new WaitForSeconds(1f);
            pbd.HandleSpecialTileRes(message);
        }

        /// <summary>
        /// 关闭玩家所有的提示
        /// </summary>
        public void CloseAllPlayerSpecialRemind()
        {
            for (int i = 0; i < 4; i++)
            {
                SpeciclNotice[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 显示玩家能跑能下的界面
        /// </summary>
        /// <param name="type">类型1能下，2能跑</param>
        public void ShowCanDownRu(int type)
        {
            //显示界面
            CanDownRu.SetActive(true);

            if (type == 1)
            {
                CanDownRu.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                CanDownRu.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                CanDownRu.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
                CanDownRu.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            }
        }


        /// <summary>
        /// 显示玩家能跑能下的界面
        /// </summary>
        /// <param name="type">类型1能下，2能跑</param>
        public void ShowChoiceCanDownRu(int type)
        {
            Transform trans = CanDownRu.transform.GetChild(1).GetChild(2 - type).GetChild(0).transform;
            trans.localScale = Vector3.one * 1.5f;
            trans.GetComponent<DOTweenAnimation>().DORestart(true);
            trans.gameObject.SetActive(true);

            //延迟一秒之后，关闭手势
            StartCoroutine(AnimationComplete());

        }

        /// <summary>
        /// 点击动画之后操作
        /// </summary>
        public IEnumerator AnimationComplete()
        {
            yield return new WaitForSeconds(1f);
            CanDownRu.gameObject.SetActive(false);
            DelayOrderByPlayBack_Ie(0.5f, MahjongLobby_AH.GameData.Instance.PlayBackData.iDoneDealNum);
        }
        #endregion

        #region 按钮方法处理
        /// <summary>
        /// 点击加速按钮
        /// </summary>
        public void BtnAcceSpeed()
        {
            iPlaySpeed *= 2;
            if (iPlaySpeed > 4)
            {
                iPlaySpeed = 1;
            }
            Time.timeScale = iPlaySpeed;
            UpdatePrograssing();
        }

        /// <summary>
        /// 点击暂停按钮
        /// </summary>
        public void BtnPause()
        {
            if (isPause)
            {
                playBtn.sprite = playStatus[1];
                isPause = false;
                //if(isFinishedAction)
                //{
                //    DelayOrderByPlayBack_Ie(0, MahjongLobby_AH.GameData.Instance.PlayBackData.iDoneDealNum);                                
                //}
            }
            else
            {
                playBtn.sprite = playStatus[0];
                isPause = true;
            }
            Time.timeScale = iPlaySpeed;
        }

        /// <summary>
        /// 点击重播按钮
        /// </summary>
        public void BtnRePlay()
        {
            if (!isClickClose)
            {
                Debug.Log("================重播按钮");
                return;
            }
            isPause = true;
            InitPanel();
            UpdatePanel();
            UpdatePrograssing();
            Debug.Log("iDoneDealNum:" + MahjongLobby_AH.GameData.Instance.PlayBackData.iDoneDealNum + ",播放速度:" + Time.timeScale);
            DelayOrderByPlayBack_Ie(0, 0);
            isClickClose = false;

        }

        /// <summary>
        /// 点击停止播放按钮
        /// </summary>
        public void BtnStopPlay()
        {
            //预加载游戏场景
            SceneManager_anhui.Instance.PreloadScene(
                ESCENE.MAHJONG_GAME_GENERAL);
            Time.timeScale = 1;
            //销毁当前场景
            UnityEngine.SceneManagement.SceneManager.UnloadScene (4);
            PlayBack_1.PlayBackData.isComePlayBack = false;
        }
        /// <summary>
        /// 延迟显示特效
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="index"></param>
        /// <param name="type_"></param>
        public void DelaySpwanSpeaiclTypeRemind(float t, byte[] msgs)
        {
            // Debug.Log("对句结束timer： " + timer + "type_" + type_);
            StartCoroutine(DelaySpwanSpeaiclTypeRemind_(t, msgs));
        }

        IEnumerator DelaySpwanSpeaiclTypeRemind_(float t, byte[] msgs)
        {
            Debug.LogWarning("对句结束timer： " + timer);
            yield return new WaitForSeconds(t);
            _gHongzhongZhongma.SetActive(false);
            Instance.gameResultNoticePanel.SpwanGameReult_Round(msgs);
        }
        public bool DealHongZhongZhongma(byte[] value)
        {
            bool isFanMa = false;
            // Debug.LogError("获得种马"+value.Length  );
            for (int i = 1; i < 7; i++)
            {
                Debug.LogWarning("翻码结果;" + value[i].ToString("x2"));

                switch (value[i] >> 4)
                {
                    case 1:
                        _gHongzhongZhongma.transform.GetChild(i - 1).GetChild(0).GetComponent<Image>().sprite = sWan[(value[i] & 0x0f) - 1];
                        break;
                    case 2:
                        _gHongzhongZhongma.transform.GetChild(i - 1).GetChild(0).GetComponent<Image>().sprite = sTong[(value[i] & 0x0f) - 1];
                        break;
                    case 3:
                        _gHongzhongZhongma.transform.GetChild(i - 1).GetChild(0).GetComponent<Image>().sprite = sTiao[(value[i] & 0x0f) - 1];
                        break;
                    case 4:
                        _gHongzhongZhongma.transform.GetChild(i - 1).GetChild(0).GetComponent<Image>().sprite = sFeng[(value[i] & 0x0f) - 1];
                        break;
                    case 5:
                        _gHongzhongZhongma.transform.GetChild(i - 1).GetChild(0).GetComponent<Image>().sprite = sJian[(value[i] & 0x0f) - 1];
                        break;
                    default:
                        break;
                }
                if (value[i] > 0)
                {
                    isFanMa = true;//显示翻码
                    _gHongzhongZhongma.transform.GetChild(i - 1).gameObject.SetActive(true);
                }
                else
                {
                    _gHongzhongZhongma.transform.GetChild(i - 1).gameObject.SetActive(false);
                }
            }
            _gHongzhongZhongma.SetActive(isFanMa);
            return isFanMa;

        }
        /// <summary>
        /// 点击玩法规则
        /// </summary>
        public void BtnRule()
        {
            PlayBackData pbd = MahjongLobby_AH.GameData.Instance.PlayBackData;
            if (pbd.iPlayBackVersion == pbd.iPbVersion_Old)
            {
                playBackGameRulePanel.gameObject.SetActive(false);
                playBackGameRulePanel_2.gameObject.SetActive(true);
                playBackGameRulePanel_2.AnslyGameRuleParam(MahjongLobby_AH.GameData.Instance.PlayBackData.iMethodId);
            }
            else if (pbd.iPlayBackVersion == pbd.iPbVersion_New)
            {
                playBackGameRulePanel_2.gameObject.SetActive(false);
                playBackGameRulePanel.gameObject.SetActive(true);
                playBackGameRulePanel.AnslyGameRuleParam(MahjongLobby_AH.GameData.Instance.PlayBackData.iMethodId);
            }
        }

        /// <summary>
        /// 更新花牌
        /// </summary>        
        /// <param name="index">玩家下标</param>
        /// <param name="count">花牌数量</param>
        public void UpdateFlower(int index, int count)
        {
            for (int i = 0; i < 4; i++)
            {
                Flower[i].SetActive(true);
            }
            MahjongLobby_AH.GameData.Instance.PlayBackData.FlowerCpount[index] += count;
            Flower[index].transform.GetChild(0).GetComponent<Text>().text = "X" + MahjongLobby_AH.GameData.Instance.PlayBackData.FlowerCpount[index];
        }
        #endregion

    }
}
