#ifndef UNIVERSAL_TOON_INPUT_INCLUDED
#define UNIVERSAL_TOON_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"


#define fixed  half
#define fixed3 half3
#define fixed4 half4

CBUFFER_START(UnityPerMaterial)
float _utsTechnique;
float4 _MainTex_ST;

float4 _Color;
fixed _Use_BaseAs1st;
fixed _Use_1stAs2nd;
fixed _Is_LightColor_Base;
//
float4 _1st_ShadeMap_ST;
float4 _1st_ShadeColor;
fixed _Is_LightColor_1st_Shade;
float4 _2nd_ShadeMap_ST;
float4 _2nd_ShadeColor;
fixed _Is_LightColor_2nd_Shade;
float4 _NormalMap_ST;

fixed _Is_NormalMapToBase;
fixed _Set_SystemShadowsToBase;
float _Tweak_SystemShadowsLevel;
float _BaseColor_Step;
float _BaseShade_Feather;

float4 _Set_1st_ShadePosition_ST;
float _ShadeColor_Step;
float _1st2nd_Shades_Feather;
float4 _Set_2nd_ShadePosition_ST;

float4 _ShadingGradeMap_ST;

float _Tweak_ShadingGradeMapLevel;
fixed _BlurLevelSGM;

float _1st_ShadeColor_Step;
float _1st_ShadeColor_Feather;
float _2nd_ShadeColor_Step;
float _2nd_ShadeColor_Feather;

float4 _HighColor;
float4 _HighColor_Tex_ST;
fixed _Is_LightColor_HighColor;
fixed _Is_NormalMapToHighColor;
float _HighColor_Power;

fixed _Is_SpecularToHighColor;
fixed _Is_BlendAddToHiColor;
fixed _Is_UseTweakHighColorOnShadow;
float _TweakHighColorOnShadow;

float4 _Set_HighColorMask_ST;

float _Tweak_HighColorMaskLevel;
fixed _RimLight;
float4 _RimLightColor;
fixed _Is_LightColor_RimLight;
fixed _Is_NormalMapToRimLight;
float _RimLight_Power;
float _RimLight_InsideMask;
fixed _RimLight_FeatherOff;
fixed _LightDirection_MaskOn;
float _Tweak_LightDirection_MaskLevel;
fixed _Add_Antipodean_RimLight;
float4 _Ap_RimLightColor;
fixed _Is_LightColor_Ap_RimLight;
float _Ap_RimLight_Power;
fixed _Ap_RimLight_FeatherOff;
float4 _Set_RimLightMask_ST;
float _Tweak_RimLightMaskLevel;
fixed _MatCap;

float4 _MatCap_Sampler_ST;

float4 _MatCapColor;
fixed _Is_LightColor_MatCap;
fixed _Is_BlendAddToMatCap;
float _Tweak_MatCapUV;
float _Rotate_MatCapUV;
fixed _Is_NormalMapForMatCap;

float4 _NormalMapForMatCap_ST;
float _Rotate_NormalMapForMatCapUV;
fixed _Is_UseTweakMatCapOnShadow;
float _TweakMatCapOnShadow;
//MatcapMask
// 
float4 _Set_MatcapMask_ST;
float _Tweak_MatcapMaskLevel;

fixed _Is_Ortho;

float _CameraRolling_Stabilizer;
fixed _BlurLevelMatcap;
fixed _Inverse_MatcapMask;

float _BumpScaleMatcap;

float4 _Emissive_Tex_ST;
float4 _Emissive_Color;

uniform fixed _Is_ViewCoord_Scroll;
float _Rotate_EmissiveUV;
float _Base_Speed;
float _Scroll_EmissiveU;
float _Scroll_EmissiveV;
fixed _Is_PingPong_Base;
float4 _ColorShift;
float4 _ViewShift;
float _ColorShift_Speed;
fixed _Is_ColorShift;
fixed _Is_ViewShift;
float3 emissive;
// 

float _Unlit_Intensity;

fixed _Is_Filter_HiCutPointLightColor;
fixed _Is_Filter_LightColor;

