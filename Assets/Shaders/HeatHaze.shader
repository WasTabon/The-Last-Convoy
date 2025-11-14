Shader "Custom/HeatHaze"
{
    Properties
    {
        _DistortionStrength ("Distortion Strength", Range(0, 0.1)) = 0.02
        _Speed ("Speed", Range(0, 5)) = 1
        _Scale ("Noise Scale", Range(0.1, 10)) = 2
        _VerticalSpeed ("Vertical Speed", Range(0, 2)) = 0.5
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        
        Pass
        {
            Name "HeatHaze"
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };
            
            float _DistortionStrength;
            float _Speed;
            float _Scale;
            float _VerticalSpeed;
            
            // Simple noise function
            float noise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            // Improved noise with interpolation
            float smoothNoise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f); // Smoothstep
                
                float a = noise(i);
                float b = noise(i + float2(1.0, 0.0));
                float c = noise(i + float2(0.0, 1.0));
                float d = noise(i + float2(1.0, 1.0));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }
            
            // Fractal Brownian Motion for better looking noise
            float fbm(float2 uv)
            {
                float value = 0.0;
                float amplitude = 0.5;
                
                for(int i = 0; i < 4; i++)
                {
                    value += amplitude * smoothNoise(uv);
                    uv *= 2.0;
                    amplitude *= 0.5;
                }
                
                return value;
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.screenPos = ComputeScreenPos(output.positionHCS);
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                
                // Animated noise for heat distortion
                float time = _Time.y * _Speed;
                float2 noiseUV = input.uv * _Scale;
                noiseUV.y += time * _VerticalSpeed; // Vertical movement (heat rises)
                
                // Create distortion using FBM noise
                float noiseValue1 = fbm(noiseUV + float2(time * 0.3, 0));
                float noiseValue2 = fbm(noiseUV * 1.5 + float2(0, time * 0.5));
                
                // Combine noise for more complex pattern
                float2 distortion = float2(noiseValue1, noiseValue2) - 0.5;
                distortion *= _DistortionStrength;
                
                // Sample the screen with distortion
                float2 distortedUV = screenUV + distortion;
                float3 sceneColor = SampleSceneColor(distortedUV);
                
                return half4(sceneColor, 1);
            }
            ENDHLSL
        }
    }
}
