using Game;
using Gameplay.Player;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Gameplay
{
    internal class GamePlayerPath : MonoBehaviour
    {
        public GamePlayer Player { get; private set; }
        public byte PosX { get; private set; }
        public byte PosY { get; private set; }

        private MeshRenderColorCom colorCom;
        private MeshRenderer meshRenderer;

        public void Init(GamePlayer pPlayer, byte pX, byte pY)
        {
            Player = pPlayer;   
            PosX = pX;
            PosY = pY;

            transform.localPosition = GameInstance.ServerPosToClient(PosX, PosY);

            meshRenderer = transform.Find("Display/Cube").GetComponent<MeshRenderer>();
            meshRenderer.sortingOrder = 10;

            colorCom = transform.Find("Display/Cube").gameObject.AddComponent<MeshRenderColorCom>();
            colorCom.ChangeColor(TempConfig.CampColorDict[pPlayer.Camp]);
        }
    }
}
