using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.IO;
using System.Security.Cryptography;


using LitJson;
using gcloud_voice;

public class OffLineVoice : MonoBehaviour {

    
	private string m_authkey; /*this key should get from your game svr*/
	private byte[] m_ShareFileID = null; /*when send record file save in svr, we will return a fileid in OnSendFileComplete callback function, you can save it ,and download  record by this fileid*/
	private IGCloudVoice m_voiceengine = GCloudVoice.GetEngine();  //engine have init int mainscene start function

	private string m_recordpath;
	private string m_downloadpath;
	private static string m_fileid="";

	private static string s_strLog;
	private Text m_logText;
	private static bool bIsStart = false;
	// Use this for initialization
	void Start () {
		m_logText = GameObject.Find ("Canvas/Panel/Text_Log").GetComponent<Text> ();
		if (!bIsStart) {
			bIsStart = true;
            m_voiceengine.OnApplyMessageKeyComplete += (IGCloudVoice.GCloudVoiceCompleteCode code) => {
				Debug.Log ("OnApplyMessageKeyComplete c# callback");
				s_strLog += "\r\n"+"OnApplyMessageKeyComplete ret="+code;
				if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_MESSAGE_KEY_APPLIED_SUCC) {
					Debug.Log ("OnApplyMessageKeyComplete succ11");
				} else {
					Debug.Log ("OnApplyMessageKeyComplete error");
				}
			};
			m_voiceengine.OnUploadReccordFileComplete += (IGCloudVoice.GCloudVoiceCompleteCode code, string filepath, string fileid) => {
				Debug.Log ("OnUploadReccordFileComplete c# callback");
				s_strLog += "\r\n"+" fileid len="+fileid.Length+"OnUploadReccordFileComplete ret="+code+" filepath:"+filepath+" fielid:"+fileid;
				if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_UPLOAD_RECORD_DONE) {
					m_fileid = fileid;
					s_strLog+="OnUploadReccordFileComplete succ, filepath:" +" fileid len="+fileid.Length+ filepath + " fileid:" + fileid+" fileid len="+fileid.Length;
					Debug.Log ("OnUploadReccordFileComplete succ, filepath:" + filepath +" fileid len="+fileid.Length+ " fileid:" + fileid+" fileid len="+fileid.Length);
				} else {
					s_strLog+="OnUploadReccordFileComplete err, filepath:" + filepath + " fileid:" + fileid;
					Debug.Log ("OnUploadReccordFileComplete error");
				}
			};
			m_voiceengine.OnDownloadRecordFileComplete += (IGCloudVoice.GCloudVoiceCompleteCode code, string filepath, string fileid) => {
				Debug.Log ("OnDownloadRecordFileComplete c# callback");
				s_strLog += "\r\n"+"OnDownloadRecordFileComplete ret="+code+" filepath:"+filepath+" fielid:"+fileid;
				if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_DOWNLOAD_RECORD_DONE) {
					Debug.Log ("OnDownloadRecordFileComplete succ, filepath:" + filepath + " fileid:" + fileid);
				} else {
					Debug.Log ("OnDownloadRecordFileComplete error");
				}
			};
			m_voiceengine.OnPlayRecordFilComplete += (IGCloudVoice.GCloudVoiceCompleteCode code, string filepath) => {
				Debug.Log ("OnPlayRecordFilComplete c# callback");
				s_strLog += "\r\n"+"OnPlayRecordFilComplete ret="+code+" filepath:"+filepath;
				if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_PLAYFILE_DONE) {
					Debug.Log ("OnPlayRecordFilComplete succ, filepath:" + filepath);
				} else {
					Debug.Log ("OnPlayRecordFilComplete error");
				}
			};
			m_voiceengine.OnSpeechToText += (IGCloudVoice.GCloudVoiceCompleteCode code, string fileID, string result) => {
				Debug.Log ("OnSpeechToText c# callback");
				s_strLog += "\r\n"+"OnSpeechToText c# callback";
				if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_STT_SUCC) {
					s_strLog += "\r\n"+"OnSpeechToText succ, result="+result;
					Debug.Log ("OnSpeechToText succ, result:" + result);
				} else {
					s_strLog += "\r\n"+"OnSpeechToText error,"+code;
					Debug.Log ("OnSpeechToText error,"+code);
				}
			};
		}

		//m_recordpath = "file://"+Application.persistentDataPath + "/" + "recording.dat";
		//m_downloadpath =  "file://"+Application.persistentDataPath + "/" + "download.dat";	
		m_recordpath = Application.persistentDataPath + "/" + "recording.dat";
		m_downloadpath =  Application.persistentDataPath + "/" + "download.dat";
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log("update...");
		if (m_voiceengine == null) {
			Debug.Log ("m_voiceengine is null");
		} else {
			m_voiceengine.Poll ();
		}
		m_logText.text = s_strLog;
	}

	public void Click_btnBack()
	{
		Application.LoadLevel ("main");
	}
	public void Click_btnClearLog()
	{
		s_strLog = "";
	}
	public void Click_btnReqAuthKey()
	{
		Debug.Log("ApplyMessageKey btn click");
		m_voiceengine.ApplyMessageKey (15000);
	}

	public void Click_btnStartRecord()
	{
		Debug.Log("startrecord btn click, recordpath="+m_recordpath);
		m_voiceengine.StartRecording (m_recordpath);

	}
	public void Click_btnStopRecord()
	{
		Debug.Log("stoprecord btn click");
		m_voiceengine.StopRecording ();
	}
	public void Click_btnUploadFile()
	{
		//GcloudVoice.GcloudVoiceErrno err;
		//err = m_voiceengine.SendRecordFile (m_recordpath, 15000);
		//PrintLog ("upload file with ret=" + err);
		//s_strLog += "\r\n upload file with ret=="+err;
		int ret1 = m_voiceengine.UploadRecordedFile (m_recordpath, 60000);
		Debug.Log ("Click_btnUploadFile file with ret==" + ret1);
	}
	public void Click_btnDownloadFile()
	{
		//GcloudVoice.GcloudVoiceErrno err;
		//err = m_voiceengine.DownRecordFile (m_ShareFileID, m_downloadpath, 5000);
		//PrintLog ("download file with ret=" + err);
		//s_strLog += "\r\n download file with ret=="+err;
		//m_fileid = "306b02010004643062020100042433343664633236302d613163302d313165362d386264392d66353435376438313232353602037a1afd02047d16a3b402045826b397042033363236333939656531366162666333396561376439613238373432383135380201000201000400";
		int ret = m_voiceengine.DownloadRecordedFile (m_fileid, m_downloadpath, 60000);
		s_strLog += "\r\n download file with ret=="+ret+" fileid="+m_fileid+" downpath"+m_downloadpath;
	}
	public void Click_btnPlayReocrdFile()
	{

		int err;
		if (false && m_ShareFileID == null) {
			//UnityEditor.EditorUtility.DisplayDialog("", "you have not download record file ,we will play local record files", "OK");
			err = m_voiceengine.PlayRecordedFile(m_recordpath);
			PrintLog ("downloadpath is nill, play local record file with ret=" + err);
			return;
		}
		err = m_voiceengine.PlayRecordedFile(m_downloadpath);
		PrintLog ("playrecord file with ret=" + err);

		//m_voiceengine.PlayRecordedFile (m_downloadpath);
	}
	public void Click_btnStopPlayRecordFile()
	{
		//GcloudVoice.GcloudVoiceErrno err;
		//err = m_voiceengine.StopPlayFile ();
		//PrintLog ("stopplay file with ret=" + err);
		//m_voiceengine.StopPlayFile ();
		m_fileid = "306b02010004643062020100042433343664633236302d613163302d313165362d386264392d66353435376438313232353602037a1afd02047d16a3b402045826b397042033363236333939656531366162666333396561376439613238373432383135380201000201000400";
		m_fileid = "306b02010004643062020100042433343664633236302d613163302d313165362d386264392d66353435376438313232353602037a1afd02041316a3b4020458298a28042033626433653134643866343434613666313537356135383338313566383563340201000201000400";
		int ret = m_voiceengine.DownloadRecordedFile (m_fileid, m_downloadpath, 60000);
		s_strLog += "\r\n download file with ret=="+ret+" fileid="+m_fileid+" downpath"+m_downloadpath;
	}

	public void Click_btnSpeechToText()
	{
		int err;
		if (m_fileid == null) {
			PrintLog ("Speech to Text but fileid is null");
			return;
		}
		err = m_voiceengine.SpeechToText(m_fileid,0,6000);
		PrintLog ("SpeechToText with ret=" + err);
	}
	
	public void PrintLog(string str)
	{
		Debug.Log (str);
	}
	public void Click_GetRecFileParam()
	{
		int [] bytes = new int[1];
		bytes [0] = 0;
		float [] seconds = new float[1];
		seconds [0] = 0;
		m_voiceengine.GetFileParam (m_recordpath, bytes, seconds);
		s_strLog += "\r\nfile:"+m_recordpath+"bytes:" + bytes[0] + " seconds:" + seconds[0];
	}
}
