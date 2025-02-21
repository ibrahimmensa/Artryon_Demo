Shader "BNB/StaticPos"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull off

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
                float4 vertex : SV_POSITION;
                float3 pos : STATIC_POS;
            };

            v2f vert (appdata v)
            {
                v2f o;

                float2 vert = v.uv * 2.0 - 1;
                //vert.y = -vert.y;
                o.vertex = float4(vert, 0, 1);
                o.pos = v.vertex.xyz;

                return o;

            }

            fixed4 frag (v2f i) : SV_Target
            {
                return float4(i.pos, 1);
            }
            ENDCG
        }
    }
}
