using UnityEngine;
using UnityEngine.Events;

namespace SJ.ImageGallery {
    /// <summary>
    /// Monitors the angle between the camera's forward direction and the vector pointing to a target object.
    /// Triggers events when the angle crosses a specified threshold.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Calculate the directional angle to a target relative to the camera's forward direction.</item>
    ///         <item>Trigger Unity events when the angle crosses above or below a defined threshold.</item>
    ///         <item>Implements a cooldown feature to prevent rapid re-triggering of events, ensuring events fire only after a specified time has elapsed.</item>
    ///     </list>
    /// </Features>
    /// <note>
    /// This script should be attached to a game object that requires monitoring of angle changes with respect to a camera, typically used in detection mechanics.
    /// </note>
    public class VisionConeTrigger : MonoBehaviour {
        public Transform target;
        public Camera cam;
        public float angleThreshold = 40.0f;
        public float cooldownTime = 2.0f; // Cooldown time in seconds

        public UnityEvent onEnter;
        public UnityEvent onExit;

        private float _angle;
        private bool _isWithinThreshold;
        private float _lastEventTime = -1.0f; // Initialize with -1 to allow immediate trigger at start

        private void Update() {
            DetectAngleChange();
        }

        private void DetectAngleChange() {
            if (target == null || cam == null) {
                return;
            }

            if (Time.time < _lastEventTime + cooldownTime) {
                // Skip the update if we are in cooldown
                return;
            }

            // Calculate direction from camera to target
            var directionToTarget = target.position - cam.transform.position;
            directionToTarget.Normalize(); // Normalize to get only the direction

            // Calculate the angle
            var dot = Vector3.Dot(cam.transform.forward, directionToTarget);
            _angle = Mathf.Acos(dot) * Mathf.Rad2Deg; // Convert to degrees

            if (_angle <= angleThreshold) {
                if (!_isWithinThreshold) {
                    _isWithinThreshold = true;
                    onEnter.Invoke();
                    _lastEventTime = Time.time; // Reset the event time on trigger
                }
            } else {
                if (_isWithinThreshold) {
                    _isWithinThreshold = false;
                    onExit.Invoke();
                    _lastEventTime = Time.time; // Reset the event time on trigger
                }
            }
        }
    }
}