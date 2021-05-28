Shader "Unlit/StarUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Aberration ("Aberration", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
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
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float colR = tex2D(_MainTex, i.uv - _Aberration).r * _Color;
                float colG = tex2D(_MainTex, i.uv).g * _Color;
                float colB = tex2D(_MainTex, i.uv + _Aberration).b * _Color;
                
                //fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                //color.r = colR;
                //color.b = colB;
                //color.rgb *= color.a;
                return float4(colR, colG, colB, 1);
            }
            ENDCG
        }
    }
}
