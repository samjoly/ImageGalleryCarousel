using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SJ.ImageGallery {
    /// <summary>
    /// Responsible the alpha transition (Show, Hide, Pulse) of an Image component.
    /// </summary>
    public class ImageAlphaCtrl : MonoBehaviour {
        [Header("Pulse Settings:")]
        public Image targetImage;
        public float pulseDuration = 1f;
        public float showHideDuration = 0.4f;
        public float minAlpha = 0.5f;
        public float maxAlpha = 1f;

        private Sequence _sequence;

        private Tween _fadeTween;

        private void OnDisable() {
            _sequence?.Kill();
            _fadeTween?.Kill();
        }

        public void Hide() {
            _sequence?.Kill();
            _fadeTween = targetImage.DOFade(0, showHideDuration).SetEase(Ease.OutSine);
        }

        public void Show() {
            _fadeTween = targetImage.DOFade(maxAlpha, showHideDuration).SetEase(Ease.InSine).OnComplete(() => {
                Pulse();
            });
        }

        private void Pulse() {
            _sequence?.Kill();
            _fadeTween?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(targetImage.DOFade(maxAlpha, 0));
            _sequence.Append(targetImage.DOFade(minAlpha, pulseDuration / 2).SetEase(Ease.OutSine));
            _sequence.Append(targetImage.DOFade(maxAlpha, pulseDuration / 2).SetEase(Ease.InSine));
            _sequence.SetLoops(-1);
            _sequence.Play();
        }
    }
}