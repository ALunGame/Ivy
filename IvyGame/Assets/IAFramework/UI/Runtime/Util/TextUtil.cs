using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LCUI
{
    public class TextUtil
    {
        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="pTrans"></param>
        /// <param name="pPath"></param>
        /// <param name="pContent">内容</param>
        public static void SetText(Transform pTrans, string pPath, string pContent)
        {
            if (pTrans == null)
            {
                return;
            }

            Transform tmpTrans = pTrans;
            if (!string.IsNullOrEmpty(pPath))
                tmpTrans = pTrans.Find(pPath);

            TextMeshProUGUI tmpUGUICom = tmpTrans.GetComponent<TextMeshProUGUI>();
            if (tmpUGUICom != null)
            {
                tmpUGUICom.text = pContent;
                return;
            }

            TextMeshPro tmpCom = tmpTrans.GetComponent<TextMeshPro>();
            if (tmpCom != null)
            {
                tmpCom.text = pContent;
                return;
            }

            Text textCom = tmpTrans.GetComponent<Text>();
            if (textCom != null)
            {
                textCom.text = pContent;
                return;
            }
        }
    }
}
