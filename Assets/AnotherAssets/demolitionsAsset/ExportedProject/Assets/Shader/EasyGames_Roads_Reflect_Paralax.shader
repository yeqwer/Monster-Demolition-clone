Shader "EasyGames/Roads_Reflect_Paralax" {
	Properties {
		_MainTex ("Base", 2D) = "white" {}
		_ShadowColor ("Shadow Color", Vector) = (1,1,1,1)
		[PowerSlider(2.0)] _Emission ("Emission", Range(1, 2)) = 1
		_TintColor ("TintColor", Vector) = (1,1,1,1)
		[PowerSlider(1.0)] _BlendTint ("BlendTint", Range(0, 1)) = 0
		_NormTex ("Normal Map", 2D) = "black" {}
		[PowerSlider(4.0)] _NormalPower ("NormalPower", Range(-2, 2)) = 0
		_RimPower ("Rim Power", Float) = 10
		_RimColor ("Rim Color", Vector) = (1,1,1,1)
		_Cube ("Reflection Map", Cube) = "" {}
		[PowerSlider(3.0)] _Rshift ("Reflect Shift", Range(-1, 2)) = 0.2
		[PowerSlider(1.0)] _ReflectPower ("Reflect Power", Range(0, 1)) = 0.2
		_TintHeight ("TintHeight", Vector) = (1,1,1,1)
		_Height ("Height", 2D) = "white" {}
		[PowerSlider(4.0)] _Parallax ("Parallax", Range(-2, 2)) = 0
		[KeywordEnum(Off, Front, Back)] _Cull ("Cull", Float) = 2
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Mobile/VertexLit"
	//CustomEditor "RoadsShaderReflectParallaxGUI"
}