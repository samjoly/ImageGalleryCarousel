using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// State property for alpha value in a shader.
    /// </summary>
    /// <note>
    /// Alpha render-queue Can be challenging.
    /// </note>
    public class StatePropertyAlpha : AStatePropertyShader {
        /// <inheritdoc />
        protected override string VALUE => "Alpha";

        /// <inheritdoc />
        protected override int SHADER_PROPERTY_ID => Shader.PropertyToID("_Alpha");
    }
}