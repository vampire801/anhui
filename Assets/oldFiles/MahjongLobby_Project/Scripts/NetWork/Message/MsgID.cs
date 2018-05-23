
using XLua;

namespace MahjongLobby_AH.Network.Message
{
    [Hotfix]
    [LuaCallCSharp]
    //消息编号显示成字符串
    /// <summary>
    /// 消息编号显示成字符串
    /// </summary>
    /// <returns>消息编号字符串</returns>
    /// <param name="id"> 消息编号</param>
    public class MsgID
    {
        public static string ToString(ushort id)
        {
            string strMsg;
            switch (id)
            {                
                default:
                    strMsg = "0x" + id.ToString("X2") + " Unkown Message ID";
                    break;
            }

            return "Lobby.Network.Message " + strMsg;
        }

        /// <summary>
        /// 指定的消息编号是否不记录到日志
        /// </summary>
        /// <returns>在日志时是否不记录</returns>
        /// <param name="id">消息编号</param>
        public static bool SkipLog(ushort id)
        {
            bool bSkip = false;
            switch (id)
            {
                //case GlobalMsg.KEEP_ALIVE_MSG:
                //case GameMsg.BET_TABLE_INFO_NOTICE_MSG:
                //    bSkip = true;
                //    break;

                default:
                    break;
            }

            return bSkip;
        }

    }

}
