using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Assists in debugging Unity image loading processes with real-time UI feedback on load status.
    /// </summary>
    public class ImageLoaderDebuggerUi : MonoBehaviour {
        private const int BAR_LENGTH = 20; // Length of the loading bar

        [SerializeField]
        private TextMeshProUGUI debugText;

        private readonly float monoSpace = 7f;

        private IImageLoader _imageLoader;

        private void Awake() {
            _imageLoader = GetComponent<IImageLoader>();
        }

        private void Update() {
            UpdateDebugText();
        }

        private void OnEnable() {
            debugText.gameObject.SetActive(true);
        }

        private void OnDisable() {
            debugText.gameObject.SetActive(false);
        }

        private string CreateLoadingBar(float progress) {
            var filledLength = (int)(progress * BAR_LENGTH);
            var bar = new string('=', filledLength) + new string('-', BAR_LENGTH - filledLength);
            return $"[{bar}]";
        }

        private void UpdateDebugText() {
            var activeRequests =
                _imageLoader.GetCurrentActiveRequests(); // Assuming this method exists in ImageLoader

            // Sort the requests by state using LINQ
            var sortedRequests = activeRequests.OrderByDescending(request => request.state).ToList();

            var debugInfo = new StringBuilder();
            foreach (var request in sortedRequests) {
                debugInfo.AppendLine($"{request.imageName} - {request.priority}");
                debugInfo.AppendLine($"{request.state} {request.progress:P}{CreateLoadingBar(request.progress)} ");
                debugInfo.AppendLine("");
            }

            debugText.text = $"<mspace={monoSpace}>{debugInfo}";
        }
    }
}