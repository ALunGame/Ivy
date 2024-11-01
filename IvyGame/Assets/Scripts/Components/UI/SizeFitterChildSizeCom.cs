using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// UI尺寸自适应组件
    /// </summary>
    [ExecuteAlways]
    public class SizeFitterChildSizeCom : MonoBehaviour
    {
        private void OnEnable()
        {
            RefreshRect();
        }


        public void RefreshRect()
        {
            float width = 0;
            float height = 0;

            RectTransform rectTrans;
            for (int i = 0; i < transform.childCount; i++)
            {
                rectTrans = transform.GetChild(i).GetComponent<RectTransform>();
                if (!rectTrans.gameObject.activeInHierarchy)
                    continue;

                if(rectTrans.sizeDelta.x > width)
                {
                    width = rectTrans.sizeDelta.x;
                }
                if (rectTrans.sizeDelta.y > height)
                {
                    height = rectTrans.sizeDelta.y;
                }
            }

            rectTrans = GetComponent<RectTransform>();
            rectTrans.sizeDelta = new Vector2(width, height);

            Debug.Log($"RefreshRect>>{width}-{height}");
        }
    } 
}
