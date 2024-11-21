using IAFramework.Server.Procedure;
using IAToolkit;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace IAFramework.Patch
{
    internal class PatchOperation : GameAsyncOperation
    {
        private enum ESteps
        {
            None,
            Update,
            Done,
        }

        public EPlayMode PlayMode { private set; get; }
        public string PackageName { private set; get; }
        public string BuildPipeline { private set; get; }

        //游戏开始流程
        private Fsm<PatchOperation> procedure;
        private ESteps steps = ESteps.None;

        public PatchOperation(string pPackageName, string pBuildPipeline, EPlayMode pPlayMode)
        {
            PlayMode = pPlayMode;   
            PackageName = pPackageName;
            BuildPipeline = pBuildPipeline;

            procedure = Fsm<PatchOperation>.Create(this, new List<FsmState<PatchOperation>>()
            {
                new Patch_InitializePackage(),
                new Patch_UpdatePackageVersion(),
                new Patch_UpdatePackageManifest(),
                new Patch_CreatePackageDownloader(),
                new Patch_DownloadPackageFiles(),
                new Patch_DownloadPackageOver(),
                new Patch_ClearPackageCache(),
                new Patch_UpdaterDone(),
            });
        }

        protected override void OnAbort()
        {
            
        }

        protected override void OnStart()
        {
            steps = ESteps.Update;
            procedure?.Start(typeof(Patch_InitializePackage));
        }

        protected override void OnUpdate()
        {
            if (steps == ESteps.None || steps == ESteps.Done)
                return;

            if (steps == ESteps.Update)
            {
                procedure.Update(Time.deltaTime, Time.realtimeSinceStartup);
                if (procedure.CurrentStateName == typeof(Patch_UpdaterDone).FullName)
                {
                    steps = ESteps.Done;
                    //操作完成
                    Status = EOperationStatus.Succeed;
                }
            }
        }
    }
}
