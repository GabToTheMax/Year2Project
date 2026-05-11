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
            
            // from: https://discussions.unity.com/t/converting-a-clip-space-point-to-world-space/930106/2
            float3 ClipToWorldPos(float4 clipPos)
            {
                #ifdef UNITY_REVERSED_Z
                    // unity_CameraInvProjection always in OpenGL matrix form
                    // that doesn't match the current view matrix used to calculate the clip space

                    // transform clip space into normalized device coordinates
                    float3 ndc = clipPos.xyz / clipPos.w;

                    // convert ndc's depth from 1.0 near to 0.0 far to OpenGL style -1.0 near to 1.0 far 
                    ndc = float3(ndc.x, ndc.y * _ProjectionParams.x, (1.0 - ndc.z) * 2.0 - 1.0);

                    // transform back into clip space and apply inverse projection matrix
                    float3 viewPos =  mul(unity_CameraInvProjection, float4(ndc * clipPos.w, clipPos.w));
                #else
                    // using OpenGL, unity_CameraInvProjection matches view matrix
                    float3 viewPos = mul(unity_CameraInvProjection, clipPos);
                #endif

                    // transform from view to world space
                    return mul(unity_MatrixInvV, float4(viewPos, 1.0)).xyz;
            }
            
            float4 frag(v2f i) : SV_TARGET
            {
                float3 WorldPos = ClipToWorldPos(i.positionCS);
                
                if ( _CurrentCameraRendering == 1 )
                {
                    if (dot(_Portal1PlaneNormal, WorldPos-_Portal1PlanePoint) > 0)
                        discard;
                }
                else if ( _CurrentCameraRendering == 2 )
                {
                    if (dot(_Portal2PlaneNormal, WorldPos-_Portal2PlanePoint) > 0)
                        discard;
                }
                
                float4 textureColor = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, i.uv);
                return textureColor * _BaseColor;
            }
            
            ENDHLSL
        }

/*
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
                float3 normalsWS = NormalizeNormalPerPixel(i.normalWS);
                
                return float4(normalsWS, 0.0f);
            }
            
            ENDHLSL
        }
*/
    }
}