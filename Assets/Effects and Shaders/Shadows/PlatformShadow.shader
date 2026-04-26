Shader "Custom/PlatformShadow"
{
    Properties
    {
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 0.4)
        _BlurStart ("Blur Start", Range(0, 1)) = 0.3
        _BlurEnd ("Blur End", Range(0, 1)) = 1.0
        _EdgeBlur ("Edge Blur", Range(0, 1)) = 0.1
        _Alpha ("Alpha", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent-1"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _ShadowColor;
                float _BlurStart;
                float _BlurEnd;
                float _EdgeBlur;
                float _Alpha;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // uv.y = 0 at platform edge, 1 at shadow tip
                float lengthFade = smoothstep(_BlurEnd, _BlurStart, IN.uv.y);

                // fade horizontal edges
                float edgeFade = smoothstep(0, _EdgeBlur, IN.uv.x) * smoothstep(1, 1 - _EdgeBlur, IN.uv.x);

                float alpha = _ShadowColor.a * lengthFade * edgeFade * _Alpha;
                return half4(_ShadowColor.rgb, alpha);
            }
            ENDHLSL
        }
    }
}