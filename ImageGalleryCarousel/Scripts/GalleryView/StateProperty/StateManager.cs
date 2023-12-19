/*
 * StateManager
 * 11/19/2023 - SJ
 *
 * NOTES:
 * Use by Carousel to manage the state transition:
 * - Record initial State
 * - Call ApplyState to all the StateProperty of carouselItem
 * - Pass common variables such as Duration, and Easing
 *
 * UPDATE:
 *
 * TO DO:
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace SJ.ImageGallery {
    public class StateManager : MonoBehaviour, IStateManager {
        #region Constants and Fields

        internal const string DURATION = "duration";

        internal const string EASE = "ease";

        private readonly List<Dictionary<string, object>> _stateList = new();

        private Action<int> _onApplyStateListComplete;

        #endregion

        #region Serialized Fields

        [FormerlySerializedAs("speed")]
        public float duration = 0.5f;
        public Ease ease = Ease.Linear;

        #endregion

        #region IStateManager Implementation
        /// <inheritdoc />
        public void ApplyStateList(ICarouselItem[] itemList, int direction, Action<int> onApplyStateListComplete,
            int queueLength) {
            _onApplyStateListComplete = onApplyStateListComplete;

            var nextDuration = duration;
            var nextEase = ease;
            if (queueLength > 0) {
                // Calculates the duration based on a diminishing factor, using  geometric progression.
                nextDuration = Mathf.Max(0.1f, duration * (float)Math.Pow(0.8f, queueLength));
                nextEase = Ease.Linear;
            }

            // Set the new states:
            for (var i = 0; i < itemList.Length; i++) {
                var index = i + direction;
                if (index > itemList.Length - 1) {
                    index = 0;
                } else if (index < 0) {
                    index = itemList.Length + index;
                }

                ApplyState(itemList[i], index, nextDuration, nextEase);
            }

            StartCoroutine(InvokeOnApplyStateListComplete(direction, nextDuration));
        }
        /// <inheritdoc />
        public void Init(ICarouselItem[] itemList) {
            foreach (var item in itemList) {
                Dictionary<string, object> stateMap = new() {
                    { DURATION, duration },
                    { EASE, ease }
                };
                foreach (var stateProp in item.StatePropertyList) {
                    stateProp.RetrieveState(stateMap);
                }

                _stateList.Add(stateMap);
            }
        }

        #endregion

        #region Private Methods

        private void ApplyState(ICarouselItem item, int stateIndex, float nextDuration, Ease nextEase) {
            _stateList[stateIndex][DURATION] = nextDuration;
            _stateList[stateIndex][EASE] = nextEase;
            foreach (var stateProp in item.StatePropertyList) {
                stateProp.ApplyState(_stateList[stateIndex]);
            }
        }

        private IEnumerator InvokeOnApplyStateListComplete(int direction, float nextDuration) {
            yield return new WaitForSeconds(nextDuration);
            _onApplyStateListComplete?.Invoke(direction);
        }

        #endregion
    }
}