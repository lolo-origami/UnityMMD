Shader "Hidden/DepthFogShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Common Bloom Factor", range(0.0, 1.0)) = 1.0
        _FogColor ("_FogColor", Color) = (1, 1, 1, 1)
    }
    
HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

CBUFFER_START(UnityPerMaterial)
    TEXTURE2D_X(_MainTex);
    TEXTURE2D_X(_CameraDepthTexture);

    TEXTURE2D_X(_RampTex);
    float _Intensity;
    float4 _FogColor;
CBUFFER_END

    half4 FragDepth(Varyings input) : SV_Target
    {
        //カラーを取得
        half4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp , input.texcoord) * _FogColor;
        
        //デプスを取得
        float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_LinearClamp, input.texcoord).r;
        
        //リニア以外あってもいいかも
        depth = Linear01Depth(depth, _ZBufferParams);
        
        //RampTextureからdepth値を使ってサンプリング
        half4 ramp = SAMPLE_TEXTURE2D_X(_RampTex, sampler_LinearClamp, float2(1, 0));
        color.rgb = lerp(color.rgb, _FogColor.rgb, _Intensity * ramp);
        
        return color;
    }

ENDHLSL
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZTest Always ZWrite Off Cull Off
        
        Pass
        {
            Name "DepthFog"

            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment FragDepth
            ENDHLSL
        }
    }
}
