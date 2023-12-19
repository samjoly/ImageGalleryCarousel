Shader "Custom/UnlitColorAlpha"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1) // Color property
        _Alpha("Alpha", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent+1" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float4 _Color; // Color property
            float _Alpha;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = _Color; // Use the color property
                col.a *= _Alpha; // Apply alpha transparency
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
