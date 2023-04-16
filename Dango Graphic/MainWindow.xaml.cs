using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Linq;

namespace PNGResolution
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG Files (*.png)|*.png";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                DisplayImage(filename);
                DisplayResolution(filename);
            }
        }

        private void DisplayImage(string filename)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filename);
            bitmap.EndInit();

            ImageBox.Source = bitmap;
        }

        private void DisplayResolution(string filename)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                reader.ReadBytes(16);
                int width = BitConverter.ToInt32(reader.ReadBytes(4).Reverse().ToArray(), 0);
                int height = BitConverter.ToInt32(reader.ReadBytes(4).Reverse().ToArray(), 0);

                ResolutionLabel.Content = $"{width} x {height}";
            }
        }
    }
}