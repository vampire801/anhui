using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode ]
[RequireComponent (typeof (Camera ))]
public class PostEffectsBase : MonoBehaviour {

	// Use this for initialization
	void Start () {
        CheckResources();
	}

    private void CheckResources()
    {
        bool isSupported = CheckSupport();
        if (!isSupported )
        {
            NotSupported();
        }
    }

    private void NotSupported()
    {
        enabled=false ;
    }

    private bool CheckSupport()
    {
        if (SystemInfo.supportsImageEffects ==false )
        {
            Debug.LogError("This platform does not support image effects");
            return false;
        };
        return true;
    }

    // Update is called once per frame
    void Update () {
		
	}

}
