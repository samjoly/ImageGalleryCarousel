/*
 * ICarouselItem
 * 11/19/2023 - SJ
 *
 * SUMMARY:
 * Interface defining the essential functionalities of an item in a carousel.
 * It includes methods for updating the item index, loading content, and displaying
 * progress, as well as properties for state management and debugging.
 *
 * FEATURES:
 * - Define contract for carousel items including index management and active state.
 * - Support for debug mode to enhance development and testing.
 * - Maintain a list of state properties for extended functionalities.
 * - Integrate with GalleryDataLoader for dynamic content management.
 *
 */

namespace SJ.ImageGallery {
    public interface ICarouselItem {
        int Index { get; }
        /// <summary>
        ///     Gets or sets a value indicating whether the application is running in debug mode.
        /// </summary>
        bool IsDebugMode { set; }
        IStateProperty[] StatePropertyList { get; }

        /// <summary>
        ///     Gets or sets the GalleryDataLoader object.
        /// </summary>
        IDataLoader galleryDataLoader { get; set; }

        #region Actions

        /// <summary>
        ///     Updates to set content's index
        /// </summary>
        /// <param name="i">The value to update the index with.</param>
        void UpdateIndex(int i);

        /// <summary>
        ///     Sets the active/visible state of the object.
        /// </summary>
        /// <param name="val">The desired active/visible state.</param>
        void SetActive(bool val);

        #endregion
    }
}