namespace SJ.ImageGallery {
    /// <summary>
    /// Interface for carousel item functionalities, including methods for index updating, content loading, and progress display.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Contract for index management and active state of carousel items.</item>
    ///         <item>Debug mode support for development and testing.</item>
    ///         <item>State properties list for functionality extension.</item>
    ///         <item>Integration with GalleryDataLoader for content management.</item>
    ///     </list>
    /// </Features>
    public interface ICarouselItem {
        int Index { get; }
        /// <summary>
        /// Gets or sets a value indicating whether the application is running in debug mode.
        /// </summary>
        bool IsDebugMode { set; }
        IStateProperty[] StatePropertyList { get; }

        /// <summary>
        /// Gets or sets the GalleryDataLoader object.
        /// </summary>
        IDataLoader galleryDataLoader { get; set; }

        /// <summary>
        /// Updates to set content's index
        /// </summary>
        /// <param name="i">The value to update the index with.</param>
        void UpdateIndex(int i);

        /// <summary>
        /// Sets the active/visible state of the object.
        /// </summary>
        /// <param name="val">The desired active/visible state.</param>
        void SetActive(bool val);
    }
}