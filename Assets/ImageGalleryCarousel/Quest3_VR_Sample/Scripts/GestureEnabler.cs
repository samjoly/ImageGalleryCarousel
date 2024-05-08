using UnityEngine;
using UnityEngine.Events;

namespace SJ.ImageGallery {
    /// <summary>
    /// Manages the enabling and disabling of gestures based on specific conditions such as proximity, gaze, and interaction status.
    /// This class is designed to facilitate context-sensitive interaction , allowing for more intuitive user experiences.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Enable gestures when an object is within a specified range, is being gazed at, and is not involved in other interactions.</item>
    ///         <item>Trigger Unity events upon enabling or disabling gestures</item>
    ///     </list>
    /// </Features>
    public class GestureEnabler : MonoBehaviour {
        public UnityEvent onEnable;
        public UnityEvent onDisable;

        [SerializeField]
        private bool _isInRange;
        [SerializeField]
        private bool _isInGaze;
        [SerializeField]
        private bool _isFreeFromOtherInteraction;

        /// <summary>
        /// Gets or sets a value indicating whether the object is within the specified range.
        /// </summary>
        public bool IsInRange {
            get => _isInRange;
            set {
                if (_isInRange != value) {
                    _isInRange = value;
                    UpdateState();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the object is being gazed at.
        /// </summary>
        public bool IsInGaze {
            get => _isInGaze;
            set {
                if (_isInGaze != value) {
                    _isInGaze = value;
                    UpdateState();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the object is free from other interactions.
        /// </summary>

        public bool IsFreeFromOtherInteraction {
            get => _isFreeFromOtherInteraction;
            set {
                if (_isFreeFromOtherInteraction != value) {
                    _isFreeFromOtherInteraction = value;
                    UpdateState();
                }
            }
        }

        private void Start() {
            UpdateState();
        }

        private void UpdateState() {
            var isOn = IsInRange && IsInGaze && IsFreeFromOtherInteraction;

            if (isOn) {
                onEnable.Invoke();
            } else {
                onDisable.Invoke();
            }
        }
    }
}