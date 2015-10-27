﻿Shader "Hidden/DistanceField"
{
	Properties {
		PointCount ("Point Count", Int) = 0
		ScreenSize ("Screen Size", Vector) = (0, 0, 0, 0)
	}
	SubShader {
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

			CGINCLUDE
			#ifdef SHADER_API_D3D11
			uint PointCount;
			float4 ScreenSize;
			StructuredBuffer<float2> PointBuf;
			#endif

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 posPixel : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.posPixel = v.uv * ScreenSize.xy;
				return o;
			}
			ENDCG	

		Pass {
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
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
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			float4 frag (v2f IN) : SV_Target {
				float4 d = 0;
				#ifdef SHADER_API_D3D11
				
				#endif
				return d;
			}
			ENDCG
		}
	}
}