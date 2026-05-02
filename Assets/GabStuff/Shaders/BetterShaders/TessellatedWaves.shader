Shader "Basics2/TessellatedWaves"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _BaseTexture("Base Texture", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBLend("Source Blend Mode", Integer) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend Mode", Integer) = 10
        _WaveHeight("Wave Height", Range(0.0, 1.0)) = 0.25
        _WaveSpeed("Wave Speed", Range(0.0, 10.0)) = 1.0
        _TesselationAmount("Tesselation Amount", Range(1.0, 64.0)) = 1.0
        _TesselationFadeStart("Tesselation Fade Start", Float) = 25.0
        _TesselationFadeEnd("Tesselation Fade End", Float) = 100.0
        
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        
        Pass
        {
            Blend [_SrcBLend] [_DstBlend]
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma  hull hull 
            #pragma domain domain
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseTexture_ST;
                float _WaveHeight;
                float _WaveSpeed;
                float _TesselationAmount;
                float _TesselationFadeStart;
                float _TesselationFadeEnd;
            CBUFFER_END
            
            TEXTURE2D(_BaseTexture);
            SAMPLER(sampler_BaseTexture);
            
            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct tessControlPoint
            {
                float3 positionWS : INTERNALTESSPOS;
                float2 uv : TEXCOORD0;
            };
            
            struct tessFactors
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };
            
            struct t2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            tessControlPoint vert(appdata v)
            {
                tessControlPoint o = (tessControlPoint)0;
                
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _BaseTexture);
                
                return o;
            }
            
            [domain("tri")]
            [outputcontrolpoints(3)]
            [outputtopology("triangle_cw")]
            [partitioning("fractional_even")]
            [patchconstantfunc("patchConstantFunc")]
            tessControlPoint hull(InputPatch<tessControlPoint, 3> patch, uint id: SV_OutputControlPointID)
            {
                return patch[id];
            }
            
            tessFactors patchConstantFunc(InputPatch<tessControlPoint,3> patch)
            {
                tessFactors f = (tessFactors) 0;
                
                float3 triPos0 = patch[0].positionWS;
                float3 triPos1 = patch[1].positionWS;
                float3 triPos2 = patch[2].positionWS;
                
                float3 edgeMid0 = (triPos1 + triPos2)/2.0f;
                float3 edgeMid1 = (triPos2 + triPos0)/2.0f;
                float3 edgeMid2 = (triPos0 + triPos1)/2.0f;
                
                float3 camPos = _WorldSpaceCameraPos;
                
                float dist0 = distance(edgeMid0, camPos);
                float dist1 = distance(edgeMid1, camPos);
                float dist2 = distance(edgeMid2, camPos);
                
                float fadeDist = _TesselationFadeEnd - _TesselationFadeStart;
                
                float edgeFactor0 = saturate(1.0f - (dist0 - _TesselationFadeStart) / fadeDist);
                float edgeFactor1 = saturate(1.0f - (dist1 - _TesselationFadeStart) / fadeDist);
                float edgeFactor2 = saturate(1.0f - (dist2 - _TesselationFadeStart) / fadeDist);
                
                f.edge[0] = max(edgeFactor0 * _TesselationAmount, 1);
                f.edge[1] = max(edgeFactor1 * _TesselationAmount, 1);
                f.edge[2] = max(edgeFactor2 * _TesselationAmount, 1);
                
                f.inside = (f.edge[0] + f.edge[1] + f.edge[2]) / 3.0f;
                
                return f;
            }
            
            [domain("tri")]
            t2f domain(tessFactors facotrs, OutputPatch<tessControlPoint, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
            {
                t2f i = (t2f)0;
                
                float3 positionWS = 
                    patch[0].positionWS * barycentricCoordinates.x +
                    patch[1].positionWS * barycentricCoordinates.y +
                    patch[2].positionWS * barycentricCoordinates.z;
                
                i.uv =
                    patch[0].positionWS * barycentricCoordinates.x +
                    patch[1].positionWS * barycentricCoordinates.y +
                    patch[2].positionWS * barycentricCoordinates.z;
                
                float waveVerticalDisplacement = sin(positionWS.x + positionWS.z + _Time.y * _WaveSpeed) * _WaveHeight;
                float3 newPositionWS = float3(positionWS.x, positionWS.y + waveVerticalDisplacement, positionWS.z);           
                
                i.positionCS = TransformWorldToHClip(newPositionWS);
                return i;
            }
            
            float4 frag(t2f i) : SV_TARGET
            {
                float4 textureColor = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, i.uv);
                return textureColor * _BaseColor;
            }
            
            ENDHLSL
        }
    }
}