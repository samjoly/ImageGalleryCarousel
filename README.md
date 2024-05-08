# Unity Image Gallery Carousel

This image gallery carousel is designed for Unity and allows for the loading and display of images on objects manually placed in the scene. It manages transitions between images, providing designers with flexibility to create custom galleries and full control over image transitions.

## Features
### Sample Scenes

- **Asymmetry_Layout_ServerScr_VR**: This Quest3 sample features a standard carousel layout that utilizes Unity's Resource folder as the media source. The carousel can be opened or closed using gestures, and rotated in the same way. The scene includes a UI feedback mechanism for gestures.

![image](https://github.com/samjoly/ImageGalleryCarousel/blob/master/Documents/Carousel_Layout_ResourcesScr_VR.gif?raw=true)

- **Carousel_Layout_ResourcesScr**: Features a standard carousel layout utilizing Unity's Resource folder as media source.

![image](https://github.com/samjoly/ImageGalleryCarousel/blob/master/Documents/Carousel_Layout_ResourcesScr.jpg?raw=true)

- **Asymmetry_Layout_ServerScr**: Demonstrates an asymmetrical layout for the image carousel.

![image](https://github.com/samjoly/ImageGalleryCarousel/blob/master/Documents/Asymmetry_Layout_ServerScr.jpg?raw=true)

- **Rising_Layout_ServerScr**: Showcases a layout design using alpha for the carousel.

![image](https://github.com/samjoly/ImageGalleryCarousel/blob/master/Documents/Rising_Layout_ServerScr.jpg?raw=true)


### Image Source Helpers
- **ImageFetcher.php**: This script lists image files from a specified server directory and returns them in JSON format.
- **AssetListGenerator.cs (PreprocessBuild)**: Generates an asset list from a specified folder path and saves it as a JSON file. This facilitates obtaining asset names within the Resources folder without the need to hardcode or load them at runtime.

## Prerequisites
- Unity 2023.2.14f1 or later
- DOTween (Tweening engine): [DOTween](https://dotween.demigiant.com/download.php)
- Meta XR All-in-One SDK (Quest3): [Meta XR All-in-One SDK](https://assetstore.unity.com/packages/tools/integration/meta-xr-all-in-one-sdk-269657)

