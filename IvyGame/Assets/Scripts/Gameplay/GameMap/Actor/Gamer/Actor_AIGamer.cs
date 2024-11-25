using Gameplay.GameData;
using IAEngine;
using UnityEngine;

namespace Gameplay.GameMap.Actor
{
    /// <summary>
    /// 电脑玩家
    /// </summary>
    internal class Actor_AIGamer : Actor_Gamer<AIGamerData>
    {
        public TimerModel AIUpdateCDTimer {  get; private set; }
        public bool AINeedUpdate {  get; set; }
        public Vector2Int AIMoveDir {  get; set; }

        public TimerModel AIMoveToCampTimer { get; private set; }
        public bool AIMoveToCamp { get; set; }

        public Actor_AIGamer(string pUid, int pId, ActorType pType, GameObject pActorGo) : base(pUid, pId, pType, pActorGo)
        {
            AINeedUpdate = true;
            AIUpdateCDTimer = new TimerModel($"Actor_AIGamer_{Uid}", 3, () =>
            {
                AINeedUpdate = true;
            });

            AIMoveToCamp = false;
            AIMoveToCampTimer = new TimerModel($"Actor_AIGamer_{Uid}", 8, () =>
            {
                AIMoveToCamp = true;
            });
        }

        public override void UpdateLogic(float pTimeDelta, float pGameTime)
        {
            base.UpdateLogic(pTimeDelta, pGameTime);
        }
    }
}
