/*
 * ImageListLoaderResources
 * 2023-12-08 - SJ
 *
 * SUMMARY:
 * Represents a class that loads a list of image names from Unity project Resources Folder assetList.json
 *
 */

using System;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Represents a class that loads a list of image names from Unity project Resources Folder assetList.json
    /// </summary>
    public class ImageListLoaderResources : MonoBehaviour, IImageListLoader {
        #region Serialized Fields

        public string imgFolder = "img";

        #endregion

        #region IImageListLoader Implementation

        /// <inheritdoc />
        public void GetImageList(Action<string[]> onCompleteCallback) {
            var assetListPath = !string.IsNullOrEmpty(imgFolder) ? $"{imgFolder}/assetList" : "assetList";
            var jsonFile = Resources.Load<TextAsset>(assetListPath);
            //Expected format:  {"assets":["img000","img001",...]}
            var assetList = JsonUtility.FromJson<AssetList>(jsonFile.text);
            if (!string.IsNullOrEmpty(imgFolder)) {
                for (var i = 0; i < assetList.assets.Length; i++) {
                    assetList.assets[i] = $"{imgFolder}/{assetList.assets[i]}";
                }
            }

            onCompleteCallback(assetList.assets);
        }

        #endregion
    }
}