using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class AssistTool : MonoBehaviour {

    // Use this for initialization
    private TimerManager _timerManager=new TimerManager ();
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        _timerManager.Tick(Time.deltaTime);
    }

#if UNITY_EDITOR
    static Vector3[] fourCorners = new Vector3[4];
    static Canvas[] canvas;
    void OnDrawGizmos()
    {

        foreach (MaskableGraphic g in GameObject.FindObjectsOfType<MaskableGraphic>())
        {
            if (g.raycastTarget)
            {
                RectTransform rectTransform = g.transform as RectTransform;
                rectTransform.GetWorldCorners(fourCorners);
                Gizmos.color = Color.blue;
                for (int i = 0; i < 4; i++)
                    Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);

            }
        }
        if (gameObject.transform.GetComponentsInChildren <Canvas >().Length >=1)
        {
            foreach (Canvas  item in gameObject.transform.GetComponentsInChildren<Canvas>())
            {
                item.worldCamera   = Camera.main;
                
            }
        }
    }
#endif
}
