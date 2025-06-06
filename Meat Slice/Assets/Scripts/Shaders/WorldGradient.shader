Shader "Custom/WorldGradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _GradColor1("Gradient Color 1", Color) = (1,1,1,1)
        _GradColor2("Gradient Color 2", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
            float4 _MainTex_ST;
             float4 _MainTex_TexelSize;

            sampler2D _CameraDepthNormalsTexture; // Contains DepthNormals map.

            float4 _GradColor1;
            float4 _GradColor2;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }



            //Get the depth map
            float GetDepth(float2 uv){

                //convert the depth normals to float4 
                fixed4 depthNormals = tex2D(_CameraDepthNormalsTexture, uv);

                //convert rgb of normals to between 0 and 1 based on 'distance From Camera' (LinearEyeDepth)
                float depth = LinearEyeDepth(depthNormals.r) * LinearEyeDepth(depthNormals.g) * LinearEyeDepth(depthNormals.b);

                return depth;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float depth = GetDepth(i.uv);

                float4 gradient = lerp(_GradColor1, _GradColor2, i.uv.y);

                // sample the texture
                fixed4 result = tex2D(_MainTex, i.uv) + gradient;

                // result.a = lerp(gradient, result, depth);

                result = fixed4(depth, depth, depth, 1);
                // fixed4 result = tex2D(_CameraDepthNormalsTexture, i.uv);

                return result;
            }
            ENDCG
        }
    }
}
