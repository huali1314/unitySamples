//Sub Shader状态值
//状态名称    　　       设置指令 　　　　　　　　　　 　　　　　　　　　　 　　　　                 解释
//Cull 　　　　   Cull Back|Front|Off 　　　　 　　　　　　　　　　　　　　　　     设置剔除模式，剔除背面|正面|关闭剔除  默认CullBack
//ZTest   　　　  ZTest Less Greater|LEqual|GEqual|Equal|NotEqual|Always 　　 设置深度时使用的函数　　　　　　　　默认ZTest LEqual 小于或等于目标深度才能被渲染
//ZWrite  　　    ZWrite On|Off   　　　　　　　　　　　　　　　　　　　　　　　     开启和关闭深度写入　　　　　　　　  默认ZWrite On
//Blend   　　　  Blend SrcFactor DstFactor    　　　　　　　　　　　　　　　　　  开启并设置混合模式　　       　


//SubShader标签类型：注意这些标签只能在SubShader中声明，不能再Pass中
//　　标签类型  　　　　　　　　　　　　　　　　           说明  　　　　　　　　    　　　　　　　　                              例　　　　子
//    Queue   　　　　　　　　         控制渲染顺序，指定该物体属于哪一个渲染队列  　　　　                             Tags{"Queue" = "Transparent"}
//  RenderType  　　　　　　          对着色器分类。例如：这是一个不透明着色器   　　　　　                            Tags{"RenderType" = "Opaque"}
// DisableBatching 　　　　          一些SubShader在使用Unity批处理时会出现问题。可以用该标签直接表明是否使用批处理 　　 Tags{"DisableBatching" = "True"}　　　　　　　　　　　                
// ForecNoShadowCasting    　　      控制该SubShader的物体是否会投射阴影  　　　　　　                              Tags{"ForceNoShadowCasting" = "True"}
// IgnoreProjector 　　　　　　       设置该SubShader的物体是否受Projector影响，True常用与半透明物体。　　　　         Tags{"IgnoreProjector" = "True"}　　　　　　　　　　　　　             
// CanUseSpriteAtlas   　　　　　     当该SubShader用于“sprite”时，将该标签设为False 　　                          Tags{"CanUseSpriteAtlas" = "False"}
// PriviewType 　　　　　　　　        材质面板的预览类型，一般默认材质预览效果是球形，还可以改为"Plane" "SkyBox"。　　   Tags{"PreviewType" = "Plane"}　　　　　　　　　　　　　             
Shader "Hidden/FisheyeShader"
{
    //属性配置,此处声明的属性在下方必须重新声明一次
    Properties
    {
        //主纹理 _MainTex
        _MainTex ("Base(RGB)", 2D) = "" {}
        //下面介绍一些常用到的属性类型
        //_ExternColor("ExternColor",Color) = (1,1,1,1)
        //_RangeNum("RangeNumber",Range(0,1)) = 2.5
        //_2DValue("2DValue",2D) = "white"{}
        //
    }
    //每个shader必须有至少一个SubShader，可以有多个，在shader使用时会选择能在目标平台上使用的SubShader，如果都不能使用，则使用FallBack中的shader代码
    //SubShader中定义了一系列可使用的标签，状态
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        //每个pass代表一次完整的渲染流程，但pass过多会使渲染性能下降，所以尽量使用较少的pass，标签和状态也可以在pass中声明，但subShader中的一些特定的标签，与pass中的标签是不一样的，状态是一样的
        //但如果在SubShader中声明的话将应用所有的pass
        Pass
        {
            //CG语言编写shader代码开始
            CGPROGRAM
            //声明顶点着色器
            #pragma vertex vert
            //声明片段着色器
            #pragma fragment frag
            //引用Unity内置的常用类型定义
            #include "UnityCG.cginc"

            struct v2f {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };
    
            float2 intensity;//亮度
            //顶点着色器实现
            v2f vert( appdata_img v ) 
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                return o;
            } 
            //声明采样器 _MainTex
            sampler2D _MainTex;
            //片段着色器实现 函数输入参数类型是个结构体v2f，输出COLOR类型的数据half4
            half4 frag(v2f i) : COLOR 
            {
                half2 coords = i.uv;
                coords = (coords - 0.5) * 2.0;      
                
                half2 realCoordOffs;
                realCoordOffs.x = (1-coords.y * coords.y) * intensity.y * (coords.x); 
                realCoordOffs.y = (1-coords.x * coords.x) * intensity.x * (coords.y);
                
                half4 color = tex2D (_MainTex, i.uv - realCoordOffs);    
                
                return color;
            }
            //CG语言编写shader代码结束
            ENDCG
        }
        //默认shader代码编写处
    }
}
