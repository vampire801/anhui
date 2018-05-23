using UnityEngine;
using UnityEngine.UI;
using System.Text;
using MahjongLobby_AH;
using System;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.Network;
using MahjongLobby_AH.Network.Message;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class PlayerMessagePrefabCommon : MonoBehaviour
{
    public Image MessageTitle; //消息的标题抬头
    public Sprite[] showMessage; //显示的标题图片，0代理邀请绑定会员，1表示解绑申请
    public Button AgreeBtn;  //同意按钮
    public Button DisAgreeBtn;  //拒绝按钮
    public Button MessageContent;  //点击查看按钮
    public GameObject MessageStatus; //信息状态
    public GameObject Btn;  //两个按钮的父物体
    public Text Content;  //消息显示的具体内容
    public Text ContentMore;  //当消息过多时，显示省略号
    public Text timer;  //消息时间    

    public PlayerMessagePanelData.Message messageContent = new PlayerMessagePanelData.Message();

    //房间剩余时间
    float remindTime = 0f;

    public string[] color = new string[2] { "6B9549FF", "C9420FFF" };  //字体颜色0蓝色  1红色

    /// <summary>
    /// 改变字体的颜色
    /// </summary>
    /// <param name="title"></param>
    /// <param name="index"></param>
    string ChangeFontColor(string title, int index)
    {
        StringBuilder str = new StringBuilder();
        str.Append("<color=#");
        str.Append(color[index]);
        str.Append(">");
        str.Append(title);
        str.Append("</color>");        
        return str.ToString();
    }

    void Start()
    {
        //消息内容的定义
        StringBuilder str = new StringBuilder();

        //处理普通的消息
        if (Convert.ToInt16(messageContent.msgType) == 1)
        {
            MessageTitle.gameObject.SetActive(false);
            timer.text = anhui.MahjongCommonMethod.Instance.UnixTimeStampToDateTime(Convert.ToDouble(messageContent.sendTim), 0).ToString("yyyy-MM-dd HH:mm");
            str.Append(messageContent.content);
            Content.text = str.ToString();
            Btn.SetActive(false);
            //如果玩家消息的长度显示大于115，就显示玩家的查看按钮
            float height = Content.preferredHeight;

            if(height > 95)
            {
                Btn.gameObject.SetActive(true);
                AgreeBtn.gameObject.SetActive(false);
                DisAgreeBtn.gameObject.SetActive(false);
                MessageContent.gameObject.SetActive(true);
                ContentMore.gameObject.SetActive(true);
                //调整消息框的大小
                Content.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 110f);
            }
            else
            {
                ContentMore.gameObject.SetActive(false);
                MessageContent.gameObject.SetActive(false);
            }
        }
        //邀请绑定会员
        else if (Convert.ToInt16(messageContent.msgType) == 2)
        {
            MessageTitle.gameObject.SetActive(true);
            MessageTitle.sprite = showMessage[0];
            if (Convert.ToInt32(messageContent.operate) == 0)
            {
                Btn.SetActive(true);
                DisAgreeBtn.gameObject.SetActive(true);
                AgreeBtn.gameObject.SetActive(true);
                AgreeBtn.transform.Find("Text").GetComponent<Text>().text = "加入";
            }
            else
            {
                Btn.SetActive(false);                
            }
            timer.text = anhui.MahjongCommonMethod.Instance.UnixTimeStampToDateTime(Convert.ToDouble(messageContent.sendTim), 0).ToString("yyyy-MM-dd HH:mm");
            str.Append(messageContent.content);
            Content.text = str.ToString();
        }
        //请求解绑会员
        else
        {
            MessageTitle.gameObject.SetActive(true);
            MessageTitle.sprite = showMessage[1];
            if (Convert.ToInt32(messageContent.operate) == 0)
            {                
                Btn.SetActive(true);
                DisAgreeBtn.gameObject.SetActive(true);
                AgreeBtn.gameObject.SetActive(true);
                AgreeBtn.transform.Find("Text").GetComponent<Text>().text = "同意";
            }
            else
            {
                Btn.SetActive(false);                
            }

            timer.text = anhui.MahjongCommonMethod.Instance.UnixTimeStampToDateTime(Convert.ToDouble(messageContent.sendTim), 0).ToString("yyyy-MM-dd HH:mm");
            str.Append(messageContent.content);
            Content.text = str.ToString();
        }
    } 
  

    /// <summary>
    /// 点击同意按钮
    /// </summary>
    public void BtnAgree(int index)
    {
        SendAgencyMessage(index);
        ChangeMessagePrefab(index);
    }

    /// <summary>
    /// 点击拒绝按钮
    /// </summary>
    public void BtnDisAgree(int index)
    {
        SendAgencyMessage(index);
        ChangeMessagePrefab(index);
    }

    /// <summary>
    /// 点击查看按钮
    /// </summary>
    public void BtnView()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/PlayerMessagePanel/MessageCotent"));
        go.transform.SetParent(UIMainView.Instance.PlayerMessagePanel.transform.Find("BlackBg"));
        //go.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        //go.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
        //go.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        //go.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.GetComponent<MessageCotent>().text.text= messageContent.content;        
    }



    /// <summary>
    /// 点击按钮之后，更新界面
    /// </summary>
    /// <param name="index"></param>
    void ChangeMessagePrefab(int index)
    {
        //StringBuilder str = new StringBuilder();
        MessageStatus.gameObject.SetActive(true);
        //处理点击同意按钮之后，更新界面
        if (index == 1)
        {
            MessageStatus.transform.Find("Text").GetComponent<Text>().text = "已同意";
            //关闭按钮 
            DisAgreeBtn.gameObject.SetActive(false);
            AgreeBtn.gameObject.SetActive(false);
        }
        //处理点击拒绝按钮之后，更新界面
        else
        {
            MessageStatus.transform.Find("Text").GetComponent<Text>().text = "已拒绝";
            //关闭按钮
            DisAgreeBtn.gameObject.SetActive(false);
            AgreeBtn.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 发送玩家请求消息
    /// </summary>
    public void SendAgencyMessage(int index)
    {
        GameData gd = GameData.Instance;
        NetMsg.ClientMessageOperateReq msg = new NetMsg.ClientMessageOperateReq();
        msg.iUserId = gd.PlayerNodeDef.iUserId;
        msg.iMessageId =Convert.ToInt32(messageContent.mid);
        msg.cMessageType = (sbyte)Convert.ToInt16(messageContent.msgType);
        msg.cOperate = (sbyte)index;
        NetworkMgr.Instance.LobbyServer.SendMessageOperateReq(msg);
    }
}
