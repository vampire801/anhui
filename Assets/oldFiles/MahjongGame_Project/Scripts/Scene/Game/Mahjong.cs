using UnityEngine;
using System.Collections.Generic;
using System;
using MahjongGame_AH.Network;
using MahjongGame_AH.Network.Message;
using MahjongGame_AH;
using MahjongGame_AH.Data;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MahjongGame_AH.GameSystem.SubSystem;
using XLua;
using anhui;
[Hotfix]
[LuaCallCSharp]
public class Mahjong : MonoBehaviour//, IBeginDragHandler, IEndDragHandler
{
    public const string MsgKai = "Messager.Kai";
    public const string MsgGuang = "Messager.Guan";
    public int iMahId;  //自己手牌的唯一id
    public byte bMahjongValue;  //存储麻将的值
    public Image image;  //显示麻将的图片
    public Image _imageFinger;//
    public float length = 86f;  //麻将的宽度
    public bool isDealCard; //是否摸得手牌    
    public bool isFlyingAnimation; //是否正在执行分型动画，不参与排序
    float ratio = 0.3f;//上升高度比/牌高度
    public GameObject ClickPanel;//可以点击的面板

    /// <summary>
    /// 初始位置
    /// </summary>
    Vector3 v3InitialLocalPos;
    Vector3 v3InitialWorldPos;
    /// <summary>
    /// 上一次位置
    /// </summary>
    Vector3 v3LastLocalPos;
    Vector3 v3LastWorldPos;
    /// <summary>
    /// 出牌分割线
    /// </summary>
    private float fSeparation;
    /// <summary>
    /// 0-未选中状态,1-选中状态，2拖动状态
    /// </summary>
    public int iState;
    /// <summary>
    /// 是否在拖动状态
    /// </summary>
    public bool isDrag;
    int number;
    int numbers;

    public Transform point;  //提示标志

    void OnEnable()
    {
        // Debug.LogError("A"+number++ +"+++++++++++"+v3InitialLocalPos);
        // v3InitialLocalPos = transform.localPosition;
        // fSeparation = transform.localPosition.y + transform.GetChild(0).GetComponent<RectTransform>().rect.height * 0.5f;

    }
    void Start()
    {
        v3InitialLocalPos = transform.parent.GetChild(0).localPosition;
        v3InitialWorldPos = transform.parent.GetChild(0).position;
        fSeparation = transform.localPosition.y + transform.GetChild(0).GetComponent<RectTransform>().rect.height;
    }

    public void OnBeginDrag()
    {
        if (!transform.GetComponent<Button>().interactable)
        {
            return;
        }
        if (GameData.Instance.PlayerPlayingPanelData.isCanHandCard)//到我出牌才可以拖动
        {
            if (!MahjongManger.Instance.hasCardDarg)
            {
                MahjongManger.Instance.hasCardDarg = true;
                v3LastLocalPos = transform.localPosition;
                v3LastWorldPos = transform.position;
                if (iState == 1)
                {
                    v3LastLocalPos = new Vector3(transform.localPosition.x, v3InitialLocalPos.y, transform.localPosition.z);
                    v3LastWorldPos = new Vector3(transform.position.x, v3InitialWorldPos.y, transform.position.z);
                }
                iState = 2;
                Mahjong[] allcards = transform.parent.GetComponentsInChildren<Mahjong>(false);
                //遍历所有牌高度
                for (int i = 0; i < allcards.Length; i++)
                {
                    if (allcards[i].iState == 1 && allcards[i] != this)
                    {
                        allcards[i].iState = 0;
                        allcards[i].MoveDown(allcards[i].bMahjongValue);
                    }
                }
                isDrag = true;
                ///Debug.LogError("开始拖动");
            }
        }

    }
    void FixedUpdate()
    {
        if (isDrag)
        {
            Vector3 v3 = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
            if (transform.localPosition.y > fSeparation)
            {
                transform.position = v3;
            }
            else
            {
                transform.position = new Vector3(v3LastWorldPos.x, v3.y, transform.position.z);
            }
        }
    }

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (isDrag)
    //    {
    //        isDrag = false;
    //       // transform.localPosition = v3InitialLocalPos;
    //        Destroy(transform.GetComponent<Rigidbody2D>());
    //        iState = 0;
    //        //ChangeChoiceCard(other.transform);
    //    }

