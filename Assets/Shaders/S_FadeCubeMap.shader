Shader "Custom/WipeFadeCubeMap" {
   Properties {
        _Tint ("Tint Color", Color) = (0.5,0.5,0.5,1)
        [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
        _Rotation ("Rotation", Range(0,360)) = 0
        [NoScaleOffset] _Tex ("Cubemap (HDR)", CUBE) = "grey" {}
        _Fade ("Fade", Range(0, 1)) = 0
        _Transition ("Transition Width", Range(0.001, 0.5)) = 0.1  // Avoid 0 to prevent division issues.
        _Falloff ("Falloff Exponent", Range(0.1, 5)) = 1.0  // Adjusts the gradient's steepness.
    }
    SubShader {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Pass {
            ZWrite Off
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex   : SV_POSITION;
                float3 texcoord : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            samplerCUBE _Tex;
            float4 _Tint;
            float _Exposure;
            float _Rotation;
            float _Fade;
            float _Transition;
            float _Falloff;
            
            v2f vert (appdata_t v) {
                v2f o;
                // Transform vertex into clip space.
                o.vertex = UnityObjectToClipPos(v.vertex);
                // Get screen-space coordinates.
                o.screenPos = ComputeScreenPos(o.vertex);
                // Pass vertex position for cubemap sampling.
                o.texcoord = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Sample the cubemap and apply tint/exposure.
                fixed4 col = texCUBE(_Tex, i.texcoord);
                col *= _Tint;
                col *= _Exposure;

                // Retrieve normalized screen UV coordinates.
                // (ComputeScreenPos already returns values that can be normalized by dividing by w.)
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                float screenY = screenUV.y; // 0 = bottom, 1 = top

                // Calculate the wipe line.
                // When _Fade == 0, wipeLine = 1, and the transition is [1, 1 + _Transition] (mostly above the screen).
                // When _Fade == 1, wipeLine = -_Transition, and the transition runs from -_Transition to 0,
                // which places the entire gradient below or at the bottom of the screen.
                float wipeLine = lerp(1.0, -_Transition, _Fade);

                // Compute t as the normalized position within the transition band.
                float t = saturate((screenY - wipeLine) / _Transition);
                // Adjust the gradient with an exponent for stronger (or softer) transparency change.
                float fadeFactor = 1.0 - pow(t, _Falloff);

                // Apply the computed fade factor.
                col.rgb *= fadeFactor;
                col.a = 1.0;
                return col;
            }
            ENDCG
        }
    }
    Fallback Off
}