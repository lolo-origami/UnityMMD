/*
 * いったん要らん奴
* #if ENABLE_EDGE_RIM
    //finalColor.rgb +=  half4(outlineFactor,outlineFactor,outlineFactor,outlineFactor);
#else
    //finalColor.rgb = lerp(finalColor.rgb, shift(finalColor.rgb, half3(0.0, _OutlineSaturation, lerp(_OutlineBrightness, saturate(_OutlineBrightness * 2.0), lllData.RampDS * _OutlineLightAffects))), outlineFactor);
#endif

float EdgeHighlight(float4 screenPos, float2 uv)
{
    float2 screenPosD = ComputeScreenPos(screenPos / screenPos.w).xy;
    
    // 近隣のテクスチャ色をサンプリング
    float diffU =  _EdgeRimWidth;
    float diffV =  _EdgeRimWidth;
    half3 col00 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, my_linear_clamp_sampler, screenPosD + half2(-diffU, -diffV));
    half3 col01 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, my_linear_clamp_sampler, screenPosD + half2(-diffU, 0.0));
    half3 col02 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, my_linear_clamp_sampler, screenPosD + half2(-diffU, diffV));
    half3 col10 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, my_linear_clamp_sampler, screenPosD + half2(0.0, -diffV));
    half3 col12 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, my_linear_clamp_sampler, screenPosD + half2(0.0, diffV));
    half3 col20 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, my_linear_clamp_sampler, screenPosD + half2(diffU, -diffV));
    half3 col21 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, my_linear_clamp_sampler, screenPosD + half2(diffU, 0.0));
    half3 col22 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, my_linear_clamp_sampler, screenPosD + half2(diffU, diffV));

    // 水平方向のコンボリューション行列適用後の色を求める
    half3 horizontalColor = 0;
    horizontalColor += col00 * -1.0;
    horizontalColor += col01 * -2.0;
    horizontalColor += col02 * -1.0;
    horizontalColor += col20;
    horizontalColor += col21 * 2.0;
    horizontalColor += col22;
                
    // 垂直方向のコンボリューション行列適用後の色を求める
    half3 verticalColor = 0;
    verticalColor += col00;
    verticalColor += col10 * 2.0;
    verticalColor += col20;
    verticalColor += col02 * -1.0;
    verticalColor += col12 * -2.0;
    verticalColor += col22 * -1.0;
                
    // この値が大きく正の方向を表す部分がアウトライン
    // ※1
    half3 outlineValue = horizontalColor * horizontalColor + verticalColor * verticalColor;
    return outlineValue - _EdgeRimWidth;
}

var EdgeRim = property.FindPropertyRelative("EnableEdgeRim");
var EdgeRimWidthValue = property.FindPropertyRelative("EnableWidthValue");

EditorGUILayout.PropertyField(EdgeRim);
EditorGUILayout.PropertyField(EdgeRimWidthValue);

TargetMat.SetFloat("_EnableEdgeRim", behaviour.EnableEdgeRim == false ? 0 : 1);
if (behaviour.EnableEdgeRim)
{
    var edgeRimWidthValue = behaviour.EnableWidthValue * weight;
    TargetMat.SetFloat("_EdgeRimWidth", edgeRimWidthValue);
}

float2 sobelSamplePoints[9] = {
        float2(-1,1),  float2(0,1),  float2(1,1),
        float2(-1,0),  float2(0,0),  float2(1,0),
        float2(-1,-1), float2(0,-1), float2(1,-1) 
    };

    static float sobelXMatrix[9] = {
        1, 0, -1,
        2, 0, -2,
        1, 0, -1
    };

    static float sobelYMatrix[9] = {
        1, 2, 1,
        0, 0, 0,
        -1, -2, -1
    };

    float rimLightLength = _EdgeRimWidth;
    float2 sobel = 0;
    float2 screenPosD = ComputeScreenPos(screenPos / screenPos.w).xy;
    for(int i = 0; i < 9; i++)
    {
        screenPosD.xy += sobelSamplePoints[i] * rimLightLength;
        float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, my_linear_clamp_sampler, screenPosD), _ZBufferParams);
        sobel += depth * float2(sobelXMatrix[i], sobelYMatrix[i]);
    }
    return length(sobel);


 */