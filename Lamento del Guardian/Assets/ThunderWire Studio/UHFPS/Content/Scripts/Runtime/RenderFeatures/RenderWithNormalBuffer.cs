using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;

namespace UHFPS.Rendering
{
    public class RenderWithNormalBuffer : CustomPass
    {
        public LayerMask layerMask;

        ShaderTagId[] depthPrepassIds;
        ShaderTagId[] forwardIds;

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            depthPrepassIds = new ShaderTagId[] { new ShaderTagId("DepthForwardOnly"), new ShaderTagId("DepthOnly") };
            forwardIds = new ShaderTagId[] { new ShaderTagId("ForwardOnly"), new ShaderTagId("Forward"), new ShaderTagId("SRPDefaultUnlit") };
        }

        protected override void AggregateCullingParameters(ref ScriptableCullingParameters cullingParameters, HDCamera hdCamera)
            => cullingParameters.cullingMask |= (uint)layerMask.value;

        protected override void Execute(CustomPassContext ctx)
        {
            PerObjectData renderConfig = PerObjectData.LightProbe | PerObjectData.Lightmaps | PerObjectData.LightProbeProxyVolume;
            if (ctx.hdCamera.frameSettings.IsEnabled(FrameSettingsField.Shadowmask))
                renderConfig |= PerObjectData.OcclusionProbe | PerObjectData.OcclusionProbeProxyVolume | PerObjectData.ShadowMask;

            bool isDepthNormal = injectionPoint == CustomPassInjectionPoint.AfterOpaqueDepthAndNormal;
            var ids = isDepthNormal ? depthPrepassIds : forwardIds;

            var result = new RendererListDesc(ids, ctx.cullingResults, ctx.hdCamera.camera)
            {
                rendererConfiguration = renderConfig,
                renderQueueRange = GetRenderQueueRange(RenderQueueType.AllOpaque),
                sortingCriteria = SortingCriteria.CommonOpaque,
                excludeObjectMotionVectors = false,
                layerMask = layerMask,
            };

            if (isDepthNormal)
            {
                CoreUtils.SetRenderTarget(ctx.cmd, ctx.cameraNormalBuffer, ctx.cameraDepthBuffer, ClearFlag.None);
                CoreUtils.SetKeyword(ctx.cmd, "WRITE_NORMAL_BUFFER", true);
            }

            RendererList rendererList = ctx.renderContext.CreateRendererList(result);
            CoreUtils.DrawRendererList(ctx.cmd, rendererList);

            if (isDepthNormal)
            {
                CoreUtils.SetKeyword(ctx.cmd, "WRITE_NORMAL_BUFFER", ctx.hdCamera.frameSettings.litShaderMode == LitShaderMode.Forward);
            }
        }

        protected override void Cleanup()
        {
            // Cleanup code
        }
    }
}