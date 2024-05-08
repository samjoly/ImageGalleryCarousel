using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Controls the rotation of an object to face a target with optional snapping to 90-degree increments.
    /// This class is designed to smoothly rotate objects such as images or models to align with a target direction.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Smoothly rotate towards a target object.</item>
    ///         <item>Optional snapping to the nearest 90-degree angle to maintain a structured alignment.</item>
    ///         <item>Adjustable rotation speed and dead zone for fine control over rotation dynamics.</item>
    ///     </list>
    /// </Features>
    /// <note>
    /// Used to adjust the orientation of the gallery when in a closed state, with the added feature of snapping to the nearest 90 degrees to provide a personalized touch.
    /// </note>
    public class GalleryParentRotator : MonoBehaviour {
        public Transform target;
        public float rotationSpeed = 1f;
        public float deadZone = 5f; // Dead zone threshold in degrees

        public bool isLateUpdate;

        private float _currentSnappedYRotation; // Initially zero or set this to a starting rotation if needed

        private void LateUpdate() {
            if (isLateUpdate) {
                UpdateAngle();
            }
        }

        private void UpdateAngle() {
            if (target != null) {
                var targetDirection = target.position - transform.position;
                targetDirection.y = 0; // Remove any vertical component

                // Convert target direction to local space
                var localTargetDirection = transform.parent.InverseTransformDirection(targetDirection);

                // Calculate the rotation required to look at the target in local space
                var localLookRotation = Quaternion.LookRotation(localTargetDirection).eulerAngles;

                // Snap the local y-axis rotation to the nearest 90 degrees with dead zone consideration
                _currentSnappedYRotation = SnapToNearest90(localLookRotation.y);

                // Smooth transition to the current snapped rotation
                var currentRotation = transform.localRotation;
                var targetRotation = Quaternion.Euler(0, _currentSnappedYRotation, 0);
                transform.localRotation =
                    Quaternion.Lerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private float SnapToNearest90(float angle) {
            var baseAngle = Mathf.Round(angle / 90) * 90;
            // Adjusting the condition to check for significant deviation from the nearest 90-degree increment
            if (Mathf.Abs(Mathf.DeltaAngle(angle, baseAngle)) > 45 - deadZone) {
                return _currentSnappedYRotation; // Maintain current rotation if within the dead zone
            }

            return baseAngle; // Snap to the nearest 90 degrees
        }
    }
}