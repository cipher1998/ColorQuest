﻿// Toony Colors Pro+Mobile 2
// (c) 2014-2019 Jean Moreno

Shader "Toony Colors Pro 2/Examples URP/Cat Demo/Cat/Style 5"
{
	Properties
	{
		[TCP2HeaderHelp(Base)]
		_BaseColor ("Color", Color) = (1,1,1,1)
		[TCP2ColorNoAlpha] _HColor ("Highlight Color", Color) = (0.75,0.75,0.75,1)
		[TCP2ColorNoAlpha] _SColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
		_MainTex ("Albedo", 2D) = "white" {}
		
		_albedo_sat ("Albedo Saturation", Range(-2,2)) = 1.0
		[TCP2Separator]

		[TCP2Header(Ramp Shading)]
		
		[Header(Main Directional Light)]
		_RampThreshold ("Threshold", Range(0.01,1)) = 0.5
		_RampSmoothing ("Smoothing", Range(0.001,1)) = 1
		[Header(Other Lights)]
		_RampThresholdOtherLights ("Threshold", Range(0.01,1)) = 0.5
		_RampSmoothingOtherLights ("Smoothing", Range(0.001,1)) = 0.5
		[Space]
		[TCP2Separator]
		
		[TCP2HeaderHelp(Specular)]
		[TCP2ColorNoAlpha] _SpecularColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_SpecularSmoothness ("Smoothness", Float) = 0.2
		[TCP2Separator]
		
		[TCP2HeaderHelp(Sketch)]
		_SketchTexture ("Sketch Texture", 2D) = "black" {}
		[TCP2ColorNoAlpha] _SketchColor ("Sketch Color", Color) = (0,0,0,1)
		_SketchMin ("Sketch Min", Range(0,1)) = 0
		_SketchMax ("Sketch Max", Range(0,1)) = 1
		[TCP2Separator]
		
		[TCP2HeaderHelp(Outline)]
		_OutlineWidth ("Width", Range(0.1,4)) = 1
		_OutlineColorVertex ("Color", Color) = (0,0,0,1)
		//This property will be ignored and will draw the custom normals GUI instead
		[TCP2OutlineNormalsGUI] __outline_gui_dummy__ ("_unused_", Float) = 0
		[TCP2Separator]

		[ToggleOff(_RECEIVE_SHADOWS_OFF)] _ReceiveShadowsOff ("Receive Shadows", Float) = 1

		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType"="Opaque"
		}

		CGINCLUDE
		
		//Get screen space UV with object offset taken into account
		inline float2 GetScreenUV(float2 clipPos)
		{
			float4x4 mvpMatrix = mul(unity_MatrixVP, unity_ObjectToWorld);
			float4 screenSpaceObjPos = float4(mvpMatrix[0][3],mvpMatrix[1][3],mvpMatrix[2][3],mvpMatrix[3][3]);
			float2 screenUV = clipPos.xy;
			screenUV.xy -= screenSpaceObjPos.xy / screenSpaceObjPos.ww;
			float ratio = _ScreenParams.x/_ScreenParams.y;
			screenUV.x *= ratio;
			screenUV *= screenSpaceObjPos.w;
			screenUV.x *= sign(UNITY_MATRIX_P[1].y); // 1 for Game View, -1 for Scene View
			return screenUV / UNITY_MATRIX_P._m11; // scale with the Camera FoV
		}
		
		//================================================================
		// HSV HELPERS
		// source: http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
		
		float3 rgb2hsv(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
			float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
		
			float d = q.x - min(q.w, q.y);
			float e = 1.0e-10;
			return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}
		
		float3 hsv2rgb(float3 c)
		{
			c.g = max(c.g, 0.0); //make sure that saturation value is positive
			float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
			float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
			return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
		}
		
		#ifdef UNITY_COLORSPACE_GAMMA
			#define ColorSpaceLuminance half4(0.22, 0.707, 0.071, 0.0) // Legacy: alpha is set to 0.0 to specify gamma mode
		#else // Linear values
			#define ColorSpaceLuminance half4(0.0396819152, 0.458021790, 0.00609653955, 1.0) // Legacy: alpha is set to 1.0 to specify linear mode
		#endif
		
		float3 ApplyHSVGrayscale(float3 color, float s)
		{
			return lerp(dot(color, ColorSpaceLuminance.rgb).xxx, color, s);
		}
		
		float4 ApplyHSVGrayscale(float4 color, float s)
		{
			return float4(lerp(dot(color.rgb, ColorSpaceLuminance.rgb).xxx, color.rgb, s), color.a);
		}
		ENDCG

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		
		//Get screen space UV with object offset taken into account
		inline float2 GetScreenUV(float2 clipPos)
		{
			float4x4 mvpMatrix = mul(unity_MatrixVP, unity_ObjectToWorld);
			float4 screenSpaceObjPos = float4(mvpMatrix[0][3],mvpMatrix[1][3],mvpMatrix[2][3],mvpMatrix[3][3]);
			float2 screenUV = clipPos.xy;
			screenUV.xy -= screenSpaceObjPos.xy / screenSpaceObjPos.ww;
			float ratio = _ScreenParams.x/_ScreenParams.y;
			screenUV.x *= ratio;
			screenUV *= screenSpaceObjPos.w;
			screenUV.x *= sign(UNITY_MATRIX_P[1].y); // 1 for Game View, -1 for Scene View
			return screenUV / UNITY_MATRIX_P._m11; // scale with the Camera FoV
		}
		
		//================================================================
		// HSV HELPERS
		// source: http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
		
		float3 rgb2hsv(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
			float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
		
			float d = q.x - min(q.w, q.y);
			float e = 1.0e-10;
			return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}
		
		float3 hsv2rgb(float3 c)
		{
			c.g = max(c.g, 0.0); //make sure that saturation value is positive
			float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
			float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
			return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
		}
		
		#ifdef UNITY_COLORSPACE_GAMMA
			#define ColorSpaceLuminance half4(0.22, 0.707, 0.071, 0.0) // Legacy: alpha is set to 0.0 to specify gamma mode
		#else // Linear values
			#define ColorSpaceLuminance half4(0.0396819152, 0.458021790, 0.00609653955, 1.0) // Legacy: alpha is set to 1.0 to specify linear mode
		#endif
		
		float3 ApplyHSVGrayscale(float3 color, float s)
		{
			return lerp(dot(color, ColorSpaceLuminance.rgb).xxx, color, s);
		}
		
		float4 ApplyHSVGrayscale(float4 color, float s)
		{
			return float4(lerp(dot(color.rgb, ColorSpaceLuminance.rgb).xxx, color.rgb, s), color.a);
		}
		ENDHLSL
		// Outline Include
		CGINCLUDE

		#include "UnityCG.cginc"
		#include "UnityLightingCommon.cginc"	// needed for LightColor

		struct appdata_outline
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			fixed4 vertexColor : COLOR;
		// TODO: need a way to know if texcoord1 is used in the Shader Properties
		#if TCP2_UV2_AS_NORMALS
			float2 uv2 : TEXCOORD1;
		#endif
		#if TCP2_TANGENT_AS_NORMALS
			float4 tangent : TANGENT;
		#endif
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f_outline
		{
			float4 vertex : SV_POSITION;
			float4 vcolor : TEXCOORD0;
			UNITY_VERTEX_OUTPUT_STEREO
		};

		// Shader Properties
		float _OutlineWidth;
		fixed4 _OutlineColorVertex;

		v2f_outline vertex_outline (appdata_outline v)
		{
			v2f_outline output;
			UNITY_INITIALIZE_OUTPUT(v2f_outline, output);

			// Shader Properties Sampling
			float __outlineWidth = ( _OutlineWidth * v.vertexColor.r );
			float4 __outlineColorVertex = ( _OutlineColorVertex.rgba );

			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

		#ifdef TCP2_COLORS_AS_NORMALS
			//Vertex Color for Normals
			float3 normal = (v.vertexColor.xyz*2) - 1;
		#elif TCP2_TANGENT_AS_NORMALS
			//Tangent for Normals
			float3 normal = v.tangent.xyz;
		#elif TCP2_UV2_AS_NORMALS
			//UV2 for Normals
			float3 n;
			//unpack uv2
			v.uv2.x = v.uv2.x * 255.0/16.0;
			n.x = floor(v.uv2.x) / 15.0;
			n.y = frac(v.uv2.x) * 16.0 / 15.0;
			//get z
			n.z = v.uv2.y;
			//transform
			n = n*2 - 1;
			float3 normal = n;
		#else
			float3 normal = v.normal;
		#endif
			float size = 1;
		
			output.vertex = UnityObjectToClipPos(v.vertex + float4(normal,0) * __outlineWidth * size * 0.01);
			
			output.vcolor.xyzw = __outlineColorVertex;
			return output;
		}

		float4 fragment_outline (v2f_outline input) : SV_Target
		{
			// Shader Properties Sampling
			float4 __outlineColor = ( float4(1,1,1,1) );

			half4 outlineColor = __outlineColor * input.vcolor.xyzw;
			return outlineColor;
		}

		ENDCG
		// Outline Include End
		Pass
		{
			Name "Main"
			Tags { "LightMode"="UniversalForward" }

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard SRP library
			// All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 3.0

			#define fixed half
			#define fixed2 half2
			#define fixed3 half3
			#define fixed4 half4

			// -------------------------------------
			// Material keywords
			//#pragma shader_feature _ALPHATEST_ON
			#pragma shader_feature _ _RECEIVE_SHADOWS_OFF

			// -------------------------------------
			// Universal Render Pipeline keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

			// -------------------------------------

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			#pragma vertex Vertex
			#pragma fragment Fragment

			// Uniforms
			CBUFFER_START(UnityPerMaterial)
			
			// Shader Properties
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			float _albedo_sat;
			fixed4 _BaseColor;
			float _RampThreshold;
			float _RampSmoothing;
			sampler2D _SketchTexture;
			float4 _SketchTexture_ST;
			float _SketchMin;
			float _SketchMax;
			fixed3 _SColor;
			fixed3 _HColor;
			fixed3 _SketchColor;
			float _SpecularSmoothness;
			fixed3 _SpecularColor;
			float _RampThresholdOtherLights;
			float _RampSmoothingOtherLights;
			CBUFFER_END

			// vertex input
			struct Attributes
			{
				float4 vertex       : POSITION;
				float3 normal       : NORMAL;
				float4 tangent      : TANGENT;
				float4 texcoord0 : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			// vertex output / fragment input
			struct Varyings
			{
				float4 positionCS     : SV_POSITION;
				float3 normal         : NORMAL;
				float4 worldPosAndFog : TEXCOORD0;
			#ifdef _MAIN_LIGHT_SHADOWS
				float4 shadowCoord    : TEXCOORD1; // compute shadow coord per-vertex for the main light
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				half3 vertexLights : TEXCOORD2;
			#endif
				float4 clipPosition : TEXCOORD3;
				float2 pack1 : TEXCOORD4; /* pack1.xy = texcoord0 */
			};

			Varyings Vertex(Attributes input)
			{
				Varyings output;

				// Texture Coordinates
				output.pack1.xy.xy = (input.texcoord0.xy) * _MainTex_ST.xy + _MainTex_ST.zw;

				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);
			#ifdef _MAIN_LIGHT_SHADOWS
				output.shadowCoord = GetShadowCoord(vertexInput);
			#endif
				float4 clipPos = vertexInput.positionCS;
				
				//Screen Space UV
				output.clipPosition.xyzw = clipPos;

				VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normal);
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				// Vertex lighting
				output.vertexLights = VertexLighting(vertexInput.positionWS, vertexNormalInput.normalWS);
			#endif

				// world position
				output.worldPosAndFog = float4(vertexInput.positionWS.xyz, 0);

				// normal
				output.normal = NormalizeNormalPerVertex(vertexNormalInput.normalWS);

				// clip position
				output.positionCS = vertexInput.positionCS;

				return output;
			}

			half4 Fragment(Varyings input) : SV_Target
			{
				float3 positionWS = input.worldPosAndFog.xyz;
				float3 normalWS = NormalizeNormalPerPixel(input.normal);
				half3 viewDirWS = SafeNormalize(GetCameraPositionWS() - positionWS);

				//Screen Space UV
				float2 screenUV = GetScreenUV(input.clipPosition.xyzw.xy / input.clipPosition.xyzw.w);
				// Shader Properties Sampling
				float4 __albedo = ( ApplyHSVGrayscale(tex2D(_MainTex, (input.pack1.xy.xy)).rgba, _albedo_sat) );
				float4 __mainColor = ( _BaseColor.rgba );
				float __alpha = ( __albedo.a * __mainColor.a );
				float __rampThreshold = ( _RampThreshold );
				float __rampSmoothing = ( _RampSmoothing );
				float3 __sketchTexture = ( tex2D(_SketchTexture, (screenUV) * _ScreenParams.zw * _SketchTexture_ST.xy + _SketchTexture_ST.zw).aaa );
				float __sketchAntialiasing = ( 20.0 );
				float __sketchThresholdScale = ( 1.0 );
				float __sketchMin = ( _SketchMin );
				float __sketchMax = ( _SketchMax );
				float3 __shadowColor = ( _SColor.rgb );
				float3 __highlightColor = ( _HColor.rgb );
				float3 __sketchColor = ( _SketchColor.rgb );
				float __specularSmoothness = ( _SpecularSmoothness );
				float3 __specularColor = ( _SpecularColor.rgb * __albedo.aaa );
				float __rampThresholdOtherLights = ( _RampThresholdOtherLights );
				float __rampSmoothingOtherLights = ( _RampSmoothingOtherLights );
				float __ambientIntensity = ( 1.0 );

				// main texture
				half3 albedo = __albedo.rgb;
				half alpha = __alpha;
				half3 emission = half3(0,0,0);

				albedo *= __mainColor.rgb;

				// main light: direction, color, distanceAttenuation, shadowAttenuation
			#ifdef _MAIN_LIGHT_SHADOWS
				Light mainLight = GetMainLight(input.shadowCoord);
			#else
				Light mainLight = GetMainLight();
			#endif

				half3 lightDir = mainLight.direction;
				half3 lightColor = mainLight.color.rgb;
				half atten = mainLight.shadowAttenuation;

				half ndl = max(0, dot(normalWS, lightDir));
				half3 ramp;
				half rampThreshold = __rampThreshold;
				half rampSmooth = __rampSmoothing * 0.5;
				ndl = saturate(ndl);
				ramp = smoothstep(rampThreshold - rampSmooth, rampThreshold + rampSmooth, ndl);
				fixed3 rampGrayscale = ramp;

				// apply attenuation
				ramp *= atten;

				half3 sketch = __sketchTexture;
				half sketchThresholdWidth = __sketchAntialiasing * fwidth(ndl);
				sketch = smoothstep(sketch - sketchThresholdWidth, sketch, clamp(saturate(ramp * __sketchThresholdScale), __sketchMin, __sketchMax));

				// highlight/shadow colors
				ramp = lerp(__shadowColor, __highlightColor, ramp);

				// output color
				half3 color = half3(0,0,0);
				color += albedo * lightColor.rgb * ramp;
				color.rgb *= lerp(__sketchColor, half3(1,1,1), sketch);

				//Blinn-Phong Specular
				half3 h = normalize(lightDir + viewDirWS);
				float ndh = max(0, dot (normalWS, h));
				float spec = pow(ndh, __specularSmoothness * 128.0);
				spec *= ndl;
				spec *= atten;
				
				//Apply specular
				color.rgb += spec * lightColor.rgb * __specularColor;

				// Additional lights loop
			#ifdef _ADDITIONAL_LIGHTS
				int additionalLightsCount = GetAdditionalLightsCount();
				for (int i = 0; i < additionalLightsCount; ++i)
				{
					Light light = GetAdditionalLight(i, positionWS);
					half atten = light.shadowAttenuation * light.distanceAttenuation;
					half3 lightDir = light.direction;
					half3 lightColor = light.color.rgb;

					half ndl = max(0, dot(normalWS, lightDir));
					half3 ramp;
					half rampThreshold = __rampThresholdOtherLights;
					half rampSmooth = __rampSmoothingOtherLights * 0.5;
					ndl = saturate(ndl);
					ramp = smoothstep(rampThreshold - rampSmooth, rampThreshold + rampSmooth, ndl);

					// apply attenuation (shadowmaps & point/spot lights attenuation)
					ramp *= atten;

					// apply highlight color
					ramp = lerp(half3(0,0,0), __highlightColor, ramp);

					// output color
					color += albedo * lightColor.rgb * ramp;

					//Blinn-Phong Specular
					half3 h = normalize(lightDir + viewDirWS);
					float ndh = max(0, dot (normalWS, h));
					float spec = pow(ndh, __specularSmoothness * 128.0);
					spec *= ndl;
					spec *= atten;
					
					//Apply specular
					color.rgb += spec * lightColor.rgb * __specularColor;
				}
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				color += input.vertexLights * albedo;
			#endif

				// ambient or lightmap
				// Samples SH fully per-pixel. SampleSHVertex and SampleSHPixel functions
				// are also defined in case you want to sample some terms per-vertex.
				half3 bakedGI = SampleSH(normalWS);
				half occlusion = 1;
				half3 indirectDiffuse = bakedGI;
				indirectDiffuse *= occlusion * albedo * __ambientIntensity;
				color += indirectDiffuse;

				color += emission;

				return half4(color, alpha);
			}
			ENDHLSL
		}

		//Outline
		Pass
		{
			Name "Outline"
			Cull Front

			CGPROGRAM
			#pragma vertex vertex_outline
			#pragma fragment fragment_outline
			#pragma target 3.0
			#pragma multi_compile TCP2_NONE TCP2_COLORS_AS_NORMALS TCP2_TANGENT_AS_NORMALS TCP2_UV2_AS_NORMALS
			#pragma multi_compile_instancing
			ENDCG
		}

		// Depth & Shadow Caster Passes
		HLSLINCLUDE
		#if defined(SHADOW_CASTER_PASS) || defined(DEPTH_ONLY_PASS)

			#define fixed half
			#define fixed2 half2
			#define fixed3 half3
			#define fixed4 half4

			float3 _LightDirection;

			CBUFFER_START(UnityPerMaterial)
			
			// Shader Properties
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			float _albedo_sat;
			fixed4 _BaseColor;
			float _RampThreshold;
			float _RampSmoothing;
			sampler2D _SketchTexture;
			float4 _SketchTexture_ST;
			float _SketchMin;
			float _SketchMax;
			fixed3 _SColor;
			fixed3 _HColor;
			fixed3 _SketchColor;
			float _SpecularSmoothness;
			fixed3 _SpecularColor;
			float _RampThresholdOtherLights;
			float _RampSmoothingOtherLights;
			CBUFFER_END

			struct Attributes
			{
				float4 positionOS   : POSITION;
				float3 normalOS     : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS     : SV_POSITION;
				float4 clipPosition : TEXCOORD0;
				float2 pack1 : TEXCOORD1; /* pack1.xy = texcoord0 */
			#if defined(DEPTH_ONLY_PASS)
				UNITY_VERTEX_OUTPUT_STEREO
			#endif
			};

			float4 GetShadowPositionHClip(Attributes input)
			{
				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

			#if UNITY_REVERSED_Z
				positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
			#else
				positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
			#endif

				return positionCS;
			}

			Varyings ShadowDepthPassVertex(Attributes input)
			{
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
			#if defined(DEPTH_ONLY_PASS)
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
			#endif
				// Texture Coordinates
				output.pack1.xy.xy = (input.texcoord0.xy) * _MainTex_ST.xy + _MainTex_ST.zw;

				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				float4 clipPos = TransformWorldToHClip(positionWS);
				
				//Screen Space UV
				output.clipPosition.xyzw = clipPos;

			#if defined(DEPTH_ONLY_PASS)
				output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
			#elif defined(SHADOW_CASTER_PASS)
				output.positionCS = GetShadowPositionHClip(input);
			#else
				output.positionCS = float4(0,0,0,0);
			#endif

				return output;
			}

			half4 ShadowDepthPassFragment(Varyings input) : SV_TARGET
			{
				//Screen Space UV
				float2 screenUV = GetScreenUV(input.clipPosition.xyzw.xy / input.clipPosition.xyzw.w);
				// Shader Properties Sampling
				float4 __albedo = ( ApplyHSVGrayscale(tex2D(_MainTex, (input.pack1.xy.xy)).rgba, _albedo_sat) );
				float4 __mainColor = ( _BaseColor.rgba );
				float __alpha = ( __albedo.a * __mainColor.a );
				float __rampThreshold = ( _RampThreshold );
				float __rampSmoothing = ( _RampSmoothing );
				float3 __sketchTexture = ( tex2D(_SketchTexture, (screenUV) * _ScreenParams.zw * _SketchTexture_ST.xy + _SketchTexture_ST.zw).aaa );
				float __sketchAntialiasing = ( 20.0 );
				float __sketchThresholdScale = ( 1.0 );
				float __sketchMin = ( _SketchMin );
				float __sketchMax = ( _SketchMax );
				float3 __shadowColor = ( _SColor.rgb );
				float3 __highlightColor = ( _HColor.rgb );
				float3 __sketchColor = ( _SketchColor.rgb );
				float __specularSmoothness = ( _SpecularSmoothness );
				float3 __specularColor = ( _SpecularColor.rgb * __albedo.aaa );
				float __rampThresholdOtherLights = ( _RampThresholdOtherLights );
				float __rampSmoothingOtherLights = ( _RampSmoothingOtherLights );
				float __ambientIntensity = ( 1.0 );

				half3 albedo = __albedo.rgb;
				half alpha = __alpha;
				half3 emission = half3(0,0,0);
				return 0;
			}

		#endif
		ENDHLSL

		Pass
		{
			Name "ShadowCaster"
			Tags{"LightMode" = "ShadowCaster"}

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0

			// using simple #define doesn't work, we have to use this instead
			#pragma multi_compile SHADOW_CASTER_PASS

			// -------------------------------------
			// Material Keywords
			//#pragma shader_feature _ALPHATEST_ON
			//#pragma shader_feature _GLOSSINESS_FROM_BASE_ALPHA

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			#pragma vertex ShadowDepthPassVertex
			#pragma fragment ShadowDepthPassFragment

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

			ENDHLSL
		}

		Pass
		{
			Name "DepthOnly"
			Tags{"LightMode" = "DepthOnly"}

			ZWrite On
			ColorMask 0

			HLSLPROGRAM

			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0

			// -------------------------------------
			// Material Keywords
			// #pragma shader_feature _ALPHATEST_ON
			// #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			// using simple #define doesn't work, we have to use this instead
			#pragma multi_compile DEPTH_ONLY_PASS

			#pragma vertex ShadowDepthPassVertex
			#pragma fragment ShadowDepthPassFragment

			ENDHLSL
		}

		// Depth prepass
		// UsePass "Universal Render Pipeline/Lit/DepthOnly"

		// Used for Baking GI. This pass is stripped from build.
		UsePass "Universal Render Pipeline/Lit/Meta"
	}

	FallBack "Hidden/InternalErrorShader"
	CustomEditor "ToonyColorsPro.ShaderGenerator.MaterialInspector_SG2"
}

