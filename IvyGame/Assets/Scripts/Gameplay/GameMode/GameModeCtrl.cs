using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameplay.GameMode
{
    public class GameModeCtrl : GameplayProcess
    {
        /// <summary>
        /// 游戏模式
        /// </summary>
        public GameModeType ModeType {  get; private set; }

        /// <summary>
        /// 游戏模式数据
        /// </summary>
        public BaseGameMode Mode {  get; private set; }

        public override void OnCreateGame(GameModeType pModeType, bool pNeedCreateRoom)
        {
            ModeType = pModeType;

            if (pModeType == GameModeType.Local)
            {
                Mode = new LocalGameMode();
            }
            else if (pModeType == GameModeType.Single)
            {
                Mode = new SingleGameMode();
            }
            else if (pModeType == GameModeType.Team)
            {
                Mode = new TeamGameMode();
            }

            Mode.CreateRoom = pNeedCreateRoom;
            Mode.Init();
        }

        public override void OnStartGame(int pGameLevelId)
        {
            Mode.StartGame(pGameLevelId);
        }
    }
}
