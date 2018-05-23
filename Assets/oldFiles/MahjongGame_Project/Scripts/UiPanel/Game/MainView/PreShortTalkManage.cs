using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MahjongGame_AH.Network.Message;
using System;
using MahjongGame_AH.Network;
using DG.Tweening;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class PreShortTalkManage : MonoBehaviour
    {
        public int _mID;
        public Text _text;
        //快捷语点击发送
        /// <summary>
        /// Type 12表情 快捷语
        /// </summary>
        /// <param name="_mID"></param>
        public void BtnSendShortTalkMessage()
        {
            NetMsg.ClientChatReq msg = new Network.Message.NetMsg.ClientChatReq();
            msg.iUserId[0] = anhui.MahjongCommonMethod.Instance.iUserid;
            msg.iUserId[1] = 0;
            msg.byChatType = 2;
            msg.byContentId = Convert.ToByte (_mID);
            NetworkMgr.Instance.GameServer.SendChatReq (msg);
            Messenger_anhui.Broadcast(MainViewShortTalkPanel .MessageCloseChatPenal);
            
        }
        private Tweener tweener;
        //void OnEnable()
        //{
        //    //Debug.LogError(this.name + _text.text.Length);
        //    if (_text.text.Length >= 15)
        //    {
        //        tweener = gameObject.transform.GetChild(0).DOLocalMoveX(-125, 3).SetRelative();
        //        tweener.SetEase(Ease.Linear);
        //        tweener.SetLoops(2, LoopType.Yoyo);
        //        tweener.SetDelay(2);
        //        tweener.Play();
        //        tweener.OnComplete(() =>
        //        {
        //            tweener.Restart();
        //        });
        //    }
        //}
        //void OnDisable()
        //{
        //    tweener.Kill();
        //}

    }
}

