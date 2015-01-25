Shader "TBC_Toon/Desktop/TBC_CGToon_HSVOutline" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_HueShift ("Hue Shift (0 to 360)", Float) = 0
		_Saturation ("Saturation Multiplier", Float)=1.0
		_Value ("Value Multiplier", Float)=1.0
		_UnlitDarkening ("Unlit Darkening Factor", Color) = (0.2,0.2,0.2,1)
		_SpecColor ("SpTecular Color", Color) = (1,1,1,1)
		_Shininess ("Shininess", Range(0.5,1)) = 1
		_DiffuseThreshold ("Light Threshold", Float) = 0.1
		_Diffusion ("Diffusion", Range(0,0.99)) = 0.0
		_SpecDiffusion ("Specular Diffusion", Range(0,0.99)) = 0.0
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_OutlineThickness ("Outline Thickness", range(0,1)) = 0.1
		_OutlineDiffusion ("Outline Diffusion",range(0,0.99)) = 0.0
	}
	SubShader {
		//Tags { "RenderType"="Opaque" "ForceNoShadowCasting"="FALSE"}
		LOD 200
		
		
		Pass {
			Tags{ "LightMode" = "ForwardBase" }
			
			CGPROGRAM
						
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			#pragma target 3.0
			
			
			float3 shift_col(float3 RGB, float3 shift)

            {

            float3 RESULT = float3(RGB);

            float VSU = shift.z*shift.y*cos(shift.x*3.14159265/180);
                float VSW = shift.z*shift.y*sin(shift.x*3.14159265/180);
                RESULT.x = (.299*shift.z+.701*VSU+.168*VSW)*RGB.x
                   + (.587*shift.z-.587*VSU+.330*VSW)*RGB.y
                   + (.114*shift.z-.114*VSU-.497*VSW)*RGB.z;
 
                RESULT.y = (.299*shift.z-.299*VSU-.328*VSW)*RGB.x
                   + (.587*shift.z+.413*VSU+.035*VSW)*RGB.y
                   + (.114*shift.z-.114*VSU+.292*VSW)*RGB.z;
             
                RESULT.z = (.299*shift.z-.3*VSU+1.25*VSW)*RGB.x
 	               + (.587*shift.z-.588*VSU-1.05*VSW)*RGB.y
		           + (.114*shift.z+.886*VSU-.203*VSW)*RGB.z;
	            return (RESULT);
            }
						
			//UI Vars	
			uniform sampler2D _MainTex;
			uniform fixed4 _MainTex_ST;
			uniform fixed4 _UnlitDarkening;
			uniform fixed4 _SpecColor;
			uniform fixed _Shininess;
			uniform half _DiffuseThreshold;
			uniform fixed _Diffusion;
			uniform fixed _SpecDiffusion;
			uniform half _HueShift;
			uniform half _Saturation;
			uniform half _Value;
			uniform fixed4 _OutlineColor;
			uniform fixed _OutlineThickness;
			uniform fixed _OutlineDiffusion;
			
			//unity defined variables
			uniform half4 _LightColor0;
			
			
			//base input structs
			struct vertexInput{
				half4 vertex : POSITION;
				half3 normal : NORMAL;
				half4 texCoords : TEXCOORD0;
			};
			
			struct vertexOutput{
				half4 pos : SV_POSITION;
				half4 texCoords : TEXCOORD0;
				fixed3 normalDir : TEXCOORD1;
				fixed4 lightDir : TEXCOORD2;
				fixed3 viewDir : TEXCOORD3;
			};
				
			//vertex Function
			vertexOutput vert(vertexInput input){
				vertexOutput output;
				
				output.texCoords = input.texCoords;
				//normalDirection
				output.normalDir = normalize( mul( half4( input.normal, 0.0 ), _World2Object ).xyz );
				
				//unity transform position
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				
				//world position
				half4 posWorld = mul(_Object2World, input.vertex);
				//view direction
				output.viewDir = normalize( _WorldSpaceCameraPos.xyz - posWorld.xyz );
				//light direction
				half3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - posWorld.xyz;
				output.lightDir = fixed4(
					normalize( lerp(_WorldSpaceLightPos0.xyz , fragmentToLightSource, _WorldSpaceLightPos0.w) ),
					lerp(1.0 , 1.0/length(fragmentToLightSource), _WorldSpaceLightPos0.w)
				);
				
				return output;
			}
			
			float4 frag(vertexOutput input) : COLOR
			{
				fixed4 diffuseColor = tex2D(_MainTex, float2(TRANSFORM_TEX(input.texCoords,_MainTex).xy));
				fixed3 shift = float3(clamp(_HueShift,0,360), _Saturation, _Value);
				diffuseColor = fixed4(half3(shift_col(diffuseColor.rgb,shift)),diffuseColor.a);
				
				fixed4 darkenedColor = diffuseColor * _UnlitDarkening;
				fixed4 specColor = saturate(diffuseColor + _SpecColor);
				fixed nDotL = saturate(dot(input.normalDir, input.lightDir.xyz));
				
				//The cutoff between light and dark areas - to be replaced with ramp
				fixed diffuseBorder = saturate( ( max(_DiffuseThreshold,nDotL) - _DiffuseThreshold) * pow( (2-_Diffusion), 10) );
				fixed specBorder = saturate( ( max( _Shininess, dot( reflect( -input.lightDir.xyz, input.normalDir ), input.viewDir ))-_Shininess)*pow((2-_SpecDiffusion),10));
				
				fixed outlineStrength = saturate( (dot( input.normalDir, input.viewDir ) - _OutlineThickness) * pow( (2-_OutlineDiffusion),10) + _OutlineThickness);
				
				//The light
				fixed3 ambientLight = (1-diffuseBorder)*darkenedColor.xyz;
				fixed3 diffuseReflection = (1-specBorder) * diffuseColor.xyz * diffuseBorder;
				fixed3 specReflection = specColor.xyz * specBorder;
				
				fixed3 finalColor = lerp(_OutlineColor.xyz,(((ambientLight + diffuseReflection) + specReflection)* _LightColor0.xyz),outlineStrength);
				
				return fixed4(finalColor,1.0);
			}
			ENDCG
		}
		Pass {
			Tags{ "LightMode" = "ForwardAdd" }
			Blend SrcAlpha SrcAlpha 
			CGPROGRAM
						
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//UI Vars	
			uniform sampler2D _MainTex;
			uniform fixed4 _MainTex_ST;
			uniform fixed4 _SpecColor;
			uniform fixed _Shininess;
			uniform fixed _SpecDiffusion;

			//unity defined variables
			uniform half4 _LightColor0;
			
			//base input structs
			struct vertexInput{
				half4 vertex : POSITION;
				half3 normal : NORMAL;
				half4 texCoords : TEXCOORD0;
			};
			
			struct vertexOutput{
				half4 pos : SV_POSITION;
				half4 texCoords : TEXCOORD0;
				fixed3 normalDir : TEXCOORD1;
				fixed4 lightDir : TEXCOORD2;
				fixed3 viewDir : TEXCOORD3;
			};
				
			//vertex Function
			vertexOutput vert(vertexInput input){
				vertexOutput output;
				
				output.texCoords = input.texCoords;
				//normalDirection
				output.normalDir = normalize( mul( half4( input.normal, 0.0 ), _World2Object ).xyz );
				
				//unity transform position
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				
				//world position
				half4 posWorld = mul(_Object2World, input.vertex);
				//view direction
				output.viewDir = normalize( _WorldSpaceCameraPos.xyz - posWorld.xyz );
				//light direction
				half3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - posWorld.xyz;
				output.lightDir = fixed4(
					normalize( lerp(_WorldSpaceLightPos0.xyz , fragmentToLightSource, _WorldSpaceLightPos0.w) ),
					lerp(1.0 , 1.0/length(fragmentToLightSource), _WorldSpaceLightPos0.w)
				);
				
				return output;
			}
			
			float4 frag(vertexOutput input) : COLOR
			{
				//fixed4 diffuseColor = tex2D(_MainTex, float2(TRANSFORM_TEX(input.texCoords,_MainTex).xy));
				fixed4 specColor = saturate(_SpecColor);
				
				//The cutoff between light and dark areas - to be replaced with ramp
				fixed specBorder = saturate( ( max( _Shininess, dot( reflect( -input.lightDir.xyz, input.normalDir ), input.viewDir ))-_Shininess)*pow((2-_SpecDiffusion),10));
				
				fixed3 specReflection = specColor.xyz * specBorder;
				
				fixed3 finalColor = saturate((specReflection)) ;
				
				return fixed4(finalColor,1.0);
			}
			ENDCG
		}		
	} 
	FallBack "TBC_Toon/Mobile/TBC_CGToon_Outline"
}
