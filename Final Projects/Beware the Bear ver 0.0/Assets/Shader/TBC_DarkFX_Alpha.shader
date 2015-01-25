Shader "TBC/Toon_DarkFX_Alpha" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _ShadowTex ("DarkFX Base (RGB)", 2D) = "black" {}
        _DarkFXIntensity ("DarkFX Intensity", Range(0,1)) = 0.5
        _SpecularColor ("Specular Color (RGB) / Intensity (A)", 2D) = "white" {}
        _GlossPower ("GlossPower", Range(0.01, 1)) = 1
        _Glossiness ("Glossiness (A)", 2D) = "white" {}
        
        _SpecularIntensity ("Specular Intensity", float) = 1
        _Bump ("Bump", 2D) = "bump" {}
        _Emissive ("Emissive (RGB)", 2D) = "black" {}
        _EmissionPower ("Emissive Power", float) = 1

        _Cutoff ("Cutoff", float) = 1
	}
	SubShader 
    {
        CGPROGRAM
		#pragma surface surf TBCRamp alphatest:_Cutoff
        #pragma target 5.0
        #pragma debug
        #include "UnityCG.cginc"

		sampler2D _MainTex;
        sampler2D _ShadowTex;
        sampler2D _SpecularColor;
        sampler2D _Glossiness;
        sampler2D _Bump;
        sampler2D _Emissive;

        uniform float _GlossPower;
        uniform float _SpecularIntensity;
        uniform float _EmissionPower;
        uniform float _DarkFXIntensity;
        
		struct Input {
			float2 uv_MainTex;
            float2 uv_Bump;
            float2 uv_SpecularColor;
            float2 uv_Glossiness;
            float2 uv_Emissive;
		};

        struct TBCSurfaceOutput {
            half3 Albedo;
            half3 Normal;
            half4 SpecColor;
            half Specular;
            half3 Emission;
            half4 DarkFX;
            half Alpha;
        };
        
        inline half4 LightingTBCRamp_PrePass (TBCSurfaceOutput s, half4 light)
        {
            half4 c;

            half lum = length(saturate(light.rgb))*2;

            half3 spec = lerp(0, saturate(s.SpecColor.rgb * s.SpecColor.a), light.a);

            half3 col = s.Albedo * light.rgb + light.rgb * spec;

            c.rgb = lerp(light.rgb + s.DarkFX.rgb, col, saturate(max(0.2, lum)));
            c.a = s.Alpha;
            return c;
        }  
            
		void surf (Input IN, inout TBCSurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
            half4 darkFX = tex2D (_ShadowTex, IN.uv_MainTex);
            half4 spec = tex2D (_SpecularColor, IN.uv_SpecularColor);
            half gloss = tex2D (_Glossiness, IN.uv_Glossiness).a;
            half4 emissive = tex2D (_Emissive, IN.uv_Emissive);

			o.Albedo = c.rgb;
            o.DarkFX.rgb = darkFX.rgb;
            o.DarkFX.a = darkFX.a * _DarkFXIntensity;
            o.Emission = lerp(0,emissive.rgb*_EmissionPower,emissive.a);
            o.Specular = _GlossPower * max(0.01,gloss);
            o.SpecColor.rgb = spec.rgb;
            o.SpecColor.a = _SpecularIntensity * max(0,spec.a);
            o.Normal = UnpackNormal(tex2D (_Bump, IN.uv_Bump));
			o.Alpha = c.a;
		}
		ENDCG
    }
    fallback "Transparent/Cutout/Diffuse"
}
