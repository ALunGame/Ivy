Shader "Game/URP/ToonShader"
{
    Properties
    {
        [Header(High Level Setting)]
        [ToggleUI]_IsFace("Is Face? (face/eye/mouth)", Float) = 0

        [Header(Base Color)]
        [MainTexture]_BaseMap("Base Map", 2D) = "white" {}
        [HDR][MainColor]_BaseColor("Base Color", Color) = (1,1,1,1)

        [Header(Alpha Clipping)]
        [Toggle(_UseAlphaClipping)]_UseAlphaClipping("Enable?", Float) = 0
        _Cutoff("    Cutoff", Range(0.0, 1.0)) = 0.5

        [Header(Emission)]
        [Toggle]_UseEmission("Enable?", Float) = 0
        [HDR] _EmissionColor("    Color", Color) = (0,0,0)
        _EmissionMulByBaseColor("    Mul Base Color", Range(0,1)) = 0
        [NoScaleOffset]_EmissionMap("    Emission Map", 2D) = "white" {}
        _EmissionMapChannelMask("        ChannelMask", Vector) = (1,1,1,0)

        [Header(Occlusion)]
        [Toggle]_UseOcclusion("Enable?", Float) = 0
        _OcclusionStrength("    Strength", Range(0.0, 1.0)) = 1.0
        [NoScaleOffset]_OcclusionMap("    OcclusionMap", 2D) = "white" {}
        _OcclusionMapChannelMask("        ChannelMask", Vector) = (1,0,0,0)
        _OcclusionRemapStart("        RemapStart", Range(0,1)) = 0
        _OcclusionRemapEnd("        RemapEnd", Range(0,1)) = 1

        [Header(Indirect Light)]
        _IndirectLightMinColor("Min Color", Color) = (0.1,0.1,0.1,1) // can prevent completely black if light prob is not baked
        _IndirectLightMultiplier("Multiplier", Range(0,1)) = 1
        
        [Header(Direct Light)]
        _DirectLightMultiplier("Brightness", Range(0,1)) = 1
        _CelShadeMidPoint("MidPoint", Range(-1,1)) = -0.5
        _CelShadeSoftness("Softness", Range(0,1)) = 0.05
        _MainLightIgnoreCelShade("Remove Shadow", Range(0,1)) = 0
        
        [Header(Additional Light)]
        _AdditionalLightIgnoreCelShade("Remove Shadow", Range(0,1)) = 0.9

        [Header(Shadow Mapping)]
        _ReceiveShadowMappingAmount("Strength", Range(0,1)) = 0.65
        _ShadowMapColor("    Shadow Color", Color) = (0.6627451,0.6627451,0.6627451)
        _ReceiveShadowMappingPosOffset("    Depth Bias", Float) = 0

        [Header(Outline)]
        _OutlineWidth("Width", Range(0,4)) = 1
        _OutlineColor("Color", Color) = (0.5,0.5,0.5,1)
        
        [Header(Outline ZOffset)]
        _OutlineZOffset("ZOffset (View Space)", Range(0,1)) = 0.0001
        [NoScaleOffset]_OutlineZOffsetMaskTex("    Mask (black is apply ZOffset)", 2D) = "black" {}
        _OutlineZOffsetMaskRemapStart("    RemapStart", Range(0,1)) = 0
        _OutlineZOffsetMaskRemapEnd("    RemapEnd", Range(0,1)) = 1
    }

    SubShader
    {
        Tags 
        { 
            //表明只在URP渲染管线运行
            "RenderPipeline" = "UniversalPipeline"

            "RenderType" = "Opaque"
            "IgnoreProjector" = "True"
            "UniversalMaterialType" = "ComplexLit"
            "Queue"="Geometry"
        }

        LOD 100

        HLSLINCLUDE

        //Alpha裁剪
        #pragma shader_feature_local_fragment _UseAlphaClipping

        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForwardOnly"
            }

            Blend One Zero
            ZWrite On
            Cull Off
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma target 2.0

            #pragma vertex VertexShaderWork
            #pragma fragment ShadeFinalColor

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _FORWARD_PLUS
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            #pragma multi_compile_fog
            #pragma multi_compile_fragment _ DEBUG_DISPLAY

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            //执行渲染
            #include "Assets/GameAsset/Common/Shaders/TestToon/Include/ToonShader_Shared.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "Outline"
            Tags
            {
                
            }

            Blend One Zero
            ZWrite On
            Cull Front
            ZTest LEqual

            HLSLPROGRAM
            #pragma target 2.0
            
            // -------------------------------------
            // Shader Stages
            #pragma vertex VertexShaderWork
            #pragma fragment ShadeFinalColor

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _FORWARD_PLUS
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            #pragma multi_compile_fog
            #pragma multi_compile_fragment _ DEBUG_DISPLAY

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            //--------------------------------------
            // Defines
            // - because this is an Outline pass, define "ToonShaderIsOutline" to inject outline related code into both VertexShaderWork() and ShadeFinalColor()
            #define ToonShaderIsOutline

            //执行渲染
            #include "Assets/GameAsset/Common/Shaders/TestToon/Include/ToonShader_Shared.hlsl"

            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
