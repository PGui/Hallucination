Shader "Custom/StripesEffect"
{
	Properties
	{
		_BackgroundColor("Background color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog{ Mode off }
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma glsl
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma target 3.0 
			#include "UnityCG.cginc"

			float4x4 _FrustumCornersWPos;
			float4 _CameraWPos;
			sampler2D _CameraDepthTexture;
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float4 _BackgroundColor;

			#define MAX_STRIPES 50
			float _StripeNum;
			float4 _StripesColor[MAX_STRIPES];
			float4 _StripesCenter[MAX_STRIPES];
			float4 _Stripes[MAX_STRIPES];

			struct v2f
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv_depth : TEXCOORD1;
				float4 interpolatedRay : TEXCOORD2;
			};

			v2f vert(appdata_img v)
			{
				v2f o;
				half index = v.vertex.z;
				v.vertex.z = 0.1f;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;
				o.uv_depth = v.texcoord.xy;
				
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv.y = 1-o.uv.y;
				#endif				
				
				o.interpolatedRay = _FrustumCornersWPos[(int)index];
				o.interpolatedRay.w = index;
				
				return o;
			}

			half4 frag(v2f input) : COLOR
			{
				float dpth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, input.uv_depth)));
				float4 wsDir = dpth * input.interpolatedRay;
				float4 wsPos = _CameraWPos + wsDir;

				float radius;
				float size;
				float dist;
				bool isLit = false;
				bool currentLit;
				float alpha = 0.0f;

				for(int i=0; i<_StripeNum; ++i)
				{
					radius = _Stripes[i].x;
					size = _Stripes[i].y;
					dist = distance(wsPos, float4(_StripesCenter[i].xyz, 1));

					currentLit = false;
					if(dist < radius - size)
					{
						isLit = true;
						currentLit = true;
						alpha = max(alpha, _Stripes[i].z);
					}
					else if(dist < radius && !currentLit)
					{
						return lerp(_BackgroundColor, _StripesColor[i], _Stripes[i].z);
					}
				}

				if(isLit) 
				{
					return lerp(_BackgroundColor, tex2D(_MainTex, input.uv), alpha);
				}
				else
				{
					return _BackgroundColor;
				}
			}

			ENDCG
		}
	}
}