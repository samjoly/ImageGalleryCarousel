/*
 * LoaderServerConfig
 * 2023-12-13 - SJ
 *
 * SUMMARY:
 * Represents the configuration for the Loader Server.
 *
 */

using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Represents the configuration for the Loader Server.
    /// </summary>
    [CreateAssetMenu(fileName = "LoaderServerConfig", menuName = "ImageGallery/LoaderServerConfig", order = 0)]
    public class LoaderServerConfig : ScriptableObject {
        // Online location of the php script and image folder
        public string baseUrl = "https://yourdomain.com/location";
        public string phpScript = "ImageFetcher.php";
        public string imgFolder = "img";
    }
}