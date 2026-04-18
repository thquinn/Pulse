Shader "thquinn/LinkShader"
{
	Properties{
		_MainTex ("Texture", 2D) = "white" {}
		_Thickness ("Thickness", Range(0, 1)) = 0
		_AnimSpeed ("Animation Speed", Range(0, 1)) = 0
	}

		SubShader{
		Tags{ 
				"RenderType"="Transparent" 
				"Queue"="Transparent"
				"DisableBatching" = "True"
	}

		Blend SrcAlpha OneMinusSrcAlpha

		ZWrite off
		Cull off

		Pass{

				CGPROGRAM

#include "UnityCG.cginc"

#pragma vertex vert
#pragma fragment frag

				sampler2D _MainTex;
	float4 _MainTex_ST;

	float _Thickness;

	struct appdata{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		fixed4 color : COLOR;
	};

	struct v2f{
		float4 position : SV_POSITION;
		float2 uv : TEXCOORD0;
		fixed4 color : COLOR;
	};

	v2f vert(appdata v){
		v2f o;
		o.position = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.color = v.color;
		return o;
	}

	fixed4 frag(v2f i) : SV_TARGET{
		float2 uv = (i.uv - float2(.5, .5)) * 2;
		float r = 1 - abs(uv.y) - .001;
		float scale = length(unity_ObjectToWorld._m00_m10_m20);
		float thickness = _Thickness;
		// Effects.
		float x = uv.x + _Time.y * sign(uv.y);
		float normsin = (sin(x * 10 * scale) + 1) / 2;
		thickness += normsin * .1;
		// Back to the rest...
		float blur = .001 / scale;
		float c = min(smoothstep(1, 1 - blur, r), smoothstep(1 - thickness - blur, 1 - thickness, r));
		fixed4 col = tex2D(_MainTex, i.uv);
		col *= i.color;
		col.a *= c;
		return col;
	}

		ENDCG
	}
	}
}