Shader "Custom/BackgroundShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,1)
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            float4 circle(float2 uv, float2 pos, float rad, float3 color)
            {
                float d = length(pos - uv) - rad;
                float t = clamp(d, 0.0, 1.0);
                return float4(color.x + 1.0 - t, color.y, color.z, 1);
            }


            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            #define mod(x, y) (x - y * floor(x / y))

            float3 circle(float2 uv, float clampedSinTime, float aspect)
            {
                float sin_factor = sin(_Time.w) / 10;
                float cos_factor = cos(_Time.w) / 10;

                uv = (uv - 0.5) * .4f + 0.5;                
                //uv += mul(float2((uv.x - 0.5) * aspect, uv.y - 0.5),  float2x2(cos_factor, sin_factor, -sin_factor, -cos_factor));

                uv = float2((uv.x - 0.5) * aspect, uv.y - 0.5);

                float d = sqrt(dot(uv, uv));
                float radius = 0.01f;   
                float thickness =  0.07f;
                float t = 1 - mod(smoothstep(abs(thickness / radius) * 100, 0, abs(radius - d)), 1.0f);

                //t -= frac(0.5f * clampedSinTime);

                return t;
            }     

            float3 hash3(float2 p)
            {
                float3 q = float3(dot(p, float2(127.1, 311.7)),
                    dot(p, float2(269.5, 183.3)),
                    dot(p, float2(419.2, 371.9)));

                return frac(sin(q) * 43758.5453);
            }

            float t = 1;
            float t2 = 1;
            sampler2D _MainTex;

            fixed4 frag(v2f input) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, input.uv);

                float time = _Time.w;
                float clampedSinTime = clamp(_SinTime.w, _Time.x / 500, _Time.y * 1000);
                float aspect = _ScreenParams.x / _ScreenParams.y;

                float t2 = 0;
                t2 += hash3(input.uv);

                input.uv.x += tan(time / 40);
                input.uv.y += tan(time / 100);
                float3 moon = float3(1, 1, 1);
                moon *= circle(float2(input.uv.x + 0.45f, input.uv.y - 0.45f), clampedSinTime, aspect);
                moon /= frac(circle(float2(input.uv.x + 0.55f, input.uv.y - 0.45f), clampedSinTime, aspect));
                moon = moon - 0.1f;



                col -= float4(moon, 1);


                //col -= float4(t2, t2, t2, 1);

                return col;
            }
            ENDCG
        }  
    }
}
