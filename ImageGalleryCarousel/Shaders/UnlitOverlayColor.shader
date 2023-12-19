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

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _OverlayColor;
				float _OverlayAlpha;

				v2f vert(appdata v)
				{
					v2f o;
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
