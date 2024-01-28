using System.Collections.Generic;

namespace Game.Network.Server
{
    internal class ServerGameRuleSystem : ServerGameSystem
    {
        private Dictionary<byte, ServerRect> rectCampDict = new Dictionary<byte, ServerRect>();

        protected override void OnStartGame()
        {
            if (NetServerLocate.Game.GameMode == Gameplay.GameModeType.Single)
            {
                InSingleMode();
            }
            else if (NetServerLocate.Game.GameMode == Gameplay.GameModeType.Team)
            {
                InTeamMode();
            }
        }

        protected override void OnAfterStartGame()
        {
            foreach (byte camp in rectCampDict.Keys)
            {
                NetServerLocate.Game.Room.Map.ChangRectCamp(rectCampDict[camp], camp);
            }
            rectCampDict.Clear();
        }

        private ServerRect CreateRectCamp(byte width, byte height, byte camp)
        {
            ServerRect rect = NetServerLocate.Game.Room.Map.CreateCampRect(width, height);
            if (rect != null)
            {
                rectCampDict.Add(camp, rect);
            }
            return rect;
        }

        private void InSingleMode()
        {
            List<ServerPlayer> players = NetServerLocate.Game.Room.GetPlayers();
            for (int i = 0; i < players.Count; i++)
            {
                byte camp = (byte)(i + 1);
                ServerPlayer player = players[i];
                player.SetCamp(camp);

                ServerRect rect = CreateRectCamp(5, 5, camp);
                ServerPoint pos = rect.TakeRandomPoint();
                player.SetGridPos(pos.x, pos.y);
                player.SetPos(pos.x, pos.y);
            }
        }


        private void InTeamMode()
        {

        }
    }
}
