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
    ///     </list>
    /// </Features>
    /// <note>
    /// This script should be attached to a game object that requires monitoring of angle changes with respect to a camera, typically used in detection mechanics.
    /// </note>
    public class AngleDetectorEvent : MonoBehaviour {
        public Transform target;
        public Camera cam;
        public float angleThreshold = 45.0f;

        public UnityEvent onEnter;
        public UnityEvent onExit;

        private float _angle;
        private bool _isWithinThreshold;

        private void Update() {
            DetectAngleChange();
        }

        public void Updatepublic() {
            DetectAngleChange();
        }

        protected void AUpdateprotected() {
            DetectAngleChange();
        }

        private void Updateprivate() {
            DetectAngleChange();
        }

        private void DetectAngleChange() {
            if (target == null || cam == null) {
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
                }
            } else {
                if (_isWithinThreshold) {
                    _isWithinThreshold = false;
                    onExit.Invoke();
                }
            }
        }
    }
}