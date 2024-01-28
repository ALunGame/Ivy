using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Network.Server
{
    internal class ServerGameSystem
    {
        protected bool IsInit { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            if (IsInit)
                return;
            IsInit = true;

            OnInit();
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            OnStartGame();
        }

        /// <summary>
        /// 开始游戏之后
        /// </summary>
        public void AfterStartGame()
        {
            OnAfterStartGame();
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="pDeltaTime">时间间隔</param>
        /// <param name="pRealElapseSeconds">已经运行时间</param>
        public void Update(float pDeltaTime, float pRealElapseSeconds)
        {
            if (!IsInit)
                return;

            OnUpdate(pDeltaTime, pRealElapseSeconds);
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        public void EndGame()
        {
            OnEndGame();
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            if (!IsInit)
                return;

            IsInit = false;
            OnClear();
        }

        #region Virtual

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void OnInit() { }

        /// <summary>
        /// 开始游戏
        /// </summary>
        protected virtual void OnStartGame() { }

        /// <summary>
        /// 开始游戏之后
        /// </summary>
        protected virtual void OnAfterStartGame() { }

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="pDeltaTime">时间间隔</param>
        /// <param name="pRealElapseSeconds">已经运行时间</param>
        protected virtual void OnUpdate(float pDeltaTime, float pRealElapseSeconds) { }

        /// <summary>
        /// 结束游戏
        /// </summary>
        protected virtual void OnEndGame() { }

        /// <summary>
        /// 清理
        /// </summary>
        protected virtual void OnClear() { }

        #endregion
    }
}
