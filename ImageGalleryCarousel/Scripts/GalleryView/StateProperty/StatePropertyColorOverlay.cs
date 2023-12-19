/*
 * StatePropertyColorOverlay
 * 2023-11-29 - SJ
 *
 * SUMMARY:
 * Represents a state property for controlling color overlay alpha.
 *
 */

using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Represents a state property for controlling color overlay alpha.
    /// </summary>
    public class StatePropertyColorOverlay : AStatePropertyShader {
        #region Protected Properties

        /// <inheritdoc />
        protected override string VALUE => "OverlayAlpha";
        /// <inheritdoc />
        protected override int SHADER_PROPERTY_ID => Shader.PropertyToID("_OverlayAlpha");

        #endregion
    }
}