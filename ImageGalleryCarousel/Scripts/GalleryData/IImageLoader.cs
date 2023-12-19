/*
 * IImageLoader
 * 2023-12-08 - SJ
 *
 * SUMMARY:
 * Interface for loading images.
 *
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Interface for loading images.
    /// </summary>
    public interface IImageLoader {
        /// <summary>
        ///     Cancels a request made to a specified URL.
        /// </summary>
        /// <param name="url">The URL of the request to cancel.</param>
        void Cancel(string url);

        /// <summary>
        ///     Cancels all ongoing operations.
        /// </summary>
        void CancelAll();

        /// <summary>
        ///     Loads an image from the given imageUrl asynchronously.
        /// </summary>
        /// <param name="path">The URL of the image to load.</param>
        /// <param name="onCompleteCallback">
        ///     The callback method to be called when the image has finished loading.
        ///     An instance of Texture will be passed as a parameter to the callback.
        /// </param>
        /// <param name="onProgressCallback">
        ///     The callback method to be called periodically to update the loading progress.
        ///     A float value between 0 and 1% will be passed as a parameter to the callback.
        /// </param>
        /// <param name="priority">The priority level at which the image should be loaded.</param>
        void LoadImage(string path, Action<Texture> onCompleteCallback, Action<float> onProgressCallback,
            LoadPriority priority = LoadPriority.Medium);

        /// <summary>
        ///     Retrieves a list of all active image load requests.
        /// </summary>
        /// <returns>
        ///     A <see cref="List{ImageLoadRequest}" /> containing all active image load requests.
        /// </returns>
        List<ImageLoadRequest> GetCurrentActiveRequests();

        int simultaneousLoadingCount { get; set; }

        /// <summary>
        ///     Loads the given images in the background.
        /// </summary>
        /// <param name="images">An array of strings representing the image paths.</param>
        void LoadImagesInBackground(string[] images);
    }
}