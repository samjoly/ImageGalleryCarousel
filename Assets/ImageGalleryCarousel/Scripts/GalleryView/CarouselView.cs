using System.Collections.Generic;
using UnityEngine;

namespace SJ.ImageGallery {
    /// <summary>
    /// Represents a carousel view in an image gallery.
    /// </summary>
    /// <Features>
    ///     <list type="bullet">
    ///         <item>Initialization with data loader.</item>
    ///         <item>Navigation through items (next, previous).</item>
    ///         <item>State management for animations and transitions.</item>
    ///         <item>Support for both looping and non-looping carousel.</item>
    ///         <item>Dynamic loading and ordering of carousel items.</item>
    ///         <item>Debug mode for development and testing.</item>
    ///     </list>
    /// </Features>
    public class CarouselView : MonoBehaviour, IGalleryView {
        /// <summary>
        /// Reference to the parent Transform of carousel items, used for grouping and manipulating them collectively.
        /// </summary>
        public Transform itemGroup;

        /// <summary>
        /// The total number of data content. This value determines the initial index and order of items. While you can set
        /// this value manually for visualization in the editor, it will be overridden by data information at runtime.
        /// </summary>
        public int contentCount = 8;

        /// <summary>
        /// Flag indicating whether the carousel loops back to the start after reaching the end.
        /// </summary>
        public bool isLooping = true;

        /// <summary>
        /// Offset value for positioning or initializing carousel items, affecting the starting point in the item array.
        /// This provides flexibility in setting the layout of the carousel in relation to the data.
        /// </summary>
        public int offset = 3;

        /// <summary>
        /// Specifies the index of an item in the carousel that should be loaded or displayed with priority.
        /// This provides flexibility in setting the layout of the carousel in relation to the data.
        /// </summary>
        public int loadPriorityIndex = 2;

        /// <summary>
        /// Enables debug mode for displaying additional information and enabling extra functionalities during development and
        /// testing.
        /// </summary>
        public bool isDebugMode = true;

        private readonly bool isDataOrderFlip = true;

        private readonly Queue<int> moveQueue = new();

        private IDataLoader _galleryGalleryData;
        private bool _isMoving;

        // Manage state animation:
        private IStateManager _stateManager;
        private int itemCount = 8;

        // Keep track of order of item:
        private ICarouselItem[] itemOrderCurrentList;

        // if not loop: hide content and disable moving:
        private int maxLastContentOnPrev;
        private int minFirstContentOnNext;

        private void Awake() {
            _stateManager = gameObject.GetOrAddComponent<StateManager>();
        }

        /// <inheritdoc />
        public void Init(IDataLoader galleryData) {
            _galleryGalleryData = galleryData;
            contentCount = galleryData.dataList.Length;
            SetInitialIndex();
            _stateManager.Init(itemOrderCurrentList);
        }

        /// <inheritdoc />
        public void Next() {
            Move(+1);
        }

        /// <inheritdoc />
        public void Prev() {
            Move(-1);
        }

        private void AddToQueue(int direction) {
            var isClear = ClearQueue(direction * -1);
            if (!isClear) {
                moveQueue.Enqueue(direction);
            }
        }

        private bool ClearQueue(int direction) {
            // if switch direction clear queue
            if (moveQueue.Count > 0) {
                if (moveQueue.Peek() == direction) {
                    moveQueue.Clear();
                    return true;
                    // Just stop if switching direction
                }
            }

            return false;
        }

