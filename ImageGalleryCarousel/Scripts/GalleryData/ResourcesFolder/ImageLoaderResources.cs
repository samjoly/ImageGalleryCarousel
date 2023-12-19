/*
 * ImageLoaderServer
 * 2023-12-07 - SJ
 *
 * NOTES:
 * Summary:
 * The ImageLoader class in Unity provides asynchronous texture loading and caching, supporting concurrent loading with progress and cancellation callbacks. It features a priority-based queue, an LRU cache (Least Recently Used), and an object pooling mechanism for efficiency. Background loading and integration with AdaptiveLoader for dynamic performance tuning are also included.
 *
 * Key Features:
 * - Asynchronous and concurrent image loading with callbacks.
 * - Priority-based request queue.
 * - LRU strategy for efficient caching.
 * - Object pooling to minimize memory allocation.
 * - Background loading and AdaptiveLoader integration for performance optimization.
 *
 * TO DO:
 *
 *
 */

using System;
using System.Collections;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Provides functionality to load images from Resources folder asynchronously and cache them for future use.
    /// </summary>
    public class ImageLoaderResources : AImageLoader {
        #region Protected Methods

        // This method is used to load images asynchronously. It is called by the ProcessQueue method.


        protected override IEnumerator LoadImageCoroutine(ImageLoadRequest request) {
            _isLoaderActive = true;
            request.state = ImageLoadRequestState.IN_PROGRESS;

            // Start loading the texture from the Resources folder
            var resourceRequest = Resources.LoadAsync<Texture>(request.imageUrl);

            while (!resourceRequest.isDone) {
                request.UpdateProgress(resourceRequest.progress);
                yield return null;
            }

            try {
                if (resourceRequest.asset is Texture texture) {
                    AddToCache(request.imageUrl, texture);
                    request.onCompleteCallback?.Invoke(texture);
                    request.state = ImageLoadRequestState.COMPLETED;
                } else {
                    Debug.LogWarning("Image load failed: " + request.imageUrl);
                    request.onCompleteCallback?.Invoke(null);
                    request.state = ImageLoadRequestState.CANCELLED;
                }
            } catch (Exception ex) {
                Debug.LogError($"Error in LoadImageCoroutine: {ex.Message} - {request.imageUrl}");
                request.onCompleteCallback?.Invoke(null);
            } finally {
                OnLoadImageCoroutineComplete(request);
            }
        }

        #endregion
    }
}