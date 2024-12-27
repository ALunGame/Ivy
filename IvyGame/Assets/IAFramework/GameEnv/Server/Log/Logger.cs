using Cysharp.Text;
using IAFramework.Log;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    //日志系统
    public static LogSystem logSystem = new LogSystem();

    //日志模块数据
    public struct LoggerData
    {
        private bool openLog;
        private bool openLogWarning;
        private bool openLogError;

        public LogModule Module { get; private set; }

        public LoggerData(LogModule pModule)
        {
            Module = pModule;
            openLog = true;
            openLogWarning = true;
            openLogError = true;
        }

        #region 日志等级开启

        public void SetOpenLog(bool pIsOpen)
        { openLogWarning = pIsOpen; }
        public bool GetOpenLog()
        { return openLog; }

        public void SetOpenLogWarning(bool pIsOpen)
        { openLogWarning = pIsOpen; }
        public bool GetOpenLogWarning()
        { return openLogWarning; }

        public void SetOpenLogError(bool pIsOpen)
        { openLogError = pIsOpen; }
        public bool GetOpenLogError()
        { return openLogError; }

        #endregion

        /// <summary>
        /// 日志（真机不输出）
        /// </summary>
        /// <param name="pMsgs"></param>
        public void Log(params object[] pMsgs)
        {
            if (!openLog)
                return;
            Logger.logSystem.Log(Module, pMsgs);
        }

        /// <summary>
        /// 日志真机输出
        /// </summary>
        /// <param name="pLogModule"></param>
        /// <param name="pMsgs"></param>
        public void RuntimeLog(params object[] pMsgs)
        {
            if (!openLog)
                return;
            Logger.logSystem.RuntimeLog(Module, pMsgs);
        }

        /// <summary>
        /// 警告（真机不输出）
        /// </summary>
        /// <param name="pLogModule"></param>
        /// <param name="pMsgs"></param>
        public void LogWarning(params object[] pMsgs)
        {
            if (!openLogWarning)
                return;
            Logger.logSystem.LogWarning(Module, pMsgs);
        }

        /// <summary>
        /// 警告真机输出
        /// </summary>
        /// <param name="pLogModule"></param>
        /// <param name="pMsgs"></param>
        public void RuntimeWarning(params object[] pMsgs)
        {
            if (!openLogWarning)
                return;
            Logger.logSystem.LogWarning(Module, pMsgs);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="pLogModule"></param>
        /// <param name="pMsgs"></param>
        public void LogError(params object[] pMsgs)
        {
            if (!openLogError)
                return;
            Logger.logSystem.LogError(Module, pMsgs);
        }
    }

    //日志模块开启
    private static Dictionary<LogModule, bool> logEnableDict = new Dictionary<LogModule, bool>();
    private static Dictionary<LogModule, LoggerData> logDataDict = new Dictionary<LogModule, LoggerData>();

    static Logger()
    {
        Application.quitting += OnApplocationQuit;
    }

    private static void OnApplocationQuit()
    {
        Debug.Log("OnApplocationQuit");
        logEnableDict.Clear();
        logDataDict.Clear();
        logSystem.Clear();
    }

    private static LoggerData? GetLoggerData(LogModule pModule) 
    {
        // 自定义的Log开关
        if (logEnableDict.TryGetValue(pModule, out bool logable))
        {
            if (logable)
            {
                if (!logDataDict.ContainsKey(pModule))
                {
                    LoggerData loggerData = new LoggerData(pModule);
                    logSystem.AddLogIO(pModule);
                    logDataDict.Add(pModule, loggerData);
                    if (pModule == LogModule.Default)
                        PrintSystemInfo();
                }
                return logDataDict[pModule];
            }
        }
        // Editor下，默认Log全开
        if (Application.isEditor)
        {
            if (!logDataDict.ContainsKey(pModule))
            {
                LoggerData loggerData = new LoggerData(pModule);
                logSystem.AddLogIO(pModule);
                logDataDict.Add(pModule, loggerData);
                if (pModule == LogModule.Default)
                    PrintSystemInfo();
            }
            return logDataDict[pModule];
        }

        return null;
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

    }

    /// <summary>
    /// 设置日志模块开启
    /// </summary>
    /// <param name="pModule"></param>
    /// <param name="pIsOpen"></param>
    public static void SetLogModuleOpen(LogModule pModule, bool pIsOpen)
    {
        if (!logEnableDict.ContainsKey(pModule))
            logEnableDict.Add(pModule, pIsOpen);
        else
            logEnableDict[pModule] = pIsOpen;
    }

    /// <summary>
    /// 编辑器下日志
    /// </summary>
    public static LoggerData? Editor => Logger.GetLoggerData(LogModule.Editor);

    /// <summary>
    /// 默认日志
    /// </summary>
    public static LoggerData? Default => Logger.GetLoggerData(LogModule.Default);

    /// <summary>
    /// UI日志
    /// </summary>
    public static LoggerData? UI => Logger.GetLoggerData(LogModule.UI);

    /// <summary>
    /// 客户端日志
    /// </summary>
    public static LoggerData? Client => Logger.GetLoggerData(LogModule.Client);

    /// <summary>
    /// 服务器日志
    /// </summary>
    public static LoggerData? Server => Logger.GetLoggerData(LogModule.Server);
}