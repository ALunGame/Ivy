
//必须定义
#pragma once

//内置文件
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

//辅助文件
#include "Assets/GameAsset/Common/Shaders/TestToon/Include/Help/NiloOutlineUtil.hlsl"
#include "Assets/GameAsset/Common/Shaders/TestToon/Include/Help/NiloZOffset.hlsl"
#include "Assets/GameAsset/Common/Shaders/TestToon/Include/Help/NiloInvLerpRemap.hlsl"


//后缀OS表示对象空间（例如positionOS =位置对象空间）
//后缀WS表示世界空间（例如positionWS =位置世界空间）
//后缀VS表示视图空间（例如positionVS =位置视图空间）
//子后缀CS表示剪辑空间（例如positionCS =位置剪辑空间）

struct Attributes
{
    float3 positionOS : POSITION;
    half3 normalOS : NORMAL;
    half4 tangentOS : TANGENT;
    float2 uv : TEXCOORD0;

    //为了GPU instancing
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv : TEXCOORD0;
    float4 positionWSAndFogFactor : TEXCOORD1; // xyz: positionWS, w: vertex fog factor
    half3 normalWS : TEXCOORD2;
    float4 positionCS : SV_POSITION;

    //为了GPU instancing
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};
    
sampler2D _BaseMap;
sampler2D _EmissionMap;
sampler2D _OcclusionMap;
sampler2D _OutlineZOffsetMaskTex;
    
CBUFFER_START(UnityPerMaterial)
    
    //脸部
    float _IsFace;

    //基础颜色和贴图
    float4 _BaseMap_ST;
    half4 _BaseColor;

    // alpha
    half _Cutoff;

    //自发光
    float _UseEmission;
    half3 _EmissionColor;
    half _EmissionMulByBaseColor;
    half3 _EmissionMapChannelMask;

    // occlusion
    float _UseOcclusion;
    half _OcclusionStrength;
    half4 _OcclusionMapChannelMask;
    half _OcclusionRemapStart;
    half _OcclusionRemapEnd;

    // lighting
    half3 _IndirectLightMinColor;
    half _CelShadeMidPoint;
    half _CelShadeSoftness;

    // shadow mapping
    half _ReceiveShadowMappingAmount;
    float _ReceiveShadowMappingPosOffset;
    half3 _ShadowMapColor;

    // outline
    float _OutlineWidth;
    half3 _OutlineColor;
    float _OutlineZOffset;
    float _OutlineZOffsetMaskRemapStart;
    float _OutlineZOffsetMaskRemapEnd;

CBUFFER_END
    
float3 _LightDirection;
    
struct ToonSurfaceData
{
    half3 albedo;
    half alpha;
    half3 emission;
    half occlusion;
};
struct ToonLightingData
{
    half3 normalWS;
    float3 positionWS;
    half3 viewDirectionWS;
    float4 shadowCoord;
};
    
///////////////////////////////////////////////////////////////////////////////////////
// vertex shared functions
///////////////////////////////////////////////////////////////////////////////////////
float3 TransformPositionWSToOutlinePositionWS(float3 positionWS, float positionVS_Z, float3 normalWS)
{
    //you can replace it to your own method! Here we will write a simple world space method for tutorial reason, it is not the best method!
    float outlineExpandAmount = _OutlineWidth * GetOutlineCameraFovAndDistanceFixMultiplier(positionVS_Z);

    #if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED) || defined(UNITY_STEREO_DOUBLE_WIDE_ENABLED)
    outlineExpandAmount *= 0.5;
    #endif
    
    return positionWS + normalWS * outlineExpandAmount;
}
    
Varyings VertexShaderWork(Attributes input)
{
    Varyings output;

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // after invalid/discard vertex, do this part asap.
    // to support GPU instancing and Single Pass Stereo rendering(VR), add the following section
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    UNITY_SETUP_INSTANCE_ID(input); // will turn into this in non OpenGL and non PSSL -> UnitySetupInstanceID(input.instanceID);
    UNITY_TRANSFER_INSTANCE_ID(input, output); // will turn into this in non OpenGL and non PSSL -> output.instanceID = input.instanceID;
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output); // will turn into this in non OpenGL and non PSSL -> output.stereoTargetEyeIndexAsRTArrayIdx = unity_StereoEyeIndex;
    
    // VertexPositionInputs contains position in multiple spaces (world, view, homogeneous clip space, ndc)
    // Unity compiler will strip all unused references (say you don't use view space).
    // Therefore there is more flexibility at no additional cost with this struct.
    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS);

    // Similar to VertexPositionInputs, VertexNormalInputs will contain normal, tangent and bitangent
    // in world space. If not used it will be stripped.
    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    float3 positionWS = vertexInput.positionWS;

