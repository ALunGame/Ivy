Shader "Custom/URP/Mesh Lit"
{
    Properties
    {
        [PerRendererData]_BaseColor("Base Color", Color) = (1, 1, 1, 1) // 基础颜色属性
        _Glossiness("Smoothness", Range(0, 1)) = 0.5    // 光滑度属性
        _Metallic("Metallic", Range(0, 1)) = 0.0        // 金属度属性
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

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
                half2 uv : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            half _Glossiness;
            half _Metallic;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.texcoord;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz).xyz;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 获取主光源信息
                Light mainLight = GetMainLight();
                half3 lightDirWS = normalize(mainLight.direction.xyz);
                half3 normalWS = normalize(IN.normalWS.xyz);

                // 计算漫反射
                half diff = saturate(dot(normalWS, lightDirWS));
                half3 diffuse = _BaseColor.rgb * mainLight.color * diff;

                // 计算高光反射
                half3 viewDirWS = normalize(_WorldSpaceCameraPos.xyz - IN.positionWS);
                half3 halfDirWS = normalize(lightDirWS + viewDirWS);
                half spec = pow(saturate(dot(normalWS, halfDirWS)), _Glossiness * 256.0);
                half3 specular = mainLight.color * spec * _Metallic;

                // 计算最终颜色
                half3 color = diffuse + specular;
                return half4(color, _BaseColor.a);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "ShadowCaster" }

            HLSLPROGRAM

            #pragma multi_compile_instancing
            #pragma vertex vertShadow
            #pragma fragment fragShadow

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vertShadow(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 fragShadow(Varyings IN) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
