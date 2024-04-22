using System.Collections.Generic;

namespace SJ.ImageGallery {
    /// <summary>
    /// Interface for retrieving and applying state from/to a dictionary.
    /// </summary>
    public interface IStateProperty {
        /// <summary>
        /// Retrieves the state from a dictionary and assigns it to the specified object.
        /// </summary>
        /// <param name="stateMap">The dictionary containing the state information.</param>
        void RetrieveState(Dictionary<string, object> stateMap);

        /// <summary>
        /// Update the state from a dictionary and assigns it to the specified object.
        /// </summary>
        void UpdateState();

        /// <summary>
        /// Applies the state defined in the given state map.
        /// </summary>
        void ApplyState();

        /// <summary>
        /// Applies the state defined in the given state map.
        /// </summary>
        /// <param name="stateMap">The dictionary containing the state information.</param>
        void ApplyState(Dictionary<string, object> stateMap);
    }
}