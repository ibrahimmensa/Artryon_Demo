Shader "BNB/CustomMorphDraw"
{
    Properties
    {
        _MorphTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"}
        LOD 100
        ZTest less
        ZWrite off

        cull back

        Pass
        {  
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                uint vid : SV_VertexID;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 var_c : VAR_C;
                //float4 norm : NORMAL;
            };

            texture2D _MorphTex;
            int _DrawID;
            uniform float _morphsWights[37];

            v2f vert (appdata v)
            {
                v2f o;
                float3 vpos = v.vertex.xyz;
                uint vertexID = v.vid;

                uint2 ij = uint2(vertexID % (3308 / 2), vertexID / (3308 / 2));
                float2 delta = float2(0, 0);

                for(int bsi = 0; bsi < 37; ++bsi){
                      delta += _MorphTex.Load(float3(vertexID, bsi, 0)).xy * _morphsWights[bsi];
                }

                vpos.xy += delta;

                o.uv = v.uv;
                //o.norm = v.vertex;
                const int EXPAND_PASSES = 8;
                const float NPUSH = 75.;
                float scale = 1.0 - float(_DrawID)/float(EXPAND_PASSES+1);
                scale = scale*scale*(3. - 2.*scale);
                float d0 = float(_DrawID)/float(EXPAND_PASSES+1);
                float d1 = float(_DrawID+1)/float(EXPAND_PASSES+1);
                float4 npush_scale = float4(NPUSH*float(_DrawID)/float(EXPAND_PASSES), scale*0.5, (d1-d0)*0.5, (d0+d1)*0.5);

                o.vertex = UnityObjectToClipPos(float4( vpos * (1.0 + npush_scale.x / length(vpos)), 1.0 ));
                o.vertex.z = o.vertex.z*npush_scale.z + o.vertex.w*npush_scale.w;

                float4 pos_no_push = UnityObjectToClipPos(float4( vpos, 1. ));
                float4 original_pos =UnityObjectToClipPos(v.vertex);
                o.var_c = npush_scale.y*(original_pos.xy/original_pos.w - pos_no_push.xy/pos_no_push.w);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                //float3 N = normalize(cross(ddx(i.norm), ddy(i.norm)));
                //float l = dot(N, float3(0., 0.8, 0.6)) * 0.5 + 0.5;
                //return float4(l * float3(1,0,0), 1.);

                return float4(i.var_c, 0, 1);
            }
            ENDCG
        }
    }
}
