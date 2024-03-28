Shader "EasyGames/VertexColorToonShader" {
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
	//CustomEditor "ToonShaderGUI"
}