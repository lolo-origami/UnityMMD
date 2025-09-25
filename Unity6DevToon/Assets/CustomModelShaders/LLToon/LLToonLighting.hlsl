#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/BRDF.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/AmbientOcclusion.hlsl"


#include "LLToonInput.hlsl"
#include "LLToonNoise.hlsl"

#if defined(LIGHTMAP_ON)
    #define DECLARE_LIGHTMAP_OR_SH(lmName, shName, index) float2 lmName : TEXCOORD##index
    #define OUTPUT_LIGHTMAP_UV(lightmapUV, lightmapScaleOffset, OUT) OUT.xy = lightmapUV.xy * lightmapScaleOffset.xy + lightmapScaleOffset.zw;
    #define OUTPUT_SH(normalWS, OUT)
#else
    #define DECLARE_LIGHTMAP_OR_SH(lmName, shName, index) half3 shName : TEXCOORD##index
    #define OUTPUT_LIGHTMAP_UV(lightmapUV, lightmapScaleOffset, OUT)
    #define OUTPUT_SH(normalWS, OUT) OUT.xyz = SampleSHVertex(normalWS)
#endif

//それぞれのライティング色
struct LLLightingData
{
    half4 BaseToonLightingColor; //基本のToonライティング色 + Specular
    half4 AdditionalLightsColor; //追加光
    half4 RimColor; //リムライト
    half4 DarkRimColor; //暗部の反射光
    half4 EmissionColor; //Emission
    half4 GIColor; //GI
    half4 SpecRimEmission; //全体的な輝きコントロール
    half RampOutline; //2影の境界線調整値(メインで計算後アウトラインにに使いまわす)
    half HalfLambert;
};
            
//Toonに必要な要素
struct ToonShadowFactor
{
    float SWeight; //影範囲
    float SFactor; //1影の塗分け範囲
    float SFactorD; //影の塗分け範囲
    half rampS; //1影の境界線調整値
    half rampDS; //2影の境界線調整値
    float HalfLambert; //Halflambert情報
};

//Toonに必要な要素
struct RimFactor
{
    half4 RimColor; //リムライト
    half4 DarkRimColor; //暗部の反射光
};

//LLToonに必要な入力要素
struct LLToonInputData
{
    InputData baseInputData;
    float3 binormal;
    float3 normalOS;
    half4 color;
    float2 matcapUV;
};

// 特定方向からの簡易シャドウ
inline float LLToonFixedDirShadow(float3 nWS, float3 fixedDirWS, float area, float smooth, float strength)
{
    float d = dot(normalize(nWS), normalize(fixedDirWS));   // 法線と固定方向の角度
    float v = saturate(-d * 0.5 + 0.5);                     // 0=同方向, 1=逆方向
    float t = smoothstep(saturate(area - smooth), saturate(area + smooth), v);
    return lerp(1.0, 1.0 - strength, t);
}

//ライト情報からToonシェードに必要な情報を取得しておく
ToonShadowFactor CalculateToonShadowFactor(
    Light light, //ライト情報
    float3 normalWS, //ノーマル情報
    float mask, //ライトマップマスク(?)
    float mainLightShadowArea, //落ち影情報
    /*float2 uv,*/
    float3 pos
    )

