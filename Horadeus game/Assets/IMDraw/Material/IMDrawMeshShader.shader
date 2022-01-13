Shader "IMDraw/IMDraw Mesh Shader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Int) = 4
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	fixed4 _Color;

	struct a2f
	{
		float4 vertex : POSITION;
	};

	struct v2f
	{
		float4 pos : POSITION;
	};

	v2f vert (a2f IN)
	{
		v2f o;
		#if UNITY_VERSION >= 540
		o.pos = UnityObjectToClipPos(IN.vertex);
		#else
		o.pos = mul(UNITY_MATRIX_VP, mul(_Object2World, float4(IN.vertex.xyz, 1.0))); // UNITY_SHADER_NO_UPGRADE
		#endif
		return o;	
	}

	void frag (v2f i, out fixed4 colour : SV_Target)
	{
		colour = _Color;
	}

	ENDCG	

	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			Lighting Off Cull Back ZTest [_ZTest] ZWrite Off
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