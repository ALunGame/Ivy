using Cysharp.Text;

namespace IAFramework.Log
{
    public static class LogBuilder
    {
        private static Utf8ValueStringBuilder logBuilder = ZString.CreateUtf8StringBuilder();
        private static Utf8ValueStringBuilder strackLogBuilder = ZString.CreateUtf8StringBuilder();

        /// <summary>
        /// 构建日志
        /// </summary>
        /// <param name="pMsg"></param>
        /// <returns></returns>
        public static Utf8ValueStringBuilder BuildLogMsg(string pMsg)
        {
            logBuilder.Clear();
            logBuilder.AppendLine(pMsg);
            return logBuilder;
        }

        /// <summary>
        /// 构建带有堆栈信息的日志
        /// </summary>
        /// <param name="pMsg"></param>
        /// <param name="pMsgLineCount"></param>
        /// <param name="pSkipLine"></param>
        /// <returns></returns>
        public static Utf8ValueStringBuilder BuildLogMsgWithStrack(string pMsg)
        {
            strackLogBuilder.Clear();
            strackLogBuilder.AppendLine(pMsg);

            var stack = new System.Diagnostics.StackTrace(1, false);

            for (int i = 0; i < stack.FrameCount; i++)
            {
                var stackFrame = stack.GetFrame(i);

                //非用户代码,系统方法及后面的都是系统调用，不获取用户代码调用结束
                if (System.Diagnostics.StackFrame.OFFSET_UNKNOWN == stackFrame.GetILOffset()) 
                    break;
                var methd = stackFrame.GetMethod();

                strackLogBuilder.Append(methd.DeclaringType.FullName);
                strackLogBuilder.Append(".");
                strackLogBuilder.Append(methd.Name);

                //最后
                if (i > stack.FrameCount)
                {
                    break;
                }
                else
                {
                    strackLogBuilder.AppendLine();
                }
            }
            return strackLogBuilder;
        }
    }
}
