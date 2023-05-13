Shader "Universal Render Pipeline/URPGenToon"
{
    Properties
    {
        [Header(MainTex)]
        [Space(5)]
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1,1,1,1)
        _WorldLightInfluence ("World Light Influence", range(0.0, 1.0)) = 0.1
        _MaskMap ("Mask Texture", 2D) = "white" { } //r.Rim g.specularMap r.emission a.secondMaterialMap
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        
        [Space(30)]

        [Header(Bloom)]
        [Space(5)]
        _BloomFactor ("Common Bloom Factor", range(0.0, 1.0)) = 1.0
        
        [Header(Emission)]
        [Toggle]_EnableEmission ("Enable Emission", Float) = 0
        _Emission ("Emission", range(0.0, 20.0)) = 1.0
        [HDR]_EmissionColor ("Emission Color", color) = (0, 0, 0, 0)
        _EmissionBloomFactor ("Emission Bloom Factor", range(0.0, 10.0)) = 1.0
        _DarkEmissionIntensity ("Dark Emission Intensity", range(0.0, 10.0)) = 1.0
        [HideInInspector]_EmissionMapChannelMask ("_EmissionMapChannelMask", Vector) = (1, 1, 1, 0)
        [Space(30)]

        [Header(Shadow Setting)]
        [Space(5)]
        _LightMap ("LightMap", 2D) = "grey" { }
        _ShadowMultColor ("Shadow Color", color) = (1.0, 1.0, 1.0, 1.0)
        _SceondMaterialShadowColor("SecondMaterialShadowColor", Color) = (1,1,1,1)
        _ShadowArea ("Shadow Area", range(0.0, 1.0)) = 0.5
        _ShadowSmooth ("Shadow Smooth", range(0.0, 1.0)) = 0.05
        _DarkShadowMultColor ("Dark Shadow Color", color) = (0.5, 0.5, 0.5, 1)
        _DarkShadowArea ("Dark Shadow Area", range(0.0, 1.0)) = 0.5
        _DarkShadowSmooth ("Shadow Smooth", range(0.0, 1.0)) = 0.05
        [Toggle]_FixDarkShadow ("Fix Dark Shadow", float) = 1
        [Toggle]_EnableDarkShadow ("Enable Dark Shadow", float) = 1
        [Toggle]_IgnoreLightY ("Ignore Light y", float) = 0
        _FixLightY ("Fix Light y", range(-10.0, 10.0)) = 0.0
        _FixDivideShadow ("Fix Divide", range(0, 1)) = 0.5
        [Space(5)]
        [Toggle] _ReceiveShadowEnable("Receive Shadow", int) = 1
        
        /*
        [Toggle(ENABLE_FACE_SHADOW_MAP)]_EnableFaceShadowMap ("Enable Face Shadow Map", float) = 0
        _FaceShadowMap ("Face Shadow Map", 2D) = "white" { }
        _FaceShadowMapPow ("Face Shadow Map Pow", range(0.001, 1.0)) = 0.2
        _FaceShadowOffset ("Face Shadow Offset", range(-1.0, 1.0)) = 0.0
        */
        
        /*[Header(Shadow Ramp)]
        [Space(5)]
        [Toggle(ENABLE_RAMP_SHADOW)] _EnableRampShadow ("Enable Ramp Shadow", float) = 1
        _RampMap ("Shadow Ramp Texture", 2D) = "white" { }
        [Header(Ramp Area LightMapAlpha RampLine)]
        _RampArea12 ("Ramp Area 1/2", Vector) = (-50, 1, -50, 4)
        _RampArea34 ("Ramp Area 3/4", Vector) = (-50, 0, -50, 2)
        _RampArea5 ("Ramp Area 5", Vector) = (-50, 3, -50, 0)
        _RampShadowRange ("Ramp Shadow Range", range(0.0, 1.0)) = 0.8
        [Space(30)]
        */
        /*[Header(Shadow Ramp Origin)]
        [Space(5)]
        [Toggle(ENABLE_RAMP_SHADOW_ORIGIN)] _EnableRampShadowOrigin ("Enable Ramp Shadow 3rd", float) = 1
        [Header(Ramp Color)]
        _HighColor("Color2", Color) = (1,1,1,1)
        _MedColor("Color3", Color) = (1,1,1,1)
        _LowColor("Color4", Color) = (1,1,1,1)
        [Space(30)]
        */
        [Header(Specular Setting)]
        [Space(5)]
        [Toggle] _EnableDefaultSpecular ("Enable Default Specular", float) = 0
        [HDR]_LightSpecColor ("Specular Color", color) = (0.8, 0.8, 0.8, 1)
        _Shininess ("Shininess", range(0.1, 20.0)) = 10.0
        _SpecMulti ("Multiple Factor", range(0.1, 1.0)) = 1
        
        [Toggle(ENABLE_HAIR_SPECULAR)] _EnableHairSpecular ("Enable Hair Specular", float) = 0
        _Sharpness("Sharpness", float) = 30
        _SpecularIntensity("Intensity", Range(0.0, 1.0)) = 0.5
        /*_JitterMap("JitterMap", 2D) = "black" {}
        _JitterIntensity("Jitter Intensity", Range(0.0, 1.0)) = 0.5
        _SharpnessHigh("SharpnessHigh", float) = 30
        _SpecularIntensityHigh("IntensityHigh", Range(0.0, 1.0)) = 0.5
        */
        //[Space(30)]
        //[Toggle(ENABLE_METAL_SPECULAR)] _EnableMetalSpecular ("Enable Metal Specular", float) = 1
        //_MetalMap ("Metal Map", 2D) = "white" { }

        [Header(RimLight Setting)]
        [Space(5)]
        [Toggle]_EnableLambert ("Enable Lambert", float) = 1
        [Toggle]_EnableRim ("Enable Rim", float) = 1
        [HDR]_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimSmooth ("Rim Smooth", Range(0.001, 10.0)) = 10
        _RimPow ("Rim Pow", Range(0.0, 10.0)) = 1.2
        /*[Space(5)]
        [Toggle]_EnableRimDS ("Enable Dark Side Rim", int) = 1
        [HDR]_DarkSideRimColor ("DarkSide Rim Color", Color) = (1, 1, 1, 1)
        _DarkSideRimSmooth ("DarkSide Rim Smooth", Range(0.001, 10.0)) = 10
        _DarkSideRimPow ("DarkSide Rim Pow", Range(0.0, 10.0)) = 1.0
        [HideInInspector][Toggle]_EnableRimOther ("Enable Other Rim", int) = 0
        [HideInInspector][HDR]_OtherRimColor ("Other Rim Color", Color) = (1, 1, 1, 1)
        [HideInInspector]_OtherRimSmooth ("Other Rim Smooth", Range(0.001, 1.0)) = 0.01
        [HideInInspector]_OtherRimPow ("Other Rim Pow", Range(0.001, 50.0)) = 10.0
        */
        [Space(30)]

        [Header(Outline Setting)]
        [Space(5)]
        _OutlineMask("Outline Mask", 2D) = "white" {}
        _OutlineWidth ("_OutlineWidth (World Space)", Range(0, 50)) = 1
        _OutlineLightAffects("Outline Light Affects", Range(0.0, 1.0)) = 1.0
        _OutlineSaturation("Outline Saturation", Range(0.0, 4.0)) = 3.0
        _OutlineBrightness("Outline Brightness", Range(0.0, 1.0)) = 0.25
        _OutlineStrength("Outline Strength", Range(0.0, 1.0)) = 0.5
        _OutlineSmoothness("Outline Smoothness", Range(0.0, 1.0)) = 1.0
        [HideInInspector]_OutlineZOffset ("_OutlineZOffset (View Space) (increase it if is face!)", Range(0, 1)) = 0.0001

        [Header(Alpha)]
        [Toggle(ENABLE_ALPHA_CLIPPING)]_EnableAlphaClipping ("_EnableAlphaClipping", Float) = 0
        _Cutoff ("_Cutoff (Alpha Cutoff)", Range(0.0, 1.0)) = 0.5        
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" "RenderQueue" = "Opaque" }
        
        HLSLINCLUDE
        #include "UniversalToonInputCustom.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        
        struct appdata
        {
            float3 positionOS : POSITION;
            half4 color: COLOR0;
            float2 uv : TEXCOORD0;
            float3 normal : NORMAL;
            half4 tangent: TANGENT;
        };

        struct v2f
        {
            float3 normal : NORMAL;
            float4 positionCS: SV_POSITION;
            float4 color: COLOR0;
            float4 uv : TEXCOORD0;
            float3 positionWS: TEXCOORD1;
            float3 positionVS: TEXCOORD2;
            float3 normalWS: TEXCOORD3;
            float4 shadowCoord: TEXCOORD5;
            float fogFactor : TEXCOORD6;
            float4 screenPos : TEXCOORD7;
            float3 binormal: TEXCOORD8; // xyz: tangent, w: sign            
        };

        v2f vertBase (appdata input)
        {
            v2f output = (v2f)0;
            output.color = input.color;

            VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normal, input.tangent);
            output.normalWS = vertexNormalInput.normalWS;

            output.positionWS = TransformObjectToWorld(input.positionOS);
            output.positionVS = TransformWorldToView(output.positionWS);
            output.positionCS = TransformWorldToHClip(output.positionWS);
            output.screenPos = output.positionCS;
            output.binormal = normalize(cross(output.normalWS.xyz, input.tangent.xyz) * input.tangent.w * unity_WorldTransformParams.w);
            output.binormal = mul(unity_ObjectToWorld, output.binormal);

            output.uv.xy = TRANSFORM_TEX(input.uv, _BaseMap);

            output.shadowCoord = TransformWorldToShadowCoord(output.positionWS);
            return output;
        }
        
        ENDHLSL
        
        //デプスに書く
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

            #pragma vertex vertBase
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
        
        //メインのライティング
        Pass
        {
            NAME "CHARACTER_BASE"
            
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZTest LEqual
            ZWrite On
            Blend One Zero
            
            HLSLPROGRAM

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            
            #pragma shader_feature_local_fragment ENABLE_ALPHA_CLIPPING
            #pragma shader_feature_local_fragment ENABLE_BLOOM_MASK
            #pragma shader_feature_local_fragment ENABLE_FACE_SHADOW_MAP
            //#pragma shader_feature_local_fragment ENABLE_RAMP_SHADOW
            #pragma shader_feature_local_fragment ENABLE_HAIR_SPECULAR
            //#pragma shader_feature_local_fragment ENABLE_RAMP_SHADOW_ORIGIN
            
            #pragma vertex vertBase
            #pragma fragment fragToon
            // make fog work
            #pragma multi_compile_fog

            SamplerState my_linear_clamp_sampler;
            //デプス取得
            float sampleSceneDepth(float2 uv)
            {
                float sceneDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, my_linear_clamp_sampler, uv);
                return Linear01Depth(sceneDepth, _ZBufferParams) * _ProjectionParams.z;
            }

            inline half remap(const half v, const half fromMin, const half fromMax, const half toMin, const half toMax)
            {
                return toMin + (v - fromMin) * (toMax - toMin) / (fromMax - fromMin);
            }
            
            half3 shift(half3 color, half3 shift)
            {
                half VSU = shift.z * shift.y * cos(shift.x * 6.28318512);
                half VSW = shift.z * shift.y * sin(shift.x * 6.28318512);
                    
                return half3(
                    (0.299 * shift.z + 0.701 * VSU + 0.168 * VSW) * color.r + (0.587 * shift.z - 0.587 * VSU + 0.330 * VSW) * color.g + (0.114 * shift.z - 0.114 * VSU - 0.497 * VSW) * color.b,
                    (0.299 * shift.z - 0.299 * VSU - 0.328 * VSW) * color.r + (0.587 * shift.z + 0.413 * VSU + 0.035 * VSW) * color.g + (0.114 * shift.z - 0.114 * VSU + 0.292 * VSW) * color.b,
                    (0.299 * shift.z - 0.300 * VSU + 1.25 * VSW)  * color.r + (0.587 * shift.z - 0.588 * VSU - 1.05 * VSW)  * color.g + (0.114 * shift.z + 0.886 * VSU - .203 * VSW) * color.b
                );
            }
            
            //アウトラインかく
            float SoftOutline(float2 uv, half width, half strength, half power)
            {
                float sceneDepth = sampleSceneDepth(uv);
                float w = width / max(sceneDepth * 1.0, 1.0);
                width = (w + 0.5) * 0.5;
                float2 delta = (1.0 / _ScreenParams.xy) * width;

                const int SAMPLE = 8;
                float depthes[SAMPLE];
                depthes[0] = sampleSceneDepth(uv + float2(-delta.x, -delta.y));
                depthes[1] = sampleSceneDepth(uv + float2(-delta.x,  0.0)    );
                depthes[2] = sampleSceneDepth(uv + float2(-delta.x,  delta.y));
                depthes[3] = sampleSceneDepth(uv + float2(0.0,      -delta.y));
                depthes[4] = sampleSceneDepth(uv + float2(0.0,       delta.y));
                depthes[5] = sampleSceneDepth(uv + float2(delta.x,  -delta.y));
                depthes[6] = sampleSceneDepth(uv + float2(delta.x,   0.0)    );
                depthes[7] = sampleSceneDepth(uv + float2(delta.x,   delta.y));

                float coeff[SAMPLE] = {0.7071, 1.0, 0.7071, 1.0, 1.0, 0.7071, 1.0, 0.7071};

                float depthValue = 0;
                float str = pow(20.0, strength * 10.0) * 0.5;
                float smoothness = 1.0 / remap(power, 0.0, 1.0, 0.01, 0.3);
                [unroll]
                for (int j = 0; j < SAMPLE; j++)
                {
                    float sub = abs(depthes[j] - sceneDepth);
                    sub = pow(sub, smoothness);
                    sub *= str * coeff[j];
                    depthValue += sub;
                }

                half outlineRate = saturate(depthValue);

                return outlineRate;
            }

            //URP内のBRDFから抜き出し BRDFデータは作らずにラフネスの値だけでとりあえずspecularさせたいとこだけ欲しい
            // Computes the scalar specular term for Minimalist CookTorrance BRDF
            // NOTE: needs to be multiplied with reflectance f0, i.e. specular color to complete
            half BRDFSpecularCustom(float smoothness, half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS)
            {
                half perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(smoothness);
                half roughness           = max(PerceptualRoughnessToRoughness(perceptualRoughness), HALF_MIN_SQRT);
                //ラフネス2乗
                half roughness2 = max(roughness * roughness, HALF_MIN);
                //ラフネス2乗 - 1
                half roughness2MinusOne = roughness2 - 1.0h;
                half normalizationTerm = roughness * 4.0h + 2.0h;

                
                float3 lightDirectionWSFloat3 = float3(lightDirectionWS);
                float3 halfDir = SafeNormalize(lightDirectionWSFloat3 + float3(viewDirectionWS));

                float NoH = saturate(dot(float3(normalWS), halfDir));
                half LoH = half(saturate(dot(lightDirectionWSFloat3, halfDir)));

                // GGX Distribution multiplied by combined approximation of Visibility and Fresnel
                // BRDFspec = (D * V * F) / 4.0
                // D = roughness^2 / ( NoH^2 * (roughness^2 - 1) + 1 )^2
                // V * F = 1.0 / ( LoH^2 * (roughness + 0.5) )
                // See "Optimizing PBR for Mobile" from Siggraph 2015 moving mobile graphics course
                // https://community.arm.com/events/1155

                // Final BRDFspec = roughness^2 / ( NoH^2 * (roughness^2 - 1) + 1 )^2 * (LoH^2 * (roughness + 0.5) * 4.0)
                // We further optimize a few light invariant terms
                // brdfData.normalizationTerm = (roughness + 0.5) * 4.0 rewritten as roughness * 4.0 + 2.0 to a fit a MAD.
                float d = NoH * NoH * roughness2MinusOne + 1.00001f;
                half d2 = half(d * d);

                half LoH2 = LoH * LoH;
                half specularTerm = roughness2 / (d2 * max(half(0.1), LoH2) * normalizationTerm);

                // On platforms where half actually means something, the denominator has a risk of overflow
                // clamp below was added specifically to "fix" that, but dx compiler (we convert bytecode to metal/gles)
                // sees that specularTerm have only non-negative terms, so it skips max(0,..) in clamp (leaving only min(100,...))
            #if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
                specularTerm = specularTerm - HALF_MIN;
                specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
            #endif

            return specularTerm;
            }

            //使いまわす色情報()
            struct ColorForCalculate
            {
                float4 LightingColor; //ライティング色
                half3 DarkShadowColor; //2影色
            };
            
            //Toonに必要な要素
            struct ToonShadowFactor
            {
                float SWeight; //影範囲
                float SFactor; //1影の塗分け範囲
                float SFactorD; //影の塗分け範囲
                half rampS; //1影の境界線調整値
                half rampDS; //2影の境界線調整値
                float HalfLambert; //lambert情報
            };

            //ライト情報からToonシェードに必要な情報を取得しておく
            ToonShadowFactor CalculateToonShadowFactor(
                Light light, //ライト情報
                float3 normalWS, //ノーマル情報
                float mask, //マスク範囲
                float mainLightShadowArea, //落ち影情報
                bool halfLambert //halfLambertの使用
                )

            {
                ToonShadowFactor tsf;
                
                float3 lightDirWS = normalize(light.direction.xyz);
                float3 fixedlightDirWS = normalize(float3(lightDirWS.x, _FixLightY, lightDirWS.z));
                lightDirWS = _IgnoreLightY ? fixedlightDirWS: lightDirWS;

                tsf.HalfLambert = halfLambert ? light.distanceAttenuation * dot(normalWS, lightDirWS) * 0.5f + 0.5f : light.distanceAttenuation * dot(normalWS, lightDirWS);
                
                //影範囲の決定
                tsf.SWeight = halfLambert ? (mask + tsf.HalfLambert) * 0.5  + 1.125 : (mask + tsf.HalfLambert);

                //影を塗り分ける範囲
                tsf.SFactor = floor(tsf.SWeight - _ShadowArea) * mainLightShadowArea;
                tsf.SFactorD = floor(tsf.SWeight - _DarkShadowArea) * mainLightShadowArea;

                // 境界線の調整
                tsf.rampS = smoothstep(0, _ShadowSmooth, tsf.HalfLambert * mainLightShadowArea - _ShadowArea);
                tsf.rampDS = smoothstep(0, _DarkShadowSmooth, tsf.HalfLambert * mainLightShadowArea - _DarkShadowArea);
                return tsf;
            }
            
            //Toonシェーディング + スぺキュラ
            ColorForCalculate BaseLighting(
                Light light, //ライト情報
                float4 baseColor, //ベース色
                half3 shadowColor, //1影色
                half3 darkShadowColor, //2影色
                half3 viewDirWS, //視線ベクトル情報
                float3 normalWS, //ノーマル情報
                float3 binormal, //従法線
                float3 positionWS, //位置情報
                ToonShadowFactor toonShadowFactor, //Toonシェーディングする用の情報
                float specularMask //specularMask
                )
            {
                ColorForCalculate lightingColor;
                lightingColor.LightingColor = float4(0,0,0,1);
                
                //影を塗り分ける
                half3 ShallowShadowColor = toonShadowFactor.SFactor * baseColor.rgb + (1 - toonShadowFactor.SFactor) * shadowColor.rgb;
                lightingColor.DarkShadowColor = toonShadowFactor.SFactorD * (_FixDarkShadow * shadowColor + (1 - _FixDarkShadow) * ShallowShadowColor) + (1 - toonShadowFactor.SFactorD) * darkShadowColor;

                // 境界線の調整
                ShallowShadowColor.rgb = lerp(shadowColor, baseColor.rgb, toonShadowFactor.rampS);
                lightingColor.DarkShadowColor.rgb = lerp(lightingColor.DarkShadowColor.rgb, ShallowShadowColor, toonShadowFactor.rampDS);
                
                float SFactor = _FixDivideShadow;//floor(LightMapColor.g * i.color.r + 0.9f);

                lightingColor.LightingColor.rgb = _EnableDarkShadow ? SFactor * ShallowShadowColor + (1 - SFactor) * lightingColor.DarkShadowColor : ShallowShadowColor.rgb;
                
                half specular = BRDFSpecularCustom(_Smoothness, normalWS, light.direction, viewDirWS) * specularMask;
                
                half4 specularColor = _EnableDefaultSpecular * _LightSpecColor * specular;
                lightingColor.LightingColor.rgb += specularColor.rgb;
                //SpecDiffuse.rgb *= _BaseColor.rgb;
                lightingColor.LightingColor.a = specularColor.a * _BloomFactor * 10;

#if ENABLE_HAIR_SPECULAR
                float3 halfDir = normalize(light.direction + viewDirWS);
                float3 tmpBinormal = normalize(binormal - normalWS * positionWS);
                float dotTH = dot(tmpBinormal, halfDir);// * 0.5 + 0.5;
                float sintTH = sqrt(1.0 - dotTH * dotTH);
                float dirAtten = smoothstep(-1.0, 0.0, dotTH);
                lightingColor.LightingColor.rgb += _EnableHairSpecular * _LightSpecColor * dirAtten * pow(sintTH, _Sharpness) * _SpecularIntensity * specularMask;
                
#endif
                //lightingColor.LightingColor.rgb *= light.color;
                lightingColor.LightingColor.rgb = _WorldLightInfluence * lightingColor.LightingColor.rgb * light.color * light.distanceAttenuation;
                return lightingColor;
            }
            
            float4 fragToon (v2f i) : SV_Target
            {
                float4 finalColor = float4(0,0,0,1);
                half4 Rim = half4(0,0,0,0);

                
                // sample the texture
                float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv.xy) * _BaseColor;
                
                #if ENABLE_ALPHA_CLIPPING
                    clip(baseColor.a - _Cutoff);
                #endif

                //メインライトの情報
                Light mainLight = GetMainLight();

                //ライトマップ取得しておく
                half4 LightMapColor = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, i.uv.xy);

                //影色情報を設定
                //2個目のカラーがある場合
                float secondColorMask = 1.0 - SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, i.uv.xy).a;
                half3 ShadowColor = secondColorMask == 0 ? baseColor.rgb * _ShadowMultColor.rgb : baseColor.rgb * _SceondMaterialShadowColor.rgb;
                half3 DarkShadowColor = baseColor.rgb * _DarkShadowMultColor.rgb;

                //影を受ける
                float4 shadowCoord = TransformWorldToShadowCoord(i.positionWS);
                mainLight.shadowAttenuation = MainLightRealtimeShadow(shadowCoord);
                float mainLightShadowArea = _ReceiveShadowEnable ? mainLight.shadowAttenuation : 1;

                //視線ベクトル情報
                half3 viewDirWS = normalize(_WorldSpaceCameraPos.xyz - i.positionWS.xyz);

                //ライトマップによるマスク情報
                float lightMapMask = LightMapColor.g * i.color.r;
                float specularMask = SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, i.uv.xy).g;

                //レイヤー
                uint meshRenderingLayers = GetMeshRenderingLightLayer();

                //メインライトのトゥーン要素をキャッシュしておく
                ToonShadowFactor mainLightTSF = CalculateToonShadowFactor(mainLight, i.normalWS, lightMapMask, mainLightShadowArea, true);
                //ライティング計算(メイン光)
                ColorForCalculate mainLightingColor = BaseLighting(mainLight, baseColor, ShadowColor, DarkShadowColor, viewDirWS, i.normalWS, i.binormal, i.positionWS,  mainLightTSF, specularMask);
                if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
                {
                    finalColor += mainLightingColor.LightingColor;

                    //リム情報
                    float rim = 1 - saturate(dot(viewDirWS, i.normalWS));
                    float rimMask = SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, i.uv.xy).r;
                    float rimDot = pow(rim, _RimPow) * rimMask;
                    rimDot = _EnableLambert * mainLightTSF.HalfLambert * rimDot + (1 - _EnableLambert) * rimDot;
                    float rimIntensity = smoothstep(0, _RimSmooth, rimDot);
                    Rim = _EnableRim * pow(rimIntensity, 5) * _RimColor * baseColor;
                    Rim.a = _EnableRim * rimIntensity * _BloomFactor;
                }
                
                //追加光情報
                #if defined(_ADDITIONAL_LIGHTS)
                uint pixelLightCount = GetAdditionalLightsCount();

                #if USE_CLUSTERED_LIGHTING
                for (uint lightIndex = 0; lightIndex < min(_AdditionalLightsDirectionalCount, MAX_VISIBLE_LIGHTS); lightIndex++)
                {
                    Light light = GetAdditionalLight(lightIndex, i.positionWS);
                    if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
                    {
                        ToonShadowFactor additionalLightTSF = CalculateToonShadowFactor(light, i.normalWS, lightMapMask, 1, false);
                        //ライティング計算(追加光)
                        finalColor += BaseLighting(light, baseColor, ShadowColor, DarkShadowColor, viewDirWS, i.normalWS, i.binormal, i.positionWS,  additionalLightTSF, specularMask).LightingColor;
                    }
                }
                #endif

                LIGHT_LOOP_BEGIN(pixelLightCount)
                    Light light = GetAdditionalLight(lightIndex, i.positionWS);

                    if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
                    {
                        ToonShadowFactor additionalLightTSF = CalculateToonShadowFactor(light, i.normalWS, lightMapMask, 1, false);
                        //ライティング計算(追加光)
                        finalColor += BaseLighting(light, baseColor, ShadowColor, DarkShadowColor, viewDirWS, i.normalWS, i.binormal, i.positionWS,  additionalLightTSF, specularMask).LightingColor;
                    }
                LIGHT_LOOP_END
                #endif
                
                // Emission & Bloom
                half4 Emission;
                float emissionMask = SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, i.uv.xy).b;
                Emission.rgb = _Emission * mainLightingColor.DarkShadowColor.rgb * _EmissionColor.rgb - finalColor.rgb * emissionMask * _EnableEmission;
                Emission.a = _EmissionBloomFactor * baseColor.a * emissionMask;

                half4 SpecRimEmission;
                SpecRimEmission.rgb = pow(mainLightingColor.DarkShadowColor, 1) * _Emission;
                SpecRimEmission.a = (finalColor.a + Rim.a/* + RimDS.a*/);

                finalColor = finalColor + Rim + /*RimDS*/ + Emission.a * Emission + SpecRimEmission.a * SpecRimEmission;
                
                //finalColor = _WorldLightInfluence * finalColor + (1 - _WorldLightInfluence) * finalColor;

                // Outline
                float2 screenPos = ComputeScreenPos(i.screenPos / i.screenPos.w).xy;
                half width = lerp(_OutlineWidth, _OutlineWidth * 0.5, mainLightTSF.rampDS * _OutlineLightAffects);
                width *= SAMPLE_TEXTURE2D(_OutlineMask, sampler_OutlineMask, i.uv).r;
                half outlineFactor = SoftOutline(screenPos, width, _OutlineStrength, _OutlineSmoothness);
                finalColor.rgb = lerp(finalColor.rgb, shift(finalColor.rgb, half3(0.0, _OutlineSaturation, lerp(_OutlineBrightness, saturate(_OutlineBrightness * 2.0), mainLightTSF.rampDS * _OutlineLightAffects))), outlineFactor);
                
                // apply fog
                finalColor.rgb = MixFog(finalColor.rgb, i.fogFactor);
                
                return finalColor;
            }

            /*
             *　いつか使うかも
             * 
             */

            // Rim Light ランバートで影の所にリムなし
            /*float lambertF = dot(mainLight.direction, i.normalWS) * mainLightShadowArea;
            float lambertD = max(0, -lambertF);
            lambertF = max(0, lambertF);
            */
            //ダークリム
            /*rimDot = pow(rim, _DarkSideRimPow);
            rimDot = _EnableLambert * lambertD * rimDot + (1 - _EnableLambert) * rimDot;
            rimIntensity = smoothstep(0, _DarkSideRimSmooth, rimDot);
            half4 RimDS = _EnableRimDS * pow(rimIntensity, 5) * _DarkSideRimColor * baseColor;
            RimDS.a = _EnableRimDS * rimIntensity * _BloomFactor;
            */

            //髪スぺキュラ
            /*
             *                //half jitter = SAMPLE_TEXTURE2D(_JitterMap, sampler_JitterMap, i.uv.xy).r;
             binormal = normalize(i.binormal - i.normalWS * i.positionWS + (jitter * 0.5 - 0.5) * _JitterIntensity);
            dotTH = dot(binormal, halfDir) * 0.5 + 0.5;
            float highlight = dotTH * (1 - dotTH) * 4;
            highlight = pow(highlight, _SharpnessHigh);
            
            SpecDiffuse.rgb += _EnableHairSpecular * _LightSpecColor * highlight * _SpecularIntensityHigh * SFactor;
            */
                //SpecDiffuse.rgb += _EnableHairSpecular * _LightSpecColor * dirAtten * pow(sintTH, _SharpnessHigh) * _SpecularIntensityHigh;
             
            // Blinn-Phong
            /*
            half3 halfViewLightWS = normalize(viewDirWS + mainLight.direction.xyz);

            half spec = pow(saturate(dot(i.normalWS, halfViewLightWS)), _Shininess);
            spec = step(1.0f - LightMapColor.b, spec);
            half4 specularColor = _EnableSpecular * _LightSpecColor * _SpecMulti * LightMapColor.r * spec;

            half4 SpecDiffuse;
            SpecDiffuse.rgb = specularColor.rgb + finalColor.rgb;
            SpecDiffuse.rgb *= _BaseColor.rgb;
            SpecDiffuse.a = specularColor.a * _BloomFactor * 10;
            */

            // FaceLightMap
            /*#if ENABLE_FACE_SHADOW_MAP
                // 光の回転オフセットを計算する
                float sinx = sin(_FaceShadowOffset);
                float cosx = cos(_FaceShadowOffset);
                float2x2 rotationOffset = float2x2(cosx, -sinx, sinx, cosx);
            
                float3 Front = unity_ObjectToWorld._12_22_32;
                float3 Right = unity_ObjectToWorld._13_23_33;
                float2 lightDir = mul(rotationOffset, mainLight.direction.xz);

                // xz 平面における光の角度を計算する
                float FrontL = dot(normalize(Front.xz), normalize(lightDir));
                float RightL = dot(normalize(Right.xz), normalize(lightDir));
                RightL = - (acos(RightL) / PI - 0.5) * 2;

                // FaceLightMap の影データの左右のサンプルが lightData に格納される
                float2 lightData = float2(SAMPLE_TEXTURE2D(_FaceShadowMap, sampler_FaceShadowMap, float2(i.uv.x, i.uv.y)).r,
                SAMPLE_TEXTURE2D(_FaceShadowMap, sampler_FaceShadowMap, float2(-i.uv.x, i.uv.y)).r);
            
                // lightDataの変化曲線を、真ん中の変化率のほとんどが平坦になるように修正する。
                lightData = pow(abs(lightData), _FaceShadowMapPow);

                // ライトの角度に基づいて正または負の lightData を使用し、バックライトかどうかを判断します。
                float lightAttenuation = step(0, FrontL) * min(step(RightL, lightData.x), step(-RightL, lightData.y));
                                    
                half3 FaceColor = lerp(ShadowColor.rgb, baseColor.rgb, lightAttenuation);
                finalColor.rgb = FaceColor;
            #endif
            */

            /*
            //ramp
            #if ENABLE_RAMP_SHADOW
                //ハーフランバートサンプリングでは、Rampの端までサンプリングすると黒い線が出るので、_RampShadowRange-0.003でこれを回避するらしい。
                float rampValue = i.lambert * (1.0 / _RampShadowRange - 0.003);

                //材質によってRampのサンプリング箇所を変えたい→肌は明るく、金属は暗く、みたいな。
                half3 ShadowRamp1 = SAMPLE_TEXTURE2D(_RampMap, sampler_RampMap, float2(0.95, rampValue)).rgb;
                half3 ShadowRamp2 = SAMPLE_TEXTURE2D(_RampMap, sampler_RampMap, float2(0.85, rampValue)).rgb;
                half3 ShadowRamp3 = SAMPLE_TEXTURE2D(_RampMap, sampler_RampMap, float2(0.75, rampValue)).rgb;
                half3 ShadowRamp4 = SAMPLE_TEXTURE2D(_RampMap, sampler_RampMap, float2(0.65, rampValue)).rgb;
                half3 ShadowRamp5 = SAMPLE_TEXTURE2D(_RampMap, sampler_RampMap, float2(0.55, rampValue)).rgb;
                half3 CoolShadowRamp1 = SAMPLE_TEXTURE2D(_RampMap, sampler_RampMap, float2(rampValue, 0.45)).rgb;
                half3 CoolShadowRamp2 = SAMPLE_TEXTURE2D(_RampMap, sampler_RampMap, float2(rampValue, 0.35)).rgb;
                half3 CoolShadowRamp3 = SAMPLE_TEXTURE2D(_RampMap, sampler_RampMap, float2(rampValue, 0.25)).rgb;
                half3 CoolShadowRamp4 = SAMPLE_TEXTURE2D(_RampMap, sampler_RampMap, float2(rampValue, 0.15)).rgb;
                half3 CoolShadowRamp5 = SAMPLE_TEXTURE2D(_RampMap, sampler_RampMap, float2(rampValue, 0.05)).rgb;
                //Lamp値の配列を作っておく、この中から何使うかを決める
                half3 AllRamps[10] = {
                    ShadowRamp1, ShadowRamp2, ShadowRamp3, ShadowRamp4, ShadowRamp5, CoolShadowRamp1, CoolShadowRamp2, CoolShadowRamp3, CoolShadowRamp4, CoolShadowRamp5
                };

                // 0    hard/emission/specular/silk
                // 77   soft/common
                // 128  metal
                // 179  tights
                // 255  skin
                // ギルティギアのLightMap方式を採用→後で探す
                // あらかじめライトマップとランプシェーダーは作っておく？→探す
                // もともとの材質ごとにライトマップで使われてる値が異なるのでそれを利用
                half3 skinRamp = step(abs(LightMapColor.a * 255 - _RampArea12.x), 10) * AllRamps[_RampArea12.y]; // CoolShadowRamp2
                half3 tightsRamp = step(abs(LightMapColor.a * 255 - _RampArea12.z), 10) * AllRamps[_RampArea12.w]; // CoolShadowRamp5
                half3 softCommonRamp = step(abs(LightMapColor.a * 255 - _RampArea34.x), 10) * AllRamps[_RampArea34.y]; // CoolShadowRamp1
                half3 hardSilkRamp = step(abs(LightMapColor.a * 255 - _RampArea34.z), 10) * AllRamps[_RampArea34.w]; // CoolShadowRamp3
                half3 metalRamp = step(abs(LightMapColor.a * 255 - _RampArea5.x), 10) * AllRamps[_RampArea5.y]; // CoolShadowRamp4

                // 5行のRampカラーを組み合わせて最終的なRampカラーデータを取得
                half3 finalRamp = ShadowRamp1 + tightsRamp + metalRamp + softCommonRamp + hardSilkRamp;

                //明るいところはベース色まま、暗いところにランプ色を使う
                rampValue = step(_RampShadowRange, i.lambert);
                half3 RampShadowColor = rampValue * baseColor.rgb + (1 - rampValue) * finalRamp * baseColor.rgb;

                ShadowColor = RampShadowColor;
                DarkShadowColor = RampShadowColor;
            
                finalColor.rgb = RampShadowColor;
            #endif            
            */
            ENDHLSL
        }
    }
}
