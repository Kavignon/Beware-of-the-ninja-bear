Shader "TBC_Shaders/TBC_RampNormals" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _Bump ("Bump", 2D) = "bump" {}
        _Ramp ("Ramp", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf TBCRamp
        
        sampler2D _Ramp;

            half4 LightingTBCRamp  (SurfaceOutput s, half3 lightDir, half atten)
            {
                half NdotL = dot(s.Normal, lightDir);
                half diff = NdotL * 0.5 + 0.5;
                half3 ramp = tex2D (_Ramp, float2(diff,diff)).rgb;
                half4 c;
                c.rgb = s.Albedo * _LightColor0.rgb * ramp * atten * 2;
                c.a = s.Alpha;
                return c;
            }           

        #include "UnityCG.cginc"

		sampler2D _MainTex;
        sampler2D _Bump;

		struct Input {
			float2 uv_MainTex;
            float2 uv_Bump;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
            o.Normal = UnpackNormal(tex2D (_Bump, IN.uv_Bump));
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
