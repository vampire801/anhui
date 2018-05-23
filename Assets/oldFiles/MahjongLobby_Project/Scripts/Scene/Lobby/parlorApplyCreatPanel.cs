using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using MahjongLobby_AH.Data;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class parlorApplyCreatPanel : MonoBehaviour
    {
        public Text AreaName;  //地区名字
        public RawImage Head;
        public Text TgName; //推广员名字
        public Text Wx; //微信号
        public Text Phone; //手机号
        public Text TgContent; //推广员提示
        public GameObject[] CertContet; //资格提示 0表示无资格 1表示有资格
        public Button[] Ok; //知道了提示按钮  0知道了  1立刻开馆
        public Text TgMemNum; //当前玩家的推广人数

        /// <summary>
        /// 更新提示界面
        /// </summary>
        /// <param name="status">0表示没有开馆资格  1表示有开馆资格</param>
        public void UpdateShow(int status)
        {
            TgMemNum.text = "【当前" + GameData.Instance.PlayerNodeDef.userDef.iSpreadAcc + "人】";
            CertContet[status].gameObject.SetActive(true);
            CertContet[1 - status].gameObject.SetActive(false);
            Ok[status].gameObject.SetActive(true);
            Ok[1 - status].gameObject.SetActive(false);
            if (status == 0)
            {
                TgContent.text = "您可以向该地区的专员申请零门槛开馆资格";
            }
            else
            {
                TgContent.text = "您有任何问题可以向该地区专员咨询和寻求帮助";
            }
            GetTgMessage(GameData.Instance.SelectAreaPanelData.iCountyId);
        }

        [Serializable]
        public class Json_Join
        {
            public Data_join[] data;
            public int status;
        }
        [Serializable]
        public struct Data_join
        {
            public int ID;
            public string NICKNAME;
            public string WEIXIN;
            public string PHONE;
            public string IMG_URL;
            public string COUNTY_NAME;
        }

        /// <summary>
        /// 改变地区
        /// </summary>
        public void ChangeArea()
        {
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            sapd.iOpenStatus = 7;
            sapd.pos_index = 5;
            sapd.isPanelShow = true;
            SystemMgr.Instance.SelectAreaSystem.UpdateShow();
        }


        /// <summary>
        /// 获取推广专员的信息
        /// </summary>
        /// <param name="countId"></param>
        public void GetTgMessage(int countId)
        {
            anhui.MahjongCommonMethod mcm = anhui.MahjongCommonMethod.Instance;
            AreaName.text = mcm._dicDisConfig[countId].COUNTY_NAME;
            string url = SDKManager.Instance.IOSCheckStaus == 0 ? LobbyContants.MAJONG_PORT_URL : LobbyContants.MAJONG_PORT_URL_T;
            Dictionary<string, string> value = new Dictionary<string, string>();
            value.Add("County_id", countId.ToString());
            anhui.MahjongCommonMethod.Instance.GetPlayerMessageData_IE(url + "communication.x", value, UpdateTgPanel, " ", 5);
        }

        /// <summary>
        /// 更新推广员界面
        /// </summary>
        /// <param name="status"></param>
        /// <param name="json"></param>
        public void UpdateTgPanel(string json, int status)
        {
            Json_Join data = new Json_Join();
            data = JsonUtility.FromJson<Json_Join>(json);
            if (data.status != 1)
            {
                TgName.text = "";
                Wx.text = "";
                Phone.text = "";
                return;
            }
            anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(Head, LobbyContants.ActivitePic + data.data[0].IMG_URL);
            TgName.text = data.data[0].NICKNAME;
            Wx.text = data.data[0].WEIXIN;
            Phone.text = data.data[0].PHONE;
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="text"></param>
        public void BtnCopy(Text text)
        {
            anhui.MahjongCommonMethod.Instance.CopyString(text.text.ToString());
            anhui.MahjongCommonMethod.Instance.ShowRemindFrame("复制成功");
        }

        /// <summary>
        /// 点击确认开馆
        /// </summary>
        public void BtnCreatParlor()
        {
            Data.ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            pspd.isShowSecondSure = true;
            pspd.isShowApplyCreatParlorPanel = false;
            SystemMgr.Instance.ParlorShowSystem.UpdateShow();
        }

        /// <summary>
        /// 跳转交流群
        /// </summary>
        public void BtnSkipJoinUs()
        {
            UIMainView.Instance.LobbyPanel.BtnJoinUs();
        }
    }
}


