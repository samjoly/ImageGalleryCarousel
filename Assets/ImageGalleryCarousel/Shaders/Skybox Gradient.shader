// This shader creates a vertical gradient for a skybox that can be rotated based on an angle.
// Properties:
// _Top: Color at the top of the skybox
// _Bottom: Color at the bottom of the skybox
// _mult: Multiplier for the gradient effect to control the spread
// _pwer: Power for the gradient transition to control the sharpness
// _Angle: Angle in radians to rotate the gradient around the vertical axis
// _Screenspace: Toggle to switch calculation to screen space instead of world space

Shader "Skybox Gradient"

Shader "Skybox Gradient"
{
    Properties
    {
        _Top("Top", Color) = (1,1,1,0)
        _Bottom("Bottom", Color) = (0,0,0,0)
        _mult("Mult", Float) = 1
        _pwer("Power", Float) = 1
        _Angle("Angle", Float) = 0 // New property for angle
        [Toggle(_SCREENSPACE_ON)] _Screenspace("Screen space", Float) = 0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGINCLUDE
        #pragma target 3.0
        ENDCG
        Blend Off
        Cull Back
        ColorMask RGBA
        ZWrite On
        ZTest LEqual
        Offset 0, 0

        Pass
        {
            Name "Unlit"
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM

            #ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
            #define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
            #endif
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #pragma shader_feature_local _SCREENSPACE_ON

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
                float4 screenPos : TEXCOORD0;
            };

            uniform float4 _Bottom;
            uniform float4 _Top;
            uniform float _mult;
            uniform float _pwer;
            uniform float _Angle; // Uniform for the angle

            // Function to rotate a 2D point by a given angle
            float2 rotate(float2 v, float a)
            {
                float s = sin(a);
                float c = cos(a);
                return float2(c * v.x - s * v.y, s * v.x + c * v.y);
            }

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float2 screenPosNorm = (i.screenPos.xy / i.screenPos.w) * 2.0 - 1.0;
                screenPosNorm = rotate(screenPosNorm, _Angle);

                // Adjust the position based on the angle
                float verticalPosition = screenPosNorm.y * 0.5 + 0.5;

                float4 gradientColor = lerp(_Bottom, _Top, pow(saturate(verticalPosition * _mult), _pwer));
                return gradientColor;
            }

            ENDCG
        }
    }
    CustomEditor "ASEMaterialInspector"
}
