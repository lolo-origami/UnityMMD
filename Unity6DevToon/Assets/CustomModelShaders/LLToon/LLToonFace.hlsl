#include "LLToonInput.hlsl"
#include "../PostProcessShader/LLToonBlend.hlsl"

// =====================
// 顔ディテール合成
// =====================

struct FaceDetailMasks
{
    float cheek;  // R
    float noseS;  // G
    float brow;   // B
    float lid;    // A
};

float SoftMask(float m, float soft, float powK)
{
    m = saturate(m);
    float sm = smoothstep(0.0, max(1e-3, soft), m);
    return pow(sm, powK) * 0.5;
}

float MaskToLine(float m, float widthPow, float soft)
{
    return SoftMask(m, soft, widthPow);
}

float NoseHighlightFromMask(float m, float width)
{
    float t = saturate((m - (1.0 - 1.0/width)) * width);
    return pow(saturate(t), 4.0);
}

float3 MultiplyShadow(float3 baseCol, float3 shadowCol, float m)
{
    return lerp(baseCol, baseCol * shadowCol, m);
}

FaceDetailMasks SampleFaceMasks(float2 uv)
{
    float4 m = SAMPLE_TEXTURE2D(_FaceMask, sampler_FaceMask, uv);
    FaceDetailMasks r;
    r.cheek = m.r;
    r.noseS = m.g;
    r.brow  = m.b;
    r.lid   = m.a;
    return r;
}

half3 ApplyFaceDetail(half3 baseColor, float2 uv)
{
    FaceDetailMasks M = SampleFaceMasks(uv);

    // --- Cheek ---
    float mCheek = SoftMask(M.cheek, _CheekSoft, _CheekPow);
    half3 faceColor = lerp(baseColor, _CheekColor.rgb, mCheek * _CheekColor.a);

    /*
    // --- Brow Shadow ---
    float mBrow = SoftMask(M.brow, _BrowSoft, _BrowPow);
    baseColor = MultiplyShadow(baseColor, _BrowColor.rgb, mBrow * _BrowStrength);

    // --- Nose Shadow ---
    float mNoseS = SoftMask(M.noseS, _NoseShadowSoft, _NoseShadowPow);
    baseColor = MultiplyShadow(baseColor, _NoseShadowColor.rgb, mNoseS * _NoseShadowStrength);

    // --- Nose Highlight ---
    float mNoseHL = NoseHighlightFromMask(M.noseS, _NoseHLWidth);
    baseColor += _NoseHLColor.rgb * (mNoseHL * _NoseHLStrength);

    // --- Philtrum Dot ---
    float phil = saturate(pow(mNoseS, 6.0)) * _PhiltrumStrength;
    baseColor = MultiplyShadow(baseColor, float3(0.9,0.9,0.9), phil);

    // --- Lower Lid ---
    float mLid = MaskToLine(M.lid, _LidPow, _LidSoft);
    baseColor = MultiplyShadow(baseColor, _LidColor.rgb, mLid * _LidStrength);
    */
    return faceColor;
}
