using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using MahjongLobby_AH;
using MahjongLobby_AH.LobbySystem.SubSystem;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.Network;
using System;
using XLua;

[Hotfix]
[LuaCallCSharp]
public struct RPContent
{
    /// <summary>
    /// 图片
    /// </summary>
    public Sprite Image;
    /// <summary>
    /// 名称
    /// </summary>
    public string Name;
    /// <summary>
    /// 带使用的数量
    /// </summary>
    public int ShareNum;
    /// <summary>
    /// 可以使用的数量
    /// </summary>
    public int CanUseNum;
    /// <summary>
    /// 获得钱或者代金券的数量
    /// </summary>
    public string GetMoneyNum;
    /// <summary>
    /// 获得的数量类型
    /// </summary>
    public string GetMoneyType;
    /// <summary>
    /// 是不是需要分享的功能
    /// </summary>
    public bool isShare;
    /// <summary>
    /// 红包的编号
    /// </summary>
    public int RpNumber;
    /// <summary>
    /// 红包描述
    /// </summary>
    public string Describe;

    public void SetShareNum(int num_)
    {
        ShareNum = num_;
    }
    public void SetCanUseNum(int num_)
    {
        CanUseNum = num_;
    }
    public void SetGetMoneyNum(string GetMoneyNum_)
    {
        GetMoneyNum = GetMoneyNum_;
    }
    public void SetGetMoneyType(string GetMoneyType_)
    {
        GetMoneyType = GetMoneyType_;
    }
}

public class MainViewRedPagePanel : MonoBehaviour
{
    public RectTransform m_gScrollviewContent;//按钮的父物体
    public GameObject m_gRedButtonPrefab;//红包预制体

    public string Lin1 = "------------------------------";//领取红包界面
    public GameObject m_gRedPageSpritePanel;
    public string Lin2 = "------------------------------";//数值

    /// <summary>
    /// 有多少个红包，内容。。。
    /// </summary>
    [HideInInspector]
    public List<RPContent> RedPage = new List<RPContent>();
    //红包数量
    [HideInInspector]
    public int m_iRedPageNum = 0;

    Sprite m_SpritePanelShare;
    Sprite m_SpritePanelOpen;

    void Satrt()
    {
        m_SpritePanelShare = Resources.Load<Sprite>("Lobby/RedPageImage/pic_0099_3");
        m_SpritePanelOpen = Resources.Load<Sprite>("Lobby/RedPageImage/pic_0099_2");
    }


    void Update()
    {

    }

