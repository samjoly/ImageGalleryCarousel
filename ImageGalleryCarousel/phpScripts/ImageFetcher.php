<?php
// This PHP script is designed to list image files from a specified directory and return them in a JSON format. It checks if the directory exists, reads the files, filters them by extension, and outputs their names in JSON format. It also sorts the files by name.


// Define a default image folder name
define('DEFAULT_IMAGE_FOLDER', 'img');

// Function to fetch image files from a directory
function getImageFiles($directory) {
    $images = [];
    $supported_file_formats = array('gif', 'jpg', 'jpeg', 'png');

    // Check if the directory exists and is readable
    if (is_dir($directory) && is_readable($directory)) {
        $dir = opendir($directory);
        while (($file = readdir($dir)) !== false) {
            $file_extension = strtolower(pathinfo($file, PATHINFO_EXTENSION));
            if (in_array($file_extension, $supported_file_formats)) {
                // Sanitize the file name to prevent XSS attacks
                $images[] = htmlspecialchars($file);
            }
        }
        closedir($dir);
    } else {
        // Use htmlspecialchars to prevent potential XSS attacks
        echo "Error: Directory not found or not readable: " . htmlspecialchars($directory);
        return [];
    }

    // Sort the images array
    sort($images);

    return $images;
}

// Retrieve the folder parameter from the URL, use default if not provided
// Sanitize and validate the 'folder' parameter to prevent directory traversal attacks
$folder = isset($_GET['folder']) ? preg_replace('/[^A-Za-z0-9_\-]/', '', $_GET['folder']) : DEFAULT_IMAGE_FOLDER;
$directoryPath = __DIR__ . '/' . $folder;

// Validate that the directory is within a list of allowed directories (Whitelist approach)
$allowedDirectories = [__DIR__ . '/img', __DIR__ . '/AnotherSafeDirectory'];
if (!in_array($directoryPath, $allowedDirectories)) {
    echo "Error: Access to this directory is not allowed";
    exit; // Stop the script execution if the directory is not in the allowed list
}

$images = getImageFiles($directoryPath);

// Convert the array to JSON in the specified format
echo json_encode(array("assets" => $images));
?>
