using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MahjongLobby_AH.Data;
using MahjongLobby_AH.Network.Message;
using XLua;
using anhui;

namespace MahjongLobby_AH
{

    [Hotfix]
    [LuaCallCSharp]
    public class ScoreExChange : MonoBehaviour
    {
        public Text Score; //玩家业绩积分
        public Transform Parent; //父物体
        public Sprite[] CoinImages; //金币图片


        public void UpdateShow_Score()
        {
            Score.text = ((float)GameData.Instance.PlayerNodeDef.userDef.fParlorScore / 100f).ToString("0.00");
        }

        public void UpdateShow()
        {
            Score.text = ((float)GameData.Instance.PlayerNodeDef.userDef.fParlorScore / 100f).ToString("0.00");
            gameObject.SetActive(true);
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            Button[] btn = Parent.GetComponentsInChildren<Button>(true);
            List<Button> lsBtn = new List<Button>();
            for (int i = 0; i < btn.Length; i++)
            {
                if (btn[i].name.Contains("ExChange"))
                {
                    lsBtn.Add(btn[i]);
                }
            }

            int count = (lsBtn.Count > MahjongCommonMethod.Instance.ExChange.Count) ? lsBtn.Count : MahjongCommonMethod.Instance.ExChange.Count;
            for (int i = 0; i < count; i++)
            {
                string content = MahjongCommonMethod.Instance.ExChange[i].iExchangeId + "_" + MahjongCommonMethod.Instance.ExChange[i].iType;
                if (i > lsBtn.Count - 1)
                {
                    GameObject go = Instantiate(lsBtn[0].gameObject);
                    go.transform.SetParent(Parent);
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                    go.transform.localScale = Vector3.one;
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = MahjongCommonMethod.Instance.ExChange[i].iCoin + "金币";
                    go.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = MahjongCommonMethod.Instance.ExChange[i].iBindCoin + "金币";
                    go.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = MahjongCommonMethod.Instance.ExChange[i].iAsset + "业绩";
                    go.transform.GetChild(1).GetComponent<Image>().sprite = CoinImages[i];
                    //添加点击方法               
                    go.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate () { BtnClick(content); });
                }
                else
                {
                    if (i > MahjongCommonMethod.Instance.ExChange.Count - 1)
                    {
                        lsBtn[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        lsBtn[i].gameObject.SetActive(true);
                        lsBtn[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = MahjongCommonMethod.Instance.ExChange[i].iCoin + "金币";
                        lsBtn[i].transform.GetChild(3).GetChild(1).GetComponent<Text>().text = MahjongCommonMethod.Instance.ExChange[i].iBindCoin + "金币";
                        lsBtn[i].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = MahjongCommonMethod.Instance.ExChange[i].iAsset + "业绩";
                        lsBtn[i].transform.GetChild(1).GetComponent<Image>().sprite = CoinImages[i];
                        //添加点击方法               
                        lsBtn[i].transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate () { BtnClick(content); });
                    }
                }
            }
        }


        void OnDisable()
        {
            Button[] btn = Parent.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < btn.Length; i++)
            {
                btn[i].onClick.RemoveAllListeners();
            }
        }

        /// <summary>
        /// 点击兑换金币
        /// </summary>
        public void BtnClick(string content)
        {
            int ExchangeId = System.Convert.ToInt32(content.Split('_')[0]);
            int type = System.Convert.ToInt32(content.Split('_')[1]);
            NetMsg.ClientScoreToCoinReq msg = new NetMsg.ClientScoreToCoinReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iExchangeId = ExchangeId;
            msg.byExchange = (byte)type;
            Network.NetworkMgr.Instance.LobbyServer.SendClientScoreToCoinReq(msg);
        }
    }
}
