/*
 * StatePropertyTransform
 * 2023-11-27 - SJ
 *
 * SUMMARY:
 * Represents a class that applies and retrieves state for a transform. (position, rotation, and scale)
 *
 */


using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Represents a class that applies and retrieves state for a transform.
    /// </summary>
    public class StatePropertyTransform : MonoBehaviour, IStateProperty {
        #region Constants and Fields

        private const string POSITION = "position";
        private const string ROTATION_ANGLE = "rotationAngle";
        private const string SCALE = "scale";

        #endregion

        #region IStateProperty Implementation

        /// <inheritdoc />
        public void ApplyState(Dictionary<string, object> stateMap) {
            var speed = (float)stateMap[StateManager.DURATION];
            var ease = (Ease)stateMap[StateManager.EASE];
            var position = (Vector3)stateMap[POSITION];
            var rotation = (Vector3)stateMap[ROTATION_ANGLE];
            var scale = (float)stateMap[SCALE];

            transform.DOMove(position, speed).SetEase(ease);
            transform.DORotate(rotation, speed).SetEase(ease);
            transform.DOScale(scale, speed).SetEase(ease);
        }

        /// <inheritdoc />
        public void RetrieveState(Dictionary<string, object> stateMap) {
            var t = transform;
            stateMap.TryAdd(POSITION, t.position);
            stateMap.TryAdd(ROTATION_ANGLE, t.eulerAngles);
            stateMap.TryAdd(SCALE, t.lossyScale.x);
        }

        #endregion
    }
}