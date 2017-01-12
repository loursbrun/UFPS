Shader "Hidden/AltTrees/AltProjector"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_Tex ("Cookie", 2D) = "" {}
	}
	
	Subshader
	{
		Tags
		{
			"Queue"="Transparent"
		}
		
		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, vertex);
				o.uv = mul (unity_Projector, vertex);
				return o;
			}
			
			fixed4 _Color;
			sampler2D _Tex;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 tex = tex2Dproj (_Tex, UNITY_PROJ_COORD(i.uv));
				tex.rgba *= _Color.rgba;
				
				return tex;
			}
			ENDCG
		}
	}
}