float _StepOffset;
fixed _Is_BLD;
float _Offset_X_Axis_BLD;
float _Offset_Y_Axis_BLD;
fixed _Inverse_Z_Axis_BLD;

float4 _ClippingMask_ST;

fixed _IsBaseMapAlphaAsClippingMask;
float _Clipping_Level;
fixed _Inverse_Clipping;
float _Tweak_transparency;

float _GI_Intensity;
fixed  _AngelRing;
float4 _AngelRing_Sampler_ST;
float4 _AngelRing_Color;
fixed _Is_LightColor_AR;
float _AR_OffsetU;
float _AR_OffsetV;
fixed _ARSampler_AlphaOn;

// OUTLINE 


fixed _Is_LightColor_Outline;

float _Outline_Width;
float _Farthest_Distance;
float _Nearest_Distance;
float4 _Outline_Sampler_ST;
float4 _Outline_Color;
fixed _Is_BlendBaseColor;
float _Offset_Z;

float4 _OutlineTex_ST;
fixed _Is_OutlineTex;

float4 _BakedNormal_ST;
fixed _Is_BakedNormal;

float _ZOverDrawMode;

//
// 
//
float4 _BaseMap_ST;
half4 _BaseColor;
half4 _SceondMaterialShadowColor;
half4 _SpecColor;
half4 _EmissionColor;
half _Cutoff;
half _Smoothness;
half _Metallic;
half _BumpScale;
half _OcclusionStrength;
half _Surface;

//追加分(CandyLock)
float _Amplitude;
float _WaveScale;
float _WaveSpeed;
float _WaveExp;
float2 _Speed;

//Visualizer
sampler2D _CameraReflectionTexture;			
sampler2D _ReflectionDepthTex;
float4x4 _ViewProjectInverse;
float4 _Spectra;
float3 _Center;
float _RingSrtide;
float _RingThicknessMin;
float _RingThicknessMax;
half3 _RingEmissionColor;
float _RingEmission;
float _RingSpeedMin;
float _RingSpeedMax;
half3 _GridColor;
float _ReflectionStrength;
float _Adjust;

//Genshin
half _WorldLightInfluence;
float _BloomFactor;
float _EnableEmission;
float _Emission;
float _EmissionBloomFactor;
half _EmissionMulByBaseColor;
half3 _EmissionMapChannelMask;
float4 _LightMap_ST;
float4 _FaceShadowMap_ST;
float _FaceShadowMapPow;
float _FaceShadowOffset;
float3 _ShadowMultColor;
float _ShadowArea;
half _ShadowSmooth;
float _DarkShadowArea;
float3 _DarkShadowMultColor;
half _DarkShadowSmooth;
half4 _RampArea12;
half4 _RampArea34;
half2 _RampArea5;
float _RampShadowRange;
float _EnableDefaultSpecular;
float _EnableHairSpecular;
float4 _LightSpecColor;
float _Shininess;
float _SpecMulti;
float _FixDarkShadow;
float _EnableDarkShadow;
float _FixDivideShadow;
float _IgnoreLightY;
float _FixLightY;
float _EnableLambert;
float _EnableRim;
half4 _RimColor;
float _RimSmooth;
float _RimPow;
float _EnableRimDS;
half4 _DarkSideRimColor;
float _DarkSideRimSmooth;
float _DarkSideRimPow;
float _EnableRimOther;
half4 _OtherRimColor;
float _OtherRimSmooth;
float _OtherRimPow;
half _ReceiveShadowMappingAmount;
float _ReceiveShadowMappingPosOffset;
float _Sharpness;
float _SpecularIntensity;
float _JitterIntensity;
float _SharpnessHigh;
float _SpecularIntensityHigh;

float _OutlineWidth;
float _OutlineLightAffects;
half4 _OutlineColor;
float _OutlineSaturation;
float _OutlineBrightness;
float _OutlineStrength;
float _OutlineSmoothness;

int _ReceiveShadowEnable;
float3 _ReceiveShadowMultColor;

float _HalfLambert;

half4 _MedColor;
half4 _LowColor;

CBUFFER_END

