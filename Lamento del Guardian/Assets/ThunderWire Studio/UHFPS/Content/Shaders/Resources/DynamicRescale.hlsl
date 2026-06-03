// UNITY_SHADER_NO_UPGRADE
#ifndef DYNAMICRESCALE_INCLUDED
#define DYNAMICRESCALE_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

TEXTURE2D_X(_CustomPostProcessInput);

// Input: 
//  ScreenPos : float2 from node Screen Position (Default) Shader Graph
// Output:
//  ColorOut : float4 UV that already corrected if DLSS/FSR active
void DynamicRescaleUV_float(float2 UVsIn, out float4 ColorOut)
{
    // Use HDRP’s canonical scaling for RTHandles (DLSS/FSR/dynamic res aware)
    float2 uvSample = UVsIn * _RTHandlePostProcessScale.xy;

    // Use HDRP’s default linear clamp sampler already defined in includes
    ColorOut = SAMPLE_TEXTURE2D_X_LOD(_CustomPostProcessInput, s_linear_clamp_sampler, uvSample, 0);
}

#endif // DYNAMICRESCALE_INCLUDED