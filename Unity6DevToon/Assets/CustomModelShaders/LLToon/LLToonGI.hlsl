#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/BRDF.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/AmbientOcclusion.hlsl"


#include "LLToonInput.hlsl"

// --- GI計算
inline float3 CalculateLLGI(LLToonInputData inputData, BRDFData brdfData, BRDFData brdfDataClearCoat,
                            SurfaceData surfaceData, Light mainLight, float maskGI)
{
    AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData.baseInputData, surfaceData);
    aoFactor.indirectAmbientOcclusion = lerp(1.0h, aoFactor.indirectAmbientOcclusion, _AOStrength);

#if ENABLE_FLAT_GI    
    //位置だけで評価する
    half3 probe = SampleSH(normalize(inputData.baseInputData.positionWS));

    half3 l0 = half3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
    
    // 平均成分を弱める
    half3 env = probe - l0 * _FlatGIL0Minus;
    env = max(0, env);             // マイナスは切る
    return env * aoFactor.indirectAmbientOcclusion * maskGI;
#endif

    
    MixRealtimeAndBakedGI(mainLight, inputData.baseInputData.normalWS, inputData.baseInputData.bakedGI);

    return GlobalIllumination(brdfData, brdfDataClearCoat, surfaceData.clearCoatMask,
                              inputData.baseInputData.bakedGI, aoFactor.indirectAmbientOcclusion,
                              inputData.baseInputData.positionWS,
                              inputData.baseInputData.normalWS,
                              inputData.baseInputData.viewDirectionWS) * maskGI;
                              
                              
}