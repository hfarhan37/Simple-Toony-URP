#ifndef SIMPLE_TOONY_URP_INCLUDED
#define SIMPLE_TOONY_URP_INCLUDED

void Shadowmask_half (float2 lightmapUV, out half4 Shadowmask){
    #ifdef SHADERGRAPH_PREVIEW
    Shadowmask = half4(1,1,1,1);
    #else
    OUTPUT_LIGHTMAP_UV(lightmapUV, unity_LightmapST, lightmapUV);
    Shadowmask = SAMPLE_SHADOWMASK(lightmapUV);
    #endif
}

void MainLight_half(float3 WorldPos, out half3 Direction, out half3 Color, out half DistanceAtten, out half ShadowAtten)
{
    #if SHADERGRAPH_PREVIEW
    Direction = half3(0.5, 0.5, 0);
    Color = 1;
    DistanceAtten = 1;
    ShadowAtten = 1;
    #else
    #if SHADOWS_SCREEN
    half4 clipPos = TransformWorldToHClip(WorldPos);
    half4 shadowCoord = ComputeScreenPos(clipPos);
    #else
    half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    #endif
    Light mainLight = GetMainLight(shadowCoord);
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;
    ShadowAtten = mainLight.shadowAttenuation;
    #endif
}

void CalculateToonLight_half(float3 NormalWS, half3 LightDirection, half3 LightColor, half ShadowAtten, half3 ViewDirectionWS, out half3 ToonLightColor, out half ToonLightFactor, out half ToonShadowsFactor)
{
    float NoL = dot(NormalWS, LightDirection);
    ToonLightFactor = smoothstep(0, 0.01, NoL);
    ToonShadowsFactor = smoothstep(0.5, 0.51, ShadowAtten);
    ToonLightColor = LightColor * ToonLightFactor * ToonShadowsFactor;
}

void CalculateToonMainLight_half(float3 PositionWS, float3 NormalWS, half3 ViewDirectionWS, out half3 ToonLightColor, out half ToonLightFactor, out half ToonShadowsFactor, out half3 Direction, out half3 Color, out half DistanceAtten, out half ShadowAtten)
{
    MainLight_half(PositionWS, Direction, Color, DistanceAtten, ShadowAtten);
    float NoL = dot(NormalWS, Direction);
    ToonLightFactor = smoothstep(0, 0.01, NoL);
    ToonShadowsFactor = smoothstep(0.5, 0.51, ShadowAtten);
    ToonLightColor = Color * ToonLightFactor * ToonShadowsFactor;
}

void CalculateRimLight_half(float3 NormalWS, half3 LightColor, half3 ViewDirectionWS, half ToonLightFactor, half ToonShadowsFactor, half RimSharpness, half RimPower, out half3 RimLightColor)
{
    float NoV = max(dot(NormalWS, ViewDirectionWS), 0.1);
    float rim = pow(abs(1.0 - NoV), RimSharpness);
    rim *= ToonLightFactor * ToonShadowsFactor;
    rim = smoothstep(0.1, 0.02, rim);
    rim *= RimPower;
    RimLightColor = rim * LightColor;
}

void CalculateToonSpecular_half(float3 SpecularColor, half Smoothness, float3 NormalWS, half3 LightDirection, half3 ViewDirectionWS, half ToonLightFactor, half ToonShadowsFactor, out half3 Specular, out half SpecularFactor)
{
    float halfVector = normalize(LightDirection + ViewDirectionWS);
    float NoH = saturate(max(dot(NormalWS, halfVector), 0.01));
    SpecularFactor = pow(NoH, Smoothness);
    SpecularFactor *= ToonLightFactor * ToonShadowsFactor;
    SpecularFactor = smoothstep(0.01, 0.02, SpecularFactor);
    Specular = SpecularColor * SpecularFactor;
}

void CalculateToonSpecular_half(half3 NormalWS, half3 LightDirection, half3 ViewDirectionWS, half3 LightColor, float3 SpecularColor, float SpecularPower, float Smoothness, out float3 SpecularReflection)
{
    if (dot(NormalWS, LightDirection) < 0.0) //Light on the wrong side - no specular
        {
        SpecularReflection = float3(0.0, 0.0, 0.0);
        }
    else
    {
        SpecularReflection = LightColor * SpecularColor * round(pow(saturate(dot(reflect(LightDirection, NormalWS), -ViewDirectionWS)), SpecularPower)) * Smoothness;
    }
}

void CalculateAdditionalToolLight_half(float3 NormalWS, float3 PositionWS, half3 ViewDirectionWS, half4 ShadowMask, float3 SpecularColor, float SpecularPower, float Smoothness, out half3 ToonLightColor, out half ToonLightFactor, out half ToonShadowsFactor)
{
    #if SHADERGRAPH_PREVIEW
    ToonLightColor = 1;
    ToonLightFactor = 1;
    ToonShadowsFactor = 1;
    CalculateToonLight_half(NormalWS, half3(0.5, 0.5, 0), 1, 1, half3(1, 1, 1), ToonLightColor, ToonLightFactor, ToonShadowsFactor);
    #else
    int lightCount = GetAdditionalLightsCount();
    ToonLightColor = half3(0,0,0);
    ToonLightFactor = 0;
    ToonShadowsFactor = 0;
    for (int i = 0; i < lightCount; i++)
    {
        Light light = GetAdditionalLight(i,  PositionWS, ShadowMask);
        half3 tlc = half3(0,0,0);
        half tlf = 0;
        half tsf = 0;
        float3 specular = 0;
        CalculateToonLight_half(NormalWS, light.direction, light.color, light.shadowAttenuation, ViewDirectionWS, tlc, tlf, tsf);
        CalculateToonSpecular_half(NormalWS, light.direction, ViewDirectionWS, light.color, SpecularColor, SpecularPower, Smoothness, specular);
        specular *= tsf * light.distanceAttenuation;
        ToonLightColor += tlc * light.distanceAttenuation;
        ToonLightColor += specular;
        ToonLightFactor += tlf;
        ToonShadowsFactor += tsf;
        
    }
    #endif
    
}

#endif