    public void InitRP()
    {
        m_SpritePanelShare = Resources.Load<Sprite>("Lobby/RedPageImage/pic_0099_3");
        m_SpritePanelOpen = Resources.Load<Sprite>("Lobby/RedPageImage/pic_0099_2");

        RedPage.Clear();

        RPContent rp1 = new RPContent();
        rp1.Image = m_SpritePanelOpen;
        rp1.Name = "建房红包";
        rp1.isShare = false;
        rp1.RpNumber = 0;
        rp1.Describe = "成功创建房间有机会获得一个红包，惊喜连连！";
        RedPage.Add(rp1);

        RPContent rp2 = new RPContent();
        rp2.Image = m_SpritePanelShare;
        rp2.Name = "推广红包";
        rp2.isShare = true;
        rp2.RpNumber = 1;
        rp2.Describe = "每次分享并成功推荐好友即可获得一个红包，惊喜连连！";
        RedPage.Add(rp2);//7

        RPContent rp3 = new RPContent();
        rp3.Image = m_SpritePanelShare;
        rp3.Name = "充值红包";
        rp3.isShare = true;
        rp3.RpNumber = 2;
        RedPage.Add(rp3);//需要分享充值红包  类似饿了吗那种的 8

        RPContent rp4 = new RPContent();
        rp4.Image = m_SpritePanelShare;
        rp4.Name = "提现红包";
        rp4.isShare = true;
        rp4.RpNumber = 3;
        rp4.Describe = "提现也有红包拿，惊喜送不停！";
        RedPage.Add(rp4);//9

        RPContent rp5 = new RPContent();
        rp5.Image = m_SpritePanelOpen;
        rp5.Name = "新手红包";
        rp5.isShare = false;
        rp5.RpNumber = 4;
        RedPage.Add(rp5);

        RPContent rp6 = new RPContent();
        rp6.Image = m_SpritePanelOpen;
        rp6.Name = "首次参与红包";
        rp6.isShare = false;
        rp6.RpNumber = 5;
        rp6.Describe = "体验一局游戏就拿到一个红包，真是惊喜连连！";
        RedPage.Add(rp6);

        RPContent rp7 = new RPContent();
        rp7.Image = m_SpritePanelOpen;
        rp7.Name = "加入麻将馆红包";
        rp7.isShare = true;
        rp7.RpNumber = 6;
        rp7.Describe = "加入麻将馆也有红包拿，您可以再加入一个！";
        RedPage.Add(rp7);//分享

        RPContent rp8 = new RPContent();
        rp8.Image = m_SpritePanelOpen;
        rp8.Name = "关注公众号红包";
        rp8.isShare = false;
        rp8.RpNumber = 7;
        rp8.Describe = "关注公众号也有红包拿，真是惊喜连连！";
        RedPage.Add(rp8);

        RPContent rp9 = new RPContent();
        rp9.Image = m_SpritePanelOpen;
        rp9.Name = "实名认证红包";
        rp9.isShare = false;
        rp9.RpNumber = 8;
        rp9.Describe = "恭喜您认证成功，请收下我们的奖励！";
        RedPage.Add(rp9);//分享

        RPContent rp10 = new RPContent();
        rp10.Image = m_SpritePanelOpen;
        rp10.Name = "首次分享红包";
        rp10.isShare = false;
        rp10.RpNumber = 9;
        rp10.Describe = "现金、话费、流量送不停！";
        RedPage.Add(rp10);

        RPContent rp11 = new RPContent();
        rp11.Image = m_SpritePanelShare;
        rp11.Name = "首次提现红包";
        rp11.isShare = true;//10
        rp11.RpNumber = 10;
        rp11.Describe = "首次提现也有红包拿，惊喜送不停！";
        RedPage.Add(rp11);

        RPContent rp12 = new RPContent();
        rp12.Image = m_SpritePanelOpen;
        rp12.Name = "活动分享红包";
        rp12.isShare = false;
        rp12.RpNumber = 11;
        rp12.Describe = "活动不停，红包不断！";
        RedPage.Add(rp12);

        RPContent rp13 = new RPContent();
        rp13.Image = m_SpritePanelOpen;
        rp13.Name = "大赢家红包";
        rp13.isShare = false;
        rp13.RpNumber = 12;//后期游戏内true
        rp13.Describe = "打牌也有红包拿！";
        RedPage.Add(rp13);

        RPContent rp14 = new RPContent();
        rp14.Image = m_SpritePanelOpen;
        rp14.Name = "最佳炮手红包";
        rp14.isShare = false;
        rp14.RpNumber = 13;//后期游戏内true
        rp14.Describe = "打牌也有红包拿！";
        RedPage.Add(rp14);

        RPContent rp15 = new RPContent();
        rp15.Image = m_SpritePanelOpen;
        rp15.Name = "玩法红包";
        rp15.isShare = false;
        rp15.RpNumber = 14;
        rp15.Describe = "感谢您对我们产品的支持与建议！";
        RedPage.Add(rp15);

        RPContent rp16 = new RPContent();
        rp16.Image = m_SpritePanelOpen;
        rp16.Name = "提交BUG红包";
        rp16.isShare = false;
        rp16.RpNumber = 15;
        rp16.Describe = "感谢您对我们产品的支持与建议！";
        RedPage.Add(rp16);

        RPContent rp17 = new RPContent();
        rp17.Image = m_SpritePanelOpen;
        rp17.Name = "麻将馆红包";
        rp17.isShare = false;
        rp17.RpNumber = 16;
        rp17.Describe = "麻将馆福利，抢到就是赚到！";
        RedPage.Add(rp17);
    }

