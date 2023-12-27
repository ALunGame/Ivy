using IAEngine;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace LCUI
{
    public static class BtnUtil
    {
        public static void SetClick(Transform trans, string path, UnityAction<PointerEventData> clickFunc)
        {
            Transform btnTrans = trans;
            if (!string.IsNullOrEmpty(path))
                btnTrans = trans.Find(path);
            BaseButton btnCom = btnTrans.GetOrAddCom<BaseButton>();
            btnCom.SetClick(clickFunc);
        }

        public static void SetClick(Transform trans, string path, Action clickFunc)
        {
            SetClick(trans, path, (data) =>
            {
                clickFunc?.Invoke();
            });
        }

        public static void ClearClick(Transform trans, string path)
        {
            Transform btnTrans = trans;
            if (!string.IsNullOrEmpty(path))
                btnTrans = trans.Find(path);
            BaseButton btnCom = btnTrans.GetComponent<BaseButton>();
            if (btnCom == null)
            {
                return;
            }
            btnCom.SetClick(null);
        }
    }
}
