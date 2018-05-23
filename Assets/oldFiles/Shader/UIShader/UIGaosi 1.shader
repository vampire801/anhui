// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UIMohu/Gaosi1"
{
    Properties
    {
		//因为shader取值都是百分比，所以我们要定出贴图的尺寸来进行计算
		_TextureSize ("_TextureSize",Float) = 400
		//取值半径
		_BlurRadius ("_BlurRadius",Range(1,15) ) = 15
    }
    SubShader
    {
        // 透明队列，在所有不透明的几何图形后绘制
        Tags { "Queue" = "Transparent" }

        // 采样后面的图像
        GrabPass { }

        Pass 
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma debug
            #pragma vertex vert
            #pragma fragment frag
			#include "UnityCG.cginc"
            sampler2D _GrabTexture : register(s0);
			int _BlurRadius;
			float _TextureSize;
			//计算权重			
            struct data 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct v2f 
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
            };
			
            v2f vert(data i)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(i.vertex);
                o.uv = float4((o.pos.x+1)/2, -(o.pos.y-1)/2, o.pos.z, o.pos.w);
                return o;
            }
			float GetGaussianDistribution( float x, float y, float rho ) 
			{
				//float g = 1.0f / sqrt( 2.0f * 3.141592654f * rho * rho );
				//return (exp( -(x * x + y * y) / (2 * rho * rho) ))/(rho*2.506f);
					float g = 1.0f / sqrt( 2.0f * 3.141592654f * rho * rho );
				return g * exp( -(x * x + y * y) / (2 * rho * rho) );
			}
			float4 GetGaussBlurColor( float2 uv )
			{
				 //算出一个像素的空间
				 float space = 1.0/_TextureSize; 
				 //参考正态分布曲线图，可以知道 3σ 距离以外的点，权重已经微不足道了。
				 //反推即可知道当模糊半径为r时，取σ为 r/3 是一个比较合适的取值。
				 float rho = (float)_BlurRadius * space / 3.0;
			    //---权重总和
				float weightTotal = 0;
				 for( int x = 0 ; x <= _BlurRadius ; x++ )
				 {
					  for( int y = 0 ; y <= _BlurRadius ; y++)
					  {
						  weightTotal += GetGaussianDistribution(x * space, y * space, rho );
					  }
				 }
				 weightTotal=weightTotal*4;
			  //--------
				 float4 colorTmp = float4(0,0,0,0);
				 for( int k = -_BlurRadius ; k <= _BlurRadius ; k++ )
				 {
					 for( int y = -_BlurRadius ; y <= _BlurRadius ; y++ )
					{
						 float weight = GetGaussianDistribution( k * space, y * space, rho )/weightTotal;
					     float4 color = tex2D(_GrabTexture,uv + float2(k * space,y * space));
						// float4 color = tex2D(_GrabTexture,uv );
						  color = color * weight;
						 colorTmp += color;
					 }
				 }
				 return colorTmp;
			}
            half4 frag( v2f i ) : COLOR
            {
                //float2 screenPos = i.screenPos.xy / i.screenPos.w;
               // float depth= _blurSizeXY * 0.0005;

              //  screenPos.x = (screenPos.x + 1) * 0.5;
              //  screenPos.y = (screenPos.y + 1) * 0.5;

				//调用普通模糊
				//return GetBlurColor(i.uv);
				//调用高斯模糊  
				return GetGaussBlurColor(i.uv);
            }
            ENDCG
        }
    }
    Fallback Off
}