Shader "IMDraw/IMDraw Line Shader"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Int) = 4
	}

	CGINCLUDE
		
	#include "UnityCG.cginc"

	struct a2f
	{
		float4 vertex : POSITION;
		fixed4 colour : COLOR;
	};

	struct v2f
	{
		float4 pos : POSITION;
		fixed4 colour : COLOR;
	};

	v2f vert (a2f IN)
	{
		v2f o;
		#if UNITY_VERSION >= 540
		o.pos = UnityObjectToClipPos(IN.vertex);
		#else
		o.pos = mul(UNITY_MATRIX_VP, mul(_Object2World, float4(IN.vertex.xyz, 1.0))); // UNITY_SHADER_NO_UPGRADE
		#endif
		o.colour = IN.colour;
		return o;	
	}

	void frag (v2f i, out fixed4 colour : SV_Target)
	{
		colour = i.colour;
	}

	ENDCG	

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			Lighting Off Cull Off ZTest [_ZTest] ZWrite Off
			Fog{ Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma target 2.0

			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		} // Pass

	} // SubShader

} // Shader