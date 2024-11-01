using Gameplay.GameData;
using System.Collections.Generic;

namespace Game.Network.Server
{
    internal class BaseServerGameData
    {
        private List<InternalServerGameDataFile> gameDataFiles = new List<InternalServerGameDataFile>();

        public virtual void UpdateLogic(float pTimeDelta, float pGameTime)
        {
        }

        public void Clear()
        {
            foreach (var field in gameDataFiles)
            {
                field.Clear();
            }
            gameDataFiles.Clear();

            OnClear();
        }

        public virtual void OnClear()
        {

        }

        internal void AddGameDataField(InternalServerGameDataFile pFile)
        {
            gameDataFiles.Add(pFile);
        }
    }
}
