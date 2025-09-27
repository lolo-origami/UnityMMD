#include "LLToonInput.hlsl"

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
    return pow(sm, powK);
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

float3 ScreenBlend(float3 baseCol, float3 addCol, float m)
{
    float3 b = lerp(0, addCol, m);
    return 1.0 - (1.0 - baseCol) * (1.0 - saturate(b));
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

half3 ApplyFaceDetail(float3 baseColor, float2 uv)
{
/*#ifdef ENABLE_FACE_CHEEK
    FaceDetailMasks M = SampleFaceMasks(uv);

    // --- Cheek ---
    float mCheek = SoftMask(M.cheek, _CheekSoft, _CheekPow);
    baseColor = ScreenBlend(baseColor, _CheekColor.rgb, mCheek * _CheekAdd);
    baseColor = MultiplyShadow(baseColor, _CheekColor.rgb * 0.85 + 0.15, mCheek * _CheekMul);

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
#endif
*/
    return half3(0,0,0);
}
