using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MahjongGame_AH.Data;

using System;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class MahjongManger : MonoBehaviour
{
    #region 单例
    static MahjongManger instance;
    public static MahjongManger Instance
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

    public GameObject LastTableCard;  //上次玩家打出到桌面上的牌
    public GameObject[] TableInisPos; //四个玩家的桌面显示牌的初始位置
    public GameObject[] TableBigCardPos; //四个大牌的显示位置

    public bool bPongGang;  //碰杠
    public bool hasCardDarg;
    public bool isEndPutAnimation;  //是否已经结束出牌动画的标志位

    public int iputCardIndex = -1; //当前打出的牌的list下标
    public int insertCardIndex = -1; //要插入牌的下标
    public int MoveStatus;  // 移动状态，1表示左移，2表示右移
    public Vector3 FirstPos;  //玩家手中第一张牌的位置
    [HideInInspector]
    public int iniAnimationStatus;

    public Mahjong PlayerDealHandCrad;  //保存玩家的刚摸的手牌
    public byte TingFirstValue;//保存上听时出的第一张牌    出的牌，当牌为0时是胡牌（自摸）  目前针对晋城阳城玩法

    //通过名字查找对应的子物体
    public GameObject FindMahjongByName(string name)
    {
        GameObject go = transform.Find(name).gameObject;
        return go;
    }

    /// <summary>
    /// 初始化麻将的状态
    /// </summary>
    public void initMah()
    {
        Mahjong[] mah = transform.GetComponentsInChildren<Mahjong>();

        for (int i = 0; i < mah.Length; i++)
        {
            mah[i].isDealCard = false;
        }
    }

    /// <summary>
    /// 获得自己的全部手牌
    /// </summary>
    /// <returns></returns>
    public Mahjong[] GetSelfCard()
    {
        //麻将的子物体
        List<Mahjong> mah = new List<Mahjong>();
        mah.Clear();
        Mahjong[] go = null;
        go = transform.GetComponentsInChildren<Mahjong>();
        for (int i = 0; i < go.Length; i++)
        {
            if (go[i].enabled && go[i].name.Contains("current"))
            {
                mah.Add(go[i]);
            }
        }
        return mah.ToArray();
    }

    /// <summary>
    /// 获取指定的手牌
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public Mahjong GetPointCard(byte value)
    {
        Mahjong mah = null;
        //Debug.LogError("value:" + value);
        Mahjong[] mahjong = GetSelfCard();
        for (int i = 0; i < mahjong.Length; i++)
        {
            //Debug.LogError("mahjong:" + mahjong[i].bMahjongValue);
            if (mahjong[i].bMahjongValue == value)
            {
                mah = mahjong[i];
              //  Debug.LogError(mah.name);
                break;
            }
        }

        return mah;
    }


    /// <summary>
    /// 获取刚摸的手牌
    /// </summary>
    /// <returns></returns>
    public Mahjong GetDealCard()
    {
        Mahjong go = null;
        Mahjong[] mah = GetSelfCard();
        for (int i = 0; i < mah.Length; i++)
        {
            if (mah[i].isDealCard)
            {
                go = mah[i];
                break;
            }
        }
        return go;
    }



    /// <summary>
    /// 获取将要移动的麻将
    /// </summary>
    /// <returns></returns>
    public Mahjong[] GetWillMoveCard()
    {
        PlayerPlayingPanelData pppd = MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData;
        List<Mahjong> go_ = new List<Mahjong>();
        Mahjong[] mah = GetSelfCard();


        //如果要插入到牌在打出的牌的后边
        if (insertCardIndex > iputCardIndex)
        {
            MoveStatus = 1;
            for (int i = iputCardIndex; i < insertCardIndex; i++)
            {
                if (i > pppd.usersCardsInfo[0].listCurrentCards.Count - 1)
                {
                    continue;
                }
                for (int j = 0; j < mah.Length; j++)
                {
                    if (pppd.usersCardsInfo[0].listCurrentCards[i].cardNum == mah[j].bMahjongValue && pppd.usersCardsInfo[0].listCurrentCards[i].MahId == mah[j].iMahId)
                    {
                        go_.Add(mah[j]);
                        continue;
                    }
                }
            }
        }
        else if (insertCardIndex < iputCardIndex)
        {
            MoveStatus = 2;
            for (int i = insertCardIndex + 1; i <= iputCardIndex; i++)
            {
                if (i > pppd.usersCardsInfo[0].listCurrentCards.Count - 1)
                {
                    continue;
                }
                for (int j = 0; j < mah.Length; j++)
                {
                    if (pppd.usersCardsInfo[0].listCurrentCards[i].cardNum == mah[j].bMahjongValue && pppd.usersCardsInfo[0].listCurrentCards[i].MahId == mah[j].iMahId)
                    {
                        go_.Add(mah[j]);
                    }
                }
            }
        }
        return go_.ToArray();
    }

    /// <summary>
    /// 获取第一个麻将的位置
    /// </summary>
    /// <param name="status">0表示正常状态，1表示插入到第一个位置</param>
    /// <returns></returns>
    public Vector3 GetFirstMahjongPos(int status)
    {
        PlayerPlayingPanelData pppd = MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData;
        Vector3 vec = new Vector3(-557f, 0, 0);
        float length = 0;
        if (pppd.usersCardsInfo[0].listSpecialCards.Count > 0)
        {
            if (pppd.usersCardsInfo[0].listSpecialCards[0].type == 5 || pppd.usersCardsInfo[0].listSpecialCards[0].type == 6)
            {
                length = 85f;
            }
            else
            {
                length = 247f;
            }
        }

        vec += new Vector3(pppd.usersCardsInfo[0].listSpecialCards.Count * length, 0, 0);

        //Mahjong[] mah = GetSelfCard();

        //if (mah.Length == 0)
        //{
        //    return new Vector3(-557f, 0, 0);
        //}                

        ////通过冒泡排序找到x轴最小的那个麻将，就是第一个麻将
        //for(int i=0;i<mah.Length;i++)
        //{
        //    //Debug.LogError("pos:" + mah[i].transform.localPosition + ",value:" + mah[i].bMahjongValue);
        //    if(vec.x>=mah[i].transform.localPosition.x)
        //    {
        //        vec = mah[i].transform.localPosition;
        //    }
        //}


        FirstPos = vec;

        return vec;
    }


    /// <summary>
    /// 移动麻将到指定的位置
    /// </summary>
    public void flyMahjongIndexPos(Vector3 firstPos)
    {
        //Debug.LogError("0==============================ceshi");
        StartCoroutine(fly(firstPos));
    }

    IEnumerator fly(Vector3 firstPos)
    {
        //Debug.LogError("1==============================ceshi");
        yield return new WaitForSeconds(0.2f);
        int index = 0;
        index = insertCardIndex;
        Vector3 pos = Vector3.zero;
        //获取将要飞的位置
        pos = firstPos + new Vector3(index * 85f, 0, 0);

        //Debug.LogError("牌将要飞的位置:" + pos + ",index:" + index);

        ////处理第一张的问题
        //if (insertCardIndex == 0)
        //{
        //    pos = firstPos;
        //}
        PlayerDealHandCrad.MoveIndexPos(pos);
    }

    /// <summary>
    /// 找到同桌其他玩家要删除的打出的手牌
    /// </summary>
    /// <param name="parent">要找的玩家的对应的父物体</param>    
    public Mahjong GetOtherPlayerHandCard(Transform parent, int index)
    {
        Mahjong mah = null;
        if (parent == null)
        {
            return null;
        }
        PlayerPlayingPanelData pppd = MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData;
        Mahjong[] maharray = parent.GetComponentsInChildren<Mahjong>();
        string name = "";
        if (index == 1 || index == 3)
        {
            name = PlayerPlayingPanelData.currentHCardPre;
        }
        else
        {
            name = PlayerPlayingPanelData.currentUCardPre;
        }
        //从最后一个寻找要删除的刚才摸得手牌
        for (int i = maharray.Length - 1; i >= 0; i--)
        {
            if (maharray[i].name == name && maharray[i].enabled)
            {
                mah = maharray[i];
                break;
            }
        }
        return mah;
    }


    //隐藏所有手牌的听的指示
    public void HideTingLogo()
    {
        Mahjong[] mahjong = GetSelfCard();
        //Debug.LogError("置为false：===================================================1");
        for (int i = 0; i < mahjong.Length; i++)
        {
            mahjong[i].transform.Find("Ting").gameObject.SetActive(false);
        }
    }

}
