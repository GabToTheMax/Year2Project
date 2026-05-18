Shader "Basics2/ClippablePortalShader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _BaseTexture("Base Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        
        // Regular pass
        Pass
        {
            Tags
            {
                "LightMode" = "SRPDefaultUnlit"
            }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // depth texture library
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            float4 _BaseTexture_ST;
            vector _Portal1PlaneNormal;
            vector _Portal1PlanePoint;
            vector _Portal2PlaneNormal;
            vector _Portal2PlanePoint;
            CBUFFER_END
            
            uniform float _CurrentCameraRendering;
            
            TEXTURE2D(_BaseTexture);
            SAMPLER(sampler_BaseTexture);
            
            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            v2f vert(appdata v)
            {
                v2f o = (v2f)0;
                
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _BaseTexture);
                
                return o;
            }
            
            float4 frag(v2f i) : SV_TARGET
            {
                float2 UV = i.positionCS.xy / _ScaledScreenParams.xy;
                
                #if UNITY_REVERSED_Z
                    real depth = SampleSceneDepth(UV);
                #else
                    // Adjust z to match NDC for OpenGL
                    real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(UV));
                #endif

                float3 worldPos = ComputeWorldSpacePosition(UV, depth, UNITY_MATRIX_I_VP);
                
                if  (depth == -1)
                {
                    discard;
                }
                else if ( _CurrentCameraRendering == 1 )
                {
                    if (dot(_Portal1PlaneNormal ,worldPos-_Portal1PlanePoint) > 0)
                    {
                        discard;
                    }
                }
                else if ( _CurrentCameraRendering == 2 )
                {
                    if (dot(_Portal2PlaneNormal ,worldPos-_Portal2PlanePoint) > 0)
                    {
                        discard;
                    }
                }
                float4 textureColor = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, i.uv);
                return textureColor * _BaseColor;
            }
            
            ENDHLSL
        }


        // Depth pass
        Pass
        {
            Tags
            {
                "LightMode" = "DepthOnly"
            }
            
            ZWrite On
            ColorMask R
            HLSLPROGRAM
            
            #pragma vertex depthOnlyVert
            #pragma fragment depthOnlyFrag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
            };
            
            struct v2f
            {
                float4 positionCS : SV_POSITION;
            };
            
            v2f depthOnlyVert(appdata v)
            {
                v2f o = (v2f)0;
                
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                
                return o;
            }
            
            float depthOnlyFrag(v2f i) : SV_TARGET
            {
                return i.positionCS.z;
            }
            
            ENDHLSL
        }

        // Depth Normal pass
        Pass
        {
            Tags
            {
                "LightMode" = "DepthNormals"
            }
            
            ZWrite On
            
            HLSLPROGRAM
            
            #pragma vertex depthNormalsVert
            #pragma fragment depthNormalsFrag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            vector _Portal1PlaneNormal;
            vector _Portal1PlanePoint;
            vector _Portal2PlaneNormal;
            vector _Portal2PlanePoint;
            uniform float _CurrentCameraRendering;

            struct appdata
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
            };
            
            v2f depthNormalsVert(appdata v)
            {
                v2f o = (v2f)0;
                
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.normalWS = NormalizeNormalPerVertex(normalWS);
                
                return o;
            }
            
            float depthNormalsFrag(v2f i) : SV_TARGET
            {
                float2 UV = i.positionCS.xy / _ScaledScreenParams.xy;
                
                #if UNITY_REVERSED_Z
                    real depth = SampleSceneDepth(UV);
                #else
                    // Adjust z to match NDC for OpenGL
                    real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(UV));
                #endif

                float3 worldPos = ComputeWorldSpacePosition(UV, depth, UNITY_MATRIX_I_VP);
                
                if ( _CurrentCameraRendering == 1 )
                {
                    if (dot(_Portal1PlaneNormal ,worldPos-_Portal1PlanePoint) > 0)
                    {
                        return -1;
                    }
                }
                else if ( _CurrentCameraRendering == 2 )
                {
                    if (dot(_Portal2PlaneNormal ,worldPos-_Portal2PlanePoint) > 0)
                    {
                        return -1;
                    }
                }
                
                float3 normalsWS = NormalizeNormalPerPixel(i.normalWS);
                return float4(normalsWS, 0.0f);
            }
            
            ENDHLSL
        }
    }
}