/*
 * AssetListGenerator
 * 2023-12-12 - SJ
 *
 * SUMMARY:
 * Generates an asset list from a specified folder path and saves it as a JSON file.
 * This allow to get the asset name within the Resources folder, without having to hardcode it, or load them to get the name at runtime.
 *
 */

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// An editor script that generates a JSON file listing all assets in a specified folder.
    /// This facilitates asset management by avoiding hardcoded asset names and runtime loads.
    /// Intended for use within the Unity Editor only.
    /// </summary>
    public class AssetListGenerator : MonoBehaviour {
        /// <summary>
        /// Scans a folder for Texture2D assets and saves their names to a JSON file in the same folder.
        /// Useful for easy asset retrieval at runtime. Editor-only functionality.
        /// </summary>
        [MenuItem("Assets/Generate Asset List")]
        public static void GenerateAssetList() {
            var folderPath = "Assets/ImageGalleryCarousel/Resources/"; // Path to your folder
            var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
            Debug.Log("Assets found: " + guids.Length); // Debug log

            var assetNames = new List<string>();

            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var item = Path.GetFileNameWithoutExtension(path);
                Debug.Log("item: " + item); // Debug log
                assetNames.Add(item);
            }

            var assetList = new AssetList {
                assets = assetNames.ToArray()
            };

            var json = JsonUtility.ToJson(assetList, false);
            var outputPath = folderPath + "assetList.json"; // Path to save the JSON file
            File.WriteAllText(outputPath, json);
            AssetDatabase.Refresh();
        }
    }
}