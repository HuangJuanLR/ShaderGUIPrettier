Shader "Juan/ShaderGUI Demo/Performance Demo"
{
    Properties
    {
        // #########################################
        // Begin Surface Options Folder
        // #########################################
        
        [Folder(Surface Options)]
        _RegularFloat("Regular Float", Range(0, 1)) = 0.0
        _RegularColor("Regular Color", Color) = (1,1,1,1)
        [Close]  
        
        // ----------------------------
        // End Surface Options Folder
        // ----------------------------
        
        // #########################################
        // Begin Text Folder
        // #########################################
        
        [Folder(Text Field Drawers)]
        [Text(Text Folder #n Label, Label)]
        [Text(Text Folder #n Info, Info)]
        [Text(Text Folder #n Warning, Warning)]
        [Text(Text Folder #n Error, Error)]
        [Close]
        
        // ----------------------------
        // End Text Folder
        // ----------------------------
        
        [Text(Standalone Label, label)]
        
        // #########################################
        // Begin Always Open Folder
        // #########################################

        [Folder(Always Open Folder, True)]
        [Toggle]_ShowFolder1("Show Folder 1", Float) = 1.0
        [Toggle(_SHOW_BLOCK_TWO)]_OpenFolder2("Show Folder 2", Float) = 1.0 
        [Folder(Folder In Always Open Folder)]
        [Folder(Nested Always Open Folder, True)]
        [Close]
        [Close]
        [Close]
        
        // ----------------------------
        // End Always Open Folder
        // ----------------------------

        [Folder(Conditional Folder 1, _ShowFolder1, 1.0)]
        [Close]

        [Folder(Conditional Folder 2, _SHOW_BLOCK_TWO, ON)]
        [Close]
        
        // #########################################
        // Begin Standard Folders
        // #########################################
        
        [Folder(Standard Folders)]

        [Folder(Folder)]
        _Folder_Color("Color", Color) = (1,1,1,1)
        [Folder(Nested Folder)]
        [Folder(Nested Folder Inside)]
        [Close]
        [Close]
        [Close]

        [Toggle(_SHADER_FEATURE_ONE)]_ShaderFeature1("Shader Feature 1", Float) = 0.0
        [ConditionFolder(ShaderFeature1, 1.0)]
        _Color2("Color2", Color) = (1,1,1,1)
        [Close]

        [Toggle(_SHADER_FEATURE_TWO)]_ShaderFeature2("Shader Feature 2", Float) = 0.0
        [ConditionFolder(SHADER_FEATURE_TWO, ON)]
        _Color3("Color3", Color) = (1,1,1,1)
        [Close]

        [FeatureFolder(Emission, _EMISSION)] 
        
        [Texture(_EmissionColor, HDR)]_EmissionMap("Emission", 2D) = "white" {}
        [hIdeiNInsPeCtoR]_EmissionColor("Color", Color) = (0,0,0)
        
        [QuickSlider(0.0, 5.0)]_EmissionIntensity("Emission Intensity", Float) = 1.0
        [Close]

        [Enum(Enum1, 0.0, Enum2, 1.0, Enum3, 2.0)]_ShaderMode1("Shader Mode 1", Float) = 0.0
        
        [ConditionFolder(_ShaderMode1, 0.0)]
        [tEXt(Shader Mode 1 Enum1, Label)]
        [Close]
        
        [ConditionFolder(_ShaderMode1, 1.0)]
        _ShaderMode1Enum2("Shader Mode 1 Enum 2", Range(0.0, 2.0)) = 1.0
        [Close]
        
        [Toggle(_ALPHATEST_ON)]_AlphaClip ("Alpha Clipping", Float) = 0.0

        [ConditionFolder(AlphaClip, 1.0)] 
        _Cutoff("Threshold", Range(0.0, 1.0)) = 0.5
        [Toggle(_SHADOW_CUTOFF)]_UseShadowThreshold("Use Shadow Threshold", Float) = 0.0
        [ConditionFolder(_SHADOW_CUTOFF, ON)]
        _AlphaCutoffShadow("Shadow Threshold", Range(0.0, 1.0)) = 0.5
        [Close]
        [Toggle]_AlphaToMask("Alpha To Mask", Float) = 0.0
        [Close]
        
        [Close]
        
        // ----------------------------
        // End Standard Folders
        // ----------------------------
        
        // #########################################
        // Begin Feature Folder
        // #########################################
        
        [FeatureFolder(Shader Feature 3, _SHADER_FEATURE_THREE)]
        [Close] 
        
        // ----------------------------
        // End Feature Folder
        // ----------------------------
        
        // #########################################
        // Begin Standard Drawer Folder
        // #########################################
        
        [Folder(Standard Drawer)]
        [Vector4]_Vector4("Vector4", Vector) = (1,0,0,0)
        [Vector3]_Vector3("Vector3", Vector) = (1,0,0,0)
        [Vector2]_Vector2("Vector2", Vector) = (1,0,0,0)
        
        [Texture(_Color)]_MainTex ("Albedo (RGB)", 2D) = "white" {}
        [HideInInspector]_Color ("Color", Color) = (1,1,1,1)
        
        [Texture(_BumpScale, _NORMALMAP)]_BumpMap("Normal Map", 2D) = "bump"{}
        [hIdeiNInsPeCtoR]_BumpScale ("Normal Map Scale", Range(0.0, 10.0)) = 1.0
        
        [Texture(_MASKMAP)]_MaskMap("Mask MADS", 2D) = "white"{}
        
        [ConditionFolder(_MASKMAP, ON)] 

        [Remapping]_MetallicRemap ("Metallic Remapping", Float) = 1.0
        [hIdeiNInsPeCtoR]_MetallicRemapMax ("Metallic Remap Max", Float) = 1.0
        [hIdeiNInsPeCtoR]_MetallicRemapMin ("Metallic Remap Min", Float) = 0.0

        [Remapping]_SmoothnessRemap ("Smoothness Remapping", Float) = 1.0
        [hIdeiNInsPeCtoR]_SmoothnessRemapMax ("Smoothness Remap Max", Float) = 1.0
        [hIdeiNInsPeCtoR]_SmoothnessRemapMin ("Smoothness Remap Min", Float) = 0.0

        [Remapping]_AORemap ("AO Remapping", Float) = 1.0
        [hIdeiNInsPeCtoR]_AORemapMax ("AO Remap Max", Float) = 1.0
        [hIdeiNInsPeCtoR]_AORemapMin ("AO Remap Min", Float) = 0.0  
        
        [Close]

        [ConditionFolder(MASKMAP, OFF)]
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [Close]
        
        [ScaleOffset(_MainTex)]_MainTexScaleOffset("Tiling & Offset", Vector) = (1,1,0,0)
        
        [Remapping]_FloatRemap ("Float Remapping", Float) = 1.0
        [HideInInspector]_FloatRemapMax ("Float Remap Max",Float) = 1.0
        [hIdeiNInsPeCtoR]_FloatRemapMin ("Float Remap Min",Float) = 0.0
        
        [Texture]_CommonTex("Common Texture", 2D) = "white"{}
        
        [hIdeiNInsPeCtoR]_FloatMin("QuickSlider Float Min", Float) = 1.0
        [hIdeiNInsPeCtoR]_FloatMax("QuickSlider Float Max", Float) = 2.0
        [QuickSlider(0.0, 1.0)]_FloatMinMax("Float Min Max", Float) = 1.0
        
        [Close]
        
        // ----------------------------
        // End Standard Drawer Folder
        // ----------------------------
        
        // #########################################
        // Begin Dedicated Drawer Folder
        // #########################################

        [Folder(Dedicated Drawer)]

        [SurfaceType]_Surface ("Surface Type", Float) = 0.0
        
        [CullMode]_Cull ("Render Face", Float) = 2.0

        [ConditionFolder(Cull, 0.0)] 
        [DoubleSidedNormalMode]_DoubleSidedNormalMode("Double-Sided Normal Mode", Float) = 1.0
        [Close] 

        [SpecularOcclusionMode] _SpecularOcclusionMode("Specular Occlusion Mode", Int) = 0
        [ConditionFolder(SpecularOcclusionMode, 3)]
        _GIOcclusionBias("GI Occlusion Bias", Range(0.0, 1.0)) = 0.0
        [Close]

        [QueueOffset]_QueueOffset ("Sorting Priority", Float) = 0.0
        
        // ----------------------------
        // End Special Drawer Folder
        // ----------------------------
    }
    SubShader
    {
        Tags{"RenderPipeline" = "UniversalPipeline"}

        Pass{
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}
            
            
            HLSLPROGRAM

            #pragma shader_feature_local_fragment _SHADER_FEATURE_ONE
            #pragma shader_feature_local_fragment _SHADER_FEATURE_TWO
            #pragma shader_feature_local_fragment _SHADER_FEATURE_THREE
            #pragma shader_feature_local_fragment _SHOW_BLOCK_TWO
            #pragma shader_feature_local_fragment _SHADOW_CUTOFF
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _MASKMAP

            #pragma shader_feature_local_fragment _ALPHATEST_ON

            // GPU Instancing
            // Instancing Field only shows up when these directive are defined
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex LitPassVertexBasic
            #pragma fragment LitPassFragmentBasic
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_EmissionMap); SAMPLER(sampler_EmissionMap);
            float4 _MainTex_ST;

            struct Input
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv0 : TEXCOORD0;
            };

            struct Varying
            {
                float4 positionCS: SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
            };
            
            float4 _Color;
            float4 _Color2;
            float4 _Color3;
            float4 _EmissionColor;
            float _EmissionIntensity;

            Varying LitPassVertexBasic(Input input)
            {
                Varying output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);

                output.positionCS = vertexInput.positionCS;
                output.uv0 = TRANSFORM_TEX(input.uv0, _MainTex);
                output.normalWS = normalize(normalInput.normalWS);
                output.positionWS = vertexInput.positionWS;

                return output;
            }

            float4 LitPassFragmentBasic(Varying input): SV_TARGET
            {
                // Albedo comes from a texture tinted by color
                float4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv0) * _Color;
                
                #ifdef _SHADER_FEATURE_ONE
                    c = _Color2;
                #elif defined(_SHADER_FEATURE_TWO)
                    c = _Color3;
                #endif

                #ifdef _EMISSION
                    float4 emissionMap = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv0);
                    c += _EmissionColor * _EmissionIntensity * emissionMap;
                #endif
                
                return c;
            }
            ENDHLSL

        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            HLSLPROGRAM

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 position     : POSITION;
                float2 texcoord     : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionCS   : SV_POSITION;
            };

            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = TransformObjectToHClip(input.position.xyz);
                return output;
            }

            half4 DepthOnlyFragment(Varyings input) : SV_TARGET
            {
                return 0;
            }

            ENDHLSL
        }

        Pass
        {
            Name "DepthNormalsOnly"
            Tags{"LightMode" = "DepthNormalsOnly"}

            HLSLPROGRAM

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 normal       : NORMAL;
                float4 positionOS   : POSITION;
                float4 tangentOS    : TANGENT;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float3 normalWS     : TEXCOORD1;
            };

            Varyings DepthNormalsVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            float4 DepthNormalsFragment(Varyings input) : SV_TARGET
            {
                return 0;
            }

            ENDHLSL
        }
    }
    CustomEditor "ShaderGUIPrettier"
}
