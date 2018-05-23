using UnityEngine;
using System.Collections;
using MahjongGame_AH.Network.Message;
using XLua;

namespace MahjongGame_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class GameResultPanelData
    {
        //面板是否显示的标志位
        public bool isPanelShow;
        //是否显示单局游戏结算的标志位
        public bool isShowRoundGameResult;
        //是否显示整个房间结算的标志位
        public bool isShowRoomGameResult;
        //public bool is
        public bool isWinner;//是否播放胜利音效
        public bool isEndGame;  //是否游戏结束的表示位
        public byte HandDissolve;  //是否是手动结束的标志位
        public int iHandClick; //1表示手动点击 0表示自动点击

        #region 一局结算的数据
        //保存用户手上的牌
        public byte[,] bHandleTiles = new byte[4, 14];

        //四个玩家的分数
        public int[] bResultPoint = new int[4];

        //四个玩家的结果信息
        public NetMsg.ResultTypeDef[] resultType = new NetMsg.ResultTypeDef[4];

        // 放炮用户座位号
        public byte byShootSeat;

        //针对武乡玩法添加的字段
        public byte byShootSeatReadHand; //是否是报听点炮
        public byte byIfNormalOrDragon; //是平胡还是大胡  0没操作1表示平胡2表示大胡

        //所有用户胡牌的标志位
        public byte[] byaWinSrat = new byte[4];

        //玩家的番种类
        public sbyte[,] caFanResult = new sbyte[4, NetMsg .F_TOTAL_NUM];
        #endregion


        #region 比赛结束的数据
        //存储四个玩家的总结算的数据信息
        public NetMsg.ClientRoomResultNotice roomResultNotice = new NetMsg.ClientRoomResultNotice();
        #endregion        

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            bHandleTiles = new byte[4, 14];
            bResultPoint = new int[4];
            resultType = new NetMsg.ResultTypeDef[4];
            byaWinSrat = new byte[4];
            roomResultNotice = new NetMsg.ClientRoomResultNotice();
            //Debug.LogError("玩家手牌：" + bHandleTiles[0, 0]);
            //Debug.LogError("玩家得分：" + bResultPoint[0]);
        }

    }
}
