using Gameplay.System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// 游戏实例
    /// 1，单例，只能存在一个游戏实例
    /// 2，包含对GameMode的引用
    /// 3，Gameplay的生命周期
    /// </summary>
    internal class GameInstance : MonoBehaviour
    {
        /// <summary>
        /// 游戏模式
        /// </summary>
        public GameMode Mode {  get; private set; }

        /// <summary>
        /// 是否是房主
        /// </summary>
        public bool IsRoomOwner { get; private set; }

        /// <summary>
        /// 帧已经运行秒数
        /// </summary>
        public float UpdateRealElapseSeconds { get; private set; }
        /// <summary>
        /// 每帧间隔时间
        /// </summary>
        public float UpdateDeltaTime { get; private set; }
        /// <summary>
        /// 每帧时间缩放
        /// </summary>
        public float UpdateTimeScale { get; private set; }

        /// <summary>
        /// 固定频率已经运行秒数
        /// </summary>
        public float FixedUpdateRealElapseSeconds { get; private set; }
        /// <summary>
        /// 固定频率间隔时间
        /// </summary>
        public float FixedUpdateDeltaTime { get; private set; }
        /// <summary>
        /// 固定频率时间缩放
        /// </summary>
        public float FixedUpdateTimeScale { get; private set; }

        //玩法系统
        private List<GameplaySystem> systems = new List<GameplaySystem>() 
        {
            new NetworkSystem(),
        };

        #region Unity

        private void Awake()
        {
            Mode = new GameMode();

            UpdateRealElapseSeconds = 0;
            UpdateDeltaTime = Time.deltaTime;
            UpdateTimeScale = Time.timeScale;

            FixedUpdateRealElapseSeconds = 0;
            FixedUpdateDeltaTime = Time.fixedDeltaTime;
            FixedUpdateTimeScale = Time.timeScale;

            GameplayLocate.SetGameInstance(this);

            foreach (GameplaySystem system in systems)
                system.Init();
        }

        private void Update()
        {
            UpdateRealElapseSeconds += UpdateDeltaTime * UpdateTimeScale;
            foreach (GameplaySystem system in systems)
                system.Update(UpdateDeltaTime * UpdateTimeScale, UpdateRealElapseSeconds);
        }

        private void FixedUpdate()
        {
            FixedUpdateRealElapseSeconds += FixedUpdateDeltaTime * FixedUpdateTimeScale;
            foreach (GameplaySystem system in systems)
                system.FixedUpdate(FixedUpdateDeltaTime * FixedUpdateTimeScale, UpdateRealElapseSeconds);
        }

        private void OnDestroy()
        {
            foreach (GameplaySystem system in systems)
                system.Clear();
        } 

        #endregion

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            foreach (GameplaySystem system in systems)
                system.StartGame();
        }

        /// <summary>
        /// 结束游戏
        /// </summary>
        public void EndGame()
        {
            foreach (GameplaySystem system in systems)
                system.EndGame();
        }
    }
}
