using IAToolkit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace IAServer
{
    public enum LogType
    {
        Log,
        LogWarn,
        LogError,
    } 


    public static class LogUtility
    {
        public static string LogSaveRootPath = $"{PathHelper.SandboxDir}/Logs/";

        private static string logfileTime = $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";
        private static string unityLogModule = "UnityLog";

        private static readonly object logLock = new object();
        private static Dictionary<string, StreamWriter> swDict = new Dictionary<string, StreamWriter>();

        private static readonly object currLogModuleLock = new object();
        public static string currLogModule = "";

        private static readonly object logIndexLock = new object();
        private static uint logIndex = 0;

        /// <summary>
        /// 对栈的输出长度
        /// </summary>
        public static int stackTraceSpacing = 128;

        static LogUtility()
        {
            
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            logIndex = 0;
            currLogModule = unityLogModule;
            InitLogLogModule(unityLogModule);
            PrintSystemInfo();
            PrintHeader();

            Application.logMessageReceivedThreaded += LogCallBack;
            Application.quitting += ApplicationQuit;
        }

        /// <summary>
        /// 打印设备信息
        /// </summary>
        private static void PrintSystemInfo()
        {
            StringBuilder systemInfo = new();

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

            //将日志输出到文本文件。
            WriteLog(unityLogModule, systemInfo);
        }

        /// <summary>
        /// 打印日志页眉到日志中。
        /// </summary>
        /// [ DATE ]    [ TIME ]    [ TYPE ]    [ STACK TRACE ]    [ MESSAGE ] 
        private static void PrintHeader()
        {
            StringBuilder header = new();
            PrintMeter("INDEX", header, 14);
            PrintMeter("DATE", header, 14);
            PrintMeter("TIME", header, 12);
            PrintMeter("TYPE", header, 10);
            PrintMeter("STACK TRACE", header, stackTraceSpacing);
            PrintMeter("MESSAGE", header);

            //将日志输出到文本文件。
            WriteLog(unityLogModule, header);
        }

        /// <summary>
        /// 打印表头到指定的StringBuilder中。
        /// </summary>
        /// <param name="meter">表头文本。</param>
        /// <param name="sbuilder">要追加内容的StringBuilder，如果为null则新建一个。</param>
        /// <param name="space">表头占用的空间宽度。</param>
        /// <returns>返回追加了表头的StringBuilder。</returns>
        private static StringBuilder PrintMeter(string meter, StringBuilder sbuilder = null, int space = 1)
        {
            sbuilder ??= new();
            space = Mathf.Max(space, meter.Length + 4);
            //[
            sbuilder.Append('[');
            //前空格
            sbuilder.Append(" ");
            //item
            sbuilder.Append(meter);
            //后空格
            int limit = space - 3 - meter.Length;
            for (int i = 0; i < limit; i++)
                sbuilder.Append(" ");
            //]
            sbuilder.Append(']');
            //制表符
            sbuilder.Append('\t');
            return sbuilder;    //[ meter ]
        }

        /// <summary>
        /// 日志回调方法，当Unity产生日志时调用。
        /// </summary>
        /// <param name="pLog">日志消息。</param>
        /// <param name="pStackTrace">堆栈跟踪信息。</param>
        /// <param name="pType">日志类型。</param>
        private static void LogCallBack(string pLog, string pStackTrace, UnityEngine.LogType pType)
        {
            //当前索引
            StringBuilder _stringBuilder;
            lock (logIndexLock)
            {
                logIndex++;
                _stringBuilder = PrintItem(logIndex.ToString(), 14);
            }

            // 获取当前时间。
            DateTime now = DateTime.Now;

            //Date
            _stringBuilder.Append(PrintItem($"{now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)}", 14));

            // Time
            _stringBuilder.Append(PrintItem(now.ToLongTimeString(), 12));

            // Type
            _stringBuilder.Append(PrintItem(pType.ToString(), 10));

            // Stacktrace
            /*            stackTrace = SimplifyStackTrace(stackTrace);*/
            pStackTrace = pStackTrace.Replace(" ", string.Empty);
            pStackTrace = pStackTrace.Replace("\n", " ");
            pStackTrace = pStackTrace.Length > stackTraceSpacing - 4
               ? pStackTrace.Substring(0, stackTraceSpacing - 4)
               : pStackTrace;    //进行字符串切割，避免超过最大限度
            _stringBuilder.Append(PrintItem(pStackTrace, stackTraceSpacing));

            //Message
            _stringBuilder.Append(PrintItem(pLog, pLog.Length + 4));

            //将日志输出到文本文件。
            WriteLog(currLogModule == "" ? unityLogModule : currLogModule, _stringBuilder);

            //重置日志模块
            SetCurrLogModule(unityLogModule);
        }

        /// <summary>
        /// 打印条目
        /// </summary>
        /// <param name="pItem">单条条目</param>
        /// <param name="pSpace">单条条目占用的空间宽度</param>
        /// <param name="pSbuilder">要追加内容的StringBuilder，如果为null则新建一个。</param>
        /// <returns></returns>
        private static StringBuilder PrintItem(string pItem, int pSpace, StringBuilder pSbuilder = null)
        {
            pSbuilder ??= new();
            pSpace = Mathf.Max(pSpace, 8);
            //[
            pSbuilder.Append('[');
            //前空格
            pSbuilder.Append(" ");
            //item
            pSbuilder.Append(pItem);
            //后空格
            int limit = pSpace - 3 - pItem.Length;
            for (int i = 0; i < limit; i++)
                pSbuilder.Append(" ");
            //]
            pSbuilder.Append(']');
            //制表符
            pSbuilder.Append('\t');
            return pSbuilder;    //[ item ]
        }

        /// <summary>
        /// 将指定的字符串输出到文本文件中
        /// </summary>
        /// <param name="pLine">要输出的字符串。</param>
        private static void WriteLog(string pLogModule, StringBuilder pLine)
        {
            lock (logLock)
            {
                if (!swDict.ContainsKey(pLogModule))
                    return;
                swDict[pLogModule]?.WriteLine(pLine); 
            }
        }

        private static void ApplicationQuit()
        {
            foreach (var sw in swDict.Values) 
            {
                sw?.Close();
            }
            swDict.Clear();
        }

        private static void CreateStreamWriter(string pLogModule)
        {
            lock (logLock)
            {
                if (swDict.ContainsKey(pLogModule))
                {
                    swDict[pLogModule]?.Close();
                    swDict.Remove(pLogModule);
                }

                if (!Directory.Exists(LogSaveRootPath))
                    Directory.CreateDirectory(LogSaveRootPath);

                string currLogSavePath = Path.Combine(LogSaveRootPath, logfileTime);
                if (!Directory.Exists(currLogSavePath))
                    Directory.CreateDirectory(currLogSavePath);

                string logFilePath = Path.Combine(currLogSavePath, GetLogFileName(pLogModule));
                StreamWriter streamWriter = File.AppendText(logFilePath);
                streamWriter.AutoFlush = true;
                swDict.Add(pLogModule, streamWriter); 
            }
        }

        private static string GetLogFileName(string pLogModule)
        {
            return $"{pLogModule}_{logfileTime}.log";
        }

        public static void InitLogLogModule(string pLogModule)
        {
            if (string.IsNullOrEmpty(pLogModule))
            {
                return;
            }
            CreateStreamWriter(pLogModule);
        }

        public static void SetCurrLogModule(string pLogModule)
        {
            lock (currLogModuleLock)
            {
                currLogModule = pLogModule;
            }
        }
    }

    public class LogServer : BaseServer
    { 
        /// <summary>
        /// 日志标签
        /// </summary>
        public virtual string LogTag {  get; }

        /// <summary>
        /// 开启日志
        /// </summary>
        public bool OpenLog { get; set; }

        /// <summary>
        /// 开启警告
        /// </summary>
        public bool OpenWarn { get; set; }

        private StringBuilder stringBuilder = new StringBuilder();

        public LogServer()
        {
            OpenLog = true;
            OpenWarn = true;

            LogUtility.InitLogLogModule(LogTag);
        }

        private void PrintLog(LogType pLogType, string pLog, params object[] pArgs)
        {
            if (pLogType == LogType.Log && !OpenLog)
                return;

            if (pLogType == LogType.LogWarn && !OpenWarn)
                return;

            stringBuilder.Length = 0;

#if UNITY_EDITOR
            stringBuilder.Append(LogTag);
            switch (pLogType)
            {
                case LogType.Log:
                    stringBuilder.Append($"[LOG][I]:");
                    break;
                case LogType.LogWarn:
                    stringBuilder.Append($"[LOG][W]:");
                    break;
                case LogType.LogError:
                    stringBuilder.Append($"[LOG][E]:");
                    break;
            } 
#endif

            if (pArgs != null && pArgs.Length > 0)
            {
                stringBuilder.Append(pLog);
                for (int i = 0; i < pArgs.Length; i++)
                {
                    stringBuilder.Append(" "+ pArgs[i]);
                }
            }
            else
                stringBuilder.Append(pLog);

            LogUtility.SetCurrLogModule(LogTag);
            string message = stringBuilder.ToString();
            switch (pLogType)
            {
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.LogWarn:
                    Debug.LogWarning(message);
                    break;
                case LogType.LogError:
                    Debug.LogError(message);
                    break;
            }
        }

        public virtual void Log(string log, params object[] args)
        {
            PrintLog(LogType.Log, log, args);
        }

        public virtual void LogError(string log, params object[] args)
        {
            PrintLog(LogType.LogWarn, log, args);
        }
        
        /// <summary>
        /// 在编辑器下错误日志
        /// </summary>
        /// <param name="log"></param>
        /// <param name="args"></param>
        public virtual void LogR(string log, params object[] args)
        {
#if UNITY_EDITOR
            PrintLog(LogType.LogError, log, args);
#endif
        }

        public virtual void LogWarning(string log, params object[] args)
        {
            PrintLog(LogType.LogWarn, log, args);
        }
    }
}