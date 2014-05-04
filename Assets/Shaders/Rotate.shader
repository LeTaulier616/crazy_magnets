//If you dont want Color tint, just remove all _Color variables
//To gain some extra speed
Shader "Rotate Texture" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_Angle ("Angle", Range(0.0, 360)) = 0
	}
	SubShader {
		Tags { "Queue" = "Geometry" "RenderType"="Opaque" }
		LOD 100
		Pass {
			Lighting Off Fog { Mode Off }

			CGPROGRAM
				#pragma exclude_renderers flash
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _Color;
				float _Angle;

				struct a2f {
					float4 vertex : POSITION;
					float4 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 Pos : SV_POSITION;
					float2 Uv : TEXCOORD0;
				};

				v2f vert (a2f v) {
					v2f o;
					o.Pos = mul(UNITY_MATRIX_MVP, v.vertex);
					half a = radians(_Angle + _Time.y * 20);
					fixed ca = cos(a);
					fixed sa = sin(a);
					float2 nuv = v.texcoord.xy;
					nuv -= float2(0.5, 0.5);
					float2 uv = float2(nuv.x * ca - nuv.y * sa, nuv.x * sa + nuv.y * ca);
					nuv = uv + float2(0.5, 0.5);
					o.Uv = nuv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					return o;
				}

				fixed4 frag( v2f i ) : COLOR {
					fixed4 c = tex2D(_MainTex, i.Uv) * _Color;
					return c;
				}
			ENDCG
		}
	} 
	FallBack Off
}