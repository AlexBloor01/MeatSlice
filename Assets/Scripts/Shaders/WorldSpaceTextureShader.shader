Shader "Custom/WorldSpaceTextureShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _LockTexture ("Lock Texture", Float) = 0.0
        _TextureOffset ("Texture Offset", Vector) = (0,0,0,0)
        _CurrentScale ("Current Scale", Vector) = (0,0,0,0)
    } 
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        fixed4 _Color;
        half _Glossiness;
        half _Metallic;
        float _LockTexture;
        float4 _TextureOffset;
        float4 _CurrentScale;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv;
            if (_LockTexture > 0.5)
            {
                // Use world position for UV coordinates, with an offset
                float scale = 0.1; // Adjust this scale to control the texture tiling
                 uv = ((IN.worldPos.x + _TextureOffset.x) * _CurrentScale.x, (IN.worldPos.z + _TextureOffset.y) * _CurrentScale.y);
            }
            else
            {
                // Use the object's UV coordinates
                uv = IN.uv_MainTex;
            }

            fixed4 c = tex2D(_MainTex, uv) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}