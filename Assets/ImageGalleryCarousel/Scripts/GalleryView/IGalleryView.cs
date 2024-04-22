namespace SJ.ImageGallery {
    /// <summary>
    /// Defines essential functionalities for a gallery view, including initialization with gallery data and item navigation.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Initialization with gallery data.</item>
    ///         <item>Navigation methods for next and previous gallery items.</item>
    ///     </list>
    /// </Features>
    public interface IGalleryView {
        /// <summary>
        /// Initializes with specified data loader.
        /// </summary>
        /// <param name="galleryData">The data loader used to load the gallery data.</param>
        /// <remarks>
        /// This method should be called before other operations
        /// </remarks>
        void Init(IDataLoader galleryData);

        /// <summary>
        /// Advances the iterator to the next element.
        /// </summary>
        void Next();

        /// <summary>
        /// Advances the iterator to the previous element
        /// </summary>
        void Prev();
    }
}