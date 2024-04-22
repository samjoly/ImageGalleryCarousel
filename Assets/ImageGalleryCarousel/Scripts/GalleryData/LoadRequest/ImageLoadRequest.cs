using System;
using System.IO;
using System.Threading;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Enumeration representing the different states of an image loading request.
    /// </summary>
    public enum ImageLoadRequestState {
        NOT_STARTED,
        READY,
        IN_PROGRESS,
        COMPLETED,
        CANCELLED
    }

    /// <summary>
    /// Represents an image loading request in the ImageLoader system, encapsulating details like URL, callbacks, priority,
    /// and cancellation. Designed for object pooling, this class includes methods for initialization and reset to enable
    /// efficient memory usage and dynamic updating of priorities and callbacks.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <strong>Key Features:</strong>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Encapsulation of image loading details and support for cancellation.</item>
    ///         <item>Object pooling ready with initialization and reset methods.</item>
    ///         <item>Dynamic updating of priorities and callbacks to accommodate changing needs during runtime.</item>
    ///     </list>
    /// </remarks>
    public class ImageLoadRequest {
        // Fields for managing the image load request
        public string imageUrl;
        public string imageName { get; private set; }
        public Action<Texture> onCompleteCallback;
        public Action<float> onProgressCallback;
        public LoadPriority priority;
        public CancellationTokenSource cancellationTokenSource;
        public float progress; // Tracks the progress of the image load operation
        public ImageLoadRequestState state; // Represents the current state of the image load operation

        // Initializes the request with necessary details
        /// <summary>
        /// Initializes the image load request with the specified parameters.
        /// </summary>
        /// <param name="url">The URL of the image to load.</param>
        /// <param name="onCompleteCallback">
        /// The callback to invoke when the image loading is complete. The loaded texture will be
        /// passed as a parameter.
        /// </param>
        /// <param name="onProgressCallback">
        /// The callback to invoke to report the progress of the image loading. The progress
        /// percentage will be passed as a parameter.
        /// </param>
        /// <param name="priority">The priority of the image load request.</param>
        public void Initialize(string url, Action<Texture> onCompleteCallback, Action<float> onProgressCallback,
            LoadPriority priority) {
            imageUrl = url;
            imageName = Path.GetFileName(url);
            this.onCompleteCallback = onCompleteCallback;
            this.onProgressCallback = onProgressCallback;
            this.priority = priority;
            cancellationTokenSource = new CancellationTokenSource();
            progress = 0.0f; // Initializing progress to 0
            state = ImageLoadRequestState.READY; // Setting initial state to NotStarted
        }

        /// <summary>
        /// Resets all the properties and variables to their default values.
        /// </summary>
        public void Reset() {
            imageUrl = null;
            onCompleteCallback = null;
            onProgressCallback = null;
            priority = LoadPriority.Low;
            if (cancellationTokenSource != null) {
                cancellationTokenSource.Cancel(); // Cancel any ongoing operations
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }

            progress = 0.0f; // Resetting progress
            state = ImageLoadRequestState.NOT_STARTED; // Resetting state to NotStarted
        }

        /// <summary>
        /// Updates the priority of the load.
        /// </summary>
        /// <param name="newPriority">The new priority of the load.</param>
        public void UpdatePriority(LoadPriority newPriority) {
            priority = newPriority;
        }

        /// <summary>
        /// Updates the callback action for Texture completion.
        /// </summary>
        /// <param name="newOnCompleteCallback">The new action to be invoked when the Texture is completed.</param>
        public void UpdateCallback(Action<Texture> newOnCompleteCallback) {
            onCompleteCallback = newOnCompleteCallback;
        }

        /// <summary>
        /// Updates the progress callback.
        /// </summary>
        /// <param name="newOnProgressCallback">The new progress callback.</param>
        public void UpdateProgressCallback(Action<float> newOnProgressCallback) {
            onProgressCallback = newOnProgressCallback;
        }

        /// <summary>
        /// Updates the progress of an image load request.
        /// </summary>
        /// <param name="val">The new progress value.</param>
        public void UpdateProgress(float val) {
            progress = val;
            state = val >= 1.0f ? ImageLoadRequestState.COMPLETED : ImageLoadRequestState.IN_PROGRESS;
            onProgressCallback?.Invoke(progress);
        }
    }
}