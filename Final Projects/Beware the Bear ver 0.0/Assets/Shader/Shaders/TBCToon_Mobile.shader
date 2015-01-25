Shader "TBC_Toon/Mobile/TBC_CGToon" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_UnlitDarkening ("Unlit Darkening Factor", Color) = (0.2,0.2,0.2,1)
		_SpecColor ("SpTecular Color", Color) = (1,1,1,1)
		_Shininess ("Shininess", Range(0.5,1)) = 1
		_DiffuseThreshold ("Light Threshold", Float) = 0.1
		_Diffusion ("Diffusion", Range(0,0.99)) = 0.0
		_SpecDiffusion ("Specular Diffusion", Range(0,0.99)) = 0.0
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
			
			//UI Vars	
			uniform sampler2D _MainTex;
			uniform fixed4 _MainTex_ST;
			uniform fixed4 _UnlitDarkening;
			uniform fixed4 _SpecColor;
			uniform fixed _Shininess;
			uniform half _DiffuseThreshold;
			uniform fixed _Diffusion;
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
				fixed4 diffuseColor = tex2D(_MainTex, float2(TRANSFORM_TEX(input.texCoords,_MainTex).xy));
				fixed4 darkenedColor = diffuseColor * _UnlitDarkening;
				fixed4 specColor = saturate(diffuseColor + _SpecColor);
				fixed nDotL = saturate(dot(input.normalDir, input.lightDir.xyz));
				
				//The cutoff between light and dark areas - to be replaced with ramp
				fixed diffuseBorder = saturate( ( max(_DiffuseThreshold,nDotL) - _DiffuseThreshold) * pow( (2-_Diffusion), 10) );
				fixed specBorder = saturate( ( max( _Shininess, dot( reflect( -input.lightDir.xyz, input.normalDir ), input.viewDir ))-_Shininess)*pow((2-_SpecDiffusion),10));
				
				//The light
				fixed3 ambientLight = (1-diffuseBorder)*darkenedColor.xyz;
				fixed3 diffuseReflection = (1-specBorder) * diffuseColor.xyz * diffuseBorder;
				fixed3 specReflection = specColor.xyz * specBorder;
				
				fixed3 finalColor = ((ambientLight + diffuseReflection) + specReflection)*_LightColor0;
				
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
				fixed4 diffuseColor = tex2D(_MainTex, float2(TRANSFORM_TEX(input.texCoords,_MainTex).xy));
				fixed4 specColor = saturate(diffuseColor+_SpecColor);
				
				//The cutoff between light and dark areas - to be replaced with ramp
				fixed specBorder = saturate( ( max( _Shininess, dot( reflect( -input.lightDir.xyz, input.normalDir ), input.viewDir ))-_Shininess)*pow((2-_SpecDiffusion),10));
				
				fixed3 specReflection = specColor.xyz * specBorder;
				
				fixed3 finalColor = specReflection*_LightColor0 ;
				
				return fixed4(finalColor,1.0);
			}
			ENDCG
		}		
	} 
	//FallBack "Diffuse"
}
