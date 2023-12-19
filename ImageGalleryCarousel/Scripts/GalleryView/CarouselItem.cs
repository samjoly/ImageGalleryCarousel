/*
 * CarouselPhoto
 * 2023-11-27 - SJ :
 *
 * SUMMARY
 * Represents an item in a carousel. This class provides the functionality for loading and displaying images in a carousel view, including progress tracking and dynamic content updates.
 *
 * FEATURES:
 * - Display carousel item with image and loading graphics.
 * - Update item index and dynamically load content.
 * - Show loading progress and handle content updates.
 * - Debug mode to display index and progress text.
 * - Reloading image logic if loading fail
 *
 * NOTES:
 * Attached to prefab
 *
 * UPDATE:
 * Added loadCount to reload image if loading fail
 *
 */


using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Represents an item in a carousel.
    /// </summary>
    public class CarouselItem : MonoBehaviour, ICarouselItem {
        #region Constants and Fields

        private readonly int loadMaxCount = 3;

        // Max retry count for loading photo:
        private int loadCount;

        #endregion

        #region Serialized Fields

        // indexText is useful to check if the current item content is correct
        public TextMeshPro indexText;
        public TextMeshPro progressText;
        [FormerlySerializedAs("photo")]
        public TextureApplier image;
        [FormerlySerializedAs("imageLoader")]
        [FormerlySerializedAs("photoLoader")]
        public LoadingGfx loading;

        #endregion

        #region ICarouselItem Implementation

        public IDataLoader galleryDataLoader { get; set; }

        public int Index { get; private set; } = -1;
        /// <inheritdoc />
        public bool IsDebugMode {
            set {
                indexText.gameObject.SetActive(value);
                progressText.gameObject.SetActive(value);
            }
        }
        public IStateProperty[] StatePropertyList { get; private set; }

        /// <inheritdoc />
        public void SetActive(bool val) {
            gameObject.SetActive(val);
        }

        /// <inheritdoc />
        public void UpdateIndex(int i) {
#if UNITY_EDITOR
            gameObject.name = $"item{i.ToString()}";
#endif
            if (Application.isPlaying) {
                if (loading) {
                    loading.OnStart();
                }

                image.Clear();
                if (galleryDataLoader != null) {
                    if (Index != -1 && Index != i) {
                        galleryDataLoader.Cancel(Index);
                        loadCount = 0;
                    }

                    LoadImage(i);
                }
            }

            Index = i;
            indexText.text = i.ToString();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Method called to update photo loading progress
        /// </summary>
        /// <param name="progress">The current progress of the photo loading, ranging from 0 to 1%</param>
        public void OnLoadingPhotoProgress(float progress) {
            var val = (int)(progress * 100);
            if (progressText) {
                progressText.text = $"{val}%";
            }

            if (loading) {
                loading.OnProgress(progress);
            }
        }

        /// <summary>
        ///     Sets the photo on the image.
        /// </summary>
        /// <param name="texture">The texture to be applied to the mesh.</param>
        public void SetPhoto(Texture texture) {
            if (texture == null && loadCount < loadMaxCount) {
                LoadImage(Index);
                return;
            }

            image.ApplyTextureToMesh(texture);
            if (loading) {
                loading.OnComplete();
            }
        }

        #endregion

        #region Unity Methods

        private void Awake() {
            StatePropertyList = GetComponents<IStateProperty>();
            if (image == null) {
                image = GetComponent<TextureApplier>();
            }
        }

        #endregion

        #region Private Methods

        private void LoadImage(int i) {
            if (gameObject.activeInHierarchy) {
                loadCount++;
                galleryDataLoader.GetContent(i, SetPhoto, OnLoadingPhotoProgress);
            }
        }

        #endregion
    }
}
