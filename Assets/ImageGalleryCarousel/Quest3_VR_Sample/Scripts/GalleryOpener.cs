using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SJ.ImageGallery {
    /// <summary>
    /// Provides functionality to animate the opening and closing of a gallery. This class uses scaling animations to transition the gallery between open and closed states.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Animate gallery using scale transformations with customizable easing.</item>
    ///         <item>Trigger events at the start and completion of opening and closing animations.</item>
    ///         <item>Toggle functionality to switch between open and closed states based on current state.</item>
    ///     </list>
    /// </Features>
    public class GalleryOpener : MonoBehaviour {
        public GameObject gallery;

        public float openScale = 1f;
        public Vector3 openPosition;

        public Vector3 closePosition;
        public float closeScale = 0.05f;

        public float duration = 0.5f;
        public Ease ease = Ease.InQuart;

        [FormerlySerializedAs("OnOpenStart")]
        public UnityEvent onOpenStart;
        [FormerlySerializedAs("OnCloseComplete")]
        public UnityEvent onCloseComplete;
        [FormerlySerializedAs("OnCloseStart")]
        public UnityEvent onCloseStart;
        [FormerlySerializedAs("OnOpenComplete")]
        public UnityEvent onOpenComplete;

        /// <summary>
        /// Gets or sets a value indicating whether the animation is allowed to override any ongoing animation. Make App more reactive.
        /// </summary>
        public bool isOverriting = true;

        private bool _isAnimating;
        private bool _isOpen;

        private Tween _tweenMove;

        private Tween _tweenScale;

        /// <summary>
        /// Toggles the active state of the object.
        /// </summary>
        public void ToggleActive() {
            if (_isAnimating) {
                return;
            }

            if (_isOpen) {
                Close();
            } else {
                Open();
            }
        }

        /// <summary>
        /// Opens the object by animating it.
        /// </summary>
        public void Open() {
            Animate(true);
        }

        /// <summary>
        /// Closes the object by animating it.
        /// </summary>
        public void Close() {
            Animate(false);
        }

        private void Animate(bool opening) {
            if (_isAnimating && !isOverriting) {
                return;
            }

            _tweenMove?.Kill();
            _tweenScale?.Kill();

            _isAnimating = true;
            _isOpen = opening;

            var targetScale = opening ? openScale : closeScale;
            var targetPosition = opening ? openPosition : closePosition;

            var startEvent = opening ? onOpenStart : onCloseStart;
            var completeEvent = opening ? onOpenComplete : onCloseComplete;

            startEvent?.Invoke();
            _tweenMove = gallery.transform.DOLocalMove(targetPosition, duration).SetEase(ease);
            _tweenScale = gallery.transform.DOScale(targetScale, duration)
                .SetEase(ease)
                .OnComplete(() => {
                    completeEvent?.Invoke();
                    _isAnimating = false;
                });
        }
    }
}