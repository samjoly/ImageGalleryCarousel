# Unity Image Gallery Carousel

This image gallery carousel is designed for Unity and allows for the loading and display of images on objects manually placed in the scene. It manages transitions between images, providing designers with flexibility to create custom galleries and full control over image transitions.

## Features
### Sample Scenes
- **Asymmetry_Layout_ServerScr**: Demonstrates an asymmetrical layout for the image carousel.



![image](https://github.com/samjoly/ImageGalleryCarousel/blob/master/Documents/Asymmetry_Layout_ServerScr.jpg?raw=true)
- **Rising_Layout_ServerScr**: Showcases a layout design using alpha for the carousel.

![image](https://github.com/samjoly/ImageGalleryCarousel/blob/master/Documents/Rising_Layout_ServerScr.jpg?raw=true)
- **Carousel_Layout_ResourcesScr**: Features a standard carousel layout utilizing Unity's Resource folder as source.

![image](https://github.com/samjoly/ImageGalleryCarousel/blob/master/Documents/Carousel_Layout_ResourcesScr.jpg?raw=true)

### Image Source Helpers
- **ImageFetcher.php**: This script lists image files from a specified server directory and returns them in JSON format.
- **AssetListGenerator.cs (PreprocessBuild)**: Generates an asset list from a specified folder path and saves it as a JSON file. This facilitates obtaining asset names within the Resources folder without the need to hardcode or load them at runtime.

## Prerequisites
- Unity 2022.3.10f1 or later
- DOTween (Tweening engine): [DOTween](https://dotween.demigiant.com/download.php)
