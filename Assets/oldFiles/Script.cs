using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.Networking;

/// <summary>
/// AssetBundle
/// </summary>
public class Script : MonoBehaviour {
    public GameObject group;
	// Use this for initialization
	void Start () {
	
	}
   // NetworkManager 

    public Transform[] trans_Pos3;
	public void OnValueChange()
    {
        //Debug.LogError("++");
    }
	// Update is called once per frame
	void Update () {
	
	}
    Vector3 v3_currentPos;
    public void OnPointerDown()
    {
        //Debug.LogWarning("开始"+group.transform.localPosition);
    }
    public void OnPointerUp()
    {
        v3_currentPos = group.transform.localPosition;
        //Debug.LogWarning("结束"+group.transform.localPosition);
        if (v3_currentPos.x >196)
        {
            group.transform.DOLocalMoveX(394, 0.3f);
        }
        else if (v3_currentPos.x<=-196)
        {
            group.transform.DOLocalMoveX(-394, 0.3f);
        }
        else 
        {
            group.transform.DOLocalMoveX(0, 0.3f);
        }
    }
}
