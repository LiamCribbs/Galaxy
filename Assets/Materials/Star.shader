Shader "Unlit/Star"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
        _Aberration ("Aberration", Range(-1.0, 1.0)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Aberration;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 colG = tex2D(_MainTex, uv) * i.color;
                uv.y -= _Aberration;
                fixed4 colR = tex2D(_MainTex, uv) * i.color;
                uv.y += _Aberration + _Aberration;
                fixed4 colB = tex2D(_MainTex, uv) * i.color;
                
                return fixed4(colR.r, colG.g, colB.b, 1) * colG.a;
            }
            ENDCG
        }
    }
}
