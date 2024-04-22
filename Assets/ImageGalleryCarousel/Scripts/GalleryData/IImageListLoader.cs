using System;

namespace SJ.ImageGallery {
    /// <summary>
    /// Interface for loading a list of images.
    /// </summary>
    public interface IImageListLoader {
        /// <summary>
        /// Retrieves a list of image URLs asynchronously and calls the specified callback method upon completion.
        /// </summary>
        /// <param name="onCompleteCallback">The callback method to be called when the list of image URLs is retrieved.</param>
        void GetImageList(Action<string[]> onCompleteCallback);
    }
}