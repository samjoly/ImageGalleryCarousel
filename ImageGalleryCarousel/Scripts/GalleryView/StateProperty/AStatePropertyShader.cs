/*
 * AStatePropertyShader
 * 2023-12-08 - SJ
 *
 * SUMMARY:
 * Abstract base class for state-based shaders properties.
 *
 * FEATURES:
 * - Include logic to avoid material leak
 *
 * TO DO:
 * prevent duplicate key
 */

using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Abstract base class for state-based shaders properties.
    /// </summary>
    public abstract class AStatePropertyShader : MonoBehaviour, IStateProperty {
        #region Constants and Fields

        private Material _instanceMaterial;

        #endregion

        #region Serialized Fields
        [SerializeField]
        [Range(0, 1)]
        public float val = 1;
        public MeshRenderer mesh;
        // To check if alpha is different before updating shader
        private float prevVal = -1;

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the value of the property.
        /// </summary>
        /// <remarks>
        ///     The name of stateProperty key needs to be unique
        /// </remarks>
        //TODO: prevent duplicate key

        /// <inheritdoc />
        protected virtual string VALUE => "alpha";

        /// <summary>
        ///     The unique identifier for the shader property.
        /// </summary>
        /// <returns>An integer value representing the unique identifier.</returns>
        protected virtual int SHADER_PROPERTY_ID => Shader.PropertyToID("_Alpha");

        #endregion

        #region IStateProperty Implementation

        /// <inheritdoc />
        public virtual void ApplyState(Dictionary<string, object> stateMap) {
            var speed = (float)stateMap[StateManager.DURATION];
            var newVal = (float)stateMap[VALUE];

            if (newVal != val) {
                DOVirtual.Float(val, newVal, speed, value => {
                    val = value;
                    UpdateVal();
                });
            }
        }

        /// <inheritdoc />
        public virtual void RetrieveState(Dictionary<string, object> stateMap) {
            stateMap.TryAdd(VALUE, val);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Updates the val property and assigns it to the material.
        /// </summary>
        protected virtual void UpdateVal() {
            val = Mathf.Round(val * 100f) / 100f;

            if (prevVal == val) {
                return;
            }
#if UNITY_EDITOR
            // Prevent Editor error, because Prefab cannot access mesh.material
            if (PrefabUtility.IsPartOfPrefabAsset(this)) {
                return;
            }
#endif
            if (Application.isPlaying) {
                // No need for new material instance in runtime
                mesh.material.SetFloat(SHADER_PROPERTY_ID, val);
            } else {

#if UNITY_EDITOR
                // Avoid memory leak in editor mode, by creating new material instance
                var isInstanceMaterialNull = _instanceMaterial == null;
                var isDifferentInstanceMaterial = mesh.sharedMaterial != _instanceMaterial;
                if (isInstanceMaterialNull || isDifferentInstanceMaterial) {
                    if (isDifferentInstanceMaterial) {
                        DestroyImmediate(_instanceMaterial);
                    }

                    _instanceMaterial = new Material(mesh.sharedMaterial);
                    mesh.material = _instanceMaterial;
                }

                _instanceMaterial.SetFloat(SHADER_PROPERTY_ID, val);
#endif
            }

            prevVal = val;
        }

        #endregion

        #region Unity Methods

        private void OnDisable() {
            if (_instanceMaterial != null) {
                Destroy(_instanceMaterial);
            }
        }

#if UNITY_EDITOR

        private void OnValidate() {
            if (!Application.isPlaying) {
                UpdateVal();
            }
        }
#endif

        #endregion
    }
}