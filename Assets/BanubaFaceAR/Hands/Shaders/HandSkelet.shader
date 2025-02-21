Shader "Unlit/HandSkelet"
{
    Properties
    {
        _Color("color", Color) = (0.0, 1.0, 0.0, 1)
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
                float size: PSIZE;
            };

            float4 _Color;
            float4x4 _MV;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = mul(_MV, v.vertex);
                o.vertex.z = 1.0;
                o.size = 10.0;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}
