using UnityEngine;
using MahjongLobby_AH.Data;
using System;
using XLua;

namespace MahjongLobby_AH.LobbySystem.SubSystem
{
    [Hotfix]
    [LuaCallCSharp]
    public class SelectAreaSystem : GameSystemBase
    {
        #region 事件处理

        /// <summary>
        /// 当进入场景处理
        /// </summary>
        /// <param name="sender"></param>
        protected override void HandleOnEnterScene(SceneManager_anhui sender)
        {
            switch (sender.EnteringScene)
            {
                case ESCENE.MAHJONG_LOBBY_MAIN_SCENE:
                    Messenger_anhui.AddListener(MainViewSelectAreaPanel.MESSAGE_BTNCOUNTYCLOSE, HandleBtnCountyPanelClose);
                    Messenger_anhui<GameObject>.AddListener(MainViewSelectAreaPanel.MESSAGE_BTNSELECTCITY, HandleBtnSelectCity);                    
                    break;
                default:
                    break;
            }
        }

        protected override void HandleOnLeaveScene(SceneManager_anhui sender)
        {
            switch (sender.LeavingScene)
            {
                case ESCENE.MAHJONG_LOBBY_GENERAL:
                    Messenger_anhui.RemoveListener(MainViewSelectAreaPanel.MESSAGE_BTNCOUNTYCLOSE, HandleBtnCountyPanelClose);
                    Messenger_anhui<GameObject>.RemoveListener(MainViewSelectAreaPanel.MESSAGE_BTNSELECTCITY, HandleBtnSelectCity);                    
                    break;
                default:
                    break;
            }
        }
        #endregion 事件处理

        //面板更新事件
        public delegate void OnSelectAreaSystemUpdate();
        public OnSelectAreaSystemUpdate SelectAreaSystemUpdate;

        /// <summary>
        /// 通知面板更新事件
        /// </summary>
        public void UpdateShow()
        {
            if(SelectAreaSystemUpdate!=null)
            {
                SelectAreaSystemUpdate();
            }
        }


        /// <summary>
        /// 处理点击关闭选择县的按钮
        /// </summary>
        void HandleBtnCountyPanelClose()
        {
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            sapd.isShowCityPanel = true;
            UpdateShow();
        }

        /// <summary>
        /// 处理选择城市按钮
        /// </summary>
        void HandleBtnSelectCity(GameObject city)
        {
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            if(city==null)
            {
                Debug.LogError("预置体为空");
                return;
            }
            int index = Convert.ToInt16(city.name.Split('_')[1]);
            sapd.iCityId = index;
            Debug.LogError("index:" + index + ",name:" + city.name);
            //激活对应的县的预置体
            GameObject go = city.GetComponent<CityConnectCounty>().County;
            go.SetActive(true);
            sapd.isShowCityPanel = false;
            UpdateShow();
        }

       
    }

}
