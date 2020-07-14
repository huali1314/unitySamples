Shader "Custom/NewSurfaceShader"
{
    //Albedo：漫反射。设置漫反射的贴图或颜色值
    //Metallic：金属，设置金属的贴图或颜色值（不能和Smoothness属性同时使用）
    //Smoothness：光滑度。设置物体表面光滑度（不能和Metallic属性同时使用）
    //Normal Map：法线贴图。用于描述物体表面凹凸程度的法线贴图
    //Height Map：高度图。用于描述视察偏移的灰度图
    //Occlusion:散射。用于设置照射到物体上的非直接光照散射的贴图
    //Emission：自发光。用于控制物体表面自发光颜色和强度的贴图
    //Detail Mask：细节蒙版。用于设置Secondary Maps的蒙版贴图
    //Tiling：平铺。用于设置贴图在物体上的平铺值
    //Offset：偏移。用于设置贴图在物体上的偏移值
    //Secondary Maps：2号贴图。带UV通道的2号贴图
    //Detail Albedo：细节漫反射。2号贴图的漫反射贴图
    //Normal Map：法线贴图。2号贴图的法线贴图
    //Tiling:平铺。用于设置2号贴图在物体表面的平铺值
    //Offset：偏移。用于设置2号贴图在物体表面的偏移值
    //UV Set：UV集。用于设置物体的UV集
    Properties
    {
        //          语法                                  说明
        //名称（“显示名称”，Vector） = 默认向量值         定义一个四维向量
        //名称（“显示名称”，Color） = 默认颜色值          定义一个颜色值（取值范围0～1的四维向量）
        //名称（“显示名称”，Float） = 默认浮点数值        定义一个浮点数属性
        //名称（“显示名称”，Range(min,max)）= 默认浮点数值        定义一个随机数，取值在min～max
        //名称（“显示名称”，2D） = 默认贴图名称{选项}        定义一个2d纹理属性
        //名称（“显示名称”，Rect） = 默认贴图名称{选项}        定义一个矩形纹理属性（非2的n次幂）
        //名称（“显示名称”，Cube） = 默认贴图名称{选项}        定义一个立方体纹理属性
        //选项：指的是一些纹理的可选参数：
        //  TexGen：纹理生成模式。可以是ObjectLinear，EyeLinear，SphereMap，CubeReflect，CubeNormal中的某个，这些模式与Opengl的纹理生成格式相对应
        //  如果使用了自定义的顶点程序，那么该参数将被忽略
        //  LightmapMode：如果使用了该选项，那么纹理将受渲染器的光照贴图参数影响。纹理将不会从材质中获取，而是曲子渲染器的设置
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    //子着色器
    SubShader
    {
        //四种类型：Opaque（不透明） Fade(渐变)  Transparent（透明）  Cutout（镂空）
        Tags { "RenderType"="Opaque" }
        
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
