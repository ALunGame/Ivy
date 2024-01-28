using Game;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Gameplay.Grid
{
    internal class GameGrid : MonoBehaviour
    {
        public byte PosX {  get; private set; }
        public byte PosY {  get; private set; }
        public byte Camp {  get; private set; }

        private float changeScaleTimer;
        private float changeScaleDelTime = 0.5f;
        private int currScale = 1;

        private MeshRenderColorCom colorCom;
        private MeshRenderer meshRenderer;

        public void Init(byte pX, byte pY)
        {
            PosX = pX;
            PosY = pY;
            Camp = 0;

            transform.localPosition = GameInstance.ServerPosToClient(PosX, PosY);

            meshRenderer = transform.Find("Display/Cube").GetComponent<MeshRenderer>();
            meshRenderer.sortingOrder = 5;

            colorCom = transform.Find("Display/Cube").gameObject.AddComponent<MeshRenderColorCom>();
            colorCom.ChangeColor(TempConfig.CampColorDict[Camp]);
        }

        public void ChangeCamp(byte camp)
        {
            if (Camp == camp)
            {
                return;
            }
            Camp = camp;
            colorCom.ChangeColor(TempConfig.CampColorDict[Camp]);
        }

        private void Awake()
        {
            changeScaleTimer = Random.Range(0,0.5f);
        }

        private void Update()
        {
            if (Camp != 0)
            {
                changeScaleTimer += Time.deltaTime;
                if (changeScaleTimer >= changeScaleDelTime)
                {
                    float newScale = Random.Range(1.0f, 3.0f);
                    meshRenderer.transform.localScale = new Vector3(1, newScale, 1);
                    changeScaleTimer = 0;
                }
            }
        }
    }
}
