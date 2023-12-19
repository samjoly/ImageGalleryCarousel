/*
 * ImageLoadRequestPool
 * 2023-12-08 - SJ
 *
 * SUMMARY:
 * Pool of ImageLoadRequest objects.
 *
 */

using System.Collections.Generic;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Pool of ImageLoadRequest objects.
    /// </summary>
    public class ImageLoadRequestPool {
        private readonly Queue<ImageLoadRequest> _requestPool = new();

        /// <summary>
        ///     Retrieves a pooled instance of the ImageLoadRequest class.
        ///     If there is a request available in the requestPool, it will be dequeued and returned.
        ///     Otherwise, a new instance of ImageLoadRequest will be created and returned.
        /// </summary>
        /// <returns>
        ///     A pooled instance of the ImageLoadRequest class, or a new instance if none are available in the pool.
        /// </returns>
        public ImageLoadRequest GetPooledRequest() {
            if (_requestPool.Count > 0) {
                return _requestPool.Dequeue();
            }

            return new ImageLoadRequest();
        }

        /// <summary>
        ///     Returns an ImageLoadRequest to the request pool.
        /// </summary>
        /// <param name="request">The ImageLoadRequest to be returned to the pool.</param>
        public void ReturnRequestToPool(ImageLoadRequest request) {
            request.Reset(); // Clear any previous data
            _requestPool.Enqueue(request);
        }
    }
}