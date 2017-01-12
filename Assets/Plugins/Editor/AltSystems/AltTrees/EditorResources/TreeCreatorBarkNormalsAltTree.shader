Shader "Hidden/AltTrees/TreeCreatorBarkNormals"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Range(0.01, 1)) = 0.078125
		_MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_BumpScale("Scale", Float) = 1.0
		_GlossMap("Gloss (A)", 2D) = "black" {}


		[PerRendererData]_HueVariationBark("Hue Variation Bark", Color) = (1.0,0.5,0.0,0.0)
		[PerRendererData]_Alpha("Alpha", Range(0,1)) = 1
		[PerRendererData]_Ind("Ind", Range(0,1)) = 0

		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		[HideInInspector] _TreeInstanceColor("TreeInstanceColor", Vector) = (1,1,1,1)
		[HideInInspector] _TreeInstanceScale("TreeInstanceScale", Vector) = (1,1,1,1)
		[HideInInspector] _SquashAmount("Squash", Float) = 1
	}

	SubShader
	{
		Tags
		{
			"IgnoreProjector" = "True" "RenderType" = "TreeBark"
		}
		LOD 200

		CGPROGRAM
			#pragma surface surf NoLighting vertex:TreeVertBark nolightmap
			#pragma target 3.0
			#pragma multi_compile __ ENABLE_ALTWIND
			#define NORMALMAP_MODE
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/UnityBuiltin3xTreeLibraryAltTree.cginc"

			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _GlossMap;
			half _Shininess;
			float _Alpha;
			float _Ind;
			half4 _HueVariationBark;
			float _BumpScale;

			struct Input
			{
				float2 uv_MainTex;
				fixed4 color : COLOR;
				#ifdef NORMALMAP_MODE
					float3 worldNormal;
					INTERNAL_DATA
				#endif
			};

			fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
			{
				return fixed4(0, 0, 0, 0);
			}

			half3 ScaleNormal(half3 packednormal, half bumpScale)
			{
				packednormal.xy *= bumpScale;
				return packednormal;
			}

			void surf(Input IN, inout SurfaceOutput o)
			{
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

				//o.Emission = (-1 * ScaleNormal(WorldNormalVector(IN, o.Normal), _BumpScale) + 1) / 2.0;

				half3 wNormal = WorldNormalVector(IN, o.Normal);
				wNormal = half3(-wNormal.x, wNormal.y, wNormal.z);
				if (length(wNormal) == 0)
					wNormal = float3(0, 0, 1);
				o.Emission = normalize(wNormal) * 0.5 + 0.5;
				o.Emission = GammaToLinearSpace(o.Emission);

				o.Albedo = fixed4(0, 0, 0, 0);
				o.Normal = float3(0, 0, 1);
			}
		ENDCG
	}
	FallBack "Diffuse"
}
