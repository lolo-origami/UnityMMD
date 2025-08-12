using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//Toguchiさんのやつまんま

public abstract class CustomPostProcessingPass<T> : ScriptableRenderPass where T : VolumeComponent
{
    protected static readonly int TempColorBufferId = UnityEngine.Shader.PropertyToID("_TempColorBuffer");
    
    protected Shader Shader;
    protected Material Material;
    protected T Component;

    private RenderTargetIdentifier _renderTargetIdentifier;
    private RenderTargetIdentifier _tempRenderTargetIdentifier;
    
    protected abstract string RenderTag { get; }
    
    //シェーダーとマテリアルの登録
    public CustomPostProcessingPass(RenderPassEvent renderPassEvent, Shader shader)
    {
        this.renderPassEvent = renderPassEvent;
        Shader = shader;

        if (shader == null)
        {
            return;
        }
        
        Material = CoreUtils.CreateEngineMaterial(shader);
    }
    
    //tmpバッファとレンダーターゲットの取得
    public virtual void Setup(in RenderTargetIdentifier renderTargetIdentifier)
    {
        _renderTargetIdentifier = renderTargetIdentifier;
        _tempRenderTargetIdentifier = new RenderTargetIdentifier(TempColorBufferId);
    }

    //パスの実行時
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (Material == null || !renderingData.cameraData.postProcessEnabled)
        {
            return;
        }

        var volumeStack = VolumeManager.instance.stack;
        Component = volumeStack.GetComponent<T>();
        if (Component == null || !Component.active || !IsActive())
        {
            return;
        }
        
        var commandBuffer = CommandBufferPool.Get(RenderTag);
        Render(commandBuffer, ref renderingData);
        context.ExecuteCommandBuffer(commandBuffer);
        CommandBufferPool.Release(commandBuffer);
    }

    /// <summary>
    /// レンダー中身
    /// </summary>
    /// <param name="commandBuffer"></param>
    /// <param name="renderingData"></param>
    private void Render(CommandBuffer commandBuffer, ref RenderingData renderingData)
    {
        var source = _renderTargetIdentifier;
        var dest = _tempRenderTargetIdentifier;
        
        //RenderTextureを準備
        SetupRenderTexture(commandBuffer, ref renderingData);
        
        //Render前にやることあれば
        BeforeRender(commandBuffer, ref renderingData);
        
        //一時バッファのコピー
        CopyToTempBuffer(commandBuffer, ref renderingData, source, dest);
        
        //レンダリング
        //Blitと、継承クラスで何かやることあれば
        Render(commandBuffer, ref renderingData, dest, source);
        
        //後処理
        CleanupRenderTexture(commandBuffer, ref renderingData);
    }

    protected virtual void SetupRenderTexture(CommandBuffer commandBuffer, ref RenderingData renderingData)
    {
        ref var cameraData = ref renderingData.cameraData;

        var desc = new RenderTextureDescriptor(cameraData.camera.scaledPixelWidth, cameraData.camera.scaledPixelHeight);
        desc.colorFormat = cameraData.isHdrEnabled ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
        
        // RT確保
        commandBuffer.GetTemporaryRT(TempColorBufferId, desc);
    }

    protected virtual void CleanupRenderTexture(CommandBuffer commandBuffer, ref RenderingData renderingData)
    {
        // RT開放
        commandBuffer.ReleaseTemporaryRT(TempColorBufferId);
    }

    protected virtual void CopyToTempBuffer(CommandBuffer commandBuffer, ref RenderingData renderingData,
        RenderTargetIdentifier source, RenderTargetIdentifier dest)
    {
        commandBuffer.Blit(source, dest);
    }

    protected virtual void Render(CommandBuffer commandBuffer, ref RenderingData renderingData,
        RenderTargetIdentifier source, RenderTargetIdentifier dest)
    {
        commandBuffer.Blit(source, dest, Material);
    }

    protected abstract void BeforeRender(CommandBuffer commandBuffer, ref RenderingData renderingData);

    protected abstract bool IsActive();
}
