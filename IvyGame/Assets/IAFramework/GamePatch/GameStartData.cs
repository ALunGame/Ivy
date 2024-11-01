using IAUI;
using System.Collections.Generic;

namespace IAFramework
{
    public enum EGamePatchTipType
    {
        /// <summary>
        /// 进度提示
        /// </summary>
        ProcessTips = 0,

        /// <summary>
        /// 弹窗提示
        /// </summary>
        PopTips,
    }

    public enum EGamePatchState
    {
        Wait,
        Success,
        Fail,

        GameStartSuccess,       //游戏业务层开始成功
    }

    public class GamePatchData : BaseUserData<GamePatchData>
    {
        public EGamePatchTipType TipType { get; set; }

        public string PopTitleText = "";

        public string TipText = "";

        public List<object> ExParams = new List<object>();

        public EGamePatchState CurrState { get; private set; }

        protected override void OnInit()
        {
            CurrState = EGamePatchState.Wait;
        }

        public void SendProcessTips(string tipStr, params object[] exParams)
        {
            TipType = EGamePatchTipType.ProcessTips;
            TipText = tipStr;
            ExParams.Clear();
            ExParams.AddRange(exParams);

            UpdateUserData();
        }

        public void SendPopTips(string popTitleText, string tipStr, params object[] exParams)
        {
            TipType = EGamePatchTipType.PopTips;
            PopTitleText = popTitleText;
            PopTitleText = tipStr;
            ExParams.Clear();
            ExParams.AddRange(exParams);

            UpdateUserData();
        }

        public void SetGamePatchState(EGamePatchState currState)
        {
            CurrState = currState;

            UpdateUserData();
        }
    }
}
