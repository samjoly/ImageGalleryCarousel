using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace SJ.ImageGallery {
    /// <summary>
    /// Manages the retrieval of image URLs dynamically from a specified server directory, using a PHP script.
    /// </summary>
    public class ImageListLoaderServer : MonoBehaviour, IImageListLoader {
        public LoaderServerConfig serverConfig;

        private string phpScriptUrl =>
            $"{serverConfig.baseUrl}/{serverConfig.phpScript}?folder={serverConfig.imgFolder}";

        /// <inheritdoc />
        public void GetImageList(Action<string[]> onCompleteCallback) {
            StartCoroutine(GetImageListCoroutine(onCompleteCallback));
        }

        private IEnumerator GetImageListCoroutine(Action<string[]> onCompleteCallback) {
            using (var webRequest = UnityWebRequest.Get(phpScriptUrl)) {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success) {
                    Debug.LogError($"Error fetching images from {phpScriptUrl}: {webRequest.error}");
                    yield break;
                }

                var assetList = JsonUtility.FromJson<AssetList>(webRequest.downloadHandler.text);
                for (var i = 0; i < assetList.assets.Length; i++) {
                    assetList.assets[i] = $"{serverConfig.baseUrl}/{serverConfig.imgFolder}/{assetList.assets[i]}";
                }

                onCompleteCallback(assetList.assets);
            }
        }
    }
}