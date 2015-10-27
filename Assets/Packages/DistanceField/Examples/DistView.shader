Shader "Unlit/DistView" {
	Properties {
		_DistTex ("Dist Texture", 2D) = "white" {}
		_NormTex ("Norm Texture", 2D) = "white" {}
		_Scale ("Scale", Vector) = (1, 0, 0, 0)
		_Filter ("Filter", Vector) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Overlay" }
		LOD 100 ZTest Always ZWrite Off Cull Off

		Pass {
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile FIT_SCREEN_OFF FIT_SCREEN_ON
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _DistTex;
			sampler2D _NormTex;
			float4 _DistTex_ST;
			float4 _NormTex_ST;
			float4 _Scale;
			float4 _Filter;
			
			v2f vert (appdata v) {
				v2f o;
				#ifdef FIT_SCREEN_ON
				o.vertex = float4(v.uv * 2 - 1, 0, 1);
				#else
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				#endif
				o.uv = TRANSFORM_TEX(v.uv, _DistTex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target {
				float4 c = tex2D(_DistTex, i.uv);
				float4 d = float4(fmod(_Scale.x * c.x, 1), _Scale.y * c.y, 0, 1);
				d.x = (d.x < 0.1 ? 1 : 0);
				d.y = (d.y > 0.9 ? 1 : 0);
				d *= _Filter;
				return d;
			}
			ENDCG
		}
	}
}
