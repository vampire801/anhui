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

public class RealTimeVoice : MonoBehaviour {

	private IGCloudVoice m_voiceengine = GCloudVoice.GetEngine();  //engine have init int mainscene start function
	private string m_roomName = "cz-test";


	private Text m_logtext;
	private static string s_logstr;
	private static bool bIsStart = false;

	public void PrintLog(string str)
	{
		Debug.Log (str);
	}

	// Use this for initialization
	void Start () {
		//m_voiceengine = GcloudVoice.Engine();
		PrintLog ("realtime voice start...");
		m_logtext = (Text)GameObject.Find ("Canvas/Panel/Text_Content").GetComponent<Text> ();
		if (!bIsStart) {
			bIsStart = true;
			//if (m_voiceengine.OnJoinRoomComplete == null) {
			m_voiceengine.OnJoinRoomComplete += (IGCloudVoice.GCloudVoiceCompleteCode code, string roomName, int memberID) => {
				PrintLog ("On Join Room With " + code);
				Debug.Log ("OnJoinRoomComplete ret=" + code + " roomName:" + roomName + " memberID:" + memberID);
				s_logstr += "\r\n"+"OnJoinRoomComplete ret="+code+" roomName:"+roomName+" memberID:"+memberID;
				//UIManager.m_Instance.OnJoinRoomDone(code);
			};
			//}
			m_voiceengine.OnQuitRoomComplete += (IGCloudVoice.GCloudVoiceCompleteCode code, string roomName, int memberID) => {
				PrintLog ("On Quit Room With " + code);
				Debug.Log ("OnQuitRoomComplete ret=" + code + " roomName:" + roomName + " memberID:" + memberID);
				s_logstr += "\r\n"+"OnJoinRoomComplete ret="+code+" roomName:"+roomName+" memberID:"+memberID;
				//UIManager.m_Instance.OnJoinRoomDone(code);
			};
		
			m_voiceengine.OnMemberVoice += (int[] members, int count) =>
			{
				//PrintLog ("OnMemberVoice");
				//s_logstr +="\r\ncount:"+count;
				for(int i=0; i < count && (i+1) < members.Length; ++i)
				{
					//s_logstr +="\r\nmemberid:"+members[i]+"  state:"+members[i+1];
					++i;
				}
				//UIManager.m_Instance.UpdateMemberState(members, length, usingCount);
			};
		}

		//recordingPath ="file://"+ Application.persistentDataPath + "/" + "recording.dat";
		//downloadPath = "file://"+ Application.persistentDataPath + "/" + "download.dat";
	}
	
	// Update is called once per frame
	void Update () {
		if (m_voiceengine != null) {
			m_voiceengine.Poll();
			if(s_MicLevelEnabel)
			{
				s_logstr+="\r\n miclevel:"+m_voiceengine.GetMicLevel();
			}
		}

		m_logtext.text = s_logstr;
	}

	static bool s_MicLevelEnabel = false;
	public void GetMicLevel_Click()
	{
		s_MicLevelEnabel = !s_MicLevelEnabel;
		Debug.Log ("GetMicLevel_Click miclevleenable="+s_MicLevelEnabel);
	}

	public void BackBtn_Click()
	{
		Application.LoadLevel ("main");
	}
	public void ClearLogBtn_Click()
	{
		s_logstr = "";
	}
	public void OnClick(GameObject sender)
	{
		Debug.Log("fk");
	}
	 
	public void JoinRoomBtn_Click()
	{
		Debug.Log ("JoinRoom Btn Click");

		int ret = m_voiceengine.JoinTeamRoom(m_roomName, 15000);
		PrintLog ("joinroom ret=" + ret);
	}
	public void AudienceJoin_Click()
	{
		Debug.Log ("AudienceJoin Btn Click");
		int ret = m_voiceengine.JoinNationalRoom(m_roomName, GCloudVoiceRole.AUDIENCE, 15000);
		PrintLog ("AudienceJoin ret=" + ret);
	}
	public void AnchorJoin_Click()
	{
		Debug.Log ("AnchorJoin Btn Click");
		int ret = m_voiceengine.JoinNationalRoom(m_roomName, GCloudVoiceRole.ANCHOR, 15000);
		PrintLog ("AnchorJoin ret=" + ret);
	}
	public void OpenMicBtn_Click()
	{
		Debug.Log ("Open Mic btn clieck");
		int ret = m_voiceengine.OpenMic ();
		PrintLog ("openmic ret=" + ret);
	}
	public void OpenSpeakerBtn_Click()
	{
		Debug.Log ("OpenSpeaker btn click");
		int ret = m_voiceengine.OpenSpeaker ();
		PrintLog ("OpenSpeaker ret=" + ret);
	}
	public void CloseMicBtn_Click()
	{
		Debug.Log ("CoseMic btn click");
		m_voiceengine.CloseMic ();
	}
	public void CloseSpeakerBtn_Click()
	{
		Debug.Log ("Close speaker btn click");
		m_voiceengine.CloseSpeaker ();
	}
	public void QuitRoomBtn_Click()
	{
		Debug.Log ("quit room btn click");
		m_voiceengine.QuitRoom(m_roomName, 15000);
	}

	static bool s_bEnableLog = true;
	public void EnableLog_Click()
	{
		Debug.Log ("EnableLog_Click");
		s_bEnableLog = !s_bEnableLog;
		s_logstr += "\r\nlogenable:" + s_bEnableLog;
		m_voiceengine.EnableLog (s_bEnableLog);
	}
	public void TestMic_Click()
	{
		Debug.Log ("TestMic_Click");
		int ret = m_voiceengine.TestMic ();
		s_logstr += "\r\ntestmic ret:" + ret;
	}
	public void SetSpeakVol_Click()
	{
		Debug.Log ("SetSpeakVol_Click");
		int ret = m_voiceengine.SetSpeakerVolume(0x0000);
		s_logstr += "\r\nSetSpeakerVolume to 0:";
	}
	public void Invoke_Click()
	{
		Debug.Log ("SetSpeakVol_Click");
		//int ret = m_voiceengine.invoke(5003, 0xff00, 0, null);
		//int ret = m_voiceengine.invoke(5004, 32000, 0, null);
		int ret = m_voiceengine.invoke (6100, 10000, 0, null);
		s_logstr += "\r\nInvoke_Click setmaxmsgtime to 10s: ret="+ret;
	}
	static bool s_forbidmemvoice = false;
	public void ForbidMembVoice_Click()
	{
		s_forbidmemvoice = !s_forbidmemvoice;
		Debug.Log ("ForbidMembVoice_Click, forbidmemevoice="+s_forbidmemvoice);
		for (int i = 0; i < 50; ++i) {
			m_voiceengine.ForbidMemberVoice(i, s_forbidmemvoice);
		}
		s_logstr += "\r\n\torbidMembVoice_Click memberid 0-50";
	}
	public void GetSpeakVol_Click()
	{
		Debug.Log ("GetSpeakVol_Click");
		int ret = m_voiceengine.GetSpeakerLevel();
		s_logstr += "\r\nGetSpeakerVolume:"+ret;
	}
}
