namespace Gameplay
{
    /// <summary>
    /// 游戏玩法系统
    /// 1，游戏实例，以及游戏规则的改变导致需要处理的逻辑
    /// </summary>
    internal class GameplaySystem
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
        /// 固定频率更新
        /// </summary>
        /// <param name="pDeltaTime">时间间隔</param>
        /// <param name="pRealElapseSeconds">已经运行时间</param>
        public void FixedUpdate(float pDeltaTime, float pRealElapseSeconds)
        {
            if (!IsInit)
                return;

            OnFixedUpdate(pDeltaTime, pRealElapseSeconds);
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
        /// 每帧更新
        /// </summary>
        /// <param name="pDeltaTime">时间间隔</param>
        /// <param name="pRealElapseSeconds">已经运行时间</param>
        protected virtual void OnUpdate(float pDeltaTime, float pRealElapseSeconds) { }

        /// <summary>
        /// 固定频率更新
        /// </summary>
        /// <param name="pDeltaTime">时间间隔</param>
        /// <param name="pRealElapseSeconds">已经运行时间</param>
        protected virtual void OnFixedUpdate(float pDeltaTime, float pRealElapseSeconds) { }

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
