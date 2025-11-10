Shader "Juan/ShaderGUI Demo/Folder Demo"
{
    Properties
    {
        [Folder(Simple Folder)]

        [Texture(_Color)]_MainTex ("Albedo (RGB)", 2D) = "white" {}
        [HideInInspector]_Color ("Color", Color) = (1,1,1,1)

        [Folder(Nested Folder Style)]
        [Close(Nested Folder Style)]

        [Close(Simple Folder)]

        [Folder(Always Open Folder, True)]
        [Toggle]_ShowFolder1("Show Folder 1", Float) = 1.0
        [Toggle(_SHOW_BLOCK_TWO)]_HideFolder2("Hide Folder 2", Float) = 1.0 
        [Toggle(_SHOW_BLOCK_THREE)]_OpenFolder3("Show Folder 3", Float) = 1.0 
        [Toggle(_SHOW_BLOCK_FOUR)]_HideFolder4("Hide Folder 4", Float) = 1.0 
        [Texture]_AssignTexToShowFolder5("Assign to Show Folder 5", 2D) = "white"{}
        _MakeFloat3ToShowFolder6("Make This Float 3.0 to Show Folder 6", Float) = 1.0
        [Close]

        [Folder(Condition Folder 1, _ShowFolder1, 1.0)]
        [Close]

        [Folder(Condition Folder 2, _HideFolder2, OFF)]
        [Close]

        [Folder(Condition Folder 3, _SHOW_BLOCK_THREE)]
        [Close]

        [Folder(Condition Folder 4, _SHOW_BLOCK_FOUR, OFF)]
        [Close]

        [Folder(Condition Folder 5, _AssignTexToShowFolder5)]
        [Close]

        [Folder(Condition Folder 6, _MakeFloat3ToShowFolder6, 3.0)]
        [Close]
        
        
        _Float("Float", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags{"RenderPipeline" = "UniversalPipeline"}

         Pass{
            Name "UnLit"
            Tags{}
            
            
            HLSLPROGRAM

            #pragma vertex LitPassVertexBasic
            #pragma fragment LitPassFragmentBasic

            #pragma shader_feature_local _SHOW_BLOCK_TWO
            #pragma shader_feature_local _SHOW_BLOCK_THREE
            #pragma shader_feature_local _SHOW_BLOCK_FOUR
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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
                output.normalWS = normalize(normalInput.normalWS);
                output.positionWS = vertexInput.positionWS;
                
                return output;
            }

            float4 LitPassFragmentBasic(Varying input): SV_TARGET
            {
                float4 c = _Color;
                
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
