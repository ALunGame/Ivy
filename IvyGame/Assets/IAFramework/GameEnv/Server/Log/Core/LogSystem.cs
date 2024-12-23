using Cysharp.Text;
using IAToolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

public enum LogModule
{
    /// <summary>
    /// 编辑器下日志
    /// </summary>
    Editor,

    Default,

    UI,

    Client,

    Server,
}

public enum LogLevel
{
    Log = 1,
    Warning = 2,
    Error = 3,
}

namespace IAFramework.Log
{
    public static class LogSetting
    {
        /// <summary>
        /// 记录几条日志
        /// </summary>
        public static int SaveLogCnt = 3;
        public static string LogRootPath = Path.Combine(PathHelper.SandboxDir, "Log");

        private static Dictionary<LogModule, string> LogModulePathDict = new Dictionary<LogModule, string>();

        public static string GetLogModuleFilePath(LogModule pLogModule, string pDirName)
        {
            if (LogModulePathDict.ContainsKey(pLogModule))
            {
                return LogModulePathDict[pLogModule];
            }

            string logDir = Path.Combine(LogRootPath, pDirName);
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            string fileName = $"{pLogModule}_{pDirName}.txt";
            string path = Path.Combine(logDir, fileName);
            LogModulePathDict.Add(pLogModule, path);

            Debug.Log(path);
            return path;
        }

        public static void CheckDelLog()
        {
            string[] dirs = Directory.GetDirectories(LogRootPath);
            if (dirs == null || dirs.Length < SaveLogCnt)
            {
                return;
            }

            List<DirectoryInfo> dirInfos = new List<DirectoryInfo>();
            for (int i = 0; i < dirs.Length; i++)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dirs[i]);
                dirInfos.Add(dirInfo);
            }

            dirInfos.Sort((x, y) =>
            {
                return y.CreationTime.CompareTo(x.CreationTime);
            });

            for (int i = SaveLogCnt; i < dirInfos.Count; i++)
            {
                Directory.Delete(dirInfos[i].FullName, true);
            }
        }
    }


    public class LogSystem
    {
        #region 日志索引

        private int logIndex = 0;
        /// <summary>
        /// 当前日志索引
        /// </summary>
        public int LogIndex { get => logIndex; }

        /// <summary>
        /// 自增日志索引
        /// </summary>
        public void AddLogIndex()
        {
            Interlocked.Increment(ref logIndex);
        }

        #endregion

        private string logDirName = "";
        private Dictionary<LogModule, LogIO> logIODict = new Dictionary<LogModule, LogIO>();

        public LogSystem()
        {
            logDirName = $"{DateTime.Now:yyyy-MM-dd(HH.mm.ss)}";
            LogSetting.CheckDelLog();
        }

        public void Clear()
        {
            foreach (var item in logIODict.Values)
            {
                item.Dispose();
            }
            logIODict.Clear();
        }

        public void AddLogIO(LogModule pLogModule)
        {
            if (logIODict.ContainsKey(pLogModule))
            {
                return;
            }

            logIODict.Add(pLogModule, new LogIO(LogSetting.GetLogModuleFilePath(pLogModule, logDirName), this));
        }

        /// <summary>
        /// 日志（真机不输出）
        /// </summary>
        /// <param name="pLogModule"></param>
        /// <param name="pMsgs"></param>
        public void Log(LogModule pLogModule, params object[] pMsgs)
        {
#if UNITY_EDITOR
            Debug.Log(ZString.Concat(pMsgs));
#endif
        }

        /// <summary>
        /// 日志真机输出
        /// </summary>
        /// <param name="pLogModule"></param>
        /// <param name="pMsgs"></param>
        public void RuntimeLog(LogModule pLogModule, params object[] pMsgs)
        {
            string msg = ZString.Concat(pMsgs); 
            Debug.Log(msg);

            if (logIODict.ContainsKey(pLogModule))
            {
                logIODict[pLogModule].WriterLog(pLogModule, LogLevel.Log, msg);
            }
        }

        /// <summary>
        /// 警告（真机不输出）
        /// </summary>
        /// <param name="pLogModule"></param>
        /// <param name="pMsgs"></param>
        public void LogWarning(LogModule pLogModule, params object[] pMsgs)
        {
#if UNITY_EDITOR
            Debug.LogWarning(ZString.Concat(pMsgs));
#endif
        }

        /// <summary>
        /// 警告真机输出
        /// </summary>
        /// <param name="pLogModule"></param>
        /// <param name="pMsgs"></param>
        public void RuntimeWarning(LogModule pLogModule, params object[] pMsgs)
        {
            string msg = ZString.Concat(pMsgs);
            Debug.LogWarning(msg);

            if (logIODict.ContainsKey(pLogModule))
            {
                logIODict[pLogModule].WriterLog(pLogModule, LogLevel.Warning, msg);
            }
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="pLogModule"></param>
        /// <param name="pMsgs"></param>
        public void LogError(LogModule pLogModule, params object[] pMsgs)
        {
            string msg = ZString.Concat(pMsgs);
            Debug.LogError(msg);

            if (logIODict.ContainsKey(pLogModule))
            {
                logIODict[pLogModule].WriterLogWithStrack(pLogModule, LogLevel.Error, msg);
            }
        }
    }
}
