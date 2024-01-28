using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// 网格渲染颜色改变
    /// </summary>
    internal class MeshRenderColorCom : MonoBehaviour
    { 
        [SerializeField]
        private Color meshColor = Color.white;

        private string colorKeyStr = "color";

        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();    
            meshRenderer.material.color = meshColor;
        }

        public void ChangeColor(Color pColor)
        {
            meshColor = pColor;
            meshRenderer.material.color = pColor;   
        }
    }
}