{
    ToonShadowFactor tsf;
                
    float3 lightDirWS = normalize(light.direction.xyz);
#if ENABLE_FACE_CHEEK
    lightDirWS = normalize(light.direction.xyz + pos); //顔は常に一定の位置から照らす
#endif
    // ★ 固定方向影が有効ならライト方向を上書き
    if (_EnableFixedDirShadow > 0.5)
    {
        lightDirWS = normalize(mul((float3x3)unity_ObjectToWorld, _FixedDirOS.xyz));
    }
    float3 fixedlightDirWS = normalize(float3(lightDirWS.x, _FixLightY, lightDirWS.z));
    lightDirWS = _IgnoreLightY ? fixedlightDirWS: lightDirWS;

    tsf.HalfLambert = light.distanceAttenuation * dot(normalWS, lightDirWS) * 0.5f + 0.5f;
/*#if ENABLE_FACE_CHEEK
    float t = (SAMPLE_TEXTURE2D(_MaskMap2, sampler_MaskMap2, uv).b - 1.0);
    float3 InverseXNormalTex = (SAMPLE_TEXTURE2D(_MaskMap2, sampler_MaskMap2, uv).b - 1.0) * float3(-1, 1, 1);
    normalWS = lerp(normalWS, InverseXNormalTex, t);
    float originHalfLambert = tsf.HalfLambert;
    float calcHalfLambert = light.distanceAttenuation * dot(normalWS, lightDirWS) * 0.5f + 0.5f;
    tsf.HalfLambert = max(originHalfLambert, calcHalfLambert);
#endif
*/    
    
    //tsf.Lambert *= alwaysShadow;
    
    //影範囲の決定
    tsf.SWeight = tsf.HalfLambert * 0.5  + 1.125;
    
    half enableDarkShadowFloat = _EnableDarkShadow;
    half darkShadowArea = _DarkShadowArea * enableDarkShadowFloat;
    half darkShadowSmooth = _DarkShadowSmooth * enableDarkShadowFloat;

#if ENABLE_INVERSE_SHADOW //Inverseの時は逆転させとく(分かりやすく)
    darkShadowArea = 1.0 - darkShadowArea;
#endif    
    //影を塗り分ける範囲
    tsf.SFactor = floor(tsf.SWeight - _ShadowArea);
    tsf.SFactorD = floor(tsf.SWeight - darkShadowArea);

    // 境界線の調整
#if ENABLE_CHARA_ON_SHADOW
    tsf.rampS = smoothstep(0, _ShadowSmooth, (tsf.HalfLambert - _ShadowArea));
    tsf.rampDS = smoothstep(0, darkShadowSmooth, (tsf.HalfLambert - darkShadowArea));
#else    
    tsf.rampS = smoothstep(0, _ShadowSmooth, (tsf.HalfLambert - _ShadowArea) * mainLightShadowArea * mask);
    tsf.rampDS = smoothstep(0, darkShadowSmooth, (tsf.HalfLambert - clamp(darkShadowArea, 0, _ShadowArea)) * mainLightShadowArea * mask);
#endif

#if ENABLE_INVERSE_SHADOW
    //逆から反射光を入れたいとき
    tsf.rampDS = (clamp((tsf.rampDS - tsf.rampS), 0.0, 1.0)) * _EnableDarkShadow;
    tsf.rampDS = (1.0 - tsf.rampDS) * _EnableDarkShadow;
#endif    
    
    return tsf;
}

//タイツ計算
half3 ApplyTightsBase(float3 skinBase, float3 normalWS, float3 viewDirWS)
{
    //フレネルで外側だけ厚くする
    float NdotV   = saturate(dot(normalize(normalWS), normalize(viewDirWS)));
    float fresnel = pow(1.0 - NdotV, _TightsFresnelPow);
    float a       = saturate(fresnel * _TightsFresnelStrength);
    float alpha   = lerp(_TightsBaseAlpha, _TightsMaxAlpha, a);

    // 透け＝肌と布色の単純 lerp
    return lerp(skinBase, _TightsColor.rgb, alpha);
}

