Shader "Kirara/StatusWaveBarShader"
{
    Properties
    {
        [MainTexture] _MainTex ("Base Texture", 2D) = "white" {}
        _EmptyColor ("Empty Color", Color) = (0.5, 0.5, 0.5, 1) // 空颜色
        _FullColor ("Full Color", Color) = (1, 1, 1, 1) // 满颜色
        _BackgroundColor ("Background Color", Color) = (0.25099, 0.25099, 0.25099, 0.25099) // 背景色
        _Value ("Value", Range(0, 1)) = 1.0     // 值比例
        _Speed ("Speed", Float) = 3.0           // 正弦波移动速度
        _Frequency ("Frequency", Float) = 15.0  // 正弦波频率
        _Amplitude ("Amplitude", Float) = 0.03  // 正弦波振幅
        _Theta ("Theta", Float) = 0.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            half4 _EmptyColor;
            half4 _FullColor;
            half4 _BackgroundColor;
            half _Value;
            half _Speed;
            half _Frequency;
            half _Amplitude;
            half _Theta;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 计算正弦波值
                half rad = radians(_Theta);
                float phi = _Time.y * _Speed;
                half xOfSin = IN.uv.y / cos(rad);
                half yOfSin = _Amplitude * sin(_Frequency * xOfSin + phi);
                half wave = IN.uv.y * tan(rad) + yOfSin * cos(rad);

                // 计算是否是背景区域
                half healthLine = _Value + wave;
                half isBackground = step(healthLine, IN.uv.x);

                // 采样条颜色
                half4 barColor = lerp(_EmptyColor, _FullColor, IN.uv.x);

                // 根据是否是背景区域采样条颜色或背景颜色
                half4 finalColor = lerp(barColor, _BackgroundColor, isBackground);

                return finalColor;
            }
            ENDHLSL
        }
    }
}