        private void Move(int direction) {
            // Get the current index reference based on direction:
            var newContentIndex = direction == 1 ? itemOrderCurrentList[0].Index : itemOrderCurrentList[^2].Index;
            // Add the new direction:
            newContentIndex += direction;


            // Ensure that newContentIndex is always within the range [0, contentCount - 1]
            newContentIndex = (newContentIndex + contentCount) % contentCount;

            if (!isLooping) {
                if (direction == -1) {
                    // Stop moving:
                    if (newContentIndex == maxLastContentOnPrev) {
                        ClearQueue(direction);
                        return;
                    }

                    // Hide or show content:
                    itemOrderCurrentList[^1].SetActive(newContentIndex <= maxLastContentOnPrev);
                } else {
                    if (newContentIndex == minFirstContentOnNext) {
                        ClearQueue(direction);
                        return;
                    }

                    // Hide or show content:
                    itemOrderCurrentList[^1].SetActive(!(newContentIndex < minFirstContentOnNext));
                }
            }

            if (_isMoving) {
                AddToQueue(direction);
                return;
            }

            _isMoving = true;

            // Pass the new index to load content:
            itemOrderCurrentList[^1].UpdateIndex(newContentIndex);

            // Apply state:
            _stateManager.ApplyStateList(itemOrderCurrentList, direction, OnMoveComplete, moveQueue.Count);
        }

        private void OnMoveComplete(int direction) {
            // Prep the list for the next Move:
            itemOrderCurrentList = ListUtils.OffsetArray(itemOrderCurrentList, direction);
            _isMoving = false;
            // Check if moveRequest inQueue:
            if (moveQueue.Count > 0) {
                Move(moveQueue.Dequeue());
            }
        }

        // Sets the initial index and properties for the items in the carousel.
        private void SetInitialIndex() {
            itemOrderCurrentList = transform.GetComponentsInChildren<ICarouselItem>(true);
            itemCount = itemOrderCurrentList.Length;
            if (!isLooping) {
                maxLastContentOnPrev = contentCount - (itemCount - offset);
                minFirstContentOnNext = offset - 1;
            }

            // Function to wrap the index within bounds
            int GetWrappedIndex(int index, int count) {
                return (index % count + count) % count;
            }

            // Function to set item properties, index, and isActive
            void SetItemProperties(int index) {
                var itemIndex = GetWrappedIndex(index, itemCount); // Index in itemOrderCurrentList
                var item = itemOrderCurrentList[itemIndex];
                item.galleryDataLoader = _galleryGalleryData;

                var contentListIndex = isDataOrderFlip ? contentCount - itemIndex - 1 : itemIndex;
                contentListIndex += offset;
                contentListIndex = GetWrappedIndex(contentListIndex, contentCount); // Ensure it's in bounds

                if (isLooping) {
                    item.UpdateIndex(contentListIndex);
                } else {
                    if (Application.isPlaying) {
                        // Hide content if not looping
                        var isVisible = contentListIndex < maxLastContentOnPrev;
                        item.SetActive(isVisible);
                    }

                    item.UpdateIndex(contentListIndex);
                }

                item.IsDebugMode = isDebugMode;
            }

            // Set the item at loadPriorityIndex first
            SetItemProperties(loadPriorityIndex);

            if (isLooping) {
                // Zigzag iteration: alternate between closest left and right items
                for (var i = 1; i < itemCount; i++) {
                    // Determine if the current iteration is even (right) or odd (left)
                    if (i % 2 != 0) {
                        // Left item
                        SetItemProperties(loadPriorityIndex - (i + 1) / 2);
                    } else {
                        // Right item
                        SetItemProperties(loadPriorityIndex + i / 2);
                    }
                }
            } else {
                // Set item before loadPriorityIndex
                for (var i = loadPriorityIndex - 1; i >= 0; i--) {
                    SetItemProperties(i);
                }

                // Set item after loadPriorityIndex
                for (var i = loadPriorityIndex + 1; i < itemCount; i++) {
                    SetItemProperties(i);
                }
            }
        }

#if UNITY_EDITOR
        // Show changes of setting in the inspector in editor:

        private void OnEnable() {
            if (!Application.isPlaying) {
                SetInitialIndex();
            }
        }

        private void OnValidate() {
            if (!Application.isPlaying) {
                SetInitialIndex();
            }
        }

#endif
    }
}