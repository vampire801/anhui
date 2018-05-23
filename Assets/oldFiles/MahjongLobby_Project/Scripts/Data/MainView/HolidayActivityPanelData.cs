using MahjongLobby_AH.Network.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class HolidayActivityPanelData
    {

        public const string URL = "active.x?a={0}";

        public CommonConfig.Json_Activity jsActive = new CommonConfig.Json_Activity();
        public Stack stackID = new Stack();

        public bool isPanelShow;
        ////0-24免 1节日
        //public bool[] isGiftCanSave = new bool[2];
        //public NetMsg.HolidayDef HolidayDef = new NetMsg.HolidayDef();
        //public NetMsg.FreeTimeDef FreeTimeDef = new NetMsg.FreeTimeDef();
        //  public int iHolidayId;
        //public bool[] _isClicked = new bool[PanelsNumber];
        internal const string strholiday = "holiday{0}";  //0-false,1-true
        /// <summary>
        /// 根据活动编号获取是不是显示红点
        /// </summary>
        /// <param name="idex"></param>
        /// <returns></returns>
        public bool GetIsRed(int idex)
        {
            if (PlayerPrefs.HasKey(string.Format(strholiday, idex)))
            {
                return false;
            }
            return true ;
        }
        /// <summary>
        /// 设置红点是否提示
        /// </summary>
        /// <param name="index"></param>
        /// <param name="is0"></param>
        public void SetIsRed(int index, bool is0, GameObject obj = null)
        {
            if (obj)
                obj.SetActive(is0);
            ///0
            int num = 0;//未选择
            if (is0)
            {
                PlayerPrefs.DeleteKey(string.Format(strholiday, index));
            }
            else
            {
                PlayerPrefs.SetInt(string.Format(strholiday, index), num);
            }
        }
        public bool IsShowLobbyRed()
        {
            if ( GetIsRed(-2017))
            {
                return GetIsRed(-2017);
            }
            if (jsActive.data == null)
            {
                return false ;
            }
            for (int i = 0; i < jsActive.data.Length; i++)
            {
                // UnityEngine.Debug.LogError("_isRedList:  " + GetIsRed(i));
                if (GetIsRed(jsActive.data[i].ACTIVITY_ID ))
                {
                    return true ;
                }
            }
            return false;
        }
    }





}
