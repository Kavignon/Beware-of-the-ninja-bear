Shader "TBC_Toon/Desktop/TBC_CGToon_HSVRamp" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp ("Ramp", 2D) = "gray" {}
		_HueShift ("Hue Shift (0 to 360)", Float) = 0
		_Saturation ("Saturation Multiplier", Float)=1.0
		_Value ("Value Multiplier", Float)=1.0
		_UnlitDarkening ("Unlit Darkening Factor", Color) = (0.2,0.2,0.2,1)
		_SpecColor ("Specular Color", Color) = (1,1,1,1)
		_Shininess ("Shininess", Range(0.5,1)) = 1
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
			uniform sampler2D _Ramp;
			uniform fixed4 _Ramp_ST;
			uniform fixed4 _UnlitDarkening;
			uniform fixed4 _SpecColor;
			uniform fixed _Shininess;
			uniform fixed _SpecDiffusion;
			uniform half _HueShift;
			uniform half _Saturation;
			uniform half _Value;
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
				fixed nDotL = dot(input.normalDir, input.lightDir.xyz) * 0.5 + 0.5;
				fixed3 ramp = tex2D(_Ramp, float2(nDotL,(TRANSFORM_TEX(input.texCoords,_Ramp).y) ) ).rgb;
				
				//The cutoff between light and dark areas - to be replaced with ramp
				fixed specBorder = saturate( ( max( _Shininess*ramp, dot( reflect( -input.lightDir.xyz, input.normalDir ), input.viewDir ))-_Shininess)*pow((2-_SpecDiffusion),10));
								
				//The light
				fixed3 ambientLight = (1-ramp)*darkenedColor.xyz;
				fixed3 diffuseReflection = (1-specBorder) * diffuseColor.xyz * ramp;
				fixed3 specReflection = specColor.xyz * specBorder * ramp;
				
				fixed3 finalColor = (ambientLight + diffuseReflection) + specReflection * _LightColor0;
				
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
			uniform sampler2D _Ramp;
			uniform fixed4 _Ramp_ST;

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
				
				fixed4 specColor = saturate(_SpecColor);
				fixed nDotL = dot(input.normalDir, input.lightDir.xyz) * 0.5 + 0.5;
				fixed3 ramp = tex2D(_Ramp, float2(nDotL,(TRANSFORM_TEX(input.texCoords,_Ramp).y) ) ).rgb;
				
				//The cutoff between light and dark areas - to be replaced with ramp
				fixed specBorder = saturate( ( max( _Shininess*ramp, dot( reflect( -input.lightDir.xyz, input.normalDir ), input.viewDir ))-_Shininess)*pow((2-_SpecDiffusion),10));
				
				fixed3 specReflection = specColor.xyz * specBorder;
				
				fixed3 finalColor = (specReflection*_LightColor0) ;
				
				return fixed4(finalColor,1.0);
			}
			ENDCG
		}		
	} 
	FallBack "TBC_Toon/Mobile/TBC_CGToon"
}
