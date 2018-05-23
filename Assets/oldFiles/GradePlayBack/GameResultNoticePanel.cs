using UnityEngine;
using System.Collections;
using MahjongGame_AH;
using XLua;

namespace PlayBack_1
{
    [Hotfix]
    [LuaCallCSharp]
    public class GameResultNoticePanel : MonoBehaviour
    {

        //一局结束的结算的父物体
        public GameObject RoundGameReult;

        /// <summary>
        /// 产生四家的结算的预置体
        /// </summary>
        public void SpwanGameReult_Round(byte[] winSeat)
        {
            Debug.LogWarning("产生四家的结算预置体");
            //在每次产生之前先删除之前的预置体
            if (MahjongLobby_AH.GameData.Instance.PlayBackData.iPlayBackVersion == MahjongLobby_AH.GameData.Instance.PlayBackData.iPbVersion_New)
            {
                RoundGameResultPrefab[] prefab = RoundGameReult.transform.GetComponentsInChildren<RoundGameResultPrefab>();
                if (prefab.Length > 0)
                {
                    for (int i = 0; i < prefab.Length; i++)
                    {
                        Destroy(prefab[i].gameObject);
                    }
                }
            }
            else
            {
                RoundGameResultPrefab_2[] prefab = RoundGameReult.transform.GetComponentsInChildren<RoundGameResultPrefab_2>();
                if (prefab.Length > 0)
                {
                    for (int i = 0; i < prefab.Length; i++)
                    {
                        Destroy(prefab[i].gameObject);
                    }
                }

            }


            string win_pb = "Game/Ma/PlayBack/RoundPlayerResult_Win_Pb";
            string win_normal = "Game/Ma/PlayBack/RoundPlayerResult_Normal_Pb";
            if (MahjongLobby_AH.GameData.Instance.PlayBackData.iPlayBackVersion == MahjongLobby_AH.GameData.Instance.PlayBackData.iPbVersion_Old)
            {
                win_pb = "Game/Ma/PlayBack/RoundPlayerResult_Win_Pb_2";
                win_normal = "Game/Ma/PlayBack/RoundPlayerResult_Normal_Pb_2";
            }

            gameObject.SetActive(true);
            for (int i = 0; i < 4; i++)
            {
                GameObject go = null;
                //通过座位号判断玩家信息是否是赢家
                if (winSeat[i] > 0)
                {
                    go = Instantiate(Resources.Load<GameObject>(win_pb));
                    go.transform.SetParent(RoundGameReult.transform);
                    go.transform.SetAsFirstSibling();
                }
                else
                {
                    go = Instantiate(Resources.Load<GameObject>(win_normal));
                    go.transform.SetParent(RoundGameReult.transform);
                }
                go.transform.localScale = Vector3.one;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);

                if (MahjongLobby_AH.GameData.Instance.PlayBackData.iPlayBackVersion == MahjongLobby_AH.GameData.Instance.PlayBackData.iPbVersion_New)
                {
                    RoundGameResultPrefab gameRes = go.GetComponent<RoundGameResultPrefab>();
                    gameRes.iseatNum = i + 1;
                    gameRes.MessageVlaue();
                    gameRes.SpwanPlayerCard();
                }
                else if (MahjongLobby_AH.GameData.Instance.PlayBackData.iPlayBackVersion == MahjongLobby_AH.GameData.Instance.PlayBackData.iPbVersion_Old)
                {
                    RoundGameResultPrefab_2 gameRes = go.GetComponent<RoundGameResultPrefab_2>();
                    gameRes.iseatNum = i + 1;
                    gameRes.MessageVlaue();
                    gameRes.SpwanPlayerCard();
                }

            }
        }


        /// <summary>
        /// 点击返回大厅按钮
        /// </summary>
        public void BtnReturnLobby()
        {
            //销毁当前场景
            PlayBack_1.PlayBackData.isComePlayBack = false;
            UnityEngine.SceneManagement.SceneManager.UnloadScene(4);
            Time.timeScale = 1;
        }

        /// <summary>
        /// 点击重播按钮
        /// </summary>
        public void BtnReplay()
        {
            gameObject.SetActive(false);
            PlayBackMahjongPanel.Instance.BtnRePlay();
        }
    }

}
