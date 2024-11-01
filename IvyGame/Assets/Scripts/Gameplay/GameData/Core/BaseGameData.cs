using System.Collections.Generic;

namespace Gameplay.GameData
{
    public class BaseGameData
    {
        private List<InternalGameDataField> gameDataFiles = new List<InternalGameDataField>();

        public void Init()
        {
            OnInit();
        }

        public virtual void OnInit()
        {

        }

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

        internal void AddGameDataField(InternalGameDataField pFile)
        {
            gameDataFiles.Add(pFile);
        }
    }
}
