using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace VU_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Launch Venice with the selected freq.
        private void LaunchVenice(String frequency, String vuPath)
        {
            switch (checkBox.IsChecked)
            {
                case false:
                    System.Diagnostics.Process.Start(vuPath, "-high" + frequency);
                    break;
                case true:
                    System.Diagnostics.Process.Start(vuPath, "-server -dedicated -high" + frequency);
                    break;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            // Getting VU/BF3 install path
            string vuPath;

            // Error prevention
            try
            {
                var regPath = (string) Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\EA Games\Battlefield 3", "Install Dir", null) ?? 
                                 // In case VU/BF3 is installed on 32bit system
                                 (string) Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\EA Games\Battlefield 3", "Install Dir", null);

                // If the key was not found, so regPath equals null, trow exception
                if (regPath == null) throw new Exception("Could not get installation directory!\n"+
                                                         "Please verify Battlefield 3 is installed correctly.");
                // VU/BF3 install path
                vuPath = (regPath + "\\vu.exe");

                // Check if VU is even installed, if not, throw exception
                if (!File.Exists(vuPath)) throw new Exception("Could not find vu.exe!\n" + 
                                                              "Please verify Venice Unleashed is installed correctly.");
            }
            catch (Exception exception)
            {
                // Show our error message and return
                MessageBox.Show(exception.Message, "An Error Occurred!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Which option has been selected, launch VU in 30Hz, 60Hz or 120Hz
            // In case of 30Hz...
            if (radioButton30Hz.IsChecked == true)
            {
                LaunchVenice("30", vuPath);
            }
            // In case of 60Hz... 
            else if (radioButton60Hz.IsChecked == true)
            {
                LaunchVenice("60", vuPath);
            }
            // In case of 120Hz...
            else if (radioButton120Hz.IsChecked == true)
            {
                LaunchVenice("120", vuPath);
            }

            // close app if checked
            if (closeAfterLaunch.IsChecked == true)
                Application.Current.Shutdown();
        }
    }
}
