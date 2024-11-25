using Game.Network;
using Gameplay.GameData;
using UnityEngine;

namespace Gameplay.GameMap.Actor
{
    public class Actor_LocalGamer : Actor_Gamer<LocalGamerData>
    {
        private float preMoveTotalTime = NetworkLogicTimer.FixedDelta * 3;
        private Vector2 currTargetPos;
        private Vector3 currMoveTargetPos;
        private float currMoveTime;

        public Actor_LocalGamer(string pUid, int pId, ActorType pType, GameObject pActorGo) : base(pUid, pId, pType, pActorGo)
        {
        }
    }
}
