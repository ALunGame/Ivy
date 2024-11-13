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
        public Vector2 ScaleRect = new Vector2();

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
                float newScale = ScaleRect.x == currScaleY ? ScaleRect.y : ScaleRect.x;
                displayTrans.localScale = new Vector3(displayTrans.localScale.x, newScale, displayTrans.localScale.z);
                animTimer = 0;
                currScaleY = newScale;
            }
        }

        public void SetAnimCfg(Vector2 pSizeRect)
        {
            ScaleRect = pSizeRect;
            animTimer = 0;
            currScaleY = ScaleRect.x;
        }
    }
}
