using System;

namespace SJ.ImageGallery {
    /// <summary>
    /// Interface state manager for a carousel control.
    /// </summary>
    public interface IStateManager {
        /// <summary>
        /// Initializes the carousel with the specified list of carousel items.
        /// </summary>
        /// <param name="itemList">The array of carousel items.</param>
        void Init(ICarouselItem[] itemList);

        /// <summary>
        /// Applies the state list to a given array of carousel items.
        /// </summary>
        /// <param name="itemList">The array of carousel items to apply the state list.</param>
        /// <param name="direction">
        /// The direction in which the state list will be applied. A positive value indicates forward,
        /// while a negative value indicates backward.
        /// </param>
        /// <param name="onApplyStateListComplete">
        /// The action to be executed after the state list has been successfully applied. It
        /// receives an integer as a parameter indicating the number of items affected.
        /// </param>
        /// <param name="queueLength">
        /// The length of the queue used to apply the state list. This can affects the processing
        /// variable.
        /// </param>
        void ApplyStateList(ICarouselItem[] itemList, int direction, Action<int> onApplyStateListComplete,
            int queueLength);
    }
}