Shader "StandartSwitchColorMask"
{
    Properties
    {
        [NoScaleOffset]
        _MainTex("Texture (RGB)", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        [NoScaleOffset]
        _MaskTex("Color mask", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MaskTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 mC = tex2D(_MaskTex, IN.uv_MainTex);
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb * (mC.r * _Color) + (c.rgb * abs(mC.r - 1));
            o.Alpha = c.a;
        }
        ENDCG
    }
        FallBack "Diffuse"
}