using UnityEngine;
using Common;
using CmdEditor;
using System.IO;
using MahjongLobby_AH.Network.Message;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class DEBUG
{
    public static bool show_TracerLog = true;
    public static bool show_SendMsgLog = true;
    public static bool show_RecvMsgLog = true;

    public const string TRACER_LOG = "[ Tracer ] ";
    public const string SEND_MSG_LOG = "[ Send ] ";
    public const string RECV_MSG_LOG = "[ Recv ] ";

    //日志类别
    public enum LogCategory
    {
        NONE,
        GUI,
        GUI_MESSAGE,
        PHYSICS,
        GRAPHICS,
        TERRAIN,
        AUDIO,
        NETWORKING,
        NETWORK_SERVER,
        NETWORK_CLIENT,
        GAME_LOGIC,
        AI,
        CONTENT,
        SYSTEM,
        INPUT,
        REPLAY,
        MAX_LOG_CATEGORY
    }

    static LogType logType = LogType.Log;
    static LogCategory logCategory = LogCategory.NONE;
    FileStream fileStream;
    StreamWriter streamWriter;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        guiEnable = false;
        guiMessageEnable = false;
        physicsEnable = false;
        graphicsEnable = false;
        terrainEnable = false;
        audioEnable = false;
        networkingEnable = true ;
        networkServerEnable = true;
        networkClientEnable = true;
        gameLogicEnable = false;
        aiEnable = false;
        contentEnable = false;
        systemEnable = false;
        inputEnable = false;
        replayEnable = false;
    }


    #region 类型日志
