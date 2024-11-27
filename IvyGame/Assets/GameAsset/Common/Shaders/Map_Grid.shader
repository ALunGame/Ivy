Shader "Custom/URP/Map Grid"
{
    Properties
    {
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}       //纹理
        _Glossiness("Smoothness", Range(0, 1)) = 0.5            // 光滑度属性
        _Metallic("Metallic", Range(0, 1)) = 0.0                // 金属度属性
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
        half _Glossiness;
        half _Metallic;

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

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

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
                half3 normalWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 color: TEXCOORD3;
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
                OUT.positionWS = positionWS;
                OUT.normalWS = TransformObjectToWorldNormal(normalize(mul(data, IN.normalOS)));

                //设置颜色
                OUT.color = color;

                // 计算裁剪空间下的顶点坐标
                OUT.positionCS = mul(unity_MatrixVP, float4(positionWS, 1.0));
                OUT.uv = TRANSFORM_TEX(IN.texcoord, _BaseMap);
               
                return OUT;
            }

            half4 Fragment(Varyings IN) : SV_Target
            {
                 // 获取主光源信息
                Light mainLight = GetMainLight();
                half3 lightDirWS = normalize(mainLight.direction.xyz);
                half3 normalWS = normalize(IN.normalWS.xyz);

                // 计算漫反射
                half diff = saturate(dot(normalWS, lightDirWS));
                half3 diffuse = IN.color.rgb * mainLight.color * diff;

                // 计算高光反射
                half3 viewDirWS = normalize(_WorldSpaceCameraPos.xyz - IN.positionWS);
                half3 halfDirWS = normalize(lightDirWS + viewDirWS);
                half spec = pow(saturate(dot(normalWS, halfDirWS)), _Glossiness * 256.0);
                half3 specular = mainLight.color * spec * _Metallic;

                // 计算最终颜色
                half3 color = diffuse + specular;
                return half4(color, IN.color.a);
            }
            ENDHLSL
        }

        Pass
        {
            Tags
            {
                "LightMode" = "ShadowCaster"
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
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };
        
            float3 _LightDirection;
        
            Varyings Vertex(Attributes IN, uint instanceID : SV_InstanceID)
            {
                Varyings OUT;
        
                #if SHADER_TARGET >= 45
                float4x4 data = _AllMatricesBuffer[instanceID].instanceToObjectMatrix;
				float4 color = _AllMatricesBuffer[instanceID].color;
                #else
                float4x4 data = 0;
                float4 color = float4(1.0f, 1.0f, 1.0f, 1.0f);
                #endif

                float3 positionWS = mul(data, IN.positionOS).xyz;
                
                // float3 normalWS = TransformObjectToWorldNormal(normalize(mul(data, IN.normalOS)));
                float3 normalWS = normalize(mul(data, float4(IN.normalOS, 0))).xyz;
                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
                
                #if UNITY_REVERSED_Z
                positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #else
                positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #endif
        
                OUT.positionCS = positionCS;
                OUT.uv = TRANSFORM_TEX(IN.texcoord, _BaseMap);
                return OUT;
            }
        
            half4 Fragment(Varyings IN) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
    }
}