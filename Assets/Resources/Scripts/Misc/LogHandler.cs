using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DMSH.Characters;

namespace DMSH.Misc.Log
{
    public class LogMessage
    {
        public string messageStackTrace { get; set; }
        public string messageText { get; set; }
        public LogType messageType { get; set; }
        public string messageSendTime { get; set; }
    }

    public class LogHandler : MonoBehaviour
    {
        private const int MAX_MESSAGES_COUNT = 200;

        [Header("Commands")]
        public List<Tuple<string, Action>> consoleCommandsList = new List<Tuple<string, Action>>();

        [Header("Draw")]
        public bool drawLogMessages = false;
        public bool drawConsole = false;

        [Header("Misc")]
        [SerializeField] private Font _font;
        [SerializeField] private int _fontSize;
        [SerializeField] private Queue _logQueue = new Queue();
        [SerializeField] private string _tempMessagesBuffer;
        [SerializeField] private string _command;
        [SerializeField] private List<LogMessage> _consoleMessageBuffer = new List<LogMessage>();
        [SerializeField] private Vector2 _scrollPosition = new Vector2(0.0f, 0.0f);
        [SerializeField] private int _savedGameActiveState;
        [SerializeField] private bool _Resume = true;
        [SerializeField] private bool _fpsCounter = false;

        protected void Start()
        {
            StartCoroutine(TimerToClear());

            consoleCommandsList.Add(new Tuple<string, Action>("fps", () =>
            {
                _fpsCounter = !_fpsCounter;
            }));
            consoleCommandsList.Add(new Tuple<string, Action>("help", () =>
            {
                Debug.Log($"Command List:{consoleCommandsList.Count}\n");
                foreach (Tuple<string, Action> item in consoleCommandsList)
                { Debug.Log($"{item.Item1}"); }
            }));
            consoleCommandsList.Add(new Tuple<string, Action>("tmessages", () =>
            {
                drawLogMessages = !drawLogMessages;
            }));
            consoleCommandsList.Add(new Tuple<string, Action>("dgui", () =>
            {
                PlayerController player = FindObjectOfType<PlayerController>();
                player.DebugGUI = !player.DebugGUI;
            }));
            consoleCommandsList.Add(new Tuple<string, Action>("tc", () =>
            {
                foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
                    audio?.Stop();
                _Resume = !_Resume;
            }));

            consoleCommandsList.Add(new Tuple<string, Action>("clear", () => { _consoleMessageBuffer.Clear(); }));
            consoleCommandsList.Add(new Tuple<string, Action>("testLog", () => { Debug.Log("Hi"); }));
            consoleCommandsList.Add(new Tuple<string, Action>("testassert", () => { Debug.Assert(false, "Assert Hi"); }));
            consoleCommandsList.Add(new Tuple<string, Action>("testexception", () => { Debug.LogException(new NotImplementedException()); }));
            consoleCommandsList.Add(new Tuple<string, Action>("killplayer", () =>
            {
                PlayerController player = FindObjectOfType<PlayerController>();
                player.Kill();
            }));
            consoleCommandsList.Add(new Tuple<string, Action>("god", () =>
            {
                PlayerController player = FindObjectOfType<PlayerController>();
                player.CheatGod = !player.CheatGod;
            }));
            consoleCommandsList.Add(new Tuple<string, Action>("infboost", () =>
            {
                PlayerController player = FindObjectOfType<PlayerController>();
                player.CheatInfBoost = !player.CheatInfBoost;
            }));
            consoleCommandsList.Add(new Tuple<string, Action>("killenemy", () =>
            {
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                    enemy.Kill(true);
            }));
        }

        private void Clear()
        {
            _tempMessagesBuffer = null;
            _logQueue.Clear();
        }

        private void Awake()
        {
            Clear();
        }

        IEnumerator TimerToClear()
        {
            while (true)
            {
                Clear();
                yield return new WaitForSeconds(2f);
            }
        }

        protected void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        protected void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            // Build new string 
            string newString = "\n [" + type + "] : " + logString;
            _logQueue.Enqueue(newString);
            if (type == LogType.Exception)
            {
                newString = "\n" + stackTrace;
                _logQueue.Enqueue(newString);
            }

