using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MahjongGame_AH.Network.Message;
using MahjongLobby_AH;
using MahjongLobby_AH.LobbySystem.SubSystem;
using System;
using MahjongGame_AH.Network;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class UserInfo : MonoBehaviour
{
    public RawImage HeadImage;  //头像的图片
    public Text NickName;  //昵称
    public Text Id; //玩家id
    public Text PlayHandSpeed; //出牌速度
    public Text EscapeRate;  //逃跑率
    public Text ComplientCount; //赞的数量
    public Text PlayPosMessage; //玩家的详细信息
    public Text PlayerIp;  //玩家的ip信息
    public Text Distance; //俩者之间的距离
    public RawImage Bg; //面板的背景
    public Texture[] bg;  //显示背景

    public GameObject[] point;  //距离的上下的位置
    public string sPosMessage; //玩家位置详细信息
    public float fDistance; //俩个玩家之间的距离
  //  bool isCanClick=true ;//是否可以点击
    public Button[] Propert;

    void Start()
    {
        if (SDKManager.Instance.IOSCheckStaus ==1)
        {
            for (int i = 0; i < Propert.Length; i++)
            {
                Propert[i].gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// 为玩家面板添加信息
    /// </summary>
    /// <param name="userinfo">玩家信息</param>
    /// <param name="type">1表示大厅，2表示游戏</param>
    public void GetUserMessage(NetMsg.UserInfoDef userinfo,int type)
    {
        //为摄像机赋值
        transform.GetComponent<Canvas>().worldCamera = Camera.main;
        Bg.texture = bg[type-1];
        anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(HeadImage, userinfo.szHeadimgurl);
        NickName.text = userinfo.szNickname;
        Id.text = userinfo.iUserId.ToString();
        if(userinfo.iPlayCardAcc==0)
        {
            PlayHandSpeed.text = "0秒(平均)";
        }
        else
        {
            PlayHandSpeed.text = userinfo.iPlayCardTimeAcc / userinfo.iPlayCardAcc + "秒(平均)";
        }

        ComplientCount.text = userinfo.iCompliment.ToString();

        if(userinfo.iGameNumAcc==0)
        {
            EscapeRate.text = "0.0%";
        }
        else
        {
            EscapeRate.text = (((float)userinfo.iDisconnectAcc/(float)userinfo.iGameNumAcc)*100f).ToString("0.0")+"%";
        }

        //ip信息赋值
        PlayerIp.text = userinfo.szIp;

        //Debug.LogError("sPosMessage:" + sPosMessage);

        //显示位置详细信息
        PlayPosMessage.text = sPosMessage;
        if (sPosMessage.Length > 8)
        {
            Distance.transform.localPosition = new Vector3(Distance.transform.localPosition.x, point[1].transform.localPosition.y, 0);
        }
        else
        {
            Distance.transform.localPosition = new Vector3(Distance.transform.localPosition.x, point[0].transform.localPosition.y, 0);
        }


        //显示距离
        if (fDistance<=0)
        {
            Distance.gameObject.SetActive(false);
        }
        else
        {
            Distance.gameObject.SetActive(true);
            Distance.transform.Find("Distance").GetComponent<Text>().text = fDistance.ToString("0")+"米";
        }
    }

    public void GetUserMessageForLobby(MahjongLobby_AH.Network.Message.NetMsg.TableUserInfoDef userinfo, int type)
    {
        //为摄像机赋值
        transform.GetComponent<Canvas>().worldCamera = Camera.main;
        Bg.texture = bg[type - 1];
        anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(HeadImage, userinfo.szHeadimgurl);
        NickName.text = userinfo.szNickname;
        Id.text = userinfo.iUserId.ToString();
        if (userinfo.iPlayCardAcc == 0)
        {
            PlayHandSpeed.text = "0秒(平均)";
        }
        else
        {
            PlayHandSpeed.text = userinfo.iPlayCardTimeAcc / userinfo.iPlayCardAcc + "秒(平均)";
        }

        ComplientCount.text = userinfo.iCompliment.ToString();

        if (userinfo.iGameNumAcc == 0)
        {
            EscapeRate.text = "0.0%";
        }
        else
        {
            EscapeRate.text = (((float)userinfo.iDisconnectAcc / (float)userinfo.iGameNumAcc) * 100f).ToString("0.0") + "%";
        }
    }
    #region Button
    public void BtnCopyId(Text id)
    {
        //SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
        anhui.MahjongCommonMethod.Instance.CopyString(id.text.ToString());
        //处理玩家复制成功之后提示文字
        anhui.MahjongCommonMethod.Instance.ShowRemindFrame("复制成功", false);
    }

    /// <summary>
    /// 关闭玩家信息面板
    /// </summary>
    public void BtnCloseUserInfoPanel()
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// 点击道具按钮
    /// </summary>
    /// <param name="aa">1-flower，2-hand,3-money,4-egg</param>
    public void BtnClickEffect(int aa)
    {
        if (MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData.time_t>= MahjongGame_AH.Data.PlayerPlayingPanelData.GLOBLE_BUTTONTIME)
        {
            MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData.time_t = 0;
            MahjongGame_AH .UIMainView.Instance.PlayerPlayingPanel .AddIEnumerator();
            NetMsg.ClientChatReq msg = new NetMsg.ClientChatReq();
            msg.iUserId[0] = anhui.MahjongCommonMethod.Instance.iUserid;
            msg.iUserId[1] = int.Parse(Id.text);
            msg.byChatType = 3;
            msg.byContentId = Convert.ToByte(aa);
            NetworkMgr.Instance.GameServer.SendChatReq(msg);
            BtnCloseUserInfoPanel();
        }
        //Messenger.Broadcast(MainViewShortTalkPanel.MessageCloseChatPenal);
    }
    #endregion Button
   
    void Update()
    {
        if (MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData.time_t< MahjongGame_AH.Data.PlayerPlayingPanelData.GLOBLE_BUTTONTIME)
        {
           // Debug.LogError(Propert[0].transform.GetChild(0).GetComponent  <Image>().name   + Propert[0].transform.GetComponentInChildren<Image>().fillAmount);

            for (int i = 0; i < Propert.Length; i++)
            {
                Propert[i].transform.GetChild(0).GetComponent<Image>().fillAmount = MahjongGame_AH.GameData.Instance.PlayerPlayingPanelData.time_t / MahjongGame_AH.Data.PlayerPlayingPanelData.GLOBLE_BUTTONTIME;
            }
        }
    }
}
