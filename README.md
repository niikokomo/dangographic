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

The CZ0 Pixel Offset Fix toggle is used to get around a limitation of reimporting CZ0 type textures. See [Texture Editing & Limitations]() for more information.

### [Video Demo (May contain spoilers!)](https://files.catbox.moe/gzkjz0.mp4).

# Texture Editing & Limitations

Due to a few limitations in both LuckSystem and Little Busters, textures imported with Dango Graphic must respect a few rules. 

### Imported images can not have different dimensions/resolution from the texture they are replacing.

There might very well be a way around this, the header data of certain textures contain values for its resolution. Limited testing has resulted in success for some textures and failure for others. Note that even if this does get implemented, the game's viewport is locked at 1280 x 720, so using this would only be useful for modifying the scale/boundaries of textures, not upscaling.

### CZ0 type textures are displaced to the left by two pixels.

This is due to a silly quirk of LuckSystem. The CZ0 Pixel Offset Fix toggle shifts the coordinates of the reimported texture two pixels to the right, visually fixing the issue. However, this fix may not be applied to everything as certain child textures (character expressions) depend on the coordinatess of their parent texture (character bodies). This video helps explain the error in detail and shows when the fix should be used.

https://user-images.githubusercontent.com/93227270/232650887-9c30fbb2-b6fb-4bd7-9b4a-5d90bf599de3.mp4

### 00 Byte Error

Due to a fault in the way LuckSystem reimports CZ3 type images, the imported result by default suffers from massive color corruption. Dango Graphic hex-edits the converted file, subtracting one from the value at 0x24, before reimporting to fix this issue. But if that value is for any reason already 00, this fix fails to work. If you run into any case of a 00 byte error, please upload it and the PNG you attempted to import to the issues page of this repository.

# Disclaimer
Much like my last tool, this program was almost entirely made using Microsoft's Bing Chat. I was mostly curious of the bot's ability to create a larger UI-focused program, even if it is a frontend for [LuckSystem](https://github.com/wetor/LuckSystem), which does all the heavy lifting.

This program is largely untested, so please use it at your own risk.
