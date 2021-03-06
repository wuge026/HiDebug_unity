﻿//*********************************************************************
// Description:日志
// Author: hiramtan@live.com
//*********************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public sealed class Debuger : MonoBehaviour
{
    /// <summary>
    /// 显示在屏幕上的字体大小
    /// </summary>
    public static int fontSize = 15;
    /// <summary>
    /// 显示在屏幕上多少列日志
    /// </summary>
    public static int itemCountOnScreen = 100;

    private static bool isLogOnScreen = false;
    private static bool isLogOnConsole = false;
    private static bool isLogOnText = false;
    private static bool isFpsOn = false;
    private static List<string> logOnScreenList = new List<string>();
    private static string logOnScreen;
    private static Vector2 scrollPosition;
    private static Debuger instance;
    private Debuger() { }
    public static void EnableOnConsole(bool _isLogOnConsole)
    {
        isLogOnConsole = _isLogOnConsole;
    }
    public static void EnableOnScreen(bool _isLogOnScreen)
    {
        if (_isLogOnScreen)
            EnableOnConsole(true);
        isLogOnScreen = _isLogOnScreen;
        if (instance == null)
        {
            var tempGo = new GameObject("Debuger");
            DontDestroyOnLoad(tempGo);
            instance = tempGo.AddComponent<Debuger>();
        }
        Application.logMessageReceived += (string _log, string _stackTrace, LogType _type) =>
        {
            UpdateScrollPosition();
        };
        Application.logMessageReceivedThreaded += (string _log, string _stackTrace, LogType _type) =>
         {
             UpdateScrollPosition();
         };
    }
    public static void EnableOnText(bool param)
    {
        if (param)
            EnableOnConsole(true);
        isLogOnText = param;
    }
    public static void EnableFps(bool param)
    {
        isFpsOn = param;
        if (instance == null)
            instance = new GameObject("Debuger").AddComponent<Debuger>();
    }
    public static void Log(object _obj)
    {
        if (isLogOnConsole)
        {
            string log = string.Format(GetTime(), _obj.ToString());
            log = "<color=white>" + log + "</color>";
            Debug.Log(log);
            if (isLogOnScreen)
                OnScreen(log);
            if (isLogOnText)
                WriteLog(log);
        }
    }
    public static void LogWarning(object _obj)
    {
        if (isLogOnConsole)
        {
            string log = string.Format(GetTime(), _obj.ToString());
            log = "<color=yellow>" + log + "</color>";
            Debug.LogWarning(log);
            if (isLogOnScreen)
                OnScreen(log);
            if (isLogOnText)
                WriteLog(log);
        }
    }
    public static void LogError(object _obj)
    {
        if (isLogOnConsole)
        {
            string log = string.Format(GetTime(), _obj.ToString());
            log = "<color=red>" + log + "</color>";
            Debug.LogError(log);
            if (isLogOnScreen)
                OnScreen(log);
            if (isLogOnText)
                WriteLog(log);
        }
    }
    void OnGUI()
    {
        if (isLogOnScreen)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
            GUIStyle tempGuiStyle = new GUIStyle();
            tempGuiStyle.fontSize = fontSize;
            GUILayout.Label(logOnScreen, tempGuiStyle);
            GUILayout.EndScrollView();
        }
        if (isFpsOn)
        {
            GUIStyle tempGuiStyle = new GUIStyle();
            tempGuiStyle.fontSize = fontSize;
            tempGuiStyle.normal.textColor = Color.red;
            GUI.Label(new Rect(0, 0, Screen.width * 0.3f, Screen.height * 0.1f), fps.ToString(), tempGuiStyle);
        }
    }
    private float count;//总次数
    private float lastTime;//记录上次的时间
    private float fps;
    private int i;//计数器
    void Update()
    {
        if (isFpsOn)
        {
            i++;
            count += Time.timeScale / Time.deltaTime;
            if (Time.realtimeSinceStartup > Time.timeScale + lastTime)
            {
                fps = count / i;
                count = i = 0;
                lastTime = Time.realtimeSinceStartup;
            }
        }
    }
    static void UpdateScrollPosition()
    {
        scrollPosition = new Vector2(scrollPosition.x, scrollPosition.y + logOnScreenList.Count);
    }
    private static void OnScreen(string _log)
    {
        logOnScreenList.Add(_log);
        if (logOnScreenList.Count > itemCountOnScreen)
            logOnScreenList.RemoveAt(0);
        logOnScreen = string.Empty;
        foreach (string _s in logOnScreenList)
            logOnScreen = string.IsNullOrEmpty(logOnScreen) ? logOnScreen = _s : logOnScreen = logOnScreen + "\n" + _s;//第一行不用换行
    }
    private static string GetTime()
    {
        DateTime time = DateTime.Now;
        return time.ToString("yyyy.MM.dd HH:mm:ss") + ": {0}";
    }
    private static void WriteLog(string param)
    {
        string path = Application.persistentDataPath + "/Debuger.txt";
        StreamWriter sw = File.AppendText(path);
        sw.WriteLine(param);
        sw.Close();
    }
}