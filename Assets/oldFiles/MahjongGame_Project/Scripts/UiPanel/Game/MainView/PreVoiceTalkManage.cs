using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using YunvaIM;
using MahjongGame_AH.Data;
using gcloud_voice;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class PreVoiceTalkManage : MonoBehaviour
    {

        //0-自己  1-别人
        public GameObject[] _gFrameWorkLR;
        public RectTransform[] _rect;
        //语音地址
        internal string _url;
        internal float _time;
        internal bool _hasRead;
        internal bool _isSelf;
        //头像地址
        internal string _headUrl;
        internal byte[] _id;

        public PreVoiceTalkManage()
        {

        }
        public void init()
        {

        }
        public void UpdateShow()
        {
            _gFrameWorkLR[0].SetActive(_isSelf);//0-self
            _gFrameWorkLR[1].SetActive(!_isSelf);
            if (_isSelf)
            {
                _rect[0].GetChild(2).gameObject.SetActive(!_hasRead);
                _rect[0].GetChild(1).GetComponent<Text>().text = Convert.ToUInt16(_time).ToString();
                //语音框长短设置
                _rect[0].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 105 + (_time - 0.5f) * 15.2631f);//Max250-Min105
                anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(_gFrameWorkLR[0].transform.GetChild(0).GetChild(0).GetComponent<RawImage>(), _headUrl);
            }
            else
            {
                _rect[1].GetChild(2).gameObject.SetActive(!_hasRead);
                _rect[1].GetChild(1).GetComponent<Text>().text = Convert.ToUInt16(_time).ToString();
                //语音框长短设置
                _rect[1].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 105 + (_time - 0.5f) * 15.2631f);//Max250-Min105
                anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(_gFrameWorkLR[1].transform.GetChild(0).GetChild(0).GetComponent<RawImage>(), _headUrl);
            }
        }
        public void Clicked()
        {
            _hasRead = true;
            if (_isSelf)
            {
                _rect[0].GetChild(2).gameObject.SetActive(!_hasRead);
            }
            else
            {
                _rect[1].GetChild(2).gameObject.SetActive(!_hasRead);
            }
            //此处播放声音
            PlayRecordVoice();
        }
        //IEnumerator DownLoadVoice()
        //{
        //    yield return new WaitUntil(() =>
        //    {
        //        bool aa = false;
        //        aa = DownVoice();
        //        Debug.LogError("是否下载完" + aa);
        //        return aa;
        //    });
        //    Debug.LogError("----------下载完播放");
        //    PlayRecordVoice();
        //}
        /// <summary>
        /// 播放声音
        /// </summary>
        public void PlayRecordVoice()
        {
            ShortTalkData std = GameData.Instance.ShortTalkData;
            if (!std._DownLoadFilePath.ContainsKey(_url))//如果未下载
            {
                Debug.LogWarning("未下载");
                DownVoice();
                //StartCoroutine(DownLoadVoice());
                return;
            }
            PlayerPlayingPanelData pppd = GameData.Instance.PlayerPlayingPanelData;         
            Debug.LogWarning("开播");
            string ext = DateTime.Now.ToFileTime().ToString();
            if (VoiceManegerMethord.Instance.isPlayingState)
            {
                VoiceManegerMethord.Instance.StopPlayingVoice();
            }
            AudioListener .pause  = true  ;
            if (VoiceManegerMethord.VoiceFlag == 1)
            {
                int downLoadErr = VoiceManegerMethord.Instance.m_voiceengine.PlayRecordedFile(std._DownLoadFilePath[_url]);
                VoiceManegerMethord.Instance.ErroOccur(downLoadErr, null);
                VoiceManegerMethord.Instance.m_voiceengine.OnPlayRecordFilComplete += (code, filePath) =>
                {
                    if (code != IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_PLAYFILE_DONE)
                    {
                        Debug.Log("OnPlayRecordFilComplete error" + code);
                    }
                    else
                    {
                        Debug.LogError("播放失败:"+ code);//关静音
                        //MahjongCommonMethod.Instance.ShowRemindFrame("语音播放失败");
                    }
                    AudioListener.pause = false;
                };
            }
            else
            {
            YunVaImSDK.instance.RecordStartPlayRequest(std._DownLoadFilePath[_url], "", ext, (data2) =>
            {
                if (data2.result == 0)
                {
                    // PanlePlayers[panelNum].FindChild("P" + pwpd.GetOtherPlayerShowPos(pwpd.GetOtherPlayerPos(voices[0].iUserId)).ToString()).FindChild("VoicePic").GetComponent<SpriteRenderer>().enabled = false;
                    Debug.Log("播放成功");
                }
                else
                {
                    Debug.LogError("播放失败");//关静音
                    anhui.MahjongCommonMethod.Instance.ShowRemindFrame("语音播放失败");
                }
                // UIMainView.Instance.PlayerPlayingPanel._playingHead[pppd.GetOtherPlayerShowPos(pppd.GetOtherPlayerPos(voices[0].iUserId)) - 1].GetChild(4).GetComponent<Animator>().SetBool("isPlayOthers", false);
                AudioListener.pause  =false ;
            });
            }
        }
        /// <summary>
        /// 当点击播放记录的时候下载到本地 
        /// </summary>
        public void DownVoice()
        {
            //bool bb = false;
            string DownLoadlocalfilePath = string.Format("{0}{1}.amr", VoiceManegerMethord.Instance.isExistGvoiceFile(MahjongLobby_AH.SDKManager.DataPath), DateTime.Now.ToFileTime());
            ShortTalkData std = GameData.Instance.ShortTalkData;
            if (!std._DownLoadFilePath.ContainsKey(_url))
            {
                std._DownLoadFilePath.Add(_url, DownLoadlocalfilePath);
            }
            string fileid = DateTime.Now.ToFileTime().ToString();
            if (VoiceManegerMethord.VoiceFlag == 1)
            {
                int downLoadErr = VoiceManegerMethord.Instance.m_voiceengine.DownloadRecordedFile(VoiceManegerMethord.ByteArrayToHexString(_id), DownLoadlocalfilePath, 6000);
            }
            else
            {
                YunVaImSDK.instance.DownLoadFileRequest(_url, DownLoadlocalfilePath, fileid, (data4) =>
                {
                    if (data4.result == 0)
                    {
                        Debug.Log("下载成功:" + data4.filename);
                        PlayRecordVoice();
                        // bb = true;
                    }
                    else
                    {
                        Debug.Log("下载失败:" + data4.msg);
                        //bb = false;
                    }
                });
            }

            //return bb;
        }
    }



}
