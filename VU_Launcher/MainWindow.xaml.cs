using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using VU_Launcher.Properties;
using System.Net;

namespace VU_Launcher
{
    /// <summary>
    ///     Double click on system tray icon will open the app again
    /// </summary>
    public class ShowAppCommand : ICommand
    {
        public void Execute(object parameter)
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            Application.Current.MainWindow.ShowInTaskbar = true;
            Application.Current.MainWindow.Focus();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _vuPath = Settings.Default.vuPath;
        private string _vuDir = Settings.Default.vuDir;
        private string _vuFreqency;

        public MainWindow()
        {
            // Getting the installation path of VU
            if (Settings.Default.vuPath == "")
                _vuPath = GetInstallPath();

            // Shutdown if we cannot get the installation path
            if (_vuPath == null)
                Application.Current.Shutdown();

            _vuDir = _vuPath;
            _vuPath = _vuDir + "vu.exe";

            InitializeComponent();

            // Enable/disable buttons based on whether VU is installed or not
            if (File.Exists(_vuPath))
                btnInstallVU.IsEnabled = false;
            else
            {
                btnLaunch.IsEnabled = false;
                qckLaunch30Hz.IsEnabled = false;
                qckLaunch60Hz.IsEnabled = false;
                qckLaunch120Hz.IsEnabled = false;
            }                

            // Loading user preferences
            checkBox_autoClose.IsChecked = Settings.Default.appAutoClose;
            minToTray.IsChecked = Settings.Default.appMinToTray;
            switch (Settings.Default.vuFrequency)
            {
                case 0:
                    radioButton_30hz.IsChecked = true;
                    break;
                case 1:
                    radioButton_60hz.IsChecked = true;
                    break;
                case 2:
                    radioButton_120hz.IsChecked = true;
                    break;
            }
            checkBox_noBorder.IsChecked = Settings.Default.vuNoBorder;
            checkBox_server.IsChecked = Settings.Default.vuServer;
        }

        private static string GetInstallPath()
        {
            try
            {
                // 64bit or 32bit OS BF3 installation path
                var bf3Path =
                    (string)
                        Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\EA Games\Battlefield 3",
                            "Install Dir", null) ??
                    (string)
                        Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\EA Games\Battlefield 3", "Install Dir", null);

                // In case of missing registry key, show error as BF3 is possibly not installed
                if (bf3Path == null)
                    throw new Exception("Could not retrieve the installation directory!\n" +
                                        "Please verify Battlefield 3 is installed correctly.");

                string vuPath;

                // Detecting new/old VU installation path
                if (File.Exists(bf3Path + "vu.exe"))
                    vuPath = bf3Path;
                else
                {
                    vuPath = 
                        (string) 
                            Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\VeniceUnleashed_is1",
                                "InstallLocation", null);
                    if (vuPath == null)
                    {
                        // Let user select the VU installation path
                        OpenFileDialog selectVUInstallPath = new OpenFileDialog();

                        selectVUInstallPath.InitialDirectory = "C:\\";
                        selectVUInstallPath.Filter = "VU Executable|vu.exe";
                        selectVUInstallPath.FileName = "Please select the folder where you've installed Venice Unleashed. Press \"Cancel\" if you want to use the Battlefield 3 folder.";

                        if (selectVUInstallPath.ShowDialog() == true)
                            vuPath = selectVUInstallPath.FileName.TrimEnd(new char[] { 'v', 'u', '.', 'e', 'x', 'e' });
                        else
                            vuPath = bf3Path; // Always default to BF3 install folder
                    }
                }

