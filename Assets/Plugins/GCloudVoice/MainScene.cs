using UnityEngine;
using System.Collections;
using System;

using gcloud_voice;
public class MainScene : MonoBehaviour {
	private IGCloudVoice m_voiceengine = null;

	// Use this for initialization
	void Start () {
		if (m_voiceengine == null) {
			m_voiceengine = GCloudVoice.GetEngine();
			System.TimeSpan ts = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0,0);
			string strTime =  System.Convert.ToInt64(ts.TotalSeconds).ToString(); 
			//m_voiceengine.SetAppInfo("932849489","d94749efe9fce61333121de84123ef9b",strTime);
			m_voiceengine.SetAppInfo("gcloud.test","test_key",strTime);
			m_voiceengine.Init();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RealTimeBtn_Click()
	{
		//Debug.Log ("realtime button click");
		m_voiceengine.SetMode (GCloudVoiceMode.RealTime);
		Application.LoadLevel ("RealTimeVoice");
	}
	public void OffLineBtn_Click()
	{
		//Debug.Log ("offline btn click");
		m_voiceengine.SetMode (GCloudVoiceMode.Messages);
		Application.LoadLevel ("OffLineVoice");
	}
	public void SpeechToTextBtn_Click ()
	{
		m_voiceengine.SetMode (GCloudVoiceMode.Translation);
		Application.LoadLevel ("SpeechToText");
	}
	public void OnApplicationPause(bool pauseStatus)
	{
		Debug.Log("Voice OnApplicationPause: " + pauseStatus);
		if (pauseStatus)
		{
			if (m_voiceengine == null)
			{
				return;
			}
			//m_voiceengine.Pause();
			//s_strLog += "\r\n pause:"+ret;
		}
		else
		{
			if (m_voiceengine == null)
			{
				return;
			}
			//m_voiceengine.Resume();
			//s_strLog += "\r\n resume:"+ret;
		}
	}
	
}
