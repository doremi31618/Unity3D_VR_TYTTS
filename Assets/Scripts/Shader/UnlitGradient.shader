Shader "Eric/GradientShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha("Alpha", Range(1,0))=0.8
        _Color("Color", Color)=(1,1,1,1)
        _Max("Max", Range(1,0))= 0.8
        _Min("Min", Range(1,0))= 0.2
        [Enum(Exponential, 0, Linear, 1)]_Mode("Mode", int) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"  "Queue" = "Transparent"}
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color,_MainTex_ST;
            float _Alpha,_Max, _Min;
            int _Mode;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col.r *= _Color.r;
                col.g *= _Color.g;
                col.b *= _Color.b;
                col.a = _Alpha;
                if(_Mode==1){
                    if(i.uv.y<=_Min)
                        col.a *= (i.uv.y/_Min);
                    else if(i.uv.y>_Max)
                        col.a *= (1-i.uv.y)/(1-_Max);
                }else{
                    _Min=clamp(_Min,0.001,0.999);
                    if(i.uv.y>=_Min)
                        col.a *= abs((exp((1-i.uv.y)/(1-_Min))-1));
                    else
                        col.a *= abs((exp(i.uv.y/_Min)-1));
                }
                
                return col;
            }
            ENDCG
        }
    }
}