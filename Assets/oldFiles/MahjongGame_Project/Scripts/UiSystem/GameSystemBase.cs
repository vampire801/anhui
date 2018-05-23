using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongGame_AH.GameSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public abstract class GameSystemBase
    {
        //<summary>
        //初始化
        //</summary>
        public virtual void Init()
        {                        
            AddEventHandler();
        }

        public virtual void Destroy()
        {
            RemoveEventHandler();
        }

        //<summary>
        //初始化事件处理器
        //</summary>
        protected virtual void AddEventHandler()
        {
            SceneManager_anhui.Instance.OnLeaveScene += HandleOnLeaveScene;
            SceneManager_anhui.Instance.OnEnterScene += HandleOnEnterScene;
            MahjongGame_AH.Network.NetworkMgr.Instance.OnRecvData += HandleOnRecvData;
        }

        protected virtual void RemoveEventHandler()
        {         
            SceneManager_anhui.Instance.OnLeaveScene -= HandleOnLeaveScene;
            SceneManager_anhui.Instance.OnEnterScene -= HandleOnEnterScene;
            MahjongGame_AH.Network.NetworkMgr.Instance.OnRecvData -= HandleOnRecvData;
        }

        //<summary>
        //处理进入场景
        //</summary>
        //<param name="sender">场景管理器</param>FishGame.Scene.SceneManager sender
        protected virtual void HandleOnEnterScene(SceneManager_anhui sender)
        {
            Debug.LogWarning(this+"==========================================");
        }

        //<summary>
        //处理离开场景
        //</summary>
        //<param name="sender">场景管理器</param>FishGame.Scene.SceneManager sender
        protected virtual void HandleOnLeaveScene(SceneManager_anhui sender)
        {
        }

        /// <summary>
        /// 处理收到网络消息
        /// </summary>
        /// <param name="cMsgType">网络消息类型</param>
        protected virtual void HandleOnRecvData(ushort cMsgType)
        {
        }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update()
        {
        }
    }
}

