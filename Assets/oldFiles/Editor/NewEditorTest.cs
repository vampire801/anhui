#define BB 
#define AA  
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using NUnit.Framework;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class NewEditorTest : MonoBehaviour
{
    [Test ]
    public void computeColumVaue()
    {

        uint d,e,f;
       // a = Convert.ToInt32("12288", 8);
       // b = Convert.ToInt32("12288", 4);
       // c = Convert.ToUInt32("12288", 8);
        d = Convert.ToUInt32("C0116A", 16);
       // e  = Convert.ToUInt32("12288", 32);
       // f  = Convert.ToUInt32("12288", 64);
       // uint[] sp = {   d,e,f  };
       // for (int i = 0; i < sp.Length ; i++)
       // {
            Debug.Log(d .ToString("X8"));
       // }
        //  int[] sp = { a, b, c, a };
        //for (int i = 0; i < sp.Length ; i++)
        //{
        //    Debug.Log(sp[i]);
        //}

        // Debug.LogWarning(ComputeColumnValue(sp,2,36));
    }
    int ComputeColumnValue(int[] _int2int, int totalRow, int column)
    {
        int num = 0;
        for (int i = totalRow-1; i >= 0; i--)
        {
            if ((_int2int[RowColumn2Index(i, totalRow, column)] & (0x8000 >> (column % 32))) > 0)
            {           
                num = (num << 1) + 1;
                Debug.Log("true" + num);
            }
            else
            {
                num = num << 1;
                Debug.Log("fale:" + num);
            }
           

        }
        return num;
    }
    int RowColumn2Index(int i, int Row, int comlum)
    {
        int index = 0;
        index = i + (comlum / 32) * Row;
        Debug.LogWarning("index"+index );
        return index;
       
    }
    [Test ]
    public void Rex()
    {
        Regex re = new Regex(@"http\b");
        Debug.LogWarning(re.IsMatch("http://dsfafasdsfdsf"));
        Debug.LogWarning(re.IsMatch("dsfhttps://dsfafasdsfdsf"));
    }

    [Test]
    public void ReadIO()
    {
        string path;
#if UNITY_EDITOR
        path = Application.dataPath + "/StreamingAssets/ShortTalk.txt";
#elif UNITY_IPHONE || UNITY_ANDROID
        path = Application.streamingAssetsPath + "/ShortTalk.txt";
#endif
        string[] str = File.ReadAllText(path, Encoding.UTF8).Split('\n');
        for (int i = 0; i < str.Length; i++)
        {
            Debug.LogError(str[i].GetHashCode());
        }
    }
   
}