//sampler2D _MainTex;
//sampler2D _1st_ShadeMap;
//sampler2D _2nd_ShadeMap;
//sampler2D _NormalMap;

TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
TEXTURE2D(_1st_ShadeMap);
TEXTURE2D(_2nd_ShadeMap);
TEXTURE2D(_NormalMap);

sampler2D _Set_1st_ShadePosition; 
sampler2D _Set_2nd_ShadePosition;
sampler2D _ShadingGradeMap;
sampler2D _HighColor_Tex;
sampler2D _Set_HighColorMask;
sampler2D _Set_RimLightMask;
sampler2D _MatCap_Sampler;
sampler2D _NormalMapForMatCap;
sampler2D _Set_MatcapMask;
sampler2D _Emissive_Tex;
//sampler2D _ClippingMask;
TEXTURE2D(_ClippingMask);
sampler2D _AngelRing_Sampler;
sampler2D _Outline_Sampler;
sampler2D _OutlineTex;
sampler2D _BakedNormal;

//Genshin Add
TEXTURE2D(_LightMap);       SAMPLER(sampler_LightMap);
TEXTURE2D(_FaceShadowMap);  SAMPLER(sampler_FaceShadowMap);
TEXTURE2D(_RampMap);        SAMPLER(sampler_RampMap);
TEXTURE2D(_MetalMap);       SAMPLER(sampler_MetalMap);
TEXTURE2D(_OutlineMask);    SAMPLER(sampler_OutlineMask);
TEXTURE2D(_MaskMap);    SAMPLER(sampler_MaskMap);

TEXTURE2D(_OcclusionMap);       SAMPLER(sampler_OcclusionMap);
TEXTURE2D(_MetallicGlossMap);   SAMPLER(sampler_MetallicGlossMap);
TEXTURE2D(_SpecGlossMap);       SAMPLER(sampler_SpecGlossMap);
TEXTURE2D(_JitterMap);              SAMPLER(sampler_JitterMap);


#ifdef _SPECULAR_SETUP
#define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_SpecGlossMap, sampler_SpecGlossMap, uv)
#else
#define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, uv)
#endif

half4 SampleMetallicSpecGloss(float2 uv, half albedoAlpha)
{
    half4 specGloss;

#ifdef _METALLICSPECGLOSSMAP
    specGloss = SAMPLE_METALLICSPECULAR(uv);
#ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
    specGloss.a = albedoAlpha * _Smoothness;
#else
    specGloss.a *= _Smoothness;
#endif
#else // _METALLICSPECGLOSSMAP
#if _SPECULAR_SETUP
    specGloss.rgb = _SpecColor.rgb;
#else
    specGloss.rgb = _Metallic.rrr;
#endif

#ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
    specGloss.a = albedoAlpha * _Smoothness;
#else
    specGloss.a = _Smoothness;
#endif
#endif

    return specGloss;
}

half SampleOcclusion(float2 uv)
{
#ifdef _OCCLUSIONMAP
    // TODO: Controls things like these by exposing SHADER_QUALITY levels (low, medium, high)
#if defined(SHADER_API_GLES)
    return SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
#else
    half occ = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
    return LerpWhiteTo(occ, _OcclusionStrength);
#endif
#else
    return 1.0;
#endif
}

inline void InitializeStandardLitSurfaceData(float2 uv, out SurfaceData outSurfaceData, float4 screenPosData)
{
    half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    outSurfaceData.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);

    half4 specGloss = SampleMetallicSpecGloss(uv, albedoAlpha.a);
    outSurfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;

#if _SPECULAR_SETUP
    outSurfaceData.metallic = 1.0h;
    outSurfaceData.specular = specGloss.rgb;
#else
    outSurfaceData.metallic = specGloss.r;
    outSurfaceData.specular = half3(0.0h, 0.0h, 0.0h);
#endif

    outSurfaceData.smoothness = specGloss.a;
    outSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
    outSurfaceData.occlusion = SampleOcclusion(uv);
    outSurfaceData.emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
}

#endif // UNIVERSAL_INPUT_SURFACE_PBR_INCLUDED
