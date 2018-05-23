using UnityEngine;

using Spine.Unity;

using XLua;

[Hotfix]
[LuaCallCSharp]
public class DeleteEffect : MonoBehaviour
{
    float timer = 2f;   //删除时间
    public bool isStartDownTimer;

    public bool isDelGetRoomBag;  //是否删除领取方法的预置体
    public float timer_Bag = 6f;
    public int CardNum;  //房卡数量
    public SkeletonAnimation Ani;

    void Update()
    {

        if (isStartDownTimer)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                isStartDownTimer = false;
                Destroy(gameObject);
            }
        }
        if (isDelGetRoomBag)
        {
            timer_Bag -= Time.deltaTime;
            if (timer_Bag <= 0)
            {
                isDelGetRoomBag = false;
                MahjongLobby_AH.UIMainView.Instance.LobbyPanel.NewPlayerBag.SetActive(false);
                MahjongLobby_AH.SystemMgr.Instance.LobbyMainSystem.UpdateShow();
                if (MahjongLobby_AH.GameData.Instance.LobbyMainPanelData.CardNumstatus == 1)
                {
                    MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iBindCoin += CardNum;                   
                }
                MahjongLobby_AH.SystemMgr.Instance.LobbyMainSystem.UpdateShow();
                Destroy(gameObject);
            }

        }
    }


    //删除自己
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
