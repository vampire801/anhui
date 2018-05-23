// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UIMohu/Dongtai"
{
    Properties
    {
        _blurSizeXY("BlurSizeXY", Range(0,10)) = 2
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

            sampler2D _GrabTexture : register(s0);
            float _blurSizeXY;
			//计算权重
			
            struct data 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct v2f 
            {
                float4 position : POSITION;
                float4 screenPos : TEXCOORD0;
            };

            v2f vert(data i)
            {
                v2f o;
                o.position = UnityObjectToClipPos(i.vertex);
                o.screenPos = float4(o.position.x, -o.position.y, o.position.z, o.position.w);
                return o;
            }

            half4 frag( v2f i ) : COLOR
            {
                float2 screenPos = i.screenPos.xy / i.screenPos.w;
                float depth= _blurSizeXY * 0.0005;

                screenPos.x = (screenPos.x + 1) * 0.5;
                screenPos.y = (screenPos.y + 1) * 0.5;

                half4 sum = half4(0,0,0,0);

                sum += tex2D( _GrabTexture, float2(screenPos.x-5.0 * depth, screenPos.y+5.0 * depth)) * 0.025;    
                sum += tex2D( _GrabTexture, float2(screenPos.x+5.0 * depth, screenPos.y-5.0 * depth)) * 0.025;

                sum += tex2D( _GrabTexture, float2(screenPos.x-4.0 * depth, screenPos.y+4.0 * depth)) * 0.05;
                sum += tex2D( _GrabTexture, float2(screenPos.x+4.0 * depth, screenPos.y-4.0 * depth)) * 0.05;

                sum += tex2D( _GrabTexture, float2(screenPos.x-3.0 * depth, screenPos.y+3.0 * depth)) * 0.09;
                sum += tex2D( _GrabTexture, float2(screenPos.x+3.0 * depth, screenPos.y-3.0 * depth)) * 0.09;

                sum += tex2D( _GrabTexture, float2(screenPos.x-2.0 * depth, screenPos.y+2.0 * depth)) * 0.12;
                sum += tex2D( _GrabTexture, float2(screenPos.x+2.0 * depth, screenPos.y-2.0 * depth)) * 0.12;

                sum += tex2D( _GrabTexture, float2(screenPos.x-1.0 * depth, screenPos.y+1.0 * depth)) *  0.15;
                sum += tex2D( _GrabTexture, float2(screenPos.x+1.0 * depth, screenPos.y-1.0 * depth)) *  0.15;

                sum += tex2D( _GrabTexture, screenPos-5.0 * depth) * 0.025;    
                sum += tex2D( _GrabTexture, screenPos-4.0 * depth) * 0.05;
                sum += tex2D( _GrabTexture, screenPos-3.0 * depth) * 0.09;
                sum += tex2D( _GrabTexture, screenPos-2.0 * depth) * 0.12;
                sum += tex2D( _GrabTexture, screenPos-1.0 * depth) * 0.15;    
                sum += tex2D( _GrabTexture, screenPos) * 0.16; 
                sum += tex2D( _GrabTexture, screenPos+5.0 * depth) * 0.15;
                sum += tex2D( _GrabTexture, screenPos+4.0 * depth) * 0.12;
                sum += tex2D( _GrabTexture, screenPos+3.0 * depth) * 0.09;
                sum += tex2D( _GrabTexture, screenPos+2.0 * depth) * 0.05;
                sum += tex2D( _GrabTexture, screenPos+1.0 * depth) * 0.025;

                return sum / 2;
            }
            ENDCG
        }
    }
    Fallback Off
}