using UnityEngine;
using System.Collections;

public class FontMessage{
     static FontMessage fontMessage;
   public static FontMessage FontMessagey
    {
        get
        {
            if (fontMessage==null)
            {
                fontMessage = new FontMessage();
            }
            return fontMessage;
        }
    }
    Font SystemFont;
    /// <summary>
    /// 创建字体方法
    /// </summary>
    /// <returns></returns>
    //public Font GetSystemFont()
    //{
    //    if (SystemFont == null)
    //    {
    //        //创建字体
    //        if (Application.platform == RuntimePlatform.Android)
    //        {
    //            //SystemFont = Font.CreateDynamicFontFromOSFont("DroidSansFallback", 32);
    //            SystemFont = Font.CreateDynamicFontFromOSFont("FZCuYuan - M03S", 32);//新字体
    //        }
    //        else if (Application.platform == RuntimePlatform.IPhonePlayer)
    //        {
    //            //SystemFont = Font.CreateDynamicFontFromOSFont("Heiti SC", 32);
    //            SystemFont = Font.CreateDynamicFontFromOSFont("FZCuYuan - M03S", 32);//新字体
    //        }
    //        else
    //        {
    //            //SystemFont = Font.CreateDynamicFontFromOSFont("Microsoft YaHei", 32);
    //            SystemFont = Font.CreateDynamicFontFromOSFont("FZCuYuan - M03S", 32);//新字体
    //        }
    //        SystemFont = Font.CreateDynamicFontFromOSFont("Microsoft YaHei", 32);
    //    }
    //    return SystemFont;
    //}
}
