using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;

public class VoiceAnimation : MonoBehaviour {
    [Hotfix]
    [LuaCallCSharp]
    public UnityEngine.Sprite[] _voiceSprite= new UnityEngine.Sprite[3];
    public float speed = 0.3f;
    int num=0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void Play()
    {
        InvokeRepeating("ChangeSprite",0, speed);
    }
    public void Stop()
    {
        CancelInvoke("ChangeSprite");
    }
    void ChangeSprite()
    {
        if (num>=3)
        {
            num = 0;
        }
        this.GetComponent<Image>().sprite = _voiceSprite[num];
        num++;
    }
}