    //}


    //void OnTriggerExit2D(Collider2D other)
    //{

    //}
    //void OnTriggerStay2D(Collider2D other)
    //{
    //    //if (isDrag)
    //    //{

    //    //    //if (Vector3.Distance(other.transform.localPosition, transform.localPosition) < transform.GetChild(0).GetComponent<RectTransform>().rect.width * 0.5f)//
    //    //    //{
    //    //    //    Debug.LogError("Trigger触发   `       ");
    //    //    //}

    //    //}
    //}
    public void OnEndDrag()
    {
        if (GameData.Instance.PlayerPlayingPanelData.isCanHandCard)//到我出牌才可以拖动
        {
            // Destroy(gameObject.GetComponent<Rigidbody2D>());
            if (iState == 2)
            {
                Debug.LogWarning("=====================2:" + transform.localPosition);
                MahjongManger.Instance.hasCardDarg = false;
                iState = 0;
                isDrag = false;
                if (transform.localPosition.y > fSeparation)
                {
                    PutCard(1);
                }
                else
                {
                    transform.localPosition = v3LastLocalPos;
                    Debug.LogWarning("=====================3:" + transform.localPosition);
                }
            }
        }
    }

    //void ChangeChoiceCard(Transform other)
    //{
    //    Debug.LogError("交换选择");
    //    #region 方案1
    //    other.localPosition = transform.localPosition;
    //    other.GetComponent<Mahjong>().isDrag = true;
    //    other.GetComponent<Mahjong>().iState = 2;

    //    isDrag = false;
    //    iState = 0;
    //   // transform.localPosition = v3InitialLocalPos;
    //    #endregion 方案1

    //    #region 方案二

    //    #endregion 方案二

    //}
    /// <summary>
    /// 初始化
    /// </summary>
    void Init()
    {
        transform.localPosition = Vector3.zero;
        bMahjongValue = 0;
        isDealCard = false;
        image = null;
    }

