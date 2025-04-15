Shader "Custom/HurshFadeCubemap" {
    Properties {
        _Tint ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        [Gamma] _Exposure ("Exposure", Range(0.0, 8.0)) = 1.0
        _Rotation ("Rotation", Range(0,360)) = 0
        [NoScaleOffset] _Tex ("Cubemap (HDR)", CUBE) = "grey" {}
        _Fade ("Fade", Range(0, 1)) = 0
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
            };

            samplerCUBE _Tex;
            float4 _Tint;
            float _Exposure;
            float _Rotation;
            float _Fade;

            // Vertex shader: Passes vertex positions and calculates cubemap directions.
            v2f vert (appdata_t v) {
                v2f o;
                // Transform the vertex position to clip space
                o.vertex = UnityObjectToClipPos(v.vertex);
                // For skybox rendering, the cube texture coordinates can often be derived from the vertex.
                // Here we simply pass the object space position as the direction vector.
                o.texcoord = v.vertex.xyz;
                return o;
            }

            // Fragment shader: Samples the cubemap, applies tint, exposure, and then fades to black.
            fixed4 frag (v2f i) : SV_Target {
                // Sample the cubemap texture.
                fixed4 col = texCUBE(_Tex, i.texcoord);

                // Apply tint and exposure.
                col *= _Tint;
                col *= _Exposure;

                // Blend between the cubemap color and black based on the _Fade parameter.
                col.rgb = lerp(col.rgb, float3(0, 0, 0), _Fade);



                col.a = 1.0;
                return col;
            }
            ENDCG
        }
    }
    Fallback Off
}
