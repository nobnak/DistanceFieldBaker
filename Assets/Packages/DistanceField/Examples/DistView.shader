Shader "Unlit/DistView" {
	Properties {
		_DistTex ("Dist Texture", 2D) = "white" {}
		_NormTex ("Norm Texture", 2D) = "white" {}
		_NoiseTex ("Noise Texture", 2D) = "white" {}
		_Filter ("Filter", Vector) = (1, 0, 0, 0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100 ZTest LEqual ZWrite On Cull Off

		Pass {
			CGPROGRAM
			#define LOOP_COUNT 4
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
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
			sampler2D _NoiseTex;
			float4 _DistTex_ST;
			float4 _NormTex_ST;
			float4 _NoiseTex_ST;
			float4 _NoiseTex_TexelSize;
			float4 _Filter;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _DistTex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target {
				float3 d = 0;
			
				float cd = tex2D(_DistTex, i.uv).x;
				float2 cn = tex2D(_NormTex, i.uv).xy;
				
				d += float3(_Filter.x * cn, 0);
				
				d += _Filter.y * cd;
				
				float2 v = normalize(2 * (cn - 0.5)) * _NoiseTex_TexelSize.xy;
				float2 uv = i.uv;
				float3 cl = tex2D(_NoiseTex, uv).xyz;
				for (uint i = 0; i < LOOP_COUNT; i++) {
					cl += tex2D(_NoiseTex, uv + i * v);
					cl += tex2D(_NoiseTex, uv - i * v);
				}
				d += _Filter.z * 
				
				return float4(d, 1);
			}
			ENDCG
		}
	}
}