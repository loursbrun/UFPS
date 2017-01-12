//#include "TerrainEngine.cginc"
#include "Lighting.cginc"

/*fixed4 _Color;
fixed3 _TranslucencyColor;
fixed _TranslucencyViewDependency;
half _ShadowStrength;
fixed4 _TreeInstanceColor;
float4 _TreeInstanceScale;*/

#ifdef ENABLE_ALTWIND
	CBUFFER_START(TreeCreatorAltWind)
		float4 _WindAltTree;
	CBUFFER_END
#endif

uniform float4 _SquashPlaneNormal;
uniform float _SquashAmount;

struct LeafSurfaceOutput {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	fixed Translucency;
	half Specular;
	fixed Gloss;
	fixed Alpha;
};

/*inline half4 LightingTreeLeaf(LeafSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
{
	half3 h = normalize(lightDir + viewDir);

	half nl = dot(s.Normal, lightDir);

	half nh = max(0, dot(s.Normal, h));
	half spec = pow(nh, s.Specular * 128.0) * s.Gloss;

	// view dependent back contribution for translucency
	fixed backContrib = saturate(dot(viewDir, -lightDir));

	// normally translucency is more like -nl, but looks better when it's view dependent
	backContrib = lerp(saturate(-nl), backContrib, _TranslucencyViewDependency);

	fixed3 translucencyColor = backContrib * s.Translucency * _TranslucencyColor;

	// wrap-around diffuse
	nl = max(0, nl * 0.6 + 0.4);

	fixed4 c;
	/////@TODO: what is is this multiply 2x here???
	c.rgb = s.Albedo * (translucencyColor * 2 + nl);
	c.rgb = c.rgb * _LightColor0.rgb + spec;

	// For directional lights, apply less shadow attenuation
	// based on shadow strength parameter.
#if defined(DIRECTIONAL) || defined(DIRECTIONAL_COOKIE)
	c.rgb *= lerp(1, atten, _ShadowStrength);
#else
	c.rgb *= atten;
#endif

	c.a = s.Alpha;

	return c;
}*/

// -------- Per-vertex lighting functions for "Tree Creator Leaves Fast" shaders
/*
fixed3 ShadeTranslucentMainLight(float4 vertex, float3 normal)
{
	float3 viewDir = normalize(WorldSpaceViewDir(vertex));
	float3 lightDir = normalize(WorldSpaceLightDir(vertex));
	fixed3 lightColor = _LightColor0.rgb;

	float nl = dot(normal, lightDir);

	// view dependent back contribution for translucency
	fixed backContrib = saturate(dot(viewDir, -lightDir));

	// normally translucency is more like -nl, but looks better when it's view dependent
	backContrib = lerp(saturate(-nl), backContrib, _TranslucencyViewDependency);

	// wrap-around diffuse
	fixed diffuse = max(0, nl * 0.6 + 0.4);

	return lightColor.rgb * (diffuse + backContrib * _TranslucencyColor);
}

fixed3 ShadeTranslucentLights(float4 vertex, float3 normal)
{
	float3 viewDir = normalize(WorldSpaceViewDir(vertex));
	float3 mainLightDir = normalize(WorldSpaceLightDir(vertex));
	float3 frontlight = ShadeSH9(float4(normal, 1.0));
	float3 backlight = ShadeSH9(float4(-normal, 1.0));
#ifdef VERTEXLIGHT_ON
	float3 worldPos = mul(unity_ObjectToWorld, vertex).xyz;
	frontlight += Shade4PointLights(
		unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
		unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
		unity_4LightAtten0, worldPos, normal);
	backlight += Shade4PointLights(
		unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
		unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
		unity_4LightAtten0, worldPos, -normal);
#endif

	// view dependent back contribution for translucency using main light as a cue
	fixed backContrib = saturate(dot(viewDir, -mainLightDir));
	backlight = lerp(backlight, backlight * backContrib, _TranslucencyViewDependency);

	// as we integrate over whole sphere instead of normal hemi-sphere
	// lighting gets too washed out, so let's half it down
	return 0.5 * (frontlight + backlight * _TranslucencyColor);
}*/


inline float4 Squash(in float4 pos)
{
	// To squash the tree the vertex needs to be moved in the direction
	// of the squash plane. The plane is defined by the the:
	// plane point - point lying on the plane, defined in model space
	// plane normal - _SquashPlaneNormal.xyz

	// we're pushing squashed tree plane in direction of planeNormal by amount of _SquashPlaneNormal.w
	// this squashing has to match logic of tree billboards

	float3 planeNormal = _SquashPlaneNormal.xyz;

	// unoptimized version:
	//float3 planePoint = -planeNormal * _SquashPlaneNormal.w;
	//float3 projectedVertex = pos.xyz + dot(planeNormal, (planePoint - pos)) * planeNormal;

	// optimized version:	
	float3 projectedVertex = pos.xyz - (dot(planeNormal.xyz, pos.xyz) + _SquashPlaneNormal.w) * planeNormal;

	pos = float4(lerp(projectedVertex, pos.xyz, _SquashAmount), 1);

	return pos;
}

