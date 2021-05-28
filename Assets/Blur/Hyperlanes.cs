using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Hyperlanes : ScriptableRendererFeature
{
    public static bool enabled = true;

    [System.Serializable]
    public class HyperlanesSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material material = null;
    }

    public HyperlanesSettings settings = new HyperlanesSettings();

    class CustomRenderPass : ScriptableRenderPass
    {
        public Material material;
        string profilerTag;

        int tmpId1;
        int tmpId2;

        RenderTargetIdentifier tmpRT1;
        RenderTargetIdentifier tmpRT2;

        private RenderTargetIdentifier source { get; set; }

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;
        }

        public CustomRenderPass(string profilerTag)
        {
            this.profilerTag = profilerTag;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var width = cameraTextureDescriptor.width;
            var height = cameraTextureDescriptor.height;

            tmpId1 = Shader.PropertyToID("tmpBlurRT1");
            tmpId2 = Shader.PropertyToID("tmpBlurRT2");
            cmd.GetTemporaryRT(tmpId1, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            cmd.GetTemporaryRT(tmpId2, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);

            tmpRT1 = new RenderTargetIdentifier(tmpId1);
            tmpRT2 = new RenderTargetIdentifier(tmpId2);

            ConfigureTarget(tmpRT1);
            ConfigureTarget(tmpRT2);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!enabled)
            {
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            cmd.DrawProcedural(Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one), material, 0, MeshTopology.Quads, 4);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass scriptablePass;

    public override void Create()
    {
        scriptablePass = new CustomRenderPass("Hyperlanes")
        {
            material = settings.material,

            renderPassEvent = settings.renderPassEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var src = renderer.cameraColorTarget;
        scriptablePass.Setup(src);
        renderer.EnqueuePass(scriptablePass);
    }
}