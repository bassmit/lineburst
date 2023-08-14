Shader "LineBurst/LineBurstLineShader" {
	Properties
	{
	}
	SubShader
    {
    	Tags
        {
            "RenderPipeline"="HDRenderPipeline"
            "RenderType"="HDUnlitShader"
            "Queue"="Geometry+0"
            "DisableBatching"="False"
            "ShaderGraphTargetId"="HDUnlitSubTarget"
        	"IgnoreProjector" = "True"
        	"ShaderModel"="2.0"
        }
    	
        LOD 100

        Pass
        {
            Name "Unlit"

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 2.0

            #pragma vertex vert
            #pragma fragment frag

            float4x4 unity_MatrixVP;
            StructuredBuffer<float4> LineBurstVertex;

            struct Varyings
            {
                float4 vertex : SV_POSITION;
           		float4 color : TEXCOORD0;
            };

            float4 unpack(float i)
            {
                return float4(i / 262144.0, i / 4096.0, i / 64.0, i) % 64.0 / 63;
            }

            Varyings vert(uint vid : SV_VertexID)
            {
                Varyings output = (Varyings)0;
                float4 pos = LineBurstVertex[vid];
                output.vertex = mul(unity_MatrixVP, float4(pos.xyz, 1));
                output.color = unpack(pos.w);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return input.color;
            }
            ENDHLSL
        }
    }
	SubShader
    {
        Tags {"RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" "ShaderModel"="2.0"}
        LOD 100

        Pass
        {
            Name "Unlit"

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 2.0

            #pragma vertex vert
            #pragma fragment frag

            float4x4 unity_MatrixVP;
            StructuredBuffer<float4> LineBurstVertex;

            struct Varyings
            {
                float4 vertex : SV_POSITION;
           		float4 color : TEXCOORD0;
            };

            float4 unpack(float i)
            {
                return float4(i / 262144.0, i / 4096.0, i / 64.0, i) % 64.0 / 63;
            }

            Varyings vert(uint vid : SV_VertexID)
            {
                Varyings output = (Varyings)0;
                float4 pos = LineBurstVertex[vid];
                output.vertex = mul(unity_MatrixVP, float4(pos.xyz, 1));
                output.color = unpack(pos.w);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return input.color;
            }
            ENDHLSL
        }
    }
	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			#pragma target 2.0

			#include "UnityCG.cginc"

			StructuredBuffer<float4> positionBuffer;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 color : TEXCOORD0;
			};

            float4 unpack(float i)
            {
                return float4(i / 262144.0, i / 4096.0, i / 64.0, i) % 64.0 / 63;
            }

			v2f vert(uint vid : SV_VertexID)
			{
				v2f o;
                float4 pos = positionBuffer[vid];
				o.pos = mul(UNITY_MATRIX_VP, float4(pos.xyz, 1));
				o.color = unpack(pos.w);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return i.color;
			}

			ENDCG
		}
	}
}
