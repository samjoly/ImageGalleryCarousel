/*
 * GalleryDataLoader
 * 2023-12-08 - SJ
 *
 * SUMMARY:
 * Manages an image gallery in Unity, enabling asynchronous loading of images from a list or URLs.
 *
 * FEATURES:
 * - Asynchronous image loading
 * - Loading prioritization and cancellation
 * - Background loading support
 * - Progress and completion callbacks
 *
 * NOTES:
 * Need ImageListLoader component to load image list, attached to the same GameObject
 * Need ImageLoader component to load images, attached to the same GameObject
 *
 */


using System;
using UnityEngine;

namespace SJ.ImageGallery {
    public class GalleryDataLoader : MonoBehaviour, IDataLoader {
        #region Constants and Fields

        private IImageListLoader _imageListLoader;

        private IImageLoader _imageLoader;


        // TODO: remove the count callback and use the imageList.Length instead
        private Action _onDataListReady;

        #endregion

        #region Serialized Fields

        [SerializeField]
        private bool isBackgroundLoading;

        #endregion

        #region IDataLoader Implementation

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

        #endregion

        #region Unity Methods

        private void Awake() {
            _imageListLoader = GetComponent<IImageListLoader>();
            _imageLoader = GetComponent<IImageLoader>();
        }

        #endregion

        #region Private Methods

        private void OnImageListReceived(string[] images) {
            dataList = images;

            if (_onDataListReady != null) {
                _onDataListReady?.Invoke();
            }

            if (isBackgroundLoading) {
                _imageLoader.LoadImagesInBackground(images);
            }
        }

        #endregion
    }
}