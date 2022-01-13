Shader "IMDraw/IMDraw Arc Shader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_InnerRadius("Inner radius", Range(0, 1)) = 0
		_DirAngle("Direction angle (deg)", Range(0, 360)) = 0
		_SectorAngle("Sector angle (deg)", Range(0, 180)) = 180
		
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Int) = 4
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	fixed4 _Color;
	float _InnerRadius;// , _OuterRadius;
	float _DirAngle;
	float _SectorAngle;

	struct a2f
	{
		float4 vertex : POSITION;
	};

	struct v2f
	{
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};

	#define DEG_TO_RAD 0.017453292519943295

	v2f vert(a2f IN)
	{
		IN.vertex *= step(0.0000001, _SectorAngle); // Optimisation where if the sector angle is close to zero, then we don't want to draw anything so reduce the mesh to a degenerate

		v2f o;

		#if UNITY_VERSION >= 540
		o.pos = UnityObjectToClipPos(IN.vertex);
		#else
		o.pos = mul(UNITY_MATRIX_VP, mul(_Object2World, float4(IN.vertex.xyz, 1.0))); // UNITY_SHADER_NO_UPGRADE
		#endif

		float2 uv = IN.vertex.xz / 0.5; // -1 to 1 in XY axis
		
		float r = cos(_DirAngle * DEG_TO_RAD);
		float s = sin(_DirAngle * DEG_TO_RAD);

		o.uv.x = uv.x * r - uv.y * s;
		o.uv.y = uv.x * s + uv.y * r;

		return o;
	}

	void frag(v2f i, out fixed4 color : SV_Target)
	{
		//clip(_SectorAngle - 0.0000001); // Do not draw anything if arc angle is close to zero - Note: replaced with vertex degenerate method

		color = _Color;

		float l = length(i.uv);

		float dp = dot(normalize(i.uv.xy), float2(0.0, 1.0));
		float cosA = cos(_SectorAngle * DEG_TO_RAD);

		color.a *= lerp(step(cosA, dp), 1, step(180, _SectorAngle)); // Clamp arc angle
		color.a *= step(l, 1.0) * step(_InnerRadius, l);

		//clip(color.a - 0.0001); // Avoid drawing if alpha is close to zero
	}

	ENDCG

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True" }

		Pass
		{
			Lighting Off Cull Back ZTest[_ZTest] ZWrite Off
			Fog{ Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha
			//ColorMask RGB

			CGPROGRAM
			#pragma target 2.0
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		} // Pass

	} // SubShader

} // Shader