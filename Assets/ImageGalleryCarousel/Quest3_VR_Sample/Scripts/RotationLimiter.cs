using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Constrains the rotation of a GameObject along specified axes to prevent undesired orientation changes.
    /// This is used to maintain stable viewing angles or alignments in interactive applications or simulations.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Ability to independently limit rotation around the X, Y, and Z axes.</item>
    ///         <item>Stores initial rotation angles to use as constraints if rotation limits are active.</item>
    ///         <item>Operates in LateUpdate to override any rotational changes made during the frame.</item>
    ///     </list>
    /// </Features>
    public class RotationLimiter : MonoBehaviour {
        public bool limitX;
        public bool limitY;
        public bool limitZ;

        private Vector3 originalAngles;

        private void Start() {
            // Cache the original rotation angles
            originalAngles = transform.eulerAngles;
        }

        private void LateUpdate() {
            var currentAngles = transform.eulerAngles;
            var newX = limitX ? originalAngles.x : currentAngles.x;
            var newY = limitY ? originalAngles.y : currentAngles.y;
            var newZ = limitZ ? originalAngles.z : currentAngles.z;

            transform.eulerAngles = new Vector3(newX, newY, newZ);
        }
    }
}