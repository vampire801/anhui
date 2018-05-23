using DG.Tweening;
using MahjongGame_AH.Data;
using MahjongGame_AH.GameSystem.SubSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewPlayingRule : MonoBehaviour
    {
        public GameObject BackGround;//黑色的背景需不需要显示出来

        public Text PlayName;//玩法名称

        public GameObject MethodParent;//实例化预制体之后的父物体
        public GameObject Method;//预制体

        public Image m_imMoveBGLeft;//左边可移动面板
        public Image m_imMoveBGRight;//右边可移动面板

        public GameObject m_gAllMoveUp;//上跟着移动
        public GameObject m_gAllMoveDown;//下跟着移动

        private bool FirstOnce = true;//第一次进入游戏


        /// <summary>
        /// 显示这个玩法规则的面板
        /// </summary>
        public void UpdateShowMethod()
        {
            int index = anhui.MahjongCommonMethod.Instance.iPlayingMethod;
            if (index <= 0)
            {
                return;
            }

            gameObject.SetActive(true);
            gameObject.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
            gameObject.transform.GetChild(0).transform.position = new Vector3(0, 0, gameObject.transform.GetChild(0).transform.position.z);
            gameObject.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);

            if (UIMainView.Instance.PlayerPlayingPanel.m_gameStart)
            {
                BackGround.GetComponent<RawImage>().enabled = true;
            }
            else
            {
                BackGround.GetComponent<RawImage>().enabled = false;
            }

            SpwanPayNum(index);
            PlayName.text = anhui.MahjongCommonMethod.Instance._dicMethodConfig[index].METHOD_NAME;
        }

        /// <summary>
        /// 产生玩法数量的预置体
        /// </summary>
        /// <param name="index"></param>
        void SpwanPayNum(int index)
        {
            if (MethodParent.GetComponentsInChildren<Transform>().Length > 1)
            {
                return;
            }
            int AllCol = 0;
            //   Debug.Log("玩法" + index);
            anhui.MahjongCommonMethod mcm = anhui.MahjongCommonMethod.Instance;
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;
            MahjongLobby_AH.Data.CreatRoomMessagePanelData cd = MahjongLobby_AH.GameData.Instance.CreatRoomMessagePanelData;

            #region 分数
            GameObject fenshu = Instantiate(Method) as GameObject;
            fenshu.transform.SetParent(MethodParent.transform);
            fenshu.transform.localPosition = Vector3.zero;
            fenshu.transform.localRotation = Quaternion.identity;
            fenshu.transform.localScale = Vector3.one;
            MainViewGameRulePlayMethod method_fenshu = fenshu.GetComponent<MainViewGameRulePlayMethod>();
            method_fenshu.transform.GetChild(1).GetComponent<GridLayoutGroup>().cellSize = new Vector2(66.0f, method_fenshu.transform.GetChild(1).GetComponent<GridLayoutGroup>().cellSize.y);
            method_fenshu.Name.text = "分数:";
            //分数
            GameObject method_fenshu_Select = Instantiate(method_fenshu.GameRuleName) as GameObject;
            method_fenshu_Select.transform.SetParent(method_fenshu.Parent);
            method_fenshu_Select.transform.localPosition = Vector3.zero;
            method_fenshu_Select.transform.localRotation = Quaternion.identity;
            method_fenshu_Select.transform.localScale = Vector3.one;

            string str_method_fenshu_Select = "";
            if (pppd.playingMethodConf.byBillingMode == 1)
                str_method_fenshu_Select = pppd.playingMethodConf.byBillingNumber + "圈";
            else if (pppd.playingMethodConf.byBillingMode == 2)
                str_method_fenshu_Select = pppd.playingMethodConf.byBillingNumber + "局";
            else if (pppd.playingMethodConf.byBillingMode == 3)
                str_method_fenshu_Select = pppd.playingMethodConf.byBillingNumber + "分";

            //Debug.LogError("局"+pppd.playingMethodConf.byBillingNumber);
            method_fenshu_Select.transform.GetChild(0).gameObject.SetActive(false);
            method_fenshu_Select.GetComponentsInChildren<Text>()[0].text = str_method_fenshu_Select;
            //需要的金币
            GameObject method_fenshu_Gold = Instantiate(method_fenshu.GameRuleGold) as GameObject;
            method_fenshu_Gold.transform.SetParent(method_fenshu.Parent);
            method_fenshu_Gold.transform.localPosition = Vector3.zero;
            method_fenshu_Gold.transform.localRotation = Quaternion.identity;
            method_fenshu_Gold.transform.localScale = Vector3.one;
            method_fenshu_Gold.GetComponentsInChildren<Text>()[0].text = "X" + pppd.playingMethodConf.byBillingPrice.ToString();
            AllCol++;
            #endregion

            #region 支付方式
            GameObject zhifufangshi = Instantiate(Method) as GameObject;
            zhifufangshi.transform.SetParent(MethodParent.transform);
            zhifufangshi.transform.localPosition = Vector3.zero;
            zhifufangshi.transform.localRotation = Quaternion.identity;
            zhifufangshi.transform.localScale = Vector3.one;
            MainViewGameRulePlayMethod method_zhifufangshi = zhifufangshi.GetComponent<MainViewGameRulePlayMethod>();
            method_zhifufangshi.Name.text = "房费:";
            //支付方式
            GameObject method_zhifufangshi_Select = Instantiate(method_zhifufangshi.GameRuleName) as GameObject;
            method_zhifufangshi_Select.transform.SetParent(method_zhifufangshi.Parent);
            method_zhifufangshi_Select.transform.localPosition = Vector3.zero;
            method_zhifufangshi_Select.transform.localRotation = Quaternion.identity;
            method_zhifufangshi_Select.transform.localScale = Vector3.one;
            method_zhifufangshi_Select.transform.GetChild(0).gameObject.SetActive(false);
            if (anhui.MahjongCommonMethod.Instance.ReadColumnValue(cd.roomMessage_, 2, 39) <= 1)
            {
                method_zhifufangshi_Select.GetComponentsInChildren<Text>()[0].text = "四家支付";
            }
            else if(anhui.MahjongCommonMethod.Instance.ReadColumnValue(cd.roomMessage_, 2, 39)==2)
            {
                method_zhifufangshi_Select.GetComponentsInChildren<Text>()[0].text = "房主支付";
            }
            
            AllCol++;
            #endregion


            #region 玩法            
            if (mcm._dicMethodCardType.ContainsKey(index))
            {
                int count = 0;
                int count_jibu = 5;
                GameObject wanfa = Instantiate(Method) as GameObject;
                wanfa.transform.SetParent(MethodParent.transform);
                wanfa.transform.localPosition = Vector3.zero;
                wanfa.transform.localRotation = Quaternion.identity;
                wanfa.transform.localScale = Vector3.one;
                MainViewGameRulePlayMethod method_wanfa = wanfa.GetComponent<MainViewGameRulePlayMethod>();
                method_wanfa.transform.GetChild(1).GetComponent<GridLayoutGroup>().padding.left = 8;
                method_wanfa.Name.text = "玩法:";
                AllCol++;

                for (int i = 0; i < mcm._dicMethodCardType[index].Count; i++)
                {
                   // Debug.LogWarning("规则：" + mcm._dicmethodToCardType[index][i].RuleId);
                    if (mcm. JudgeIsShow(mcm._dicmethodToCardType[index][i].RuleId))
                    {
                        string name = mcm._cardType_[mcm._dicmethodToCardType[index][i].RuleId].card_type;
                      //  Debug.Log(mcm._cardType_[mcm._dicmethodToCardType[index][i].RuleId].card_type);
                        GameObject method_wanfa_Select = Instantiate(method_wanfa.GameRuleName) as GameObject;
                        method_wanfa_Select.transform.SetParent(method_wanfa.Parent);
                        method_wanfa_Select.transform.localPosition = Vector3.zero;
                        method_wanfa_Select.transform.localRotation = Quaternion.identity;
                        method_wanfa_Select.transform.localScale = Vector3.one;
                        method_wanfa_Select.GetComponentsInChildren<Text>()[0].text = name;
                        count++;

                        if (count == count_jibu)
                        {
                            GameObject wanfa_1 = Instantiate(Method) as GameObject;
                            wanfa_1.transform.SetParent(MethodParent.transform);
                            wanfa_1.transform.localPosition = Vector3.zero;
                            wanfa_1.transform.localRotation = Quaternion.identity;
                            wanfa_1.transform.localScale = Vector3.one;
                            MainViewGameRulePlayMethod method_wanfa_1 = wanfa_1.GetComponent<MainViewGameRulePlayMethod>();
                            method_wanfa_1.Name.text = "";
                            AllCol++; count_jibu += 4;
                        }
                    }
                }
            }

            #endregion


            #region 托管
            if (cd.roomMessage_[5] > 0)
            {
                GameObject tuoguan = Instantiate(Method) as GameObject;
                tuoguan.transform.SetParent(MethodParent.transform);
                tuoguan.transform.localPosition = Vector3.zero;
                tuoguan.transform.localRotation = Quaternion.identity;
                tuoguan.transform.localScale = Vector3.one;
                MainViewGameRulePlayMethod method_tuoguan = tuoguan.GetComponent<MainViewGameRulePlayMethod>();
                method_tuoguan.Name.text = "托管:";
                //托管
                GameObject method_tuoguan_Select = Instantiate(method_tuoguan.GameRuleName) as GameObject;
                method_tuoguan_Select.transform.SetParent(method_tuoguan.Parent);
                method_tuoguan_Select.transform.localPosition = Vector3.zero;
                method_tuoguan_Select.transform.localRotation = Quaternion.identity;
                method_tuoguan_Select.transform.localScale = Vector3.one;
                method_tuoguan_Select.transform.GetChild(0).gameObject.SetActive(false);
                method_tuoguan_Select.GetComponentsInChildren<Text>()[0].text = (cd.roomMessage_[5] * 60) + "秒";
                AllCol++;
            }
            #endregion

            if (AllCol >= 4)
            {
                int leng = 20;
                leng = ((AllCol - 4) + 1) * 25;
                //背景框面板
                m_imMoveBGLeft.rectTransform.offsetMin = new Vector2(m_imMoveBGLeft.rectTransform.offsetMin.x, m_imMoveBGLeft.rectTransform.offsetMin.y - leng);
                m_imMoveBGLeft.transform.localPosition = new Vector3(m_imMoveBGLeft.transform.localPosition.x, 0);
                m_imMoveBGRight.rectTransform.offsetMin = new Vector2(m_imMoveBGRight.rectTransform.offsetMin.x, m_imMoveBGRight.rectTransform.offsetMin.y - leng);
                m_imMoveBGRight.transform.localPosition = new Vector3(m_imMoveBGRight.transform.localPosition.x, 0);

                //其他面板
                m_gAllMoveUp.transform.localPosition = new Vector3(m_gAllMoveUp.transform.localPosition.x, leng, m_gAllMoveUp.transform.localPosition.z);
                m_gAllMoveDown.transform.localPosition = new Vector3(m_gAllMoveUp.transform.localPosition.x, -(leng - leng / 4), m_gAllMoveUp.transform.localPosition.z);
            }

        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        public void BtnClose()
        {
            FirstOnce = false;
            StopCoroutine(OnShowFirstComeInGame());
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 规则的面板
        /// </summary>
        public void BtnSeeRule()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(AudioSystem.AudioMenel.Btn_Click, false, false);
            int index = anhui.MahjongCommonMethod.Instance.iPlayingMethod;
            if (index <= 0)
            {
                return;
            }
            Messenger_anhui<int>.Broadcast(MainViewGameRulePanel.MESSAGE_OPEN, index);
        }

        /// <summary>
        /// 显示第一次进入游戏的时候显示一次
        /// </summary>
        /// <returns></returns>
        public IEnumerator OnShowFirstComeInGame()
        {
            if (gameObject.activeSelf == true) yield break;
            //显示房间的规则
            UpdateShowMethod();

            yield return new WaitForSeconds(3.0f);

            if (FirstOnce == false) yield break;

            transform.GetChild(0).transform.DOLocalMove(new Vector3(550 + 640, 380 + 360, 0), 0.5f);//(UIMainView.Instance.PlayerPlayingPanel.RuleButton.localPosition,0.5f);
            transform.GetChild(0).transform.DOScale(0, 0.5f);
            if (transform.GetChild(0).GetComponent<CanvasGroup>() == null)
                transform.GetChild(0).gameObject.AddComponent<CanvasGroup>();
            transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
            yield return new WaitForSeconds(0.1f);
            transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0.7f;
            yield return new WaitForSeconds(0.1f);
            transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0.4f;
            yield return new WaitForSeconds(0.1f);
            transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0.1f;
            yield return new WaitForSeconds(0.3f);
            gameObject.SetActive(false);
            FirstOnce = false;
        }
    }
}
