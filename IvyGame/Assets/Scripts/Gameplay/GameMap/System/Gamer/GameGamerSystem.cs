using Gameplay.GameData;
using Gameplay.GameMap.Actor;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameMap.System
{
    public class GameGamerSystem : BaseGameMapSystem
    {
        public override void OnEnterMap()
        {
            CreateGamers();
        }

        public override void OnStartGame()
        {
            List<GamerData> gamers = GameplayCtrl.Instance.GameData.Gamers.Gamers;
            GameActorSystem gameActorSystem = GameplayCtrl.Instance.GameMap.GetSystem<GameActorSystem>();
            foreach (GamerData gamer in gamers)
            {
                Actor_InternalGamer actor = gameActorSystem.GetActor(gamer.GamerUid) as Actor_InternalGamer;
                actor.SetGamerData(gamer);
                actor.SetPos(gamer.Position);
                actor.SetRotation(Quaternion.Euler(new Vector3(0, gamer.Rotation, 0)));
                actor.SetSpeed(gamer.MoveSpeed);
            }
        }

        private void CreateGamers()
        {
            List<GamerData> gamers = GameplayCtrl.Instance.GameData.Gamers.Gamers;

            GameActorSystem gameActorSystem = GameplayCtrl.Instance.GameMap.GetSystem<GameActorSystem>();
            foreach (GamerData gamer in gamers)
            {
                if (gamer.GamerUid == GameplayGlobal.Data.SelfGamerUid)
                {
                    gameActorSystem.CreateActor(gamer.GamerUid, gamer.GamerId, ActorType.LocalPlayer);
                }
                else
                {
                    gameActorSystem.CreateActor(gamer.GamerUid, gamer.GamerId, ActorType.RemotePlayer);
                }
            }
        }
    }
}