#region 创建assetbundles资源
public class CreatAssetBundles
{
    [MenuItem("AssetBundle/buildAndroid")]
    static void BuildAssetAndroid()
    {
        string dir = "AssetBundles";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        ///
        ///安卓手机文件打包
        ///
        BuildPipeline.BuildAssetBundles("AssetBundles", BuildAssetBundleOptions.None, BuildTarget.Android);

        // BuildPipeline.BuildAssetBundles("AssetBundles");
    }
    [MenuItem("AssetBundle/buildIOS")]
    static void BuildAssetIOS()
    {
        string dir = "AssetBundles";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        ///
        ///IOS手机文件打包
        ///
        BuildPipeline.BuildAssetBundles("AssetBundles", BuildAssetBundleOptions.None, BuildTarget.iOS);

        // BuildPipeline.BuildAssetBundles("AssetBundles");

    }
    [MenuItem("AssetBundle/buildWin")]
    static void BuildAssetWin()
    {
        string dir = "AssetBundles";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        ///
        ///windows文件打包
        ///
        BuildPipeline.BuildAssetBundles("AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

    }

    [MenuItem("AssetBundle/Get AssetBundle names")]
    static void GetNames()
    {
        var names = AssetDatabase.GetAllAssetBundleNames();
        foreach (var name in names)
            Debug.Log("AssetBundle: " + name);

    }

    #endregion 创建assetbundles资源

    [MenuItem("Tools/JsonForShare")]
    static void BuildJson()
    {
        WeChatShareConfig wc = new WeChatShareConfig(9);

        #region 赋值
        wc. p[0].iType = 0;
        wc.p[0].iDes = 0;
        wc.p[0].spTitle = "我在【双喜麻将】领取了5元红包邀请好友一起来玩就能领取！";
        wc.p[1].iType = 1;
        wc.p[1].iDes = 1;
        wc.p[1].shTitle = "充值也能获得红包！刚在游戏里充值购买金币然后叮的一声提示：恭喜您获得一个充值红包";
        wc.p[2].iType = 2;
        wc.p[2].iDes = 2;
        wc.p[2].spTitle = "我在【双喜麻将】领取了现金红包在游戏里取钱还能获得红包！";
        wc.p[2].shTitle = "在游戏里取钱还能得红包！好开森，可以把游戏里红包的钱取出来了然后叮的一声提示：恭喜您获得了一个提现红包";
        wc.p[3].iType = 3;
        wc.p[3].iDes = 2;
        wc.p[3].spTitle = "我在【双喜麻将】领取了现金红包在游戏里第一次取钱还能获得红包！";
        wc.p[3].shTitle = "在游戏里取钱还能得红包！好开森，第一次在游戏里把红包钱取出来然后叮的一声提示：恭喜您获得了一个首次提现红包";
        wc.p[4].iType = 4;
        wc.p[4].iDes = 2;
        wc.p[4].spTitle = "我在【双喜麻将】领取了现金红包参加活动点击分享就能领取！";
        wc.p[3].shTitle = "点击分享也能获得红包！刚在游戏里的活动点击了分享然后叮的一声提示：恭喜您获得一个分享红包";
        wc.p[5].iType = 5;
        wc.p[5].iDes = 1;
        wc.p[5].shTitle = "打牌也能获得红包！刚在游戏里获得了大赢家称号然后叮的一声提示：恭喜您获得一个大赢家红包";
        wc.p[6].iType = 6;
        wc.p[6].iDes = 1;
        wc.p[6].shTitle = "打牌也能获得红包！刚在游戏里获得了最佳炮手称号然后叮的一声提示：恭喜您获得一个最佳炮手红包";
        wc.p[7].iType = 7;
        wc.p[7].iDes = 2;
        wc.p[7].spTitle = "我在【双喜麻将】领取了现金红包进入游戏点击分享就能领取！";
        wc.p[7].shTitle = "点击分享也能获得红包！第一次进入游戏，点击了游戏里的分享然后叮的一声提示：恭喜您获得一个分享红包";
        wc.p[8].iType = 8;
        wc.p[8].iDes = 2;
        wc.p[8].spTitle = "充值也能获得红包！刚在游戏里充值购买金币然后叮的一声提示：恭喜您获得一个充值红包";
        wc.p[8].shTitle = "点击分享也能获得红包！刚在游戏里麻将馆抢到了红包分享炫耀了一下然后叮的一声提示：恭喜您获得一个分享红包";
        #endregion

        string url = Application.streamingAssetsPath + "/" + wc.ToString().Split('+')[1] + ".json";
        if (!File.Exists(url))
        {
            string json = JsonUtility.ToJson(wc);
            Debug.LogError(json);
            using (StreamWriter sw = File.CreateText(url))
            {
                sw.WriteLine(json);
                sw.Flush();
                sw.Close();
            }
        }
        // wc=MahjongCommonMethod . ReadLocalJson(wc);

       // string str = "";
     //  str = File.ReadAllText(url);
        
       // wc = JsonUtility.FromJson<CommonConfig.WeChatShareConfig>(str);
        // Debug.LogWarning (wc.p0.sp );
        //读取

        //js = JsonUtility.FromJson<MyJson>(str);
        //Debug.LogError("aa" + ":" + js.aa + "js" + ":" + js.sz + "fl" + ":" + js.fl);
    }

    [MenuItem("Tools/JsonForFAQ")]
    static void BuildJson1()
    {
        Json_FAQ js = new Json_FAQ(4);
        js.data[0].title = "1.	如何创建房间邀请好友？";
        js.data[0].content  = "1).点击“创建房间”按钮。2).选择房间玩法设置，确认后即可进入房间。3).点击“邀请好友按钮可以邀请微信好友。”4).选择您的牌友群，点击“分享”，发送邀请。5).等待牌友进入房间，桌上人满就可以开始啦。";
        js.data[1].title = "2.	如何加入房间？";
        js.data[1].content = "1)	进入游戏后，点击大厅的“加入房间”按钮2)	输入您朋友分享的六位数房间号，就可以进入房间了3)	点击微信好友的分享链接，直接进入房间";
        js.data[2].title = "3.	如何加入麻将馆？";
        js.data[2].content = "1)	点击大厅的雀神广场2)	选择一个麻将馆3)	点击“申请”麻将馆4)	老板通过审核后即可加入麻将馆";
        js.data[3].title = "4.	如何使用现金、话费、流量?";
        js.data[3].content = "1)	打开微信，点击“通讯录”，点击“公众号”2)	输入“双喜麻将”搜索3)	点击“关注”4)	点击“兑换”5)	选择现金、话费或流量可进行兑换";
        string url = Application.streamingAssetsPath + "/" + js.ToString().Split('+')[1] + ".json";
        if (!File.Exists(url))
        {
            string json = JsonUtility.ToJson(js);
            Debug.LogError(json);
            using (StreamWriter sw = File.CreateText(url))
            {
                sw.WriteLine(json);
                sw.Flush();
                sw.Close();
            }
        }
    }
    #region 生成FAQ
    [Serializable ]
    public class Json_FAQ{
        public int num;
        public  FaqContent[]data;
        public Json_FAQ() { }
        public Json_FAQ(int a)
        {
            num = a;
            data = new FaqContent[a];
        }
    }
    [Serializable ]
    public struct  FaqContent
    {
        public string title;
        public string content;
    }
    #endregion FAQ

    #region 生成分享配置
    [Serializable]
    public class WeChatShareConfig
    {
        public int iTotalNum;
        // public MyShareStruct a, b, c, d, e, f, g, h, i;
        public MyShareStruct[] p;
        public WeChatShareConfig() { }
        public WeChatShareConfig(int a)
        {
            iTotalNum = a;
            p = new MyShareStruct[a];
        }

    }
    [Serializable]
    public struct MyShareStruct
    {
        /// <summary>
        /// 0-推广红包 1-充值红包分享 2-提现红包分享 3-首次提现红包分享 4-活动分享 5-大赢家分享 6-最佳炮手分享 7- 大厅分享 8- 麻将馆红包分享
        /// </summary>
        public int iType;
        /// <summary>
        /// 0-朋友圈 1-好友 2-both
        /// </summary>
        public int iDes;
        /// <summary>
        /// 朋友圈
        /// </summary>
        public string spTitle, spContent;
        /// <summary>
        /// 好友
        /// </summary>
        public string shTitle, shContent;
    }
    #endregion 生成分享配置
}