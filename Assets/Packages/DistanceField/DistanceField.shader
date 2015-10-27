Shader "Hidden/DistanceField"
{
	Properties {
		PointCount ("Point Count", Int) = 0
		ScreenSize ("Screen Size", Vector) = (0, 0, 0, 0)
		_DistTex ("Dist Texture", 2D) = "white" {}
	}
	SubShader {
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

			CGINCLUDE
			#pragma target 5.0			
			#include "UnityCG.cginc"
			
			sampler2D _DistTex;
			sampler2D _NormTex;
			float4 _DistTex_ST;
			float4 ScreenSize;
			uint PointCount;
			#ifdef SHADER_API_D3D11
			StructuredBuffer<float2> PointBuf;
			#endif

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float2 posPixel : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.posPixel = v.uv * ScreenSize.xy;
				return o;
			}
			ENDCG	

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			float4 frag (v2f IN) : SV_Target {
				float4 d = 0;
				#ifdef SHADER_API_D3D11
				float minSqrDist = dot(ScreenSize.xy, ScreenSize.xy);
				float meta = 0;
				for (uint i = 0; i < PointCount; i++) {
					float2 p = PointBuf[i];
					float2 dir = p - IN.posPixel;
					float sqrDist = dot(dir, dir);
					if (sqrDist < minSqrDist)
						minSqrDist = sqrDist;
					
					meta += 1.0 / sqrDist;
				}
				d.x = sqrt(minSqrDist);
				d.y = meta;
				#endif
				return d;
			}
			ENDCG
		}
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			float4 frag (v2f IN) : SV_Target {
				float2 c = tex2D(_DistTex, IN.uv).xy;
				float2 n = normalize(float2(ddx(c.y), ddy(c.y)));
				return float4(n, 0, 1);
			}
			ENDCG
		}
	}
}
