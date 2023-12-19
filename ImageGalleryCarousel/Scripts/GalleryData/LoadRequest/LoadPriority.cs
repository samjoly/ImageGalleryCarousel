namespace SJ.ImageGallery {
    // The LoadPriority enum defines the possible priorities for image load requests. Requests with higher priority are processed before requests with lower priority.
    public enum LoadPriority {
        Low,
        Medium,
        High,
        Critical
    }
}