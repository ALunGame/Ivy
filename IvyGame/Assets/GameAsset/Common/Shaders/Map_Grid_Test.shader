Shader "Custom/URP/Map Grid Test"
{
    Properties
    {
        _BaseMap("Albedo", 2D) = "white" {}       //纹理
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        CBUFFER_START(UnityPerMaterial)             //常量缓冲区，这样才会被合批处理
        float4 _BaseMap_ST;

        struct InstanceParam
		{			
			float4 color;
			float4x4 instanceToObjectMatrix;
		};

        StructuredBuffer<InstanceParam> _AllMatricesBuffer;
        CBUFFER_END

        TEXTURE2D(_BaseMap);
        SAMPLER(sampler_BaseMap);
        ENDHLSL

        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vertex
            #pragma fragment Fragment

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color: TEXCOORD1;
            };

            Varyings Vertex(Attributes IN, uint instanceID : SV_InstanceID)
            {
                Varyings OUT;

                // 旋转和坐标转换
                #if SHADER_TARGET >= 45
                    float4x4 data = _AllMatricesBuffer[instanceID].instanceToObjectMatrix;
				    float4 color = _AllMatricesBuffer[instanceID].color;
                #else
                    float4x4 data = 0;
                    float4 color = float4(1.0f, 1.0f, 1.0f, 1.0f);
                #endif

                // 计算世界空间下的顶点坐标
                float3 positionWS = mul(data, IN.positionOS).xyz;
                // OUT.positionWS = positionWS;

                //设置颜色
                OUT.color = color;

                // 计算裁剪空间下的顶点坐标
                OUT.positionCS = mul(unity_MatrixVP, float4(positionWS, 1.0));
                OUT.uv = TRANSFORM_TEX(IN.texcoord, _BaseMap);

                return OUT;
            }

            half4 Fragment(Varyings IN) : SV_Target
            {
                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                return half4(albedo.rgb, 1.0) * IN.color;
            }
            ENDHLSL
        }
    }
}