using System;
using System.Collections;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Provides asynchronous texture loading and caching in Unity, supporting concurrent loading with progress and cancellation callbacks.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Asynchronous and concurrent image loading with callbacks for progress and cancellation.</item>
    ///         <item>Priority-based request queue to manage loading precedence.</item>
    ///         <item>LRU strategy for efficient caching to optimize memory usage.</item>
    ///         <item>Object pooling to minimize memory allocation and reduce GC overhead.</item>
    ///         <item>Background loading and AdaptiveLoader integration for performance optimization under varying conditions.</item>
    ///     </list>
    /// </Features>
    public class ImageLoaderResources : AImageLoader {
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
    }
}