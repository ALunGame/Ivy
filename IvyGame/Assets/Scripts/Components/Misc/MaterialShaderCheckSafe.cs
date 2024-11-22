using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Gameplay
{
    internal class MaterialShaderCheckSafe : MonoBehaviour
    {
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void OnEnable()
        {
            if (meshRenderer != null)
            {
                if (!meshRenderer.material.shader.isSupported)
                {
                    Debug.LogError($"Mesh:{meshRenderer.name} Shader:{meshRenderer.material.shader.name}不支持，自动关闭");
                    meshRenderer.enabled = false;
                }
            }
        }
    }
}
