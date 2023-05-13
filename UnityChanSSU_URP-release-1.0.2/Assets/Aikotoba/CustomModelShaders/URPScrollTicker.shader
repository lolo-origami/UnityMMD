Shader "Universal Render Pipeline/URPScroll"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        [HDR] _EmissionColor("EmissionColor", Color) = (0,0,0)
        _Speed("Scroll Speed (U, V)", Vector) = (1, 1, 0, 0)
        _Amplitude("Amplitude", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" "LightMode" = "UniversalForward"}
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UniversalToonInputCustom.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float fogFactor : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                o.normal = TransformObjectToWorldNormal(v.normal);
                o.fogFactor = ComputeFogFactor(o.vertex.z);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 d = _Speed * _Time.y;
                // sample the texture
                float4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv + d);

                // �G�~�b�V�����𑫂�
                col.rgb += _EmissionColor * _Amplitude;

                // apply fog
                col.rgb = MixFog(col.rgb, i.fogFactor);
                return col;
            }
            ENDHLSL
        }
    }
}
