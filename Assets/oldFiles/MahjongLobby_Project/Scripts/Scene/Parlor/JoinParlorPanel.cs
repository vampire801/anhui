using UnityEngine;
using UnityEngine.UI;
using MahjongLobby_AH;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.LobbySystem.SubSystem;
using System;
using System.Collections.Generic;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class JoinParlorPanel : MonoBehaviour
{
    public InputField RoomID;  //输入房间号的输入框
    public Text[] RoomId; //显示房间id

    public string sRoomId = "";   //玩家输入的房间号
    public List<char> lsRoomId = new List<char>();  //玩家输入的房间号,最长6位     


    void OnEnable()
    {
        lsRoomId.Clear();
        UpdateShow();
    }

    /// <summary>
    /// 点击数字按钮
    /// </summary>
    /// <param name="num"></param>
    public void BtnNum(GameObject num)
    {
        SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
        string name = num.name;
        int NumIndex = Convert.ToInt32(name.Split('_')[1]);
        if (NumIndex < 0 || NumIndex > 9)
        {
            return;
        }

        if (lsRoomId.Count < 5)
        {
            lsRoomId.Add(NumIndex.ToString().ToCharArray()[0]);
            if (lsRoomId.Count >= 5)
            {
                string sRoomId = "";
                for (int i = 0; i < 5; i++)
                {
                    sRoomId += lsRoomId[i];
                }
                int roomId = Convert.ToInt32(sRoomId);
                ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                pspd.FromWebGetSearchParlorMessage(5, sRoomId);
                pspd.isShowSearchPointParlor = true;
            }
        }

        UpdateShow();
    }


    void UpdateShow()
    {
        //为房间码赋值
        for (int i = 0; i < 5; i++)
        {
            if (lsRoomId.Count > i)
            {
                RoomId[i].text = lsRoomId[i].ToString();
            }
            else
            {
                RoomId[i].text = "";
            }
        }
    }

    /// <summary>
    /// 点击清空键
    /// </summary>
    public void BtnClaerNum()
    {
        SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
        lsRoomId.Clear();
        UpdateShow();
    }


    /// <summary>
    /// 点击删除键
    /// </summary>
    public void BtnDelNum()
    {
        SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
        if (lsRoomId.Count > 0)
        {
            lsRoomId.RemoveAt(lsRoomId.Count - 1);
        }
        UpdateShow();
    }

    /// <summary>
    /// 点击关闭按钮
    /// </summary>
    public void BtnClose()
    {
        SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
        gameObject.SetActive(false);
    }
}
