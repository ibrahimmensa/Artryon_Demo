Shader "BNB/Makeup/Smoothing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
            sampler2D _CameraTex;
            sampler2D _BlurTex;


            fixed4 frag (v2f i) : SV_Target
            {

                float UnsharpAmount = 0.1;
                float UnsharpThreshold = 0.1;
                float ALPHA_MULTIPLIER = 0.4;

              	float3 original = tex2D(_CameraTex, i.uv).rgb;
	            float3 gauss = tex2D(_MainTex, i.uv).rgb;

	            float alpha = ALPHA_MULTIPLIER;


	            float3 difference = gauss - original;
	            float3 curve = clamp(original, 0.0, 1.0);
	            float val2 = clamp(length(original.gb) - length(gauss.gb) + 0.5, 0.0, 1.0);
	            float2 case1 = float2(val2, 1.0 - val2);
	            case1 *= case1;
	            case1 *= case1;
	            case1 = case1 * case1 * 128.0;

	            float val2mixAmount = step(val2, 0.5);
	            val2 = lerp(1.0 - case1.y, case1.x, val2mixAmount);
	            float3 origCurve = lerp(curve, original, val2);
	            float mixAmount = step(UnsharpThreshold * UnsharpThreshold, dot(difference, difference));
	            float3 smoothCol = origCurve + (mixAmount * UnsharpAmount) * difference;
                //return float4(gauss, 1.0);
	            return float4(lerp(original, smoothCol, alpha), 1.0);
            }
            ENDCG
        }
    }
}
