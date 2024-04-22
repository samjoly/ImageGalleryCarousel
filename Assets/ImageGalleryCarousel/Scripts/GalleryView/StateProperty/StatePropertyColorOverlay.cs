using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// State property for controlling color overlay alpha.
    /// </summary>
    public class StatePropertyColorOverlay : AStatePropertyShader {
        /// <inheritdoc />
        protected override string VALUE => "OverlayAlpha";
        /// <inheritdoc />
        protected override int SHADER_PROPERTY_ID => Shader.PropertyToID("_OverlayAlpha");
    }
}