#ifdef ToonShaderIsOutline
    positionWS = TransformPositionWSToOutlinePositionWS(vertexInput.positionWS, vertexInput.positionVS.z, vertexNormalInput.normalWS);
#endif

    // Computes fog factor per-vertex.
    float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

    // TRANSFORM_TEX is the same as the old shader library.
    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);

    // packing positionWS(xyz) & fog(w) into a vector4
    output.positionWSAndFogFactor = float4(positionWS, fogFactor);
    output.normalWS = vertexNormalInput.normalWS; //normlaized already by GetVertexNormalInputs(...)

    output.positionCS = TransformWorldToHClip(positionWS);

#ifdef ToonShaderIsOutline
    // [Read ZOffset mask texture]
    // we can't use tex2D() in vertex shader because ddx & ddy is unknown before rasterization, 
    // so use tex2Dlod() with an explict mip level 0, put explict mip level 0 inside the 4th component of param uv)
    float outlineZOffsetMaskTexExplictMipLevel = 0;
    float outlineZOffsetMask = tex2Dlod(_OutlineZOffsetMaskTex, float4(input.uv,0,outlineZOffsetMaskTexExplictMipLevel)).r; //we assume it is a Black/White texture

    // [Remap ZOffset texture value]
    // flip texture read value so default black area = apply ZOffset, because usually outline mask texture are using this format(black = hide outline)
    outlineZOffsetMask = 1-outlineZOffsetMask;
    outlineZOffsetMask = invLerpClamp(_OutlineZOffsetMaskRemapStart,_OutlineZOffsetMaskRemapEnd,outlineZOffsetMask);// allow user to flip value or remap

    // [Apply ZOffset, Use remapped value as ZOffset mask]
    output.positionCS = NiloGetNewClipPosWithZOffset(output.positionCS, _OutlineZOffset * outlineZOffsetMask + 0.03 * _IsFace);
#endif

    // ShadowCaster pass needs special process to positionCS, else shadow artifact will appear
    //--------------------------------------------------------------------------------------
#ifdef ToonShaderApplyShadowBiasFix
    // see GetShadowPositionHClip() in URP/Shaders/ShadowCasterPass.hlsl
    // https://github.com/Unity-Technologies/Graphics/blob/master/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl
    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, output.normalWS, _LightDirection));

#if UNITY_REVERSED_Z
    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#else
    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#endif
    output.positionCS = positionCS;
#endif
    //--------------------------------------------------------------------------------------    

    return output;
}
    

///////////////////////////////////////////////////////////////////////////////////////
// fragment shared functions (Step1: prepare data structs for lighting calculation)
///////////////////////////////////////////////////////////////////////////////////////
half4 GetFinalBaseColor(Varyings input)
{
    return tex2D(_BaseMap, input.uv) * _BaseColor;
}
half3 GetFinalEmissionColor(Varyings input)
{
    half3 result = 0;
    if (_UseEmission)
    {
        result = tex2D(_EmissionMap, input.uv).rgb * _EmissionMapChannelMask * _EmissionColor.rgb;
    }

    return result;
}
half GetFinalOcculsion(Varyings input)
{
    half result = 1;
    if (_UseOcclusion)
    {
        half4 texValue = tex2D(_OcclusionMap, input.uv);
        half occlusionValue = dot(texValue, _OcclusionMapChannelMask);
        occlusionValue = lerp(1, occlusionValue, _OcclusionStrength);
        occlusionValue = invLerpClamp(_OcclusionRemapStart, _OcclusionRemapEnd, occlusionValue);
        result = occlusionValue;
    }

    return result;
}
void DoClipTestToTargetAlphaValue(half alpha)
{
#if _UseAlphaClipping
    clip(alpha - _Cutoff);
#endif
}
ToonSurfaceData InitializeSurfaceData(Varyings input)
{
    ToonSurfaceData output;

    // albedo & alpha
    float4 baseColorFinal = GetFinalBaseColor(input);
    output.albedo = baseColorFinal.rgb;
    output.alpha = baseColorFinal.a;
    DoClipTestToTargetAlphaValue(output.alpha); // early exit if possible

    // emission
    output.emission = GetFinalEmissionColor(input);

    // occlusion
    output.occlusion = GetFinalOcculsion(input);

    return output;
}
ToonLightingData InitializeLightingData(Varyings input)
{
    ToonLightingData lightingData;
    lightingData.positionWS = input.positionWSAndFogFactor.xyz;
    lightingData.viewDirectionWS = SafeNormalize(GetCameraPositionWS() - lightingData.positionWS);
    lightingData.normalWS = normalize(input.normalWS); //interpolated normal is NOT unit vector, we need to normalize it

    return lightingData;
}

