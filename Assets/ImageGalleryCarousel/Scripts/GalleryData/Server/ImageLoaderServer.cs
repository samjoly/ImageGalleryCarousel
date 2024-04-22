using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace SJ.ImageGallery {
    /// <summary>
    /// Class providing asynchronous texture loading and caching,
    /// supporting concurrent loading with progress and cancellation callbacks.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Asynchronous and concurrent image loading with callbacks.</item>
    ///         <item>Priority-based request queue.</item>
    ///         <item>LRU strategy for efficient caching.</item>
    ///         <item>Object pooling to minimize memory allocation.</item>
    ///         <item>Background loading and AdaptiveLoader integration for performance optimization.</item>
    ///     </list>
    /// </Features>
    public class ImageLoaderServer : AImageLoader {
        private AdaptiveLoader _adaptiveLoader;

        private void Awake() {
            _adaptiveLoader = GetComponent<AdaptiveLoader>();
        }

        private void Start() {
            _adaptiveLoader?.Init(this);
        }

        private void Update() {
            lock (_queueLock) {
                if (_isLoaderActive) {
                    _adaptiveLoader?.Update();
                }
            }
        }

        private void OnDestroy() {
            _adaptiveLoader?.Cleanup();
        }

        /// <summary>
        /// Loads an image asynchronously using a coroutine.
        /// </summary>
        /// <param name="request">The ImageLoadRequest that contains the image URL.</param>
        /// <returns>The coroutine enumerator.</returns>
        protected override IEnumerator LoadImageCoroutine(ImageLoadRequest request) {
            if (!Uri.IsWellFormedUriString(request.imageUrl, UriKind.Absolute)) {
                Debug.LogError("Invalid URL: " + request.imageUrl);

                OnLoadImageCoroutineComplete(request);
                yield break;
            }

            _adaptiveLoader?.BeginDownloadMeasurement();

            _isLoaderActive = true;
            request.state = ImageLoadRequestState.IN_PROGRESS;

            using (var uwr = UnityWebRequestTexture.GetTexture(request.imageUrl)) {
                var operation = uwr.SendWebRequest();


                while (!operation.isDone) {
                    if (request.cancellationTokenSource == null ||
                        request.cancellationTokenSource.Token.IsCancellationRequested) {
                        uwr.Abort();
                        OnLoadImageCoroutineComplete(request);
                        yield break;
                    }

                    request.UpdateProgress(operation.progress);
                    yield return null;
                }

                try {
                    if (uwr.result == UnityWebRequest.Result.Success) {
                        Texture texture = DownloadHandlerTexture.GetContent(uwr);
                        AddToCache(request.imageUrl, texture);
                        request.onCompleteCallback?.Invoke(texture);
                        request.state = ImageLoadRequestState.COMPLETED;
                        _adaptiveLoader?.EndDownloadMeasurement(uwr.downloadedBytes);
                    } else {
                        Debug.LogWarning("Image load failed: " + uwr.error + " - " + request.imageUrl);
                        request.onCompleteCallback?.Invoke(null);
                        request.state = ImageLoadRequestState.CANCELLED;
                        _adaptiveLoader?.EndDownloadMeasurement(0);

                        // Check for Curl error 56
                        if (uwr.error.Contains("56")) {
                            Debug.LogError("Curl error 56 occurred: Receiving data failed.");
                            // Implement specific handling for Curl error 56 here
                        }

                        _requestQueue.Add(request);
                    }
                } catch (Exception ex) {
                    request.onCompleteCallback?.Invoke(null);
                    Debug.LogError($"Error in LoadImageCoroutine: {ex.Message} - {request.imageUrl}");
                    _requestQueue.Add(request);
                } finally {
                    OnLoadImageCoroutineComplete(request);
                }
            }
        }
    }
}