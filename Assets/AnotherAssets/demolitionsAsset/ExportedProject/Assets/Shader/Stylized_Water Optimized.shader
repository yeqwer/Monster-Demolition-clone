Shader "Stylized/Water Optimized" {
	Properties {
		_DistanceDensity ("Distance", Range(0, 0.01)) = 0.1
		_MinAlpha ("Min Alpha", Range(0, 1)) = 0.5
		[Header(Color)] _BaseColor ("Base", Vector) = (1,1,1,1)
		_FarColor ("Far", Vector) = (1,1,1,1)
		[Header(Foam)] _FoamContribution ("Contribution", Range(0, 1)) = 1
		[NoScaleOffset] _FoamTexture ("Texture", 2D) = "black" {}
		_FoamScale ("Scale", Float) = 1
		_FoamSpeed ("Speed", Float) = 1
		_FoamNoiseScale ("Noise Contribution", Range(0, 1)) = 0.5
		[Header(Sparkle)] [NoScaleOffset] _SparklesNormalMap ("Normal Map", 2D) = "bump" {}
		_SparkleScale ("Scale", Float) = 10
		_SparkleSpeed ("Speed", Float) = 0.75
		_SparkleColor ("Color", Vector) = (1,1,1,1)
		_SparkleExponent ("Exponent", Float) = 10000
		[Header(Vertex Waves #1)] _Wave1Direction ("Direction", Range(0, 1)) = 0
		_Wave1Amplitude ("Amplitude", Float) = 1
		_Wave1Wavelength ("Wavelength", Float) = 1
		_Wave1Speed ("Speed", Float) = 1
		[Header(Vertex Waves #2)] _Wave2Direction ("Direction", Range(0, 1)) = 0
		_Wave2Amplitude ("Amplitude", Float) = 1
		_Wave2Wavelength ("Wavelength", Float) = 1
		_Wave2Speed ("Speed", Float) = 1
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
}