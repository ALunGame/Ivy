using UnityEngine;

namespace IAUI
{
    public class UICanvas : MonoBehaviour
    {
        [SerializeField]
        private RectTransform baseTrans;

        [SerializeField]
        private RectTransform firstTrans;

        [SerializeField]
        private RectTransform secondTrans;

        [SerializeField]
        private RectTransform threeTrans;

        [SerializeField]
        private RectTransform topTrans;

        public RectTransform BaseTrans { get => baseTrans; }
        public RectTransform FirstTrans { get => firstTrans;}
        public RectTransform SecondTrans { get => secondTrans;}
        public RectTransform ThreeTrans { get => threeTrans; }
        public RectTransform TopTrans { get => topTrans;}
    }
}