Shader "Juan/ShaderGUI Demo/ColorRamp Demo"
{
    Properties
    {
        [ColorRamp]_Ramp("Color Ramp", Float) = 0
        [HideInInspector]_RampColor0("", Color) = (0,0,0,0)
        [HideInInspector]_RampColor1("", Color) = (1,1,1,1)
        [HideInInspector]_RampColor2("", Color) = (1,1,1,1)
        [HideInInspector]_RampColor3("", Color) = (1,1,1,1)
        [HideInInspector]_RampColor4("", Color) = (1,1,1,1)
        [HideInInspector]_RampColor5("", Color) = (1,1,1,1)
        [HideInInspector]_RampColor6("", Color) = (1,1,1,1)
        [HideInInspector]_RampColor7("", Color) = (1,1,1,1)
        [HideInInspector]_RampAlpha0("", Vector) = (1,0,1,1)
        [HideInInspector]_RampAlpha2("", Vector) = (1,1,1,1)
        [HideInInspector]_RampAlpha4("", Vector) = (1,1,1,1)
        [HideInInspector]_RampAlpha6("", Vector) = (1,1,1,1)
        [HideInInspector]_RampColorCount("", Int) = 2
        [HideInInspector]_RampAlphaCount("", Int) = 2
    }
    SubShader
    {
        Tags{"RenderPipeline" = "UniversalPipeline"}

        Pass{
            Name"ForwardLit"
            Tags{"LightMode" = "UniversalForward"}
            
            HLSLPROGRAM

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
                return float4(0.8,0.8,0.8,1);
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
