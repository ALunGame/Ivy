using Game;
using Gameplay.GameData;
using UnityEngine;

namespace Gameplay.GameMap.Actor
{
    public class Actor_Grid : ActorModel
    {
        public static Vector2 GridSize = new Vector2(0.5f, 0.5f);

        public int Camp { get; private set; }

        private float changeScaleTimer;
        private float changeScaleDelTime = 0.5f;
        private int currScale = 1;

        private MeshRenderColorCom colorCom;
        private MeshRenderer meshRenderer;

        public Actor_Grid(string pUid, int pId, ActorType pType, GameObject pActorGo) : base(pUid, pId, pType, pActorGo)
        {
            meshRenderer = pActorGo.transform.Find("State/Default/Display/Cube").GetComponent<MeshRenderer>();
            meshRenderer.sortingOrder = 5;

            colorCom = pActorGo.transform.Find("State/Default/Display/Cube").gameObject.AddComponent<MeshRenderColorCom>();
            colorCom.ChangeColor(TempConfig.CampColorDict[0]);

            changeScaleTimer = Random.Range(0, 0.5f);
        }

        public override void UpdateLogic(float pTimeDelta, float pGameTime)
        {
            if (Camp != 0)
            {
                changeScaleTimer += pTimeDelta;
                if (changeScaleTimer >= changeScaleDelTime)
                {
                    float newScale = Random.Range(1.0f, 3.0f);
                    meshRenderer.transform.localScale = new Vector3(1, newScale, 1);
                    changeScaleTimer = 0;
                }
            }
        }

        public void SetData(GameMapGridData pData)
        {
            Camp = pData.Camp.Value;
            colorCom.ChangeColor(TempConfig.CampColorDict[Camp]);
            pData.Camp.RegValueChangedEvent(OnCampChange);
        }

        public void OnCampChange(int pCamp, int pOldValue)
        {
            Camp = pCamp;
            colorCom.ChangeColor(TempConfig.CampColorDict[pCamp]);
        }
    }
}
