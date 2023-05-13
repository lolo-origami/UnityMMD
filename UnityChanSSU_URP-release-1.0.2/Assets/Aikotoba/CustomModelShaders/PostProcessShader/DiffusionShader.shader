Shader "Hidden/DiffusionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Contrast ("Contrast", range(0.0, 1.0)) = 0.0
        _Intensity ("Intensity", range(0.0, 1.0)) = 0.0
    }
    SubShader
    {

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

        CBUFFER_START(UnityPerMaterial)
            TEXTURE2D_X(_MainTex);
            float4 _MainTex_TexelSize;

            TEXTURE2D_X(_BlurTex);

            float _Contrast;
            float _Intensity;
        CBUFFER_END

        static const float Weights[9] = {0.5352615, 0.7035879, 0.8553453, 0.9616906, 1, 0.9616906, 0.8553453, 0.7035879, 0.5352615};

        float3 Contrast(float3 In, float Contrast)
        {
            const float midpoint = pow(0.5, 2.2);
            return (In - midpoint) * Contrast + midpoint;
        }

        half4 Frag_Contrast(Varyings input) : SV_Target
        {
            half4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp , input.uv);

            color.rgb = Contrast(color.rgb, _Contrast);
            
            return color;
        }

        //横ぼかし
        half4 Frag_Blur1(Varyings input) : SV_Target
        {
            half4 color = 0;

            float totalWeight = 0;
            
                
            totalWeight += Weights[0];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(0 * _MainTex_TexelSize.x, 0, 0, 0)) * Weights[0];
                
            totalWeight += Weights[1];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(1 * _MainTex_TexelSize.x, 0, 0, 0)) * Weights[1];

            totalWeight += Weights[2];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(2 * _MainTex_TexelSize.x, 0, 0, 0)) * Weights[2];

            totalWeight += Weights[3];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(3 * _MainTex_TexelSize.x, 0, 0, 0)) * Weights[3];

            totalWeight += Weights[4];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(4 * _MainTex_TexelSize.x, 0, 0, 0)) * Weights[4];

            totalWeight += Weights[5];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(5 * _MainTex_TexelSize.x, 0, 0, 0)) * Weights[5];

            totalWeight += Weights[6];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(6 * _MainTex_TexelSize.x, 0, 0, 0)) * Weights[6];

            totalWeight += Weights[7];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(7 * _MainTex_TexelSize.x, 0, 0, 0)) * Weights[7];                

            totalWeight += Weights[8];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(8 * _MainTex_TexelSize.x, 0, 0, 0)) * Weights[8];          
            

            color /= totalWeight;
            
            return color;
        }

        //縦ぼかし
        half4 Frag_Blur2(Varyings input) : SV_Target
        {
            half4 color = 0;

            float totalWeight = 0;
            
            totalWeight += Weights[0];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(0, 0 * _MainTex_TexelSize.y, 0, 0)) * Weights[0];
            totalWeight += Weights[1];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(0, 1 * _MainTex_TexelSize.y, 0, 0)) * Weights[1];
            totalWeight += Weights[2];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(0, 2 * _MainTex_TexelSize.y, 0, 0)) * Weights[2];
            totalWeight += Weights[3];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(0, 3 * _MainTex_TexelSize.y, 0, 0)) * Weights[3];
            totalWeight += Weights[4];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(0, 4 * _MainTex_TexelSize.y, 0, 0)) * Weights[4];
            totalWeight += Weights[5];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(0, 5 * _MainTex_TexelSize.y, 0, 0)) * Weights[5];
            totalWeight += Weights[6];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(0, 6 * _MainTex_TexelSize.y, 0, 0)) * Weights[6];
            totalWeight += Weights[7];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(0, 7 * _MainTex_TexelSize.y, 0, 0)) * Weights[7];
            totalWeight += Weights[8];
            color += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv + float4(0, 8 * _MainTex_TexelSize.y, 0, 0)) * Weights[8];

            color /= totalWeight;
            
            return color;
        }

        //合成
        half4 Frag_Blend(Varyings input) : SV_Target
        {
            half4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp , input.uv);
            half4 blur = SAMPLE_TEXTURE2D_X(_BlurTex, sampler_LinearClamp, input.uv);

              color.rgb = 1.0 - (1.0 - color.rgb) * (1.0 - blur.rgb * _Intensity);
            // color.rgb = lerp(color.rgb, blur.rgb, _Intensity);
            
            return color;
        }

    ENDHLSL
        
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZTest Always ZWrite Off Cull Off
        
        Pass // 0
        {
            Name "Contrast"

            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag_Contrast
            ENDHLSL
        }
        
        Pass // 1
        {
            Name "Blur1"
            
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag_Blur1
            ENDHLSL
        }     
           
        Pass // 2
        {
            Name "Blur2"
            
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag_Blur2
            ENDHLSL
        }
        
        Pass // 3
        {
            Name "Blend"
            
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag_Blend
            ENDHLSL
        }
    }
}
