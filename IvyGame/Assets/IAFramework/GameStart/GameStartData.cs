using IAUI;
using System.Collections.Generic;

namespace IAFramework
{
    public enum EGameStartTipType
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

    public enum EGameStartState
    {
        Success,
        Fail,
    }

    public class GameStartData : BaseUserData<GameStartData>
    {
        public EGameStartTipType TipType { get; set; }

        public string PopTitleText = "";

        public string TipText = "";

        public List<object> ExParams = new List<object>();

        public EGameStartState CurrState { get; private set; }

        public void SendProcessTips(string tipStr, params object[] exParams)
        {
            TipType = EGameStartTipType.ProcessTips;
            TipText = tipStr;
            ExParams.Clear();
            ExParams.AddRange(exParams);

            UpdateUserData();
        }

        public void SendPopTips(string popTitleText, string tipStr, params object[] exParams)
        {
            TipType = EGameStartTipType.PopTips;
            PopTitleText = popTitleText;
            PopTitleText = tipStr;
            ExParams.Clear();
            ExParams.AddRange(exParams);

            UpdateUserData();
        }

        public void SetGameStartState(EGameStartState currState)
        {
            CurrState = currState;
        }
    }
}
