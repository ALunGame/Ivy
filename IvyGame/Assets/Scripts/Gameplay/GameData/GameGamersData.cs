using Proto;
using System.Collections.Generic;

namespace Gameplay.GameData
{
    public class GameGamersData : BaseGameData
    {
        public List<GamerData> Gamers {  get; private set; }

        public GameGamersData() 
        { 
            Gamers = new List<GamerData>();
        }

        public override void UpdateLogic(float pTimeDelta, float pGameTime)
        {
            for (int i = 0; i < Gamers.Count; i++)
            {
                Gamers[i].UpdateLogic(pTimeDelta, pGameTime);
            }
        }

        public override void OnClear()
        {
            base.OnClear();

            for (int i = 0; i < Gamers.Count; i++)
            {
                Gamers[i].Clear();
            }
        }

        public GamerData GetGamer(string pGamerUid)
        {
            for (int i = 0; i < Gamers.Count; i++)
            {
                if (Gamers[i].GamerUid == pGamerUid)
                {
                    return Gamers[i];
                }
            }
            return null;
        }

        public void RemoveGamer(string pGamerUid)
        {
            for (int i = 0; i < Gamers.Count; i++)
            {
                if (Gamers[i].GamerUid == pGamerUid)
                {
                    Gamers[i].Clear();
                    Gamers.RemoveAt(i);
                }
            }
        }

        public void UpdateGamer(GamerInfo info)
        {
            RemoveGamer(info.Uid);

            GamerData gamer;
            if (info.Uid == GameplayGlobal.Data.SelfGamerUid) 
            {
                gamer = new LocalGamerData(info);
            }
            else
            {
                gamer = new RemoteGamerData(info);
            }
            Gamers.Add(gamer);
        }
    }
}
