using System;
using UnityEngine;

namespace Gameplay
{
    public class MapGamerPath : MonoBehaviour
    {
        [SerializeField]
        private Transform displayTrans;
        private float animTimer;

        public int animIndex = -1;
        public float currScaleY;

        public float AnimTime = 0.5f;

        public void UpdateLogic(float pDeltaTime, float pGameTime)
        {
            if (animIndex == -1) 
            {
                return;
            }
            animTimer += pDeltaTime;
            if (animTimer >= AnimTime)
            {
                float newScale = MapGrids.GetAnimScaleY(currScaleY, animIndex);
                displayTrans.localScale = new Vector3 (displayTrans.localScale.x, newScale, displayTrans.localScale.z);
                animTimer = 0;
                currScaleY = newScale;
            }
        }

        public void SetAnimCfg(int pAnimIndex)
        {
            animIndex = pAnimIndex;
            animTimer = 0;
            currScaleY = MapGrids.AnimGridCfgList[animIndex].minScaleY;
        }
    }
}
