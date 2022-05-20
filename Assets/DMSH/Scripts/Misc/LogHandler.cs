using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LogMessage
{
    public string StackTrace = string.Empty;
    public string Message = string.Empty;
    public LogType Type = LogType.Log;
}

public class LogHandler : MonoBehaviour
{
    private const int MAX_MESSAGES_COUNT = 200;

    public List<Tuple<string, Action>> consoleCommandsList = new List<Tuple<string, Action>>();
    public bool             drawLogMessages = false;
    public bool             drawConsole = false;

    [SerializeField] private GUIStyle _style;
    [SerializeField] private Queue _logQueue = new Queue();
    [SerializeField] private string _tempMessagesBuffer;
    [SerializeField] private string _command;
    [SerializeField] private List<LogMessage> _consoleMessageBuffer = new List<LogMessage>();
    [SerializeField] private Vector2  _scrollPosition = new Vector2(0.0f, 0.0f);
    
    protected void Start()
    {
        StartCoroutine(TimerToClear());

        consoleCommandsList.Add(new Tuple<string, Action>("Help", () => {
            Debug.Log("Command List:\n");
            foreach (Tuple<string, Action> item in consoleCommandsList)
            { Debug.Log($"{item.Item1}"); }
        }));
        consoleCommandsList.Add(new Tuple<string, Action>("Clear", () => { _consoleMessageBuffer.Clear(); }));
        consoleCommandsList.Add(new Tuple<string, Action>("TestLog", () => { Debug.Log("Hi"); }));
        consoleCommandsList.Add(new Tuple<string, Action>("TestAssert", () => { Debug.Assert(false, "Assert Hi"); }));
        consoleCommandsList.Add(new Tuple<string, Action>("TestException", () => { Debug.LogException(new NotImplementedException()); }));
        consoleCommandsList.Add(new Tuple<string, Action>("KillPlayer", () => {
            PlayerController player = FindObjectOfType<PlayerController>();
            player.Kill();
        }));
        consoleCommandsList.Add(new Tuple<string, Action>("KillAllEnemy", () => {
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
        _tempMessagesBuffer = logString;
        string newString = "\n [" + type + "] : " + _tempMessagesBuffer;
        _logQueue.Enqueue(newString);
        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            _logQueue.Enqueue(newString);
        }

        _tempMessagesBuffer = string.Empty;
        foreach (string mylog in _logQueue)
            _tempMessagesBuffer += mylog;
        
        _scrollPosition.y = Mathf.Infinity;

        LogMessage logMessage = new LogMessage();
        logMessage.Type = type;
        logMessage.Message = logString;
        if (type == LogType.Exception)
            logMessage.StackTrace = stackTrace;

        if (_consoleMessageBuffer.Count > MAX_MESSAGES_COUNT)
            _consoleMessageBuffer.RemoveAt(0);

        _consoleMessageBuffer.Add(logMessage);

    }

    protected void OnGUI()
    {
        if (drawConsole)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height - 25));
            {
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));

                foreach (LogMessage message in _consoleMessageBuffer)
                {
                    GUIStyle style = _style;
                    switch (message.Type)
                    {
                        case LogType.Log:
                            style.normal.textColor = new Color(192, 192, 192);
                            break;
                        case LogType.Error: case LogType.Assert: case LogType.Exception:
                            style.normal.textColor = Color.red;
                            break;
                        case LogType.Warning:
                            style.normal.textColor = Color.yellow;
                            break;
                    }

                    GUILayout.Label($"[{message.Type}] {message.Message}", style);

                    if(message.StackTrace != string.Empty)
                        GUILayout.Label(message.StackTrace, style);
                }

                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();

            if (Event.current.type == EventType.KeyDown) 
            {
                if (Event.current.keyCode == KeyCode.Return && Event.current.isKey)
                {
                    //TODO
                    //Need find better way to get function by name
                    foreach (Tuple<string, Action> item in consoleCommandsList)
                    {
                        if (_command == item.Item1)
                        {
                            item.Item2?.Invoke();
                            _command = string.Empty;
                            break;
                        }

                        if (consoleCommandsList.IndexOf(item)  == consoleCommandsList.Count - 1)
                            Debug.LogError("No such command found!");

                    }
                }
            }

            GUILayout.BeginArea(new Rect(0, Screen.height - 25, Screen.width, 100));
            _command = GUILayout.TextField(_command);
            GUILayout.EndArea();


        }
        else
        {
            if(drawLogMessages)
                GUILayout.Label(_tempMessagesBuffer, _style, GUILayout.Height(500));
        }
    }

}
