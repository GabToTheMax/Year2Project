Shader "Basics/ShaderTest1"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            float4 _BaseColor;
            
            struct appdata
            {
                float4 positionOS : POSITION;               
            };
            
            struct t2f
            {
                float4 positionCS : SV_POSITION;
            };
            
            t2f vert(appdata v)
            {
                t2f o = (t2f)0;
                
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                
                return o;
            }
            
            float4 frag(t2f i) : SV_TARGET
            {
                return _BaseColor;
            }
            
            ENDHLSL
        }
    }
}