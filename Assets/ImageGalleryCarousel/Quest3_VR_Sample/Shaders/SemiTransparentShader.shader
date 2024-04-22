Shader "Custom/SemiTransparentShader" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,0.5)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        _FresnelPower ("Fresnel Power", Range(1, 5)) = 2.5
    }

    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        AlphaToMask Off
        Cull Off // Disable backface culling

        CGPROGRAM
        #pragma surface surf Standard keepalpha fullforwardshadows

        #pragma target 3.0

        struct Input {
            float2 uv_MainTex;
            float3 viewDir;
        };

        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _FresnelColor;
        float _FresnelPower;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a; // Ensure alpha is passed to output

            float fresnel = pow(1.0 - max(dot(normalize(IN.viewDir), o.Normal), 0.0), _FresnelPower);
            // Add a conditional statement to reverse the fresnel effect when facing away from the normal
            if (dot(normalize(IN.viewDir), o.Normal) < 0) {
                fresnel = 1 - fresnel;
            }
            o.Emission = _FresnelColor.rgb * fresnel;
        }
        ENDCG
    }

    FallBack "Diffuse"
}