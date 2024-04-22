namespace SJ.ImageGallery {
    /// <summary>
    /// Interface for a loading graphics component.
    /// </summary>
    public interface ILoadingGfx {
        void OnComplete();
        void OnProgress(float progress);
        void OnStart();
    }
}