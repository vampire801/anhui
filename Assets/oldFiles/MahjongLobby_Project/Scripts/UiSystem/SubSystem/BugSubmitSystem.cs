using UnityEngine;
using System.Collections;
using MahjongLobby_AH.LobbySystem;
using MahjongLobby_AH.Data;
using System;
using DG.Tweening;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class CustomSystem : GameSystemBase
    {
        #region 事件处理
        /// <summary>
        /// 处理进入场景
        /// </summary>
        /// <param name="sender"></param>
        protected override void HandleOnEnterScene(SceneManager_anhui sender)
        {
            switch (sender.EnteringScene)
            {
                case ESCENE.MAHJONG_LOBBY_MAIN_SCENE:
                    Messenger_anhui.AddListener(MainViewCustomSeverPanel.MESSAGE_CLOSEBTN, HandleCloseBtn);
                    Messenger_anhui.AddListener(MainViewCustomSeverPanel.MESSAGE_WHATWX, HandleWhatWxBtn);
                    Messenger_anhui.AddListener(MainViewCustomSeverPanel.MESSAGE_BTNMETHODCOLLECT, HandleMethodCollect);
                    break;
                default:
                    break;
            }
        }

        protected override void HandleOnLeaveScene(SceneManager_anhui sender)
        {
            switch (sender.LeavingScene)
            {
                case ESCENE.MAHJONG_LOBBY_MAIN_SCENE:
                    Messenger_anhui.RemoveListener(MainViewCustomSeverPanel.MESSAGE_CLOSEBTN, HandleCloseBtn);
                    Messenger_anhui.RemoveListener(MainViewCustomSeverPanel.MESSAGE_WHATWX, HandleWhatWxBtn);
                    Messenger_anhui.RemoveListener(MainViewCustomSeverPanel.MESSAGE_BTNMETHODCOLLECT, HandleMethodCollect);
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理


        public delegate void BugSubmitUpdateShowEventHandler();
        public BugSubmitUpdateShowEventHandler OnBugSubmitUpdate;

        public void UpdateShow()
        {
            if (OnBugSubmitUpdate != null)
            {
                OnBugSubmitUpdate();
            }
        }

        /// <summary>
        /// 处理点击关闭按钮
        /// </summary>
        void HandleCloseBtn()
        {
            GameData gd = GameData.Instance;
            CustomPanelData bspd = gd.CustomPanelData;
            bspd.PanelShow = false;
            UpdateShow();
        }

        /// <summary>
        /// 处理如何关注微信公众号按钮
        /// </summary>
        /// <param name="text"></param>
        void HandleWhatWxBtn()
        {
            UpdateShow();
        }
        /// <summary>
        /// 处理点击玩法收集按钮
        /// </summary>
        void HandleMethodCollect()
        {
            GameData gd = GameData.Instance;
            CustomPanelData bspd = gd.CustomPanelData;
            bspd.isShowMethodCollect = true;
            bspd.isShowCustomSever = false;
            UpdateShow();
        }
        internal void OnPointerDown(GameObject go)
        {
            CustomPanelData cd = GameData.Instance.CustomPanelData;
            cd.downPos = UIMainView.Instance.CustomSeverPanel.gameObj_ShowGroup.transform.localPosition.x;
        }

        internal void OnPointerUp(GameObject go)
        {
            CustomPanelData cd = GameData.Instance.CustomPanelData;
            MainViewCustomSeverPanel cp = UIMainView.Instance.CustomSeverPanel;
            int num = 3;//滑动总个数
            float oriPos = GameData.Instance.CustomPanelData.oriPos;
            float upPos = cp.gameObj_ShowGroup.transform.localPosition.x;
            float lenth = cp.gameObj_ShowGroup.GetComponent<RectTransform>().sizeDelta.x;

            cd.indexCurrent = upPos - cd.downPos >= 0 ? cd.indexCurrent - 1 : cd.indexCurrent + 1;
            cd.indexCurrent = cd.indexCurrent > num ? num : cd.indexCurrent;
            cd.indexCurrent = cd.indexCurrent < 1 ? 1 : cd.indexCurrent;

            switch (cd.indexCurrent)
            {
                case 1:
                    cp.text_detal.text = "点击微信 右上角 “+” 添加友好";
                    cp.toggle_point[0].isOn = true;
                    cp.gameObj_ShowGroup.transform.DOLocalMoveX(oriPos, 0.3f);
                    break;
                case 2:
                    cp.text_detal.text = "点击“公众号”";
                    cp.toggle_point[1].isOn = true;
                    cp.gameObj_ShowGroup.transform.DOLocalMoveX(oriPos - lenth / 3, 0.3f);
                    break;
                case 3:
                    cp.text_detal.text = "输入 双喜麻将 点击 搜索结果 关注 双喜麻将";
                    cp.toggle_point[2].isOn = true;
                    cp.gameObj_ShowGroup.transform.DOLocalMoveX(oriPos - lenth / 3 * 2, 0.3f);
                    break;
                default:
                    break;
            }
        }
    }

}
