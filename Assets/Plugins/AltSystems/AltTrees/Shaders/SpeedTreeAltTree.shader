Shader "AltTrees/SpeedTree"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0,0,0,0)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.1
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_DetailTex ("Detail", 2D) = "black" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_Cutoff ("Alpha Cutoff", Range(0,1)) = 0.333
		[MaterialEnum(Off,0,Front,1,Back,2)] _Cull ("Cull", Int) = 2
		[MaterialEnum(None,0,Fastest,1,Fast,2,Better,3,Best,4,Palm,5)] _WindQuality ("Wind Quality", Range(0,5)) = 0
		
		[MaterialEnum(Leave,0,Bark,1)] _Type ("Type", Int) = 0
		[PerRendererData]_HueVariationLeave("Hue Variation Leave", Color) = (1.0,0.5,0.0,0.0)
		[PerRendererData]_HueVariationBark("Hue Variation Bark", Color) = (1.0,0.5,0.0,0.0)
        [PerRendererData]_Alpha ("Alpha", Range(0,1)) = 1
        [PerRendererData]_Ind ("Ind", Range(0,1)) = 0
        [PerRendererData]_smoothValue ("SmoothValue", Range(0,1)) = 0
	}

	// targeting SM3.0+
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
			#pragma surface surf Lambert vertex:SpeedTreeVert nolightmap
			#pragma target 3.0
			#pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
			#pragma shader_feature EFFECT_BUMP
			#pragma shader_feature EFFECT_HUE_VARIATION
			#pragma multi_compile __ CROSSFADE
			#pragma multi_compile __ ENABLE_ALTWIND
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/SpeedTreeCommonAltTree.cginc"
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

            float _Alpha;
            float _Ind;

			void surf(Input IN, inout SurfaceOutput OUT)
			{
                #ifdef CROSSFADE
					CrossFadeUV(IN.screenPos, _Alpha, _Ind);
				#endif
                
				SpeedTreeFragOut o;
				SpeedTreeFrag(IN, o);
				SPEEDTREE_COPY_FRAG(OUT, o)
			}
		ENDCG

		Pass
		{
			Tags { "LightMode" = "ShadowCaster" }

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
				#pragma multi_compile __ CROSSFADE
			#pragma multi_compile __ ENABLE_ALTWIND
				#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/SpeedTreeCommonAltTree.cginc"
			    #include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"
                
                float _Alpha;
                float _Ind;

				struct v2f 
				{
					V2F_SHADOW_CASTER;
					#ifdef SPEEDTREE_ALPHATEST
						half2 uv : TEXCOORD1;
					#endif
					float4 screenPos : TEXCOORD3;
				};

				v2f vert(SpeedTreeVB v)
				{
					v2f o;
					o.screenPos = ComputeScreenPos(mul(UNITY_MATRIX_MVP, v.vertex));

					#ifdef SPEEDTREE_ALPHATEST
						o.uv = v.texcoord.xy;
					#endif
					OffsetSpeedTreeVertex(v, _smoothValue);
					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)

					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					#ifdef CROSSFADE
						CrossFadeUV(i.screenPos, _Alpha, _Ind);
					#endif
					#ifdef SPEEDTREE_ALPHATEST
						clip(tex2D(_MainTex, i.uv).a - _Cutoff);
					#endif
					SHADOW_CASTER_FRAGMENT(i)
				}
			ENDCG
		}
	}

	//  SM 2.0
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
			#pragma surface surf Lambert vertex:SpeedTreeVert nolightmap
			#pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
			#pragma shader_feature EFFECT_HUE_VARIATION
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/SpeedTreeCommonAltTree.cginc"
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

            float _Alpha;
            float _Ind;

			void surf(Input IN, inout SurfaceOutput OUT)
			{
				SpeedTreeFragOut o;
				SpeedTreeFrag(IN, o);
				SPEEDTREE_COPY_FRAG(OUT, o)
			}
		ENDCG

		Pass
		{
			Tags { "LightMode" = "ShadowCaster" }

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
				#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/SpeedTreeCommonAltTree.cginc"
			    #include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"
                
                float _Alpha;
                float _Ind;

				struct v2f 
				{
					V2F_SHADOW_CASTER;
					#ifdef SPEEDTREE_ALPHATEST
						half2 uv : TEXCOORD1;
					#endif
					float4 screenPos : TEXCOORD3;
				};

				v2f vert(SpeedTreeVB v)
				{
					v2f o;
					o.screenPos = ComputeScreenPos(mul(UNITY_MATRIX_MVP, v.vertex));

					#ifdef SPEEDTREE_ALPHATEST
						o.uv = v.texcoord.xy;
					#endif
					OffsetSpeedTreeVertex(v, _smoothValue);
					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)

					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					#ifdef SPEEDTREE_ALPHATEST
						clip(tex2D(_MainTex, i.uv).a - _Cutoff);
					#endif
					SHADOW_CASTER_FRAGMENT(i)
				}
			ENDCG
		}
	}

	FallBack "Transparent/Cutout/VertexLit"
	//CustomEditor "SpeedTreeMaterialInspectorAltTree"
}
