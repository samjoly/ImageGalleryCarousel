namespace SJ.ImageGallery {
    /// <summary>
    /// The LoadPriority enum defines the possible priorities for image load requests. Requests with higher priority are processed before requests with lower priority.
    /// </summary>
    public enum LoadPriority {
        Low,
        Medium,
        High,
        Critical
    }
}