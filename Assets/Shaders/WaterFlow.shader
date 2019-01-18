/*
Shader originally created by Markus Werner. Used with permission.
Modified for Scorch by Leon Arndt.
*/


Shader "Scorch/Unlit/WaterFlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _xScroll ("Up Down Scrolling", float) = 0
        _yScroll ("Left Right Scrolling", float) = 0
        _Tint    ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaqu"}
       
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 position : SV_POSITION;
            };
 
            float _yScroll;
            float _xScroll;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Tint;
           
            v2f vert (appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.position);
                o.uv.x = v.uv.x - _xScroll * _Time.y;
                o.uv.y = v.uv.y - _yScroll * _Time.y;
                return o;
            }
           
            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 col = tex2D(_MainTex, i.uv) * _Tint;
                return col;
            }
            ENDCG
        }
    }
}