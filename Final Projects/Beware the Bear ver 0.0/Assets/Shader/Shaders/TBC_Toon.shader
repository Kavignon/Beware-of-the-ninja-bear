Shader "Custom/TBC_Toon" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color("DEBUG COLOR", Color) = (1,1,1,1)
		_UnlitDarkening ("Unlit Darkening Factor", Color) = (0.2,0.2,0.2,1)
		_DiffuseThreshold ("Light Threshold", Float) = 0.1
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_LitOutlineThickness ("Lit Outline Thickness",Range(0,1)) = 0.1
		_UnlitOutlineThickness ("Unlit Outline Thickness",Range(0,1)) = 0.4
		_SpecColor ("Specular Color", Color) = (1,1,1,1)
		_Shininess ("Shininess", Float) = 10
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			Tags{ "LightMode" = "ForwardBase" }
			GLSLPROGRAM
			#include "UnityCG.glslinc"
			
			//UI Vars	
			uniform sampler2D _MainTex;
			uniform vec4 _UnlitDarkening;
			uniform fixed _DiffuseThreshold;
			uniform vec4 _OutlineColor;
			uniform fixed _LitOutlineThickness;
			uniform fixed _UnlitOutlineThickness;
			uniform vec4 _SpecColor;
			uniform half _Shininess;
			
			uniform vec4 _LightColor0;
			
			varying vec4 position;
			varying vec3 varyingNormalDirection;
			varying vec4 textureCoordinates;
			
			#ifdef VERTEX
	
			void main()
			{
				mat4 modelMatrix = _Object2World;
				mat4 modelMatrixInverse = _World2Object;
				
				position = modelMatrix * gl_Vertex;
				varyingNormalDirection = normalize(vec3(vec4(gl_Normal, 0.0) * modelMatrixInverse));
				textureCoordinates = gl_MultiTexCoord0;
				
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}
			
			#endif
			
			#ifdef FRAGMENT
			
			void main()
			{
				vec3 normalDirection = normalize(varyingNormalDirection);
				
				vec3 viewDirection = normalize(_WorldSpaceCameraPos - vec3(position));
				vec3 lightDirection;
				
				half attenuation;
				
				if(0.0 == _WorldSpaceLightPos0.w) // directional light
				{
					attenuation = 1.0;
					lightDirection = normalize(vec3(_WorldSpaceLightPos0));
				}
				else // spot light or point light
				{
					vec3 vertexToLightSource = vec3 (_WorldSpaceLightPos0 - position);
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance; //linear attenuation (can we parametrize this so user can choose attenuation?)
					lightDirection = normalize(vertexToLightSource);
				}
				
				// default: unlit multiply the _MainTex with the darkening
				vec3 fragmentColor = texture2D(_MainTex, vec2(textureCoordinates.xy)) * vec3(_UnlitDarkening.xyz);
				
				// diffuse illum
				if(attenuation * max(0.0, dot(normalDirection, lightDirection)) >= _DiffuseThreshold)
				{
					fragmentColor = vec3(_LightColor0) * texture2D(_MainTex, vec2(textureCoordinates.xy));
				}
				
				
				gl_FragColor = vec4(fragmentColor,1.0);				
			}
			
			#endif
			ENDGLSL
		}
	} 
	//FallBack "Diffuse"
}