//Toonシェーディング + スぺキュラ
half4 ToonBaseLighting(
    float4 baseColor, //ベース色
    half3 shadowColor, //1影色
    half3 darkShadowColor, //2影色
    ToonShadowFactor toonShadowFactor, //Toonシェーディングする用の情報
    out half3 DarkShadowColor
    )
{
    half4 baseToonLightingColor = float4(0,0,0,1);
    
    //影を塗り分ける
    half3 ShallowShadowColor = toonShadowFactor.SFactor * baseColor.rgb + (1 - toonShadowFactor.SFactor) * shadowColor.rgb;
    ShallowShadowColor.rgb = lerp(shadowColor, baseColor.rgb, toonShadowFactor.rampS);

    // 境界線の調整
    half3 tempDarkShadowColor = toonShadowFactor.SFactorD * ShallowShadowColor + (1 - toonShadowFactor.SFactorD) * darkShadowColor;
    tempDarkShadowColor.rgb = lerp(darkShadowColor, ShallowShadowColor, toonShadowFactor.rampDS);

    // _EnableDarkShadowの値を使って、DarkShadowColorをShallowShadowColorか、2影の色に切り替える
    DarkShadowColor = lerp(ShallowShadowColor, tempDarkShadowColor, _EnableDarkShadow);

    baseToonLightingColor.rgb = DarkShadowColor.rgb;
    
    return baseToonLightingColor * _DiffuseIntensity;
}

half4 ToonBaseLightingAdd(
    float4 baseColor, //ベース色
    half3 shadowColor, //1影色
    half3 darkShadowColor, //2影色
    ToonShadowFactor toonShadowFactor,
    float lightMapMask
    )
{
    half3 dark;
    return ToonBaseLighting(baseColor, shadowColor, darkShadowColor, toonShadowFactor, dark);
}

//Specular計算
half4 LLToonSpecularLighting(
    BRDFData brdfData,
    LLToonInputData llToonInputData,
    float specularMask,
    float specularMaskHigh,
    Light light,
    bool chara,
    float rampS,
    half specularRadiance,
    float2 uv
    )
{
    half finalSpecularRadiance = specularRadiance <= 0.5 ? 0.5 : specularRadiance; //影でも0.1以下にはしない

    half calRadianceHair = (specularRadiance * 1.75) * (specularRadiance * 1.75);
    half finalSpecularHairRadiance = calRadianceHair <= 0.4 ? 0.4 : calRadianceHair; //影でも0.1以下にはしない
    half4 finalspecularColor = half4(0,0,0,0);
    //SpecDiffuse.rgb *= _BaseColor.rgb;
    
#if ENABLE_HAIR_SPECULAR    
    float3 halfDir = normalize(light.direction + llToonInputData.baseInputData.viewDirectionWS);
    float3 tmpBinormal = normalize(llToonInputData.binormal - llToonInputData.baseInputData.normalWS * llToonInputData.baseInputData.positionWS);
    float dotTH = dot(tmpBinormal, halfDir);
    float sintTH = sqrt(1.0 - dotTH * dotTH);
    float dirAtten = smoothstep(-1.0, 0.0, dotTH);
    half specular = dirAtten * pow(sintTH, _Sharpness) * specularMask;
    finalspecularColor = (_LightSpecColor * specular * rampS * _SpecularIntensity + (1 - rampS) * _LightSpecShadowColor * specular * _SpecularIntensityShadow) * finalSpecularHairRadiance;
    
    /*float3 dotTH = dot(normalize(llToonInputData.baseInputData.normalWS), normalize(llToonInputData.baseInputData.viewDirectionWS));
    float anisoHairFrenel = pow((1.0 - saturate(dotTH)), _Sharpness) * _SpecularIntensity;
    float anisoHair = saturate(1.0 - anisoHairFrenel) * specularMask * dot(light.direction, llToonInputData.baseInputData.normalWS);
    finalspecularColor = _LightSpecColor * anisoHair;
    */
    
#else
    half directSpecular = DirectBRDFSpecular(brdfData, llToonInputData.baseInputData.normalWS, light.direction, llToonInputData.baseInputData.viewDirectionWS);
    half surfaceSpecular = chara ? 1 : brdfData.specular; //キャラのマテリアルは少し強めにspecular出したい(マスクはここでやってるし)
    half specular = surfaceSpecular * directSpecular  * specularMask * _SpecularIntensity;
    half specularHigh = surfaceSpecular * directSpecular * specularMaskHigh * _SpecularIntensityHigh;
    finalSpecularRadiance = pow(saturate(finalSpecularRadiance), _SpecContrast);
    
#if ENABLE_TIGHTS
    // -------------------------
    // 正面ハイライト(線) × 高さプロファイル
    // -------------------------
    float3 viewDir = normalize(llToonInputData.baseInputData.viewDirectionWS);
    float3 normal  = normalize(llToonInputData.baseInputData.normalWS);
    float facing   = saturate(dot(viewDir, normal));
    // 線っぽさ：ここで形状は完成
    float baseline = pow(facing, _TightsHighlightScale);

    // 太もも範囲の0..1正規化
    float h = saturate((uv.y - _TightsThighStart) / max(1e-5, (_TightsThighEnd - _TightsThighStart)));

    // 「どの高さで最強にするか」を制御（中心=_SpecThreshold, 半幅=_SpecWidth）
    // 三角～ベル型の高さマスク。_SpecContrastでシャープさ可変。
    float band = 1.0 - abs(h - _TightsSpecThreshold) / max(1e-5, _TightsSpecWidth); // 中心1→端0
    float heightMask = pow(saturate(band), _SpecContrast);
    
    // ファイバーノイズ
    float fiberNoise = AnisotropicNoise(
        uv,
        llToonInputData.baseInputData.positionWS,
        unity_WorldToObject,
        _TightsNoiseDir,
        _TightsNoiseTex,
        sampler_TightsNoiseTex,
        _TightsNoiseScale,
        _TightsNoiseSharpness,
        _TightsNoiseJitter,
        _UseTightsNoiseTex
    );
    
    // Radiance によるスケール（弱光でも完全には消さない）
    float radianceScale = lerp(0.3, 1.0, saturate(finalSpecularRadiance));

    // 線×高さプロファイル×強度
    float highFactor = baseline * heightMask * _TightsHighlightIntensity;
    highFactor *= (1.0 + fiberNoise * _TightsSpecContrast);

    // specularHigh は残すが最終合成で強制加算する
    specularHigh = highFactor * radianceScale;
#endif    
    
    float4 specColor = lerp(_LightSpecShadowColor, _LightSpecColor, specularRadiance);    
    finalspecularColor = specColor * (specular + specularHigh) * finalSpecularRadiance;

#if ENABLE_TIGHTS
    // 正面ラインを強制加算
    finalspecularColor = specColor * specular * finalSpecularRadiance;
    finalspecularColor.rgb = specColor * specularHigh;
#endif
    
#endif
    finalspecularColor.a = finalspecularColor.a * _BloomFactor;
    
    return finalspecularColor;
}

