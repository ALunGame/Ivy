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

        public override void UpdateLogic(float pTimeDelta, float pGameTime)
        {
            base.UpdateLogic(pTimeDelta, pGameTime);

            if (!Data.Position.Equals(currTargetPos))
            {
                currMoveTime = 0;
                currTargetPos = Data.Position;
                currMoveTargetPos = new Vector3(currTargetPos.x, GetPos().y, currTargetPos.y);
            }

            Vector3 movePos = Vector3.Lerp(GetPos(), currMoveTargetPos, currMoveTime / preMoveTotalTime);
            currMoveTime += NetworkLogicTimer.FixedDelta;

            SetPos(movePos);
            SetRotation(Quaternion.Euler(0, Data.Rotation, 0));
        }
    }
}
