Shader "Custom/CheckerTransparency_Soft"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _CellCountX ("Cell Count X", Range(1,100)) = 8
        _CellCountY ("Cell Count Y", Range(1,100)) = 8
        _AlphaVisible ("White Alpha", Range(0,1)) = 0.6
        _AlphaHidden ("Dark Alpha", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float _CellCountX;
            float _CellCountY;
            float _AlphaVisible;
            float _AlphaHidden;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * float2(_CellCountX, _CellCountY);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 grid = floor(i.uv + 0.0001);
                float checker = fmod(grid.x + grid.y, 2.0);
                float alpha = lerp(_AlphaHidden, _AlphaVisible, checker);
                return float4(_Color.rgb, alpha * _Color.a);
            }

            ENDHLSL
        }
    }
}