    public void OnClick()
    {
        if (isDrag)
        {
            return;
        }
        if (MahjongManger.Instance.hasCardDarg)
        {
            return;
        }
        // Debug.LogError("点击Onclick");
        if (iState == 1)
        {
            iState = 0;
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            if (pppd.isCanHandCard)
            {
                PutCard(1);
            }
            else
            {
                MoveDown(bMahjongValue);
            }

        }
        else if (iState == 0)
        {
            iState = 1;
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.pick_card, false, false);
            for (int i = 0; i < transform.parent.GetComponentsInChildren<Mahjong>(false).Length; i++)
            {
                Mahjong mj = transform.parent.GetComponentsInChildren<Mahjong>(false)[i];
                if (mj != this)
                {
                    if (mj.iState == 1)
                    {
                        // Debug.LogError("有牌上去了");
                        mj.iState = 0;
                        mj.MoveDown(mj.bMahjongValue);
                    }
                }
            }
            MoveUp();
        }

    }
    public void MoveUp()
    {
        byte num = 0;
        transform.localPosition = new Vector3(transform.localPosition.x, v3InitialLocalPos.y + transform.GetChild(0).GetComponent<RectTransform>().rect.height * ratio, transform.localPosition.z);
        showM[] show = transform.parent.parent.parent.GetComponentsInChildren<showM>();
        for (int i = 0; i < show.Length; i++)
        {
            if (show[i].GetComponent<Mahjong>().bMahjongValue == bMahjongValue)
            {
                show[i].Play();
                for (int j = 0; j < show[i].m.Length; j++)
                {
                    num++;
                    show[i].m[j].enabled = true;
                    show[i].isOpen = true;
                }
            }
        }
        num = 0;
        if (!GameData.Instance.PlayerPlayingPanelData.isCanHandCard)
        {
            return;
        }

        //如果该牌可以听牌，点击之后，显示可以胡的牌
        if (MahjongHelper.Instance.mahjongTing.ContainsKey(bMahjongValue) && MahjongHelper.Instance.mahjongTing[bMahjongValue].Length > 0)
        {
            UIMainView.Instance.PlayerPlayingPanel.SpwanTingShow(MahjongHelper.Instance.mahjongTing[bMahjongValue]);
        }
        else
        {
            UIMainView.Instance.PlayerPlayingPanel.TingShow.SetActive(false);
        }
    }
    public void MoveDown(byte Value)
    {
        #region 关闭提示
        showM[] show = transform.parent.parent.parent.GetComponentsInChildren<showM>();
        // Debug.LogError("down" + show.Length);
        for (int i = 0; i < show.Length; i++)
        {
            show[i].Stop();
            if (show[i].isOpen)
            {
                // Debug.LogError(show.Length);
                for (int j = 0; j < show[i].m.Length; j++)
                {
                    show[i].m[j].enabled = false;
                }
                show[i].isOpen = false;
            }
        }
        #endregion 关闭提示

        transform.localPosition = new Vector3(transform.localPosition.x, v3InitialLocalPos.y, transform.localPosition.z);
    }

    /// <summary>
    /// 打出牌之后的操作
    /// </summary>
    /// <param name="status">1表示正常打出牌，2表示碰杠的处理</param>
    public void PutCard(int status)
    {
        #region 关闭提示
        showM[] show = transform.parent.parent.parent.GetComponentsInChildren<showM>();
        //Debug.LogError("down" + show.Length);
        for (int i = 0; i < show.Length; i++)
        {
            if (show[i].isOpen)
            {
                show[i].Stop();
                for (int j = 0; j < show[i].m.Length; j++)
                {
                    show[i].m[j].enabled = false;
                }
                show[i].isOpen = false;
            }
        }
        #endregion 关闭提示
        Vector3 firstPos = Vector3.zero;  //第一张麻将的位置
        PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
        pppd.isCanHandCard = false;
        if (status == 1 && pppd.iPlayerHostStatus == 0)
        {
            //发送出牌请求
            byte value = Convert.ToByte(bMahjongValue.ToString("x8"));
            NetMsg.ClientDiscardTileReq msg = new NetMsg.ClientDiscardTileReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.byDrawSeaBottom = bMahjongValue;
            MahjongManger.Instance.TingFirstValue = msg.byDrawSeaBottom;
            NetworkMgr.Instance.GameServer.SendDiscardTileReq(msg);
            MahjongManger.Instance.isEndPutAnimation = false;
            UIMainView.Instance.PlayerPlayingPanel.TingShow.SetActive(false);
            MahjongManger.Instance.HideTingLogo();
        }


        //隐藏该麻将
        //transform.localScale = Vector3.one * 0.001f;
        iState = 0;
        //获取第一张麻将的位置
        //firstPos = MahjongManger.Instance.GetFirstMahjongPos(0);
        firstPos = MahjongManger.Instance.FirstPos;
        //获取打出牌的下标
        for (int i = 0; i < pppd.usersCardsInfo[0].listCurrentCards.Count; i++)
        {
            if (bMahjongValue == pppd.usersCardsInfo[0].listCurrentCards[i].cardNum && iMahId == pppd.usersCardsInfo[0].listCurrentCards[i].MahId)
            {
                pppd.usersCardsInfo[0].listCurrentCards.RemoveAt(i);
                MahjongManger.Instance.iputCardIndex = i;
                break;
            }
        }


        int deadcardValue = -1;
        int mahid = -1;
        if (MahjongManger.Instance.PlayerDealHandCrad != null)
        {
            deadcardValue = MahjongManger.Instance.PlayerDealHandCrad.bMahjongValue;
            mahid = MahjongManger.Instance.PlayerDealHandCrad.iMahId;
        }
        //判断打出的牌是不是摸到的手牌
        if (isDealCard)
        {
            isDealCard = false;
        }
        else
        {
            //对list进行排序，获取手牌的下标
            pppd.CurrentCradSort(0);
            pppd.CurrentCradSort(0);
            //处理玩家吃碰之后的打牌操作
            if (deadcardValue == -1 && mahid == -1)
            {
                if (pppd.isSpwanSpecialCard)
                {
                    pppd.isSpwanSpecialCard = false;
                    MahjongManger.Instance.insertCardIndex = pppd.usersCardsInfo[0].listCurrentCards.Count;
                }
                else
                {
                    MahjongManger.Instance.insertCardIndex = -1;
                    MahjongManger.Instance.iputCardIndex -= 1;
                }

            }
            else
            {
                for (int i = 0; i < pppd.usersCardsInfo[0].listCurrentCards.Count; i++)
                {
                    if (deadcardValue == pppd.usersCardsInfo[0].listCurrentCards[i].cardNum && mahid == pppd.usersCardsInfo[0].listCurrentCards[i].MahId)
                    {
                        MahjongManger.Instance.insertCardIndex = i;
                        break;
                    }
                }
            }

            Mahjong[] mah = new Mahjong[Mathf.Abs(MahjongManger.Instance.insertCardIndex - MahjongManger.Instance.iputCardIndex)];


            if (Mathf.Abs(MahjongManger.Instance.insertCardIndex - MahjongManger.Instance.iputCardIndex) > 0)
            {
                mah = MahjongManger.Instance.GetWillMoveCard();
            }

            //调用移动方法插入手牌
            if (mah.Length > 0)
            {
                for (int i = 0; i < mah.Length; i++)
                {
                    mah[i].MoveSelf(MahjongManger.Instance.MoveStatus, 1);
                }
            }

            //Debug.LogError("这张麻将的信息，deadcardValue:" + deadcardValue + ",mahid:" + mahid);

            if (deadcardValue != -1 && mahid != -1)
            {
                MahjongManger.Instance.flyMahjongIndexPos(firstPos);
            }
            else
            {
                MahjongManger.Instance.GetFirstMahjongPos(0);
            }

        }

        int index = pppd.GetOtherPlayerShowPos(pppd.bySeatNum + 1) - 1;
        if (status == 1)
        {
            byte value = bMahjongValue;
            //if (PlayerPrefs.HasKey("TingOneCard"))
            //{
            //    if (PlayerPrefs.GetInt("TingOneCard") == 1 && pppd.playingMethodConf.byDiscardSeeReadHandTile == 0)
            //    {
            //        value = 255;
            //    }
            //}            
            pppd.iSpwanCardNum--;
            //先在同一个位置产生一个对应的预置体，然后移动
            transform.localScale = Vector3.zero;
            GameObject go = Instantiate(Resources.Load<GameObject>("Game/Ma/TabelBigCard"));
            go.transform.SetParent(UIMainView.Instance.PlayerPlayingPanel._Cards[0].transform.Find("currentGroup"));
            go.name = "TabelBigCard";
            go.transform.localPosition = transform.localPosition;
            go.transform.localScale = Vector3.one * 1.2f;
            go.transform.localEulerAngles = Vector3.zero;
            UIMainView.Instance.PlayerPlayingPanel.ChangeCardNum(go.transform.Find("Image/num").GetComponent<Image>(), value, index);
            go.GetComponent<Mahjong>().bMahjongValue = value;
            go.GetComponent<Mahjong>().PutCardAnimator(pppd.bySeatNum, value, 1);
            PoolManager.Unspawn(gameObject);
        }
        else
        {

        }
        isDealCard = false;
    }


    /// <summary>
    /// 出牌动画
    /// </summary>
    /// <param name="targetPos">牌的目标位置</param>
    /// <param name="mahjongValue">牌面的花色值</param>
    public void PutCardAnimator(int seatnum, int value, int status)
    {
        //直线移动麻将到指定位置
        int index = GameData.Instance.PlayerPlayingPanelData.GetOtherPlayerShowPos(seatnum) - 1;
        Tweener tweener_0 = transform.transform.DOLocalMove(MahjongManger.Instance.TableBigCardPos[index].transform.localPosition, 0.07f);
        tweener_0.SetEase(Ease.Linear);
        tweener_0.OnComplete(() => Oncomplete_0(seatnum, status));
    }



    //第一段移动完成
    void Oncomplete_0(int seatNum, int status)
    {
        int index = GameData.Instance.PlayerPlayingPanelData.GetOtherPlayerShowPos(seatNum) - 1;
        //打开背景图
        transform.GetChild(0).gameObject.SetActive(true);

        //直线移动麻将到指定位置
        Vector3 targetPos = MahjongManger.Instance.TableInisPos[index].transform.localPosition;
        Transform d3 = UIMainView.Instance.PlayerPlayingPanel._Cards[index].Find("showGroup_3");
        Transform up = UIMainView.Instance.PlayerPlayingPanel._Cards[index].Find("showGroup_u");
        Transform down = UIMainView.Instance.PlayerPlayingPanel._Cards[index].Find("showGroup_d");
        if (up.childCount >= 10)//第二行满了
        {
            if (index == 0)
            {
                targetPos += new Vector3((d3.childCount) * 45f, 90, 0);
            }
            else if (index == 1)
            {
                targetPos += new Vector3(-120f, (d3.childCount) * 25f, 0);
            }
            else if (index == 2)
            {
                targetPos += new Vector3((-d3.childCount) * 45f, -120f, 0);
            }
            else
            {
                targetPos += new Vector3(120, -(d3.childCount) * 25f, 0);
            }
        }
        if (down.childCount >= 10)//第一行满了
        {
            //根据位置不同进行微调            
            if (index == 0)
            {
                targetPos += new Vector3((up.childCount) * 45f, 45, 0);
            }
            else if (index == 1)
            {
                targetPos += new Vector3(-60f, (up.childCount) * 25f, 0);
            }
            else if (index == 2)
            {
                targetPos += new Vector3((-up.childCount) * 45f, -60f, 0);
            }
            else
            {
                targetPos += new Vector3(60, -(up.childCount) * 25f, 0);
            }

        }
        else
        {
            if (index == 0)
            {
                targetPos += new Vector3((down.childCount) * 45f, 0, 0);
            }
            else if (index == 1)
            {
                targetPos += new Vector3(0, (down.childCount) * 25f, 0);
            }
            else if (index == 2)
            {
                targetPos += new Vector3(-(down.childCount) * 45f, 0, 0);
            }
            else
            {
                targetPos += new Vector3(0, -(down.childCount) * 25f, 0);
            }

        }

        Tweener tweener_0 = transform.transform.DOLocalMove(targetPos, 0.07f);
        tweener_0.SetDelay(0.4f);
        tweener_0.SetEase(Ease.Linear);
        Tweener tweener_1 = transform.DOScale(Vector3.one * 0.85f, 0.03f);
        tweener_1.SetEase(Ease.Linear);
        tweener_1.SetDelay(0.44f);
        tweener_1.OnComplete(() => Oncomplete_1(seatNum, status));
    }

    //第二段移动完成
    void Oncomplete_1(int seatNum, int starus)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        //产生桌面麻将。删除自己
        UIMainView.Instance.PlayerPlayingPanel.SpwanTableCrad(seatNum, bMahjongValue);

        //  if (!GameData.Instance.PlayerPlayingPanelData.isGangToPeng_Later)
        // {
        //开始接受消息
        NetworkMgr.Instance.GameServer.Unlock();
        //  }

        MahjongManger.Instance.isEndPutAnimation = true;
        SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.card_sound, false, false);

        //Debug.LogError("开始准备删除麻将预置体，starus：" + starus);

        if (starus == 1 || starus == 4)
        {
            Init();

            if (transform.GetComponent<PoolAgent>())
            {
                PoolManager.Unspawn(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }

        if (starus != 3)
        {
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            if (seatNum == pppd.bySeatNum)
            {
                MahjongManger.Instance.iniAnimationStatus++;
                InitMahjongPos();
            }
        }

    }


    /// <summary>
    /// 平移麻将
    /// </summary>
    /// <param name="movestatus">表示麻将的平移状态，1表示左移，2表示右移</param>
    /// <param name="movelength">表示要平移的麻将的个数，乘以麻将的宽度，</param>
    public void MoveSelf(int movestatus, int movelength)
    {
        if (movelength >= 3)
        {
            movelength = 3;
        }

        float distance = movelength * length;
        Tweener tweener;
        //左移
        if (movestatus == 1)
        {
            tweener = transform.DOLocalMoveX(transform.localPosition.x - distance, 0.15f);
        }
        //右移
        else
        {
            tweener = transform.DOLocalMoveX(transform.localPosition.x + distance, 0.15f);
        }
        tweener.SetEase(Ease.Linear);

        PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
        //在这之后处理用户的碰杠之后，处理牌的位置
        if (pppd.isGangToPeng_Put)
        {
            pppd.isGangToPeng_Put = false;
            //pppd.CurrentCradSort(2);
        }
    }


    //移动麻将到指定位置
    public void MoveIndexPos(Vector3 pos)
    {
        if (pos == transform.localPosition)
        {
            pos = MahjongManger.Instance.GetFirstMahjongPos(1);
        }

        isDealCard = false;
        Tweener tweenner = transform.DOLocalMoveY(transform.localPosition.y + 86f, 0.1f);
        tweenner.OnComplete(() => Oncomplete_3(pos));
    }

    void Oncomplete_3(Vector3 pos)
    {
        //  Debug.LogError("3==============================ceshi");
        Tweener tweenner = transform.DOLocalMoveX(pos.x, 0.15f);
        tweenner.OnComplete(() => Oncomplete_4(pos));
    }

    void Oncomplete_4(Vector3 pos)
    {
        //   Debug.LogError("4==============================ceshi");
        Tweener tweenner = transform.DOLocalMoveY(transform.localPosition.y - 86f, 0.1f);
        tweenner.OnComplete(OnComplete_5);
    }


    void OnComplete_5()
    {
        NetworkMgr.Instance.GameServer.Unlock();
        StartCoroutine(Delay_Pong());
    }




    System.Collections.IEnumerator Delay_Pong()
    {
        yield return new WaitForSeconds(0.15f);
        PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;

        MahjongManger.Instance.iniAnimationStatus++;
        InitMahjongPos();

        if (pppd.isGangToPeng_Later)
        {
            pppd.isGangToPeng_Later = false;
            //pppd.CurrentCradSort(2);
            //处理碰杠之后，摸得牌
            pppd.DelayDiscardTileNotice(pppd.DiscardTileNotice);
        }
    }


    //初始化牌的值和位置
    void InitMahjongPos()
    {
        if (MahjongManger.Instance.iniAnimationStatus < 2 && !MahjongManger.Instance.bPongGang)
        {
            return;
        }
        MahjongManger.Instance.bPongGang = false;

        //   Debug.LogError("iniAnimationStatus:================:" + MahjongManger.Instance.iniAnimationStatus);
        MahjongManger.Instance.iniAnimationStatus = 0;
        PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
        pppd.CurrentCradSort(2);
        MahjongManger.Instance.GetFirstMahjongPos(0);
    }



    void OnDisable()
    {
        isDealCard = false;
    }


    //玩家刚出的牌进行提示
    public void RemindJustPutCard()
    {
        if (MahjongManger.Instance.LastTableCard != null)
        {
            MahjongManger.Instance.LastTableCard.GetComponent<Mahjong>().point.gameObject.SetActive(false);
        }
        point.gameObject.SetActive(true);
    }


    #region 回放时出牌动画特殊处理
    /// <summary>
    /// 出牌动画
    /// </summary>
    /// <param name="targetPos">牌的目标位置</param>
    /// <param name="mahjongValue">牌面的花色值</param>
    public void PutCardAnimator_pb(int seatnum, int value)
    {
        //直线移动麻将到指定位置
        int index = PlayBack_1.PlayBackMahjongPanel.Instance.GetPlayerIndex(seatnum);
        Tweener tweener_0 = transform.transform.DOLocalMove(PlayBack_1.PlayBackMahjongPanel.Instance.Big_Pos[index].transform.localPosition, 0.07f);
        tweener_0.SetEase(Ease.Linear);
        tweener_0.OnComplete(() => Oncompletepb_0(seatnum));

        Tweener tweener_1 = transform.DOScale(Vector3.one * 1.2f, 0.02f);
        tweener_1.SetDelay(0.02f);
        tweener_0.SetEase(Ease.Linear);
    }



    //第一段移动完成
    void Oncompletepb_0(int seatNum)
    {
        //打开背景图
        transform.GetChild(0).gameObject.SetActive(true);
        int index = PlayBack_1.PlayBackMahjongPanel.Instance.GetPlayerIndex(seatNum);
        //直线移动麻将到指定位置
        Vector3 targetPos = PlayBack_1.PlayBackMahjongPanel.Instance.Table_Pos[index].transform.localPosition;
        Transform up = PlayBack_1.PlayBackMahjongPanel.Instance.ShowCardParent_U[index];
        Transform down = PlayBack_1.PlayBackMahjongPanel.Instance.ShowCardParent_D[index];
        Transform d3 = PlayBack_1.PlayBackMahjongPanel.Instance.ShowCardParent_3[index];
        if (up.childCount >= 10)//第二行满了
        {
            if (index == 0)
            {
                targetPos += new Vector3((d3.childCount) * 45f, 90, 0);
            }
            else if (index == 1)
            {
                targetPos += new Vector3(-120f, (d3.childCount) * 25f, 0);
            }
            else if (index == 2)
            {
                targetPos += new Vector3((-d3.childCount) * 45f, -120f, 0);
            }
            else
            {
                targetPos += new Vector3(120, -(d3.childCount) * 25f, 0);
            }
        }
        if (down.childCount >= 10)//第一行满了
        {
            //根据位置不同进行微调            
            if (index == 0)
            {
                targetPos += new Vector3((up.childCount) * 45f, 45, 0);
            }
            else if (index == 1)
            {
                targetPos += new Vector3(-60f, (up.childCount) * 25f, 0);
            }
            else if (index == 2)
            {
                targetPos += new Vector3((-up.childCount) * 45f, -60f, 0);
            }
            else
            {
                targetPos += new Vector3(60, -(up.childCount) * 25f, 0);
            }

        }
        else
        {
            if (index == 0)
            {
                targetPos += new Vector3((down.childCount) * 45f, 0, 0);
            }
            else if (index == 1)
            {
                targetPos += new Vector3(0, (down.childCount) * 25f, 0);
            }
            else if (index == 2)
            {
                targetPos += new Vector3(-(down.childCount) * 45f, 0, 0);
            }
            else
            {
                targetPos += new Vector3(0, -(down.childCount) * 25f, 0);
            }
        }

        Tweener tweener_0 = transform.transform.DOLocalMove(targetPos, 0.07f);
        tweener_0.SetDelay(0.4f);
        tweener_0.SetEase(Ease.Linear);
        Tweener tweener_1 = transform.DOScale(Vector3.one * 0.85f, 0.03f);
        tweener_1.SetEase(Ease.Linear);
        tweener_1.SetDelay(0.44f);
        tweener_1.OnComplete(() => Oncompletepb_1(seatNum));
    }

    //第二段移动完成
    void Oncompletepb_1(int seatNum)
    {
        //关闭背景图
        transform.GetChild(0).gameObject.SetActive(false);
        //产生桌面麻将。删除自己
        PlayBack_1.PlayBackMahjongPanel.Instance.SpwanTableCard(bMahjongValue, seatNum);
        PoolManager.Unspawn(gameObject);
    }


    #endregion
}
