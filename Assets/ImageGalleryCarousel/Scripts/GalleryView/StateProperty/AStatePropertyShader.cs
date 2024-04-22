using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Abstract base class for state-based shader properties, manages shader behavior dynamically.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Include logic to avoid material leaks.</item>
    ///     </list>
    /// </Features>
    public abstract class AStatePropertyShader : MonoBehaviour, IStateProperty {
        [SerializeField]
        [Range(0, 1)]
        public float val = 1;
        public MeshRenderer mesh;

        private Material _instanceMaterial;

        private Dictionary<string, object> _stateMap;

        // To check if alpha is different before updating shader
        private float prevVal = -1;

        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        /// <remarks>
        /// The name of stateProperty key needs to be unique
        /// </remarks>
        /// <inheritdoc />
        protected virtual string VALUE => "alpha";

        /// <summary>
        /// The unique identifier for the shader property.
        /// </summary>
        /// <returns>An integer value representing the unique identifier.</returns>
        protected virtual int SHADER_PROPERTY_ID => Shader.PropertyToID("_Alpha");

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

        /// <inheritdoc />
        public virtual void ApplyState() {
            var speed = (float)_stateMap[StateManager.DURATION];
            var newVal = (float)_stateMap[VALUE];

            if (newVal != val) {
                DOVirtual.Float(val, newVal, speed, value => {
                    val = value;
                    UpdateVal();
                });
            }
        }

        /// <inheritdoc />
        public virtual void ApplyState(Dictionary<string, object> stateMap) {
            _stateMap = stateMap;
            var speed = (float)_stateMap[StateManager.DURATION];
            var newVal = (float)_stateMap[VALUE];

            if (newVal != val) {
                DOVirtual.Float(val, newVal, speed, value => {
                    val = value;
                    UpdateVal();
                });
            }
        }

        /// <inheritdoc />
        public virtual void RetrieveState(Dictionary<string, object> stateMap) {
            _stateMap = stateMap;
            stateMap.TryAdd(VALUE, val);
            // if (stateMap.TryAdd(VALUE, val)) {
            //     Debug.LogWarning(
            //         $"Ensure that the key {VALUE} is unique. It appears that the key {VALUE} already exists in the stateMap dictionary.");
            // }
        }

        public void UpdateState() {
            _stateMap[VALUE] = val;
        }

        /// <summary>
        /// Updates the val property and assigns it to the material.
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
                if (mesh != null && mesh.sharedMaterial != null) {
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
                }
#endif
            }

            prevVal = val;
        }
    }
}