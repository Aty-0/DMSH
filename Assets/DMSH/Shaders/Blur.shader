Shader "Custom/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Directions("Directions", Range(0,100)) = 16
        _Quality("Quality", Range(0,10)) = 4
        _Size("Size", Range(0,100)) = 20
        
    }
    SubShader
    {
        // No culling or depth
        //Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
   
               return o;
            }

            sampler2D _MainTex;
            float       _Directions;
            float      _Quality;
            float      _Size;

            fixed4 frag (v2f i) : SV_Target
            {
                //Original version of shader
                //https://www.shadertoy.com/view/Xltfzj

                fixed4 col = tex2D(_MainTex, i.uv);

                float Pi = 6.28318530718;       // Pi*2
                float Directions = _Directions; // BLUR DIRECTIONS (Default 16.0 - More is better but slower)
                float Quality = _Quality;       // BLUR QUALITY (Default 4.0 - More is better but slower)
                float Size = _Size;             // BLUR SIZE (Radius)
                float2 Radius = Size / _ScreenParams.xy;
                float2 uv = i.uv;

                // Pixel colour
                float4 Color = tex2D(_MainTex, uv);

                // Blur calculations
                for (float d = 0.0; d < Pi; d += Pi / Directions)
                    for (float i = 1.0 / Quality; i <= 1.0; i += 1.0 / Quality)
                        Color += tex2D(_MainTex, uv + float2(cos(d), sin(d)) * Radius * i);
        

                // Output to screen
                Color /= Quality * Directions - 15.0;
                col.rgb = Color;

                return col;
            }
            ENDCG
        }
    }
}
