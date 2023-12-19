/*
 * AbstractImageLoader
 * 2023-12-12 - SJ
 *
 * SUMMARY:
 *
 * FEATURES:
 *
 * NOTES:
 *
 * UPDATE:
 *
 * TO DO:
 *
 */

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace SJ.ImageGallery {
    public abstract class AImageLoader : MonoBehaviour, IImageLoader {
        #region Constants and Fields

        protected readonly ConcurrentDictionary<string, Texture> _cache = new();
        protected readonly ConcurrentDictionary<string, DateTime> _cacheLastAccess = new();
        protected readonly HashSet<string> _currentLoadingRequests = new();

        // 'loadingQueue' is a list of ImageLoadRequest objects representing the requests that are currently being processed. This list is used to manage and execute active image loading operations.
        protected readonly List<ImageLoadRequest> _loadingQueue = new();
        protected readonly LinkedList<string> _lruList = new();
        protected readonly object _lruLock = new();


        // The queueLock object is used as a synchronization mechanism to ensure thread safety when accessing or modifying shared resources in a multithreaded environment. In your class, it primarily safeguards the operations on loadingQueue and inProgressLoads, which are collections that can be accessed and modified by different threads simultaneously.
        protected readonly object _queueLock = new();

        // 'requestPool' is an instance of ImageLoadRequestPool, which manages a pool of ImageLoadRequest objects. This pool is used to reuse request objects, reducing the overhead of object creation and garbage collection.
        protected readonly ImageLoadRequestPool _requestPool = new();

        // 'requestQueue' is a list of ImageLoadRequest objects that are queued up and waiting to be processed. Requests in this queue are processed based on their priority and available loading capacity.
        protected readonly List<ImageLoadRequest> _requestQueue = new();


        protected bool _isLoaderActive;

        #endregion

        #region Serialized Fields

        // The maximum number of image load requests to store in the queue.
        public int maxQueueSize = 50;

        // The maximum number of textures to store in the cache.
        protected int maxCacheSize = 100;

        #endregion

        #region IImageLoader Implementation

        /// <inheritdoc />
        public int simultaneousLoadingCount { get; set; } = 3;

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

        public List<ImageLoadRequest> GetCurrentActiveRequests() {
            var activeRequests = new List<ImageLoadRequest>();

            lock (_queueLock) {
                activeRequests.AddRange(_requestQueue);
                activeRequests.AddRange(_loadingQueue);
            }

            return activeRequests;
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
                    // Check if adding this request would exceed maxQueueSize
                    if (_requestQueue.Count >= maxQueueSize) {
                        // Remove the oldest request from the queue
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
                    // Update the priority, callback, and progressCallback if new ones are provided
                    var updated = TryUpdateRequest(_requestQueue, path, onCompleteCallback, onProgressCallback,
                        priority);

                    TryUpdateRequest(_loadingQueue, path, onCompleteCallback, onProgressCallback, priority);

                    if (updated) {
                        // Re-sort the queue only if an update occurred on requestQueue
                        _requestQueue.Sort((a, b) => b.priority.CompareTo(a.priority));
                    }
                }
            }
        }

        /// <inheritdoc />
        public void LoadImagesInBackground(string[] backgroundImages) {
            foreach (var imageUrl in backgroundImages) {
                if (!_cache.ContainsKey(imageUrl)) {
                    // Using LoadImage for background loading with null callbacks and a lower priority
                    LoadImage(imageUrl, null, null, LoadPriority.Low);
                }
            }
        }

        #endregion

        #region Protected Methods

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

        protected abstract IEnumerator LoadImageCoroutine(ImageLoadRequest request);

        protected void OnLoadImageCoroutineComplete(ImageLoadRequest request) {
            _loadingQueue.Remove(request);
            _currentLoadingRequests.Remove(request.imageUrl);
            _requestPool.ReturnRequestToPool(request);
            ProcessQueue();
        }

        protected void ProcessQueue() {
            if (_requestQueue.Count == 0 && _loadingQueue.Count == 0) {
                _isLoaderActive = false;
                return;
            }

            // Create a loadingQueueTemp to avoid modifying the collection directly while iterating over it. 
            var loadingQueueTemp = new List<ImageLoadRequest>();
            var lowPriorityCount = 0;
            var maxLowPriorityLoads = simultaneousLoadingCount / 2; // Example: half of the slots for low priority

            lock (_queueLock) {
                _requestQueue.Sort((a, b) => b.priority.CompareTo(a.priority));

                foreach (var request in _requestQueue) {
                    if (_loadingQueue.Count >= simultaneousLoadingCount) {
                        break; // Stop if the active load count reaches the limit
                    }

                    if (request.priority == LoadPriority.Low) {
                        if (lowPriorityCount >= maxLowPriorityLoads) {
                            continue; // Skip low priority request if limit is reached
                        }

                        lowPriorityCount++;
                    }

                    loadingQueueTemp.Add(request);
                }

                foreach (var request in loadingQueueTemp) {
                    _requestQueue.Remove(request);
                    _loadingQueue.Add(request);
                    // activeLoadCount++;
                    StartCoroutine(LoadImageCoroutine(request));
                }
            }
        }

        protected bool TryUpdateRequest(List<ImageLoadRequest> queue, string imageUrl,
            Action<Texture> onCompleteCallback,
            Action<float> onProgressCallback, LoadPriority priority) {
            foreach (var request in queue) {
                if (request.imageUrl == imageUrl) {
                    if (priority > request.priority) {
                        request.UpdatePriority(priority);
                    }

                    if (onCompleteCallback != null) {
                        request.UpdateCallback(onCompleteCallback);
                    }

                    if (onProgressCallback != null) {
                        request.UpdateProgressCallback(onProgressCallback);
                    }

                    return true;
                }
            }

            return false;
        }

        #endregion

        // Unity lifecycle methods can also be included here if they share common logic
    }
}