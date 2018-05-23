using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Util
{
    //==================================获取所有apk的包的大小以及MD5编码==========================================
    [MenuItem("生成资源/MD5")]
    public static void GetMD5()
    {
        //保存所有的apk文件和差异包的文件的版本号|文件的md5码|文件的大小
        int OldVersion = 10011; //旧版本id
        int NewVersion = 10015;  //新版本id
        int ChannelVersion = 1001;  //渠道编号

        string sFileName = "正式";  //存放安装包的文件名，局域网、外网测试、正式

        if (OldVersion >= NewVersion)
        {
            Debug.LogError("版本id一样，无法产生相应的md5码");
        }

        //保存所有apk文件的信息
        List<string> AllApkPath = new List<string>();

        //先获取apk的信息
        for (int i = OldVersion; i <= NewVersion; i++)
        {
            string path = "sxmj_" + ChannelVersion + "_" + i + ".apk";
            AllApkPath.Add(path);
        }

        //获取所有差异包的信息
        for (int i = OldVersion; i <= NewVersion; i++)
        {
            for (int j = i + 1; j <= NewVersion; j++)
            {
                string path = "sxmj_" + ChannelVersion + "_" + j + "_from_" + i + ".patch";
                AllApkPath.Add(path);
            }
        }

        string file = @"E:\ShuangXi_workspace\ApkWork\" + sFileName + "\\md5_files.txt";        //生成的版本信息文件

        using (StreamWriter sw = File.CreateText(file))
        {
            //分别读取所有文件
            for (int i = 0; i < AllApkPath.Count; i++)
            {
                Debug.LogError("路径：" + AllApkPath[i]);
                string path_apk = @"E:\ShuangXi_workspace\ApkWork\" + sFileName + "\\" + AllApkPath[i];
                sw.Write(AllApkPath[i] + "|");
                sw.Write(anhui.CheckUpdateManager.md5file(path_apk) + "|");
                if (i == AllApkPath.Count - 1)
                {
                    sw.Write(anhui.CheckUpdateManager.GetFileSize(path_apk));
                }
                else
                {
                    sw.WriteLine(anhui.CheckUpdateManager.GetFileSize(path_apk));
                }
            }
        }
    }




}
