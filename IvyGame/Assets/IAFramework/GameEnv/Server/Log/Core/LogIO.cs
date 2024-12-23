using Cysharp.Text;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IAFramework.Log
{
    /// <summary>
    /// 日志读写
    /// </summary>
    public class LogIO
    {
        struct LogInfo
        {
            const string LogFormatStr = "[{0}][{1}][{2}][{3}]{4}";

            public int index;
            public LogModule module;
            public LogLevel level;
            public DateTime time;
            public Utf8ValueStringBuilder msg;

            public byte[] GetLogBytes()
            {
                string logStr = ZString.Format(LogFormatStr, index, module, level, time, msg);
                return Encoding.UTF8.GetBytes(logStr);
            }
        }

        public string LogFileName { get; private set; }

        private LogSystem logSystem;

        private FileStream writeFileStream;
        private FileStream WriteFileStream
        {
            get
            {
                if (this.writeFileStream == null)
                {
                    string logDirPath = Path.GetDirectoryName(LogFileName);
                    if (!Directory.Exists(logDirPath))
                        Directory.CreateDirectory(logDirPath);
                    this.writeFileStream = new FileStream(LogFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                }
                return writeFileStream;
            }
        }

        //等待写入的日志信息
        private ConcurrentQueue<LogInfo> writeLogQueue = new ConcurrentQueue<LogInfo>();
        private Task writeTask;

        public LogIO(string pLogFilePath, LogSystem pLogSystem)
        {
            this.logSystem = pLogSystem;
            this.LogFileName = pLogFilePath;
        }

        public void Dispose()
        {
            this.writeFileStream?.Close();
        }

        public void WriterLog(LogModule pLogModule, LogLevel pLogLevel, string pMsg)
        {
            logSystem.AddLogIndex();

            LogInfo logInfo = new LogInfo();
            logInfo.index = logSystem.LogIndex;
            logInfo.module = pLogModule;
            logInfo.level = pLogLevel;
            logInfo.time = DateTime.Now;
            logInfo.msg = LogBuilder.BuildLogMsg(pMsg);

            writeLogQueue.Enqueue(logInfo);

            WriteLogInfo_Task();
        }

        public void WriterLogWithStrack(LogModule pLogModule, LogLevel pLogLevel, string pMsg)
        {
            logSystem.AddLogIndex();

            LogInfo logInfo = new LogInfo();
            logInfo.index = logSystem.LogIndex;
            logInfo.module = pLogModule;
            logInfo.level = pLogLevel;
            logInfo.time = DateTime.Now;
            logInfo.msg = LogBuilder.BuildLogMsgWithStrack(pMsg);

            writeLogQueue.Enqueue(logInfo);

            WriteLogInfo_Task();
        }

        private async void WriteLogInfo_Task()
        {
            if (writeTask != null)
                return;
            writeTask = Task.Run(() =>
            {
                while (writeLogQueue.Count > 0)
                {
                    if (writeLogQueue.TryDequeue(out LogInfo tInfo))
                    {
                        byte[] bytes = tInfo.GetLogBytes();
                        WriteFileStream.Write(bytes, 0, bytes.Length);
                    }
                }
            });
            await writeTask;
            writeTask.Dispose();
            writeTask = null;
            if (writeLogQueue.Count > 0)
            {
                WriteLogInfo_Task();
            }
        }
    }
}
