Shader "Juan/ShaderGUI Demo/ConditionFolder Demo"
{
    Properties
    {
        [Folder(Condition, True)]
        [Toggle]_Boolean("Show Folder 1 with Toggle", Float) = 0.0
        [Toggle(_SHADER_FEATURE_TWO)]_HideFolder2("Hide Folder 2 with KEYWORD", Float) = 0.0
        [Texture]_AssignToShowFolder3("Hide Folder 3 with Texture", 2D) = "white"{}
        [Close]

        [ConditionFolder(_Boolean)]
        _Color1("Color1", Color) = (1,1,1,1)
        [Close]

        [ConditionFolder(_SHADER_FEATURE_TWO, OFF)]
        _Color2("Color2", Color) = (1,1,1,1)
        [Close]

        [ConditionFolder(_AssignToShowFolder3, OFF)]
        _Color3("Color3", Color) = (1,1,1,1)
        [Close]

        _Float("Float", Range(0, 1)) = 0
        
    }
    SubShader
    {
        Tags{"RenderPipeline" = "UniversalPipeline"}

        Pass{
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}
            
            
            HLSLPROGRAM
            
            #pragma shader_feature_local_fragment _SHADER_FEATURE_TWO

            // GPU Instancing
            // Instancing Field only shows up when these directive are defined
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex LitPassVertexBasic
            #pragma fragment LitPassFragmentBasic
            
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
            
            float4 _Color1;
            float4 _Color2;

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
                float4 c = _Color1;
                
                #if defined(_SHADER_FEATURE_TWO)
                    c = _Color2;
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
