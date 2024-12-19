Shader "Custom/Embroidery"
{
    Properties
    {
        _BaseMap ("Base Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _ShineColor ("Shine Color", Color) = (1,1,1,1)
        _NoiseScale ("Noise Scale", Float) = 10.0
        _ShineIntensity ("Shine Intensity", Float) = 0.5
        _EdgeSoftness ("Edge Softness", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _BaseMap; // Альбедо-текстура
            float4 _BaseColor;
            float4 _ShineColor;
            float _NoiseScale;
            float _ShineIntensity;
            float _EdgeSoftness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float noise(float2 uv)
            {
                return frac(sin(dot(uv.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * _NoiseScale;

                // Получение базового цвета из текстуры
                float4 baseColor = tex2D(_BaseMap, i.uv) * _BaseColor;

                // Генерация полосок
                float stripe = abs(sin(uv.y * 10.0 + noise(uv) * 0.5));

                // Блеск
                float shine = pow(abs(dot(normalize(float3(1.0, 1.0, 1.0)), float3(0,0,1))), _ShineIntensity);

                // Смягчение краёв
                float edge = smoothstep(0.0, _EdgeSoftness, stripe);

                // Финальный цвет
                return lerp(baseColor, _ShineColor, stripe * shine) * edge;
            }
            ENDCG
        }
    }
    }
