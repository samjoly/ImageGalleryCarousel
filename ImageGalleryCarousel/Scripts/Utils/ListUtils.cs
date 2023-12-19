/*
 * ListUtils
 * 11/19/2023 - SJ
 *
 * NOTES:
 * Provides utility functions for working with lists.
 *
 */

using System.Collections.Generic;
using System.Linq;

namespace SJ.ImageGallery {
    /// <summary>
    ///     Provides utility functions for working with lists.
    /// </summary>
    /// <remarks>
    ///     This class includes various methods to manipulate and transform lists,
    ///     such as offsetting elements within a list. The methods are designed to be
    ///     generic and work with any type of list.
    /// </remarks>
    public static class ListUtils {
        /// <summary>
        ///     Offsets the elements in the given list by the specified amount.
        /// </summary>
        /// <param name="list">The list to be offset.</param>
        /// <param name="offset">
        ///     The number of positions by which to offset the list elements.
        ///     Positive values offset from the end, negative values from the start.
        /// </param>
        /// <returns>
        ///     A new list with elements offset by the specified amount. Returns the original
        ///     list if it's null or empty.
        /// </returns>
        /// <example>
        ///     <code>
        /// var myList = new List&lt;int&gt; { 1, 2, 3, 4, 5 };
        /// var offsetListPositive = ListUtils.OffsetList(myList, 2);
        /// // offsetListPositive will be { 4, 5, 1, 2, 3 }
        /// </code>
        ///     This example shows how a list { 1, 2, 3, 4, 5 } is transformed when an offset of 2 is applied.
        /// </example>
        /// <example>
        ///     <code>
        /// var offsetListNegative = ListUtils.OffsetList(myList, -2);
        /// // offsetListNegative will be { 3, 4, 5, 1, 2 }
        /// </code>
        ///     This example shows how the same list { 1, 2, 3, 4, 5 } is transformed when an offset of -2 is applied.
        /// </example>
        public static List<T> OffsetList<T>(List<T> list, int offset) {
            if (list == null || list.Count == 0) {
                return list;
            }

            var count = list.Count;
            // Normalize offset to handle values larger than list size
            offset = offset % count;

            // Adjust offset to ensure it's positive and within the bounds of the list length
            if (offset < 0) {
                offset += count;
            }

            return list.Skip(count - offset).Concat(list.Take(count - offset)).ToList();
        }

        /// <summary>
        ///     Offsets the elements in the given array by the specified amount.
        /// </summary>
        /// <param name="array">The array to be offset.</param>
        /// <param name="offset">
        ///     The number of positions by which to offset the array elements.
        ///     Positive values offset from the end, negative values from the start.
        /// </param>
        /// <returns>
        ///     A new array with elements offset by the specified amount. Returns the original
        ///     array if it's null or empty.
        /// </returns>
        /// <example>
        ///     <code>
        /// var myArray = new int[] { 1, 2, 3, 4, 5 };
        /// var offsetArrayPositive = ArrayUtils.OffsetArray(myArray, 2);
        /// // offsetArrayPositive will be { 4, 5, 1, 2, 3 }
        /// </code>
        ///     This example shows how an array { 1, 2, 3, 4, 5 } is transformed when an offset of 2 is applied.
        /// </example>
        /// <example>
        ///     <code>
        /// var offsetArrayNegative = ArrayUtils.OffsetArray(myArray, -2);
        /// // offsetArrayNegative will be { 3, 4, 5, 1, 2 }
        /// </code>
        ///     This example shows how the same array { 1, 2, 3, 4, 5 } is transformed when an offset of -2 is applied.
        /// </example>
        public static T[] OffsetArray<T>(T[] array, int offset) {
            if (array == null || array.Length == 0) {
                return array;
            }

            var count = array.Length;
            offset = offset % count;

            if (offset < 0) {
                offset += count;
            }

            return array.Skip(count - offset).Concat(array.Take(count - offset)).ToArray();
        }
    }
}