using System;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Represents a class that loads a list of image names from Unity project Resources Folder assetList.json
    /// </summary>
    public class ImageListLoaderResources : MonoBehaviour, IImageListLoader {
        public string imgFolder = "img";

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
    }
}