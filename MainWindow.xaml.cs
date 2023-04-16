using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private string litbusDirectory;
        private string workingDirectory;
        private string userSelectedImagePath;

        private TextBlock displayedImageInfo;
        private TextBlock ResolutionWarningTextBlock;
        private Image userSelectedImage;
        private TextBlock userSelectedImageInfo;

        public MainWindow()
        {
            InitializeComponent();
            ResolutionWarningTextBlock = (TextBlock)FindName("ResolutionWarning");
            displayedImageInfo = (TextBlock)FindName("DisplayedImageInfo");
            userSelectedImageInfo = (TextBlock)FindName("UserSelectedImageInfo");
            userSelectedImage = (Image)FindName("UserSelectedImage");
            LoadConfig();
            PopulateFileList();
            FileList.PreviewKeyDown += FileList_PreviewKeyDown;
            ContentList.PreviewKeyDown += ContentList_PreviewKeyDown;
        }

        private void LoadConfig()
        {
            string configPath = "config.txt";
            if (File.Exists(configPath))
            {
                litbusDirectory = File.ReadAllText(configPath);
                if (!Directory.Exists(litbusDirectory))
                {
                    MessageBox.Show("Please select the LITBUS_WIN32.exe file in your Little Busters! English Edition directory.", "Select LITBUS_WIN32.exe", MessageBoxButton.OK, MessageBoxImage.Information);
                    SelectLitbusDirectory();
                }
            }
            else
            {
                MessageBox.Show("Please select the LITBUS_WIN32.exe file in your Little Busters! English Edition directory.", "Select LITBUS_WIN32.exe", MessageBoxButton.OK, MessageBoxImage.Information);
                SelectLitbusDirectory();
            }
            workingDirectory = Directory.GetCurrentDirectory() + "\\working";
            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }
        }

        private void SelectLitbusDirectory()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "LITBUS_WIN32.exe|LITBUS_WIN32.exe";
            if (openFileDialog.ShowDialog() == true)
            {
                litbusDirectory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                File.WriteAllText("config.txt", litbusDirectory);
            }
        }

        private void PopulateFileList()
        {
            if (Directory.Exists("dict"))
            {
                string[] files = Directory.GetFiles("dict", "*.txt");
                foreach (string file in files)
                {
                    string fileName = System.IO.Path.GetFileName(file);
                    string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
                    FileList.Items.Add(fileNameWithoutExtension + ".PAK");
                }
            }
            else
            {
                MessageBox.Show("The 'dict' folder was not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FileList_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (FileList.SelectedItem != null)
            {
                // Unload currently loaded image
                DisplayedImage.Source = null;
                // Wipe DisplayedImageInfo
                DisplayedImageInfo.Text = string.Empty;
                // Disable PixelOffsetFixCheckBox
                PixelOffsetFixCheckBox.IsEnabled = false;
                string selectedItem = FileList.SelectedItem.ToString();
                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(selectedItem);
                string file = "dict/" + fileNameWithoutExtension + ".txt";
                string[] lines = File.ReadAllLines(file);
                ContentList.Items.Clear();
                foreach (string line in lines)
                {
                    ContentList.Items.Add(line);
                }
            }
        }
        private void FileList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Call same function as when user double-clicks an item in FileList
                FileList_MouseDoubleClick(sender, null);
            }
        }

        private void ContentList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Call same function as when user double-clicks an item in ContentList
                ContentList_MouseDoubleClick(sender, null);
            }
        }

        private async void ContentList_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (ContentList.SelectedItem != null && ContentList.SelectedItem.ToString().EndsWith(".CZ0"))
            {
                PixelOffsetFixCheckBox.IsEnabled = true;
            }
            else
            {
                PixelOffsetFixCheckBox.IsEnabled = false;
            }

            if (ContentList.SelectedItem != null)
            {
                if (DisplayedImage.Source != null)
                {
                    BitmapImage bitmap = DisplayedImage.Source as BitmapImage;
                    if (bitmap != null && bitmap.StreamSource != null)
                    {
                        bitmap.StreamSource.Dispose();
                    }
                    DisplayedImage.Source = null;
                }
                ClearWorkingDirectory();

                string pakFile = litbusDirectory + "\\files\\" + System.IO.Path.GetFileNameWithoutExtension(FileList.SelectedItem.ToString()) + ".PAK";
                string outputFolder = workingDirectory + "\\" + ContentList.SelectedItem.ToString();
                string name = ContentList.SelectedItem.ToString().Split('.')[0];
                Process process = Process.Start(new ProcessStartInfo
                {
                    FileName = "dependencies\\LuckSystem.exe",
                    Arguments = $"pak extract -i \"{pakFile}\" -o \"{outputFolder}\" -n \"{name}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                await Task.Run(() => process.WaitForExit());

                string inputFile = workingDirectory + "\\" + ContentList.SelectedItem.ToString();
                string outputFile = workingDirectory + "\\" + name + ".png";
                process = Process.Start(new ProcessStartInfo
                {
                    FileName = "dependencies\\LuckSystem.exe",
                    Arguments = $"image export -i \"{inputFile}\" -o \"{outputFile}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                await Task.Run(() => process.WaitForExit());

                if (File.Exists(outputFile))
                {
                    try
                    {
                        BitmapImage newBitmap = new BitmapImage();
                        newBitmap.BeginInit();
                        newBitmap.CacheOption = BitmapCacheOption.OnLoad;
                        newBitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        newBitmap.UriSource = new Uri(outputFile);
                        newBitmap.EndInit();
                        DisplayedImage.Source = newBitmap;

                        // Display the image info
                        string selectedText = ContentList.SelectedItem.ToString();
                        int periodIndex = selectedText.IndexOf('.');
                        string textAfterPeriod = periodIndex >= 0 ? selectedText.Substring(periodIndex + 1) : string.Empty;
                        displayedImageInfo.Text = $"{newBitmap.PixelWidth}x{newBitmap.PixelHeight} {textAfterPeriod}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while loading the image: " + ex.Message + " This may occur when attempting to view or export an already modified image.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("The image file was not found. You may be inputting too quickly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                CompareResolutions();
            }
        }

        private void ClearWorkingDirectory()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(workingDirectory);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (Exception)
                {
                    // Skip this file
                }
            }
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                try
                {
                    directory.Delete(true);
                }
                catch (Exception)
                {
                    // Skip this directory
                }
            }
        }

        private void ChangeDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            SelectLitbusDirectory();
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG Files|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage newBitmap = new BitmapImage();
                    newBitmap.BeginInit();
                    newBitmap.CacheOption = BitmapCacheOption.OnLoad;
                    newBitmap.UriSource = new Uri(openFileDialog.FileName);
                    newBitmap.EndInit();
                    userSelectedImage.Source = newBitmap;

                    // Store file path of user-selected image
                    userSelectedImagePath = openFileDialog.FileName;

                    // Display the image info
                    userSelectedImageInfo.Text = $"{newBitmap.PixelWidth}x{newBitmap.PixelHeight} PNG";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading the image: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            CompareResolutions();
        }
        private void CompareResolutions()
        {
            if (DisplayedImage.Source != null && UserSelectedImage.Source != null)
            {
                BitmapSource displayedBitmap = DisplayedImage.Source as BitmapSource;
                BitmapSource userSelectedBitmap = UserSelectedImage.Source as BitmapSource;
                if (displayedBitmap != null && userSelectedBitmap != null)
                {
                    if (displayedBitmap.PixelWidth == userSelectedBitmap.PixelWidth &&
                        displayedBitmap.PixelHeight == userSelectedBitmap.PixelHeight)
                    {
                        ResolutionWarningTextBlock.Text = "Resolutions match!";
                    }
                    else
                    {
                        ResolutionWarningTextBlock.Text = "Warning: Resolutions do not match!";
                    }
                }
            }
        }

        private void ExportImage()
        {
            if (DisplayedImage.Source != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PNG Files|*.png";
                if (saveFileDialog.ShowDialog() == true)
                {
                    BitmapSource bitmapSource = DisplayedImage.Source as BitmapSource;
                    if (bitmapSource != null)
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                        using (Stream stream = File.Create(saveFileDialog.FileName))
                        {
                            encoder.Save(stream);
                        }
                    }
                }
            }
        }

        private void ExportImageButton_Click(object sender, RoutedEventArgs e)
        {
            ExportImage();
        }

        private async void PushChanges()
        {
            if (DisplayedImage.Source == null)
            {
                MessageBox.Show("No texture is currently loaded. Select one from the list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (UserSelectedImage.Source == null)
            {
                MessageBox.Show("No custom image is currently loaded. Click Open Image to load one.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (DisplayedImage.Source != null && UserSelectedImage.Source != null)
            {
                BitmapSource displayedBitmap = DisplayedImage.Source as BitmapSource;
                BitmapSource userSelectedBitmap = UserSelectedImage.Source as BitmapSource;
                if (displayedBitmap != null && userSelectedBitmap != null)
                {
                    if (displayedBitmap.PixelWidth != userSelectedBitmap.PixelWidth ||
                        displayedBitmap.PixelHeight != userSelectedBitmap.PixelHeight)
                    {
                        MessageBox.Show("Warning: Resolutions do not match!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            string backupDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\backup";
            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            string pakFileName = System.IO.Path.GetFileNameWithoutExtension(FileList.SelectedItem.ToString()) + ".PAK";
            string pakFile = litbusDirectory + "\\files\\" + pakFileName;
            string backupFile = backupDirectory + "\\" + pakFileName;
            if (!File.Exists(backupFile))
            {
                MessageBoxResult result = MessageBox.Show($"A backup of {pakFileName} must be created. Create one now?", "Backup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    File.Copy(pakFile, backupFile);
                }
                else
                {
                    return;
                }    
            }

            string outputFolder = workingDirectory;
            string name = ContentList.SelectedItem.ToString().Split('.')[0];
            string inputFile = workingDirectory + "\\" + ContentList.SelectedItem.ToString();

            Process process = Process.Start(new ProcessStartInfo
            {
                FileName = "dependencies\\LuckSystem.exe",
                Arguments = $"image import -i \"{userSelectedImagePath}\" -s \"{inputFile}\" -o \"{outputFolder}\\{name}_output\"",
                CreateNoWindow = true,
                UseShellExecute = false
            });
            await Task.Run(() => process.WaitForExit());

            // Perform byte modification on output file
            string outputFilePath = $"{outputFolder}\\{name}_output";
            if (File.Exists(outputFilePath))
            {
                byte[] inputFileBytes = File.ReadAllBytes(inputFile);
                byte[] outputFileBytes = File.ReadAllBytes(outputFilePath);

                // Get file type from inputFile
                string fileType = System.IO.Path.GetExtension(inputFile).ToUpperInvariant();
                switch (fileType)
                {
                    case ".CZ0":
                        // Copy bytes from inputFile to outputFile
                        Array.Copy(inputFileBytes, 0x1C, outputFileBytes, 0x1C, 7);
                        if (PixelOffsetFixCheckBox.IsChecked == true)
                        {
                            // Get value from bytes at 0x1C and 0x1D
                            int value = BitConverter.ToInt16(outputFileBytes, 0x1C);
                            // Subtract 2 from value
                            value -= 2;
                            // Write new value to bytes at 0x1C and 0x1D
                            Array.Copy(BitConverter.GetBytes((short)value), 0, outputFileBytes, 0x1C, 2);
                        }
                        break;
                    case ".CZ3":
                        // Reduce value at address 0x24 by one
                        if (outputFileBytes[0x24] == 0)
                        {
                            MessageBox.Show("The image contains a \"00 Byte Error\" and cannot be used.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        outputFileBytes[0x24]--;
                        break;
                    default:
                        // Do nothing for other file types
                        break;
                }

                // Write modified bytes to output file
                File.WriteAllBytes(outputFilePath, outputFileBytes);
            }

            process = Process.Start(new ProcessStartInfo
            {
                FileName = "dependencies\\LuckSystem.exe",
                Arguments = $"pak replace -o \"{litbusDirectory}\\files\\{System.IO.Path.GetFileNameWithoutExtension(pakFileName)}_output.PAK\" -s \"{pakFile}\" -i \"{outputFolder}\\{name}_output\" -n \"{name}\"",
                CreateNoWindow = true,
                UseShellExecute = false
            });
            await Task.Run(() => process.WaitForExit());

            // Delete original pak file and rename output pak file
            string outputPakFile = $"{litbusDirectory}\\files\\{System.IO.Path.GetFileNameWithoutExtension(pakFileName)}_output.PAK";
            if (File.Exists(outputPakFile))
            {
                File.Delete(pakFile);
                File.Move(outputPakFile, pakFile);
            }

            // Show message to user
            MessageBox.Show("The file has been imported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PushChangesButton_Click(object sender, RoutedEventArgs e)
        {
            PushChanges();
        }

        private async void RevertTexture()
        {
            /*
            if (DisplayedImage.Source == null)
            {
                MessageBox.Show("No texture is currently loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            */
            MessageBoxResult result = MessageBox.Show("Are you sure you want to undo any modifications to the currently selected texture?", "Revert Texture", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                string backupDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\backup";
                string pakFileName = System.IO.Path.GetFileNameWithoutExtension(FileList.SelectedItem.ToString()) + ".PAK";
                string backupFile = backupDirectory + "\\" + pakFileName;
                if (!File.Exists(backupFile))
                {
                    MessageBox.Show("No backup exists for the currently selected .PAK file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string outputFolder = workingDirectory;
                string name = ContentList.SelectedItem.ToString().Split('.')[0];
                string outputFilePath = $"{outputFolder}\\{name}_backup";

                Process process = Process.Start(new ProcessStartInfo
                {
                    FileName = "dependencies\\LuckSystem.exe",
                    Arguments = $"pak extract -i \"{backupFile}\" -o \"{outputFilePath}\" -n \"{name}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                await Task.Run(() => process.WaitForExit());

                string pakFile = litbusDirectory + "\\files\\" + pakFileName;
                process = Process.Start(new ProcessStartInfo
                {
                    FileName = "dependencies\\LuckSystem.exe",
                    Arguments = $"pak replace -s \"{pakFile}\" -o \"{litbusDirectory}\\files\\{System.IO.Path.GetFileNameWithoutExtension(pakFileName)}_output.PAK\" -i \"{outputFilePath}\" -n \"{name}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                await Task.Run(() => process.WaitForExit());

                // Delete original pak file and rename output pak file
                string outputPakFile = $"{litbusDirectory}\\files\\{System.IO.Path.GetFileNameWithoutExtension(pakFileName)}_output.PAK";
                if (File.Exists(outputPakFile))
                {
                    File.Delete(pakFile);
                    File.Move(outputPakFile, pakFile);
                }
                MessageBox.Show("The texture has been reverted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RevertTextureButton_Click(object sender, RoutedEventArgs e)
        {
            RevertTexture();
        }

        private void RevertAll()
        {
            /*
            if (DisplayedImage.Source == null)
            {
                MessageBox.Show("No texture is currently loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            */
            MessageBoxResult result = MessageBox.Show("Are you sure you want to revert all changes to the currently selected .PAK file?", "Revert All", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                string backupDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\backup";
                string pakFileName = System.IO.Path.GetFileNameWithoutExtension(FileList.SelectedItem.ToString()) + ".PAK";
                string backupFile = backupDirectory + "\\" + pakFileName;
                if (!File.Exists(backupFile))
                {
                    MessageBox.Show("No backup exists for the currently selected .PAK file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string pakFile = litbusDirectory + "\\files\\" + pakFileName;
                File.Copy(backupFile, pakFile, true);
            }
            MessageBox.Show("All changes have been reverted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RevertAllButton_Click(object sender, RoutedEventArgs e)
        {
            RevertAll();
        }
    }
}