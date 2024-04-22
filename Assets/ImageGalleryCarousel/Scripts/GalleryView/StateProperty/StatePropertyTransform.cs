using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Applies and retrieves state for a transform. (position, rotation, and scale)
    /// </summary>
    public class StatePropertyTransform : MonoBehaviour, IStateProperty {
        private const string POSITION = "position";
        private const string ROTATION_ANGLE = "rotationAngle";
        private const string SCALE = "scale";

        private Dictionary<string, object> _stateMap;

        /// <inheritdoc />
        public void RetrieveState(Dictionary<string, object> stateMap) {
            _stateMap = stateMap;
            var t = transform;
            _stateMap.TryAdd(POSITION, t.localPosition);
            _stateMap.TryAdd(ROTATION_ANGLE, t.eulerAngles);
            _stateMap.TryAdd(SCALE, t.localScale.x);
        }

        /// <inheritdoc />
        public void ApplyState(Dictionary<string, object> stateMap) {
            _stateMap = stateMap;

            var speed = (float)_stateMap[StateManager.DURATION];
            var ease = (Ease)_stateMap[StateManager.EASE];
            var position = (Vector3)_stateMap[POSITION];
            var rotation = (Vector3)_stateMap[ROTATION_ANGLE];
            var scale = (float)_stateMap[SCALE];

            transform.DOLocalMove(position, speed).SetEase(ease);
            transform.DOLocalRotate(rotation, speed).SetEase(ease);
            transform.DOScale(scale, speed).SetEase(ease);
        }

        public void ApplyState() {
            var speed = (float)_stateMap[StateManager.DURATION];
            var ease = (Ease)_stateMap[StateManager.EASE];
            var position = (Vector3)_stateMap[POSITION];
            var rotation = (Vector3)_stateMap[ROTATION_ANGLE];
            var scale = (float)_stateMap[SCALE];

            transform.DOLocalMove(position, speed).SetEase(ease);
            transform.DOLocalRotate(rotation, speed).SetEase(ease);
            transform.DOScale(scale, speed).SetEase(ease);
        }

        public void UpdateState() {
            var t = transform;
            _stateMap[POSITION] = t.localPosition;
            _stateMap[ROTATION_ANGLE] = t.eulerAngles;
            _stateMap[SCALE] = t.localScale.x;
        }
    }
}