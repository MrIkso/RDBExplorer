# RDB Explorer
![License](https://img.shields.io/badge/License-MIT-blue.svg)
![Language](https://img.shields.io/badge/Language-C%23-purple.svg)
![Platform](https://img.shields.io/badge/Platform-Windows-teal.svg)

A utility for browsing, extracting, and modifying **.fdata**/**archive_xx.bin** files from **Nioh 3**

## Disclaimer
This project is intended for educational and research purposes only. The authors are not responsible for any damage caused to your game files. Always back up your archives before modifying them. Required .NET 10 for running

## Features

- **Archive Exploration:** Open `.rdb` files to view internal contents with full metadata support.
- **Search & Filtering:** Real-time search by filename, KTID (Hex ID), or Type name.
- **File Extraction:**
  - Extract individual files via the right-click context menu.
  - Bulk extract the entire archive.
- **Data Injection:** Replace any file within the RDB archive with your own modified version.
- **Localization Tools:**
  - **Unpack Locales:** Batch convert binary language files into editable `.csv` files.
  - **Pack Locales:** Recompile modified `.csv` files back into binary format.
- **Archive Management:**
   - **Bin Unpacker/Packer:** Handle `archive_xx.bin` files using manifest-based processing.
   - **Name Grabber:** Scan internal data for original paths and export them to recover filenames.
   - **Magic Header Scanner:** Identify unknown file types by scanning file headers.

## How to Use

### 1. Opening Archives
1. Go to **File > Open** and select an `.rdb` file.
2. The tool will automatically load the file list (ensure the associated `.rdx` file is in the same folder).
3. Use the **Search Bar** at the top to filter files by name or ID instantly.

### 2. Extracting and Modifying
- **To Extract:** Right-click any file in the list and select **Extract Selected**, or use **Tools > Extract All**.
- **To Modify (Inject):** Right-click a file and select **Inject New Data**. Choose your modified file, and the tool will update the archive container automatically.

### 3. Bin Archives
- **Unpack:** Use **Tools > Unpack Bin Archive** to extract files from `archive_xx.bin`. This process generates a `manifest.json` file.
- **Pack:** Use **Tools > Pack Bin Archive** and select the `.json` manifest created during unpacking to rebuild the archive. After processing archive saved to `Packed` folder.

### 4. Localization (Modding Text)
The localization process follows a specific multi-step workflow:
1. **Unpack Bin:** Follow the "Bin Archives" step above to extract the binary locale files from the game's bin archives.
2. **Convert to CSV:** Go to **Locale > Unpack Locales** and select the folder containing the extracted binary files. Unpacked CSVs will be placed in the `Export` folder.
3. **Edit:** Open and modify the `.csv` files using any text editor or spreadsheet software (like Excel).
4. **Convert to Binary:** Use **Locale > Pack Locales** to convert the edited CSVs back to binary format. The new files will be saved in the `Result` folder.
5. **Prepare for Rebuild:** Copy the original `manifest.json` into the `Result` folder alongside your newly created binary locale files.
6. **Rebuild Bin:** Use **Tools > Pack Bin Archive** and select the `manifest.json` inside the `Result` folder to build the final `.bin` archive for the game.
7. **Install:** Replace the original game `.bin` files with your newly created ones.

## Technical Details
- **Sortable Columns:** Click on **Name**, **Type**, **Size**, or **Container** headers to sort the view.
- **Requirements:** Built with **.NET 10.0**, requires Windows 10/11.


## Acknowledgments
Special thanks to the developers of any third-party libraries and information used in this project:
- [ImageView.PictureBox](https://github.com/tonyp7/ImageView)
- [BCnEncoder.Net](https://github.com/Nominom/BCnEncoder.NET)
- [AssetRipper.TextureDecoder](https://github.com/AssetRipper/TextureDecoder)
- [G1T](https://github.com/hearhellacopters/G1T) - @hearhellacopters for amazing g1t file researh
- [Metanoia](https://github.com/Ploaj/Metanoia) for amazing implementatin 3D model viewing
- [Nioh3-Model-Texture-Mapping-Database](https://github.com/kassent/Nioh3-Model-Texture-Mapping-Database) for amazing texture database
- [Hex.Box](https://github.com/harborsiem/Be.HexEditor) fork
- [FastColoredTextBox](https://github.com/vurdalakov/FastColoredTextBox.NET10) fork
- [Project-G1M] (https://github.com/Joschuka/Project-G1M) Joschuka for G1M file research
- etc.

Icons used from Visual Studio Image Library 2022
https://www.microsoft.com/en-us/download/details.aspx?id=35825