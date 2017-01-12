Shader "AltTrees/Billboard"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		[NoScaleOffset]_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset]_BumpMap ("Normal Map", 2D) = "bump" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.0
		_Metallic ("Metallic", Range(0,1)) = 0.4

		[PerRendererData]_Alpha ("Alpha", Range(0,1)) = 1
		[PerRendererData]_HueVariation ("Hue Variation", Color) = (1.0,0.5,0.0,0.0)
		[PerRendererData]_Width("Width", Range(0,1)) = 1
		[PerRendererData]_Height("Height", Range(0,1)) = 1
		[PerRendererData]_Rotation("Rotation", Range(0,1)) = 1
	}
	/*SubShader
    {
		Tags {"Queue"="AlphaTest" "RenderType"="TransparentCutout" "DisableBatching"="True" "IgnoreProjector"="True"}
		LOD 1001
		
		CGPROGRAM
		
			#pragma surface surf Standard vertex:vert nolightmap nodynlightmap fullforwardshadows  alphatest:_Cutoff
			#pragma target 3.0
			#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

			sampler2D _MainTex;
			sampler2D _BumpMap;
			//float _BumpScale;

			struct Input
			{
				float4 screenPos;
				float2 uv_MainTex;
				float2 uv_BumpMap;
				float4 color;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			fixed4 _HueVariation;
			float _Alpha;
			float _Ind;

			void vert (inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input,o);
				float3 eyeVector = ObjSpaceViewDir( v.vertex );

				float3 upVector = float3(0,1,0);
				float3 sideVector = normalize(cross(eyeVector,upVector));
			
				float3 finalposition = v.vertex.xyz;
				finalposition += (v.texcoord1.x) * sideVector;
				finalposition += (v.texcoord1.y) * upVector;
				    
				float4 pos = float4(finalposition, 1);
		    
				o.color = _HueVariation;
            
            
				float3 dir = normalize(cross(sideVector, upVector) * -1);
				//float3 dir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
				//if(dir.z==0)
				//	dir.z=0.01;

				float _float = (atan2(dir.x,dir.z)*180.0)/3.1415+180.0;
					
				_float -= v.texcoord2.x;
					
				if(_float<0)
  					_float = 360.0 + _float;

				float _intY = 3.0-floor(_float/120.0);
  				float _intX = floor((_float - 120.0*_intY)/40.0);
					
				v.texcoord.x += _intX * 1.0/3.0;
				v.texcoord.y += _intY * 1.0/3.0;

				v.normal = dir;
				v.vertex = pos;
			}

			void surf (Input IN, inout SurfaceOutputStandard o)
			{
				CrossFadeUV(IN.screenPos, _Alpha, _Ind);
            
				fixed4 c = tex2D (_MainTex, IN.uv_MainTex.xy) * _Color;
				if(c.a <= 1.0 - _Alpha)
					discard;


				//_Color *= _Color.r;

				half3 shiftedColor = lerp(c.rgb, IN.color.rgb, IN.color.a);
				//half3 shiftedColor = lerp(c.rgb, _HueVariation.rgb, 1);
				half maxBase = max(c.r, max(c.g, c.b));
				half newMaxBase = max(shiftedColor.r, max(shiftedColor.g, shiftedColor.b));
				maxBase /= newMaxBase;
				maxBase = maxBase * 0.5f + 0.5f;
				// preserve vibrance
				shiftedColor.rgb *= maxBase;
				c.rgb = saturate(shiftedColor);


				o.Albedo = c.rgb * _Color.rgb;
				//o.Normal = UnpackScaleNormal(tex2D (_BumpMap, IN.uv_BumpMap), _BumpScale);
				o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap.xy));
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;

			}
		ENDCG

		Pass
		{
			Tags { "LightMode" = "ShadowCaster" }

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag alphatest:_Cutoff
				#pragma target 3.0
				#pragma multi_compile_shadowcaster
				#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"



                
                float _Alpha;
                float _Ind;
				sampler2D _MainTex;
				float _Cutoff;

				struct Input
				{
					V2F_SHADOW_CASTER;
					float2 uv : TEXCOORD1;
					float4 screenPos : TEXCOORD3;
					//float2 uv_BumpMap : TEXCOORD2;
				};

				Input vert(appdata_full v)
				{
					Input o;
					UNITY_INITIALIZE_OUTPUT(Input,o);

					//float3 objSpaceLightPos = mul(unity_WorldToObject, _WorldSpaceLightPos0).xyz;

					//float3 eyeVector = objSpaceLightPos.xyz * -1;
					float3 eyeVector;

					if (unity_LightShadowBias.z != 0.0)
					{
						eyeVector  = ObjSpaceLightDir(v.vertex) * -1;
					}
					else
					{
						eyeVector = ObjSpaceViewDir(v.vertex);
					}

					float3 upVector = float3(0,1,0);
					float3 sideVector = normalize(cross(eyeVector,upVector));
			
					float3 finalposition = v.vertex.xyz;
					finalposition += (v.texcoord1.x) * sideVector;
					finalposition += (v.texcoord1.y) * upVector;
				    
					float4 pos = float4(finalposition, 1);
		    
					float3 dir = normalize(cross(sideVector, upVector) * -1);
					//float3 dir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
					//if(dir.z==0)
					//	dir.z=0.01;

					float _float = (atan2(dir.x,dir.z)*180.0)/3.1415+180.0;
					
					_float -= v.texcoord2.x;
					
					if(_float<0)
  						_float = 360.0 + _float;

					float _intY = 3.0-floor(_float/120.0);
  					float _intX = floor((_float - 120.0*_intY)/40.0);
					
					v.texcoord.x += _intX * 1.0/3.0;
					v.texcoord.y += _intY * 1.0/3.0;

					v.normal = dir;
					v.vertex = pos;

					o.screenPos = ComputeScreenPos(mul(UNITY_MATRIX_MVP, v.vertex));
					o.uv = v.texcoord.xy;

					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)

					return o;
				}

				float4 frag(Input i) : SV_Target
				{
					#ifdef CROSSFADE
						CrossFadeUV(i.screenPos, _Alpha, _Ind);
					#endif
					clip(tex2D(_MainTex, i.uv).a - _Cutoff);
					SHADOW_CASTER_FRAGMENT(i)
				}
			ENDCG
		}

	}*/

	//  SM 3.0+
	SubShader
    {
		Tags {"Queue"="AlphaTest" "RenderType"="TransparentCutout" "DisableBatching"="True" "IgnoreProjector"="True"}
		LOD 1000
		
		CGPROGRAM
		
		#pragma surface surf Lambert vertex:vert nolightmap nodynlightmap fullforwardshadows
		#pragma shader_feature DEBUG_ON
		#pragma multi_compile __ CROSSFADE
		#pragma target 3.0
	    #include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

		sampler2D _MainTex;
        sampler2D _BumpMap;

		struct Input {
            float4 screenPos;
			float2 uv_MainTex;
            float2 uv_BumpMap;
            float4 color;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        fixed4 _HueVariation;
        float _Alpha;
		float _Width;
		float _Height;
		float _Rotation;

		void vert (inout appdata_full v, out Input o)
		{
            UNITY_INITIALIZE_OUTPUT(Input,o);
		    
			o.color = _HueVariation;

			float3 upVector = float3(0, 1, 0);
			float3 dir = ObjSpaceViewDir(v.vertex);
			if(dir.z==0)
				dir.z=0.01;

			float _float = (atan2(dir.x,dir.z)*180.0)/3.1415+180.0;
					
			_float -= _Rotation;
					
			if(_float<0)
  				_float = 360.0 + _float;

			float _intY = 3.0-floor(_float/120.0);
  			float _intX = floor((_float - 120.0*_intY)/40.0);
					
			v.texcoord.x += _intX * 0.3333333;
			v.texcoord.y += _intY * 0.3333333;
			float3 sideVector = normalize(cross(dir, upVector));

			float3 finalposition = v.vertex.xyz;
			finalposition += (v.texcoord1.x) * _Width * sideVector;
			finalposition += (v.texcoord1.y) * _Height * upVector;

			float4 pos = float4(finalposition, 1);


			float3 billboardTangent = normalize(float3(-dir.z, 0, dir.x));
			v.tangent = float4(billboardTangent.xyz, -1);
			v.normal = float3(billboardTangent.z, 0, -billboardTangent.x);
			v.vertex = pos;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
            
            #ifdef CROSSFADE
				CrossFadeUV(IN.screenPos, _Alpha, 1.0);
			#endif
            
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex.xy);

			clip(c.a - 0.2);

            half3 shiftedColor = lerp(c.rgb, IN.color.rgb, IN.color.a);
		    half maxBase = max(c.r, max(c.g, c.b));
		    half newMaxBase = max(shiftedColor.r, max(shiftedColor.g, shiftedColor.b));
		    maxBase /= newMaxBase;
		    maxBase = maxBase * 0.5f + 0.5f;

		    shiftedColor.rgb *= maxBase;
		    c.rgb = saturate(shiftedColor);


			o.Albedo = c.rgb * _Color.rgb;
			
			#ifdef DEBUG_ON
				o.Albedo = IN.color.rgb;
			#endif

            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap.xy));
			o.Alpha = c.a;

		}
		ENDCG
			

		Pass
		{
			Tags { "LightMode" = "ShadowCaster" }

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile __ CROSSFADE
				#pragma multi_compile_shadowcaster
				#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"



                
                float _Alpha;
				sampler2D _MainTex;
				float _Width;
				float _Height;
				float _Rotation;

				struct Input
				{
					V2F_SHADOW_CASTER;
					float2 uv : TEXCOORD1;
					float4 screenPos : TEXCOORD3;
				};

				Input vert(appdata_full v)
				{
					Input o;
					UNITY_INITIALIZE_OUTPUT(Input,o);

					float3 eyeVector;

					#ifdef SHADOWS_DEPTH // might be directional shadow or screen depth
						if (unity_LightShadowBias.z != 0.0)
						{
							eyeVector  = ObjSpaceLightDir(v.vertex) * -1;
						}
						else
						{
							eyeVector = ObjSpaceViewDir(v.vertex);
						}
					#else
						float3 objSpaceLightPos = mul(unity_WorldToObject, _WorldSpaceLightPos0).xyz;
						eyeVector  = objSpaceLightPos.xyz - v.vertex.xyz;
					#endif

					float3 upVector = float3(0,1,0);
					float3 sideVector = normalize(cross(eyeVector,upVector));
			
					float3 finalposition = v.vertex.xyz;
					finalposition += (v.texcoord1.x) * _Width * sideVector;
					finalposition += (v.texcoord1.y) * _Height * upVector;
				    
					float4 pos = float4(finalposition, 1);
		    
					float3 dir = normalize(cross(sideVector, upVector) * -1);

					float _float = (atan2(dir.x,dir.z)*180.0)/3.1415+180.0;
					
					_float -= _Rotation;
					
					if(_float<0)
  						_float = 360.0 + _float;

					float _intY = 3.0-floor(_float/120.0);
  					float _intX = floor((_float - 120.0*_intY)/40.0);
					
					v.texcoord.x += _intX * 0.3333333;
					v.texcoord.y += _intY * 0.3333333;

					v.normal = dir;
					v.vertex = pos;

					o.screenPos = ComputeScreenPos(mul(UNITY_MATRIX_MVP, v.vertex));
					o.uv = v.texcoord.xy;

					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)

					return o;
				}

				float4 frag(Input i) : SV_Target
				{
					#ifdef CROSSFADE
						CrossFadeUV(i.screenPos, _Alpha, 1.0);
					#endif
					clip(tex2D(_MainTex, i.uv).a - 1.0);
					SHADOW_CASTER_FRAGMENT(i)
				}
			ENDCG
		}
	}

	//  SM 2.0
	SubShader
    {
		Tags {"Queue"="AlphaTest" "RenderType"="TransparentCutout" "DisableBatching"="True" "IgnoreProjector"="True"}
		LOD 1000
		
		CGPROGRAM
		
		#pragma surface surf Lambert vertex:vert nolightmap nodynlightmap fullforwardshadows
		#pragma shader_feature DEBUG_ON
	    #include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"

		sampler2D _MainTex;
        sampler2D _BumpMap;

		struct Input {
            float4 screenPos;
			float2 uv_MainTex;
            float2 uv_BumpMap;
            float4 color;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        fixed4 _HueVariation;
        float _Alpha;
		float _Width;
		float _Height;
		float _Rotation;

		void vert (inout appdata_full v, out Input o)
		{
            UNITY_INITIALIZE_OUTPUT(Input,o);
		    
			o.color = _HueVariation;
            

			float3 upVector = float3(0, 1, 0);
			float3 dir = ObjSpaceViewDir(v.vertex);
			if(dir.z==0)
				dir.z=0.01;

			float _float = (atan2(dir.x,dir.z)*180.0)/3.1415+180.0;
					
			_float -= _Rotation;
					
			if(_float<0)
  				_float = 360.0 + _float;

			float _intY = 3.0-floor(_float/120.0);
  			float _intX = floor((_float - 120.0*_intY)/40.0);
					
			v.texcoord.x += _intX * 0.3333333;
			v.texcoord.y += _intY * 0.3333333;

			float3 sideVector = normalize(cross(dir, upVector));

			float3 finalposition = v.vertex.xyz;
			finalposition += (v.texcoord1.x) * _Width * sideVector;
			finalposition += (v.texcoord1.y) * _Height * upVector;

			float4 pos = float4(finalposition, 1);




			v.normal = dir;
			v.vertex = pos;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex.xy);
			if(c.a <= 1.0 - _Alpha)
				discard;

            half3 shiftedColor = lerp(c.rgb, IN.color.rgb, IN.color.a);
		    half maxBase = max(c.r, max(c.g, c.b));
		    half newMaxBase = max(shiftedColor.r, max(shiftedColor.g, shiftedColor.b));
		    maxBase /= newMaxBase;
		    maxBase = maxBase * 0.5f + 0.5f;

		    shiftedColor.rgb *= maxBase;
		    c.rgb = saturate(shiftedColor);


			o.Albedo = c.rgb * _Color.rgb;
			
			#ifdef DEBUG_ON
				o.Albedo = IN.color.rgb;
			#endif

            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap.xy));
			o.Alpha = c.a;

		}
		ENDCG
			

		Pass
		{
			Tags { "LightMode" = "ShadowCaster" }

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_shadowcaster
				#include "Assets/Plugins/AltSystems/AltTrees/Shaders/Library/AltTrees.cginc"



                
                float _Alpha;
				sampler2D _MainTex;
				float _Width;
				float _Height;
				float _Rotation;

				struct Input
				{
					V2F_SHADOW_CASTER;
					float2 uv : TEXCOORD1;
					float4 screenPos : TEXCOORD3;
				};

				Input vert(appdata_full v)
				{
					Input o;
					UNITY_INITIALIZE_OUTPUT(Input,o);

					float3 eyeVector;

					#ifdef SHADOWS_DEPTH // might be directional shadow or screen depth
						if (unity_LightShadowBias.z != 0.0)
						{
							eyeVector  = ObjSpaceLightDir(v.vertex) * -1;
						}
						else
						{
							eyeVector = ObjSpaceViewDir(v.vertex);
						}
					#else
						float3 objSpaceLightPos = mul(unity_WorldToObject, _WorldSpaceLightPos0).xyz;
						eyeVector  = objSpaceLightPos.xyz - v.vertex.xyz;
					#endif

					float3 upVector = float3(0,1,0);
					float3 sideVector = normalize(cross(eyeVector,upVector));
			
					float3 finalposition = v.vertex.xyz;
					finalposition += (v.texcoord1.x) * _Width * sideVector;
					finalposition += (v.texcoord1.y) * _Height * upVector;
				    
					float4 pos = float4(finalposition, 1);
		    
					float3 dir = normalize(cross(sideVector, upVector) * -1);

					float _float = (atan2(dir.x,dir.z)*180.0)/3.1415+180.0;
					
					_float -= _Rotation;
					
					if(_float<0)
  						_float = 360.0 + _float;

					float _intY = 3.0-floor(_float/120.0);
  					float _intX = floor((_float - 120.0*_intY)/40.0);
					
					v.texcoord.x += _intX * 0.3333333;
					v.texcoord.y += _intY * 0.3333333;

					v.normal = dir;
					v.vertex = pos;

					o.screenPos = ComputeScreenPos(mul(UNITY_MATRIX_MVP, v.vertex));
					o.uv = v.texcoord.xy;

					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)

					return o;
				}

				float4 frag(Input i) : SV_Target
				{
					clip(tex2D(_MainTex, i.uv).a - 1.0);
					SHADOW_CASTER_FRAGMENT(i)
				}
			ENDCG
		}
	}
	FallBack "Transparent/Cutout/Diffuse"
}
