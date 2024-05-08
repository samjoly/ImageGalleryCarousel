using DG.Tweening;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Provides visual feedback for gestures by animating an UI object. Designed to enhance user interaction with visual cues.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Animate an object's movement and scale to indicate gesture direction and intensity.</item>
    ///         <item>Support for gestures in multiple directions: right, left, up, and down.</item>
    ///         <item>Customizable animation duration, offset, and scale settings for tailored feedback.</item>
    ///     </list>
    /// </Features>
    public class UiGestureFeedback : MonoBehaviour {
        public Transform target;
        public float moveDuration = 0.5f;

        [SerializeField]
        private float offset = 0.5f;

        [SerializeField]
        private float scale = 2.5f;
        private float _offsetMoveSide = 1.0f;
        private bool _isMoving;
        private Tween _moveTween;
        private Tween _scaleTween;

        private Vector3 _positionAfterMove;

        /// <summary>
        /// Resets the UiGestureFeedback component to its initial state.
        /// </summary>
        public void Reset() {
            CancelInvoke();
            _scaleTween?.Kill();
            _positionAfterMove = Vector3.zero;
            target.localScale = Vector3.zero;
            target.localPosition = Vector3.zero;
            _isMoving = false;
        }

        private void Start() {
            _offsetMoveSide = offset * 3;
            Cancel();
        }

        /// <summary>
        /// Moves the target to the right with a specified offset.
        /// </summary>
        public void GoToRight() {
            GoTo(new Vector3(offset, 0, 0));
        }

        /// <summary>
        /// Moves the target to the left with a specified offset.
        /// </summary>
        public void GoToLeft() {
            GoTo(new Vector3(-offset, 0, 0));
        }

        /// <summary>
        /// Moves the target to the right with a specified offset and initiates another move to the side.
        /// </summary>
        public void MoveToRight() {
            GoTo(new Vector3(-offset, 0, 0), true);
            MoveTo(new Vector3(_offsetMoveSide, 0, 0), true);
        }

        /// <summary>
        /// Moves the target to the left with a specified offset and initiates a side move animation.
        /// </summary>
        public void MoveToLeft() {
            GoTo(new Vector3(offset, 0, 0), true);
            MoveTo(new Vector3(-_offsetMoveSide, 0, 0), true);
        }

        /// <summary>
        /// Moves the target up with a specified offset.
        /// </summary>
        public void GoToUp() {
            GoTo(new Vector3(0, offset, 0));
        }

        /// <summary>
        /// Moves the target down with a specified offset.
        /// </summary>
        public void GoToDown() {
            GoTo(new Vector3(0, -offset, 0));
        }

        /// <summary>
        /// Moves the target to the up direction with a specified offset and also moves it back to the original position.
        /// </summary>
        public void MoveToUp() {
            GoTo(new Vector3(0, -offset, 0), true);
            MoveTo(new Vector3(0, offset, 0));
        }

        /// <summary>
        /// Moves the target downwards with a specified offset.
        /// </summary>
        public void MoveToDown() {
            GoTo(new Vector3(0, offset, 0), true);
            MoveTo(new Vector3(0, -offset, 0));
        }

        /// <summary>
        /// Cancels the ongoing motion and hides the target object after a delay.
        /// </summary>
        public void Cancel() {
            Invoke(nameof(CancelDelay), 0.1f);
        }

        private void CancelDelay() {
            if (_isMoving) {
                return;
            }

            _positionAfterMove = Vector3.zero;
            _scaleTween?.Kill();
            _scaleTween = target.DOScale(0, 0.2f).OnComplete(() => {
                // Object out of view:
                target.localScale = Vector3.zero;
            });
        }

        private void MoveTo(Vector3 position, bool isSide = false) {
            CancelInvoke();
            _isMoving = true;
            _moveTween?.Kill();
            _moveTween = target.DOLocalMove(position, moveDuration).OnComplete(() => {
                    _isMoving = false;
                    if (isSide) {
                        // Object out of view:
                        target.localScale = Vector3.zero;
                    }

                    if (_positionAfterMove != Vector3.zero) {
                        GoTo(_positionAfterMove);
                        _positionAfterMove = Vector3.zero;
                    }
                })
                .OnKill(() => {
                    _isMoving = false;
                    _positionAfterMove = Vector3.zero;
                });
        }

        private void GoTo(Vector3 position, bool isDirect = false) {
            CancelInvoke();
            if (_isMoving && !isDirect) {
                _positionAfterMove = position;
                return;
            }

            _isMoving = true;
            _moveTween?.Kill();
            _scaleTween?.Kill();

            // Check if object is out of view, and move object further to force smooth gfx transition:
            if (!isDirect && target.localScale == Vector3.zero) {
                target.localPosition = position * 2;
            }

            _scaleTween = target.DOScale(scale, moveDuration / 2).OnComplete(() => { _isMoving = false; });
            if (isDirect) {
                target.localPosition = position;
            } else {
                _moveTween = target.DOLocalMove(position, moveDuration / 3);
            }
        }
    }
}