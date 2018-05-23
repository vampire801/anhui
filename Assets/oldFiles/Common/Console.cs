using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// A console that displays the contents of Unity's debug log.
/// </summary>
/// <remarks>
/// Developed by Matthew Miner (www.matthewminer.com)
/// Permission is given to use this script however you please with absolutely no restrictions.
/// </remarks>

namespace CmdEditor
{
    public class Console : MonoBehaviour
    {
        private static Console m_Instance = null;

        public static Console GetInstance()
        {
            return m_Instance;
        }

        private UnityEngine.Object m_MsgLock = new UnityEngine.Object();

        public static readonly Version version = new Version(1, 0);

        struct ConsoleMessage
        {
            public readonly string message;
            public readonly string stackTrace;
            public readonly LogType type;

            public ConsoleMessage(string message, string stackTrace, LogType type)
            {
                this.message = message;
                this.stackTrace = stackTrace;
                this.type = type;
            }
        }

        public KeyCode toggleKey = KeyCode.BackQuote;

        List<ConsoleMessage> entries = new List<ConsoleMessage>();
        Vector2 scrollPos;
        public bool show;
        bool collapse;

        // Visual elements:

        const int margin = 50;
        Rect windowRect = new Rect(margin, margin, Screen.width - (2 * margin), Screen.height - (2 * margin));

        GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
        GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

        void OnEnable() { Application.logMessageReceived += (HandleLog); }
        void OnDisable() { Application.logMessageReceived -= (HandleLog); }

        void Awake()
        {
            m_Instance = this;
            //            GameObject.DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                show = !show;
            }

            UpdateHelper();
        }

        void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, Screen.width / 10, Screen.height / 10), "Debug"))
            {
                show = !show;
            }

            if (!show)
            {
                return;
            }

            windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
        }

        /// <summary>
        /// A window displaying the logged messages.
        /// </summary>
        /// <param name="windowID">The window's ID.</param>
        void ConsoleWindow(int windowID)
        {
            lock (m_MsgLock)
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos);

                GUIStyle style = new GUIStyle();
                style.normal.background = null;

#if UNITY_EDITOR
                style.fontSize = 40;
#elif UNITY_ANDROID
				style.fontSize = 50;
#endif

                // Go through each logged entry
                for (int i = 0; i < entries.Count; i++)
                {
                    ConsoleMessage entry = entries[i];

                    // If this message is the same as the last one and the collapse feature is chosen, skip it
                    if (collapse && i > 0 && entry.message == entries[i - 1].message)
                    {
                        continue;
                    }

                    // Change the text colour according to the log type
                    switch (entry.type)
                    {
                        case LogType.Error:
                        case LogType.Exception:
                            GUI.contentColor = Color.red;
                            break;

                        case LogType.Warning:
                            GUI.contentColor = Color.yellow;
                            break;

                        default:
                            GUI.contentColor = Color.white;
                            break;
                    }

                    style.normal.textColor = GUI.contentColor;

                    GUILayout.Label(entry.message, style);
                }

                GUI.contentColor = Color.white;

                GUILayout.EndScrollView();

                GUILayout.BeginHorizontal();

                // Clear button
                if (GUILayout.Button(clearLabel))
                {
                    entries.Clear();
                }

                // Collapse toggle
                collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

                GUILayout.EndHorizontal();

                // Set the window to be draggable by the top title bar
                GUI.DragWindow(new Rect(0, 0, 10000, 40));
            }
        }

        /// <summary>
        /// Logged messages are sent through this callback function.
        /// </summary>
        /// <param name="message">The message itself.</param>
        /// <param name="stackTrace">A trace of where the message came from.</param>
        /// <param name="type">The type of message: error/exception, warning, or assert.</param>
        void HandleLog(string message, string stackTrace, LogType type)
        {
            //            if (type == LogType.Log)
            //                return;

            AutoClear();

            lock (m_MsgLock)
            {
                ConsoleMessage entry = new ConsoleMessage(message, stackTrace, type);
                entries.Add(entry);
            }
        }

        public void Log(string message)
        {
            AutoClear();

            lock (m_MsgLock)
            {
                ConsoleMessage entry = new ConsoleMessage(message, "", LogType.Log);
                entries.Add(entry);
            }
        }

        public void LogWarning(string message)
        {
            AutoClear();

            lock (m_MsgLock)
            {
                ConsoleMessage entry = new ConsoleMessage(message, "", LogType.Warning);
                entries.Add(entry);
            }
        }

        public void LogError(string message)
        {
            AutoClear();

            lock (m_MsgLock)
            {
                ConsoleMessage entry = new ConsoleMessage(message, "", LogType.Error);
                entries.Add(entry);
            }
        }

        private void AutoClear()
        {
            lock (m_MsgLock)
            {
                if (entries.Count > 3000)
                {
                    entries.RemoveRange(0, 100);
                    //entries.Clear();
                }
            }
        }


        #region

        private void UpdateHelper()
        {
            FTime = Time.realtimeSinceStartup;
        }

        public static float FTime = 0.0f;

        public static string STime
        {
            get
            {
                string t1 = FormatTime(FTime);
                string t2 = ("[ " + System.DateTime.Now + " ] \t");

                //			long sec = System.DateTime.Now.Ticks / 10000000;
                //			long ms  = System.DateTime.Now.Ticks % 10000000;
                //			string t2 = ("[ " + sec + "." + ms + " ] \t");

                return t1 + t2;
            }
        }

        public static string FormatTime(float fTime)
        {
            int h = (int)(fTime / (60 * 60));
            int m = (int)((fTime % (60 * 60)) / 60);
            int s = (int)(fTime % 60);
            string sh = h < 10 ? ("0" + h) : ("" + h);
            string sm = m < 10 ? ("0" + m) : ("" + m);
            string ss = s < 10 ? ("0" + s) : ("" + s);

            string t1 = ("[ " + sh + " : " + sm + " : " + ss + " ] \t");
            return t1;
        }

        public static void LOG(string s)
        {
            if (Console.GetInstance() != null)
            {
                Console.GetInstance().Log(s);
            }
            else
            {
                Debug.Log(s);
            }
        }
        #endregion
    }
}