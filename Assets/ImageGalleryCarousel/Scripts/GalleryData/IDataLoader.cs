using System;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Data loader that retrieves content asynchronously.
    /// </summary>
    public interface IDataLoader {
        /// <summary>
        /// Retrieves the content at the specified index asynchronously.
        /// </summary>
        /// <param name="index">The index of the content to retrieve.</param>
        /// <param name="onCompleteCallback">
        /// The callback function to be called when the content is retrieved successfully.
        /// The retrieved content will be passed as a parameter of type Texture.
        /// </param>
        /// <param name="onProgressCallback">
        /// The callback function to be called periodically during the retrieval process.
        /// The current progress, represented as a floating-point value between 0 and 1%, will be passed as a parameter of type
        /// float.
        /// </param>
        /// <param name="priority">The priority level of the retrieval process.</param>
        /// <returns>None</returns>
        void GetContent(int index, Action<Texture> onCompleteCallback, Action<float> onProgressCallback,
            LoadPriority priority = LoadPriority.High);

        /// <summary>
        /// Cancels the operation at the specified index.
        /// </summary>
        /// <param name="index">The index of the operation to cancel.</param>
        void Cancel(int index);

        /// <summary>
        /// Retrieves a list of content items asynchronously and invokes the specified callback when the operation is complete.
        /// </summary>
        /// <param name="onDataListReady">A callback function that accepts an integer as a parameter.</param>
        void GetContentList(Action onDataListReady);

        /// <summary>
        /// List of data that can be loaded
        /// </summary>
        string[] dataList { get; set; }
    }
}