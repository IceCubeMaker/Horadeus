Shader "IMDraw/IMDraw Text Mesh Shader"
{
	Properties
	{
		_MainTex ("Font Texture", 2D) = "white" {}
		_Color ("Text Color", Color) = (1,1,1,1)
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Int) = 4
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	struct a2f
	{
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	uniform fixed4 _Color;

	v2f vert (a2f IN)
	{
		v2f o;
		#if UNITY_VERSION >= 540
		o.vertex = UnityObjectToClipPos(IN.vertex);
		#else
		o.vertex = mul(UNITY_MATRIX_VP, mul(_Object2World, float4(IN.vertex.xyz, 1.0))); // UNITY_SHADER_NO_UPGRADE
		#endif
		o.color = IN.color * _Color;
		o.texcoord = TRANSFORM_TEX(IN.texcoord,_MainTex);
		return o;
	}

	fixed4 frag (v2f i) : SV_Target
	{
		fixed4 col = i.color;
		col.a *= tex2D(_MainTex, i.texcoord).a;
		return col;
	}
	ENDCG

	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }

		Pass
		{
			Lighting Off Cull Off ZTest [_ZTest] ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma target 2.0

			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		} // Pass
	} // SubShader
} // Shader