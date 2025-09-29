#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/BRDF.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/AmbientOcclusion.hlsl"


#include "LLToonInput.hlsl"
#include "LLToonUnique.hlsl"

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
    specularHigh = ApplyTightsSpecular(llToonInputData, uv, finalSpecularRadiance);
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