                // Save VU installation path
                Settings.Default.vuPath = vuPath;
                return vuPath;
            }

            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "VU Launcher - Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return null;
        }

        private void LaunchVenice(string freq, string vuPath)
        {
            // Single player levels names
            var spLevel = new[]
            {
                "SP_New_York",      // 1. Semper Fidelis
                "SP_Earthquake",    // 2. Operation Swordbreaker
                "SP_Earthquake2",   // 3. Uprising
                "SP_Jet",           // 4. Going Hunting
                "SP_Bank",          // 5. Operation Guillotine
                "SP_Paris",         // 6. Comrades
                "SP_Tank",          // 7. Thunder Run
                "SP_Tank_b",        // 8. Fear No Evil
                "SP_Sniper",        // 9. Night Shift
                "SP_Valley",        // 10. Rock and a Hard Place
                "SP_Villa",         // 11. Kaffarov
                "SP_Finale"         // 12. The Great Destroyer
            };

            switch (checkBox_server.IsChecked)
            {
                // Launch VU Client
                case false:
                    // Launch VU on the selected campaign level
                    if (comboBox_spLevel.SelectedItem != null)
                    {
                        // If option checked, lauch without border
                        if (checkBox_noBorder.IsChecked == true)
                            Process.Start(vuPath,
                                "-noBorder -high" + freq + " -level " + spLevel[comboBox_spLevel.SelectedIndex]);
                        // Otherwise, launch VU normally
                        else
                            Process.Start(vuPath, "-high" + freq + " -level " + spLevel[comboBox_spLevel.SelectedIndex]);
                    }
                    // If no level is selected, launch VU normally
                    else if (comboBox_spLevel.SelectedItem == null)
                    {
                        // If option checked, lauch without border
                        if (checkBox_noBorder.IsChecked == true)
                            Process.Start(vuPath, "-noBorder -high" + freq);
                        // Otherwise, launch VU normally
                        else
                            Process.Start(vuPath, "-high" + freq);
                    }
                    break;

                // Launch VU Server
                case true:
                    Process.Start(vuPath, "-server -dedicated -high" + freq);
                    break;
            }

            // Auto close the app if desired
            if (checkBox_autoClose.IsChecked == true)
                Application.Current.Shutdown();
        }

        private void btnLaunch_Click(object sender, RoutedEventArgs e)
        {
            // Check if "vu.exe" exists, if not show error message and disable launch button
            if (!File.Exists(_vuPath))
            {
                MessageBox.Show("Could not retrieve the installation directory!\n" +
                                        "Please verify Venice Unleashed is installed correctly.", "VU Launcher - Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                btnLaunch.IsEnabled = false;
                btnInstallVU.IsEnabled = true;
            }
            else LaunchVenice(_vuFreqency, _vuPath);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            // Minimize to system tray
            switch (WindowState)
            {
                case WindowState.Minimized:
                    if (minToTray.IsChecked == true)
                        ShowInTaskbar = false;

                    // One time notification when minimizing to system tray
                    if (Settings.Default.appTrayNotify == true)
                        sysTrayIcon.ShowBalloonTip("VU Launcher",
                            "Venice Unleashed Launcher is now minimized to the system tray. From here you can quickly launch VU.",
                            BalloonIcon.Info);
                    Settings.Default.appTrayNotify = false;
                    break;
            }
        }

        private void launchVU30Hz_Click(object sender, RoutedEventArgs e)
        {
            // Quick launch VU at 30Hz
            Process.Start(_vuPath, "-high30");
        }

        private void launchVU60Hz_Click(object sender, RoutedEventArgs e)
        {
            // Quick launch VU at 60Hz
            Process.Start(_vuPath, "-high60");
        }

        private void launchVU120Hz_Click(object sender, RoutedEventArgs e)
        {
            // Quick launch VU at 120Hz
            Process.Start(_vuPath, "-high120");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Saving user preferences
            // Application automically closes
            switch (checkBox_autoClose.IsChecked)
            {
                case false:
                    Settings.Default.appAutoClose = false;
                    break;

                case true:
                    Settings.Default.appAutoClose = true;
                    break;
            }

            // Application minimize to tray
            switch (minToTray.IsChecked)
            {
                case false:
                    Settings.Default.appMinToTray = false;
                    break;

                case true:
                    Settings.Default.appMinToTray = true;
                    break;
            }

            // VU launch at desired frequency
            if (radioButton_30hz.IsChecked == true)
                Settings.Default.vuFrequency = 0;
            else if (radioButton_60hz.IsChecked == true)
                Settings.Default.vuFrequency = 1;
            else if (radioButton_120hz.IsChecked == true)
                Settings.Default.vuFrequency = 2;

            // VU launch with no borders
            switch (checkBox_noBorder.IsChecked)
            {
                case false:
                    Settings.Default.vuNoBorder = false;
                    break;

                case true:
                    Settings.Default.vuNoBorder = true;
                    break;
            }

            // VU launch as a server
            switch (checkBox_server.IsChecked)
            {
                case false:
                    Settings.Default.vuServer = false;
                    break;

                case true:
                    Settings.Default.vuServer = true;
                    break;
            }

            Settings.Default.Save();
        }

        private void openVULauncher_Click(object sender, RoutedEventArgs e)
        {
            // Open VU Launcher from system tray icon
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
            Focus();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            // Close VU Launcher from system tray icon
            Application.Current.Shutdown();
        }

        // Set _vuFreqency on Checked event to simplify getting selected frequency
        private void radioButton_30hz_Checked(object sender, RoutedEventArgs e)
        {
            _vuFreqency = "30";
        }

        private void radioButton_60hz_Checked(object sender, RoutedEventArgs e)
        {
            _vuFreqency = "60";
        }

        private void radioButton_120hz_Checked(object sender, RoutedEventArgs e)
        {
            _vuFreqency = "120";
        }

        // Manually install/update latest VU version
        void update_VU(object sender, RoutedEventArgs e)
        {
            // Disable the GUI Buttons
            btnLaunch.IsEnabled = false;
            btnInstallVU.IsEnabled = false;
            
            string name_downloadedFile = "VU_latest.zip";

            WebClient client = new WebClient();

            try
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri("http://veniceunleashed.net/VeniceUnleashed.zip"), _vuDir + name_downloadedFile);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "VU Launcher - Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Sync progress bar with download percentage
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBarDownload.Maximum = (int)e.TotalBytesToReceive / 100;
            progressBarDownload.Value = (int)e.BytesReceived / 100;
            progressBarDownload.ToolTip = (int)e.BytesReceived / 1000000 + "MB / " + (int)e.TotalBytesToReceive / 1000000 + "MB";
        }

        // Extract downloaded VU zip file
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string nameDownloadedFile = "VU_latest.zip";
            try {
                System.IO.Compression.ZipFile.ExtractToDirectory((_vuDir + nameDownloadedFile), _vuDir);
                File.Delete(_vuDir + nameDownloadedFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "VU Launcher - Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Venice Unleashed has been successfully installed!", "VU Launcher - Info!", MessageBoxButton.OK, MessageBoxImage.Information);
            btnLaunch.IsEnabled = true;
            qckLaunch30Hz.IsEnabled = true;
            qckLaunch60Hz.IsEnabled = true;
            qckLaunch120Hz.IsEnabled = true;
            progressBarDownload.Value = 0;
        }

        private void btnInstallVU_Click(object sender, RoutedEventArgs e)
        {
            update_VU(sender, e);
        }
    }
}