using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.IO;
using System.Text;
using MahjongLobby_AH;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewShortTalkPanel : MonoBehaviour
    {
        public GameObject _gPanelchat;
        public GameObject[] _gChildPanel;
        public Image[] _imageColor;
        public const string MessageOpenChatPenal = "MainViewShortTalkPanel.Message_OpenChatPenal";
        public const string MessageCloseChatPenal = "MainViewShortTalkPanel.Message_CloseChatPenal";
        public const string MessageClickEmotion = "MainViewShortTalkPanel.Message_ClickEmotion";
        void Start()
        {
            StartCoroutine(ReadTips());
        }
        void Update()
        {

        }

        internal void UpdateShow()
        {
            ShortTalkData std = GameData.Instance.ShortTalkData;
            if (std.isShowPanel)
            {
                _gPanelchat.SetActive(true);
                if (std.szShortTalk[0].Count >6&& _gChildPanel[0].activeInHierarchy )
                {
                    ig1 = _gChildPanel[0].transform.GetChild(0).GetComponent<InfinityGridLayoutGroup>();
                    ig1.SetAmount(std.szShortTalk[0].Count);
                    ig1.updateChildrenCallback = UpdateCallBack;
                }
            }
            else
            {
                _gPanelchat.SetActive(false);
            }
        }
        #region 按钮
        /// <summary>
        /// 面板切换
        /// </summary>
        /// <param name="index"></param>
        public void BtnOnChildPanelChange(int index)
        {
            for (int i = 0; i < _gChildPanel.Length; i++)
            {
                _gChildPanel[i].SetActive(false);
                _imageColor[index].color = new Color(Color.white.r, Color.white.b, Color.white.g, 0.39f);
            }
            _gChildPanel[index].SetActive(true);
            _imageColor[index].color = Color.white;
        }
        /// <summary>
        /// 关闭面板
        /// </summary>
        public void BtnClosePanel()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(GameSystem.SubSystem.AudioSystem.AudioMenel.Btn_Click, false, false);
            Messenger_anhui.Broadcast(MessageCloseChatPenal);
        }
        /// <summary>
        /// 点击表情
        /// </summary>
        /// <param name="iconID">表情ID</param>
        public void BtnOnClickEmotion(int iconID)
        {
            SystemMgr.Instance.AudioSystem.PlayManual(GameSystem.SubSystem.AudioSystem.AudioMenel.Btn_Click, false, false);
            Messenger_anhui<int>.Broadcast(MessageClickEmotion, iconID);
            Messenger_anhui.Broadcast(MessageCloseChatPenal);
        }
        #endregion 按钮

        #region Methord
        /// <summary>
        /// 当收到语音消息通知
        /// </summary>
        /// <param name="url"></param>
        /// <param name="time"></param>
        /// <param name="HasRead"></param>
        /// <param name="isSelf"></param>
        /// <param name="szurl">头像地址</param>
        internal void OnReceiveVoice(string url, float time, bool HasRead, bool isSelf, string szurl,byte []id)
        {
          //  ig2 = _gChildPanel[2].transform.GetChild(0).GetComponent<InfinityGridLayoutGroup>();
            ShortTalkData std = GameData.Instance.ShortTalkData;
            GameObject obj = Instantiate(Resources.Load<GameObject>("Game/ShortTalk/VoiceMessage"));
            obj.transform.SetParent(_gChildPanel[2].transform.GetChild(0));
            obj.transform.localEulerAngles = Vector3.zero;
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0);
            obj.transform.localScale = Vector3.one;
            PreVoiceTalkManage pvtm = obj.GetComponent<PreVoiceTalkManage>();

            pvtm._url = url;
            pvtm._time = time;
            pvtm._hasRead = HasRead;
            pvtm._isSelf = isSelf;
            pvtm._headUrl = szurl;
            pvtm._id = id;
            pvtm.UpdateShow();//更新显示条
            ShortTalkData.VoiceData _pvtm = new ShortTalkData.VoiceData();
            _pvtm._url = pvtm._url;
            _pvtm._time = pvtm._time;
            _pvtm._hasRead = pvtm._hasRead;
            _pvtm._isSelf = pvtm._isSelf;
            _pvtm._headUrl = pvtm._headUrl;
            std.AddVoiceData(_pvtm);
            //  Debug.LogError("count" + std._voiceData.Count);
            //if (std._voiceData.Count>6)
            //{
            //    ig2.SetAmount(std._voiceData.Count);
            //    ig2.updateChildrenCallback = UpdateVoiceCallBack;
            //}
        }
        InfinityGridLayoutGroup ig1;
        InfinityGridLayoutGroup ig2;
        //InfinityGridLayoutGroup ig3;
        void UpdateVoiceCallBack(int index, Transform trans)
        {
            ShortTalkData std = GameData.Instance.ShortTalkData;
            PreVoiceTalkManage pvt = trans.GetComponent<PreVoiceTalkManage>();
            pvt._url = std._voiceData[index]._url;
            pvt._time = std._voiceData[index]._time;
            pvt._hasRead = std._voiceData[index]._hasRead;
            pvt._isSelf = std._voiceData[index]._isSelf;
            pvt._headUrl = std._voiceData[index]._headUrl;
            // pvt._url = std._voiceData[index]._url;
            pvt.UpdateShow();
        }
        /// <summary>
        /// 在游戏一开始的时候加载快捷语
        /// </summary>
        public void CloneShortTalkPre()
        {
            int isex = SDKManager.Instance.isex;

            ShortTalkData std = GameData.Instance.ShortTalkData;
            for (int i = 0; i < std.szShortTalk[isex].Count; i++)
            {
               // Debug.LogError(std.szShortTalk[i]);
                GameObject go = Instantiate(Resources.Load<GameObject>("Game/ShortTalk/ShortTalkText"));
                go.transform.SetParent(_gChildPanel[0].transform.GetChild(0));
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                go.transform.localScale = Vector3.one;
                go.GetComponent<PreShortTalkManage>()._text.text = std.szShortTalk[isex ][i];
                go.GetComponent<PreShortTalkManage>()._mID = i;
            }
        }
        public IEnumerator ReadTips()
        {
            for (int isex = 0; isex < 2; isex++)
            {
                string path2 = "";
                if (isex == 1)
                {
                    path2 = "/BShortTalk.txt";
                }
                else
                {
                    path2 = "/GShortTalk.txt";
                }
                string path =
            // path = Application.streamingAssetsPath + "/ShortTalk.txt";
#if UNITY_ANDROID && !UNITY_EDITOR
         Application.streamingAssetsPath + path2; 
#elif UNITY_IPHONE && !UNITY_EDITOR
          Application.streamingAssetsPath + path2;
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            Application.dataPath + "/StreamingAssets" + path2;
#else
        string.Empty;  
#endif
                ShortTalkData std = GameData.Instance.ShortTalkData;
                
                if (path.Contains("://"))
                {
                    WWW www = new WWW(path);

                    while (!www.isDone)
                    {
                        yield return www;
                    }

                    if (www.error != null)
                    {
                        Debug.LogWarning("读取ShortTalk.txt文件失败:" + www.error);
                    }
                    //获取tips中的所有内容
                    string[] str = www.text.Split('\n');

                    for (int i = 0; i < str.Length; i++)
                    {
                        std.szShortTalk[isex].Add(str[i]);
                    }
                }
                else
                {
                    string[] str = (System.IO.File.ReadAllText(path)).Split('\n');
                    for (int i = 0; i < str.Length; i++)
                    {
                        std.szShortTalk[isex].Add(str[i]);
                    }
                }
            }
            CloneShortTalkPre();
        }
        void UpdateCallBack(int index, Transform tran)
        {
            PreShortTalkManage rstm = tran.GetComponent<PreShortTalkManage>();
            rstm._text.text = GameData.Instance.ShortTalkData.szShortTalk[SDKManager.Instance.isex][index];
            rstm._mID = index;
        }

        #endregion Methord
    }

}
