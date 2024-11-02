Shader "Custom/SimpleInstanceShader"
{
    Properties
    {        
    }
    SubShader
    {
		Tags{"Queue" = "Geometry"}

		Pass
		{	
			CGPROGRAM
			#include "UnityCG.cginc" 
			#pragma vertex vert	
			#pragma fragment frag
			#pragma target 4.5

			sampler2D _MainTex;
			
			float4x4 ObjectToWorld;
	
			struct InstanceParam
			{			
				float4 color;
				float4x4 instanceToObjectMatrix;
			};
	
		#if SHADER_TARGET >= 45			
			StructuredBuffer<InstanceParam> dataBuffer;
		#endif
		
			//顶点着色器输入
			struct a2v
			{
				float4  position : POSITION;
				float3  normal: NORMAL;
				float2  texcoord : TEXCOORD0;	
 			};

			//顶点着色器输出
			struct v2f
			{
				float4 position: SV_POSITION;
				float2 texcoord: TEXCOORD0;
				float4 color: COLOR;
			};

			v2f vert(a2v v, uint instanceID : SV_InstanceID)
			{
			#if SHADER_TARGET >= 45
				float4x4 instanceToObjectMatrix = dataBuffer[instanceID].instanceToObjectMatrix;
				float4 color = dataBuffer[instanceID].color;
			#else
				float4x4 instanceToObjectMatrix = float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
				float4 color = float4(1.0f, 1.0f, 1.0f, 1.0f);
			#endif

				float4 localPosition = mul(instanceToObjectMatrix, v.position);
				//float4 localPosition = v.position;
				float4 worldPosition = mul(ObjectToWorld, localPosition);						

				v2f o;
				//o.position = UnityObjectToClipPos(v.position);
				o.position = mul(UNITY_MATRIX_VP, worldPosition);		
				o.texcoord = v.texcoord;
				o.color = color;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target 
			{												
				return i.color;					
			}

            ENDCG
        }
    }

	Fallback "Diffuse"
}
