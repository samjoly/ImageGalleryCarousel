/*
 * AssetList
 * 2023-12-12 - SJ
 *
 * SUMMARY:
 * Serializable class representing a list of assets. Model for JsonUtility.FromJson<AssetList>
 */

using System;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Serializable class representing a list of assets. Model for JsonUtility.FromJson
    /// </summary>
    [Serializable]
    public class AssetList {
        #region Serialized Fields

        // public List<string> assets;
        public string[] assets;

        #endregion
    }
}