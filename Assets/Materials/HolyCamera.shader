﻿Shader "Hidden/HolyCamera"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SubTex("Texture", 2D) = "white" {}
		_Viewport("Viewport", Vector) = (0, 0, 1, 1)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _SubTex;
			float4 _Viewport;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(_SubTex, i.uv);
				
				return i.uv.x > _Viewport.x && i.uv.x < _Viewport.z &&
					i.uv.y > _Viewport.y && i.uv.y < _Viewport.w ? col2 : col;
			}
			ENDCG
		}
	}
}
