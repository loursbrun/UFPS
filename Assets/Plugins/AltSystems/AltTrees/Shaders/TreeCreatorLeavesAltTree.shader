Shader "AltTrees/TreeCreatorLeaves"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Range(0.01, 1)) = 0.078125
		_MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_GlossMap("Gloss (A)", 2D) = "black" {}
		_TranslucencyMap("Translucency (A)", 2D) = "white" {}
		_ShadowOffset("Shadow Offset (A)", 2D) = "black" {}


		[PerRendererData]_HueVariationLeave("Hue Variation Leave", Color) = (1.0,0.5,0.0,0.0)
		[PerRendererData]_Alpha("Alpha", Range(0,1)) = 1
		[PerRendererData]_Ind("Ind", Range(0,1)) = 0



		_Cutoff("Alpha cutoff", Range(0,1)) = 0.3
		[HideInInspector] _TreeInstanceColor("TreeInstanceColor", Vector) = (1,1,1,1)
		[HideInInspector] _TreeInstanceScale("TreeInstanceScale", Vector) = (1,1,1,1)
		[HideInInspector] _SquashAmount("Squash", Float) = 1
	}

	//  SM 3.0+
	SubShader
	{
		Tags
		{
			"IgnoreProjector" = "True" "RenderType" = "Opaque"
		}
		LOD 200

		CGPROGRAM
			#pragma surface surf Lambert vertex:TreeVertLeaf nolightmap noforwardadd
			#pragma target 3.0
			#pragma multi_compile __ CROSSFADE
			#pragma multi_compile __ ENABLE_ALTWIND
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/UnityBuiltin3xTreeLibraryAltTree.cginc"
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _GlossMap;
			sampler2D _TranslucencyMap;
			half _Shininess;
			half _Cutoff;
			float _Alpha;
			float _Ind;
			half4 _HueVariationLeave;

			struct Input
			{
				float2 uv_MainTex;
				fixed4 color : COLOR;
				float4 screenPos;
			};

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



				o.Albedo = c.rgb/* * IN.color.rgb*/;
				o.Gloss = tex2D(_GlossMap, IN.uv_MainTex).a;
				o.Alpha = c.a;
				o.Specular = _Shininess;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			}
		ENDCG

			
		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }

			CGPROGRAM


				#pragma vertex TreeVertLeaf2
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile __ CROSSFADE
				#pragma multi_compile __ ENABLE_ALTWIND
				#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/UnityBuiltin3xTreeLibraryAltTree.cginc"
				#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

				struct v2f 
				{
					V2F_SHADOW_CASTER;
					float4 screenPos : TEXCOORD3;
					half2 uv : TEXCOORD1;
				};

				sampler2D _MainTex;
				float _Alpha;
				float _Ind;
				float _Cutoff;

				v2f TreeVertLeaf2(appdata_full v)
				{
					v2f o;
					o.screenPos = ComputeScreenPos(mul(UNITY_MATRIX_MVP, v.vertex));

					ExpandBillboard(UNITY_MATRIX_IT_MV, v.vertex, v.normal, v.tangent);
					//v.vertex.xyz *= _TreeInstanceScale.xyz;
					v.vertex = AnimateVertex(v.vertex, v.normal, float4(v.color.xy, v.texcoord1.xy));

					v.vertex = Squash(v.vertex);

					o.uv = v.texcoord.xy;

					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);

					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					#ifdef CROSSFADE
						CrossFadeUV(i.screenPos, _Alpha, _Ind);
					#endif

					clip(tex2D(_MainTex, i.uv).a - _Cutoff);

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
			"IgnoreProjector" = "True" "RenderType" = "Opaque"
		}
		LOD 200

		CGPROGRAM
			#pragma surface surf Lambert vertex:TreeVertLeaf nolightmap noforwardadd
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/UnityBuiltin3xTreeLibraryAltTree.cginc"
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _GlossMap;
			sampler2D _TranslucencyMap;
			half _Shininess;
			half _Cutoff;
			float _Alpha;
			float _Ind;
			half4 _HueVariationLeave;

			struct Input
			{
				float2 uv_MainTex;
				fixed4 color : COLOR;
				float4 screenPos;
			};

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
				// preserve vibrance
				shiftedColor.rgb *= maxBase;
				c.rgb = saturate(shiftedColor);



				o.Albedo = c.rgb/* * IN.color.rgb*/;
				o.Gloss = tex2D(_GlossMap, IN.uv_MainTex).a;
				o.Alpha = c.a;
				o.Specular = _Shininess;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			}
		ENDCG

			
		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }

			CGPROGRAM


				#pragma vertex TreeVertLeaf2
				#pragma fragment frag
				#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/UnityBuiltin3xTreeLibraryAltTree.cginc"
				#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

				struct v2f 
				{
					V2F_SHADOW_CASTER;
					float4 screenPos : TEXCOORD3;
					half2 uv : TEXCOORD1;
				};

				sampler2D _MainTex;
				float _Alpha;
				float _Ind;
				float _Cutoff;

				v2f TreeVertLeaf2(appdata_full v)
				{
					v2f o;
					o.screenPos = ComputeScreenPos(mul(UNITY_MATRIX_MVP, v.vertex));

					ExpandBillboard(UNITY_MATRIX_IT_MV, v.vertex, v.normal, v.tangent);
					//v.vertex.xyz *= _TreeInstanceScale.xyz;
					v.vertex = AnimateVertex(v.vertex, v.normal, float4(v.color.xy, v.texcoord1.xy));

					v.vertex = Squash(v.vertex);

					o.uv = v.texcoord.xy;

					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);

					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					clip(tex2D(_MainTex, i.uv).a - _Cutoff);

					SHADOW_CASTER_FRAGMENT(i)
				}
			ENDCG
		}
	}
	FallBack "Transparent/Cutout/VertexLit"
}
