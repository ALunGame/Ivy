using System;
using System.Collections;
using System.Collections.Generic;
using IAFramework.Server;
using IAToolkit;
using UnityEngine;
using YooAsset;

namespace IAFramework
{
    public class GameStart : MonoBehaviour
    {
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

        private GameStartServer gameStartServer = new GameStartServer();
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
            
            GameContext.Init();

            gameStartServer.OnStartSuccess += OnStartSuccess;
            gameStartServer.OnStartFail += OnStartFail;
            gameStartServer.Init();
        }

        private void Start()
        {
            //初始化资源系统
            YooAssets.Initialize();
            YooAssets.SetOperationSystemMaxTimeSlice(30);
            
            //开始游戏流程
            gameStartServer.Start(PlayMode);
        }
        
        private void OnApplicationQuit()
        {
            gameStartServer.Clear();
            YooAssets.Destroy();
            GameContext.Clear();
        }

        private void OnStartSuccess()
        {
            Debug.Log("游戏开始------------>");
        }
        
        private void OnStartFail(string pReason)
        {
            
        }
    }
}
