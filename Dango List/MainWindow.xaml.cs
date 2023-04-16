using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private string _selectedPakFile;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectPakFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "PAK files (*.pak)|*.pak" };
            if (openFileDialog.ShowDialog() == true)
            {
                _selectedPakFile = openFileDialog.FileName;
                ExtractButton.IsEnabled = true;
            }
        }

        private void ExtractButton_Click(object sender, RoutedEventArgs e)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dependencies\\LuckSystem.exe",
                Arguments = $"pak extract -i \"{_selectedPakFile}\" -o /dango -a dango",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            var process = Process.Start(processStartInfo);
            process?.WaitForExit();

            var dangoDirectory = new DirectoryInfo("dango");
            var outputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                Path.GetFileNameWithoutExtension(_selectedPakFile) + ".txt");
            using (var writer = new StreamWriter(outputFile))
            {
                var files = dangoDirectory.GetFiles()
                    .Select(file => new
                    {
                        File = file,
                        FileTypeBytes = File.ReadAllBytes(file.FullName).Take(3).ToArray()
                    })
                    .Where(x => new[] { "CZ0", "CZ1", "CZ2", "CZ3", "CZ4", "CZ5", "CZ6" }
                        .Contains(System.Text.Encoding.ASCII.GetString(x.FileTypeBytes)))
                    .OrderBy(x => x.File.Name, new NaturalStringComparer())
                    .ToList();

                foreach (var file in files)
                {
                    var fileType = System.Text.Encoding.ASCII.GetString(file.FileTypeBytes);
                    writer.WriteLine($"{file.File.Name}.{fileType}");
                    file.File.Delete();
                }
            }
        }

        public class NaturalStringComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x == null || y == null) return 0;

                var xFragments = Regex.Split(x, "([0-9]+)").Where(f => !string.IsNullOrEmpty(f)).ToArray();
                var yFragments = Regex.Split(y, "([0-9]+)").Where(f => !string.IsNullOrEmpty(f)).ToArray();

                for (int i = 0; i < xFragments.Length && i < yFragments.Length; i++)
                {
                    if (int.TryParse(xFragments[i], out var xNumber) && int.TryParse(yFragments[i], out var yNumber))
                    {
                        if (xNumber != yNumber)
                            return xNumber.CompareTo(yNumber);
                    }
                    else
                    {
                        var comparisonResult = string.Compare(xFragments[i], yFragments[i], StringComparison.CurrentCultureIgnoreCase);
                        if (comparisonResult != 0)
                            return comparisonResult;
                    }
                }

                return xFragments.Length.CompareTo(yFragments.Length);
            }
        }
    }
}