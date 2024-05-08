using System.Collections.Generic;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Manages a group of followers that mimic the movement, rotation, and scale of a designated leader object within a scene.
    /// This class is designed for scenarios where multiple objects need to maintain relative transformations to a leader object.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Synchronize position, rotation, and scale of follower objects with a leader object.</item>
    ///         <item>Dynamically add or remove followers during runtime.</item>
    ///         <item>Maintain initial relative transformations between the leader and its followers.</item>
    ///     </list>
    /// </Features>
    /// <note>
    /// Useful for creating dynamic groups of objects that move together.
    /// </note>
    public class FollowerManager : MonoBehaviour {
        public GameObject leader;
        public List<GameObject> followers;

        public bool isPosition = true;
        public bool isRotation;
        public bool isScale;

        // List to store initial relative positions, rotations, and scales of followers
        private List<Vector3> _initialRelativePositions;
        private List<Quaternion> _initialRelativeRotations;
        private List<Vector3> _initialRelativeScales;

        private void Start() {
            _initialRelativePositions = new List<Vector3>();
            _initialRelativeRotations = new List<Quaternion>();
            _initialRelativeScales = new List<Vector3>();

            foreach (var follower in followers) {
                if (follower != null) {
                    var relativePosition = follower.transform.position - leader.transform.position;
                    _initialRelativePositions.Add(relativePosition);

                    var relativeRotation = Quaternion.Inverse(leader.transform.rotation) * follower.transform.rotation;
                    _initialRelativeRotations.Add(relativeRotation);

                    var relativeScale = new Vector3(
                        follower.transform.localScale.x / leader.transform.localScale.x,
                        follower.transform.localScale.y / leader.transform.localScale.y,
                        follower.transform.localScale.z / leader.transform.localScale.z
                    );
                    _initialRelativeScales.Add(relativeScale);
                }
            }
        }

        private void LateUpdate() {
            for (var i = 0; i < followers.Count; i++) {
                if (followers[i] != null) {
                    if (isPosition) {
                        followers[i].transform.position = leader.transform.position + _initialRelativePositions[i];
                    }

                    if (isRotation) {
                        followers[i].transform.rotation = leader.transform.rotation * _initialRelativeRotations[i];
                    }

                    if (isScale) {
                        followers[i].transform.localScale = new Vector3(
                            leader.transform.localScale.x * _initialRelativeScales[i].x,
                            leader.transform.localScale.y * _initialRelativeScales[i].y,
                            leader.transform.localScale.z * _initialRelativeScales[i].z
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Adds a follower object to the FollowerManager.
        /// </summary>
        /// <param name="follower">The GameObject to be added as a follower.</param>
        /// <returns>Returns true if the follower was successfully added, false otherwise.</returns>
        public bool AddFollower(GameObject follower) {
            if (follower != null && !followers.Contains(follower)) {
                followers.Add(follower);
                _initialRelativePositions.Add(follower.transform.position - leader.transform.position);
                _initialRelativeRotations.Add(Quaternion.Inverse(leader.transform.rotation) *
                                              follower.transform.rotation);
                _initialRelativeScales.Add(new Vector3(
                    follower.transform.localScale.x / leader.transform.localScale.x,
                    follower.transform.localScale.y / leader.transform.localScale.y,
                    follower.transform.localScale.z / leader.transform.localScale.z
                ));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a follower object from the FollowerManager.
        /// </summary>
        /// <param name="follower">The GameObject to be removed as a follower.</param>
        /// <returns>Returns true if the follower was successfully removed, false otherwise.</returns>
        public bool RemoveFollower(GameObject follower) {
            var index = followers.IndexOf(follower);
            if (index != -1) {
                followers.RemoveAt(index);
                _initialRelativePositions.RemoveAt(index);
                _initialRelativeRotations.RemoveAt(index);
                _initialRelativeScales.RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all follower objects from the FollowerManager.
        /// </summary>
        public void RemoveAllFollowers() {
            followers.Clear();
            _initialRelativePositions.Clear();
            _initialRelativeRotations.Clear();
            _initialRelativeScales.Clear();
        }
    }
}