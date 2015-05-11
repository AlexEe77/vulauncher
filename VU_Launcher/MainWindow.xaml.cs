using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            // Getting VU/BF3 install path 
            String reg_path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\EA Games\Battlefield 3", "Install Dir", null);

            // In case VU/BF3 is installed on 32bit system
            if (reg_path == null)
            {
                reg_path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\EA Games\Battlefield 3", "Install Dir", null);
            }

            // VU/BF3 install path
            String vu_path = (reg_path + "\\vu.exe");

            // Which option has been selected, launch VU in 30Hz, 60Hz or 120Hz
            // In case of 30Hz...
            if (radioButton.IsChecked == true)
            {
                if (checkBox.IsChecked == false)
                {
                    System.Diagnostics.Process.Start(vu_path);
                }  
                // Launch VU Server at 30Hz
                else if (checkBox.IsChecked == true)
                {
                    System.Diagnostics.Process.Start(vu_path, "-server -dedicated");
                }
            }
            // In case of 60Hz...
            else if (radioButton1.IsChecked == true)
            {
                if (checkBox.IsChecked == false)
                {
                    System.Diagnostics.Process.Start(vu_path, "-high60");
                }
                // Launch VU Server at 60Hz
                else if (checkBox.IsChecked == true)
                {
                    System.Diagnostics.Process.Start(vu_path, "-server -dedicated -high60");
                }
            }
            // In case of 120Hz...
            else if (radioButton2.IsChecked == true)
            {
                if (checkBox.IsChecked == false)
                {
                    System.Diagnostics.Process.Start(vu_path, "-high120");
                }
                // Launch VU Server at 120Hz
                else if (checkBox.IsChecked == true)
                {
                    System.Diagnostics.Process.Start(vu_path, "-server -dedicated -high120");
                }
            }
        }
    }
}
