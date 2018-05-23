#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class InAppPurchasePluginiOS
{
	[DllImport("__Internal")]
	private static extern void _UniIAPCharge(int type);

	[DllImport("__Internal")]
	private static extern void _copyTextToClipboard(string textlist);

    public static void UniIAPCharge(int type) 
    {
		if (Application.platform == RuntimePlatform.IPhonePlayer) 
        {
			_UniIAPCharge(type);
		}
	}

	public static void CopyTextToClipboard(string content)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) 
		{
			_copyTextToClipboard(content);
		}
	}
}
#endif