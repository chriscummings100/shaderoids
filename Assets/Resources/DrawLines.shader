Shader "DrawLines"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

        CGINCLUDE
		struct v2f
		{
			float4 vertex : SV_POSITION;
		};

        struct Line {
            float2 a;
            float2 b;
        };
        struct Character {
            float2 pos;
            float2 scl;
            int id;
        };

        StructuredBuffer<Line> lines;
        StructuredBuffer<Character> characters;

        #define LINES_PER_CHARACTER 16

        float4 toScreen(float2 p) {
            p = 2 * (p - float2(1024, 768)*0.5) / float2(1024, 768);
            return float4(p, 0, 1);
        }
			
        //simple just reads line from buffer
		v2f linevert (uint id : SV_VertexID, uint inst : SV_InstanceID)
		{
            Line l = lines[id / 2];
            float2 p = (id & 1) ? l.a : l.b;
			v2f o;
            o.vertex = toScreen(p);
			return o;
		}

        //reads character id, then looks it up in lines buffer which should contain font
        v2f charvert(uint id : SV_VertexID, uint inst : SV_InstanceID)
        {
            int vertsPerCharacter = (LINES_PER_CHARACTER * 2);
            int charIdx = id / vertsPerCharacter;

            int charVertex = id % vertsPerCharacter;
            int charLine = charVertex / 2;
            int charLineVertex = charVertex % 2;

            Character c = characters[charIdx];

            Line l = lines[c.id*LINES_PER_CHARACTER + charLine];
            float2 p = charLineVertex==0 ? l.a : l.b;

            p = p * c.scl + c.pos;

            v2f o;
            o.vertex = toScreen(p);
            return o;
        }

		fixed4 frag (v2f i) : SV_Target
		{
			return 1;
		}
        ENDCG


		Pass
		{
			CGPROGRAM
			#pragma vertex linevert
			#pragma fragment frag
			ENDCG
		}
		Pass
		{
			CGPROGRAM
			#pragma vertex charvert
			#pragma fragment frag
			ENDCG
		}
	}
}
