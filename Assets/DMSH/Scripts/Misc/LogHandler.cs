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

    [SerializeField] private Queue _logQueue = new Queue();
    [SerializeField] private string _logBuffer;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GUIStyle _style;

    public static bool      drawConsole = false;
    private List<LogMessage> _consoleMessageBuffer = new List<LogMessage>();
    private static Vector2  _scrollPosition = new Vector2(0.0f, 0.0f);
    
    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();

        StartCoroutine(TimerToClear());
    }

    private void Clear()
    {
        _logBuffer = null;
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

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        _logBuffer = logString;
        string newString = "\n [" + type + "] : " + _logBuffer;

        _logQueue.Enqueue(newString);

        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            _logQueue.Enqueue(newString);
        }

        _logBuffer = string.Empty;
        foreach (string mylog in _logQueue)
            _logBuffer += mylog;
        
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

    private void OnGUI()
    {
        if (drawConsole)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

            GUILayout.BeginHorizontal();
            if (GUI.Button(new Rect(0, 0, 70, 30), "Clear"))
                _consoleMessageBuffer.Clear();
            GUILayout.EndHorizontal();

            GUILayout.BeginArea(new Rect(0, 50, Screen.width, Screen.height - 70));
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
                        case LogType.Error:
                            style.normal.textColor = Color.red;
                            break;
                        case LogType.Assert:
                            style.normal.textColor = Color.red;
                            break;
                        case LogType.Warning:
                            style.normal.textColor = Color.yellow;
                            break;
                    }

                    GUILayout.Label(message.Message, style);

                    if(message.StackTrace != string.Empty)
                        GUILayout.Label(message.StackTrace, style);
                }

                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        }
        else
        {
            if (_playerController.debugLog)
                GUILayout.Label(_logBuffer, _style, GUILayout.Height(500));
        }
    }

}
