using System.Collections;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Dynamically adjusts image loading capacity based on network conditions and device performance to optimize resource use and user experience.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Monitors network speed from image download times.</item>
    ///         <item>Estimates device performance from average frame time.</item>
    ///         <item>Balances concurrent image downloads with network speed and device performance.</item>
    ///         <item>Runs in background, updating load rate as performance metrics change.</item>
    ///     </list>
    /// </Features>
    public class AdaptiveLoader : MonoBehaviour {
        /// <summary>
        /// The maximum number of images that can be loaded simultaneously by the AdaptiveLoader.
        /// </summary>
        /// <remarks>
        /// This value is used to balance concurrent image downloads with network speed and device performance.
        /// Higher values allow more images to be loaded simultaneously, while lower values prioritize resource use and user experience.
        /// </remarks>
        public int simultaneousLoadingCount = 3;

        /// <summary>
        /// The time interval (in seconds) for monitoring device performance and network speed.
        /// </summary>
        public float MonitoringInterval = 5.0f;

        // Frame rate threshold for good device performance.
        /// <summary>
        /// The threshold value for determining good device performance (in frames per second).
        /// </summary>
        public float GoodPerformanceThreshold = 75.0f;

        /// <summary>
        /// The network speed threshold for high-speed internet (in Mbps).
        /// </summary>
        public float HighSpeedThreshold = 6f;

        /// <summary>
        /// The frame rate threshold for poor device performance.
        /// </summary>
        public float PoorPerformanceThreshold = 50.0f;

        /// <summary>
        /// The network speed threshold for low-speed internet (in Mbps).
        /// </summary>
        public float LowSpeedThreshold = 2.0f;

        // Variables for performance metrics and accumulators
        private float devicePerformance; // as a performance metric
        private int frameCount;
        private float frameTimeAccumulator;

        private IImageLoader imageLoader;
        private Coroutine monitoringCoroutine;
        private float networkSpeed; // in Mbps
        private float startTime;
        private long totalDownloadedBytes;
        private float totalDownloadTime;

        /// <summary>
        /// Updates the frameTimeAccumulator and frameCount by adding the Time.deltaTime value.
        /// </summary>
        public void Update() {
            frameTimeAccumulator += Time.deltaTime;
            frameCount++;
        }

        // Called by IImageLoader at the start of an image download
        /// <summary>
        /// Begins the download of a measurement.
        /// </summary>
        public void BeginDownloadMeasurement() {
            startTime = Time.time;
        }

        /// <summary>
        /// Cleans up the monitoring coroutine.
        /// </summary>
        public void Cleanup() {
            if (monitoringCoroutine != null) {
                StopCoroutine(monitoringCoroutine);
                monitoringCoroutine = null;
            }
        }

        /// <summary>
        /// Ends the measurement of a download, calculates the network speed based on the downloaded bytes and duration.
        /// </summary>
        /// <param name="downloadedBytes">The number of bytes downloaded.</param>
        public void EndDownloadMeasurement(ulong downloadedBytes) {
            var downloadDuration = Time.time - startTime;
            totalDownloadTime += downloadDuration;
            totalDownloadedBytes += (long)downloadedBytes;
            networkSpeed = totalDownloadedBytes / totalDownloadTime / 125000; // Convert bytes/sec to Mbps
        }

        /// <summary>
        /// Initializes with the given image loader.
        /// </summary>
        /// <param name="loader">The image loader to use for loading images </param>
        public void Init(IImageLoader loader) {
            imageLoader = loader;
            imageLoader.simultaneousLoadingCount = simultaneousLoadingCount;
            monitoringCoroutine = StartCoroutine(MonitoringCoroutine());
        }

        // Estimate device performance
        private void EstimateDevicePerformance() {
            if (frameCount > 0) {
                var averageFrameTime = frameTimeAccumulator / frameCount;
                devicePerformance = 1 / averageFrameTime;
                frameTimeAccumulator = 0f;
                frameCount = 0;
            }
        }

        // Coroutine for periodic monitoring of performance and network speed
        private IEnumerator MonitoringCoroutine() {
            while (true) {
                EstimateDevicePerformance();
                UpdateLoadingRate();
                yield return new WaitForSeconds(MonitoringInterval);
            }
        }

        // Update the simultaneousLoading rate based on performance metrics
        private void UpdateLoadingRate() {
            if (networkSpeed == 0 && devicePerformance == 0) {
                return;
            }

            if (networkSpeed > HighSpeedThreshold && devicePerformance > GoodPerformanceThreshold) {
                simultaneousLoadingCount = Mathf.Min(simultaneousLoadingCount + 1, 10);
            } else if (networkSpeed < LowSpeedThreshold || devicePerformance < PoorPerformanceThreshold) {
                simultaneousLoadingCount = Mathf.Max(simultaneousLoadingCount - 1, 1);
            }

            imageLoader.simultaneousLoadingCount = simultaneousLoadingCount;
        }
    }
}