/**追加光はURPの追加光計算そのまま→Toonはメインライトで作る
 * \brief 
 * \param brdfData
 * \param llTInput 
 * \param surfaceData 
 * \param meshRenderingLayers 
 * \return 
 */
half4 LLToonAddLighting(
    BRDFData brdfData,
    LLToonInputData llTInput,
    SurfaceData surfaceData,
    uint meshRenderingLayers
    )
{
    //追加光情報
    uint pixelLightCount = GetAdditionalLightsCount();
    half4 addLightColor = half4(0,0,0,0);

    #if USE_CLUSTERED_LIGHTING
    for (uint lightIndex = 0; lightIndex < min(_AdditionalLightsDirectionalCount, MAX_VISIBLE_LIGHTS); lightIndex++)
    {
        Light light = GetAdditionalLight(lightIndex, llTInput.baseInputData.positionWS);
#ifdef _LIGHT_LAYERS 
        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
#endif    
        {
            addLightColor.rgb += LightingPhysicallyBased(brdfData, brdfData, light,
                                                                  llTInput.baseInputData.normalWS, llTInput.baseInputData.viewDirectionWS,
                                                                  surfaceData.clearCoatMask, true);
        }
    }
    #endif

    LIGHT_LOOP_BEGIN(pixelLightCount)
        Light light = GetAdditionalLight(lightIndex, llTInput.baseInputData.positionWS);
#ifdef _LIGHT_LAYERS    
    if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
#endif        
    {
        addLightColor.rgb += LightingPhysicallyBased(brdfData, brdfData, light,
                                                              llTInput.baseInputData.normalWS, llTInput.baseInputData.viewDirectionWS,
                                                              surfaceData.clearCoatMask, true);
    }
    LIGHT_LOOP_END
    return addLightColor;
}

