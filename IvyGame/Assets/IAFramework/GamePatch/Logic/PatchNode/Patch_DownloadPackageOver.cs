using IAFramework.Server.Procedure;
using IAToolkit;

namespace IAFramework.Patch
{
    /// <summary>
    /// 下载完毕
    /// </summary>
    internal class Patch_DownloadPackageOver : FsmState<PatchOperation>
    {
        protected override void OnEnter()
        {
            GamePatchData.Instance.SendProcessTips($"下载{Owner.PackageName}补丁文件完成...");
            Fsm.ChangeState(typeof(Patch_ClearPackageCache));
        }
    }
}
