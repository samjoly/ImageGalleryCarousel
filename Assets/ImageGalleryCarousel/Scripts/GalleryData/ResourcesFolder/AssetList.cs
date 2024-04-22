using System;

namespace SJ.ImageGallery {
    /// <summary>
    /// Serializable class representing a list of assets. Model for JsonUtility.FromJson<AssetList>
    /// </summary>
    [Serializable]
    public class AssetList {
        // public List<string> assets;
        public string[] assets;
    }
}