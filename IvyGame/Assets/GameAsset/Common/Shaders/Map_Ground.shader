Shader "Custom/URP/MapGround"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _Tiling("Tiling", Vector) = (1, 1, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Tiling;
                TEXTURE2D(_BaseMap);
                SAMPLER(sampler_BaseMap);
            CBUFFER_END

            // Vertex Shader
            #pragma vertex Vert
            // Fragment Shader
            #pragma fragment Frag

            // Shader Target
            #pragma target 2.0

            Varyings Vert(Attributes input)
            {
                Varyings output;
                float4 positionWS = TransformObjectToWorld(input.positionOS);
                output.positionHCS = TransformWorldToHClip(positionWS);
                output.uv = input.uv * _Tiling.xy;
                return output;
            }

            half4 Frag(Varyings input) : SV_TARGET
            {
                float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                return half4(baseColor.rgb, 1.0);
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
