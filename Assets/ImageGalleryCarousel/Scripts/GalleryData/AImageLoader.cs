using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Abstract base class for image loading that manages the queuing, processing, and caching of image requests.
    /// It ensures thread-safe operations for concurrent access to resources and provides functionality to cancel requests and manage loading priorities.
    /// </summary>
    public abstract class AImageLoader : MonoBehaviour, IImageLoader {
        /// <summary>
        /// Maximum number of image load requests that can be queued at once.
        /// </summary>
        public int maxQueueSize = 50;

        /// <summary>
        /// Cache for storing loaded textures to avoid reloading.
        /// </summary>
        protected readonly ConcurrentDictionary<string, Texture> _cache = new();

        /// <summary>
        /// Cache to track the last access times of textures for cache eviction purposes.
        /// </summary>
        protected readonly ConcurrentDictionary<string, DateTime> _cacheLastAccess = new();

        /// <summary>
        /// Set of URLs currently being processed to prevent duplicate requests.
        /// </summary>
        protected readonly HashSet<string> _currentLoadingRequests = new();

        /// <summary>
        /// List of ImageLoadRequest objects representing the requests that are currently being processed.
        /// </summary>
        protected readonly List<ImageLoadRequest> _loadingQueue = new();

        /// <summary>
        /// LinkedList to maintain the least recently used (LRU) order of cache entries.
        /// </summary>
        protected readonly LinkedList<string> _lruList = new();

        /// <summary>
        /// Lock used for thread-safe manipulation of LRU list.
        /// </summary>
        protected readonly object _lruLock = new();

        /// <summary>
        /// Lock used as a synchronization mechanism to ensure thread safety when accessing or modifying the request queues.
        /// </summary>
        protected readonly object _queueLock = new();

        /// <summary>
        /// Pool of ImageLoadRequest objects used to reuse request instances, reducing overhead of object creation and garbage collection.
        /// </summary>
        protected readonly ImageLoadRequestPool _requestPool = new();

        /// <summary>
        /// List of ImageLoadRequest objects that are queued up and waiting to be processed.
        /// </summary>
        protected readonly List<ImageLoadRequest> _requestQueue = new();

        /// <summary>
        /// Flag indicating whether the loader is actively processing any requests.
        /// </summary>
        protected bool _isLoaderActive;

        /// <summary>
        /// Maximum number of textures that can be stored in the cache.
        /// </summary>
        protected int maxCacheSize = 100;

        /// <inheritdoc />
        public int simultaneousLoadingCount { get; set; } = 3;

        /// <inheritdoc />
        public void Cancel(string url) {
            lock (_queueLock) {
                var foundAndCanceled = false;
                foreach (var request in _requestQueue) {
                    if (request.imageUrl == url && request.state != ImageLoadRequestState.IN_PROGRESS) {
                        request.cancellationTokenSource.Cancel();
                        _requestQueue.Remove(request);
                        _requestPool.ReturnRequestToPool(request);
                        foundAndCanceled = true;
                        break;
                    }
                }

                if (!foundAndCanceled) {
                    foreach (var request in _loadingQueue) {
                        if (request.imageUrl == url) {
                            request.cancellationTokenSource.Cancel();
                            break;
                        }
                    }
                }

                _currentLoadingRequests.Remove(url);
            }
        }

        /// <inheritdoc />
        public void CancelAll() {
            lock (_queueLock) {
                foreach (var request in _requestQueue) {
                    request.cancellationTokenSource.Cancel();
                    _requestPool.ReturnRequestToPool(request);
                }

                _requestQueue.Clear();

                foreach (var request in _loadingQueue) {
                    request.cancellationTokenSource.Cancel();
                }

                _currentLoadingRequests.Clear();
            }
        }

        /// <inheritdoc />
        public List<ImageLoadRequest> GetCurrentActiveRequests() {
            lock (_queueLock) {
                var activeRequests = new List<ImageLoadRequest>();
                activeRequests.AddRange(_requestQueue);
                activeRequests.AddRange(_loadingQueue);
                return activeRequests;
            }
        }

        /// <inheritdoc />
        public void LoadImage(string path, Action<Texture> onCompleteCallback,
            Action<float> onProgressCallback,
            LoadPriority priority = LoadPriority.Medium) {
            if (_cache.TryGetValue(path, out var cachedTexture)) {
                onProgressCallback?.Invoke(1f); // Report 100% progress for cached images
                onCompleteCallback?.Invoke(cachedTexture);
                return;
            }

            lock (_queueLock) {
                if (!_currentLoadingRequests.Contains(path)) {
                    if (_requestQueue.Count >= maxQueueSize) {
                        var oldestRequest = _requestQueue[0];
                        _requestQueue.RemoveAt(0);
                        oldestRequest.cancellationTokenSource.Cancel();
                        Debug.LogWarning($"Oldest load request removed to make space: {oldestRequest.imageUrl}");
                    }

                    var request = _requestPool.GetPooledRequest();
                    request.Initialize(path, onCompleteCallback, onProgressCallback, priority);
                    _requestQueue.Add(request);
                    _currentLoadingRequests.Add(path);
                    ProcessQueue();
                } else {
                    var updated = TryUpdateRequest(_requestQueue, path, onCompleteCallback, onProgressCallback,
                        priority);
                    TryUpdateRequest(_loadingQueue, path, onCompleteCallback, onProgressCallback, priority);
                    if (updated) {
                        _requestQueue.Sort((a, b) => b.priority.CompareTo(a.priority));
                    }
                }
            }
        }

        /// <inheritdoc />
        public void LoadImagesInBackground(string[] backgroundImages) {
            foreach (var imageUrl in backgroundImages) {
                if (!_cache.ContainsKey(imageUrl)) {
                    LoadImage(imageUrl, null, null, LoadPriority.Low);
                }
            }
        }

        /// <summary>
        /// Adds an image to the cache.
        /// </summary>
        /// <param name="imageUrl">URL of the image.</param>
        /// <param name="texture">Texture to cache.</param>
        protected void AddToCache(string imageUrl, Texture texture) {
            lock (_lruLock) {
                while (_cache.Count >= maxCacheSize) {
                    var oldestKey = _lruList.Last.Value;
                    _cache.TryRemove(oldestKey, out _);
                    _cacheLastAccess.TryRemove(oldestKey, out _);
                    _lruList.RemoveLast();
                }

                _cache[imageUrl] = texture;
                _cacheLastAccess[imageUrl] = DateTime.Now;
                _lruList.AddFirst(imageUrl);
            }
        }

        /// <inheritdoc />
        protected abstract IEnumerator LoadImageCoroutine(ImageLoadRequest request);

        /// <summary>
        /// Handles the completion of an image load coroutine, updating the request queues and recycling the completed request.
        /// </summary>
        /// <param name="request">The completed image load request.</param>
        protected void OnLoadImageCoroutineComplete(ImageLoadRequest request) {
            _loadingQueue.Remove(request);
            _currentLoadingRequests.Remove(request.imageUrl);
            _requestPool.ReturnRequestToPool(request);
            ProcessQueue();
        }

        /// <summary>
        /// Processes the image load queue, starting new loads if capacity allows. Manages priority and ensures maximum loading efficiency.
        /// </summary>
        protected void ProcessQueue() {
            if (_requestQueue.Count == 0 && _loadingQueue.Count == 0) {
                _isLoaderActive = false;
                return;
            }

            var loadingQueueTemp = new List<ImageLoadRequest>();
            var lowPriorityCount = 0;
            var maxLowPriorityLoads = simultaneousLoadingCount / 2; // Example: half of the slots for low priority

            lock (_queueLock) {
                _requestQueue.Sort((a, b) => b.priority.CompareTo(a.priority));

                foreach (var request in _requestQueue) {
                    if (_loadingQueue.Count >= simultaneousLoadingCount) {
                        break; // Stop if the active load count reaches the limit
                    }

                    if (request.priority == LoadPriority.Low && lowPriorityCount >= maxLowPriorityLoads) {
                        continue; // Skip low priority requests if limit is reached
                    }

                    if (request.priority == LoadPriority.Low) {
                        lowPriorityCount++;
                    }

                    loadingQueueTemp.Add(request);
                }

                foreach (var request in loadingQueueTemp) {
                    _requestQueue.Remove(request);
                    _loadingQueue.Add(request);
                    StartCoroutine(LoadImageCoroutine(request));
                }
            }
        }

        /// <summary>
        /// Tries to update an existing request with new parameters. This method is called to adjust the priority or callbacks of an in-queue request.
        /// </summary>
        /// <param name="queue">The queue containing the request.</param>
        /// <param name="imageUrl">The URL of the image request to update.</param>
        /// <param name="onCompleteCallback">The new complete callback, if any.</param>
        /// <param name="onProgressCallback">The new progress callback, if any.</param>
        /// <param name="priority">The new priority for the request.</param>
        /// <returns>True if the request was successfully updated; otherwise, false.</returns>
        protected bool TryUpdateRequest(List<ImageLoadRequest> queue, string imageUrl,
            Action<Texture> onCompleteCallback, Action<float> onProgressCallback, LoadPriority priority) {
            foreach (var request in queue) {
                if (request.imageUrl == imageUrl) {
                    var updated = false;
                    if (priority > request.priority) {
                        request.UpdatePriority(priority);
                        updated = true;
                    }

                    if (onCompleteCallback != null) {
                        request.UpdateCallback(onCompleteCallback);
                        updated = true;
                    }

                    if (onProgressCallback != null) {
                        request.UpdateProgressCallback(onProgressCallback);
                        updated = true;
                    }

                    return updated;
                }
            }

            return false;
        }
    }
}