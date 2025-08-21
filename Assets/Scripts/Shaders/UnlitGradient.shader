Shader "Unlit/UnlitGradient"
{
    Properties
    {
        //Texture that will go overtop of the gradient
        _MainTex("Texture",2D) = "white" {}
        _ColorOverlay("Colour Overlay", Color) = (1,1,1,1)

        _GradientPosition("Colour Gradient Center", Range(0, 1)) = 0.5

        //Top Colour
       	_Color1("Colour 1", Color) = (1,1,1,1)
        _Color1Position("Colour 1 Amount", Range(0, 1)) = 0.5

        //Bottom Colour.
		_Color2("Colour 2", Color) = (0,0,0,1)
        _Color2Position("Colour 2 Amount", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" } //"RenderType"="Opaque"
        LOD 100

        Pass
        {
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
             half _GradientPosition;
           fixed4 _ColorOverlay;

           fixed4 _Color1;
            half _Color1Position;

           fixed4 _Color2;
           half _Color2Position;

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
               fixed4 result = lerp (_Color1 , _Color2, (_GradientPosition + (-_Color1Position  - -_Color2Position)) - 0.5 + (i.uv.y));
            //    fixed4 textureColor = tex2D(_MainTex, i.uv) + result;
            //    textureColor *= _ColorOverlay;
               return result ;
            }
            ENDCG
        }
    }
}