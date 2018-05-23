using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class TextConstant
    {

        public const string GENERA_TITILE = "温馨提示";
        public const string NET_SERVER_DISCONNECT = "无网络连接，请打开网络后重试";
        public const string NET_SEVER_FAILED = "服务器正在维护，请稍候登录";
        public const string DISSOLVE_GAME = "房间已被房主解散";
        public const string ROOMONWER_DISSOLVEROOM = "确定要退出并解散房间吗？";
        public const string ROOMONWER_DISSOLVEROOM_ = "亲，现在已经有别的玩家了，您确定解散房间吗？";
        public const string ROOMONWER_DISSOLVEROOM_NOSTART = "房间未开始，解散房间不会扣除金币。";
        public const string ROOMONWER_DISSOLVEROOM_APPOINTMENT = "若强制离开预约房间将扣除金币，是否继续操作？";
        public const string ROOMONWER_DISSOLVEROOM_APPOINTMENT_TIMEOVER = "预约时间已结束，人数不足4人无法开启游戏，是否继续等待，继续等待将变成普通房间！";
        public const string ROOM_NOTREADY = "由于您没有准备游戏,现将您踢出游戏，返回大厅。";
    }
}
