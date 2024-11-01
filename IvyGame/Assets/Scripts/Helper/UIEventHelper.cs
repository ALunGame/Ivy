using IAUI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Helper
{
    public static class UIEventHelper
    {
        public static Vector3 PointerToWorldPos(PointerEventData pEventData)
        {
            Canvas uiCanvas = UILocate.UICenter.StaticCanvas.GetComponent<Canvas>();

            Vector3 worldPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(uiCanvas.transform as RectTransform,
                Clamp(Camera.main.pixelRect, pEventData.position), Camera.main, out worldPos);

            return worldPos;
        }

        public static Vector2 Clamp(Rect rect, Vector2 pos)
        {
            var x = Mathf.Clamp(pos.x, rect.xMin, rect.xMax);
            var y = Mathf.Clamp(pos.y, rect.yMin, rect.yMax);
            return new Vector2(x, y);
        }
    }
}
