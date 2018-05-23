using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongLobby_AH.UISystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class UIConstant 
    {

        public const ushort UIID_WRONG_ID = 0x0000; //错误ID

        // 普通UI
        public const ushort UIID_GENERAL_LOADING = 0x1001; //加载面板
        public const ushort UIID_MAHJONG_LOBBY_GENERAL_MESSAGE = 0x1002; //消息面板
        public const ushort UIID_GENERAL_WAITING = 0x1003; //等待面板

        // 游戏的UI
        public const ushort UIID_MAHJONG_LOBBY_MAIN_PANEL = 0x1011; //主面板

        /// <summary>
        /// UIID转字符串
        /// </summary>
        /// <returns>ID的字符串</returns>
        /// <param name='id'>UIID</param>
        public static string UIIdToString(ushort id)
        {
            switch (id)
            {
                case UIConstant.UIID_WRONG_ID:
                    return DecToHex(id) + " UIConstant.UIID_WRONG_ID";

                // 普通UI
                //  case UIConstant.UIID_GENERAL_LOADING:
                //     return DecToHex(id) + " UIConstant.UIID_GENERAL_LOADING";
                case UIConstant.UIID_MAHJONG_LOBBY_GENERAL_MESSAGE:
                    return DecToHex(id) + " UIConstant.UIID_FISH_LOBBY_GENERAL_MESSAGE";
                // case UIConstant.UIID_GENERAL_WAITING:
                //    return DecToHex(id) + " UIConstant.UIID_GENERAL_WAITING";

                // 游戏的UI
                case UIConstant.UIID_MAHJONG_LOBBY_MAIN_PANEL:
                    return DecToHex(id) + " UIConstant.UIID_FISH_LOBBY_MAIN_PANEL";

                default:
                    return "Unknow UI ID: " + DecToHex(id);
            }
        }

        /// <summary>
        /// 十进制数转成十六进制字符串
        /// </summary>
        /// <returns>十六进制字符串</returns>
        /// <param name='id'>十进制数</param>
        public static string DecToHex(ushort id)
        {
            return "0x" + id.ToString("X4");
        }

    }

}
