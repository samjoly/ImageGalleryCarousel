/*
 * ImageListLoaderServer
 * 2023-12-08 - SJ
 *
 * SUMMARY:
 * Represents a class that loads a list of image names from a remote server.
 *
 * TO DO:
 * Rename to mention the php
 *
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Represents a class that loads a list of image names from a remote server.
    /// </summary>
    public class ImageListLoaderServer : MonoBehaviour, IImageListLoader {
        #region Serialized Fields

// https://19011310.com/photoGallery/ImageFetcher.php?folder=jpg
        // public string baseUrl = "https://19011310.com/photoGallery";
        // public string phpScript = "ImageFetcher.php";
        // public string imgFolder = "img";

        public LoaderServerConfig serverConfig;
        #endregion

        #region Private Properties

        // private string phpScriptUrl => $"{baseUrl}/{phpScript}?folder={imgFolder}";
        private string phpScriptUrl => $"{serverConfig.baseUrl}/{serverConfig.phpScript}?folder={serverConfig.imgFolder}";
        #endregion

        #region IImageListLoader Implementation

        /// <inheritdoc />
        public void GetImageList(Action<string[]> onCompleteCallback) {
            StartCoroutine(GetImageListCoroutine(onCompleteCallback));
        }

        #endregion

        #region Private Methods

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

        #endregion
    }
}