// Expand billboard and modify normal + tangent to fit
inline void ExpandBillboard(in float4x4 mat, inout float4 pos, inout float3 normal, inout float4 tangent)
{
	// tangent.w = 0 if this is a billboard
	float isBillboard = 1.0f - abs(tangent.w);

	// billboard normal
	float3 norb = normalize(mul(float4(normal, 0), mat)).xyz;

	// billboard tangent
	float3 tanb = normalize(mul(float4(tangent.xyz, 0.0f), mat)).xyz;

	pos += mul(float4(normal.xy, 0, 0), mat) * isBillboard;
	normal = lerp(normal, norb, isBillboard);
	tangent = lerp(tangent, float4(tanb, -1.0f), isBillboard);
}

float4 SmoothCurve(float4 x) {
	return x * x *(3.0 - 2.0 * x);
}
float4 TriangleWave(float4 x) {
	return abs(frac(x + 0.5) * 2.0 - 1.0);
}
float4 SmoothTriangleWave(float4 x) {
	return SmoothCurve(TriangleWave(x));
}

// Detail bending
float4 AnimateVertex(float4 pos, float3 normal, float4 animParams)
{
	// animParams stored in color
	// animParams.x = branch phase
	// animParams.y = edge flutter factor
	// animParams.z = primary factor
	// animParams.w = secondary factor

	float fDetailAmp = 0.1f;
	float fBranchAmp = 0.3f;

	// Phases (object, vertex, branch)
	float fObjPhase = dot(unity_ObjectToWorld[3].xyz, 1);
	float fBranchPhase = fObjPhase + animParams.x;

	float fVtxPhase = dot(pos.xyz, animParams.y + fBranchPhase);

	// x is used for edges; y is used for branches
	float2 vWavesIn = _Time.yy + float2(fVtxPhase, fBranchPhase);

	// 1.975, 0.793, 0.375, 0.193 are good frequencies
	float4 vWaves = (frac(vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193)) * 2.0 - 1.0);

	vWaves = SmoothTriangleWave(vWaves);
	float2 vWavesSum = vWaves.xz + vWaves.yw;

	// Edge (xz) and branch bending (y)
	float3 bend = animParams.y * fDetailAmp * normal.xyz;
	bend.y = animParams.w * fBranchAmp;

	#ifdef ENABLE_ALTWIND
		float4 _WindAltTree2 = _WindAltTree * length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x));
		_WindAltTree2.w = 0;
		_WindAltTree.xyz = (mul (unity_WorldToObject, mul(unity_ObjectToWorld, pos) + _WindAltTree2) - pos).xyz;

		pos.xyz += ((vWavesSum.xyx * bend) + (_WindAltTree.xyz * vWavesSum.y * animParams.w)) * _WindAltTree.w;
		// Primary bending
		// Displace position
		pos.xyz += animParams.z * _WindAltTree.xyz;
	#endif

	return pos;
}

void TreeVertBark(inout appdata_full v)
{
	//v.vertex.xyz *= _TreeInstanceScale.xyz;
	v.vertex = AnimateVertex(v.vertex, v.normal, float4(v.color.xy, v.texcoord1.xy));

	v.vertex = Squash(v.vertex);

	//v.color.rgb = _TreeInstanceColor.rgb * _Color.rgb;
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);
	
	
	#ifdef NORMALMAP_MODE
		float3 dirr = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
		half checkDirr = dot(mul(unity_ObjectToWorld, v.normal).xyz, dirr);
		
		v.normal = (checkDirr < 0) ? v.normal * -1 : v.normal;
	#endif
}

void TreeVertLeaf(inout appdata_full v)
{
	ExpandBillboard(UNITY_MATRIX_IT_MV, v.vertex, v.normal, v.tangent);
	//v.vertex.xyz *= _TreeInstanceScale.xyz;
	v.vertex = AnimateVertex(v.vertex, v.normal, float4(v.color.xy, v.texcoord1.xy));

	v.vertex = Squash(v.vertex);

	//v.color.rgb = _TreeInstanceColor.rgb * _Color.rgb;
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);
	
	#ifdef NORMALMAP_MODE
		float3 dirr = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
		half checkDirr = dot(mul(unity_ObjectToWorld, v.normal).xyz, dirr);
		
		v.normal = (checkDirr < 0) ? v.normal * -1 : v.normal;
	#endif
}