using System;
using System.Diagnostics;

namespace Game.Network
{
    /// <summary>
    /// 用于网路时间更新
    /// </summary>
    internal class NetworkLogicTimer
    {   
        /// <summary>
        /// 每秒多少帧
        /// </summary>
        public const float FramesPerSecond = 30.0f;

        /// <summary>
        /// 帧时间间隔
        /// </summary>
        public const float FixedDelta = 1.0f / FramesPerSecond;

        private readonly Stopwatch stopwatch;
        private readonly Action callBack;

        //上一次时间
        private long lastTime;

        //记录距离上一帧经过多长时间
        private double accumulator;

        /// <summary>
        /// 距离下一帧的比例
        /// </summary>
        public float LerpFrame => (float)accumulator / FixedDelta;

        public NetworkLogicTimer(Action pCallBack)
        {
            stopwatch = new Stopwatch();
            callBack = pCallBack;
        }

        public void Start()
        {
            lastTime = 0;
            accumulator = 0.0;
            stopwatch.Restart();
        }

        public void Stop()
        {
            stopwatch.Stop();
        }

        public void Update()
        {
            //总运行时间
            long elapsedTicks = stopwatch.ElapsedTicks;

            accumulator += (double)(elapsedTicks - lastTime) / Stopwatch.Frequency;
            lastTime = elapsedTicks;

            while (accumulator >= FixedDelta)
            {
                callBack();
                accumulator -= FixedDelta;
            }
        }

    }
}
