/*
 * StatePropertyAlpha
 * 2023-11-27 - SJ
 *
 * SUMMARY:
 * Represents a state property for alpha value in a shader.
 *
 * NOTES:
 * Alpha render-queue Can be challenging.
 *
 */

using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Represents a state property for alpha value in a shader.
    /// </summary>
    public class StatePropertyAlpha : AStatePropertyShader {
        #region Protected Properties

        /// <inheritdoc />
        protected override string VALUE => "Alpha";

        /// <inheritdoc />
        protected override int SHADER_PROPERTY_ID => Shader.PropertyToID("_Alpha");

        #endregion
    }
}