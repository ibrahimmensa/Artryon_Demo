Shader "BNB/CameraStencilMask" {
	Properties {
        _MainTex ("Main Tex", 2D) = "white" {}
		_BNBCameraStencil("_BNBCameraStencil", Int) = 0
	}

	SubShader {
		Tags {
			"RenderType" = "Opaque"
			"Queue" = "Geometry-100"
		}

		ColorMask 0
		ZWrite off
        Cull Front

		Stencil {
			Ref [_BNBCameraStencil]
			Comp always
			Pass replace
		}

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 frag(v2f i) : COLOR {
				return half4(1, 1, 0, 1);
			}

			ENDCG
		}
	}
}