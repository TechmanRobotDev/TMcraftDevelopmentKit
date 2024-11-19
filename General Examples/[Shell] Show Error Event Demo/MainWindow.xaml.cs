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
using System.Windows.Threading;
using Newtonsoft.Json;
using TMcraft;

namespace ShowErrorEvent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TMcraftShellAPI ShellUI;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ShellUI == null)
            {
                ShellUI = new TMcraftShellAPI();
                ShellUI.InitialTMcraftShell();
            }

            if (ShellUI == null || ShellUI.RobotStatusProvider == null)
            {
                MessageBox.Show("Connection failed");
            }
            else
            {
                ShellUI.RobotStatusProvider.ErrorEvent += RobotStatusProvider_ErrorEvent;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RobotStatusProvider_ErrorEvent(object data)
        {
            try
            {
                if (data == null)
                {
                    MessageBox.Show("Null Error Status data...");
                    return;
                }

                ErrorStatus temp = JsonConvert.DeserializeObject<ErrorStatus>((string)data);
                string strErr = "[" + temp.Last_Error_Time + "] " + temp.Last_Error_Code.ToString();
                strErr += Environment.NewLine;

                string str = string.Empty;
                ShellUI.GetErrMsg(temp.Last_Error_Code, out str);

                strErr += str;

                Dispatcher.BeginInvoke(
                                   DispatcherPriority.Background,
                                   new Action(delegate ()
                                   {
                                       TextBox_Content.Clear();
                                       TextBox_Content.Text = strErr;
                                       Elp_errorStatus.Fill = Brushes.OrangeRed;
                                   }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Btn_Back_Click(object sender, RoutedEventArgs e)
        {
            if (ShellUI == null || ShellUI.SystemProvider == null)
            {
                MessageBox.Show("no Connection");
                Close();
                return;
            }

            ShellUI.SystemProvider.ShowTMflow();
        }

        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            Elp_errorStatus.Fill = Brushes.GreenYellow;
            TextBox_Content.Clear();
        }
    }
}
