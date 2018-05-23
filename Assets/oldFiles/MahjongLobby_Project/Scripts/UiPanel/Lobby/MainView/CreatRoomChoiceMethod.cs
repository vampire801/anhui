using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using MahjongLobby_AH.Data;
using MahjongLobby_AH;
using MahjongLobby_AH.LobbySystem.SubSystem;
using System;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class CreatRoomChoiceMethod : MonoBehaviour
{

    public Text GameNum;  //数量
    public Text PayNum; //支付数量

    public int type;  //类型
    public int num;  //玩法数量
    public int pay;  //支付房卡数量

    public Color color_nor = new Color(113 / 255.0f, 85 / 255.0f, 72 / 255.0f, 1);//灰色  不选中为灰色;
    public bool isfatherusOn = false;

    public void BtnChioce()
    {
        CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;
        anhui.MahjongCommonMethod mcm = anhui.MahjongCommonMethod.Instance;
        if (!mcm._dicMethodConfig.ContainsKey(crmpd.MethodId))
        {
            return;
        }

        if (SystemMgr.Instance)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
        }


        if (MahjongLobby_AH.UIMainView.Instance.CreatRoomMessagePanel != null)
        {

            Text label = null;
            if (gameObject.GetComponent<Toggle>().isOn)
            {
                Text[] label2 = UIMainView.Instance.CreatRoomMessagePanel.RuleParent_.GetComponentsInChildren<Text>();
                for (int j = 0; j < label2.Length; j++)
                {
                    if (label2[j].text == "抢庄")
                    {
                        isfatherusOn = label2[j].transform.parent.GetComponent<Toggle>().isOn;
                        color_nor = label2[j].color;
                        label = label2[j];
                        break;
                    }
                }
            }

            for (int j = 0; j < mcm.methiod.Count; j++)
            {
                if (mcm._dicMethodConfig[crmpd.MethodId].METHOD == mcm.methiod[j].id)
                {
                    if (mcm.methiod[j].num1 == num)
                    {
                        crmpd.iPrice = 0;
                        mcm.WriteInt32toInt4(ref crmpd.roomMessage_[1], crmpd.iPrice, 16);
                        UIMainView.Instance.CreatRoomMessagePanel.QuanBukexuan = true;
                        OnSetSelfPlayState(label, true);
                    }
                    else if (mcm.methiod[j].num2 == num)
                    {
                        crmpd.iPrice = 1; UIMainView.Instance.CreatRoomMessagePanel.QuanBukexuan = true;
                        mcm.WriteInt32toInt4(ref crmpd.roomMessage_[1], crmpd.iPrice, 16);
                        OnSetSelfPlayState(label, true);
                    }
                    else if (mcm.methiod[j].num3 == num)
                    {
                        crmpd.iPrice = 2; UIMainView.Instance.CreatRoomMessagePanel.QuanBukexuan = true;
                        mcm.WriteInt32toInt4(ref crmpd.roomMessage_[1], crmpd.iPrice, 16);
                        OnSetSelfPlayState(label, true);
                    }
                    break;
                }
            }
           // Debug.LogError("MID" + GameData.Instance.CreatRoomMessagePanelData.MethodId);
            if (GameData.Instance.CreatRoomMessagePanelData.MethodId == 20015)
            {
                if (num == 4)
                {
                    crmpd.iPrice = 3;
                    mcm.WriteInt32toInt4(ref crmpd.roomMessage_[1], crmpd.iPrice, 16);
                    UIMainView.Instance.CreatRoomMessagePanel.QuanBukexuan = false;
                    OnSetSelfPlayState(label, false);
                }
                else if (num == 8)
                {
                    crmpd.iPrice = 4;
                    mcm.WriteInt32toInt4(ref crmpd.roomMessage_[1], crmpd.iPrice, 16);
                    UIMainView.Instance.CreatRoomMessagePanel.QuanBukexuan = false;
                    OnSetSelfPlayState(label, false);
                }
                else if (num == 12)
                {
                    crmpd.iPrice = 5;
                    mcm.WriteInt32toInt4(ref crmpd.roomMessage_[1], crmpd.iPrice, 16);
                    UIMainView.Instance.CreatRoomMessagePanel.QuanBukexuan = false;
                    OnSetSelfPlayState(label, false);
                }
           }
        }
        else
        {
            for (int j = 0; j < mcm.methiod.Count; j++)
            {
                if (mcm._dicMethodConfig[crmpd.MethodId].METHOD == mcm.methiod[j].id)
                {
                    if (mcm.methiod[j].num1 == num)
                    {
                        crmpd.iPrice = 0;
                        mcm.WriteInt32toInt4(ref crmpd.roomMessage_[1], crmpd.iPrice, 16);
                    }
                    else if (mcm.methiod[j].num2 == num)
                    {
                        crmpd.iPrice = 1;
                        mcm.WriteInt32toInt4(ref crmpd.roomMessage_[1], crmpd.iPrice, 16);
                    }
                    else if (mcm.methiod[j].num3 == num)
                    {
                        crmpd.iPrice = 2;
                        mcm.WriteInt32toInt4(ref crmpd.roomMessage_[1], crmpd.iPrice, 16);
                    }
                    break;
                }
            }

        }
      //  Debug.LogError(num);
        mcm.WriteColumnValue(ref crmpd.roomMessage_, 4, (sbyte)(crmpd.iPrice > 2 ? 1 : 0), 2);
        crmpd.iRoomCard = pay;
        UIMainView.Instance.CreatRoomMessagePanel.UpdataToShowForSelectUseGoldCreatRoom();
    }

    /// <summary>
    /// 要改变文本的预制体
    /// </summary>
    /// <param name="text">要改变文本的预制体</param>
    /// <param name="state">状态</param>
    void OnSetSelfPlayState(Text text, bool state)
    {
        if (text == null) return;

        if (state)
        {
            text.transform.parent.GetComponent<Toggle>().isOn = isfatherusOn;
            text.transform.parent.transform.GetChild(0).gameObject.SetActive(false);
            text.transform.parent.transform.GetChild(0).GetChild(0).gameObject.SetActive(isfatherusOn);
            text.transform.parent.transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            text.transform.parent.GetComponent<Toggle>().isOn = isfatherusOn;
            text.transform.parent.transform.GetChild(0).gameObject.SetActive(true);
            text.transform.parent.transform.GetChild(0).GetChild(0).gameObject.SetActive(isfatherusOn);
            text.transform.parent.transform.GetChild(2).gameObject.SetActive(false);
        }

        if (state == false && isfatherusOn)
        {
            text.color = new Color(1, 0, 0, 1);//红色
        }
        else if (state == false && isfatherusOn == false)
        {
            text.color = new Color(113 / 255.0f, 85 / 255.0f, 72 / 255.0f, 1);//灰色
        }
        else if (state)
        {
            text.color = new Color(143 / 255.0f, 143 / 255.0f, 143 / 255.0f, 1);//银色
        }

    }

    //更新界面
    public void UpdateShow()
    {
        StringBuilder str = new StringBuilder();
        str.Append(num.ToString());
        if (type == 1)
        {
            str.Append("圈");
        }
        else if (type == 2)
        {
            str.Append("局");
        }
        else if (type == 3)
        {
            str.Append("分");
        }

        GameNum.text = str.ToString();
        PayNum.text = "*" + pay;
    }

}
