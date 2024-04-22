using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Class for setting the target frame rate of the application.
    /// </summary>
    public class FrameRateSetter : MonoBehaviour {
        public int targetFrameRate = 60;

        private void Start() {
            Application.targetFrameRate = targetFrameRate;
        }
    }
}