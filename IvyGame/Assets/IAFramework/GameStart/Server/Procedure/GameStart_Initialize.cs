using System;
using System.IO;
using Cysharp.Threading.Tasks;
using IAToolkit;
using UnityEngine;
using YooAsset;
using IAFramework.StreamingAsset;

namespace IAFramework.Server.Procedure
{
    public class GameStart_Initialize : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            InitPackage().Forget();
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }
        
        private async UniTaskVoid InitPackage()
		{
			var playMode = Owner.AssetMode;
			
			// 创建默认的资源包
			string packageName = "DefaultPackage";
			var package = YooAssets.TryGetPackage(packageName);
			if (package == null)
			{
				package = YooAssets.CreatePackage(packageName);
				YooAssets.SetDefaultPackage(package);
			}
			
			// 编辑器下的模拟模式
			InitializationOperation initializationOperation = null;
			if (playMode == EPlayMode.EditorSimulateMode)
			{
				var createParameters = new EditorSimulateModeParameters();
				createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(packageName);
				initializationOperation = package.InitializeAsync(createParameters);
			}
			
			// 单机运行模式
			if (playMode == EPlayMode.OfflinePlayMode)
			{
				var createParameters = new OfflinePlayModeParameters();
				initializationOperation = package.InitializeAsync(createParameters);
			}
			
			// 联机运行模式
			if (playMode == EPlayMode.HostPlayMode)
			{
				string defaultHostServer = GetHostServerURL();
				string fallbackHostServer = GetHostServerURL();
				var createParameters = new HostPlayModeParameters();
				createParameters.BuildinQueryServices = new GameQueryServices();
				createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
				initializationOperation = package.InitializeAsync(createParameters);
			}
			
			await initializationOperation;
			
			if (initializationOperation.Status == EOperationStatus.Succeed)
			{
				Fsm.ChangeState(typeof(GameStart_UpdateVersion));
			}
			else
			{
				Debug.LogError($"{initializationOperation.Error}");
			}
		}

		/// <summary>
		/// 获取资源服务器地址
		/// </summary>
		/// <summary>
		/// 获取资源服务器地址
		/// </summary>
		private string GetHostServerURL()
		{
			//string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
			string hostServerIP = "http://127.0.0.1";
			string appVersion = "v1.0";

#if UNITY_EDITOR
			if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
				return $"{hostServerIP}/CDN/Android/{appVersion}";
			else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
				return $"{hostServerIP}/CDN/IPhone/{appVersion}";
			else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
				return $"{hostServerIP}/CDN/WebGL/{appVersion}";
			else
				return $"{hostServerIP}/CDN/PC/{appVersion}";
#else
			if (Application.platform == RuntimePlatform.Android)
				return $"{hostServerIP}/CDN/Android/{appVersion}";
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
				return $"{hostServerIP}/CDN/IPhone/{appVersion}";
			else if (Application.platform == RuntimePlatform.WebGLPlayer)
				return $"{hostServerIP}/CDN/WebGL/{appVersion}";
			else
				return $"{hostServerIP}/CDN/PC/{appVersion}";
#endif
		}
        
        
        /// <summary>
        /// 远端资源地址查询服务类
        /// </summary>
        private class RemoteServices : IRemoteServices
        {
            private readonly string _defaultHostServer;
            private readonly string _fallbackHostServer;
 
            public RemoteServices(string defaultHostServer, string fallbackHostServer)
            {
                _defaultHostServer = defaultHostServer;
                _fallbackHostServer = fallbackHostServer;
            }
            string IRemoteServices.GetRemoteMainURL(string fileName)
            {
                return $"{_defaultHostServer}/{fileName}";
            }
            string IRemoteServices.GetRemoteFallbackURL(string fileName)
            {
                return $"{_fallbackHostServer}/{fileName}";
            }
        }
    }
}