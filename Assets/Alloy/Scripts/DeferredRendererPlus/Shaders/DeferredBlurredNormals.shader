// Alloy Physical Shader Framework
// Copyright 2013-2015 RUST LLC.
// http://www.alloy.rustltd.com/

Shader "Hidden/Alloy/Deferred Blurred Normals" {
Properties {
    _MainTex ("Render Input", 2D) = "white" {}
}
SubShader {
    ZTest Always Cull Off ZWrite Off Fog { Mode Off }

    CGINCLUDE
    #pragma target 3.0
    #pragma exclude_renderers gles
    
    #include "UnityCG.cginc"
    #include "UnityDeferredLibrary.cginc"

    // Screen-space diffusion
    // cf http://www.iryoku.com/screen-space-subsurface-scattering
    // cf http://uaasoftware.com/xi/PDSS/diffuseShader.fx

    // Gaussian Distribution blur coefficients.
    #define NUM_TAPS 7

    const static float blurOffsets[NUM_TAPS] = {
        0.0f, -3.0f,  -2.0f,  -1.0f,   1.0f,   2.0f,   3.0f
    };

    const static float blurWeights[NUM_TAPS] = {
        0.199471f,  0.0647588f, 0.120985f, 0.176033f, 0.176033f, 0.120985f, 0.0647588f
    };

    // distanceToProjectionWindow = 1 / tan(radians(FoV) / 2);
    // stepScale = (blurWidth / farClipDistance) * distanceToProjectionWindow
    // depthDifferenceScale = blurDepthRange * farClipDistance * 25 * distanceToProjectionWindow
    // (X = stepScale, Y = depthDifferenceScale)
    float2 _DeferredBlurredNormalsParams; 

    sampler2D _MainTex;
    sampler2D _CameraGBufferTexture0;
    sampler2D _CameraGBufferTexture1;
    sampler2D _CameraGBufferTexture2;
    float4 _CameraGBufferTexture2_TexelSize;

    void FetchSamples(float2 uv, out float3 normal, out float depth)
    {
        float4 sampleUv = float4(uv, 0.0f, 0.0f);
        normal = tex2Dlod(_MainTex, sampleUv).xyz * 2.0f - 1.0f;
        depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampleUv));
    }

    float4 BlurInDir(v2f_img IN, float2 direction) {
        float3 normalM;
        float depthM;

        FetchSamples(IN.uv.xy, normalM, depthM);

        float scale = _DeferredBlurredNormalsParams.x / depthM;
        float2 finalStep = scale * direction * dot(direction, _CameraGBufferTexture2_TexelSize.xy);
        float3 normalBlurred = normalM * blurWeights[0];

        UNITY_UNROLL
        for (int i = 1; i < NUM_TAPS; i++) {
            float3 normal;
            float depth;

            FetchSamples(IN.uv.xy + finalStep * blurOffsets[i], normal, depth);

            // Lerp back to middle sample when blur sample crosses an edge.
            float s = min(1.0f, _DeferredBlurredNormalsParams.y * abs(depth - depthM));

            normalBlurred += lerp(normal, normalM, s) * blurWeights[i];
        }
        
        // Renormalize then pack.
        return float4(normalize(normalBlurred) * 0.5f + 0.5f, 0.0f);
    }
    ENDCG
        
    Pass {
        CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            
            float4 frag(v2f_img IN) : SV_TARGET {
                return BlurInDir(IN, float2(1.0f, 0.0f));
            }
        ENDCG
    }
        
    Pass{
        CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            float4 frag(v2f_img IN) : SV_TARGET {
                return BlurInDir(IN, float2(0.0f, 1.0f));
            }
        ENDCG
    }
}
}