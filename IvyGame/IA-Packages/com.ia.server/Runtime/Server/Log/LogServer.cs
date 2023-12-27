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
    
    public class LogServer : BaseServer
    {
        public virtual string LogTag {  get; }


        private StringBuilder stringBuilder = new StringBuilder();

        private void PrintLog(LogType logType, string log, params object[] args)
        {
            stringBuilder.Length = 0;

            stringBuilder.Append(LogTag);

            switch (logType)
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

            if (args != null && args.Length > 0)
            {
                stringBuilder.Append(log);
                for (int i = 0; i < args.Length; i++)
                {
                    stringBuilder.Append(" "+args[i]);
                }
            }
            else
                stringBuilder.Append(log);

            string message = stringBuilder.ToString();
            switch (logType)
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