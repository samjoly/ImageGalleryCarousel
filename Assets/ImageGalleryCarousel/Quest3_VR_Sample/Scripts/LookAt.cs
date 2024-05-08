using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace SJ.ImageGallery {
    /// <summary>
    /// Facilitates the rotation of a GameObject to face another GameObject continuously. This class provides the ability to start or stop the rotation and restore the initial orientation of the object.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Continuously rotate an object to look at another specified object.</item>
    ///         <item>Ability to start and stop the look at functionality dynamically.</item>
    ///         <item>Restore the object to its initial rotation.</item>
    ///         <item>UpdateLookAt on Update or LateUpdate</item>
    ///     </list>
    /// </Features>
    public class LookAt : MonoBehaviour {
        [FormerlySerializedAs("_toRotate")]
        public Transform toRotate;

        [FormerlySerializedAs("_target")]
        public Transform target;

        public bool isLateUpdate;
        [SerializeField]
        private bool isAutoStart;

        private Vector3 _initialRotation;

        private bool _isLookingAt;

        private void Awake() {
            _initialRotation = toRotate.localEulerAngles;
        }

        private void Start() {
            if (isAutoStart) {
                StartLookingAt();
            }
        }

        private void Update() {
            if (!isLateUpdate) {
                UpdateLookAt();
            }
        }

        private void LateUpdate() {
            if (isLateUpdate) {
                UpdateLookAt();
            }
        }

        private void UpdateLookAt() {
            if (!_isLookingAt) {
                return;
            }

            var dirToTarget = (target.position - toRotate.position).normalized;
            toRotate.LookAt(toRotate.position - dirToTarget, Vector3.up);
        }

        /// <summary>
        /// Activates the continuous rotation towards the target object.
        /// </summary>
        public void StartLookingAt() {
            _isLookingAt = true;
        }

        /// <summary>
        /// Deactivates the continuous rotation and smoothly restores the GameObject to its initial rotation.
        /// </summary>
        public void StopAndRestore() {
            _isLookingAt = false;
            toRotate.DOLocalRotate(_initialRotation, 0.5f);
        }
    }
}