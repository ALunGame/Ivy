using Cinemachine;
using Game.Network.Client;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.System
{
    internal class CameraSystem : GameplaySystem
    {
        private CinemachineVirtualCamera vCamera;

        protected override void OnInit()
        {
            vCamera = GameObject.Find("VCamera").GetComponent<CinemachineVirtualCamera>();
        }

        protected override void OnAfterStartGame()
        {
            GamePlayer localPlayer = GameplayLocate.GameIns.GetPlayer(NetClientLocate.LocalToken.PlayerUid);
            vCamera.Follow = localPlayer.transform;
        }

    }
}