// ============================================================
// LLToonAddLightingSimple
// ============================================================

half4 LLToonAddLightingSimple(LLToonInputData inputData, float2 uv)
{
    half3 N = normalize(inputData.baseInputData.normalWS);
    float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv) * _BaseColor;

    half3 result = 0;

    uint count = GetAdditionalLightsCount();
    [loop] for (uint i = 0u; i < count; i++)
    {
        Light light = GetAdditionalLight(i, inputData.baseInputData.positionWS);
        #ifdef _LIGHT_LAYERS
        if (!IsMatchingLightLayer(light.layerMask, meshLayers))
            continue;
        #endif

        float ndl = saturate(dot(N, -light.direction));
        ToonShadowFactor tsf = CalculateToonShadowFactor(light, N, 1.0, light.shadowAttenuation, inputData.baseInputData.positionWS);

        // トゥーン影付きライティング（Simple: Specular抜き）
        half4 toonLit = ToonBaseLighting(baseColor, baseColor.rgb * _ShadowMultColor.rgb, baseColor.rgb * _DarkShadowMultColor.rgb, tsf, baseColor.rgb);

        result += toonLit.rgb * light.color.rgb * light.distanceAttenuation * light.shadowAttenuation;
    }

    return half4(result, 0);
}


//リムライト
RimFactor LLToonRimLighting(InputData inputData, float2 uv, float lambert, half4 baseColor)
{
    RimFactor Rim;

    float rim = 1 - saturate(dot(inputData.viewDirectionWS, inputData.normalWS));
    float rimMask = SAMPLE_TEXTURE2D(_MaskMap2, sampler_MaskMap2, uv).r;
    float rimDot = pow(rim, _RimPow) * rimMask;
    rimDot = _EnableLambert * lambert * rimDot + (1 - _EnableLambert) * rimDot;
    float rimIntensity = smoothstep(0, _RimSmooth, rimDot);
    half4 rimColor = lerp(_RimColor, _RimColor * baseColor, _BlendRimWithBaseColor);
    Rim.RimColor = _EnableRim * pow(rimIntensity, 5) * rimColor;
    Rim.RimColor.a = _EnableRim * rimIntensity * _BloomFactor;

    float darkRimDot = pow(rim, _DarkSideRimPow) * rimMask;
    darkRimDot = _EnableLambert * (1 - lambert) * darkRimDot + (1 - _EnableLambert) * darkRimDot;
    float darkRimIntensity = smoothstep(0, _DarkSideRimSmooth, darkRimDot);
    half4 darkRimColor = lerp(_RimColor, _RimColor * baseColor, _BlendRimWithBaseColor);
    Rim.DarkRimColor = _EnableRimDS * pow(darkRimIntensity, 5) * darkRimColor;
    Rim.DarkRimColor.a = _EnableRimDS * darkRimIntensity * _BloomFactor;

    return Rim;
}

//Emission
half4 LLToonEmission(float2 uv, half4 baseLightingColor, half3 darkShadowColor, float baseAlpha, half emissionMask)
{
    half4 EmissionColor = half4(0,0,0,0);
    EmissionColor.rgb = (_Emission * darkShadowColor * _EmissionColor.rgb * emissionMask - baseLightingColor.rgb * emissionMask) * _EnableEmission;
    EmissionColor.a = _EmissionBloomFactor * baseAlpha * emissionMask * _EnableEmission;
    return EmissionColor;
}

