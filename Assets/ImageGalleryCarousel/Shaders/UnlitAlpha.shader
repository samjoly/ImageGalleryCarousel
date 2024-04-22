// This shader allows for an unlit texture to be rendered with transparency controlled by an alpha value.
// Properties:
// _MainTex: The main texture
// _Alpha: The transparency level
Shader "Custom/UnlitAlpha"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Alpha("Alpha", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent+1" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On // Enable Z-write
        Cull Back // Cull back faces so only front faces are rendered

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
                UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO //Insert
            };

            sampler2D _MainTex;
            float _Alpha;

            v2f vert(appdata v)
            {
                v2f o;
                					UNITY_SETUP_INSTANCE_ID(v); //Insert
					UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a *= _Alpha; // Apply alpha transparency
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
