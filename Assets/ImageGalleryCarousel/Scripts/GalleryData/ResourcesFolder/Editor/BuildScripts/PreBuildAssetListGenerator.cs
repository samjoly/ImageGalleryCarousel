using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace SJ.ImageGallery {
    /// <summary>
    /// Generates a list of pre-built assets before building the project.
    /// Implements the IPreprocessBuildWithReport interface.
    /// </summary>
    public class PreBuildAssetListGenerator : IPreprocessBuildWithReport {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report) {
            AssetListGenerator.GenerateAssetList();
        }
    }
}