using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using IO = System.IO;


namespace WpfUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly StartClock _startClock = new StartClock();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select a Folder",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection",
                Filter = "Folders|\n",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                RestoreDirectory = true,
                ValidateNames = false
            };

            var response = openFileDialog.ShowDialog();

            if (response == true)
            {
                string folderPath = IO.Path.GetDirectoryName(openFileDialog.FileName);
                MessageBox.Show($"Selected Folder: {folderPath}");
                FolderPath.Text = folderPath;
            }
        }

        private async void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClockStart.Text = "CLOCK STARTED";
            var folderPath = FolderPath.Text;

            // Run GeneratePNG on a separate thread
            await Task.Run(() => _startClock.GeneratePng(folderPath));
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            _startClock.StopGeneratePng();
            ClockStart.Text = "CLOCK STOPPED";
        }
    }
}
