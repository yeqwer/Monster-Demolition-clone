Shader "EasyGames/DissolveVertexColorToonShader" {
	Properties {
		_ShadowColor ("Shadow Color", Vector) = (1,1,1,1)
		[PowerSlider(2.0)] _Emission ("Emission", Range(0, 2)) = 0
		_Ramp ("Ramp", 2D) = "white" {}
		[PowerSlider(1.0)] _RampIntensity ("RampIntensity", Range(0, 1)) = 1
		_TintColor ("TintColor", Vector) = (1,1,1,1)
		[PowerSlider(1.0)] _BlendTint ("BlendTint", Range(0, 1)) = 0
		[PowerSlider(2.0)] _TintPower ("TintPower", Range(0, 2)) = 0
		_RimPower ("Rim Power", Float) = 10
		_RimColor ("Rim Color", Vector) = (1,1,1,1)
		[PowerSlider(2.0)] _ReflectionPower ("Reflection Power", Range(0, 10)) = 0
		[PowerSlider(1.0)] _ReflectionSmoothness ("Reflection Smoothness", Range(0, 1)) = 1
		[PowerSlider(1.0)] _FresnelPower ("Fresnel Power", Range(0, 10)) = 1
		[PowerSlider(1.0)] _Rotation ("Fresnel Power", Range(0, 360)) = 0
		[HideInInspector] _YMin ("Y Min", Float) = 0
		[HideInInspector] _YMax ("Y Max", Float) = 0
		[HideInInspector] _XMin ("X Min", Float) = 0
		[HideInInspector] _XMax ("X Max", Float) = 0
		[HideInInspector] _ZMin ("Z Min", Float) = 0
		[HideInInspector] _ZMax ("Z Max", Float) = 0
		_DissolveMap ("Dissolve Map", 2D) = "white" {}
		_TilingX ("X", Float) = 1
		_TilingY ("Y", Float) = 1
		_DirectionMap ("Direction Map", 2D) = "white" {}
		_DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0.5
		_DissolveRampUp ("Dissolve Ramp Up", Range(1, 5)) = 5
		_OuterEdgeColor ("Outer Edge Color", Vector) = (1,0,0,1)
		_InnerEdgeColor ("Inner Edge Color", Vector) = (1,1,0,1)
		_EdgeThickness ("Edge Thickness", Range(0, 1)) = 0.1
		[KeywordEnum(Off, Front, Back)] _Cull ("Cull", Float) = 2
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	Fallback "Mobile/VertexLit"
	//CustomEditor "DissolveToonShaderGUI"
}