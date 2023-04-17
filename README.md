# Dango Graphic
A small portable Windows GUI application to quickly modify the graphics of Little Busters! English Edition.

# Credits
Dango Graphic relies on and includes binaries for [LuckSystem](https://github.com/wetor/LuckSystem).

# Installation
Simply [download dangographic.zip]() and extract the contents to any location of the user's choosing, so long as it has user write permissions.

# Usage

![image](https://user-images.githubusercontent.com/93227270/232514754-6f000919-230f-4252-a577-0f47c52662cf.png)


After running through a first time setup an locating Little Busters' installation directory, the user will have a number of options...

### .PAK File List
Double-clicking on an archive in the .PAK file list will display a list of the textures contained within that archive.

### Raw Texture List

Double-clicking on a texture from the raw texture list will select the file and load it into the preview window.

### Preview Window

The preview window shows the currently selected texture that will be replaced, the selected image to replace that texture with, and some data about both.

### Open Image

The open image button allows the user to select any PNG file for importing.

### Change Directory

If for any reason the program is referencing the incorrect directory or no directory at all, the user may use the change directory button to fix the issue.

### Export Texture

The export texture button allows the user to save the currently selected texture to a .PNG file.

### Import Image

Assuming the user has selected both a texture and an image, the import image button will replace the texture with the custom image. If no backup for the currently selected .PAK file exists, a backup will be created. Once confirmation that the texture has been replaced, the user may start Little Busters to see their changes.

### Restore Texture

The restore texture button reverts the currently selected texture to its default state. This only functions if the user has a backup of the currently selected .PAK file.

### Restore All

The restore all button will revert every texture in the currently selected .PAK file to its default state. This only functions if the user has a backup of the currently selected .PAK file.

### CZ0 Pixel Offset Fix

The CZ0 Pixel Offset Fix toggle is used to get around a limitation of reimporting CZ0 type textures. See [Texture Editing]() for more information.

### [Video Demo (May contain spoilers!)](https://files.catbox.moe/gzkjz0.mp4).

# Texture Editing



# Disclaimer
Much like my last tool, this program was almost entirely made using Microsoft's Bing Chat. I was mostly curious of the bot's ability to create a larger UI-focussed program, even if it is a frontend for [LuckSystem](https://github.com/wetor/LuckSystem), which does all the heavy lifting.

This program is largely untested, so please use it at your own risk.
