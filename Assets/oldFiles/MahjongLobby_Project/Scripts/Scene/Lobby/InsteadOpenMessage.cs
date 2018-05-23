using UnityEngine;
using UnityEngine.UI;
using MahjongLobby_AH.Data;
using XLua;
using MahjongLobby_AH;

[Hotfix]
[LuaCallCSharp]
public class InsteadOpenMessage : MonoBehaviour
{

    public Image RoomStatus_Image; //房间的显示颜色状态
    public Image[] ReadyStatus;  //玩家的准备状态    
    public Sprite[] StatusSprite;  //房间状态对应的所有图片，0表示红色，1表示蓝色

    //保存该房间的信息
    public InsteadOpenRoomPanelData.RoomInfo roomInfo = new InsteadOpenRoomPanelData.RoomInfo();
   
    void Start()
    {
        UpdateShow();
    }

    /// <summary>
    /// 更新显示
    /// </summary>
    public void UpdateShow()
    {
        if(roomInfo==null)
        {
            return;
        }        
        if (roomInfo.cOpenRoomStatus > 0)
        {
            ChangeRoomStatus(roomInfo.byColorFlag);
            ShowRoomPlayerStatus(roomInfo.iuserId);
        }
    }
   

    /// <summary>
    /// 改变房间颜色状态
    /// </summary>
    /// <param name="status"></param>
    public void ChangeRoomStatus(int status)
    {
        RoomStatus_Image.gameObject.SetActive(true);
        if(status>0)
        {
            RoomStatus_Image.sprite = StatusSprite[status - 1];
        }
   
    }

    /// <summary>
    /// 显示该房间的玩家在线状态
    /// </summary>
    /// <param name="userid"></param>
    public void ShowRoomPlayerStatus(int[] userid)
    {
        if(roomInfo.cOpenRoomStatus==3)
        {
            return;
        }
        InsteadOpenRoomPanelData ioprd = MahjongLobby_AH.GameData.Instance.InsteadOpenRoomPanelData;
        int num = 0;  //保存玩家信息已经开始的信息
        for (int i = 0; i < userid.Length; i++)
        {
            if (userid[i] != 0)
            {
                ReadyStatus[i].gameObject.SetActive(true);
                num++;
            }
            else
            {
                ReadyStatus[i].gameObject.SetActive(false);
            }
        }

        //如果状态是未开始游戏，但是已经坐满了四个玩家，则直接删除当前面板，产生新的游戏面板
        if(num==4)
        {
            InsteadOpenRoomPanelData.InsteadOpenStatusNotice notice = new InsteadOpenRoomPanelData.InsteadOpenStatusNotice();
            notice.byOpenRoomStatus = 3;
            notice.sTableNum = roomInfo.iTableNum;
            notice.iaUserId = roomInfo.iuserId;
            ioprd.ChangePlayerInsteadMessage(notice);
        }
    }

  

    /// <summary>
    /// 点击未创建房间按钮
    /// </summary>
    public void BtnUnOpenRoom()
    {
        Messenger_anhui.Broadcast(MahjongLobby_AH.MainViewInsteadOpenRoomPanel.MESSAGE_OPENCREATROOM);
    }

    /// <summary>
    /// 点击所有已经创建的游戏房间按钮
    /// </summary>
    public void BtnCreatGameRoom()
    {        
        //弹出对应的预置体面板
        SpwanRoomMessagePanel();
    } 
  


    /// <summary>
    /// 点击房间按钮对应的面板信息
    /// </summary>
    void SpwanRoomMessagePanel()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/InsteadCreatRoomPanel/RoomMessage"));
        if (go == null)
        {
            return;
        }
        go.transform.SetParent(MahjongLobby_AH.UIMainView.Instance.transform);

        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        RoomMessagePanel room = go.GetComponent<RoomMessagePanel>();
    
        //更新该面板的显示数据
        room.roomInfo = roomInfo;

    }


    //删除自己
    public void Del()
    {
        Destroy(gameObject);
    }

}
