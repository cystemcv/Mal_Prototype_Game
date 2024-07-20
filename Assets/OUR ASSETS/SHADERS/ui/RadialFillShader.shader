Shader "Custom/RadialFillWithTransparencyShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _FillAmount("Fill Amount", Range(0,1)) = 0.0
        _Center("Center", Vector) = (0.5, 0.5, 0, 0)
        _StartAngle("Start Angle", Range(0,1)) = 0.0
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
            LOD 100
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
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

                sampler2D _MainTex;
                float _FillAmount;
                float4 _Center;
                float _StartAngle;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    float2 center = _Center.xy;
                    float2 uv = i.uv;
                    float angle = atan2(uv.y - center.y, uv.x - center.x) / (2 * UNITY_PI) + 0.5;
                    angle = fmod(angle - _StartAngle + 1.0, 1.0); // Adjust angle to start from _StartAngle
                    half4 texColor = tex2D(_MainTex, uv);
                    if (angle > _FillAmount)
                        texColor.a = 0;
                    return texColor;
                }
                ENDCG
            }
        }
}
