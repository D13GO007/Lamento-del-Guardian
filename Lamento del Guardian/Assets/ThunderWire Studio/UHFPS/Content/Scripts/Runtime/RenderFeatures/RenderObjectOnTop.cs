using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.RendererUtils;

namespace UHFPS.Runtime
{
    public class RenderObjectOnTop : CustomPass
    {
        [SerializeField] private LayerMask renderOnTopLayer;
        [SerializeField] private bool clearDepth;
        [SerializeField] private bool overrideDepth;
        
        ShaderTagId[] colorPassTags;

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            colorPassTags = new[]
            {
                new ShaderTagId("ForwardOnly"),
                new ShaderTagId("Forward")
            };
        }

        protected override void Execute(CustomPassContext ctx)
        {
            if (clearDepth)
            {
                CoreUtils.ClearRenderTarget(ctx.cmd, ClearFlag.Depth, Color.clear);
            }

            var colorDesc = new RendererListDesc(colorPassTags, ctx.cullingResults, ctx.hdCamera.camera)
            {
                layerMask = renderOnTopLayer,
                renderQueueRange = RenderQueueRange.all,
                sortingCriteria = SortingCriteria.CommonOpaque,
            };
            
            if (overrideDepth)
            {
                colorDesc.stateBlock = new RenderStateBlock(RenderStateMask.Depth)
                {
                    depthState = new DepthState(true, CompareFunction.LessEqual)
                };
            }

            var list = ctx.renderContext.CreateRendererList(colorDesc);
            ctx.cmd.DrawRendererList(list);
        }
    }
}