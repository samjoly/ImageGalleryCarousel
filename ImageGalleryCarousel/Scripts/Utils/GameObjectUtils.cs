/*
 * ComponentUtils
 * 2023-11-29 - SJ
 *
 * NOTES:
 * Provides utility functions and extensions for Unity components.
 *
 */

using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Provides utility functions and extensions for Unity components.
    /// </summary>
    public static class GameObjectUtils {
        /// <summary>
        ///     Gets an existing component of type T attached to the GameObject or adds one if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">The type of Component to get or add.</typeparam>
        /// <param name="gameObject">The GameObject to which the component is attached.</param>
        /// <returns>The existing or newly added component.</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component {
            var component = gameObject.GetComponent<T>();
            if (component == null) {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}