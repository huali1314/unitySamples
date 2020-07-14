Shader "MyShader/BattleFogShader" {
	Properties{
		_MainTex("MainTex",2D) = "white"{}
		_MaskTex("MaskTex",2D) = "white"{}
		_Lerp("Lerp",Range(0,3)) = 1
	}
	SubShader{
		Pass{
			Tags{ "LightMode" = "ForwardBase" }
 
			CGPROGRAM
			#include "Lighting.cginc"
			#pragma vertex vert
			#pragma fragment frag
			sampler2D _MaskTex;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _AlphaBase;
			float _Lerp;
			struct a2v {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			struct v2f {
				float4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
			};
			v2f vert(a2v i) {
				v2f o;
				o.pos = UnityObjectToClipPos(i.vertex);
				o.uv = TRANSFORM_TEX(i.texcoord, _MainTex);
				return o;
			}
			fixed4 frag(v2f o) :SV_TARGET{
				fixed4 color = tex2D(_MaskTex, o.uv);
				fixed4 color2 = tex2D(_MainTex, o.uv);
				color2.r *= color.r*_Lerp;
				color2.g *= color.g*_Lerp;
				color2.b *= color.b*_Lerp;
				return  color2;
			}
			ENDCG
		}
	}
}