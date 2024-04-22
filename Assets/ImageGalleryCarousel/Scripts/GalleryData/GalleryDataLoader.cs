using System;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Manages an image gallery in Unity, enabling asynchronous loading of images from a list of URLs.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Asynchronous image loading.</item>
    ///         <item>Loading prioritization and cancellation.</item>
    ///         <item>Background loading support.</item>
    ///         <item>Progress and completion callbacks.</item>
    ///     </list>
    /// </Features>
    /// <note>
    /// Requires ImageListLoader component to load image list, attached to the same GameObject
    /// Requires ImageLoader component to load images, attached to the same GameObject
    /// </note>
    public class GalleryDataLoader : MonoBehaviour, IDataLoader {
        [SerializeField]
        private bool isBackgroundLoading;

        private IImageListLoader _imageListLoader;

        private IImageLoader _imageLoader;

        // TODO: remove the count callback and use the imageList.Length instead
        private Action _onDataListReady;

        private void Awake() {
            _imageListLoader = GetComponent<IImageListLoader>();
            _imageLoader = GetComponent<IImageLoader>();
        }

        /// <inheritdoc />
        public string[] dataList { get; set; }

        /// <inheritdoc />
        public void Cancel(int index) {
            _imageLoader.Cancel(dataList[index]);
        }

        /// <inheritdoc />
        public void GetContent(int index, Action<Texture> onCompleteCallback, Action<float> onProgressCallback,
            LoadPriority priority = LoadPriority.High) {
            _imageLoader.LoadImage(dataList[index], onCompleteCallback, onProgressCallback, priority);
        }

        /// <inheritdoc />
        public void GetContentList(Action onDataListReady) {
            _onDataListReady = onDataListReady;

            // Call GetImageList and pass a method to handle the callback.
            _imageListLoader.GetImageList(OnImageListReceived);
        }

        private void OnImageListReceived(string[] images) {
            dataList = images;

            if (_onDataListReady != null) {
                _onDataListReady?.Invoke();
            }

            if (isBackgroundLoading) {
                _imageLoader.LoadImagesInBackground(images);
            }
        }
    }
}