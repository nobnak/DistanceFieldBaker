Shader "Hidden/DistanceField"
{
	Properties {
		PointCount ("Point Count", Int) = 0
		ScreenSize ("Screen Size", Vector) = (0, 0, 0, 0)
		_MainTex ("Main Texture", 2D) = "white" {}
		_Scale ("Scale", Float) = 1
	}
	SubShader {
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

			CGINCLUDE
			#pragma target 5.0
			#pragma multi_compile DISTANCE_FIELD METABALL_FIELD
			#pragma multi_compile GENERATE_TANGENT GENERATE_NORMAL
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 ScreenSize;
			float _Scale;
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
				float d = 0;
				#ifdef SHADER_API_D3D11

				#if defined(METABALL_FIELD)
				for (uint i = 0; i < PointCount; i++) {
					float2 p = PointBuf[i];
					float2 dir = p - IN.posPixel;
					float sqrDist = dot(dir, dir);
					d += 1.0 / sqrDist;
				}
				#else
				d = dot(ScreenSize.xy, ScreenSize.xy);
				for (uint i = 0; i < PointCount; i++) {
					float2 p = PointBuf[i];
					float2 dir = p - IN.posPixel;
					float sqrDist = dot(dir, dir);
					if (sqrDist < d)
						d = sqrDist;
				}
				d = sqrt(d);
				#endif
				
				#endif
				return _Scale * d;
			}
			ENDCG
		}
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			float4 frag (v2f IN) : SV_Target {
				float d = tex2D(_MainTex, IN.uv).x;
				float2 n = normalize(float2(-ddx(d), -ddy(d)));
				#if defined(GENERATE_TANGENT)
				n = float2(-n.y, n.x);
				#endif
				return float4(0.5 *n + 0.5, 0, 1);
			}
			ENDCG
		}
	}
}