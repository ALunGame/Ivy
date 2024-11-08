using Game.Network;
using Gameplay.GameData;
using UnityEngine;

namespace Gameplay.GameMap.Actor
{
    public class Actor_LocalGamer : Actor_Gamer<LocalGamerData>
    {
        public Actor_LocalGamer(string pUid, int pId, ActorType pType, GameObject pActorGo) : base(pUid, pId, pType, pActorGo)
        {
        }

        public override void UpdateLogic(float pTimeDelta, float pGameTime)
        {
            SetPos(Vector3.MoveTowards(GetPos(), new Vector3(Data.Position.x, 0, Data.Position.y) , Data.MoveSpeed * pTimeDelta));
            SetRotation(Quaternion.Euler(0, Data.Rotation, 0));
        }
    }
}
