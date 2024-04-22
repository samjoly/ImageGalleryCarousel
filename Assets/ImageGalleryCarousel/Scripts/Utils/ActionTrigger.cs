using UnityEngine;
using UnityEngine.Events;

namespace SJ.ImageGallery {
    /// <summary>
    /// Manages trigger interactions for GameObjects, invoking specific Unity Events upon entering or exiting the trigger.
    /// Supports both 3D and 2D environments and can target specific objects or layers for interaction.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <strong>Features:</strong>
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Trigger events for both 3D and 2D colliders.</item>
    ///         <item>Optional targeting of a specific collider to restrict trigger interactions.</item>
    ///         <item>Layer mask functionality to filter interactions based on object layers.</item>
    ///         <item>Debug mode for tracing trigger entries and exits.</item>
    ///     </list>
    /// </remarks>
    /// <note>
    /// Attach this script to any GameObject that needs to interact with other colliders via Unity's physics system.
    /// </note>
    public class ActionTrigger : MonoBehaviour {
        public UnityEvent OnTriggerEnterEvent = new();
        public UnityEvent OnTriggerExitEvent = new();

        public bool is2D;
        public bool specificTarget;
        public LayerMask layerMask; // Layer mask to specify which layer(s) this trigger should interact with

        public Collider colTarget;

        [SerializeField]
        public bool isTrace_debug;

        [Space(20)]
        private bool _isIn;

        private void Awake() {
            if (colTarget != null) {
                if (Physics.GetIgnoreLayerCollision(gameObject.layer, colTarget.gameObject.layer)) {
                    Debug.LogWarning("<color=#ff0000>Warning: LayerCollision Ignored for</color> " + gameObject.name);
                }
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (!is2D && IsValidLayer(other.gameObject)) {
                if (colTarget == null || other == colTarget) {
                    InvokeTriggerEnter();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (is2D && IsValidLayer(other.gameObject)) {
                InvokeTriggerEnter();
            }
        }

        private void OnTriggerExit(Collider other) {
            if (!is2D && IsValidLayer(other.gameObject)) {
                if (colTarget == null || other == colTarget) {
                    InvokeTriggerExit();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (is2D && IsValidLayer(other.gameObject)) {
                InvokeTriggerExit();
            }
        }

        private void InvokeTriggerEnter() {
            _isIn = true;
            OnTriggerEnterEvent?.Invoke();
            if (isTrace_debug) {
                Debug.Log("OnTriggerEnterEvent: " + gameObject.name);
            }
        }

        private void InvokeTriggerExit() {
            _isIn = false;
            OnTriggerExitEvent?.Invoke();
            if (isTrace_debug) {
                Debug.Log("OnTriggerExitEvent: " + gameObject.name);
            }
        }

        // Check if the object's layer is included in the layerMask
        private bool IsValidLayer(GameObject obj) {
            return ((1 << obj.layer) & layerMask) != 0;
        }
    }
}