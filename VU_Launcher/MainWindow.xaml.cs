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
        private readonly string _vuPath;
        private string _vuFreqency;

        public MainWindow()
        {
            // Getting the installation path of VU
            _vuPath = GetInstallPath();

            // Shutdown if we cannot get the installation path
            if (_vuPath == null)
                Application.Current.Shutdown();

            _vuPath += "\\vu.exe";

            InitializeComponent();

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
                var regPath =
                    (string)
                        Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\EA Games\Battlefield 3",
                            "Install Dir", null) ??
                    (string)
                        Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\EA Games\Battlefield 3", "Install Dir", null);

                // In case of missing registry key, show error as BF3 is possibly not installed
                if (regPath == null)
                    throw new Exception("Could not retrieve the installation directory!\n" +
                                        "Please verify Battlefield 3 is installed correctly.");
<<<<<<< HEAD
                return regPath;
=======

                // VU installation path
                var vuPath =
                    // New VU path since December 2015
                    (string)
                        Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\VeniceUnleashed_is1",
                            "InstallLocation", null) + "vu.exe" ??
                    
                    // Old path, just in case
                    (regPath + "\\vu.exe");

                // Check if VU is properly installed, if not show error
                if (!File.Exists(vuPath))
                    throw new Exception("Could not retrieve the installation directory!\n" +
                                        "Please verify Venice Unleashed is installed correctly.");

                return vuPath;
>>>>>>> refs/remotes/Dendari92/master
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "VU Launcher - Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        private void LaunchVenice(string freq, string vuPath)
        {
            var spLevel = new[]
            {
                "SP_New_York", "SP_Earthquake", "SP_Earthquake2", "SP_Jet", "SP_Bank", "SP_Paris", "SP_Tank",
                "SP_Tank_b",
                "SP_Sniper", "SP_Valley", "SP_Villa", "SP_Finale"
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
            // VU installation path
            string vuFilePath = GetInstallPath() + "\\vu.exe";

            if (!File.Exists(vuFilePath))
            {
                MessageBox.Show("Install VU first!");
                btnLaunch.IsEnabled = false;
                btn_installVU.IsEnabled = true;
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
        private void Download_VU(object sender, EventArgs e)
        {

        }

        void update_VU(object sender, RoutedEventArgs e)
        {
            btnLaunch.IsEnabled = false;
            btn_installVU.IsEnabled = false;
            WebClient client = new WebClient();
            string vuPath = GetInstallPath();
            string name_downloadedFile = "VU_latest.zip";
            try
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri("http://veniceunleashed.net/VeniceUnleashed.zip"), vuPath + name_downloadedFile);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "VU Launcher - Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressbar_download.Maximum = (int)e.TotalBytesToReceive / 100;
            progressbar_download.Value = (int)e.BytesReceived / 100;
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string vuPath = GetInstallPath();
            string name_downloadedFile = "VU_latest.zip";
            try {
                System.IO.Compression.ZipFile.ExtractToDirectory((vuPath + name_downloadedFile), vuPath);
                File.Delete(vuPath + name_downloadedFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "VU Launcher - Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Update completed and successfully installed!");
            btnLaunch.IsEnabled = true;
            progressbar_download.Value = 0;
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            update_VU(sender, e);
        }
    }
}