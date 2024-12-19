#nullable enable

using ClassifiedConsole;
using Cysharp.Text;
using System;
using System.Collections.Generic;
using UnityEngine;

// 日志分类
[CDebugSubSystem]
public enum LoggerType
{
    Default,

    [CDebugSubSystemLabel("UI")]
    UI,

    [CDebugSubSystemLabel("客户端")]
    Client,

    [CDebugSubSystemLabel("服务器")]
    Server,
}

public class Logger
{
    private static Logger? _Instance = null;
    public static Logger Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new Logger();
            }
            return _Instance;
        }
    }

    //日志开关
    private Dictionary<LoggerType, bool> logableDict = new Dictionary<LoggerType, bool>();

    //一个分类日志数据
    public class _Logger
    {
        public LoggerType loggerType;

        public bool OpenLog = true;
        public bool OpenLogWarning = true;
        public bool OpenLogError = true;

        public void Log(string pMsg, UnityEngine.Object context = null)
        {
            if (!OpenLog)
                return;
            CDebug.Log(1, pMsg, context, (int)loggerType);
        }

        public void Log(params object[] pMsgs)
        {
            if (!OpenLog)
                return;
            CDebug.Log(1, ZString.Concat(pMsgs), null, (int)loggerType);
        }

        public void LogWarning(string pMsg, UnityEngine.Object context = null)
        {
            if (!OpenLogWarning)
                return;
            CDebug.LogWarning(1, pMsg, context, (int)loggerType);
        }

        public void LogWarning(params object[] pMsgs)
        {
            if (!OpenLogWarning)
                return;
            CDebug.LogWarning(1, ZString.Concat(pMsgs), null, (int)loggerType);
        }

        public void LogError(string pMsg, UnityEngine.Object context = null)
        {
            if (!OpenLogError)
                return;
            CDebug.LogError(1, pMsg, context, (int)loggerType);
        }

        public void LogError(params object[] pMsgs)
        {
            if (!OpenLogError)
                return;
            CDebug.LogError(1, ZString.Concat(pMsgs), null, (int)loggerType);
        }
    }

    //获得日志模块
    public _Logger? this[LoggerType pType]
    {
        get
        {
            // 自定义的Log开关
            if (this.logableDict.TryGetValue(pType, out bool logable))
            {
                if (logable)
                {
                    if (pType == LoggerType.Default)
                    {
                        PrintSystemInfo();
                    }
                    return new _Logger() { loggerType = pType };
                }
            }
            // Editor下，默认Log全开
            if (Application.isEditor)
            {
                if (pType == LoggerType.Default)
                {
                    PrintSystemInfo();
                }
                return new _Logger() { loggerType = pType };
            }

            return null;
        }
    }

    /// <summary>
    /// 开启某个分类日志
    /// </summary>
    /// <param name="pLoggerType"></param>
    /// <param name="isOpen"></param>
    public void SetOpen(LoggerType pLoggerType, bool isOpen)
    {
        if (!logableDict.ContainsKey(pLoggerType))
            logableDict.Add(pLoggerType, false);
        logableDict[pLoggerType] = isOpen;
    }

    /// <summary>
    /// 打印设备信息
    /// </summary>
    private static void PrintSystemInfo()
    {
        Utf8ValueStringBuilder systemInfo = ZString.CreateUtf8StringBuilder();

        systemInfo.AppendLine("*********************************************************************************************************start");
        systemInfo.AppendLine("By: " + SystemInfo.deviceName);
        DateTime now = DateTime.Now;
        systemInfo.AppendLine(string.Concat(new object[] { now.Year.ToString(), "年", now.Month.ToString(), "月", now.Day, "日  ", now.Hour.ToString(), ":", now.Minute.ToString(), ":", now.Second.ToString() }));
        systemInfo.AppendLine();
        systemInfo.AppendLine("操作系统:  " + SystemInfo.operatingSystem);
        systemInfo.AppendLine("系统内存大小:  " + SystemInfo.systemMemorySize);
        systemInfo.AppendLine("设备模型:  " + SystemInfo.deviceModel);
        systemInfo.AppendLine("设备唯一标识符:  " + SystemInfo.deviceUniqueIdentifier);
        systemInfo.AppendLine("处理器数量:  " + SystemInfo.processorCount);
        systemInfo.AppendLine("处理器类型:  " + SystemInfo.processorType);
        systemInfo.AppendLine("显卡标识符:  " + SystemInfo.graphicsDeviceID);
        systemInfo.AppendLine("显卡名称:  " + SystemInfo.graphicsDeviceName);
        systemInfo.AppendLine("显卡标识符:  " + SystemInfo.graphicsDeviceVendorID);
        systemInfo.AppendLine("显卡厂商:  " + SystemInfo.graphicsDeviceVendor);
        systemInfo.AppendLine("显卡版本:  " + SystemInfo.graphicsDeviceVersion);
        systemInfo.AppendLine("显存大小:  " + SystemInfo.graphicsMemorySize);
        systemInfo.AppendLine("显卡着色器级别:  " + SystemInfo.graphicsShaderLevel);
        systemInfo.AppendLine("是否支持内置阴影:  " + SystemInfo.supportsShadows);
        systemInfo.AppendLine("*********************************************************************************************************end");
        systemInfo.AppendLine();
        systemInfo.AppendLine();

        Default?.Log(systemInfo);
    }

    /// <summary>
    /// 默认日志
    /// </summary>
    public static _Logger? Default => Logger.Instance[LoggerType.Default];

    /// <summary>
    /// UI日志
    /// </summary>
    public static _Logger? UI => Logger.Instance[LoggerType.UI];

    /// <summary>
    /// 客户端日志
    /// </summary>
    public static _Logger? Client => Logger.Instance[LoggerType.Client];

    /// <summary>
    /// 服务器日志
    /// </summary>
    public static _Logger? Server => Logger.Instance[LoggerType.Server];
}
//static class Test
//{
//    public struct data
//    {
//        public bool open;
//    }

//    private static data tt = new data();
//    public static data? Data => tt;

//    public static void TestFunc()
//    {
//        // 检查 Data 是否为 null
//        if (Data != null) 
//        { 
//            Data.open = true; 
//        }
//    }
//}

//static class TestB
//{
//    static TestB()
//    {
//        if (Test.Data != null)
//        {
//            Test.Data.open = true;
//        }

//    }
//}

