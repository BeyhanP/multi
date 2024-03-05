Shader "Custom/CelShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Colour", Color) = (1.0,1.0,1.0,1.0)
		[HDR]
		_AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)
		[HDR]
		_SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
		_Glossiness("Glossiness", Float) = 32
		[HDR]
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716
		_RimThreshold("Rim Threshold", Range(0,1)) = 0.1
		_OutlineAmount("Outline Amount", Float) = 0.01
		_Transparency("Transparency", Range(0,1)) = 1
		_OutlineByNormal("Outline By Normal", Range(0,1)) = 1
	}
	SubShader
	{
		
		Tags
		{
			"Queue"="Transparent"
			"RenderType"="Transparent"
			"LightMode"="ForwardBase"
			"PassFlags"="OnlyDirectional"
		}
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 worldNormal : NORMAL;
			};

			float _OutlineAmount;
			float _Transparency;
			float _OutlineByNormal;

			v2f vert (appdata v)
			{
				v2f o;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.vertex = UnityObjectToClipPos(_OutlineByNormal > 0.5 ? v.vertex + (v.normal * _OutlineAmount) : v.vertex * _OutlineAmount);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0,0,0,_Transparency);
				return col;
			}
			ENDCG
		}

		Pass
		{
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase // pragma HERE -------------------------------------------------------------------------------
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc" // INCLUDE HERE ----------------------------------------------------------------------------------

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldNormal : NORMAL;
				float3 viewDir : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				LIGHTING_COORDS(3,4) // LIGHTING COORDS HERE -------------------------------------------------------------------------
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			fixed4 _AmbientColor;
			float _Glossiness;
			float4 _SpecularColor;
			float4 _RimColor;
			float _RimAmount;
			float _RimThreshold;
			float _OutlineAmount;
			float _Transparency;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				TRANSFER_VERTEX_TO_FRAGMENT(o) // TRANSFER DONE HERE ---------------------------------------------------------------------
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				float3 normal = normalize(i.worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal);
				//float lightIntensity = smoothstep(0, 0.01, NdotL);
				float shadow = LIGHT_ATTENUATION(i);
				float lightIntensity = (NdotL > 0.75 ? 1 : (NdotL > 0.25 ? 0.5 : 0)) * shadow;  // SHADOW APPLIED HERE -----------------------------------------------
				float4 light = lightIntensity * _LightColor0;
				float3 viewDir = normalize(i.viewDir);

				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);

				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
				float specularIntensitySmooth = specularIntensity > 0.5 ? 1 : 0;
				float4 specular = specularIntensitySmooth * _SpecularColor;

				float4 rimDot = 1 - dot(viewDir, normal);
				float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
 				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
				float4 rim = rimIntensity * _RimColor;

				float4 outlineDot = 1 - dot(viewDir, normal);
				float outline = outlineDot > _OutlineAmount ? 1 : 0;

				fixed4 finalCol = _Color * col * (_AmbientColor + light + specular + rim);
				finalCol.a = _Transparency;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, finalCol);
				return finalCol;//outline > 0 ? fixed4(0,0,0,1) : finalCol;
			}
			ENDCG
		}
	}
	Fallback "VertexLit"
}