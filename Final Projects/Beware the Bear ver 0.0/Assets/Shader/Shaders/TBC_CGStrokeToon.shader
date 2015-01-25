Shader "Custom/TBC_CGStrokeToon" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BrushTex ("Stroke (Grey)", 2D) = "white" {}
		_UnlitDarkening ("Unlit Darkening Factor", Color) = (0.2,0.2,0.2,1)
		_DiffuseThreshold ("Light Threshold", Float) = 0.1
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_LitOutlineThickness ("Lit Outline Thickness",Float) = 0.1
		_UnlitOutlineThickness ("Unlit Outline Thickness",Float) = 0.4
		_SpecColor ("Specular Color", Color) = (1,1,1,1)
		_Shininess ("Shininess", Float) = 10
	}
	SubShader {
		Tags { "RenderType"="Opaque" "ForceNoShadowCasting"="FALSE"}
		LOD 2000
		
		//OUTLINE PASS
		Pass {
			Cull Front
			Lighting off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
 
			#include "UnityCG.cginc"
			
			struct vertexInput{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct vertexOutput{
				float4 pos : POSITION;
			};
			
			uniform float _LitOutlineThickness;
			uniform float4 _OutlineColor;
			
			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
				
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex + (float4(input.normal,0) * _LitOutlineThickness));
				
				return output;
			}
			
			float4 frag	(vertexOutput input):COLOR
			{
				return _OutlineColor;
			}
			ENDCG
		}
		
		Pass {
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
						
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"
			
			//UI Vars	
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform sampler2D _BrushTex;
			uniform float4 _BrushTex_ST;
			uniform float4 _UnlitDarkening;
			uniform float _DiffuseThreshold;
			uniform float4 _OutlineColor;
			uniform float _LitOutlineThickness;
			uniform float _UnlitOutlineThickness;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			
			uniform float4 _LightColor0;
			
			struct vertexInput{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texCoords : TEXCOORD0;
			};
			
			struct vertexOutput{
				float4 pos : SV_POSITION;
				float4 texCoords : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float4 posWorld : TEXCOORD2;
				float4 posLocal : TEXCOORD3;
			};
	
			
			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
			
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object;
				
				output.texCoords = input.texCoords;
				output.posWorld = mul(modelMatrix, input.vertex);
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				output.normal = normalize( float3( mul( float4(input.normal, 0.0), modelMatrixInverse).xyz ) );
				output.posLocal = output.pos;

				return output;
			}
			
			float4 frag(vertexOutput input) : COLOR
			{
				float3 normalDirection = normalize(input.normal);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - float3(input.posWorld.xyz));
				float3 lightDirection = normalize(WorldSpaceLightDir(input.posWorld));
				float4 mainTexColor = tex2D(_MainTex, float2(TRANSFORM_TEX(input.texCoords,_MainTex).xy));
				float4 brushTexColor = tex2D(_BrushTex, float2(TRANSFORM_TEX(input.texCoords,_BrushTex).xy)); 
				
				half attenuation;
				
				if(0.0 == _WorldSpaceLightPos0.w) // directional light
				{
					attenuation = 1.0;
				}
				else // spot light or point light
				{
					float3 vertexToLightSource = float3 (input.posWorld.xyz - _WorldSpaceLightPos0.xyz);
					float distance = length(vertexToLightSource);
					attenuation = 1.0/distance; //linear attenuation (can we parametrize this so user can choose attenuation?)
				}
				
				// default: unlit multiply the _MainTex with the darkening
				float3 fragmentColor = float3(_UnlitDarkening.xyz);
				
				// diffuse illum
				if(attenuation * max(0.0, dot(float3(normalDirection.xyz), lightDirection)) >= _DiffuseThreshold)
				{
					fragmentColor = float3(_LightColor0.xyz);
				}
				
				// outline
				if( dot(viewDirection,normalDirection) <= lerp(_UnlitOutlineThickness, _LitOutlineThickness, max(0.0, dot(normalDirection, lightDirection))))
				{
					fragmentColor = float3(_OutlineColor.xyz);
				}								
				
				fragmentColor *= float4(mainTexColor.xyz,1.0)*pow((abs(dot(float3(normalDirection.xyz), lightDirection))),float3(1.0,1.0,1.0)-brushTexColor.xyz);										
																																		
				// highLights
				if( dot(normalDirection, lightDirection) > 0.0 && attenuation * pow( max( 0.0, dot( reflect( -lightDirection, normalDirection), viewDirection) ), _Shininess) > 0.5)
				{
					fragmentColor = _SpecColor.w * float3(_LightColor0.xyz) * float3(_SpecColor.xyz) + (1.0 - _SpecColor.w)*fragmentColor;
				}
				
				return float4(fragmentColor, 1.0);		
			}
			ENDCG
		}
		
		Pass{
			Tags{ "LightMode" = "ForwardAdd" }
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			//UI Vars	
			uniform sampler2D _MainTex;
			uniform float4 _UnlitDarkening;
			uniform float _DiffuseThreshold;
			uniform float4 _OutlineColor;
			uniform float _LitOutlineThickness;
			uniform float _UnlitOutlineThickness;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			
			uniform float4 _LightColor0;
			
			struct vertexInput{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct vertexOutput{
				float4 pos : SV_POSITION;
				float3 normalDir : TEXCOORD1;
				float4 posWorld : TEXCOORD2;				
			};
			
			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object;
				
				output.pos = input.vertex;
				output.posWorld = mul(modelMatrix, input.vertex);
				output.normalDir = normalize( float3( mul( float4(input.normal,0.0), modelMatrixInverse).xyz));
				
				return output;
			}
			
			float4 frag(vertexOutput input): COLOR
			{
				float3 normalDirection = normalize(input.normalDir);
				float3 viewDirection = normalize( _WorldSpaceCameraPos.xyz - float3(input.posWorld.xyz));
				float3 lightDirection = normalize(WorldSpaceLightDir(input.posWorld));
				float attenuation;
				
				if(0.0 == _WorldSpaceLightPos0.w) // directional light
				{
					attenuation = 1.0;
				}
				else // spot light or point light
				{
					float3 vertexToLightSource = float3 (_WorldSpaceLightPos0.xyz - input.posWorld.xyz);
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance; //linear attenuation (can we parametrize this so user can choose attenuation?)
				}
				
				float4 fragmentColor = float4(0.0,0.0,0.0,0.0);
				
				// highLights
				if( dot(normalDirection, lightDirection) > 0.0 && attenuation * pow( max( 0.0, dot( reflect( -lightDirection, normalDirection), viewDirection) ), _Shininess) > 0.5)
				{
					fragmentColor = float4(_LightColor0.xyz,1.0) * _SpecColor;
				}
				
				return fragmentColor;	
			}
			
			ENDCG
		}
			
	} 
	//FallBack "Diffuse"
}
