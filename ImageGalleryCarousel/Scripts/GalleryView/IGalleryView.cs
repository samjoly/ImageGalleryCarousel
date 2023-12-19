/*
 * IGalleryView
 * 2023-12-08 - SJ
 *
 * SUMMARY:
 * Defines the essential functionalities for a gallery view. It allows for initialization with gallery data and navigation through the gallery items.
 *
 * FEATURES:
 * - Initialization with Gallery Data: Allows the interface to be initialized with specific gallery data.
 * - Navigation: Provides methods to navigate to the next and previous items in the gallery.
 *
 *
 */

namespace SJ.ImageGallery {
    /// <summary>
    ///     Represents a gallery view.
    /// </summary>
    public interface IGalleryView {
        /// <summary>
        ///     Initializes with specified data loader.
        /// </summary>
        /// <param name="galleryData">The data loader used to load the gallery data.</param>
        /// <remarks>
        ///     This method should be called before other operations
        /// </remarks>
        void Init(IDataLoader galleryData);

        /// <summary>
        ///     Advances the iterator to the next element.
        /// </summary>
        void Next();

        /// <summary>
        ///     Advances the iterator to the previous element
        /// </summary>
        void Prev();
    }
}