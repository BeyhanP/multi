Shader "Ciconia Studio/CS_Glass/Builtin/Glass"
{
	Properties
	{
		[Space(15)][Header(Global Properties )][Space(10)]_TilingX("Tiling X", Float) = 1
		_TilingY("Tiling Y", Float) = 1
		[Space(10)]_OffsetX("Offset X", Float) = 0
		_OffsetY("Offset Y", Float) = 0
		[Space(15)]
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2 //"Back"
		[Enum(Off,0,On,1)] _ZWrite("ZWrite", Float) = 1.0 //"On"

		[Space(10)][Header(Main Properties)][Space(15)]_Color("Color", Color) = (0,0,0,0)
		[Space(10)]_MainTex("Albedo -->(Mask A)", 2D) = "white" {}
		_Saturation("Saturation", Float) = 0
		_Brightness("Brightness", Range( 1 , 8)) = 1
		[Space(35)]_MetallicGlossMap("Metallic(RoughA)", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 2)) = 0.2
		_Glossiness("Smoothness", Range( 0 , 1)) = 0.5
		[Space(35)]_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Scale", Float) = 0.3
		_Refraction("Refraction", Range( 0 , 2)) = 1.1
		[Space(35)]_OcclusionMap("Ambient Occlusion Map", 2D) = "white" {}
		_AoIntensity("Ao Intensity", Range( 0 , 2)) = 0
		[Space(45)][Header(Self Illumination)][Space(15)]_Intensity("Intensity", Range( 1 , 10)) = 1
		[Space(45)][Header(Reflection Properties) ][Space(15)]_ColorCubemap("Color ", Color) = (1,1,1,1)
		[HDR]_CubeMap("Cube Map", CUBE) = "black" {}
		_ReflectionIntensity("Reflection Intensity", Float) = 1
		_BlurReflection("Blur", Range( 0 , 8)) = 0
		[Space(15)]_ColorFresnel1("Color Fresnel", Color) = (1,1,1,0)
		[ToggleOff(_USECUBEMAP_OFF)] _UseCubemap("Use Cubemap", Float) = 1
		_FresnelStrength("Fresnel Strength", Range( 0 , 8)) = 0
		_PowerFresnel("Power", Float) = 1
		[Space(45)][Header(Transparency Properties)][Space(15)]_Opacity("Opacity", Range( 0 , 1)) = 1
		[Space(10)][Toggle]_UseAlbedoA1("Use AlbedoA", Float) = 0
		[Toggle]_InvertAlbedoA1("Invert", Float) = 0
		[Space(10)][Toggle]_UseSmoothness("Use Smoothness", Float) = 0
		[Space(10)][Toggle]_FalloffOpacity("Falloff Opacity", Float) = 0
		[Toggle]_Invert("Invert", Float) = 0
		[Space(10)]_FalloffOpacityIntensity("Falloff Intensity", Range( 0 , 1)) = 1
		_PowerFalloffOpacity("Power", Float) = 1
		[Space(45)][Header(Fade Properties)][Space(15)]_Fade("Fade", Range( 0 , 1)) = 0.2
		[Space(10)][Toggle]_FalloffFade1("Exclude Decal", Float) = 0
		[Space(10)][Toggle]_FalloffFade("Falloff", Float) = 0
		[Toggle]_InvertFresnelFade("Invert", Float) = 0
		[Space(10)]_GradientFade("Falloff Intensity", Range( 0 , 1)) = 1
		_PowerFalloffFade("Power", Float) = 1
		[Space(45)][Header(Decal Properties)][Space(15)]_ColorDecal("Color -->(Transparency A)", Color) = (1,1,1,1)
		[Space(10)]_DetailAlbedoMap("Decal Map -->(Mask A)", 2D) = "black" {}
		_SaturationDecal("Saturation", Float) = 0
		[Space(20)]_MetallicDecal("Metallic", Range( 0 , 2)) = 0.2
		_GlossinessDecal("Smoothness", Range( 0 , 1)) = 0.5
		_ReflectionDecal("Reflection", Range( 0 , 1)) = 0
		[Space(35)]_DetailNormalMap("Normal Map", 2D) = "bump" {}
		_BumpScaleDecal("Scale", Range( 0 , 5)) = 0.1
		_BumpScaleDecal1("NormalBlend", Range( 0 , 1)) = 0
		[Space(25)]_Rotation("Rotation", Float) = 0
		[HDR][Space(35)]_EmissionColor("Emission Color", Color) = (0,0,0,0)
		_EmissiveIntensity("Emissive Intensity", Range( 0 , 2)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Glass"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		GrabPass{ "_ScreenGrab0" }
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _USECUBEMAP_OFF
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
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
		uniform float _OffsetX;
		uniform float _OffsetY;
		uniform float _BumpScale;
		uniform sampler2D _DetailNormalMap;
		uniform float4 _DetailNormalMap_ST;
		uniform float _Rotation;
		uniform float _BumpScaleDecal;
		uniform float _BumpScaleDecal1;
		uniform sampler2D _DetailAlbedoMap;
		uniform float4 _DetailAlbedoMap_ST;
		uniform float4 _ColorDecal;
		uniform float _Brightness;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Saturation;
		uniform float _SaturationDecal;
		uniform float _FresnelStrength;
		uniform float _PowerFresnel;
		uniform float4 _ColorFresnel1;
		uniform samplerCUBE _CubeMap;
		uniform float _BlurReflection;
		uniform float _ReflectionIntensity;
		uniform float4 _ColorCubemap;
		uniform float _ReflectionDecal;
		uniform float4 _EmissionColor;
		uniform float _EmissiveIntensity;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _ScreenGrab0 )
		uniform float _Refraction;
		uniform float _UseSmoothness;
		uniform float _UseAlbedoA1;
		uniform float _FalloffOpacity;
		uniform float _Intensity;
		uniform float _Opacity;
		uniform float _Invert;
		uniform float _FalloffOpacityIntensity;
		uniform float _PowerFalloffOpacity;
		uniform float _InvertAlbedoA1;
		uniform sampler2D _MetallicGlossMap;
		uniform float4 _MetallicGlossMap_ST;
		uniform float _Metallic;
		uniform float _MetallicDecal;
		uniform float _Glossiness;
		uniform float _GlossinessDecal;
		uniform sampler2D _OcclusionMap;
		uniform float4 _OcclusionMap_ST;
		uniform float _AoIntensity;
		uniform float _FalloffFade1;
		uniform float _FalloffFade;
		uniform float _Fade;
		uniform float _InvertFresnelFade;
		uniform float _GradientFade;
		uniform float _PowerFalloffFade;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			float2 break26_g21 = uv_BumpMap;
			float GlobalTilingX281 = ( _TilingX - 1.0 );
			float GlobalTilingY282 = ( _TilingY - 1.0 );
			float2 appendResult14_g21 = (float2(( break26_g21.x * GlobalTilingX281 ) , ( break26_g21.y * GlobalTilingY282 )));
			float GlobalOffsetX541 = _OffsetX;
			float GlobalOffsetY540 = _OffsetY;
			float2 appendResult13_g21 = (float2(( break26_g21.x + GlobalOffsetX541 ) , ( break26_g21.y + GlobalOffsetY540 )));
			float3 tex2DNode2 = UnpackScaleNormal( tex2D( _BumpMap, ( appendResult14_g21 + appendResult13_g21 ) ), _BumpScale );
			float2 uv_DetailNormalMap = i.uv_texcoord * _DetailNormalMap_ST.xy + _DetailNormalMap_ST.zw;
			float Rotator_Detailsmaps411 = _Rotation;
			float cos409 = cos( radians( Rotator_Detailsmaps411 ) );
			float sin409 = sin( radians( Rotator_Detailsmaps411 ) );
			float2 rotator409 = mul( uv_DetailNormalMap - float2( 0.5,0.5 ) , float2x2( cos409 , -sin409 , sin409 , cos409 )) + float2( 0.5,0.5 );
			float3 tex2DNode194 = UnpackScaleNormal( tex2D( _DetailNormalMap, rotator409 ), _BumpScaleDecal );
			float3 lerpResult308 = lerp( tex2DNode194 , BlendNormals( tex2DNode2 , tex2DNode194 ) , _BumpScaleDecal1);
			float2 uv_DetailAlbedoMap = i.uv_texcoord * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
			float cos400 = cos( radians( _Rotation ) );
			float sin400 = sin( radians( _Rotation ) );
			float2 rotator400 = mul( uv_DetailAlbedoMap - float2( 0.5,0.5 ) , float2x2( cos400 , -sin400 , sin400 , cos400 )) + float2( 0.5,0.5 );
			float4 tex2DNode181 = tex2D( _DetailAlbedoMap, rotator400 );
			float DecalOpacity251 = _ColorDecal.a;
			float DecalMask182 = ( tex2DNode181.a * DecalOpacity251 );
			float3 lerpResult192 = lerp( tex2DNode2 , lerpResult308 , DecalMask182);
			float3 Normal101 = lerpResult192;
			o.Normal = Normal101;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 break26_g18 = uv_MainTex;
			float2 appendResult14_g18 = (float2(( break26_g18.x * GlobalTilingX281 ) , ( break26_g18.y * GlobalTilingY282 )));
			float2 appendResult13_g18 = (float2(( break26_g18.x + GlobalOffsetX541 ) , ( break26_g18.y + GlobalOffsetY540 )));
			float4 tex2DNode76 = tex2D( _MainTex, ( appendResult14_g18 + appendResult13_g18 ) );
			float4 temp_output_297_0 = ( _Color * tex2DNode76 );
			float clampResult239 = clamp( _Saturation , -1.0 , 100.0 );
			float3 desaturateInitialColor211 = temp_output_297_0.rgb;
			float desaturateDot211 = dot( desaturateInitialColor211, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar211 = lerp( desaturateInitialColor211, desaturateDot211.xxx, -clampResult239 );
			float4 temp_output_303_0 = CalculateContrast(_Brightness,float4( desaturateVar211 , 0.0 ));
			float clampResult235 = clamp( _SaturationDecal , -1.0 , 100.0 );
			float3 desaturateInitialColor203 = tex2DNode181.rgb;
			float desaturateDot203 = dot( desaturateInitialColor203, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar203 = lerp( desaturateInitialColor203, desaturateDot203.xxx, -clampResult235 );
			float4 Decal248 = ( _ColorDecal * float4( desaturateVar203 , 0.0 ) );
			float4 lerpResult183 = lerp( temp_output_303_0 , Decal248 , DecalMask182);
			float4 AlbedoAmbient117 = lerpResult183;
			o.Albedo = AlbedoAmbient117.rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV163 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode163 = ( -0.05 + 1.0 * pow( 1.0 - fresnelNdotV163, _PowerFresnel ) );
			float4 clampResult468 = clamp( ( _ColorFresnel1 * fresnelNode163 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 ifLocalVar470 = 0;
			if( 0.0 > _FresnelStrength )
				ifLocalVar470 = float4( ( float3(1,1,1) * fresnelNode163 ) , 0.0 );
			else if( 0.0 < _FresnelStrength )
				ifLocalVar470 = clampResult468;
			float clampResult463 = clamp( _FresnelStrength , -1.0 , 75.0 );
			float3 NormalmapXYZ170 = tex2DNode2;
			float4 texCUBENode6 = texCUBElod( _CubeMap, float4( WorldReflectionVector( i , NormalmapXYZ170 ), _BlurReflection) );
			float4 temp_cast_6 = (1.0).xxxx;
			#ifdef _USECUBEMAP_OFF
				float4 staticSwitch461 = temp_cast_6;
			#else
				float4 staticSwitch461 = texCUBENode6;
			#endif
			float4 temp_output_169_0 = ( ( ( ifLocalVar470 * clampResult463 ) * staticSwitch461 ) + ( texCUBENode6 * ( texCUBENode6.a * _ReflectionIntensity ) * _ColorCubemap ) );
			float4 lerpResult292 = lerp( temp_output_169_0 , ( temp_output_169_0 * _ReflectionDecal ) , DecalMask182);
			float4 Cubmap179 = lerpResult292;
			float4 AlbedoDecal_RGB232 = tex2DNode181;
			float4 Emission220 = ( ( _EmissionColor * AlbedoDecal_RGB232 * _EmissiveIntensity ) * DecalMask182 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float4 screenColor381 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_ScreenGrab0,( (ase_grabScreenPosNorm).xyzw + float4( (( ( Normal101 + mul( float4( ase_normWorldNormal , 0.0 ), UNITY_MATRIX_V ).xyz ) * (-1.0 + (_Refraction - 0.0) * (1.0 - -1.0) / (2.0 - 0.0)) )).xyz , 0.0 ) ).xy);
			float4 GrabSreenRefraction385 = screenColor381;
			float lerpResult483 = lerp( 0.0 , _Intensity , _Opacity);
			float lerpResult68 = lerp( -3.0 , 0.0 , _FalloffOpacityIntensity);
			float fresnelNdotV25 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode25 = ( lerpResult68 + _PowerFalloffOpacity * pow( 1.0 - fresnelNdotV25, (( 1.0 + -lerpResult483 ) + (1.0 - 0.0) * (lerpResult483 - ( 1.0 + -lerpResult483 )) / (1.0 - 0.0)) ) );
			float clampResult45 = clamp( (( _Invert )?( ( 1.0 - fresnelNode25 ) ):( fresnelNode25 )) , 0.0 , 1.0 );
			float AlbedoA250 = tex2DNode76.a;
			float2 uv_MetallicGlossMap = i.uv_texcoord * _MetallicGlossMap_ST.xy + _MetallicGlossMap_ST.zw;
			float2 break26_g22 = uv_MetallicGlossMap;
			float2 appendResult14_g22 = (float2(( break26_g22.x * GlobalTilingX281 ) , ( break26_g22.y * GlobalTilingY282 )));
			float2 appendResult13_g22 = (float2(( break26_g22.x + GlobalOffsetX541 ) , ( break26_g22.y + GlobalOffsetY540 )));
			float4 tex2DNode123 = tex2D( _MetallicGlossMap, ( appendResult14_g22 + appendResult13_g22 ) );
			float RougnessA370 = tex2DNode123.a;
			float lerpResult189 = lerp( (( _UseSmoothness )?( ( (( _UseAlbedoA1 )?( ( (( _FalloffOpacity )?( clampResult45 ):( ( 1.0 - lerpResult483 ) )) * (( _InvertAlbedoA1 )?( ( 1.0 - AlbedoA250 ) ):( AlbedoA250 )) ) ):( (( _FalloffOpacity )?( clampResult45 ):( ( 1.0 - lerpResult483 ) )) )) * RougnessA370 ) ):( (( _UseAlbedoA1 )?( ( (( _FalloffOpacity )?( clampResult45 ):( ( 1.0 - lerpResult483 ) )) * (( _InvertAlbedoA1 )?( ( 1.0 - AlbedoA250 ) ):( AlbedoA250 )) ) ):( (( _FalloffOpacity )?( clampResult45 ):( ( 1.0 - lerpResult483 ) )) )) )) , 1.0 , DecalMask182);
			float Opacity87 = lerpResult189;
			float4 lerpResult396 = lerp( ( Cubmap179 + Emission220 + ( GrabSreenRefraction385 * ( 1.0 - Opacity87 ) ) ) , ( Cubmap179 + Emission220 ) , DecalMask182);
			o.Emission = lerpResult396.rgb;
			float lerpResult264 = lerp( ( _Metallic * tex2DNode123.r ) , _MetallicDecal , DecalMask182);
			float Metallic110 = lerpResult264;
			o.Metallic = Metallic110;
			float lerpResult185 = lerp( ( tex2DNode123.a * _Glossiness ) , ( tex2DNode123.a * _GlossinessDecal ) , DecalMask182);
			float Roughness111 = lerpResult185;
			o.Smoothness = Roughness111;
			float2 uv_OcclusionMap = i.uv_texcoord * _OcclusionMap_ST.xy + _OcclusionMap_ST.zw;
			float2 break26_g23 = uv_OcclusionMap;
			float2 appendResult14_g23 = (float2(( break26_g23.x * GlobalTilingX281 ) , ( break26_g23.y * GlobalTilingY282 )));
			float2 appendResult13_g23 = (float2(( break26_g23.x + GlobalOffsetX541 ) , ( break26_g23.y + GlobalOffsetY540 )));
			float blendOpSrc136 = tex2D( _OcclusionMap, ( appendResult14_g23 + appendResult13_g23 ) ).r;
			float blendOpDest136 = ( 1.0 - _AoIntensity );
			float Occlusion140 = ( saturate( ( 1.0 - ( 1.0 - blendOpSrc136 ) * ( 1.0 - blendOpDest136 ) ) ));
			o.Occlusion = Occlusion140;
			float lerpResult513 = lerp( -3.0 , 0.0 , _GradientFade);
			float fresnelNdotV515 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode515 = ( lerpResult513 + _PowerFalloffFade * pow( 1.0 - fresnelNdotV515, (( 1.0 + -( 1.0 - _Fade ) ) + (1.0 - 0.0) * (( 1.0 - _Fade ) - ( 1.0 + -( 1.0 - _Fade ) )) / (1.0 - 0.0)) ) );
			float clampResult518 = clamp( (( _InvertFresnelFade )?( ( 1.0 - fresnelNode515 ) ):( fresnelNode515 )) , 0.0 , 1.0 );
			float lerpResult537 = lerp( (( _FalloffFade )?( clampResult518 ):( ( 1.0 - _Fade ) )) , 1.0 , DecalMask182);
			float Fade450 = (( _FalloffFade1 )?( lerpResult537 ):( (( _FalloffFade )?( clampResult518 ):( ( 1.0 - _Fade ) )) ));
			o.Alpha = Fade450;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
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