///////////////////////////////////////////////////////////////////////////////////////
// fragment shared functions (Step2: calculate lighting & final color)
///////////////////////////////////////////////////////////////////////////////////////

// all lighting equation written inside this .hlsl,
// just by editing this .hlsl can control most of the visual result.
#include "ToonShader_LightingEquation.hlsl"

// this function contains no lighting logic, it just pass lighting results data around
// the job done in this function is "do shadow mapping depth test positionWS offset"
half3 ShadeAllLights(ToonSurfaceData surfaceData, ToonLightingData lightingData)
{
    // Indirect lighting
    half3 indirectResult = ShadeGI(surfaceData, lightingData);

    //////////////////////////////////////////////////////////////////////////////////
    // Light struct is provided by URP to abstract light shader variables.
    // It contains light's
    // - direction
    // - color
    // - distanceAttenuation 
    // - shadowAttenuation
    //
    // URP take different shading approaches depending on light and platform.
    // You should never reference light shader variables in your shader, instead use the 
    // -GetMainLight()
    // -GetLight()
    // funcitons to fill this Light struct.
    //////////////////////////////////////////////////////////////////////////////////

    //==============================================================================================
    // Main light is the brightest directional light.
    // It is shaded outside the light loop and it has a specific set of variables and shading path
    // so we can be as fast as possible in the case when there's only a single directional light
    // You can pass optionally a shadowCoord. If so, shadowAttenuation will be computed.
    Light mainLight = GetMainLight();

    float3 shadowTestPosWS = lightingData.positionWS + mainLight.direction * (_ReceiveShadowMappingPosOffset + _IsFace);
#ifdef _MAIN_LIGHT_SHADOWS
    // compute the shadow coords in the fragment shader now due to this change
    // https://forum.unity.com/threads/shadow-cascades-weird-since-7-2-0.828453/#post-5516425

    // _ReceiveShadowMappingPosOffset will control the offset the shadow comparsion position, 
    // doing this is usually for hide ugly self shadow for shadow sensitive area like face
    float4 shadowCoord = TransformWorldToShadowCoord(shadowTestPosWS);
    mainLight.shadowAttenuation = MainLightRealtimeShadow(shadowCoord);
#endif 

    // Main light
    half3 mainLightResult = ShadeSingleLight(surfaceData, lightingData, mainLight, false);

    //==============================================================================================
    // All additional lights

    half3 additionalLightSumResult = 0;

#ifdef _ADDITIONAL_LIGHTS
    // Returns the amount of lights affecting the object being renderer.
    // These lights are culled per-object in the forward renderer of URP.
    int additionalLightsCount = GetAdditionalLightsCount();
    for (int i = 0; i < additionalLightsCount; ++i)
    {
        // Similar to GetMainLight(), but it takes a for-loop index. This figures out the
        // per-object light index and samples the light buffer accordingly to initialized the
        // Light struct. If ADDITIONAL_LIGHT_CALCULATE_SHADOWS is defined it will also compute shadows.
        int perObjectLightIndex = GetPerObjectLightIndex(i);
        Light light = GetAdditionalPerObjectLight(perObjectLightIndex, lightingData.positionWS); // use original positionWS for lighting
        light.shadowAttenuation = AdditionalLightRealtimeShadow(perObjectLightIndex, shadowTestPosWS); // use offseted positionWS for shadow test

        // Different function used to shade additional lights.
        additionalLightSumResult += ShadeSingleLight(surfaceData, lightingData, light, true);
    }
#endif
    //==============================================================================================

    // emission
    half3 emissionResult = ShadeEmission(surfaceData, lightingData);

    return CompositeAllLightResults(indirectResult, mainLightResult, additionalLightSumResult, emissionResult, surfaceData, lightingData);
}

