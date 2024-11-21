using Cysharp.Threading.Tasks;
using GameContext;
using IAEngine;
using IAFramework;
using IAUI;
using UnityEngine;

namespace Game
{
    public class GameStart : MonoBehaviour
    {
        private void Start()
        {
            CachePool.Init();
            GamePatchData.Instance.RegUserDataChange(OnPatchDataChange);
        }

        private void OnDestroy()
        {
            CachePool.Clear();
            GamePatchData.Instance.RemoveUserDataChange(OnPatchDataChange);
        }

        private void OnPatchDataChange()
        {
            if (GamePatchData.Instance.CurrState == EGamePatchState.Success)
            {
                StartGame().Forget();
            }
        }

        public async UniTaskVoid StartGame()
        {
            //游戏存档初始化
            GameContextLocate.Init();

            //UI初始化
            UICenter uICenter = GameObject.Find("Game/UICenter").GetComponent<UICenter>();
            uICenter.Init();
            await UniTask.Yield(PlayerLoopTiming.Update);

            //显示起始UI
            UILocate.UI.Show(UIPanelDef.MainGamePanel);
            await UniTask.Yield(PlayerLoopTiming.Update);

            GamePatchData.Instance.SetGamePatchState(EGamePatchState.GameStartSuccess);
        }
    } 
}
