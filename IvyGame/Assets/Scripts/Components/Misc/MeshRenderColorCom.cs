using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// 网格渲染颜色改变
    /// </summary>
    internal class MeshRenderColorCom : MonoBehaviour
    {
        [Header("当前颜色")]
        public Color CurrColor = Color.white;

        [Header("修改的颜色属性名")]
        public string ColorKeyStr = "_BaseColor";

        private MeshRenderer meshRenderer;
        private MaterialPropertyBlock propertyBlock;

        private void Awake()
        {
            if (propertyBlock == null)
                propertyBlock = new MaterialPropertyBlock();
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.GetPropertyBlock(propertyBlock);
        }

        private void OnEnable()
        {
            //ChangeColor(meshColor);
        }

        private void Update()
        {
            ChangeColor(CurrColor);
        }

        public void ChangeColor(Color pColor)
        {
            CurrColor = pColor;
            propertyBlock.SetColor(ColorKeyStr, pColor);
            meshRenderer.SetPropertyBlock(propertyBlock);

            //meshRenderer.material.SetColor("_BaseColor", pColor);
        }
    }
}
