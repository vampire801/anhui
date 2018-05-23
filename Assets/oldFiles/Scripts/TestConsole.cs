
#define USE_TESTCONSOLE  
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using XLua;

namespace Consolation
{
    [Hotfix]
    [LuaCallCSharp]
    /// <summary>  
    /// A console to display Unity's debug logs in-game.  
    /// </summary>  
    public class TestConsole : MonoBehaviour
    {
#if USE_TESTCONSOLE
        struct Log
        {
            public string message;
            public string stackTrace;
            public LogType type;
        }

        #region Inspector Settings  

        /// <summary>  
        /// The hotkey to show and hide the console window.  
        /// </summary>  
        public KeyCode toggleKey = KeyCode.BackQuote;
        public UnityEngine.UI.Toggle _isShowWind;
        /// <summary>  
        /// Whether to open the window by shaking the device (mobile-only).  
        /// </summary>  
        public bool shakeToOpen = true;

        /// <summary>  
        /// The (squared) acceleration above which the window should open.  
        /// </summary>  
        public float shakeAcceleration = 3f;

        /// <summary>  
        /// Whether to only keep a certain number of logs.  
        ///  
        /// Setting this can be helpful if memory usage is a concern.  
        /// </summary>  
        public bool restrictLogCount = false;

        /// <summary>  
        /// Number of logs to keep before removing old ones.  
        /// </summary>  
        public int maxLogs;

        #endregion

        readonly List<Log> logs = new List<Log>();
        Vector2 scrollPosition;
        public bool visible;
        bool collapse;
        Stopwatch sw = new Stopwatch();
        // Visual elements:  

        static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
        {
            { LogType.Assert, Color.white },
            { LogType.Error, Color.red },
            { LogType.Exception, Color.blue},
            { LogType.Log, Color.white },
            { LogType.Warning, Color.yellow },
        };

        const string windowTitle = "Console";
        const int margin = 30;
        static readonly GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
        static readonly GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

        readonly Rect titleBarRect = new Rect(0, 0, 10000, 50);
        Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 3), Screen.height - (margin * 4));
        GUIStyle bb = new GUIStyle();


        void Start()
        {
            if (MahjongLobby_AH.LobbyContants.SeverType == "" || (MahjongLobby_AH.SDKManager.Instance.CheckStatus == 1 || MahjongLobby_AH.SDKManager.Instance.IOSCheckStaus == 1))
            {
#if OpenDebug
                 return;
#endif

                //隐藏打印日志的界面
                transform.Find("Console").gameObject.SetActive(false);
                Destroy(transform.GetComponent<TestConsole>());
            }
        }



        void OnEnable()
        {
            sw.Start();
            bb.fontSize = 30;

#if UNITY_5
            Application.logMessageReceived += HandleLog;
#else
            Application.RegisterLogCallback(HandleLog);  
#endif
        }

        void OnDisable()
        {
#if UNITY_5
            Application.logMessageReceived -= HandleLog;
#else
            Application.RegisterLogCallback(null);  
#endif
        }

        void Update()
        {

            if (Input.GetKeyDown(toggleKey) || !_isShowWind.isOn)
            {
                visible = false;
            }

            if (shakeToOpen && Input.acceleration.sqrMagnitude > shakeAcceleration || _isShowWind.isOn)
            {
                visible = true;
            }
        }

        void OnGUI()
        {
            if (!visible)
            {
                return;
            }

            windowRect = GUILayout.Window(123456, windowRect, DrawConsoleWindow, windowTitle);
        }

        /// <summary>  
        /// Displays a window that lists the recorded logs.  
        /// </summary>  
        /// <param name="windowID">Window ID.</param>  
        void DrawConsoleWindow(int windowID)
        {
            DrawLogsList();
            DrawToolbar();

            // Allow the window to be dragged by its title bar.  
            GUI.DragWindow(titleBarRect);
        }

        /// <summary>  
        /// Displays a scrollable list of logs.  
        /// </summary>  
        void DrawLogsList()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            // Iterate through the recorded logs.  
            for (var i = 0; i < logs.Count; i++)
            {
                var log = logs[i];

                // Combine identical messages if collapse option is chosen.  
                if (collapse && i > 0)
                {
                    var previousMessage = logs[i - 1].message;

                    if (log.message == previousMessage)
                    {
                        continue;
                    }
                }
                bb.fontSize = 30;
                bb.normal.textColor = logTypeColors[log.type];


                //GUI.contentColor = logTypeColors[log.type];  
                GUILayout.Label(log.message, bb);
            }

            GUILayout.EndScrollView();

            // Ensure GUI colour is reset before drawing other components.  
            GUI.contentColor = Color.white;
        }

        /// <summary>  
        /// Displays options for filtering and changing the logs list.  
        /// </summary>  
        void DrawToolbar()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(clearLabel))
            {
                logs.Clear();
            }

            collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

            GUILayout.EndHorizontal();
        }

        /// <summary>  
        /// Records a log from the log callback.  
        /// </summary>  
        /// <param name="message">Message.</param>  
        /// <param name="stackTrace">Trace of where the message came from.</param>  
        /// <param name="type">Type of message (error, exception, warning, assert).</param>  
        void HandleLog(string message, string stackTrace, LogType type)
        {
            logs.Add(new Log
            {
                message = sw.ElapsedMilliseconds / 60000 + ":" + (sw.ElapsedMilliseconds % 60000) / 1000 + "://" + message,
                stackTrace = stackTrace,
                type = type,
            });

            TrimExcessLogs();
        }

        /// <summary>  
        /// Removes old logs that exceed the maximum number allowed.  
        /// </summary>  
        void TrimExcessLogs()
        {
            if (!restrictLogCount)
            {
                return;
            }

            var amountToRemove = Mathf.Max(logs.Count - maxLogs, 0);

            if (amountToRemove == 0)
            {
                return;
            }

            logs.RemoveRange(0, amountToRemove);
        }
#endif
    }
}