Shader "Custom/Gradient"
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

        //Metallic
        _Metallic("Metallic", Range(0, 1)) = 0

        //Gloss
        _Glossiness("Glossiness", Range(0, 1)) = 0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        
        CGPROGRAM

        #pragma surface surf Standard  //alpha:fade 
        #pragma target 3.0
        
        struct Input
        {
            float2 uv_MainTex;
        };
        
        sampler2D _MainTex;

        half _Glossiness;
        half _Metallic;

        half _GradientPosition;
        fixed4 _ColorOverlay;

        fixed4 _Color1;
        half _Color1Position;

        fixed4 _Color2;
        half _Color2Position;

        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        { 
            //Setup Colour.
            fixed4 result = lerp (_Color1 , _Color2, (_GradientPosition + (-_Color1Position  - -_Color2Position)) - 0.5 + (IN.uv_MainTex.y));
            //Add Texure
            fixed4 textureColor = tex2D(_MainTex, IN.uv_MainTex) + result;
            textureColor *= _ColorOverlay;
            o.Albedo = textureColor.rgb;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = result.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
