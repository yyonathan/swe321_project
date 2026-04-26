Shader "Custom/PlatformShard"
{
    Properties
    {
        _ColorBase ("Base Color", Color) = (0.4, 0.15, 0.02, 1)
        _ColorLayer ("Layer Color", Color) = (0.9, 0.5, 0.1, 1)
        _Speed ("Animation Speed", Float) = 1.2
        _Scale ("Pattern Scale", Float) = 1.5
        _AspectRatio ("Aspect Ratio", Float) = 10.0
        _Seed ("Seed", Float) = 0.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _ColorBase;
                float4 _ColorLayer;
                float _Speed;
                float _Scale;
                float _AspectRatio;
                float _Seed;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                return OUT;
            }

            float rand(float2 seed)
            {
                return frac(sin(dot(seed, float2(127.1, 311.7))) * 43758.5453);
            }

            float triangleMask(float2 uv, int index, float time)
            {
                // offset seed by per-platform _Seed so each platform has unique pattern
                float fi = float(index) + _Seed;

                float2 center = float2(
                    rand(float2(fi, 0.1)) * _AspectRatio,
                    rand(float2(fi, 0.2))
                );

                float angle = rand(float2(fi, 0.3)) * 6.2831 + time * (rand(float2(fi, 0.4)) - 0.5) * 0.2;
                float size = (rand(float2(fi, 0.5)) * 0.3 + 0.1) * _Scale;

                float2 v0 = center + float2(cos(angle), sin(angle)) * size;
                float2 v1 = center + float2(cos(angle + 2.094), sin(angle + 2.094)) * size;
                float2 v2 = center + float2(cos(angle + 4.189), sin(angle + 4.189)) * size;

                float2 p = uv;
                float d0 = (p.x - v1.x) * (v0.y - v1.y) - (v0.x - v1.x) * (p.y - v1.y);
                float d1 = (p.x - v2.x) * (v1.y - v2.y) - (v1.x - v2.x) * (p.y - v2.y);
                float d2 = (p.x - v0.x) * (v2.y - v0.y) - (v2.x - v0.x) * (p.y - v0.y);

                bool hasNeg = (d0 < 0) || (d1 < 0) || (d2 < 0);
                bool hasPos = (d0 > 0) || (d1 > 0) || (d2 > 0);
                return (hasNeg && hasPos) ? 0.0 : 1.0;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float time = _Time.y * _Speed;
                float2 uv = float2(IN.uv.x * _AspectRatio, IN.uv.y);

                float totalBrightness = 0.0;
                int numTriangles = 12;

                for (int i = 0; i < numTriangles; i++)
                {
                    float fi = float(i) + _Seed;

                    float phase = rand(float2(fi, 0.9)) * 6.2831;
                    float rate = rand(float2(fi, 0.7)) * 1.5 + 0.5;
                    float alpha = sin(time * rate + phase) * 0.5 + 0.5;

                    float inside = triangleMask(uv, i, time);
                    totalBrightness += inside * alpha;
                }

                float shade = saturate(totalBrightness / (float(numTriangles) * 0.5));

                float4 col = lerp(_ColorBase, _ColorLayer, shade);
                col.a *= IN.color.a;
                return col;
            }
            ENDHLSL
        }
    }
}
