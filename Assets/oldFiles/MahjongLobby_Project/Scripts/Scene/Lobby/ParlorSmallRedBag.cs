using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class ParlorSmallRedBag : MonoBehaviour
    {
        public RawImage Head;
        public Text NickName;
        public Text Get;
        public Image[] Status;

        public Data.ParlorShowPanelData.ParlorRedBagMessage info = new Data.ParlorShowPanelData.ParlorRedBagMessage();


        public void UpdateShow(Data.ParlorShowPanelData.ParlorRedBagMessage info_)
        {
            info = info_;
            Data.ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(Head, info.head);
            NickName.text = info.nickname;

            string type = " ";
            switch (info.assetType)
            {
                case "1":
                    type = "元现金";
                    break;
                case "2":
                    type = "元话费";
                    break;
                case "3":
                    type = "M";
                    break;
                case "4":
                    type = "元储值卡";
                    break;
                case "5":
                    type = "元代金券";
                    break;
                case "6":
                    type = "赠币";
                    break;
            }

            Get.text = info.assetNum;
            //最大
            if (info.rank == 2)
            {
                Status[0].gameObject.SetActive(true);
                Status[1].gameObject.SetActive(false);
            }
            //最小
            else if (info.rank == 1)
            {
                Status[0].gameObject.SetActive(false);
                Status[1].gameObject.SetActive(true);
            }
            //普通
            else
            {
                Status[0].gameObject.SetActive(false);
                Status[1].gameObject.SetActive(false);
            }
        }
    }
}
