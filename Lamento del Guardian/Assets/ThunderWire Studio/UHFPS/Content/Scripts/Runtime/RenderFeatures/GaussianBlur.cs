using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Experimental.Rendering;

namespace UHFPS.Rendering
{
    public class GaussianBlur : CustomPass
    {
        [Range(0f, 1f)] public float BlurWeight = 1f;
        public float BlurRadius = 8f;
        public int SampleCount = 9;

        RTHandle temp;

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            temp = RTHandles.Alloc(
                Vector2.one,
                TextureXR.slices,
                dimension: TextureXR.dimension,
                colorFormat: GraphicsFormat.B10G11R11_UFloatPack32,
                useDynamicScale: true,
                name: "GaussianBlurTemp"
            );
        }

        protected override void Execute(CustomPassContext ctx)
        {
            if (ctx.hdCamera.camera.cameraType == CameraType.SceneView)
                return;
            
            if (BlurWeight <= 0.0001f || BlurRadius <= 0.0001f || SampleCount <= 1)
                return;

            float radius = BlurRadius * BlurWeight;
            radius *= ctx.cameraColorBuffer.rtHandleProperties.rtHandleScale.x;
            
            if (radius <= 0.0001f)
                return;

            CustomPassUtils.GaussianBlur(
                ctx,
                ctx.cameraColorBuffer,
                ctx.cameraColorBuffer,
                temp,
                SampleCount,
                radius,
                downSample: false
            );
        }

        protected override void Cleanup()
        {
            temp?.Release();
        }
    }
}