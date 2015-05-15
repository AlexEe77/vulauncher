using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace VU_Launcher
{
    /// <summary>
    /// Double click on system tray icon will open the app again
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _vuFreqency;
        private readonly string _vuPath;

        public MainWindow()
        {
            if (!File.Exists("Hardcodet.Wpf.TaskbarNotification.dll"))
                MessageBox.Show("\"Hardcodet.Wpf.TaskbarNotification.dll\" is missing!\n" +
                                "Be sure it's in the same folder of VU Launcher.", "VU Launcher - Missing .dll!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);


            // Getting the installation path of VU
            _vuPath = GetInstallPath();

            // Shutdown if we cannot get the installation path
            if (_vuPath == null)
                Application.Current.Shutdown();

            InitializeComponent();

            // Loading user preferences
            checkBox_autoClose.IsChecked = Properties.Settings.Default.appAutoClose;
            minToTray.IsChecked = Properties.Settings.Default.appMinToTray;
            switch (Properties.Settings.Default.vuFrequency)
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
            checkBox_noBorder.IsChecked = Properties.Settings.Default.vuNoBorder;
            checkBox_server.IsChecked = Properties.Settings.Default.vuServer;
        }

        private static string GetInstallPath()
        {
            try
            {
                // 64bit or 32bit OS BF3 installation path
                var regPath =
                    (string)
                        Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\EA Games\Battlefield 3", "Install Dir", null) ?? (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\EA Games\Battlefield 3", "Install Dir", null);

                // In case of missing registry key, show error as BF3 is possibly not installed
                if (regPath == null)
                    throw new Exception("Could not retrieve the installation directory!\n" +
                                        "Please verify Battlefield 3 is installed correctly.");

                // VU installation path
                var vuPath = (regPath + "\\vu.exe");

                // Check if VU is properly installed, if not show error
                if (!File.Exists(vuPath))
                    throw new Exception("Cound not find \"vu.exe\" in the Battlefield 3 directory!\n" +
                                        "Please verify Venice Unleashed is installed correctly.");

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
            var spLevel = new []{"SP_New_York", "SP_Earthquake", "SP_Earthquake2", "SP_Jet", "SP_Bank", "SP_Paris", "SP_Tank", "SP_Tank_b", "SP_Sniper", "SP_Valley", "SP_Villa", "SP_Finale"};

            switch (checkBox_server.IsChecked)
            {
                // Launch VU Client
                case false:
                    // Launch VU on the selected campaign level
                    if (comboBox_spLevel.SelectedItem != null)
                    {
                        System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq + " -level " + spLevel[comboBox_spLevel.SelectedIndex]);
                    }
                    // If no level is selected, launch VU normally
                    else if (comboBox_spLevel.SelectedItem == null)
                    {
                        // If option checked, lauch without border
                        if (checkBox_noBorder.IsChecked == true)
                            System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq);
                        // Otherwise, launch VU normally
                        else
                        {
                            System.Diagnostics.Process.Start(vuPath, "-high" + freq);  
                        }
                    }
                    break;

                // Launch VU Server
                case true:
                    System.Diagnostics.Process.Start(vuPath, "-server -dedicated -high" + freq);
                    break;
            }

            // Auto close the app if desired
            if (checkBox_autoClose.IsChecked == true)
                Application.Current.Shutdown();
        }

        private void btnLaunch_Click(object sender, RoutedEventArgs e)
        {
            LaunchVenice(_vuFreqency, _vuPath);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            // Minimize to system tray
            switch (WindowState)
            {
                case WindowState.Minimized:
                    if (minToTray.IsChecked == true)
                        ShowInTaskbar = false;
                        if (Properties.Settings.Default.appTrayNotify == true)
                            sysTrayIcon.ShowBalloonTip("VU Launcher", "Venice Unleashed Launcher is now minimized to the system tray. From here you can quickly launch VU.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
                        Properties.Settings.Default.appTrayNotify = false;
                    break;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Saving user preferences
            // Application automically closes
            switch (checkBox_autoClose.IsChecked)
            {
                case false:
                    Properties.Settings.Default.appAutoClose = false;
                    break;

                case true:
                    Properties.Settings.Default.appAutoClose = true;
                    break;
            }

            // Application minimize to tray
            switch (minToTray.IsChecked)
            {
                case false:
                    Properties.Settings.Default.appMinToTray = false;
                    break;

                case true:
                    Properties.Settings.Default.appMinToTray = true;
                    break;
            }

            // VU launch at desired frequency
            if (radioButton_30hz.IsChecked == true)
                Properties.Settings.Default.vuFrequency = 0;
            else if (radioButton_60hz.IsChecked == true)
                Properties.Settings.Default.vuFrequency = 1;
            else if (radioButton_120hz.IsChecked == true)
                Properties.Settings.Default.vuFrequency = 2;

            // VU launch with no borders
            switch (checkBox_noBorder.IsChecked)
            {
                case false:
                    Properties.Settings.Default.vuNoBorder = false;
                    break;

                case true:
                    Properties.Settings.Default.vuNoBorder = true;
                    break;
            }

            // VU launch as a server
            switch (checkBox_server.IsChecked)
            {
                case false:
                    Properties.Settings.Default.vuServer = false;
                    break;

                case true:
                    Properties.Settings.Default.vuServer = true;
                    break;
            }

            Properties.Settings.Default.Save();
        }

        private void openVULauncher_Click(object sender, RoutedEventArgs e)
        {
            // Open VU Launcher from system tray icon
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
            Focus();
        }

        private void launchVU30Hz_Click(object sender, RoutedEventArgs e)
        {
            // Quick launch VU at 30Hz
            System.Diagnostics.Process.Start(_vuPath, "-high30");
        }

        private void launchVU60Hz_Click(object sender, RoutedEventArgs e)
        {
            // Quick launch VU at 60Hz
            System.Diagnostics.Process.Start(_vuPath, "-high60");
        }

        private void launchVU120Hz_Click(object sender, RoutedEventArgs e)
        {
            // Quick launch VU at 120Hz
            System.Diagnostics.Process.Start(_vuPath, "-high120");
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
    }
}
