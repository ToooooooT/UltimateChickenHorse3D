Shader "Zololgo/Standard Displacement (Specular setup)" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_SpecGloss ("Specular (RGB) smoothness (A)", 2D) = "white" {}
		_Specular ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Glossiness ("Smoothness", Range (0.0, 1.0)) = 0.5
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_DisplacementMap ("Displacement map (A)", 2D) = "black" {}
		_DisplacementPower ("Displacement power", Range (0.0, 1.0)) = 0.5
		_EdgeLength ("Edge length", Range(3,50)) = 10
	}

	SubShader 
	{ 
		Tags { "RenderType"="Opaque" }
		LOD 800
		CGPROGRAM
		#pragma surface surf StandardSpecular addshadow vertex:disp tessellate:tessEdge
		#include "Tessellation.cginc"

		struct appdata 
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
			float2 texcoord2 : TEXCOORD2;
		};
		fixed4 _Color, _Specular, _DisplacementMap_ST;
		float _EdgeLength, _DisplacementPower, _Glossiness;
		sampler2D _MainTex, _SpecGloss, _BumpMap, _DisplacementMap;

		float4 tessEdge (appdata v0, appdata v1, appdata v2)
		{
			return UnityEdgeLengthBasedTessCull (v0.vertex, v1.vertex, v2.vertex, _EdgeLength, _DisplacementPower * 1.5f);
		}
		void disp (inout appdata v)
		{
			float4 dispUV = float4(v.texcoord.xy,0,0)*float4(_DisplacementMap_ST.xy,0,0)+float4(_DisplacementMap_ST.zw,0,0);
			float d = tex2Dlod(_DisplacementMap, dispUV).a * _DisplacementPower;
			v.vertex.xyz += v.normal * d;
		}
		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_SpecGloss;			
		};
		void surf (Input IN, inout SurfaceOutputStandardSpecular o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 specGloss = tex2D(_SpecGloss, IN.uv_SpecGloss);
			o.Albedo = tex.rgb * _Color.rgb;
			o.Smoothness = _Glossiness * specGloss.a;
			o.Alpha = tex.a * _Color.a;
			o.Specular = specGloss.rgb * _Specular.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	}
	FallBack "Bumped Specular"
}
