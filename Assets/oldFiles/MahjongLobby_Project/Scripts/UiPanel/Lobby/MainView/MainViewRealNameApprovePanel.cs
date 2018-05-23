using UnityEngine;
using System.Collections;
using MahjongLobby_AH.Data;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewRealNameApprovePanel : MonoBehaviour
    {
        /// <summary>
        /// 名字输入框
        /// </summary>
        public InputField names;
        /// <summary>
        /// 身份证号码输入框
        /// </summary>
        public InputField IdCard;  

        public const string MESSAGE_CLOSE = "MainViewRealNameApprovePanel.close";   //关闭该面板
        public const string MESSAGE_OK = "MainViewRealNameApprovePanel.Ok";  //点击面板的确认按钮
        
        
        void Awake()
        {
            
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 面板更新
        /// </summary>
        public void UpdateShow()
        {
            GameData gd = GameData.Instance;
            RealNameApprovePanelData rnapd = gd.RealNameApprovePanelData;
            if(rnapd.PanelShow)
            {
                names.text = "";
                IdCard.text = "";
                gameObject.SetActive(true);
                GameData.Instance.isShowQuitPanel = false;
            }
            else
            {
                GameData.Instance.isShowQuitPanel = true;
                gameObject.SetActive(false);
            }
        }
      
        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        public void CloseBtn()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MESSAGE_CLOSE);
        }

        /// <summary>
        /// 检查屏蔽字
        /// </summary>
        public void CheckFOrbid()
        {
            RealNameApprovePanelData rnapd = GameData.Instance.RealNameApprovePanelData;
            names.text = rnapd.Filter(names.text);                      
        }


        /// <summary>
        /// 点击确认按钮
        /// </summary>
        public void OkBtn()
        {
            RealNameApprovePanelData rnapd = GameData.Instance.RealNameApprovePanelData;
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            anhui.MahjongCommonMethod mcm = anhui.MahjongCommonMethod.Instance;            

            if(names.text.Length<=1)
            {                
                mcm.ShowRemindFrame("名字输入错误，请重新输入");
                return;
            }
            else
            {
                names.text= rnapd.Filter(names.text);
            }           

            if (IdCard.text.Length!=18)
            {                
                mcm.ShowRemindFrame("身份证号长度错误，请检查后重新输入");
                return;
            }

            Regex regex = new Regex(@"^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}([0-9]|X)$");
            if(!regex.IsMatch(IdCard.text)||System.Convert.ToInt64(IdCard.text)== 111111111111111111)
            {                
                mcm.ShowRemindFrame("身份证号输入规范错误，请检查后重新输入");
                return;
            }
            Messenger_anhui<string, string>.Broadcast(MESSAGE_OK,names.text,IdCard.text);
        }
    }
}

