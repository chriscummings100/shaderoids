Shader "DrawLines"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

            struct Line {
                float3 a;
                float3 b;
            };

            StructuredBuffer<Line> lines;
			
			v2f vert (uint id : SV_VertexID, uint inst : SV_InstanceID)
			{
                Line l = lines[id / 2];
                float3 p = (id & 1) ? l.a : l.b;
				v2f o;
                o.vertex = float4(p, 1);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return 1;
			}
			ENDCG
		}
	}
}
