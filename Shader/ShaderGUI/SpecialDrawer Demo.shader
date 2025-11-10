Shader "Juan/ShaderGUI Demo/SpecialDrawer Demo"
{
    Properties
    {   
        [Texture(_Color)]_MainTex ("Albedo (RGB)", 2D) = "white" {}
        [HideInInspector]_Color ("Color", Color) = (1,1,1,1)
        
        [Text(Dedicated Drawers that we normally wont use for other properties #n I make these just for my own Uber Shader, Warning)]

        [Folder(Dedicated Drawer)]
        
        [SurfaceType]_Surface ("Surface Type", Float) = 0.0

        [CullMode]_Cull ("Render Face", Float) = 2.0

        [ConditionFolder(_Cull, 0.0)] 
        [DoubleSidedNormalMode]_DoubleSidedNormalMode("Double-Sided Normal Mode", Float) = 1.0
        [Close] 

        [SpecularOcclusionMode] _SpecularOcclusionMode("Specular Occlusion Mode", Int) = 0
        [ConditionFolder(_SpecularOcclusionMode, 3)]
        _GIOcclusionBias("GI Occlusion Bias", Range(0.0, 1.0)) = 0.0
        [Close]

        [QueueOffset]_QueueOffset ("Sorting Priority", Float) = 0.0
        
        [ScaleOffset(_MainTex)]_MainTexScaleOffset("Tiling & Offset", Vector) = (1,1,0,0)

    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "Lit"
            "IgnoreProjector" = "True"
        }
        LOD 300

        Pass{
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            
            #pragma vertex LitPassVertexBasic
            #pragma fragment LitPassFragmentBasic

            #pragma shader_feature_local _USE_MASKMAP
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
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
                float4 c = float4(0.5,0.5,0.5,1);

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
