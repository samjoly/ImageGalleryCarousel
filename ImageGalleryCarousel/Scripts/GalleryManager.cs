/*
 * GalleryManager
 * 2023-11-30 - SJ
 *
 * SUMMARY:
 * Manages the initialization and coordination of gallery components within SJ.ImageGallery.
 * It connects the gallery view with the data loader to facilitate the display of gallery content.
 *
 * FEATURES:
 * - Retrieves and initializes gallery data.
 * - Connects IDataLoader with IGalleryView for seamless data integration.
 * - Handles initialization and setup of gallery view.
 */

using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Manages the gallery by loading data and initializing the view.
    /// </summary>
    public class GalleryManager : MonoBehaviour {
        #region Constants and Fields

        private IDataLoader _galleryData;

        private IGalleryView _galleryView;

        #endregion

        #region Serialized Fields


        #endregion

        #region Unity Methods

        private void Awake() {
            _galleryView = GetComponentInChildren<IGalleryView>();
            _galleryData = GetComponentInChildren<IDataLoader>();
        }

        private void Start() {
            _galleryData.GetContentList(OnDataListReady);
        }

        #endregion

        #region Private Methods

        private void OnDataListReady() {
            _galleryView.Init(_galleryData);
        }

        #endregion
    }
}