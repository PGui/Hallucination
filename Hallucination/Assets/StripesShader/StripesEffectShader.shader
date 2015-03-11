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
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma glsl

			#ifdef SHADER_API_D3D9
				#pragma target 3.0
			#endif

			#include "UnityCG.cginc"

			float4x4 _FrustumCornersWPos;
			float4 _CameraWPos;
			sampler2D _CameraDepthTexture;
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float4 _BackgroundColor;

			float _TexGranularity;
			sampler2D _DataTex;

			#define MAX_STRIPES 50
			float _StripeNum;

			struct v2f
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv_depth : TEXCOORD1;
				float4 interpolatedRay : TEXCOORD2;
			};

			
			//Decodes floats in range [-5000; 5000]
			inline float RGBAToFloat(float4 rgba)
			{
				return (dot(rgba, float4(1.0, 1/255.0, 1/65025.0, 1/160581375.0)) - 0.5f)*10000.0;
			}

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

				float fade;
				float radius;
				float size;
				float dist;
				float4 color;
				bool isLit = false;
				bool currentLit;
				float alpha = 0.0f;

				for(int i=0; i<_StripeNum; ++i)
				{
					color = tex2Dlod(_DataTex, float4(_TexGranularity*i*7, 0, 0, 0));
					
					radius = RGBAToFloat(tex2Dlod(_DataTex, float4(_TexGranularity*(i*7 + 1), 0, 0, 0)));
					size = RGBAToFloat(tex2Dlod(_DataTex, float4(_TexGranularity*(i*7 + 2), 0, 0, 0)));
					fade = RGBAToFloat(tex2Dlod(_DataTex, float4(_TexGranularity*(i*7 + 3), 0, 0, 0)));

					float4 center = float4(RGBAToFloat(tex2Dlod(_DataTex, float4(_TexGranularity*(i*7 + 4), 0, 0, 0))),
										   RGBAToFloat(tex2Dlod(_DataTex, float4(_TexGranularity*(i*7 + 5), 0, 0, 0))),
										   RGBAToFloat(tex2Dlod(_DataTex, float4(_TexGranularity*(i*7 + 6), 0, 0, 0))),
										   1.0);

					dist = distance(wsPos, center);

					currentLit = false;
					if(dist < radius - size)
					{
						isLit = true;
						currentLit = true;
						alpha = max(alpha, fade);
					}
					else if(dist < radius && !currentLit)
					{
						return lerp(_BackgroundColor, color, fade);
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