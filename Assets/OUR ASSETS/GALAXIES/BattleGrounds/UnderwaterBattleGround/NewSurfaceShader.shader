Shader "Custom/Underwater2D"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _WaveSpeed("Wave Speed", Float) = 1.0
        _WaveStrength("Wave Strength", Float) = 0.05
        _TintColor("Tint Color", Color) = (0.0,0.5,0.7,1)
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 100
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _WaveSpeed;
                float _WaveStrength;
                float4 _TintColor;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);

                    // Use built-in _Time.y for time in seconds
                    float wave = sin(v.vertex.y * 10 + _Time.y * _WaveSpeed) * _WaveStrength;
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex) + float2(wave, 0);

                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    col.rgb = lerp(col.rgb, _TintColor.rgb, 0.3); // blend with underwater tint
                    return col;
                }
                ENDCG
            }
        }
}
