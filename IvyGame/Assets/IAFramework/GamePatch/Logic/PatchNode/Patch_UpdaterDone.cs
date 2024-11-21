using IAToolkit;

namespace IAFramework.Patch
{
    /// <summary>
    /// Patch流程完成
    /// </summary>
    internal class Patch_UpdaterDone : FsmState<PatchOperation>
    {
        protected override void OnEnter()
        {
            GamePatchData.Instance.SendProcessTips($"加载->Package:{Owner.PackageName} Buildline:{Owner.BuildPipeline}完成");
        }
    }
}
