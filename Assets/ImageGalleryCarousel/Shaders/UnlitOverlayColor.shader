// This shader allows for an unlit texture to be rendered with an overlay color and adjustable transparency.
// Properties:
// _MainTex: The main texture.
// _OverlayColor: The color used for overlaying on top of the texture.
// _OverlayAlpha: The transparency level of the overlay.

Shader "Custom/UnlitOverlayColor"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_OverlayColor("Overlay Color", Color) = (1,1,1,1)
		_OverlayAlpha("Overlay Alpha", Range(0,1)) = 1.0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100
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
				float4 _MainTex_ST;
				float4 _OverlayColor;
				float _OverlayAlpha;

				v2f vert(appdata v)
				{
					v2f o;

					UNITY_SETUP_INSTANCE_ID(v); //Insert
					UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

					
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// Sample the texture
					fixed4 texColor = tex2D(_MainTex, i.uv);

				// Calculate the tinted color (texture color * overlay color)
				fixed4 tintedColor = texColor * _OverlayColor;

				// Interpolate between original texture color and tinted color
				// based on the overlay alpha
				fixed4 finalColor = lerp(texColor, tintedColor, _OverlayAlpha);

				return finalColor;
				}

			ENDCG
		}
		}
			FallBack "Diffuse"
}
