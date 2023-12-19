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
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Provides functionality to load images asynchronously and cache them for future use.
    /// </summary>
    public class ImageLoaderServer : AImageLoader {
        #region Constants and Fields

        private AdaptiveLoader _adaptiveLoader;

        #endregion

        #region Protected Methods

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

        #endregion

        #region Unity Methods

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

        #endregion
    }
}