half3 ConvertSurfaceColorToOutlineColor(half3 originalSurfaceColor)
{
    return originalSurfaceColor * _OutlineColor;
}
half3 ApplyFog(half3 color, Varyings input)
{
    half fogFactor = input.positionWSAndFogFactor.w;
    // Mix the pixel color with fogColor. You can optionaly use MixFogColor to override the fogColor
    // with a custom one.
    color = MixFog(color, fogFactor);

    return color;
}

// only the .shader file will call this function by 
// #pragma fragment ShadeFinalColor
half4 ShadeFinalColor(Varyings input) : SV_TARGET
{
    // to support GPU instancing and Single Pass Stereo rendering(VR), add the following section
    //------------------------------------------------------------------------------------------------------------------------------
    UNITY_SETUP_INSTANCE_ID(input); // in non OpenGL and non PSSL, MACRO will turn into -> UnitySetupInstanceID(input.instanceID);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input); // in non OpenGL and non PSSL, MACRO will turn into -> unity_StereoEyeIndex = input.stereoTargetEyeIndexAsRTArrayIdx;
    //------------------------------------------------------------------------------------------------------------------------------

    //////////////////////////////////////////////////////////////////////////////////////////
    // first prepare all data for lighting function
    //////////////////////////////////////////////////////////////////////////////////////////

    // fillin ToonSurfaceData struct:
    ToonSurfaceData surfaceData = InitializeSurfaceData(input);

    // fillin ToonLightingData struct:
    ToonLightingData lightingData = InitializeLightingData(input);

    // apply all lighting calculation
    half3 color = ShadeAllLights(surfaceData, lightingData);

#ifdef ToonShaderIsOutline
    color = ConvertSurfaceColorToOutlineColor(color);
#endif

    color = ApplyFog(color, input);

    return half4(color, surfaceData.alpha);
}

//////////////////////////////////////////////////////////////////////////////////////////
// fragment shared functions (for ShadowCaster, DepthOnly, DepthNormalsOnly pass to use only)
//////////////////////////////////////////////////////////////////////////////////////////

// copy and edit of ShadowCasterPass.hlsl
void AlphaClipAndLODTest(Varyings input)
{
    DoClipTestToTargetAlphaValue(GetFinalBaseColor(input).a);

    #ifdef LOD_FADE_CROSSFADE
    LODFadeCrossFade(input.positionCS);
    #endif
}

// copy and edit of DepthOnlyPass.hlsl
half DepthOnlyFragment(Varyings input) : SV_TARGET
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    AlphaClipAndLODTest(input);

    return input.positionCS.z;
}

// copy and edit of LitDepthNormalsPass.hlsl
void DepthNormalsFragment(
    Varyings input
    , out half4 outNormalWS : SV_Target0
#ifdef _WRITE_RENDERING_LAYERS
    , out float4 outRenderingLayers : SV_Target1
#endif
)
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    AlphaClipAndLODTest(input);

    #if defined(_GBUFFER_NORMALS_OCT)
        float3 normalWS = normalize(input.normalWS);
        float2 octNormalWS = PackNormalOctQuadEncode(normalWS);           // values between [-1, +1], must use fp32 on some platforms
        float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);   // values between [ 0,  1]
        half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);      // values between [ 0,  1]
        outNormalWS = half4(packedNormalWS, 0.0);
    #else
        float2 uv = input.uv;
        #if defined(_PARALLAXMAP)
            #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
                half3 viewDirTS = input.viewDirTS;
            #else
                half3 viewDirTS = GetViewDirectionTangentSpace(input.tangentWS, input.normalWS, input.viewDirWS);
            #endif
            ApplyPerPixelDisplacement(viewDirTS, uv);
        #endif

        #if defined(_NORMALMAP) || defined(_DETAIL)
            float sgn = input.tangentWS.w;      // should be either +1 or -1
            float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
            float3 normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
        
            #if defined(_DETAIL)
                half detailMask = SAMPLE_TEXTURE2D(_DetailMask, sampler_DetailMask, uv).a;
                float2 detailUv = uv * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
                normalTS = ApplyDetailNormal(detailUv, normalTS, detailMask);
            #endif

            float3 normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz));
        #else
            float3 normalWS = input.normalWS;
        #endif

        outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
    #endif

    #ifdef _WRITE_RENDERING_LAYERS
        uint renderingLayers = GetMeshRenderingLayer();
        outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
    #endif
}