Shader "Custom/TransparentFadeLit" {
    Properties {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _Fade("Fade", Range(0,1)) = 0.0
    }
    SubShader {
        Tags {
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "RenderPipeline"="UniversalPipeline" 
            "UniversalMaterialType"="Lit"
        }
        Pass {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Uniforms (material properties)
            float4 _BaseColor;
            float _Fade;

            struct Attributes {
                float3 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
            };

            Varyings vert (Attributes IN) {
                Varyings OUT;
                // Transform object-space position to homogenous clip-space
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                // Transform normal to world-space and normalize
                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target {
                // Fetch base color and fade factor
                float4 baseColor = _BaseColor;
                half fade = saturate(_Fade);

                // Lambertian (diffuse) lighting from main directional light
                Light mainLight = GetMainLight();
                half3 normalWS = normalize(IN.normalWS);
                half3 diffuse = LightingLambert(mainLight.color, mainLight.direction, normalWS);

                // Apply lighting to the base color (diffuse shading)
                half3 litColor = baseColor.rgb * diffuse;

                // Compute final alpha: base alpha scaled by (1 - fade). 
                // (Fade=1 => alphaOut=0, fully transparent)
                half alphaOut = baseColor.a * (1.0 - fade);
                return half4(litColor, alphaOut);
            }
            ENDHLSL
        }
    }
}
