Shader "AltTrees/Leaves Bumped"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}

		_Cutoff("Alpha cutoff", Range(0,1)) = 0.3
		[MaterialEnum(Off,0,Front,1,Back,2)] _Cull("Cull", Int) = 2

		[PerRendererData]_HueVariationLeave("Hue Variation Leave", Color) = (1.0,0.5,0.0,0.0)
		[PerRendererData]_Alpha("Alpha", Range(0,1)) = 1
		[PerRendererData]_Ind("Ind", Range(0,1)) = 0
	}

	//  SM 3.0+
	SubShader
	{
		Tags{ "RenderType" = "Opaque" "IgnoreProjector" = "True" }
		LOD 400
		Cull[_Cull]

		CGPROGRAM

			#pragma surface surf Lambert fullforwardshadows nolightmap noforwardadd
			#pragma target 3.0
			#pragma multi_compile __ CROSSFADE
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

			sampler2D _MainTex;
			sampler2D _BumpMap;
			half _Cutoff;
			float _Alpha;
			float _Ind;
			half4 _HueVariationLeave;

			struct Input {
				float2 uv_MainTex;
				float4 screenPos;
			};

			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutput o)
			{
				#ifdef CROSSFADE
					CrossFadeUV(IN.screenPos, _Alpha, _Ind);
				#endif

				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				if (c.a <= _Cutoff)
					discard;


				half3 shiftedColor = lerp(c.rgb, _HueVariationLeave.rgb, _HueVariationLeave.a);
				half maxBase = max(c.r, max(c.g, c.b));
				half newMaxBase = max(shiftedColor.r, max(shiftedColor.g, shiftedColor.b));
				maxBase /= newMaxBase;
				maxBase = maxBase * 0.5f + 0.5f;
				shiftedColor.rgb *= maxBase;
				c.rgb = saturate(shiftedColor);



				o.Albedo = c.rgb * _Color.rgb;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			}

		ENDCG


		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }

			CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile __ CROSSFADE
				#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

				sampler2D _MainTex;
				float _Alpha;
				float _Ind;
				half _Cutoff;

				struct v2f
				{
					V2F_SHADOW_CASTER;
					half2 uv : TEXCOORD1;
					float4 screenPos : TEXCOORD3;
				};

				v2f vert(appdata_full v)
				{
					v2f o;
					o.screenPos = ComputeScreenPos(mul(UNITY_MATRIX_MVP, v.vertex));
					o.uv = v.texcoord.xy;

					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)

					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					fixed4 c = tex2D(_MainTex, i.uv);
					if (c.a <= _Cutoff)
						discard;

					#ifdef CROSSFADE
						CrossFadeUV(i.screenPos, _Alpha, _Ind);
					#endif
					SHADOW_CASTER_FRAGMENT(i)
				}

			ENDCG
		}
	}

	//  SM 2.0
	SubShader
	{
		Tags{ "RenderType" = "Opaque" "IgnoreProjector" = "True" }
		LOD 400
		Cull[_Cull]

		CGPROGRAM

			#pragma surface surf Lambert fullforwardshadows nolightmap noforwardadd
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

			sampler2D _MainTex;
			sampler2D _BumpMap;
			half _Cutoff;
			float _Alpha;
			float _Ind;
			half4 _HueVariationLeave;

			struct Input {
				float2 uv_MainTex;
				float4 screenPos;
			};

			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				if (c.a <= _Cutoff)
					discard;


				half3 shiftedColor = lerp(c.rgb, _HueVariationLeave.rgb, _HueVariationLeave.a);
				half maxBase = max(c.r, max(c.g, c.b));
				half newMaxBase = max(shiftedColor.r, max(shiftedColor.g, shiftedColor.b));
				maxBase /= newMaxBase;
				maxBase = maxBase * 0.5f + 0.5f;
				shiftedColor.rgb *= maxBase;
				c.rgb = saturate(shiftedColor);



				o.Albedo = c.rgb * _Color.rgb;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			}

		ENDCG


		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }

			CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

				sampler2D _MainTex;
				float _Alpha;
				float _Ind;
				half _Cutoff;

				struct v2f
				{
					V2F_SHADOW_CASTER;
					half2 uv : TEXCOORD1;
					float4 screenPos : TEXCOORD3;
				};

				v2f vert(appdata_full v)
				{
					v2f o;
					o.screenPos = ComputeScreenPos(mul(UNITY_MATRIX_MVP, v.vertex));
					o.uv = v.texcoord.xy;

					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)

					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					fixed4 c = tex2D(_MainTex, i.uv);
					if (c.a <= _Cutoff)
						discard;

					SHADOW_CASTER_FRAGMENT(i)
				}

			ENDCG
		}
	}

	FallBack "Transparent/Cutout/VertexLit"
}
