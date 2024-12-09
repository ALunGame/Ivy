using Cinemachine;
using Gameplay.GameMap.Actor;
using Gameplay.GameMap.System;

namespace Gameplay.System
{
    internal class GameCameraSystem : BaseGameMapSystem
    {
        private CinemachineVirtualCamera vCamera;

        public override void OnStartGame()
        {
            GameActorSystem actorSystem = GameplayGlobal.Map.GetSystem<GameActorSystem>();
            ActorModel localGamer = actorSystem.GetActor(GameplayGlobal.Data.SelfGamerUid);

            vCamera = GameplayGlobal.Map.MapTrans.Find("VCCameras/FollowCamera").GetComponent<CinemachineVirtualCamera>();
            vCamera.Follow = localGamer.ActorDisplay.CameraFollowGo.transform;
            vCamera.LookAt = localGamer.ActorDisplay.CameraFollowGo.transform;
        }
    }
}
