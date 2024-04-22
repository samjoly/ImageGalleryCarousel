using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace SJ.ImageGallery {
    /// <summary>
    /// Item in a carousel.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Display carousel item with image and loading graphics.</item>
    ///         <item>Update item index and dynamically load content.</item>
    ///         <item>Show loading progress and handle content updates.</item>
    ///         <item>Debug mode to display index and progress text.</item>
    ///         <item>Reloading image logic if loading fails.</item>
    ///     </list>
    /// </Features>
    public class CarouselItem : MonoBehaviour, ICarouselItem {
        // indexText is useful to check if the current item content is correct
        public TextMeshPro indexText;
        public TextMeshPro progressText;
        [FormerlySerializedAs("photo")]
        public TextureApplier image;
        [FormerlySerializedAs("imageLoader")]
        [FormerlySerializedAs("photoLoader")]
        public LoadingGfx loading;

        private readonly int loadMaxCount = 3;
        // Max retry count for loading photo:
        private int loadCount;

        private void Awake() {
            StatePropertyList = GetComponents<IStateProperty>();
            if (image == null) {
                image = GetComponent<TextureApplier>();
            }
        }

        public IDataLoader galleryDataLoader { get; set; }

        /// <summary>
        /// Represents the index of a carousel item.
        /// </summary>
        public int Index { get; private set; } = -1;

        /// <inheritdoc />
        public bool IsDebugMode {
            set {
                indexText.gameObject.SetActive(value);
                progressText.gameObject.SetActive(value);
            }
        }
        /// <summary>
        /// Represents a list of state properties (such as transform, alpha,...) associated with a carousel item.
        /// </summary>
        public IStateProperty[] StatePropertyList { get; private set; }

        /// <inheritdoc />
        public void SetActive(bool val) {
            gameObject.SetActive(val);
        }

        /// <inheritdoc />
        public void UpdateIndex(int i) {
#if UNITY_EDITOR
            if (Application.isPlaying) {
                gameObject.name = $"item{i.ToString()}";
            }
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

        /// <summary>
        /// Method called to update photo loading progress
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
        /// Sets the photo on the image.
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

        private void LoadImage(int i) {
            if (gameObject.activeInHierarchy) {
                loadCount++;
                galleryDataLoader.GetContent(i, SetPhoto, OnLoadingPhotoProgress);
            }
        }
    }
}