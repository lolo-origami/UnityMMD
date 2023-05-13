// ========== ========== ==========
//   Macheiyora.cginc
//      Author : にしおかすみす！
//      Twitter : @nsokSMITHdayo
//      改良
// ========== ========== ==========

float4 Tlerp(float4 value1, float4 value2, float4 value3, float factor)
{
    float4 l1 = lerp(value1, value2, factor * 2);
    float4 l2 = lerp(value2, value3, factor * 2 - 1);
    return lerp(l1, l2, step(0.5, factor));
}
float selectMask(float4 tex, int channel, int inv, int masked)
{
    float mask = 1;
    if(masked == 1){
        if(channel == 0){
            mask = tex.r;
        }else if(channel == 1){
            mask = tex.g;
        }else if(channel == 2){
            mask = tex.b;
        }else if(channel == 3){
            mask = tex.a;
        }
        // inversion
        if(inv == 1 && channel < 4){
            mask = 1 - mask;
        }
    }
    return mask;
}
float selectChannel(float4 tex, int channel, float val)
{
    float value = val;
    if(channel == 0){
        value = tex.r;
    }else if(channel == 1){
        value = tex.g;
    }else if(channel == 2){
        value = tex.b;
    }else if(channel == 3){
        value = tex.a;
    }
    return value;
}