    /// <summary>
    ///  实例化红包
    /// </summary>
    public void OnInitRedPage()
    {
        if (RedPage.Count <= 0)
            return;
        RectTransform[] parent = m_gScrollviewContent.GetComponentsInChildren<RectTransform>();
        for (int i = 1; i < parent.Length; i++)
        {
            Destroy(parent[i].gameObject);
        }

        //红包的数量
        int RpNum = 0;

        bool isHaveData = false;
        for (int i = 0; i < RedPage.Count; i++)
        {
            if ((RedPage[i].CanUseNum + RedPage[i].ShareNum) > 0)
            {
                GameObject go = Instantiate(m_gRedButtonPrefab) as GameObject;
                go.transform.SetParent(m_gScrollviewContent.gameObject.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = new Vector3(1, 1, 1);
                go.name = RedPage[i].Name;
                if ((RedPage[i].CanUseNum + RedPage[i].ShareNum) == 1)
                {
                    go.transform.GetChild(0).gameObject.SetActive(false);
                    go.transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    go.transform.GetChild(0).gameObject.SetActive(true);
                    go.transform.GetChild(1).gameObject.SetActive(true);
                    go.transform.GetChild(1).transform.GetComponent<Text>().text = (RedPage[i].CanUseNum + RedPage[i].ShareNum).ToString();//有多少个这个红包
                }
                //go.transform.GetChild(2).transform.GetComponent<Text>().text = RedPage[i].Name;
                go.transform.GetChild(2).GetChild(RedPage[i].RpNumber).gameObject.SetActive(true);
                if (RedPage[i].CanUseNum > 0)
                {
                    go.transform.GetChild(3).transform.GetComponent<Image>().sprite = m_SpritePanelOpen;//使用什么样的图
                    go.transform.GetChild(4).gameObject.SetActive(false);
                }
                else
                {
                    go.transform.GetChild(3).transform.GetComponent<Image>().sprite = m_SpritePanelShare;//使用什么样的图
                    go.transform.GetChild(4).gameObject.SetActive(true);
                }
                OnOnOpenWhileRedPage(go.GetComponent<Button>(), i);
                isHaveData = true;
                RpNum++;
            }
        }
        if (isHaveData == false)
        {
            gameObject.SetActive(false);
            UIMainView.Instance.LobbyPanel.RedPage.SetActive(false);
        }
        if (RpNum <= 3)
        {
            if (m_gScrollviewContent.GetComponent<ContentSizeFitter>()!=null)
            {
                m_gScrollviewContent.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            }
        }
        else
        {
            if (m_gScrollviewContent.GetComponent<ContentSizeFitter>() != null)
            {
                m_gScrollviewContent.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.MinSize;
            }
        }
    }

    void OnOnOpenWhileRedPage(Button btn, int index)
    {
        btn.onClick.AddListener(delegate () { OnOpenWhileRedPage(index); });
    }

    /// <summary>
    /// 打开什么红包
    /// </summary>
    /// <param name="isShare"> 是不是分享true为分享 </param>
    void OnOpenWhileRedPage(int index)
    {
        Debug.LogError("玩法红包" + index);
        if (index == 2)
        {
            NetMsg.ClientOpenReceiveRedReqDef msg = new NetMsg.ClientOpenReceiveRedReqDef();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iParlorId = 0;
            msg.byRedPagType = (byte)(index + 1);
            NetworkMgr.Instance.LobbyServer.SendClientReceiveRedReq(msg);
            return;
        }

        if (RedPage[index].isShare && RedPage[index].CanUseNum <= 0)
        {
            UIMgr.GetInstance().ShowRedPagePanel.OnSetShareRP(index, RedPage[index].isShare);
            //UIMgr.GetInstance().ShowRedPagePanel.OnSetValue(index, RedPage[index].ShareNum, 1, RedPage[index].Name);
        }
        else
        {
            if (index == 14 || index == 15)//玩法红包  提交BUG红包
            {
                Debug.LogError("玩法红包" + index);
                LoadRP(index + 1);
            }
            else
            {
                NetMsg.ClientOpenReceiveRedReqDef msg = new NetMsg.ClientOpenReceiveRedReqDef();
                msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                msg.iParlorId = 0;
                msg.byRedPagType = (byte)(index + 1);
                NetworkMgr.Instance.LobbyServer.SendClientReceiveRedReq(msg);
                UIMgr.GetInstance().ShowRedPagePanel.OnSetValue(index, RedPage[index].CanUseNum, 0, RedPage[index].Name, RedPageShowPanel.NowState.Lobby);
            }
        }
    }

    void LoadRP(int index)
    {
        Debug.LogError("红包缩影：" + index);
        string str = LobbyContants.MAJONG_PORT_URL + "userOpenGmRp.x";
        if (SDKManager.Instance.IOSCheckStaus == 1)
        {
            str = LobbyContants.MAJONG_PORT_URL_T + "userOpenGmRp.x";
        }
        Dictionary<string, string> value = new Dictionary<string, string>();
        value.Add("uid", GameData.Instance.PlayerNodeDef.iUserId.ToString());
        value.Add("type", index.ToString());
        anhui.MahjongCommonMethod.Instance.GetPlayerMessageData_IE(str, value, OnGetValue_, "rp");
    }
    
    void OnGetValue_(string json, int status)
    {
        RPConfigFor15_16_ rpc = new RPConfigFor15_16_();
        rpc = JsonBase.DeserializeFromJson<RPConfigFor15_16_>(json.ToString());

        RPConfigFor15_16 data = new RPConfigFor15_16();

        if (rpc.rp[0].status != 1)
        {
            Debug.LogError("获取网页json数据状态错误,status:" + rpc.rp[0].status+","+ rpc.rp[0].assetNum+","+ rpc.rp[0].assetType);
            return;
        }
        data.status = rpc.rp[0].status;
        data.assetNum = rpc.rp[0].assetNum;
        data.assetType = rpc.rp[0].assetType;
        data.type = rpc.rp[0].type;
        int Type = Convert.ToInt32(data.assetType);
        int RpType = -1;
        string str = "", str1 = "";
        switch (Type)
        {
            case 1:
                {
                    str = "现金";
                    str1 = "元";
                    RpType = 3;
                    GameData.Instance.PlayerNodeDef.userDef.da2Asset[0] += Convert.ToInt32(data.assetNum);
                }
                break;
            case 2:
                {
                    str = "话费";
                    str1 = "元";
                    RpType = 4;
                    GameData.Instance.PlayerNodeDef.userDef.da2Asset[1] += Convert.ToInt32(data.assetNum);
                }
                break;
            case 3:
                {
                    str = "流量";
                    str1 = "M";
                    RpType = 1;
                    GameData.Instance.PlayerNodeDef.userDef.da2Asset[2] += Convert.ToInt32(data.assetNum);
                }
                break;
            case 4:
                {
                    str = "储值卡";
                    str1 = "元";
                    RpType = 2;
                    GameData.Instance.PlayerNodeDef.userDef.da2Asset[3] += Convert.ToInt32(data.assetNum);
                }
                break;
            case 5:
                {
                    str = "代金券";
                    str1 = "张";
                    RpType = 5;
                }
                break;
            case 6:
                {
                    str = "赠币";
                    str1 = "个";
                    RpType = 0;
                    GameData.Instance.PlayerNodeDef.iBindCoin += Convert.ToInt32(data.assetNum);
                }
                break;
        }
        int index = data.type - 1;

        UIMgr.GetInstance().ShowRedPagePanel.OnSetValueAndNotOpenPanel(index, 1, 3/*表示点击红包的时候不做处理*/, RedPage[index].Name, RedPageShowPanel.NowState.Lobby);
        
        RPContent rp2 = new RPContent();
        rp2.Image = UIMainView.Instance.RedPagePanel.RedPage[index].Image;
        rp2.Name = UIMainView.Instance.RedPagePanel.RedPage[index].Name;
        rp2.ShareNum = UIMainView.Instance.RedPagePanel.RedPage[index].ShareNum;
        rp2.CanUseNum = UIMainView.Instance.RedPagePanel.RedPage[index].CanUseNum;
        rp2.GetMoneyNum = data.assetNum;
        rp2.GetMoneyType = str;
        rp2.isShare = UIMainView.Instance.RedPagePanel.RedPage[index].isShare;
        rp2.RpNumber = UIMainView.Instance.RedPagePanel.RedPage[index].RpNumber;
        rp2.Describe = UIMainView.Instance.RedPagePanel.RedPage[index].Describe;
        UIMainView.Instance.RedPagePanel.RedPage[index] = rp2;
        UIMgr.GetInstance().ShowRedPagePanel.DirectOpenRedPagePanel(data.assetNum, RpType, rp2.Describe);// str, str1, rp2.Name);

        Debug.LogError("领取的红包：" + data.assetNum + str1 + str);
        Debug.LogError("获得资源类型:1现金，2话费，3流量，4储值卡，（5代金券，6赠币没有对应字段）" + Type);

        SystemMgr.Instance.RedPageShowSystem.UpdateShow();

        //获取红包数量
        Messenger_anhui.Broadcast(MainViewLobbyPanel.MESSAGE_REDPAGE);
        UIMainView.Instance.LobbyPanel.BtnRefresh();
    }

    //[Serializable]
    public class RPConfigFor15_16
    {
        public int status;  //1成功  9系统错误 0无数据
        public string assetType;  //2.打开红包获得的资源类型，1现金，2话费，3流量，4储值卡,5代金券,6赠币。
        public string assetNum;  //打开红包获得的资源数量
        public int type;  //类型1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16
    }
    public class RPConfigFor15_16_
    {
        public List<RPConfigFor15_16> rp = new List<RPConfigFor15_16>();
    }


    /// <summary>
    /// 关闭这个界面
    /// </summary>
    public void BtnClose()
    {
        gameObject.SetActive(false);
    }

}
