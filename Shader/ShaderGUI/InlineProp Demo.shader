Shader "Juan/ShaderGUI Demo/InlineProp Demo"
{
    Properties
    {
        [Folder(Inline Properties, True)]
        [Vector4]_Vector4("Vector4", Vector) = (1,0,0,0)
        [Vector3]_Vector3("Vector3", Vector) = (1,0,0,0)
        [Vector2]_Vector2("Vector2", Vector) = (1,0,0,0)
        
        [Texture]_CommonTex("Common Texture", 2D) = "white"{}

        [Texture(_Color)]_MainTex ("Albedo (RGB)", 2D) = "white" {}
        [HideInInspector]_Color ("Color", Color) = (1,1,1,1)

        [Texture(#_HDRColor, HDR)]_TexWithHDR ("Texture with HDR", 2D) = "white" {}
        [HideInInspector]_HDRColor ("HDR Color", Color) = (1,1,1,1)

        [Texture(_FloatAfterTex)]_TexWithFloat("Texture with Float", 2D) = "white"{}
        [HideInInspector]_FloatAfterTex("Float After Texture", Range(0, 1)) = 0.0

        [Texture(#_VectorAfterTex)]_TexWithVector("Texture with Vector", 2D) = "white"{}
        [HideInInspector]_VectorAfterTex("Vector After Texture", Vector) = (0,0,0,0)

        [Texture(_SHADER_FEATURE)]_TexWithKeyword("Texture with Keyword", 2D) = "white"{}
        
        [Remapping]_FloatRemap ("Float Remapping", Float) = 1.0
        [HideInInspector]_FloatRemapMax ("Float Remap Max",Float) = 5.0
        [HideInInspector]_FloatRemapMin ("Float Remap Min",Float) = 0.0

        [Remapping]_VectorRemap("Vector Remapping", Vector) = (0,2,0,0)
        
        [QuickSlider(0.0, 5.0)]_QuickSlider("Quick Slider", Float) = 1.0
        [QuickSlider]_QuickSliderDefault("Quick Slider Default", Float) = 1.0
        [QuickSlider(0, 5, Int)]_QuickSliderInt("Quick Slider Int", Float) = 1.0
    }
    SubShader
    {
        Tags{"RenderPipeline" = "UniversalPipeline"}

        Pass{
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}
            
            
            HLSLPROGRAM

            #pragma shader_feature_local _SHADER_FEATURE
            
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
                float4 c = _Color;

                #ifdef _SHADER_FEATURE
                    c = float4(0,0,0,0);
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
