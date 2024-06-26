using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Manages texture application to MeshRenderers.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Aspect ratio adjustment for texture application.</item>
    ///         <item>Option for fitting texture to holder's size.</item>
    ///         <item>Default texture handling and texture clearing.</item>
    ///     </list>
    /// </Features>
    public class TextureApplier : MonoBehaviour {
        public MeshRenderer imageHolder;

        public bool isFitToHolder = true;
        [SerializeField]
        private float holderAspectRatio;

        [SerializeField]
        private Vector3 originalScale;

        [SerializeField]
        private float textureAspectRatio;

        public Texture defaultTexture;

        private void Awake() {
            originalScale = imageHolder.transform.localScale;
            holderAspectRatio = originalScale.x / originalScale.y;
            if (imageHolder == null) {
                imageHolder = FindMeshRendererInChildren();
                if (imageHolder == null) {
                    Debug.LogError("ImageHolder (MeshRenderer) is not assigned and not found among children.");
                }
            }
        }

        /// <summary>
        /// Applies a texture to a mesh while maintaining the texture's aspect ratio.
        /// </summary>
        /// <param name="texture">The texture to apply.</param>
        public void ApplyTextureToMesh(Texture texture) {
            // return;
            if (!imageHolder) {
                Debug.LogError("ImageHolder (MeshRenderer) is not assigned.");
                return;
            }

            // If no texture is provided, use the default texture
            if (!texture) {
                texture = defaultTexture;
            }


            // Calculate the aspect ratio of the texture and the imageHolder
            textureAspectRatio = (float)texture.width / texture.height;

            // Determine whether to scale based on width or height
            if (isFitToHolder ? textureAspectRatio > holderAspectRatio : textureAspectRatio < holderAspectRatio) {
                // Texture is wider than the imageHolder, so scale height
                var newHeight = originalScale.x / textureAspectRatio;
                imageHolder.transform.localScale = new Vector3(originalScale.x, newHeight, originalScale.z);
            } else {
                // Texture is taller than the imageHolder, so scale width
                var newWidth = originalScale.y * textureAspectRatio;
                imageHolder.transform.localScale = new Vector3(newWidth, originalScale.y, originalScale.z);
            }

            // Apply the texture
            imageHolder.material.mainTexture = texture;
        }

        /// <summary>
        /// Clears the image held by the <paramref name="imageHolder" />.
        /// </summary>
        public void Clear() {
            if (!imageHolder.material || !imageHolder.material.mainTexture) {
                return;
            }

            imageHolder.transform.localScale = originalScale;

            // Create a new 1x1 texture
            var emptyTexture = new Texture2D(1, 1);
            // Set the pixel to transparent (or any other color)
            // newTexture.SetPixel(0, 0, Color.clear);
            emptyTexture.SetPixel(0, 0, Color.white);
            // Apply all SetPixel calls
            emptyTexture.Apply();

            imageHolder.material.mainTexture = emptyTexture;
        }

        private MeshRenderer FindMeshRendererInChildren() {
            foreach (Transform child in transform) {
                var meshRenderer = child.GetComponent<MeshRenderer>();
                if (meshRenderer != null) {
                    return meshRenderer;
                }
            }

            return null;
        }
    }
}