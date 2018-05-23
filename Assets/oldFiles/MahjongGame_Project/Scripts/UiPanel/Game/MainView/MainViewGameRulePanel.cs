using UnityEngine;
using System;
using MahjongGame_AH.GameSystem.SubSystem;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewGameRulePanel : MonoBehaviour
    {      
        public GameObject BtnParent;  //按钮的父物体
        
        Vector3 PmParentPos = Vector3.zero;
        #region 常量
        public const string MESSAGE_OPEN = "MainViewGameRulePanel.MSEEAGE_OPEN";  //关闭按钮
        public const string MESSAGE_CLOSEBTN = "MainViewGameRulePanel.MSEEAGE_CLOSEBTN";  //关闭按钮
        #endregion 常量    

        void Start()
        {
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        public void BtnClose()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            Messenger_anhui.Broadcast(MESSAGE_CLOSEBTN);
        }

        /// <summary>
        /// 根据id获取指定的面板
        /// </summary>
        /// <param name="index">指定的id</param>
        public void GetIndexMethodPanel(int index)
        {
            if (BtnParent.GetComponentInChildren<RectTransform>().childCount <= 0)
            {
                anhui.MahjongCommonMethod.Instance.ShowRemindFrame("预置体丢失");
                return;
            }

            for (int i = 0; i < BtnParent.GetComponentInChildren<RectTransform>().childCount; i++)
            {
                int id = Convert.ToInt32(BtnParent.transform.GetChild(i).name.Split('_')[1]);
                if (index == id)
                {
                    BtnParent.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    BtnParent.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}