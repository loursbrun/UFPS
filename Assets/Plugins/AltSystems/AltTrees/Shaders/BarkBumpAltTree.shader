Shader "AltTrees/Bark Bumped"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		
		[MaterialEnum(Off,0,Front,1,Back,2)] _Cull("Cull", Int) = 2

		[PerRendererData]_HueVariationBark("Hue Variation Bark", Color) = (1.0,0.5,0.0,0.0)
		[PerRendererData]_Alpha("Alpha", Range(0,1)) = 1
		[PerRendererData]_Ind("Ind", Range(0,1)) = 0
	}

		
	//  SM 3.0+
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 400
		Cull[_Cull]

		CGPROGRAM
			#pragma surface surf Lambert fullforwardshadows nolightmap noforwardadd
			#pragma target 3.0
			#pragma multi_compile __ CROSSFADE
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

			sampler2D _MainTex;
			sampler2D _BumpMap;
			float _Alpha;
			float _Ind;
			half4 _HueVariationBark;

			struct Input
			{
				float2 uv_MainTex;
				float4 screenPos;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutput o)
			{
				#ifdef CROSSFADE
					CrossFadeUV(IN.screenPos, _Alpha, _Ind);
				#endif
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);


				half3 shiftedColor = lerp(c.rgb, _HueVariationBark.rgb, _HueVariationBark.a);
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
	}
			
	//  SM 2.0
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 400
		Cull[_Cull]

		CGPROGRAM
			#pragma surface surf Lambert fullforwardshadows nolightmap noforwardadd
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

			sampler2D _MainTex;
			sampler2D _BumpMap;
			float _Alpha;
			float _Ind;
			half4 _HueVariationBark;

			struct Input
			{
				float2 uv_MainTex;
				float4 screenPos;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

				half3 shiftedColor = lerp(c.rgb, _HueVariationBark.rgb, _HueVariationBark.a);
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
	}
	FallBack "Diffuse"
}
