Shader "Ciconia Studio/CS_Glass/Builtin/SimpleLiquid(IOR)"
{
	Properties
	{
		[Header(Refraction)]
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		[Space(15)][Header(Global Properties )][Space(10)]_TilingX("Tiling X", Float) = 1
		_TilingY("Tiling Y", Float) = 1
		[Space(10)][Header(Main Properties)][Space(15)]_Color("Color", Color) = (0,0,0,0)
		_Brightness("Brightness", Range( 1 , 8)) = 1
		[Space(35)]_MetallicGlossMap("Metallic(RoughA)", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 2)) = 0
		_Glossiness("Smoothness", Range( 0 , 1)) = 1
		[Space(35)]_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Scale", Range( 0 , 20)) = 0.1
		[Space(10)]_IndexofRefraction("Index of Refraction", Float) = 1.3
		[Space(35)]_OcclusionMap("Ambient Occlusion Map", 2D) = "white" {}
		_AoIntensity("Ao Intensity", Range( 0 , 2)) = 0
		[Space(45)][Header(Reflection Properties) ][Space(15)]_ColorCubemap("Color ", Color) = (1,1,1,1)
		[HDR]_CubeMap("Cube Map", CUBE) = "black" {}
		_ReflectionIntensity("Reflection Intensity", Float) = 0
		_BlurReflection("Blur", Range( 0 , 8)) = 0
		[Space(15)]_ColorFresnel("Color Fresnel", Color) = (0,0,0,0)
		[ToggleOff(_USECUBEMAP_OFF)] _UseCubemap("Use Cubemap", Float) = 0
		_FresnelStrength("Fresnel Strength", Float) = 0
		_PowerFresnel("Power", Float) = 1
		[Space(45)][Header(Transparency Properties)][Space(15)]_Opacity("Opacity", Range( 0 , 1)) = 0.7
		[Space(10)][Toggle]_FalloffOpacity("Falloff Opacity", Float) = 0
		[Toggle]_Invert("Invert", Float) = 0
		[Space(10)]_FalloffOpacityIntensity("Falloff Intensity", Range( 0 , 1)) = 1
		_PowerFalloffOpacity("Power", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Liquid"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		AlphaToMask On
		GrabPass{ "RefractionGrab1" }
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _USECUBEMAP_OFF
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldRefl;
			float4 screenPos;
		};

		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform float _TilingX;
		uniform float _TilingY;
		uniform float _BumpScale;
		uniform float _Brightness;
		uniform float4 _Color;
		uniform float _FresnelStrength;
		uniform float _PowerFresnel;
		uniform float4 _ColorFresnel;
		uniform samplerCUBE _CubeMap;
		uniform float _BlurReflection;
		uniform float _ReflectionIntensity;
		uniform float4 _ColorCubemap;
		uniform float _Metallic;
		uniform sampler2D _MetallicGlossMap;
		uniform float4 _MetallicGlossMap_ST;
		uniform float _Glossiness;
		uniform sampler2D _OcclusionMap;
		uniform float4 _OcclusionMap_ST;
		uniform float _AoIntensity;
		uniform float _FalloffOpacity;
		uniform float _Opacity;
		uniform float _Invert;
		uniform float _FalloffOpacityIntensity;
		uniform float _PowerFalloffOpacity;
		uniform sampler2D RefractionGrab1;
		uniform float _ChromaticAberration;
		uniform float _IndexofRefraction;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		inline float4 Refraction( Input i, SurfaceOutputStandard o, float indexOfRefraction, float chomaticAberration ) {
			float3 worldNormal = o.Normal;
			float4 screenPos = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			float halfPosW = screenPos.w * 0.5;
			screenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;
			#if SHADER_API_D3D9 || SHADER_API_D3D11
				screenPos.w += 0.00000000001;
			#endif
			float2 projScreenPos = ( screenPos / screenPos.w ).xy;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 refractionOffset = ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) );
			float2 cameraRefraction = float2( refractionOffset.x, refractionOffset.y );
			float4 redAlpha = tex2D( RefractionGrab1, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( RefractionGrab1, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( RefractionGrab1, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutputStandard o, inout half4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
			float IOR103 = _IndexofRefraction;
			color.rgb = color.rgb + Refraction( i, o, IOR103, _ChromaticAberration ) * ( 1 - color.a );
			color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			float GlobalTilingX281 = _TilingX;
			float GlobalTilingY282 = _TilingY;
			float4 appendResult95 = (float4(( uv_BumpMap.x * GlobalTilingX281 ) , ( uv_BumpMap.y * GlobalTilingY282 ) , 0.0 , 0.0));
			float3 tex2DNode2 = UnpackScaleNormal( tex2D( _BumpMap, appendResult95.xy ), _BumpScale );
			float3 Normal101 = tex2DNode2;
			o.Normal = Normal101;
			float4 Albedo84 = CalculateContrast(_Brightness,_Color);
			float4 AlbedoAmbient117 = Albedo84;
			o.Albedo = AlbedoAmbient117.rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV163 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode163 = ( -0.05 + 1.0 * pow( 1.0 - fresnelNdotV163, _PowerFresnel ) );
			float4 clampResult482 = clamp( ( _ColorFresnel * fresnelNode163 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 ifLocalVar474 = 0;
			if( 0.0 > _FresnelStrength )
				ifLocalVar474 = float4( ( float3(1,1,1) * fresnelNode163 ) , 0.0 );
			else if( 0.0 < _FresnelStrength )
				ifLocalVar474 = clampResult482;
			float clampResult488 = clamp( _FresnelStrength , -1.0 , 75.0 );
			float3 NormalmapXYZ170 = tex2DNode2;
			float4 texCUBENode6 = texCUBElod( _CubeMap, float4( WorldReflectionVector( i , NormalmapXYZ170 ), _BlurReflection) );
			float4 temp_cast_3 = (1.0).xxxx;
			#ifdef _USECUBEMAP_OFF
				float4 staticSwitch457 = temp_cast_3;
			#else
				float4 staticSwitch457 = texCUBENode6;
			#endif
			float4 Cubmap179 = ( ( ( ifLocalVar474 * clampResult488 ) * staticSwitch457 ) + ( texCUBENode6 * ( texCUBENode6.a * _ReflectionIntensity ) * _ColorCubemap ) );
			o.Emission = Cubmap179.rgb;
			float2 uv_MetallicGlossMap = i.uv_texcoord * _MetallicGlossMap_ST.xy + _MetallicGlossMap_ST.zw;
			float4 appendResult275 = (float4(( uv_MetallicGlossMap.x * GlobalTilingX281 ) , ( uv_MetallicGlossMap.y * GlobalTilingY282 ) , 0.0 , 0.0));
			float4 tex2DNode123 = tex2D( _MetallicGlossMap, appendResult275.xy );
			float Metallic110 = ( _Metallic * tex2DNode123.r );
			o.Metallic = Metallic110;
			float Roughness111 = ( tex2DNode123.a * _Glossiness );
			o.Smoothness = Roughness111;
			float2 uv_OcclusionMap = i.uv_texcoord * _OcclusionMap_ST.xy + _OcclusionMap_ST.zw;
			float4 appendResult267 = (float4(( uv_OcclusionMap.x * GlobalTilingX281 ) , ( uv_OcclusionMap.y * GlobalTilingY282 ) , 0.0 , 0.0));
			float blendOpSrc136 = tex2D( _OcclusionMap, appendResult267.xy ).r;
			float blendOpDest136 = ( 1.0 - _AoIntensity );
			float Occlusion140 = ( saturate( ( 1.0 - ( 1.0 - blendOpSrc136 ) * ( 1.0 - blendOpDest136 ) ) ));
			o.Occlusion = Occlusion140;
			float lerpResult68 = lerp( -3.0 , 0.0 , _FalloffOpacityIntensity);
			float fresnelNdotV25 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode25 = ( lerpResult68 + _PowerFalloffOpacity * pow( 1.0 - fresnelNdotV25, (( 1.0 + -_Opacity ) + (1.0 - 0.0) * (_Opacity - ( 1.0 + -_Opacity )) / (1.0 - 0.0)) ) );
			float clampResult45 = clamp( (( _Invert )?( ( 1.0 - fresnelNode25 ) ):( fresnelNode25 )) , 0.0 , 1.0 );
			float Opacity87 = (( _FalloffOpacity )?( clampResult45 ):( ( 1.0 - _Opacity ) ));
			o.Alpha = Opacity87;
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha finalcolor:RefractionF fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			AlphaToMask Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.worldRefl = -worldViewDir;
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}