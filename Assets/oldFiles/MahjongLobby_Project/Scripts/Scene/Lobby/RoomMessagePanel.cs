using UnityEngine;
using UnityEngine.UI;
using MahjongLobby_AH.Data;
using System.Text;
using MahjongLobby_AH.LobbySystem.SubSystem;
using System;
using XLua;
using anhui;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class RoomMessagePanel : MonoBehaviour
    {

        //该房间面板的信息
        public InsteadOpenRoomPanelData.RoomInfo roomInfo = new InsteadOpenRoomPanelData.RoomInfo();
        public Text[] RoomMessage;  //0表示房间号，1表示房间规则，2表示开启时间，3表示房间状态
        public Image[] RoomSignColor; //以创建未开始游戏的房间的颜色标记
        public GameObject[] ShareWx;  //0表示分享到微信的按钮，1表示标记的信息框,2表示房间的txt信息

        void Awake()
        {
            transform.GetComponent<Canvas>().worldCamera = Camera.main;
        }


        void Start()
        {
            SpwanRoomMessagePanel();
        }


        /// <summary>
        /// 产生房间对应的预置体
        /// </summary>
        /// <param name="status">1表示已创建未开始房间，2表示已开始的房间</param>
        /// <param name="rule">房间的规则参数</param>
        /// <param name="time">房间创建时间</param>
        void SpwanRoomMessagePanel()
        {
            int status = 0;
            if (roomInfo.cOpenRoomStatus < 3)
            {
                status = 1;
            }
            else
            {
                status = 2;
            }
            //已创建未开始房间
            if (status == 1)
            {
                for (int i = 0; i < ShareWx.Length; i++)
                {
                    ShareWx[i].SetActive(true);
                }
                //设置间距
                ShareWx[2].GetComponent<GridLayoutGroup>().spacing = new Vector2(0, -20f);
                ShareWx[2].GetComponent<GridLayoutGroup>().transform.localPosition = new Vector3(-75f, 8f, 0);
                int num = 0;   //在线玩家个数
                for (int i = 0; i < roomInfo.iuserId.Length; i++)
                {
                    if (roomInfo.iuserId[i] != 0)
                    {
                        Debug.LogError("num:" + num);
                        num++;
                    }
                }
                RoomMessage[3].text = num + "/4";
                //处理房间的颜色状态
                for (int i = 0; i < 4; i++)
                {
                    if (i == roomInfo.byColorFlag - 1)
                    {
                        RoomSignColor[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        RoomSignColor[i].gameObject.SetActive(false);
                    }
                }

            }

            //已开始的房间
            if (status == 2)
            {
                ShareWx[0].SetActive(false);
                ShareWx[1].SetActive(false);
                ShareWx[2].SetActive(true);
                //设置房间信息的间距位置
                ShareWx[2].GetComponent<GridLayoutGroup>().spacing = new Vector2(0, 10f);
                ShareWx[2].GetComponent<GridLayoutGroup>().transform.localPosition = new Vector3(-75f, -32f, 0);
                RoomMessage[3].text = "游戏中";
            }

            StringBuilder text = new StringBuilder();


            string name = MahjongCommonMethod.Instance._dicMethodConfig[roomInfo.iPlayingMethod].METHOD_NAME;
            text.Append(name);
            MahjongCommonMethod mcm = MahjongCommonMethod.Instance;
            mcm.ShowParamOfOpenRoom(ref text, roomInfo.caOpenRoomParam, 1, MahjongCommonMethod.Instance.iPlayingMethod);
            //text.Append(mcm.ReadInt32toInt4(roomInfo.caOpenRoomParam[1], 0) * 10 + mcm.ReadInt32toInt4(roomInfo.caOpenRoomParam[0], 0));
            //int rowNum = 2;
            //text.Append(mcm.ReadColumnValue(roomInfo.caOpenRoomParam, rowNum, 4) > 0 ? "局 " : "圈 ");

            #region oldCode
            //switch (roomInfo.iPlayingMethod)
            //{
            //    case 1:
            //    case 2:
            //    case 4:
            //    case 5:
            //    case 8:
            //    case 9:
            //    case 11:
            //    case 12:
            //    case 13:
            //    case 14:
            //        text.Append(roomInfo.caOpenRoomParam[0]);
            //        text.Append("圈");
            //        break;
            //    case 7:
            //    case 10:
            //        if(roomInfo.caOpenRoomParam[0]==1)
            //        {
            //            text.Append("8局");
            //        }
            //        else if(roomInfo.caOpenRoomParam[0] == 2)
            //        {
            //            text.Append("16局");
            //        }
            //        else if (roomInfo.caOpenRoomParam[0] == 3)
            //        {
            //            text.Append("24局");
            //        }
            //        break;
            //    case 3:
            //    case 6:
            //        text.Append(roomInfo.caOpenRoomParam[0] * 10);
            //        text.Append("分");
            //        break;
            //    default:
            //        text.Append("获取数据错误");
            //        break;
            //}
            #endregion

            RoomMessage[0].text = string.Format("{0:d6}", roomInfo.roomNum);
            RoomMessage[1].text = text.ToString();
            RoomMessage[2].text = MahjongCommonMethod.Instance.UnixTimeStampToDateTime(roomInfo.iOpenRoomTime, 0).ToString("yyyy-MM-dd  HH:mm");

        }


        public void Des()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            //发送玩家的开房信息请求

            MahjongLobby_AH.Network.Message.NetMsg.ClientOpenRoomInfoReq msg = new MahjongLobby_AH.Network.Message.NetMsg.ClientOpenRoomInfoReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            MahjongLobby_AH.Network.NetworkMgr.Instance.LobbyServer.SendOpenRoomInfoReq(msg);
            GameData.Instance.InsteadOpenRoomPanelData.isClickInsteadOpenRoom = true; Destroy(gameObject);
        }

        /// <summary>
        /// 处理点击邀请按钮
        /// </summary>
        public void HandleBtnShareWx()
        {
            // Debug.LogError("+++RoomId :" +Convert.ToInt32( RoomMessage[0].text));
            string url = SDKManager.WXInviteUrl + Convert.ToInt32(RoomMessage[0].text);
            // Debug.LogError("分享链接" + url);
            string city;
            try
            {
                city = MahjongLobby_AH.GameData.Instance.SelectAreaPanelData.listCityMessage[MahjongLobby_AH.GameData.Instance.PlayerNodeDef.iCityId].cityName;
            }
            catch (System.Exception)
            {
                city = "双喜";
            }
            StringBuilder title = new StringBuilder();
            // Debug.LogError("玩法Num" + MahjongCommonMethod.Instance.iPlayingMethod);
            MahjongCommonMethod mcm = MahjongCommonMethod.Instance;
            if (MahjongLobby_AH.SDKManager.Instance.CheckStatus == 1)
            {
                title.AppendFormat("代开房间：{0}麻将->{1}<-房号：{2} 点击进入房间", city, "推倒胡", Convert.ToInt32(RoomMessage[0].text).ToString("d6"));
            }
            else
            {
                title.AppendFormat("代开房间：{0}麻将->{1}<-房号：{2} 点击进入房间", city, mcm._dicMethodConfig[roomInfo.iPlayingMethod].METHOD_NAME, Convert.ToInt32(RoomMessage[0].text).ToString("d6"));
            }
            StringBuilder discription = new StringBuilder("玩法：");
            mcm.ShowParamOfOpenRoom(ref discription, roomInfo.caOpenRoomParam, 0, MahjongCommonMethod.Instance.iPlayingMethod);

            #region 代替换
            //switch (roomInfo.iPlayingMethod)
            //{
            //    case 1:
            //        if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("1圈 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 2) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("");
            //        else discription.Append("明杠收三家 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("可胡七对 ");

            //        if (roomInfo.caOpenRoomParam[3] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[3] == 1) discription.Append("十三不靠 ");

            //        if (roomInfo.caOpenRoomParam[4] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[4] == 1) discription.Append("四杠荒庄 ");

            //        if (roomInfo.caOpenRoomParam[5] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[5] == 1) discription.Append("前台 ");
            //        else discription.Append("后和 ");

            //        if (roomInfo.caOpenRoomParam[6] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[6] == 1) discription.Append("一炮单响 ");
            //        else discription.Append("一炮多响 ");
            //        break;
            //    case 2:
            //        if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("1圈 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 2) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("");
            //        else discription.Append("明杠收三家 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("可胡七对 ");

            //        if (roomInfo.caOpenRoomParam[3] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[3] == 1) discription.Append("带字牌 ");

            //        if (roomInfo.caOpenRoomParam[4] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[4] == 1) discription.Append("前台 ");
            //        else discription.Append("后和 ");

            //        if (roomInfo.caOpenRoomParam[5] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[5] == 1) discription.Append("一炮单响 ");
            //        else discription.Append("一炮多响 ");
            //        break;
            //    case 3:
            //        if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("10分 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 2) discription.Append("20分 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("");
            //        else discription.Append("明杠收三家 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("可胡七对 ");

            //        if (roomInfo.caOpenRoomParam[3] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[3] == 1) discription.Append("带字牌 ");

            //        if (roomInfo.caOpenRoomParam[4] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[4] == 1) discription.Append("前台 ");
            //        else discription.Append("后和 ");
            //        break;
            //    case 4:
            //        if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("1圈 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 2) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[1] == 1) discription.Append("带字牌 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("可胡七对 ");

            //        if (roomInfo.caOpenRoomParam[3] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[3] == 1) discription.Append("前台 ");
            //        else discription.Append("后和 ");
            //        break;
            //    case 5:
            //        if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("1圈 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 2) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[1] == 1) discription.Append("带字牌 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("可胡七对 ");

            //        if (roomInfo.caOpenRoomParam[3] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[3] == 1) discription.Append("前台 ");
            //        else discription.Append("后和 ");
            //        break;
            //    case 6:
            //        if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("30分 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 2) discription.Append("60分 ");
            //        else discription.Append("90分 ");
            //        break;
            //    case 7:
            //        if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("8局 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 2) discription.Append("16局 ");
            //        else discription.Append("24局 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[1] == 1) discription.Append("自摸翻倍 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("带字牌 ");
            //        break;
            //    case 8:
            //        if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("1圈 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 2) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomInfo.caOpenRoomParam[3] == 0) discription.Append("");
            //        else discription.Append("明杠收三家 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[1] == 1) discription.Append("可胡七对 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("十三不靠 ");

            //        if (roomInfo.caOpenRoomParam[5] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[5] == 1) discription.Append("前台 ");
            //        else discription.Append("后和 ");

            //        if (roomInfo.caOpenRoomParam[4] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[4] == 1) discription.Append("四杠荒庄 ");

            //        break;
            //    case 9:

            //        if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("1圈 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 2) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[1] == 1) discription.Append("带字牌 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("可胡七对 ");

            //        if (roomInfo.caOpenRoomParam[3] == 0) discription.Append("一人付 ");
            //        else if (roomInfo.caOpenRoomParam[3] == 1) discription.Append("多人付 ");
            //        break;
            //    case 10:
            //        if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("8局 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 2) discription.Append("16局 ");
            //        else discription.Append("24局 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[1] == 1) discription.Append("带字牌 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("可胡七对 ");

            //        if (roomInfo.caOpenRoomParam[3] == 0) discription.Append("一炮单响 ");
            //        else if (roomInfo.caOpenRoomParam[3] == 1) discription.Append("一炮多响 ");
            //        break;
            //    case 11:
            //        if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("1圈 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 2) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("");
            //        else discription.Append("带字牌 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("可胡七对 ");

            //        if (roomInfo.caOpenRoomParam[3] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[3] == 1) discription.Append("明杠收三家 ");

            //        if (roomInfo.caOpenRoomParam[4] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[4] == 1) discription.Append("前台 ");
            //        else if (roomInfo.caOpenRoomParam[4] == 2) discription.Append("后和 ");
            //        else discription.Append("前台加后和");

            //        if (roomInfo.caOpenRoomParam[5] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[5] == 1) discription.Append("坐庄 ");

            //        if (roomInfo.caOpenRoomParam[6] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[6] == 1) discription.Append("能跑能下 ");

            //        if (roomInfo.caOpenRoomParam[7] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[7] == 1) discription.Append("放几出几 ");
            //        break;
            //    case 12:
            //        if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("1圈 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 2) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("");
            //        else discription.Append("带字牌 ");
            //        break;
            //    case 13:
            //        if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("1圈 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 2) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("");
            //        else discription.Append("带字牌 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("前台 ");
            //        else if (roomInfo.caOpenRoomParam[2] == 2) discription.Append("后和 ");
            //        else discription.Append("前台加后和");

            //        if (roomInfo.caOpenRoomParam[3] == 0) discription.Append("");
            //        else discription.Append("翻" + roomInfo.caOpenRoomParam[3] + "倍 ");

            //        if (roomInfo.caOpenRoomParam[4] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[4] == 1) discription.Append("可胡七对 ");

            //        break;
            //    case 14:
            //        if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("1圈 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 2) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("");
            //        else discription.Append("十三幺 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("可胡七对 ");
            //        break;
            //    case 15:
            //        if (roomInfo.caOpenRoomParam[0] == 0) discription.Append("1圈 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("不带字牌 ");
            //        else discription.Append("带字牌 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("轮庄 ");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("连庄 ");
            //        break;
            //    case 16:
            //        if (roomInfo.caOpenRoomParam[0] == 0) discription.Append("1圈 ");
            //        else if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("2圈 ");
            //        else discription.Append("3圈 ");

            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("不带字牌 ");
            //        else discription.Append("带字牌 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("被杠者支付 ");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("三家都支付 ");

            //        if (roomInfo.caOpenRoomParam[3] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[3] == 1) discription.Append("点炮三分 ");

            //        if (roomInfo.caOpenRoomParam[4] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[4] == 1) discription.Append("一炮多响 ");

            //        if (roomInfo.caOpenRoomParam[5] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[5] == 1) discription.Append("前抬 ");

            //        if (roomInfo.caOpenRoomParam[6] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[6] == 1) discription.Append("后和 ");
            //        break;
            //    case 17:
            //        if (roomInfo.caOpenRoomParam[0] >= 3)
            //        {
            //            if (roomInfo.caOpenRoomParam[0] == 3) discription.Append("4局 ");
            //            else if (roomInfo.caOpenRoomParam[0] == 4) discription.Append("8局 ");
            //            else discription.Append("12局 ");
            //        }
            //        else
            //        {
            //            if (roomInfo.caOpenRoomParam[0] == 0) discription.Append("1圈 ");
            //            else if (roomInfo.caOpenRoomParam[0] == 1) discription.Append("2圈 ");
            //            else discription.Append("3圈 ");
            //        }
            //        if (roomInfo.caOpenRoomParam[1] == 0) discription.Append("");
            //        else discription.Append("带庄 ");

            //        if (roomInfo.caOpenRoomParam[2] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[2] == 1) discription.Append("带字牌 ");

            //        if (roomInfo.caOpenRoomParam[3] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[3] == 1) discription.Append("明杠收3家 ");

            //        if (roomInfo.caOpenRoomParam[4] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[4] == 1) discription.Append("点炮三分 ");

            //        if (roomInfo.caOpenRoomParam[5] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[5] == 1) discription.Append("一炮多响 ");

            //        if (roomInfo.caOpenRoomParam[6] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[6] == 1) discription.Append("前抬 ");

            //        if (roomInfo.caOpenRoomParam[7] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[7] == 1) discription.Append("后和 ");

            //        if (roomInfo.caOpenRoomParam[8] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[8] == 1) discription.Append("抢庄 ");

            //        if (roomInfo.caOpenRoomParam[9] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[9] == 1) discription.Append("杠随胡走 ");

            //        if (roomInfo.caOpenRoomParam[10] == 0) discription.Append("");
            //        else if (roomInfo.caOpenRoomParam[10] == 1) discription.Append("仅自摸 ");
            //        break;
            //    default:
            //        break;
            //}
            #endregion

            MahjongLobby_AH.SDKManager.Instance.HandleShareWX(url, title.ToString(), discription.ToString(), 0, 0, 0, "");
        }
    }
}