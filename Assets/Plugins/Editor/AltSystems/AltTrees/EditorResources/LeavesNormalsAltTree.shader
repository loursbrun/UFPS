Shader "Hidden/AltTrees/LeavesNormals"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpScale("Scale", Float) = 1.0
		
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.3
		[MaterialEnum(Off,0,Front,1,Back,2)] _Cull ("Cull", Int) = 2

		[PerRendererData]_HueVariationLeave("Hue Variation Leave", Color) = (1.0,0.5,0.0,0.0)
		[PerRendererData]_Alpha("Alpha", Range(0,1)) = 1
		[PerRendererData]_Ind("Ind", Range(0,1)) = 0
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector" = "True" }
		LOD 400
		Cull [_Cull]
		
		CGPROGRAM
			#pragma surface surf NoLighting vertex:vert nolightmap noforwardadd
			#pragma target 3.0
			#define NORMALMAP_MODE

			sampler2D _MainTex;
			half _Cutoff;
			float _Alpha;
			float _Ind;
			half4 _HueVariationLeave;
			float _BumpScale;

			struct Input {
				float2 uv_MainTex;
				float4 screenPos;
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

			void vert(inout appdata_full v)
			{
				#ifdef NORMALMAP_MODE
					float3 dirr = ObjSpaceViewDir(v.vertex);
					half checkDirr = dot(mul(unity_ObjectToWorld, v.normal).xyz, dirr);
		
					v.normal = (checkDirr < 0) ? v.normal * -1 : v.normal;
				#endif
			}

			fixed4 _Color;

			void surf (Input IN, inout SurfaceOutput o)
			{
				o.Normal = float3(0, 0, 1);

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
	
	FallBack "Transparent/Cutout/VertexLit"
}
