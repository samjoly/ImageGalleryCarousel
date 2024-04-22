using System.Collections.Generic;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Manages the activation and deactivation of a list of GameObjects based on user gaze and a locking mechanism.
    /// This functionality is particularly useful in virtual and augmented reality environments where object interaction
    /// depends significantly on user attention (gaze) and state conditions (lock).
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Toggle GameObjects based on the presence of gaze and the absence of a locking state.</item>
    ///     </list>
    /// </Features>
    public class GestureActivation : MonoBehaviour {
        [SerializeField]
        private List<GameObject> gameObjects = new();
        private bool _isInGaze; // Indicates if the object is currently being gazed at
        private bool _isLock; // Lock state that prevents activation even if gazed upon
        public bool isInGaze {
            get => _isInGaze;
            set {
                if (_isInGaze != value) {
                    _isInGaze = value;
                    UpdateActiveState();
                }
            }
        }
        public bool isLock {
            get => _isLock;
            set {
                if (_isLock != value) {
                    _isLock = value;
                    UpdateActiveState();
                }
            }
        }

        private void Start() {
            UpdateActiveState();
        }

        private void UpdateActiveState() {
            var desiredState = isInGaze && !isLock;
            SetActiveState(desiredState);
        }

        private void SetActiveState(bool isActive) {
            foreach (var obj in gameObjects) {
                if (obj != null) {
                    obj.SetActive(isActive);
                }
            }
        }
    }
}