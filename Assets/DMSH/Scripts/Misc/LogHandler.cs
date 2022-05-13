using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogHandler : MonoBehaviour
{
    public static bool      drawConsole = false;
    private static string   _consoleBuffer = string.Empty;
    private static Vector2  _scrollPosition = new Vector2(0.0f, 0.0f);

    [SerializeField] private Queue _logQueue = new Queue();
    //TODO
    //Cleanup 
    [SerializeField] private string _logBuffer;
    [SerializeField] private PlayerController _playerController;

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

        _consoleBuffer += _logBuffer;
    }

    private void OnGUI()
    {
        if (drawConsole)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
            GUILayout.Label(_consoleBuffer);
            GUILayout.EndScrollView();
        }
        else
        {
            if (_playerController.debugLog)
                GUILayout.Label(_logBuffer, GUILayout.Height(500));
        }
    }

}