//輝き調整
half4 LLToonBloom(half baseLightingColorAlpha,half rimAlpha, half3 darkShadowColor, half emissionMask)
{
    half4 bloom = half4(0,0,0,0);
    // 暗影色ベースで光色を作る
    bloom.rgb = pow(darkShadowColor, _DarkEmissionIntensity) * _Emission * emissionMask;

    // 強度は Base と Rim の寄与度をまとめる
    bloom.a = (baseLightingColorAlpha + rimAlpha/* + RimDS.a*/);

    // 最終的な輝き調整を BloomFactor に一任
    bloom.rgb *= _BloomFactor;
    bloom.a   *= _BloomFactor;
    
    return bloom * _EnableEmission;
}

// --- マスク情報まとめ
struct LLMaskData
{
    float lightMapMask;
    float specularMask;
    float emissionMask;
    float GIOffMapMask;
    float specularMaskHigh;
};

inline LLMaskData SampleLLMask(float2 uv)
{
    LLMaskData m;
    float4 mask1 = SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, uv);
    float4 mask2 = SAMPLE_TEXTURE2D(_MaskMap2, sampler_MaskMap2, uv);

    m.lightMapMask    = mask1.r;
    m.specularMask    = mask1.g;
    m.emissionMask    = mask1.b;
    m.GIOffMapMask    = mask2.g;
    m.specularMaskHigh= mask2.b;
    return m;
}

// --- BaseColor
inline float4 SampleLLBaseColor(float2 uv, float3 normalWS, float3 viewDirWS)
{
    float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv) * _BaseColor;
    #if defined(ENABLE_TIGHTS)
    baseColor.rgb = ApplyTightsBase(baseColor, normalWS, viewDirWS);
    #endif
    return baseColor;
}

// --- GI計算
inline float3 CalculateLLGI(LLToonInputData inputData, BRDFData brdfData, BRDFData brdfDataClearCoat,
                            SurfaceData surfaceData, Light mainLight, float maskGI)
{
    AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData.baseInputData, surfaceData);
    aoFactor.indirectAmbientOcclusion = lerp(1.0h, aoFactor.indirectAmbientOcclusion, _AOStrength);

    MixRealtimeAndBakedGI(mainLight, inputData.baseInputData.normalWS, inputData.baseInputData.bakedGI);

    return GlobalIllumination(brdfData, brdfDataClearCoat, surfaceData.clearCoatMask,
                              inputData.baseInputData.bakedGI, aoFactor.indirectAmbientOcclusion,
                              inputData.baseInputData.positionWS,
                              inputData.baseInputData.normalWS,
                              inputData.baseInputData.viewDirectionWS) * maskGI;
}

// ============================================================
// Simple GI : bakedGI(SH or Lightmap) のみ
// ============================================================
inline float3 CalculateSimpleGI(Light mainLight, LLToonInputData inputData, float maskGI)
{
    MixRealtimeAndBakedGI(mainLight, inputData.baseInputData.normalWS, inputData.baseInputData.bakedGI);
    float ao = 1.0;
#if defined(_SCREEN_SPACE_OCCLUSION)
    float2 uv = inputData.baseInputData.normalizedScreenSpaceUV;
    ao = SAMPLE_TEXTURE2D_X(_ScreenSpaceOcclusionTexture, sampler_ScreenSpaceOcclusionTexture, uv).r;
#endif

    return inputData.baseInputData.bakedGI * ao * maskGI;
}