            // Reset _tempMessagesBuffer 
            _tempMessagesBuffer = string.Empty;
            foreach (string mylog in _logQueue)
                _tempMessagesBuffer += mylog;

            // Create message 
            LogMessage logMessage = new LogMessage();
            logMessage.messageType = type;
            logMessage.messageText = logString;
            logMessage.messageSendTime = DateTime.Now.ToString();

            if (type == LogType.Exception)
                logMessage.messageStackTrace = stackTrace;

            if (_consoleMessageBuffer.Count > MAX_MESSAGES_COUNT)
                _consoleMessageBuffer.RemoveAt(0);

            _consoleMessageBuffer.Add(logMessage);

            _scrollPosition.y = Mathf.Infinity;
        }

        protected void OnGUI()
        {
            GUIStyle textStyle = new GUIStyle(GUI.skin.label);
            textStyle.font = _font;
            textStyle.fontSize = _fontSize;

            if (Event.current.type == EventType.KeyUp)
            {
                if (Event.current.keyCode == KeyCode.Escape && Event.current.isKey)
                    drawConsole = false;

                if (Event.current.keyCode == KeyCode.BackQuote && Event.current.isKey)
                {
                    _command = string.Empty;
                    drawConsole = !drawConsole;
                    Cursor.visible = !_Resume || (drawConsole || !Convert.ToBoolean(_savedGameActiveState));

                    if (_Resume)
                    {
                        if (drawConsole)
                            _savedGameActiveState = GlobalSettings.gameActiveAsInt; //FIX ME: I'm too lazy to fix type 

                        GlobalSettings.SetGameActive(_savedGameActiveState == 1 && !drawConsole);
                    }


                    GUI.FocusControl("UICommandTextField");
                }

                if (Event.current.keyCode == KeyCode.Return && Event.current.isKey && drawConsole)
                {
                    if (_command == string.Empty)
                        return;

                    foreach (Tuple<string, Action> item in consoleCommandsList)
                    {
                        if (_command.IndexOf(item.Item1, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            Debug.Log($"~{_command}");
                            item.Item2?.Invoke();
                            _command = string.Empty;
                            break;
                        }

                        if (consoleCommandsList.IndexOf(item) == consoleCommandsList.Count - 1)
                            Debug.LogError($"{_command} - no such command found!");
                    }
                }
            }

            if (_fpsCounter)
            {
                GUILayout.BeginArea(new Rect(0, 30, 500, 500));
                GUILayout.Label($"FPS:{(int)(1f / Time.unscaledDeltaTime)}", textStyle);
                GUILayout.EndArea();
            }

            if (drawConsole)
            {
                GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height - 35));
                {
                    _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height - 35));

                    foreach (LogMessage message in _consoleMessageBuffer)
                    {
                        switch (message.messageType)
                        {
                            case LogType.Log:
                                textStyle.normal.textColor = new Color(192, 192, 192);
                                break;
                            case LogType.Error:
                            case LogType.Assert:
                            case LogType.Exception:
                                textStyle.normal.textColor = Color.red;
                                break;
                            case LogType.Warning:
                                textStyle.normal.textColor = Color.yellow;
                                break;
                        }

                        GUILayout.Label($"[{message.messageSendTime}] [{message.messageType}] {message.messageText}", textStyle);

                        if (message.messageStackTrace != string.Empty)
                            GUILayout.Label(message.messageStackTrace, textStyle);
                    }

                    GUILayout.EndScrollView();
                }
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect(0, Screen.height - 25, Screen.width, 100));
                GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);
                //textFieldStyle.normal.background = null;
                //textFieldStyle.onNormal.background = null;
                textFieldStyle.font = _font;
                textFieldStyle.fontSize = _fontSize;

                GUI.SetNextControlName("UICommandTextField");
                _command = GUILayout.TextField(_command, textFieldStyle);
                GUILayout.EndArea();
            }
            else
            {
                if (drawLogMessages)
                {
                    textStyle.normal.textColor = new Color(255, 97, 0);
                    GUILayout.Label(_tempMessagesBuffer, textStyle, GUILayout.Height(500));

                    GUILayout.BeginArea(new Rect(Screen.width - 100, 0, 500, 500));
                    GUILayout.Label($"gameActive: {GlobalSettings.gameActiveAsBool}", textStyle);
                    GUILayout.EndArea();
                }
            }
        }
    }
}