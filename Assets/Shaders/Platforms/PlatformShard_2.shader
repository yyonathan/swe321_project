Shader "Custom/PlatformShard"
{
    Properties
    {
        _ColorBase ("Base Color", Color) = (0.4, 0.15, 0.02, 1)
        _ColorLayer ("Layer Color", Color) = (0.9, 0.5, 0.1, 1)
        _Speed ("Animation Speed", Float) = 1.2
        _Scale ("Pattern Scale", Float) = 1.5
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
                float3 worldPos : TEXCOORD1;
                float4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _ColorBase;
                float4 _ColorLayer;
                float _Speed;
                float _Scale;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.worldPos = worldPos;
                OUT.color = IN.color;
                return OUT;
            }

            float rand(float2 seed)
            {
                return frac(sin(dot(seed, float2(127.1, 311.7))) * 43758.5453);
            }

            // returns 1 if point p is on the positive side of a line through origin with given normal
            float halfPlane(float2 p, float2 normal)
            {
                return dot(p, normal) > 0.0 ? 1.0 : 0.0;
            }

            // returns whether point p is inside a triangle defined by three half planes
            // each triangle is randomly oriented and positioned based on its index
            float triangleMask(float2 worldXY, int index, float time)
            {
                float fi = float(index);

                // random center position in world space
                float2 center = float2(
                    rand(float2(fi, 0.1)) * 10.0 - 5.0,
                    rand(float2(fi, 0.2)) * 10.0 - 5.0
                );

                // random rotation angle, slowly drifting
                float angle = rand(float2(fi, 0.3)) * 6.2831 + time * (rand(float2(fi, 0.4)) - 0.5) * 0.2;

                // random size
                float size = (rand(float2(fi, 0.5)) * 1.5 + 0.5) * _Scale;

                // three vertices of the triangle rotated around center
                float2 v0 = center + float2(cos(angle), sin(angle)) * size;
                float2 v1 = center + float2(cos(angle + 2.094), sin(angle + 2.094)) * size;
                float2 v2 = center + float2(cos(angle + 4.189), sin(angle + 4.189)) * size;

                // test if worldXY is inside triangle using cross products
                float2 p = worldXY;
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
                float2 worldXY = IN.worldPos.xy;

                float totalBrightness = 0.0;
                int numTriangles = 12;

                for (int i = 0; i < numTriangles; i++)
                {
                    float fi = float(i);

                    // each triangle smoothly fades in and out at its own rate and phase
                    float phase = rand(float2(fi, 0.9)) * 6.2831;
                    float rate = rand(float2(fi, 0.7)) * 1.5 + 0.5;
                    float alpha = sin(time * rate + phase) * 0.5 + 0.5;

                    float inside = triangleMask(worldXY, i, time);
                    totalBrightness += inside * alpha;
                }

                // normalize and remap
                float shade = saturate(totalBrightness / (float(numTriangles) * 0.5));

                float4 col = lerp(_ColorBase, _ColorLayer, shade);
                col.a *= IN.color.a;
                return col;
            }
            ENDHLSL
        }
    }
}
