using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class CustomRenderPipeline : RenderPipeline
{
    private Color clearColor = Color.black;
    public CustomRenderPipeline(Color clearColor)
    {
        this.clearColor = clearColor;
    }
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        //base.Render(context, cameras);
        var cmd = new CommandBuffer();
        cmd.ClearRenderTarget(true, true, clearColor);
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        context.Submit();
    }
}
