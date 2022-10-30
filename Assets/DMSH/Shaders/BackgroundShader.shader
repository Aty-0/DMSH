Shader "Custom/BackgroundShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,1)
        _GameActive ("GameActive", Range(0, 1)) = 0
    }

    SubShader
    {
        Pass
        {
            Tags
            {
                "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;

            v2f vert(const appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            float t = 1;
            float t2 = 1;
            int _GameActive = 1;
            sampler2D _MainTex;

            float3 G(float2 uv, float clamped_sin_time, float aspect)
            {
                const float sin_factor = sin(_Time.w) / 60;
                float cos_factor = cos(_Time.w) / 60;
                uv = (uv - 0.5) * .4f + 0.5;
                uv += mul(float2((uv.x - 0.5) * aspect, uv.y - 0.5),
                          float2x2(cos_factor, sin_factor - _SinTime.w, -sin_factor, -tan(cos_factor)));
                uv = float2((uv.x - 0.5) * aspect, uv.y - 0.5);
                const float d = sqrt(dot(uv, uv));
                const float radius = 0.006f;
                const float thickness = 0.07f;
                const float t = 1 - fmod(smoothstep(abs(thickness / radius) * 100, 0, abs(-radius - d)), 1.0f);
                return t;
            }

            float3 hash3(float2 p)
            {
                const float3 q = float3(dot(p, float2(127.1, 311.7)),
                                        dot(p, float2(269.5, 183.3)),
                                        dot(p, float2(419.2, 371.9)));

                return frac(sin(q) * 43758.5453);
            }

            fixed4 frag(v2f input) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, input.uv);

                const float time = _Time.w;
                const float clampedSinTime = clamp(_SinTime.w, _Time.x / 500, _Time.y * 1000);
                const float aspect = _ScreenParams.x / _ScreenParams.y;

                float t2 = 0;
                t2 += dot(hash3(input.uv + time), 0.05f);

                input.uv.x -= clampedSinTime / 10;
                //input.uv.y -= tan(time / 5);

                float3 f = float3(1, 1, 1);
                f *= G(float2(input.uv.x + 0.45f, input.uv.y - 0.45f), clampedSinTime, aspect);
                f /= frac(G(float2(input.uv.x + 0.55f, input.uv.y - 0.45f), clampedSinTime, aspect));

                f *= G(float2(1 - input.uv.x + 0.45f, 1 - input.uv.y - 0.45f), clampedSinTime, aspect);
                f /= frac(G(float2(1 - input.uv.x + 0.55f, 1 - input.uv.y - 0.45f), clampedSinTime, aspect));


                f = f * 1.3f;
                float s = dot(_SinTime, 0.02f);
                col -= float4(f, 1);
                col += float4(0.2f * s, s, s, 1) - .3f;
                col += float4(t2, t2, t2, 1);

                return col;
            }
            ENDCG
        }
    }
}