#if UNITY_EDITOR
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Assert(bool comparison, string message)
    {
        if (!comparison)
        {
            EditorUtility.DisplayDialog("Assert", "[ " + message + " ]", "OK");

            Debug.LogWarning("Assert: " + message);
            Debug.Break();
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Assert(bool comparison)
    {
        if (!comparison)
        {
            EditorUtility.DisplayDialog("Assert", "[ Assert ! ]", "OK");

            Debug.LogWarning("Assert !");
            Debug.Break();
        }
    }
#else
	public static void Assert(bool comparison, string message)
	{
		if(!comparison)
		{
			Debug.LogWarning("Assert: " + message);
		}
	}
	
	public static void Assert(bool comparison)
	{
		if(!comparison)
		{
			Debug.LogWarning("Assert !");
		}
	}
#endif

    //Error
    public static void Error(string s)
    {
        Log(s, LogCategory.NONE, LogType.Error);
    }

    //Assert
    public static void Assert(string s)
    {
        Log(s, LogCategory.NONE, LogType.Assert);
    }

    //Warning
    public static void Warning(string s)
    {
        Log(s, LogCategory.NONE, LogType.Warning);
    }



    //Exception
    public static void Exception(string s)
    {
        Log(s, LogCategory.NONE, LogType.Exception);
    }

    #endregion 类型日志

    #region 类别日志

    //GUI
    static bool guiEnable;
    public static void Gui(string s)
    {
        if (guiEnable)
        {
            Log(s, LogCategory.GUI, LogType.Log);
        }
    }
    public static void Gui(string s, LogType type)
    {
        if (guiEnable)
        {
            Log(s, LogCategory.GUI, type);
        }
    }

    //GUIMessage
    static bool guiMessageEnable;
    public static void GuiMessage(string s)
    {
        if (guiMessageEnable)
        {
            Log(s, LogCategory.GUI_MESSAGE, LogType.Log);
        }
    }

    public static void GuiMessage(string s, LogType type)
    {
        if (guiMessageEnable)
        {
            Log(s,LogCategory.GUI_MESSAGE, type);
        }
    }

    //Physics
    static bool physicsEnable;
    public static void Physics(string s)
    {
        if (physicsEnable)
        {
            Log(s, LogCategory.PHYSICS, LogType.Log);
        }
    }

    public static void Physics(string s, LogType type)
    {
        if (physicsEnable)
        {
            Log(s, LogCategory.PHYSICS, type);
        }
    }

    //Graphics
    static bool graphicsEnable;
    public static void Graphics(string s)
    {
        if (graphicsEnable)
        {
            Log(s, LogCategory.GRAPHICS, LogType.Log);
        }
    }

    public static void Graphics(string s, LogType type)
    {
        if (graphicsEnable)
        {
            Log(s, LogCategory.GRAPHICS, type);
        }
    }

    //Terrain
    static bool terrainEnable;
    public static void Terrain(string s)
    {
        if (terrainEnable)
        {
            Log(s, LogCategory.TERRAIN, LogType.Log);
        }
    }

    public static void Terrain(string s, LogType type)
    {
        if (terrainEnable)
        {
            Log(s, LogCategory.TERRAIN, type);
        }
    }

    //Audio
    static bool audioEnable;
    public static void Audio(string s)
    {
        if (audioEnable)
        {
            Log(s, LogCategory.AUDIO, LogType.Log);
        }
    }

    public static void Audio(string s, LogType type)
    {
        if (audioEnable)
        {
            Log(s, LogCategory.AUDIO, type);
        }
    }

    public static void SoundLog(string s)
    {
        //		return;
        Debug.Log(s);
    }

    /// <summary>
    /// Tracer
    /// </summary>
    public static void LogTracer(string s)
    {
        Debug.LogWarning("[Tracer] " + s);
    }

    //Networking
    static bool networkingEnable;
    public static void Networking(string s)
    {
        return;        
        if (false
            || (show_TracerLog && s.Contains(TRACER_LOG))
            || (show_SendMsgLog && s.Contains(SEND_MSG_LOG))
            || (show_RecvMsgLog && s.Contains(RECV_MSG_LOG))
        )
        {
            if (networkingEnable)
            {
                Log(s, LogCategory.NETWORKING, LogType.Log);
            }
        }
    }

    public static void Networking(string s, LogType type)
    {
        return;

        Debug.Log("[Networking]" + s);

        if (networkingEnable)
        {
            Log(s, LogCategory.NETWORKING, type);
        }
    }

    //NetworkServer
    static bool networkServerEnable;
    public static void NetworkServer(string s)
    {
       // Debug.Log("[NetworkServer]" + s);

        if (networkServerEnable)
        {
            Log(s, LogCategory.NETWORK_SERVER, LogType.Log);
        }
    }

    public static void NetworkServer(string s, LogType type)
    {
        Debug.Log("[NetworkServer]" + s);

        if (networkServerEnable)
        {
            Log(s, LogCategory.NETWORK_SERVER, type);
        }
    }

    //NetworkClient
    static bool networkClientEnable;
    public static void NetworkClient(string s)
    {
       // Debug.Log("[NetworkClient]" + s);

        if (networkClientEnable)
        {
            Log(s, LogCategory.NETWORK_CLIENT, LogType.Log);
        }
    }

    public static void NetworkClient(string s, LogType type)
    {
        Debug.Log("[NetworkClient]" + s);

        if (networkClientEnable)
        {
            Log(s, LogCategory.NETWORK_CLIENT, type);
        }
    }

    public static void NetLog_Recv(string s)
    {
        Debug.Log(s);
    }

    public static void NetLog_Send(string s)
    {
        Debug.Log(s);
    }

    public static void GameInfoLog(string s)
    {
        Debug.Log(s);
    }

    public static void GemBonusLog(string s)
    {
        Debug.Log(s);
    }

    //GameLogic
    static bool gameLogicEnable;
    public static void GameLogic(string s)
    {
        if (gameLogicEnable)
        {
            Log(s, LogCategory.GAME_LOGIC, LogType.Log);
        }
    }

    public static void GameLogic(string s, LogType type)
    {
        if (gameLogicEnable)
        {
            Log(s, LogCategory.GAME_LOGIC, type);
        }
    }

    //AI
    static bool aiEnable;
    public static void Ai(string s)
    {
        if (aiEnable)
        {
            Log(s, LogCategory.AI, LogType.Log);
        }
    }

    public static void Ai(string s, LogType type)
    {
        if (aiEnable)
        {
            Log(s,LogCategory.AI, type);
        }
    }

    //Content
    static bool contentEnable;
    public static void Content(string s)
    {
        //		Console.LOG(s);

        if (contentEnable)
        {
            Log(s, LogCategory.CONTENT, LogType.Log);
        }
    }

    public static void Content(string s, LogType type)
    {
        //		Console.LOG(s);

        if (contentEnable)
        {
            Log(s, LogCategory.CONTENT, type);
        }
    }

    //System
    static bool systemEnable;
    public static void System(string s)
    {
        //		Debug.Log("[System] " + s);
        if (systemEnable)
        {
            Log(s, LogCategory.SYSTEM, LogType.Log);
        }
    }

    public static void System(string s, LogType type)
    {
        if (systemEnable)
        {
            Log(s, LogCategory.SYSTEM, type);
        }
    }

    //Input
    static bool inputEnable;
    public static void Input(string s)
    {
        if (inputEnable) 
        {
            Log(s, LogCategory.INPUT, LogType.Log);
        }
    }

    public static void Input(string s, LogType type)
    {
        if (inputEnable)
        {
            Log(s, LogCategory.INPUT, type);
        }
    }

    //Replay
    static bool replayEnable;
    public static void Replay(string s)
    {
        if (replayEnable)
        {
            Log(s, LogCategory.REPLAY, LogType.Log);
        }
    }

    public static void Replay(string s, LogType type)
    {
        if (replayEnable)
        {
            Log(s, LogCategory.REPLAY, type);
        }
    }

    public static void Log_Lobby(string s)
    {
        return;

        Debug.Log(s);

        Log(s, LogCategory.SYSTEM, LogType.Log);

        if (Console.GetInstance() != null)
            Console.GetInstance().Log(CurTime() + s);
    }

    public static string CurTime()
    {
#if WIN32		
		long sec = System.DateTime.Now.Ticks / 10000000;
		long ms  = System.DateTime.Now.Ticks % 10000000;
		string s = ("[ " + sec + "." + ms + " ] \t");
		
//		string t = ("[ " + System.DateTime.Now + " ] \t");
		
		return s;
#else
        //		return (Console.STime);
        return "";
#endif
    }

    #endregion 类别日志

//    /// <summary>
//    /// 新建日志文件
//    /// </summary>
//    /// <param name="filename"></param>
//    public static void NewLog(string filename)
//    {
//#if ENABLE_LOG
//            fileStream = new FileStream(filename, FileMode.Create);
//            streamWriter = new StreamWriter(fileStream);

//            WriteHeader();
//#endif
//        Application.RegisterLogCallback(HandleLog); //注册委托调用日志信息

//    }

//    /// <summary>
//    /// 写入文件头
//    /// </summary>
//    protected void WriteHeader()
//    {
//        string s = @"<html>
//<head>
//<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
//<script language=""javascript"" src=""./log.js"">
//</script>
//<link rel=""stylesheet"" type=""text/css"" href=""./log.css"">
//</head>
//<body>
//<div class=""Header"">
//<input type=""button"" value=""Error"" class=""Error Button"" /><input type=""checkbox"" id=""Error"" onclick=""hide_class(&#39;Error&#39;)"" checked=""checked"" />
//<input type=""button"" value=""Assert"" class=""Assert Button"" /><input type=""checkbox"" id=""Assert"" onclick=""hide_class(&#39;Assert&#39;)"" checked=""checked"" />
//<input type=""button"" value=""Warning"" class=""Warning Button"" /><input type=""checkbox"" id=""Warning"" onclick=""hide_class(&#39;Warning&#39;)"" checked=""checked"" />
//<input type=""button"" value=""Log"" class=""Log Button"" /><input type=""checkbox"" id=""Log"" onclick=""hide_class(&#39;Log&#39;)"" checked=""checked"" />
//<input type=""button"" value=""Exception"" class=""Exception Button"" /><input type=""checkbox"" id=""Exception"" onclick=""hide_class(&#39;Exception&#39;)"" checked=""checked"" />
//<input type=""button"" value=""GUI"" class=""GUI Button"" /><input type=""checkbox"" id=""GUI"" onclick=""hide_class(&#39;GUI&#39;)"" checked=""checked"" />
//<input type=""button"" value=""GUIMessage"" class=""GUIMessage Button"" /><input type=""checkbox"" id=""GUIMessage"" onclick=""hide_class(&#39;GUIMessage&#39;)"" checked=""checked"" />
//<input type=""button"" value=""Physics"" class=""Physics Button"" /><input type=""checkbox"" id=""Physics"" onclick=""hide_class(&#39;Physics&#39;)"" checked=""checked"" />
//<input type=""button"" value=""Graphics"" class=""Graphics Button"" /><input type=""checkbox"" id=""Graphics"" onclick=""hide_class(&#39;Graphics&#39;)"" checked=""checked"" />
//<input type=""button"" value=""Terrain"" class=""Terrain Button"" /><input type=""checkbox"" id=""Terrain"" onclick=""hide_class(&#39;Terrain&#39;)"" checked=""checked"" />
//<input type=""button"" value=""Audio"" class=""Audio Button"" /><input type=""checkbox"" id=""Audio"" onclick=""hide_class(&#39;Audio&#39;)"" checked=""checked"" />
//<input type=""button"" value=""Networking"" class=""Networking Button"" /><input type=""checkbox"" id=""Networking"" onclick=""hide_class(&#39;Networking&#39;)"" checked=""checked"" />
//<input type=""button"" value=""NetworkServer"" class=""NetworkServer Button"" /><input type=""checkbox"" id=""NetworkServer"" onclick=""hide_class(&#39;NetworkServer&#39;)"" checked=""checked"" />
//<input type=""button"" value=""NetworkClient"" class=""NetworkClient Button"" /><input type=""checkbox"" id=""NetworkClient"" onclick=""hide_class(&#39;NetworkClient&#39;)"" checked=""checked"" />
//<input type=""button"" value=""GameLogic"" class=""GameLogic Button"" /><input type=""checkbox"" id=""GameLogic"" onclick=""hide_class(&#39;GameLogic&#39;)"" checked=""checked"" />
//<input type=""button"" value=""AI"" class=""AI Button"" /><input type=""checkbox"" id=""AI"" onclick=""hide_class(&#39;AI&#39;)"" checked=""checked"" />
//<input type=""button"" value=""Content"" class=""Content Button"" /><input type=""checkbox"" id=""Content"" onclick=""hide_class(&#39;Content&#39;)"" checked=""checked"" />
//<input type=""button"" value=""System"" class=""System Button"" /><input type=""checkbox"" id=""System"" onclick=""hide_class(&#39;System&#39;)"" checked=""checked"" />
//<input type=""button"" value=""Input"" class=""Input Button"" /><input type=""checkbox"" id=""Input"" onclick=""hide_class(&#39;Input&#39;)"" checked=""checked"" />
//<input type=""button"" value=""Replay"" class=""Replay Button"" /><input type=""checkbox"" id=""Replay"" onclick=""hide_class(&#39;Replay&#39;)"" checked=""checked"" />
//</div>
//<div class=""LogBody"">
//";

//        streamWriter.Write(s);
//        streamWriter.Flush();
//    }

    /// <summary>
    /// 记录日志
    /// </summary>
    /// <param name="message">日志内容</param>
    public static void Log(string message)
    {
        Log(message, LogCategory.NONE, LogType.Log);
    }

    /// <summary>
    /// 记录日志
    /// </summary>
    /// <param name="message">日志内容</param>
    /// <param name="category">日志类别</param>
    public static void Log(string message, LogCategory category)
    {
        Log(message, category, LogType.Log);
    }

    /// <summary>
    /// 记录日志
    /// </summary>
    /// <param name="message">日志内容</param>
    /// <param name="category">日志类别</param>
    /// <param name="type">日志类型</param>
    public static void Log(string message, LogCategory category, LogType type)
    {
        if (category == LogCategory.MAX_LOG_CATEGORY)
        {
            //return;
        }

        //logCategory = category;

        if (type == LogType.Warning)
        {
            Debug.LogWarning(message);
        }
        else if (type == LogType.Error)
        {
            Debug.LogError(message);
        }
        else
        {
            //Debug.Log(message);
        }
    }

    #region 网络消息日志

    /// <summary>
    /// 记录客户端发送的消息
    /// </summary>
    /// <param name='id'>
    /// 消息类型
    /// </param>
    public static void NetworkClient(byte id)
    {
        //if (MsgID.SkipLog(id))
        //{
        //    return;
        //}

        //NetworkClient(MsgID.ToString(id));
    }

    /// <summary>
    /// 记录接收服务器的消息
    /// </summary>
    /// <param name='id'>
    /// 消息类型
    /// </param>
    public static void NetworkServer(byte id)
    {
        //if (MsgID.SkipLog(id))
        //{
        //    return;
        //}

        //NetworkServer(MsgID.ToString(id));
    }

    /// <summary>
    /// 记录客户端发送的消息数据
    /// </summary>
    /// <param name='id'>
    /// 消息类型
    /// </param>
    /// <param name='bytes'>
    /// 消息的字节数组
    /// </param>
    public static void NetworkClient(ushort id, byte[] bytes)
    {
        if (MsgID.SkipLog(id))
        {
            //return;
        }

        string s = MsgID.ToString(id);
        s += ("  长度: " + bytes.Length);
        s += "  数据:";
        for (int i = 0; i < bytes.Length; ++i)
        {
            s += (" " + bytes[i].ToString("X2"));
        }

        NetworkClient(s);
    }

    /// <summary>
    /// 记录接收服务器的消息数据
    /// </summary>
    /// <param name='id'>
    /// 消息类型
    /// </param>
    /// <param name='bytes'>
    /// 消息的字节数组
    /// </param>
    public static void NetworkServer(ushort id, byte[] bytes)
    {
        

        if (MsgID.SkipLog(id))
        {
            return;
        }

        string s = MsgID.ToString(id);
        s += ("  长度: " + bytes.Length);
        s += "  数据:";
        for (int i = 0; i < bytes.Length; ++i)
        {
            s += (" " + bytes[i].ToString("X2"));
        }
        //Debug.Log(s);
        NetworkServer(s);
    }

    public static void DataLog(string type, string s)
    {
        return;

        string s1 = ">--------------------<\n";
        string s2 = "DataLog : " + type + "\n";
        string s3 = "\n<-------------------->";

        Debug.Log(s1 + s2 + s + s3);
    }

    #endregion 网络消息日志
    
}

