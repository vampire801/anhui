using UnityEngine;
using System.Collections;
using MahjongLobby_AH.Network.Message;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class ShareWxPanelData 
    {
        /// <summary>
        /// 面板是否显示的标志位
        /// </summary>
        public bool PanelShow;

        public string CodeKey; //玩家的推广码

        public string szShareTitle= "双喜安徽麻将->长治麻将<-房号：【12306】点击进入房间";                                        //分享标头 ：双喜安徽麻将->合肥麻将<-房号：【12306】点击进入房间
        public string szShareDiscription= "8局，点炮胡 一门八张 无闹庄";                                  //8局，点炮胡 一门八张 无闹庄

        public string szShareUrl = "https://by.ibluejoy.com/m/t.html?mj="; //微信分享链接
        public string szInNeedSendNumber="110";                                  //微信分享链接携带参数
        public NetMsg.ClientShareSuccessRes shareSuccess = new NetMsg.ClientShareSuccessRes();


    }
}