/* TCP_DATA u config(ver:"2.5.0";tmplt:"SG2_Template_URP";features:list["UNITY_5_4","UNITY_5_5","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","UNITY_2019_1","SPEC_LEGACY","SPECULAR","RAMP_MAIN_OTHER","RAMP_SEPARATED","OUTLINE","SKETCH_GRADIENT","SHADOW_COLOR_MAIN_DIR","TEMPLATE_URP"];flags:list[];keywords:dict[RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.0",RIM_LABEL="Rim Outline",RENDER_TYPE="Opaque"];shaderProperties:list[sp(name:"Albedo";imps:list[imp_mp_texture(guid:"1e351ce4-cbd8-4482-a69f-efd0b9f6ef3a";uto:True;tov:"";gto:True;sbt:False;scr:False;scv:"";gsc:False;roff:False;goff:False;notile:False;def:"white";locked_uv:False;uv:0;cc:4;chan:"RGBA";mip:-1;mipprop:False;ssuv:False;ssuv_vert:False;ssuv_obj:False;prop:"_MainTex";md:"";custom:False;refs:"";op:Multiply;lbl:"Albedo";gpu_inst:False;locked:False;impl_index:-1),imp_hsv(type:SaturationOffset;chue:False;csat:False;cval:False;op:Multiply;lbl:"Albedo";gpu_inst:False;locked:False;impl_index:-1)]),,,,,,,,,,sp(name:"Specular Color";imps:list[imp_mp_color(def:RGBA(0.500, 0.500, 0.500, 1.000);hdr:False;cc:3;chan:"RGB";prop:"_SpecularColor";md:"";custom:False;refs:"";op:Multiply;lbl:"Specular Color";gpu_inst:False;locked:False;impl_index:-1),imp_spref(cc:3;chan:"AAA";lsp:"Albedo";op:Multiply;lbl:"Specular Color";gpu_inst:False;locked:False;impl_index:-1)]),,sp(name:"Sketch Texture";imps:list[imp_mp_texture(guid:"1c042d40-947f-4133-aad2-5d00caa86ce4";uto:True;tov:"";gto:False;sbt:False;scr:False;scv:"";gsc:False;roff:False;goff:False;notile:False;def:"black";locked_uv:False;uv:4;cc:3;chan:"AAA";mip:-1;mipprop:False;ssuv:True;ssuv_vert:False;ssuv_obj:True;prop:"_SketchTexture";md:"";custom:False;refs:"";op:Multiply;lbl:"Sketch Texture";gpu_inst:False;locked:False;impl_index:-1)]),sp(name:"Sketch Color";imps:list[imp_mp_color(def:RGBA(0.000, 0.000, 0.000, 1.000);hdr:False;cc:3;chan:"RGB";prop:"_SketchColor";md:"";custom:False;refs:"";op:Multiply;lbl:"Sketch Color";gpu_inst:False;locked:False;impl_index:-1)]),,,,,sp(name:"Outline Width";imps:list[imp_mp_range(def:1;min:0.1;max:4;prop:"_OutlineWidth";md:"";custom:False;refs:"";op:Multiply;lbl:"Width";gpu_inst:False;locked:False;impl_index:-1),imp_vcolors(cc:1;chan:"R";op:Multiply;lbl:"Outline Width";gpu_inst:False;locked:False;impl_index:-1)])];customTextures:list[]) */
/* TCP_HASH 85d554a64b6f3e4927ce52f75fa04428 */