void LLToonLighting(
    LLToonInputData inputData,
    SurfaceData surfaceData,
    float2 uv,
    bool chara,
    out LLLightingData o,
    float4 screenPos)
{
    o = (LLLightingData)0;

    // --- マスク情報
    LLMaskData masks = SampleLLMask(uv);

    // --- BRDF
    BRDFData brdfData;
    InitializeBRDFData(surfaceData, brdfData);
    BRDFData brdfDataClearCoat = CreateClearCoatBRDFData(surfaceData, brdfData);

    // --- Debug override
    #if defined(DEBUG_DISPLAY)
    half4 debugColor;
    if (CanDebugOverrideOutputColor(inputData, surfaceData, brdfData, debugColor))
    {
        o.BaseToonLightingColor = debugColor;
        return;
    }
    #endif

    // --- Main Light
    half4 shadowMask = CalculateShadowMask(inputData.baseInputData);
    AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData.baseInputData, surfaceData);
    Light mainLight = GetMainLight(inputData.baseInputData, shadowMask, aoFactor);

    float mainLightShadowArea = _ReceiveShadows ? mainLight.shadowAttenuation : 1;
    half NdotL = saturate(dot(inputData.baseInputData.normalWS, mainLight.direction));
    half radianceBase = mainLightShadowArea * masks.lightMapMask * NdotL;
    half3 radiance = chara ? mainLight.color : radianceBase * mainLight.color;

    float maskGI = chara ? masks.GIOffMapMask : 1.0f;

    // --- GI
    o.GIColor.rgb = CalculateLLGI(inputData, brdfData, brdfDataClearCoat, surfaceData, mainLight, maskGI);
    o.GIColor.a = 0;

    // --- BaseColor
    float4 baseColor = SampleLLBaseColor(uv, inputData.baseInputData.normalWS, inputData.baseInputData.viewDirectionWS);

    // --- 影色情報
    float secondColorMask = 1.0 - SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, uv).a;
    half3 ShadowColor       = secondColorMask == 0 ? baseColor.rgb * _ShadowMultColor.rgb
                                                   : baseColor.rgb * _SceondMaterialShadowColor.rgb;
    half3 DarkShadowColorIn = secondColorMask == 0 ? baseColor.rgb * _DarkShadowMultColor.rgb
                                                   : baseColor.rgb * _SceondMaterialDarkShadowColor.rgb;

    // --- ToonShadowFactor
    ToonShadowFactor mainTSF = CalculateToonShadowFactor(mainLight, inputData.baseInputData.normalWS,
                                                         masks.lightMapMask, mainLightShadowArea * maskGI,
                                                         inputData.baseInputData.positionWS.xyz);
    o.RampOutline = mainTSF.rampS;
    o.HalfLambert = mainTSF.HalfLambert;

    // --- Base Toon Lighting
    half4 baseLightingColor = ToonBaseLighting(baseColor, ShadowColor, DarkShadowColorIn, mainTSF, baseColor.rgb);
#if ENABLE_SPECULAR
    // --- Specular
    baseLightingColor += LLToonSpecularLighting(brdfData, inputData, masks.specularMask, masks.specularMaskHigh,
                                                mainLight, chara, mainTSF.rampS * mainLightShadowArea,
                                                radianceBase, uv);
    
#endif    

    // --- MatCap
#if ENABLE_MATCAP_SPECULAR
    baseLightingColor += SAMPLE_TEXTURE2D(_MatCap, sampler_MatCap, inputData.matcapUV) * _MatCapIntensity;
#endif

    // --- ライト色適用
    baseLightingColor.rgb = lerp(baseLightingColor.rgb, radiance * baseLightingColor.rgb, _WorldLightInfluence);

    o.BaseToonLightingColor = baseLightingColor;

#if ENABLE_RIM
    // --- Rim Lighting
    RimFactor rim = LLToonRimLighting(inputData.baseInputData, uv, mainTSF.HalfLambert, baseColor);
    o.RimColor.rgb     = rim.RimColor.rgb     * _WorldLightInfluence;
    o.DarkRimColor.rgb = rim.DarkRimColor.rgb * _WorldLightInfluence;
#else
    o.RimColor.rgb     = 0;
    o.DarkRimColor.rgb = 0;
#endif
    
    // --- Additional Lights
    #if defined(_ADDITIONAL_LIGHTS)
    uint meshLayers = GetMeshRenderingLayer();
    o.AdditionalLightsColor = LLToonAddLighting(brdfData, inputData, surfaceData, meshLayers);
    #endif

    // --- Emission / Bloom
    o.EmissionColor   = LLToonEmission(uv, baseLightingColor, baseColor.rgb, baseColor.a, masks.emissionMask);
    o.SpecRimEmission = LLToonBloom(baseLightingColor.a, o.RimColor.a, baseColor.rgb, masks.emissionMask);
}
