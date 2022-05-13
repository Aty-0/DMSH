Shader "Custom/BackgroundShader"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
    }

    SubShader
    {
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        
        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _Color.rgb + _SinTime / 5;
            o.Alpha = _Color.a;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
