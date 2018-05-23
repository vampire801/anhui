using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using MahjongLobby_AH;
using MahjongLobby_AH.LobbySystem.SubSystem;
using System.Collections.Generic;
using MahjongLobby_AH.Network;
using MahjongLobby_AH.Network.Message;
using System;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class RedPageShowPanel : MonoBehaviour
{
    #region 实例化
    static RedPageShowPanel instance;
    public static RedPageShowPanel Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        instance = this;

        m_SpritePanelShare = Resources.Load<Sprite>("Lobby/RedPageImage/pic_0099_3");
        m_SpritePanelOpen = Resources.Load<Sprite>("Lobby/RedPageImage/pic_0099_2");
    }
    #endregion

    /// <summary>
    /// 红包或者按钮之类的父物体
    /// </summary>
    /// 0 红包外部图片 0开启或者分享图片
    /// 1红包的金额  后面的金额的类型 目前有六种
    /// 2红包名称  修改成图片
    /// 3分享按钮
    /// 4下一个按钮
    /// 5关闭这个界面的按钮
    /// 6关闭这个界面按钮  不与其他阿牛通用  右上角的关闭按钮
    /// 7标题  说明这个红包是不是需要分享说说明的标题  成功分享后领取
    public GameObject Content;


    public Animator RpAnimator;
    public Button RpBackCancle;//如果是分享界面点击其他地方关闭
    public Button RpBackCancle_mogu;//如果是分享界面点击其他地方关闭
    public Image RpImage_Close;//
    public GameObject RpImage_OpenOrShare;//开启或者分享的图片
    public GameObject RpMoneyType;//1红包的金额  后面的金额的类型 目前有六种
    public GameObject RpMoneyType_Image;//
    public GameObject RpName;//2红包名称  修改成图片
    public GameObject RpShare;//3分享按钮
    public GameObject RpNext;//4下一个按钮
    public GameObject RpCanle;//6关闭这个界面按钮  不与其他阿牛通用  右上角的关闭按钮
    public GameObject RpTitle;//7标题  说明这个红包是不是需要分享说说明的标题  成功分享后领取
    public Text RpDesc;//这个红包的描述

    public GameObject start;
    public GameObject Open;


    int m_iIndex = -1;//保存是第几个数组的索引
    int m_iLostNum = 0;//多少个这个红包
    string m_sMoneyNum;//红包金额
    string m_sMoneyType;//红包类型
    int m_iState;//是不是需要分享
    [HideInInspector]
    public int m_iRpid = 0;//红包
    string m_sRedPageName;//红包名称
    Sprite m_SpritePanelShare;//分享的按钮
    Sprite m_SpritePanelOpen;//开的按钮
    int From = 0;//是大厅还是游戏调用的红包 0大厅 1游戏

    public enum NowState
    {
        Lobby = 0,
        Game
    }

    /// <summary>
    /// 保存值，但是不开启界面
    /// </summary>
    /// <param name="index_"></param>
    /// <param name="LostNum_"></param>
    /// <param name="state_"></param>
    /// <param name="RedPageName_"></param>
    /// <param name="BigRedpageID_"></param>
    public void OnSetValueAndNotOpenPanel(int index_, int LostNum_, int state_, string RedPageName_, NowState From_, int BigRedpageID_ = 0)
    {
        m_iIndex = index_;
        m_iLostNum = LostNum_;
        m_iState = state_;
        m_sRedPageName = RedPageName_;
        m_iRpid = BigRedpageID_;
        From = (int)From_;
    }

    void OnSetRPName()
    {
        Image[] rpname = RpName.GetComponentsInChildren<Image>();
        for (int i = 0; i < rpname.Length; i++)
        {
            rpname[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 赋值
    /// </summary>
    /// <param name="index_"> 保存是第几个数组的索引 </param>
    /// <param name="LostNum_"> 多少个这个红包 </param>
    /// <param name="state_"> 是否分享 0什么都没有 1需要分享 2分享成功 </param>
    /// <param name="RedPageName_"> 红包名称 </param>
    /// <param name="BigRedpageID_"> 大红包的ID </param>
    public void OnSetValue(int index_, int LostNum_, int state_, string RedPageName_, NowState From_, int BigRedpageID_ = 0)
    {
        m_iIndex = index_;
        m_iLostNum = LostNum_;
        m_iState = state_;
        m_sRedPageName = RedPageName_;
        m_iRpid = BigRedpageID_;
        From = (int)From_;

        if (RedPageName_ == "新手红包")
        {
            //新手红包
            RpCanle.gameObject.SetActive(false);
        }
        else
        {
            //新手红包
            RpCanle.gameObject.SetActive(true);
        }

        Debug.Log("state_" + state_);
        if (state_ == 2)
        {
            RpBackCancle.enabled = false;
            RpBackCancle_mogu.enabled = false;
            RpCanle.gameObject.SetActive(false);
        }
        else
        {
            RpBackCancle.enabled = true;
            RpBackCancle_mogu.enabled = true;
            RpCanle.gameObject.SetActive(false);
        }

        //红包界面打开之后是不是还需要分享
        if (m_iState != 0)
        {
            BehindShareForOpenRedPagePanel();
            gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 需要分享之后打开
    /// </summary>
    void BehindShareForOpenRedPagePanel()
    {
        Debug.Log("需要分享之后打开");
        RpAnimator.enabled = false;
        RpAnimator.StopRecording();
        RpAnimator.SetInteger("Redbag", 0);
        start.SetActive(true);
        RpBackCancle.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        OnSetRPName();

        //是红包
        RpImage_Close.gameObject.SetActive(true);

        RpImage_OpenOrShare.gameObject.SetActive(true);
        RpImage_OpenOrShare.GetComponent<Image>().sprite = m_iState == 1 ? m_SpritePanelShare : m_SpritePanelOpen;
        //是红包的金额
        RpMoneyType.SetActive(false);
        //红包名称
        RpName.SetActive(true);
        RpName.transform.GetChild(m_iIndex).gameObject.SetActive(true);
        //分享按钮
        RpShare.SetActive(false);
        //下一个按钮
        RpNext.SetActive(false);
        if (m_iState == 2)
        {
            //成功分享后领取
            RpTitle.SetActive(false);
        }
        else
        {
            //成功分享后领取
            RpTitle.SetActive(true);
        }
    }

    IEnumerator OpenRp(string MoneyNum_, int RpType)
    {
        yield return new WaitForSeconds(2.0f);
        hanshu(MoneyNum_, RpType);
    }

    /// <summary>
    ///  直接打开界面
    /// </summary>
    /// <param name="MoneyNum_">多少钱</param>
    /// <param name="MoneyType_">钱 的类型 上</param>
    /// <param name="MoneyTypeType_">钱的类型 下</param>
    /// <param name="Name">名称</param>
    public void DirectOpenRedPagePanel(string MoneyNum_, int RpType, string Desc)
    {
        Debug.Log ("开启红包的索引：" + m_iIndex);
        //if (index > 0)
        //    m_iIndex = (index-1);
        if (m_iIndex == -1) return;
        if (From == 1 && m_iIndex == 16)//说明在游戏中的麻将馆红包
        {
            RpBackCancle_mogu.GetComponent<Image>().enabled = false;
        }
        else
        {
            RpBackCancle_mogu.GetComponent<Image>().enabled = true;
        }

        OnSetRPName();
        RpBackCancle.enabled = false;
        RpCanle.gameObject.SetActive(true);
        gameObject.SetActive(true);
        RpBackCancle.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        //是红包的金额
        RpAnimator.enabled = true;
        RpAnimator.StopRecording();
        RpAnimator.SetInteger("Redbag", 1);
        //RpAnimator.Play("Redbag");
        //红包名称
        Image[] RpTypeImageName = RpName.GetComponentsInChildren<Image>();
        Debug.Log ("m_iIndex = " + m_iIndex + "," + RpTypeImageName.Length);
        for (int i = 0; i < RpTypeImageName.Length; i++)
            RpTypeImageName[i].gameObject.SetActive(false);
        RpName.SetActive(true);
        RpName.transform.GetChild(m_iIndex).gameObject.SetActive(true);

        //是红包的类型
        Image[] RpTypeImageType = RpMoneyType_Image.GetComponentsInChildren<Image>();
        for (int i = 0; i < RpTypeImageType.Length; i++)
            RpTypeImageType[i].gameObject.SetActive(false);
        RpMoneyType_Image.SetActive(true);
        RpMoneyType_Image.transform.GetChild(RpType).gameObject.SetActive(true);

        //这个红包的描述
        RpDesc.text = Desc;

        RpMoneyType.SetActive(true);
        RpMoneyType.transform.GetComponent<Text>().text = MoneyNum_;//红包金额
        if (MoneyNum_.Length >= 4)
        {
            RpMoneyType.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
            RpMoneyType.transform.localPosition = new Vector3(-42, 0, 0);
        }
        else
        {
            RpMoneyType.transform.localScale = new Vector3(1, 1, 1);
            RpMoneyType.transform.localPosition = new Vector3(-100, -10.8f, 0);
        }

        if (m_iIndex != 1)
        {
            //下一个按钮
            RpNext.SetActive(false);
            //
            RpCanle.SetActive(true);
        }
        else
        {
            if (m_iLostNum < 2)
            {
                //下一个按钮
                RpNext.SetActive(false);
                //
                RpCanle.SetActive(true);
            }
            else
            {
                //下一个按钮
                RpNext.SetActive(true);
                //
                RpCanle.SetActive(true);
            }
        }
        if (m_iIndex == 16)
        {
            RpShare.SetActive(true);
        }
        else
        {
            RpShare.SetActive(false);
        }
        StartCoroutine(OpenRp(MoneyNum_, RpType));
    }
    public void hanshu(string MoneyNum_, int RpType)
    {
        Debug.Log("红包的数量：" + MoneyNum_ + "," + RpType);

        //红包名称
        RpName.SetActive(false);
        RpName.transform.GetChild(m_iIndex).gameObject.SetActive(true);

        m_iLostNum--;
    }

    /// <summary>
    /// 这个红包可以领取了，带领取状态
    /// </summary>
    public void OnSetState()
    {
        if (m_iIndex < 0) return;
        Debug.Log("分享成功");
        if (m_iIndex == 2)
        {
            OnSetClose();

            //获取红包数量
            Messenger_anhui.Broadcast(MainViewLobbyPanel.MESSAGE_REDPAGE);
            return;
        }
        OnSetRPName();
        if (m_iState == 1)
        {
            RpBackCancle.enabled = true;
            RpBackCancle_mogu.enabled = true;
            RpCanle.SetActive(false);
        }
        else
        {
            RpBackCancle.enabled = false;
            RpBackCancle_mogu.enabled = false;
            RpCanle.SetActive(false);
        }
        start.SetActive(true);
        RpBackCancle.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        //是红包
        RpImage_Close.gameObject.SetActive(true);

        RpImage_OpenOrShare.gameObject.SetActive(true);
        RpImage_OpenOrShare.GetComponent<Image>().sprite = m_iState == 1 ? m_SpritePanelShare : m_SpritePanelOpen;
        //是红包的金额
        RpMoneyType.SetActive(false);
        RpMoneyType_Image.SetActive(false);
        //红包名称
        RpName.SetActive(true);
        RpName.transform.GetChild(m_iIndex).gameObject.SetActive(true);
        //分享按钮
        RpShare.SetActive(false);
        //下一个按钮
        RpNext.SetActive(false);
        //
        RpCanle.SetActive(true);
        //成功分享后领取
        RpTitle.SetActive(false);

        gameObject.SetActive(true);
        RpImage_OpenOrShare.SetActive(true);
        RpImage_OpenOrShare.GetComponent<Image>().sprite = m_SpritePanelOpen;//可以领取的一个图标  开
        //成功分享后领取
        RpTitle.SetActive(false);
        m_iState = 2;
    }

    /// <summary>
    /// 红包这个图片的按钮
    /// </summary>
    public void Btn_RedPage()
    {
        if (From == 1)
            MahjongGame_AH.SystemMgr.Instance.AudioSystem.PlayManual(MahjongGame_AH.GameSystem.SubSystem.AudioSystem.AudioMenel.Btn_Click, false, false);
        else
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);

        if (m_iState == 2)
        {
            if (From == 0)
            {
                Debug.Log("大厅红包");
                NetMsg.ClientOpenReceiveRedReqDef msg = new NetMsg.ClientOpenReceiveRedReqDef();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;

                if (GameData.Instance.ParlorShowPanelData.isShowMyParlorMessage)
                {
                    msg.iParlorId = GameData.Instance.ParlorShowPanelData.iParlorId;
                }
                else
                {
                    msg.iParlorId = 0;
                }

                msg.byRedPagType = (byte)(m_iIndex + 1);
                NetworkMgr.Instance.LobbyServer.SendClientReceiveRedReq(msg);
            }
            else
            {
                Debug.Log("游戏红包");
                MahjongGame_AH.Network.Message.NetMsg.ClientOpenReceiveRedReqDef msg = new MahjongGame_AH.Network.Message.NetMsg.ClientOpenReceiveRedReqDef();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg.iParlorId = 0;
                msg.byRedPagType = (byte)(m_iIndex + 1);
                MahjongGame_AH.Network.NetworkMgr.Instance.GameServer.SendClientReceiveRedReq(msg);
            }

            m_iState = 0;
        }
        else if (m_iState == 1)
        {
            if (m_iIndex == 2)
            {
                Debug.Log("最大红包的ID：" + m_iRpid);
                SDKManager.Instance.BtnShare(1, 3,"");
            }
            else
            {
                int index_ = 0;
                switch (m_iIndex)
                {
                    case 1: index_ = 2; break;//推广红包
                    case 3: index_ = 4; break;//提现红包
                    case 6: index_ = 7; break;//加入麻将馆红包
                    case 10: index_ = 11; break;//首次提现红包
                }
                SDKManager.Instance.BtnShare(1, index_,"");
            }
        }
        else
        {

        }
    }

    public void OnSetShareRP(int index__, bool iShare, int BigRp = 0)
    {
        SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
        m_iIndex = index__;
        m_iLostNum = 1;
        m_iState = 1;
        m_sRedPageName = UIMainView.Instance.RedPagePanel.RedPage[index__].Name;
        m_iRpid = BigRp;

        Debug.Log("分享" + index__ + "," + iShare);
        if (index__ == 2)
        {
            Debug.Log("最大红包的ID：" + m_iRpid);
            SDKManager.Instance.BtnShare(1, 3,"");
        }
        else
        {
            Debug.Log("红包分享");

            int index_ = 0;
            switch (m_iIndex)
            {
                case 1: index_ = 2; break;//推广红包
                case 3: index_ = 4; break;//提现红包
                case 6: index_ = 7; break;//加入麻将馆红包
                case 10: index_ = 11; break;//首次提现红包
            }
            SDKManager.Instance.BtnShare(1, index_,"");
        }
    }

    /// <summary>
    /// 固定的底下分享按钮
    /// </summary>
    public void Btn_Share()
    {
        if (From == 1)
           MahjongGame_AH.SystemMgr.Instance.AudioSystem.PlayManual(MahjongGame_AH.GameSystem.SubSystem.AudioSystem.AudioMenel.Btn_Click, false, false);
        else
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
        
        SDKManager.Instance.BtnShare(1,17, "");
    }

    /// <summary>
    /// 下一个按钮
    /// </summary>
    public void Btn_Next()
    {
        SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
        if (m_iIndex == 1)
        {
            if (m_iLostNum <= 0)
            {
                //分享按钮
                RpShare.SetActive(false);
                //下一个按钮
                RpNext.SetActive(false);
            }
            else
            {
                NetMsg.ClientOpenReceiveRedReqDef msg = new NetMsg.ClientOpenReceiveRedReqDef();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg.iParlorId = 0;
                msg.byRedPagType = (byte)(m_iIndex + 1); ;
                NetworkMgr.Instance.LobbyServer.SendClientReceiveRedReq(msg);
                gameObject.SetActive(false);
            }
        }
        start.SetActive(false);
        Open.SetActive(false);
    }

    /// <summary>
    /// 关闭这个界面
    /// </summary>
    public void Btn_Cancle()
    {
        if (From == 1)
        {
            MahjongGame_AH.SystemMgr.Instance.AudioSystem.PlayManual(MahjongGame_AH.GameSystem.SubSystem.AudioSystem.AudioMenel.Btn_Click, false, false);
        }
        else
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            SystemMgr.Instance.RedPageShowSystem.UpdateShow();
            UIMainView.Instance.LobbyPanel.BtnRefresh();
        }
        m_iIndex = -1;
        gameObject.SetActive(false);
        start.SetActive(false);
        Open.SetActive(false);
    }

    /// <summary>
    /// 关闭这个界面
    /// </summary>
    public void OnSetClose()
    {
        if (From == 1)
        {
            MahjongGame_AH.SystemMgr.Instance.AudioSystem.PlayManual(MahjongGame_AH.GameSystem.SubSystem.AudioSystem.AudioMenel.Btn_Click, false, false);
        }
        else
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            SystemMgr.Instance.RedPageShowSystem.UpdateShow();
            UIMainView.Instance.LobbyPanel.BtnRefresh();
        }
        m_iIndex = -1;
        gameObject.SetActive(false);
        start.SetActive(false);
        Open.SetActive(false);
    }
}
