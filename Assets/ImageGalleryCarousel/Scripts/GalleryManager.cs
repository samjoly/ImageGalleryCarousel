using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Manages the gallery by loading data and initializing the view.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Retrieves and initializes gallery data.</item>
    ///         <item>Connects IDataLoader with IGalleryView for data integration.</item>
    ///         <item>Handles initialization and setup of gallery view.</item>
    ///     </list>
    /// </Features>
    public class GalleryManager : MonoBehaviour {
        private IDataLoader _galleryData;
        private IGalleryView _galleryView;

        private void Awake() {
            _galleryView = GetComponentInChildren<IGalleryView>();
            _galleryData = GetComponentInChildren<IDataLoader>();
        }

        private void Start() {
            _galleryData.GetContentList(OnDataListReady);
        }

        // Shorthand
        public void Next() {
            _galleryView.Next();
        }

        public void Prev() {
            _galleryView.Prev();
        }

        private void OnDataListReady() {
            _galleryView.Init(_galleryData);
        }
    }
}