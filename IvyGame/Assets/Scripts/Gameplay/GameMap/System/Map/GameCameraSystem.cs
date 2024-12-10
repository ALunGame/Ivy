using Gameplay.GameMap.Actor;
using Gameplay.GameMap.System;
using Unity.Cinemachine;

namespace Gameplay.System
{
    internal class GameCameraSystem : BaseGameMapSystem
    {
        private CinemachineCamera vCamera;

        public override void OnStartGame()
        {
            GameActorSystem actorSystem = GameplayGlobal.Map.GetSystem<GameActorSystem>();
            ActorModel localGamer = actorSystem.GetActor(GameplayGlobal.Data.SelfGamerUid);

            vCamera = GameplayGlobal.Map.MapTrans.Find("VCCameras/FollowCamera").GetComponent<CinemachineCamera>();
            vCamera.Follow = localGamer.ActorDisplay.CameraFollowGo.transform;
            vCamera.LookAt = localGamer.ActorDisplay.CameraFollowGo.transform;
        }
    }
}
