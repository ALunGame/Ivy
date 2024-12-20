using Cysharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public enum LogModule
{
    Default,
}

public enum LogLevel
{
    Log = 1,
    Warning = 2,
    Error = 3,
    Exception = 4,
}

namespace IAFramework.Log
{
    public class LogSystem
    {
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
    }
}
