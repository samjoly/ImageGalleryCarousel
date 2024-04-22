using UnityEngine;
using UnityEngine.Events;

namespace SJ.ImageGallery {
    /// <summary>
    /// Manages trigger events for carousel items in an image gallery. This class ensures that specific actions are taken
    /// when a user interacts with carousel items via entering or exiting the trigger zone.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Capture and delegate enter and exit interactions on carousel items to custom Unity events.</item>
    ///         <item>Allow external subscribers to react to these interactions.</item>
    ///     </list>
    /// </Features>
    /// <note>
    /// This script should be attached to individual carousel item prefabs that are designed to detect entry and exit via triggers.
    /// </note>
    public class CarouselItemEvent : MonoBehaviour {
        public UnityEvent<CarouselItem> OnTriggerEnterEvent = new();
        public UnityEvent<CarouselItem> OnTriggerExitEvent = new();

        private CarouselItem item;

        private void Start() {
            item = GetComponent<CarouselItem>();
            var triggers = GetComponentsInChildren<ActionTrigger>();
            foreach (var trigger in triggers) {
                // Subscribe to child events
                trigger.OnTriggerEnterEvent.AddListener(ChildTriggerEnter);
                trigger.OnTriggerExitEvent.AddListener(ChildTriggerExit);
            }
        }

        private void OnDestroy() {
            var triggers = GetComponentsInChildren<ActionTrigger>();
            foreach (var trigger in triggers) {
                trigger.OnTriggerEnterEvent.RemoveListener(ChildTriggerEnter);
                trigger.OnTriggerExitEvent.RemoveListener(ChildTriggerExit);
            }
        }

        private void ChildTriggerEnter() {
            OnTriggerEnterEvent.Invoke(item); // Trigger the parent's own event
        }

        private void ChildTriggerExit() {
            OnTriggerExitEvent.Invoke(item); // Trigger the parent's own event
        }
    }
}