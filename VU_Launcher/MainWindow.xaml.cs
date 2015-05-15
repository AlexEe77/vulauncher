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
        public MainWindow()
        {
            if (!File.Exists("Hardcodet.Wpf.TaskbarNotification.dll"))
                MessageBox.Show("\"Hardcodet.Wpf.TaskbarNotification.dll\" is missing!\n" +
                                "Be sure it's in the same folder of VU Launcher.", "VU Launcher - Missing .dll!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);

            InitializeComponent();

            // Loading user preferences
            checkBox_autoClose.IsChecked = Properties.Settings.Default.appAutoClose;
            minToTray.IsChecked = Properties.Settings.Default.appMinToTray;
            if (Properties.Settings.Default.vuFrequency == 0)
                radioButton_30hz.IsChecked = true;
            else if (Properties.Settings.Default.vuFrequency == 1)
                radioButton_60hz.IsChecked = true;
            else if (Properties.Settings.Default.vuFrequency == 2)
                radioButton_120hz.IsChecked = true;
            checkBox_noBorder.IsChecked = Properties.Settings.Default.vuNoBorder;
            checkBox_server.IsChecked = Properties.Settings.Default.vuServer;
        }

        private string getInstallPath(string vuPath)
        {
            try
            {
                // 64bit or 32bit OS BF3 installation path
                var regPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\EA Games\Battlefield 3", "Install Dir", null) ?? (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\EA Games\Battlefield 3", "Install Dir", null);

                // In case of missing registry key, show error as BF3 is possibly not installed
                if (regPath == null)
                    throw new Exception("Could not retrieve the installation directory!\n" +
                                        "Please verify Battlefield 3 is installed correctly.");

                // VU installation path
                vuPath = (regPath + "\\vu.exe");

                // Check if VU is properly installed, if not show error
                if (!File.Exists(vuPath))
                    throw new Exception("Cound not find \"vu.exe\" in the Battlefield 3 directory!\n" +
                                        "Please verify Venice Unleashed is installed correctly.");

                return vuPath;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "VU Launcher - Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void launchVenice(string freq, string vuPath)
        {
            string[] spLevel = new string[]{"SP_New_York", "SP_Earthquake", "SP_Earthquake2", "SP_Jet", "SP_Bank", "SP_Paris", "SP_Tank", "SP_Tank_b", "SP_Sniper", "SP_Valley", "SP_Villa", "SP_Finale"};

            switch (checkBox_server.IsChecked)
            {
                // Launch VU Client
                case false:
                    // Launch VU on the selected campaign level
                    if (comboBox_spLevel.SelectedItem != null)
                    {
                        switch (comboBox_spLevel.SelectedIndex)
                        {
                            // Semper Fidelis
                            case 0:
                                System.Diagnostics.Process.Start(vuPath, "-high" + freq + " -level " + spLevel[0]);
                                if (checkBox_noBorder.IsChecked == true)
                                    System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq + " -level " + spLevel[0]);
                                break;
                            // Operation Swordbreaker
                            case 1:
                                System.Diagnostics.Process.Start(vuPath, "-high" + freq + " -level " + spLevel[1]);
                                if (checkBox_noBorder.IsChecked == true)
                                    System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq + " -level " + spLevel[1]);
                                break;
                            // Uprising
                            case 2:
                                System.Diagnostics.Process.Start(vuPath, "-high" + freq + " -level " + spLevel[2]);
                                if (checkBox_noBorder.IsChecked == true)
                                    System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq + " -level " + spLevel[2]);
                                break;
                            // Going Hunting
                            case 3:
                                System.Diagnostics.Process.Start(vuPath, "-high" + freq + " -level " + spLevel[3]);
                                if (checkBox_noBorder.IsChecked == true)
                                    System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq + " -level " + spLevel[3]);
                                break;
                            // Operation Guillotine
                            case 4:
                                System.Diagnostics.Process.Start(vuPath, "-high" + freq + " -level " + spLevel[4]);
                                if (checkBox_noBorder.IsChecked == true)
                                    System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq + " -level " + spLevel[4]);
                                break;
                            // Comrades
                            case 5:
                                System.Diagnostics.Process.Start(vuPath, "-high" + freq + " -level " + spLevel[5]);
                                if (checkBox_noBorder.IsChecked == true)
                                    System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq + " -level " + spLevel[5]);
                                break;
                            // Thunder Run
                            case 6:
                                System.Diagnostics.Process.Start(vuPath, "-high" + freq + " -level " + spLevel[6]);
                                if (checkBox_noBorder.IsChecked == true)
                                    System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq + " -level " + spLevel[6]);
                                break;
                            // Fear No Evil
                            case 7:
                                System.Diagnostics.Process.Start(vuPath, "-high" + freq + " -level " + spLevel[7]);
                                if (checkBox_noBorder.IsChecked == true)
                                    System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq + " -level " + spLevel[7]);
                                break;
                            // Night Shift
                            case 8:
                                System.Diagnostics.Process.Start(vuPath, "-high" + freq + " -level " + spLevel[8]);
                                if (checkBox_noBorder.IsChecked == true)
                                    System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq + " -level " + spLevel[8]);
                                break;
                            // Rock and a Hard Place
                            case 9:
                                System.Diagnostics.Process.Start(vuPath, "-high" + freq + " -level " + spLevel[9]);
                                if (checkBox_noBorder.IsChecked == true)
                                    System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq + " -level " + spLevel[9]);
                                break;
                            // Kaffarov
                            case 10:
                                System.Diagnostics.Process.Start(vuPath, "-high" + freq + " -level " + spLevel[10]);
                                if (checkBox_noBorder.IsChecked == true)
                                    System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq + " -level " + spLevel[10]);
                                break;
                            // The Great Destroyer
                            case 11:
                                System.Diagnostics.Process.Start(vuPath, "-high" + freq + " -level " + spLevel[11]);
                                if (checkBox_noBorder.IsChecked == true)
                                    System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq + " -level " + spLevel[11]);
                                break;
                        }
                    }
                    // If no level is selected, launch VU normally
                    else if (comboBox_spLevel.SelectedItem == null)
                    {
                        System.Diagnostics.Process.Start(vuPath, "-high" + freq);
                        if (checkBox_noBorder.IsChecked == true)
                            System.Diagnostics.Process.Start(vuPath, "-noBorder -high" + freq);
                    }
                // Auto close the app if desired
                if (checkBox_autoClose.IsChecked == true)
                    Application.Current.Shutdown();
                break;

                // Launch VU Server
                case true:
                    // Launch VU with no borders if desired
                    switch (checkBox_noBorder.IsChecked)
                    {
                        case false:
                            System.Diagnostics.Process.Start(vuPath, "-server -dedicated -high" + freq);
                            break;

                        case true:
                            System.Diagnostics.Process.Start(vuPath, "-noBorder -server -dedicated -high" + freq);
                            break;
                    }
                // Auto close the app if desired
                if (checkBox_autoClose.IsChecked == true)
                    Application.Current.Shutdown();
                break;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string vuPath = "";

            try
            {
                vuPath = getInstallPath(vuPath);
            }
            catch (Exception)
            {
                return;
            }

            // Launch VU with whatever option has been selected
            // In case of 30Hz
            if (radioButton_30hz.IsChecked == true)
                launchVenice("30", vuPath); // This will add -high30 parameter which is unnecessary, but the end result it's the same anyway 
            // In case of 60Hz
            else if (radioButton_60hz.IsChecked == true)
                launchVenice("60", vuPath);
            // In case of 120Hz
            else if (radioButton_120hz.IsChecked == true)
                launchVenice("120", vuPath);
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
            string vuPath = "";

            try
            {
                vuPath = getInstallPath(vuPath);
            }
            catch (Exception)
            {
                return;
            }
            // Quick launch VU at 30Hz
            System.Diagnostics.Process.Start(vuPath);
        }

        private void launchVU60Hz_Click(object sender, RoutedEventArgs e)
        {
            string vuPath = "";

            try
            {
                vuPath = getInstallPath(vuPath);
            }
            catch (Exception)
            {
                return;
            }
            // Quick launch VU at 60Hz
            System.Diagnostics.Process.Start(vuPath, "-high60");
        }

        private void launchVU120Hz_Click(object sender, RoutedEventArgs e)
        {
            string vuPath = "";

            try
            {
                vuPath = getInstallPath(vuPath);
            }
            catch (Exception)
            {
                return;
            }
            // Quick launch VU at 120Hz
            System.Diagnostics.Process.Start(vuPath, "-high120");
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            // Close VU Launcher from system tray icon
            Application.Current.Shutdown();
        }
    }
}
