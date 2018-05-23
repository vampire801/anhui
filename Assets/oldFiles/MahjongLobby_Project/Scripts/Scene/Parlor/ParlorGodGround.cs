using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MahjongLobby_AH;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.Network;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class ParlorGodGround : MonoBehaviour
{
    public GameObject NoRecrd; //无记录
    public GameObject ParlorGodMessage; //麻将馆信息
    public Text ParlorMessage; //县的提示信息  

    //没有麻将馆的相关显示信息
    public void NoParlorMessage(int type)
    {
        if (type == 1)
        {
            GameData.Instance.ParlorShowPanelData.isShowParlorRoundPanel = true;
            UIMainView.Instance.ParlorShowPanel.ParlorGodPanel.transform.GetChild(1).
                GetChild(0).gameObject.SetActive(false);
            SystemMgr.Instance.ParlorShowSystem.UpdateShow();
            NoRecrd.SetActive(true);
            ParlorGodMessage.SetActive(false);
            //更新广场的标题
            UIMainView.Instance.ParlorShowPanel.UpdateMyParlorMessage_Title(null, 2);
            //ParlorMessage.text = @"您来开办【" + MahjongCommonMethod.Instance._dicDisConfig[GameData.Instance.SelectAreaPanelData.iCountyId].COUNTY_NAME + "】\n第一个麻将馆呗！";
        }
        else
        {
            UIMainView.Instance.ParlorShowPanel.ParlorGodPanel.transform.GetChild(1).
                GetChild(0).gameObject.SetActive(true);
            NoRecrd.SetActive(false);
            ParlorGodMessage.SetActive(true);
        }
    }

    void OnEnable()
    {
        if (UIMainView.Instance.ParlorShowPanel.ParlorGodNoviceGuidance != null)
        {
            int ParlorGodNoviceGuidanceNum = 1;
            if (PlayerPrefs.HasKey("ParlorGodNoviceGuidance"))
            {
                ParlorGodNoviceGuidanceNum = PlayerPrefs.GetInt("ParlorGodNoviceGuidance");
            }
            if (ParlorGodNoviceGuidanceNum <= 3)
            {
                UIMainView.Instance.ParlorShowPanel.ParlorGodNoviceGuidance.SetActive(true);
                ParlorGodNoviceGuidanceNum++;
            }
            else
            {
                Destroy(UIMainView.Instance.ParlorShowPanel.ParlorGodNoviceGuidance);
            }
            PlayerPrefs.SetInt("ParlorGodNoviceGuidance", ParlorGodNoviceGuidanceNum);//麻将馆新手引导
        }
    }

    /// <summary>
    /// 处理对搜索结果的处理
    /// </summary>
    public void SearchResultDeal(int iParlorId)
    {
        ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
        int status = 1;  //搜索结果的状态 1表示不在玩家的审核和已有麻将馆队列  2表示在已有麻将馆队列  3表示在申请麻将馆队列  
        for (int i = 0; i < pspd.parlorInfoDef.Length; i++)
        {
            if (pspd.parlorInfoDef[i] != null && pspd.parlorInfoDef[i].iParlorId == iParlorId)
            {
                status = 2;
                break;
            }
        }


        if (status != 1)
        {
            for (int i = 0; i < pspd.ApplyParlorMessage.Count; i++)
            {
                if (pspd.ApplyParlorMessage[i].iParlorId == iParlorId)
                {
                    status = 3;
                    break;
                }
            }
        }
        UIMainView.Instance.ParlorShowPanel.joinParlorPanel.BtnClose();
        //直接发送馆的申请请求
        if (status == 1)
        {
            NetMsg.ClientJoinParlorReq msg = new NetMsg.ClientJoinParlorReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iParlorId = iParlorId;
            NetworkMgr.Instance.LobbyServer.SendJoinParlorReq(msg);
        }
        //直接进入申请或者已加入界面
        else if (status == 2 || status == 3)
        {
            //如果是3给出提示
            if (status == 3)
            {
                UIMgr.GetInstance().GetUIMessageView().Show("您已申请该麻将馆，请耐心等待老板审核!");
            }
            PlayerPrefs.SetInt(ParlorShowPanelData.SaveLaseParlorId, iParlorId);
            //请求麻将馆请求信息            
            Messenger_anhui.Broadcast(MainViewLobbyPanel.MESSAGE_OPENPARLOR);
        }

    }
    public void BtnMessage()
    {
        System.Text.StringBuilder str = new System.Text.StringBuilder();

        str.Append(@"<size=30>如何创建麻将馆:</size>
方法一:账户中有<color=#FF0000FF>3000</color>金币即可创建;
方法二:通过大厅【分享】，成功邀请<color=#FF0000FF>15</color>位好友即可免费创建【当前<color=#00FF00FF>");
        str.Append(GameData.Instance.PlayerNodeDef.userDef.iSpreadAcc);
        str.Append("</color>人】");
        UIMgr.GetInstance().GetUIMessageView().LblMsg.alignment = TextAnchor.MiddleLeft;
        UIMgr.GetInstance().GetUIMessageView().Show("开馆条件", str.ToString());
    }
}
