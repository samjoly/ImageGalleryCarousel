/*
 * ILoadingGfx
 * 2023-12-08 - SJ
 *
 * SUMMARY:
 * Represents the interface for a loading graphics component.
 *
 */

namespace SJ.ImageGallery {
    /// <summary>
    ///     Represents the interface for a loading graphics component.
    /// </summary>
    public interface ILoadingGfx {
        void OnComplete();
        void OnProgress(float progress);
        void OnStart();
    }
}