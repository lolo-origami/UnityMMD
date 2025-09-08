Shader "Hidden/LightShaftShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"         

    CBUFFER_START(UnityPerMaterial)
    TEXTURE2D_X(_LightShaftTempTex);

    float4 _CamWorldSpace;
    float4x4 _CamFrustum, _CamToWorld;
    int _MaxIterations;
    float _MaxDistance;
    float _MinDistance;

    float _Intensity;

    CBUFFER_END

    struct RayAttributes
    {
        float3 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
    };

    struct RayVaryings
    {
        float4 positionCS : SV_POSITION;
        float2 uv         : TEXCOORD0;
        float4 ray        : TEXCOORD1;
    };

    ENDHLSL
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "SunShaft"

            HLSLPROGRAM

            RayVaryings VertRay(RayAttributes input)
            {
                RayVaryings output;
                output.positionCS = float4(input.positionOS.xy, 0.0, 1.0);
                output.uv = input.uv;

                // Frustum index を uv の 0/1 判定で安全に決定
                int ix = (output.uv.x > 0.5) ? 1 : 0;
                int iy = (output.uv.y > 0.5) ? 2 : 0;
                int index = ix + iy;

                output.ray = _CamFrustum[index];
                return output;
            }

            float GetRandomNumber(float2 texCoord, int Seed)
            {
                return frac(sin(dot(texCoord.xy, float2(12.9898, 78.233)) + Seed) * 43758.5453);
            }

            half4 SimpleRaymarching(float3 rayOrigin, float3 rayDirection, float depth)
            {
                half4 result = float4(_MainLightColor.xyz, 1) * _Intensity;

                float step = _MaxDistance / max(1, _MaxIterations);
                float t = _MinDistance + step * GetRandomNumber(rayDirection.xy, (int)(_Time.y * 100));
                float alpha = 0;

                [loop]
                for (int i = 0; i < _MaxIterations; i++)
                {
                    if (t > _MaxDistance || t >= depth) break;

                    float3 p = rayOrigin + rayDirection * t;

                    #ifdef _MAIN_LIGHT_SHADOWS
                    float4 shadowCoord = TransformWorldToShadowCoord(p);
                    float shadow = SAMPLE_TEXTURE2D_SHADOW(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, shadowCoord);
                    if (shadow >= 1) alpha += step * 0.2;
                    #endif

                    t += step;
                }

                result.a *= saturate(alpha);
                return result;
            }

            half4 Frag(RayVaryings input) : SV_Target
            {
                float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_LinearClamp, input.uv).r;
                depth = Linear01Depth(depth, _ZBufferParams);
                depth *= length(input.ray);

                float3 rayOrigin = _CamWorldSpace.xyz;
                float3 rayDir = normalize(input.ray.xyz);
                float4 result = SimpleRaymarching(rayOrigin, rayDir, depth);

                return result;
            }

            #pragma vertex VertRay
            #pragma fragment Frag
            ENDHLSL
        }

        Pass
        {
            Name "Combine"

            HLSLPROGRAM

            half4 Frag_Combine(Varyings input) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord);
                half4 shaft = SAMPLE_TEXTURE2D_X(_LightShaftTempTex, sampler_LinearClamp, input.texcoord);

                color.rgb = color.rgb * (1 - shaft.a) + shaft.rgb * shaft.a;
                return color;
            }

            #pragma vertex Vert
            #pragma fragment Frag_Combine
            ENDHLSL
        }
    }
}
