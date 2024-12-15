Shader "Custom/DimmedOpaque"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _DimAmount ("Dim Amount", Range(0, 1)) = 0.5  // Điều chỉnh mức độ làm mờ
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            float _DimAmount;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = float4(1 - _DimAmount, 1 - _DimAmount, 1 - _DimAmount, 1);  // Làm giảm độ sáng của màu sắc
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return i.color; // Trả về màu đã bị giảm sáng
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}