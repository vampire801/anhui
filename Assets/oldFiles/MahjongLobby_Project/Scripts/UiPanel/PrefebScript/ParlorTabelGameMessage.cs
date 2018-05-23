using UnityEngine.UI;
using UnityEngine;
using MahjongLobby_AH.Network;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class ParlorTabelGameMessage : MonoBehaviour
{
    public GameObject UnStart_p;
    public GameObject Started_p;
    public GameObject UnStartRedPoint; //为开始游戏的红点
    public GameObject TabelToggel; //按键的toggel组
    public GameObject UnStart; //为开始游戏
    public GameObject Started; //已开始游戏
    public GameObject NoRecord_Started; //已经开始游戏无记录面板
    public GameObject ApplyPanel; //申请界面
    public GameObject TabelPanel; //卓界面

    public RawImage BossImage; //馆主的头像
    public Text BossNick; //馆主的昵称
    public Text ParlorId;  //馆主的id
    public Text UnStartCount; //未开始的数量
    public Text StartedCount; //未开始的数量
    public Text BossContact;  //馆主的联系方式
    public Text ParlorMemberCount;  //馆的成员数量
    public Text ParlorMonthActi;  //馆的月活跃度
    public Text ParlorAllActi; //馆的总活跃度
    public Text ParlorBulletin;  //馆的公告信息
    public Button Setting; //设置按钮
    public Button QuitParlor; //退出馆的按钮
    public Button CanelApplyParlor; //取消申请馆的按钮   
    public GameObject Member; //成员列表    
    public int KickType; //成员提出类型

    //取消申请麻将馆信息
    public void BtnCanelApplyParlor()
    {
        NetMsg.ClientCAncelApplyOrJudgeApplyTooReq msg = new NetMsg.ClientCAncelApplyOrJudgeApplyTooReq();
        msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
        msg.iParlorId = GameData.Instance.ParlorShowPanelData.iParlorId;
        msg.iType = 2;
        NetworkMgr.Instance.LobbyServer.SendClientCAncelApplyOrJudgeApplyTooReq(msg);
    }


    public void RefreshParlorMessage()
    {
        //获取玩家的申请馆的id
        GameData.Instance.ParlorShowPanelData.FromWebGetApplyParlorId(6, 1);
        SDKManager.Instance.GetComponent<anhui.CameControler>().PostMsg("loading", "正在刷新麻将馆数据");
    }


    /// <summary>
    /// 显示指定的面板
    /// </summary>
    /// <param name="type">1显示待开始 2显示已开始 3显示成员列表</param>
    public void ShowPointPanel(int type)
    {
        KickType = type;
        switch (type)
        {
            case 1:
                UnStart_p.SetActive(true);
                Started_p.SetActive(false);
                UnStart.SetActive(true);
                Started.SetActive(false);
                Member.SetActive(false);
                break;
            case 2:
                UnStart.SetActive(false);
                UnStart_p.SetActive(false);
                Started_p.SetActive(true);
                Started.SetActive(true);
                Member.SetActive(false);
                break;
            case 3:
                UnStart.SetActive(false);
                Started.SetActive(false);
                UnStart_p.SetActive(false);
                Started_p.SetActive(false);
                NoRecord_Started.SetActive(false);
                Member.SetActive(true);
                break;
        }
    }

}
