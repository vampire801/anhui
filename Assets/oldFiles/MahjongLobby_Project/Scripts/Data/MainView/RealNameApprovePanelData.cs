using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class RealNameApprovePanelData
    {
        /// <summary>
        /// 面板是否显示的标志位
        /// </summary>
        public bool PanelShow;

        /// <summary>
        /// 玩家实名认证的名字
        /// </summary>
        public string Name;

        /// <summary>
        /// 玩家实名认证的身份证号码
        /// </summary>
        public string IdCard;

        /// <summary>
        /// 是否已经读取过屏蔽字
        /// </summary>
        public bool isReadForbidFont;

        //保存禁止字
        public List<string> ForbiddenNameList = new List<string>();
        public TextAsset ForbiddenName = new TextAsset();
        public string[] sForbiddenName;


        /// <summary>
        /// 读取禁用名字
        /// </summary>
        public void LoadForbiddenName()
        {
            if (isReadForbidFont)
            {
                return;
            }

            ForbiddenNameList.Clear();
            string PathName = "Json/forbid_name";

            ForbiddenName = Resources.Load<TextAsset>(PathName);
            sForbiddenName = ForbiddenName.text.Split('\n');
            for (int i = 0; i < sForbiddenName.Length; i++)
            {
                ForbiddenNameList.Add(sForbiddenName[i]);
            }

            isReadForbidFont = true;
        }

        /// <summary>
        /// 过滤输入的文字
        /// </summary>
        /// <param name="msg">Input输入的文字</param>
        /// <returns></returns>
        public string Filter(string msg)
        {
            GameData gd = GameData.Instance;
            for (int i = 0; i < ForbiddenNameList.Count; i++)
            {

                if (msg.IndexOf(ForbiddenNameList[i]) >= 0)
                {
                    string replaceWords = "";
                    int leng = ForbiddenNameList[i].Length;
                    for (int j = 0; j < leng; j++)
                    {
                        replaceWords += "*";
                    }
                    msg = msg.Replace(ForbiddenNameList[i], replaceWords);
                }

            }
            return msg;
        }
    }

}
