using UnityEngine;
using UnityEngine.Events;

namespace SJ.ImageGallery {
    /// <summary>
    /// Manages the interactions within an image gallery. This class handles events triggered by entering and exiting carousel items,
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Subscribe to child trigger enter and exit events.</item>
    ///         <item>Forward trigger events from individual carousel items to general gallery management events.</item>
    ///     </list>
    /// </Features>
    /// <note>
    /// This script should be attached to the gallery manager object, which should be a parent to carousel items. It acts as a central hub for event managements.
    /// </note>
    public class GalleryManagerEvent : MonoBehaviour {
        public UnityEvent<GameObject> OnTriggerEnterEvent = new();
        public UnityEvent<GameObject> OnTriggerExitEvent = new();

        private void Start() {
            var triggers = GetComponentsInChildren<CarouselItemEvent>();
            foreach (var trigger in triggers) {
                // Subscribe to child events
                trigger.OnTriggerEnterEvent.AddListener(ChildTriggerEnter);
                trigger.OnTriggerExitEvent.AddListener(ChildTriggerExit);
            }
        }

        private void OnDestroy() {
            var triggers = GetComponentsInChildren<CarouselItemEvent>();
            foreach (var trigger in triggers) {
                trigger.OnTriggerEnterEvent.RemoveListener(ChildTriggerEnter);
                trigger.OnTriggerExitEvent.RemoveListener(ChildTriggerExit);
            }
        }

        private void ChildTriggerEnter(CarouselItem item) {
            OnTriggerEnterEvent.Invoke(item.gameObject);
        }

        private void ChildTriggerExit(CarouselItem item) {
            OnTriggerExitEvent.Invoke(item.gameObject);
        }
    }
}