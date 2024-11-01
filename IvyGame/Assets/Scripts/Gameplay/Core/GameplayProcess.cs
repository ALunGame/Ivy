﻿namespace Gameplay
{
    /// <summary>
    /// 游戏玩法流程
    /// </summary>
    public class GameplayProcess
    {
        protected GameplayCtrl Ctrl {  get; private set; }

        public void Init(GameplayCtrl pCtrl)
        {
            Ctrl = pCtrl;
            OnInit();
        }


        public void UpdateLogic(float pDeltaTime, float pGameTime)
        {
            OnUpdateLogic(pDeltaTime, pGameTime);
        }

        public void Clear()
        {
            OnClear();
        }

        /// <summary>
        /// 流程初始化
        /// </summary>
        public virtual void OnInit() { }

        /// <summary>
        /// 更新逻辑
        /// </summary>
        /// <param name="pDeltaTime">间隔时间</param>
        /// <param name="pGameTime">游戏运行时间</param>
        public virtual void OnUpdateLogic(float pDeltaTime, float pGameTime) { }

        /// <summary>
        /// 流程数据清理
        /// </summary>
        public virtual void OnClear() { }

        /// <summary>
        /// 游戏创建时
        /// </summary>
        public virtual void OnCreateGame(GameModeType pModeType, bool pNeedCreateRoom) { }

        /// <summary>
        /// 进入地图时
        /// </summary>
        public virtual void OnEnterMap(int pMapId) { }

        /// <summary>
        /// 开始游戏时
        /// </summary>
        public virtual void OnStartGame(int pGameLevelId) { }

        /// <summary>
        /// 游戏结束时
        /// </summary>
        public virtual void OnEndGame() { }

        /// <summary>
        /// 游戏暂停时
        /// </summary>
        public virtual void OnPauseGame() { }

        /// <summary>
        /// 游戏恢复时
        /// </summary>
        public virtual void OnReuseGame() { }

        /// <summary>
        /// 离开游戏时
        /// </summary>
        public virtual void OnExitGame() { }
    }
}