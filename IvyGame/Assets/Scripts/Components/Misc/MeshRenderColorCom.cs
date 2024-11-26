using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// 网格渲染颜色改变
    /// </summary>
    internal class MeshRenderColorCom : MonoBehaviour
    { 
        public Color meshColor = Color.white;

        private string colorKeyStr = "color";

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
            ChangeColor(meshColor);
        }

        private void Update()
        {
            ChangeColor(meshColor);
        }

        public void ChangeColor(Color pColor)
        {
            if (pColor == Color.white)
            {
                Debug.LogError("aaaaaaaaaaaa");
            }
            Debug.LogError($"ChangeColor-->{gameObject.name}::{pColor}");
            meshColor = pColor;
            propertyBlock.SetColor("_BaseColor", pColor);
            meshRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
