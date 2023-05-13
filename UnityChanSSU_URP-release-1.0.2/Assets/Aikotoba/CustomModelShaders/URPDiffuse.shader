Shader "Universal Render Pipeline/URPDiffuse"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1,1,1,1)
        [Toggle] _ReceiveShadowEnable("Receive Shadow", int) = 1
        [Toggle] _HalfLambert("HalfLambert", int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode" = "UniversalForward"}
        LOD 100
        
        HLSLINCLUDE

        #include "UniversalToonInputCustom.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
        
        struct appdata
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : NORMAL;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 positionCS : SV_POSITION;
            float3 positionWS: TEXCOORD1;
            float3 positionVS: TEXCOORD2;
            float fogFactor : TEXCOORD3;
            float3 normal : NORMAL;
            float4 screenPos : TEXCOORD4;
        };

        v2f vert (appdata v)
        {
            v2f o;
            //変換
            VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
            o.positionWS = vertexInput.positionWS;
            o.positionVS = vertexInput.positionVS;
            o.positionCS = vertexInput.positionCS;
            o.screenPos = o.positionCS;
            o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
            o.normal = TransformObjectToWorldNormal(v.normal);
            o.fogFactor = ComputeFogFactor(o.positionWS.z);
            return o;
        }
        
        ENDHLSL

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            
            ENDHLSL
        }
        
        //シャドウマップに書く
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment FragmentAlphaClip
            half4 FragmentAlphaClip(v2f i): SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                #if ENABLE_ALPHA_CLIPPING
                    clip(SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv).b - _Cutoff);
                #endif
                return 0;
            }

            ENDHLSL
        }
        
        Pass
        {
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv) * _BaseColor;

                // ���C�g�����擾
                Light light = GetMainLight();
                //影を受ける
                float4 shadowCoord = TransformWorldToShadowCoord(i.positionWS);
                light.shadowAttenuation = MainLightRealtimeShadow(shadowCoord);
                float mainLightShadowArea = _ReceiveShadowEnable ? light.shadowAttenuation : 1;

                // �s�N�Z���̖@���ƃ��C�g�̕����̓��ς��v�Z����
                float t = dot(i.normal, light.direction) * light.shadowAttenuation;
                // ���ς̒l��0�ȏ�̒l�ɂ���
                t = max(0, t);
                t = _HalfLambert ? t * 0.5f + 0.5f : t;
                // �g�U���ˌ����v�Z����
                float3 diffuseLight = light.color * t;

                // �g�U���ˌ��𔽉f
                col.rgb *= diffuseLight;

                // apply fog
                //col.rgb = MixFog(col.rgb, i.fogFactor);
                return col;
            }
            ENDHLSL
        }
    }
}
