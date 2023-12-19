/*
 * LoadingGfx
 * 2023-12-08 - SJ
 *
 * SUMMARY:
 * Represents a class for handling loading UI.
 *
 */

using System;
using DG.Tweening;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Represents a class for handling loading UI.
    /// </summary>
    public class LoadingGfx : MonoBehaviour, ILoadingGfx {
        #region Constants and Fields

        private Vector3 _loaderGfxInScale;

        private Tween _tweenOut;

        #endregion

        #region Serialized Fields

        // parent with different pivot
        private Transform loaderGfxIn;
        private Transform loaderGfxOut;


        private void Awake() {
            loaderGfxOut = transform;
            loaderGfxIn = transform.GetChild(0);
        }

        #endregion

        #region ILoadingGfx Implementation

        public void OnComplete() {
            KillTween(_tweenOut);
            _tweenOut = loaderGfxOut.DOScaleX(0, 0.2f).SetEase(Ease.InSine);
        }

        /// <summary>
        ///     Updates the progress of the loader graphics.
        /// </summary>
        /// <param name="progress">The value representing the progress of the loader. 0 - 1%</param>
        public void OnProgress(float progress) {
            _loaderGfxInScale.x = progress;
            loaderGfxIn.localScale = _loaderGfxInScale;
        }

        public void OnStart() {
            KillTween(_tweenOut);
            loaderGfxOut.localScale = Vector3.one;
            _loaderGfxInScale = loaderGfxIn.localScale;
            _loaderGfxInScale.x = 0;
            loaderGfxIn.localScale = _loaderGfxInScale;
        }
        
         void KillTween(Tween p_tween) {
            if (p_tween != null) {
                p_tween.Kill();
            }
        }

        #endregion
    }
}