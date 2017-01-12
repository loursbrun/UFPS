Shader "Hidden/AltTrees/SpeedTreeNormals"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0,0,0,0)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.1
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_DetailTex ("Detail", 2D) = "black" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpScale("Scale", Float) = 1.0
		_Cutoff ("Alpha Cutoff", Range(0,1)) = 0.333
		[MaterialEnum(Off,0,Front,1,Back,2)] _Cull ("Cull", Int) = 2
		[MaterialEnum(None,0,Fastest,1,Fast,2,Better,3,Best,4,Palm,5)] _WindQuality ("Wind Quality", Range(0,5)) = 0
		
		[MaterialEnum(Leave,0,Bark,1)] _Type ("Type", Int) = 0


	}

	SubShader
	{
		Tags
		{
			"Queue"="Geometry"
			"IgnoreProjector"="True"
			"RenderType"="Opaque"
			"DisableBatching"="True"
		}
		LOD 400
		Cull [_Cull]

		CGPROGRAM
			#pragma surface surf NoLighting vertex:SpeedTreeVert nolightmap
			#pragma target 3.0
			#pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
			#pragma shader_feature EFFECT_BUMP
			#define NORMALMAP_MODE
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/SpeedTreeCommonAltTree.cginc"

            float _Alpha;
            float _Ind;
			float _BumpScale;


			fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
			{
				return fixed4(0, 0, 0, 0);
			}

			half3 ScaleNormal(half3 packednormal, half bumpScale)
			{
				packednormal.xy *= bumpScale;
				return packednormal;
			}

			void surf(Input IN, inout SurfaceOutput OUT)
			{
				half4 diffuseColor = tex2D(_MainTex, IN.mainTexUV);

				OUT.Alpha = diffuseColor.a * _Color.a;
				#ifdef SPEEDTREE_ALPHATEST
					clip(OUT.Alpha - _Cutoff);
				#endif

				#ifdef GEOM_TYPE_BRANCH_DETAIL
					half4 detailColor = tex2D(_DetailTex, IN.Detail.xy);
					diffuseColor.rgb = lerp(diffuseColor.rgb, detailColor.rgb, IN.Detail.z < 2.0f ? saturate(IN.Detail.z) : detailColor.a);
				#endif


				#ifdef EFFECT_BUMP
					OUT.Normal = UnpackNormal(tex2D(_BumpMap, IN.mainTexUV));
				#else
					OUT.Normal = float3(0, 0, 1);
				#endif

				//OUT.Emission = (-1 * ScaleNormal(WorldNormalVector(IN, OUT.Normal), _BumpScale) + 1) / 2.0;
					
				half3 wNormal = WorldNormalVector(IN, OUT.Normal);
				wNormal = half3(-wNormal.x, wNormal.y, wNormal.z);
				if (length(wNormal) == 0)
					wNormal = float3(0, 0, 1);
				OUT.Emission = normalize(wNormal) * 0.5 + 0.5;
				OUT.Emission = GammaToLinearSpace(OUT.Emission);


				OUT.Albedo = fixed4(0, 0, 0, 0);
				OUT.Normal = float3(0, 0, 1);
			}
		ENDCG
	}
}
