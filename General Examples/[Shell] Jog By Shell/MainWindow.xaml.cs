using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using TMcraft;

namespace JogbyShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TMcraftShellAPI ShellUI;
        bool JogStatus = false;
        Thread th_KeepJog;

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
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (ShellUI != null)
                {
                    ShellUI.CloseShellConnection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Btn_Back_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ShellUI == null || ShellUI.RobotStatusProvider == null)
                {
                    MessageBox.Show("TMflow not connected...");
                    Close();
                    return;
                }

                uint result = 0;
                result = ShellUI.SystemProvider.ShowTMflow();

                if (result != 0)
                {
                    string errMsg = string.Empty;
                    TMcraftErr TMe = ShellUI.GetErrMsg(result, out errMsg);

                    if (TMe != TMcraftErr.OK)
                    {
                        MessageBox.Show(result.ToString() + " ; TMcraftErr : " + TMe.ToString());
                    }
                    else
                    {
                        MessageBox.Show(result.ToString() + " : " + errMsg);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void Btn_Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ShellUI == null || ShellUI.RobotStatusProvider == null)
                {
                    MessageBox.Show("TMflow not connected...");
                    return;
                }

                uint result = 0;
                int mode = 0;
                result = ShellUI.RobotStatusProvider.GetOperationMode(out mode);
                if (result == 0)
                {
                    switch (mode)
                    {
                        case 0: //Manual mode
                            result = 0;
                            result = ShellUI.SystemProvider.LogIn("administrator", "");
                            if (result == 0 || result == 4026532065)
                            {
                                result = 0;
                                result = ShellUI.SystemProvider.GetControl(true);
                                if (result == 0)
                                {
                                    MessageBox.Show("Login successfully");
                                    return;
                                }
                            }
                            break;
                        case 1: //Auto mode
                            MessageBox.Show("Current mode : Auto \r\nPlease switch to Manual mode before login...");
                            return;
                        default:
                            MessageBox.Show("Invalid mode: " + mode.ToString());
                            return;
                    }

                    string errMsg = string.Empty;
                    TMcraftErr TMe = ShellUI.GetErrMsg(result, out errMsg);

                    if (TMe != TMcraftErr.OK)
                    {
                        MessageBox.Show(result.ToString() + " ; TMcraftErr : " + TMe.ToString());
                    }
                    else
                    {
                        MessageBox.Show(result.ToString() + " : " + errMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Btn_Jog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ShellUI == null || ShellUI.RobotStatusProvider == null)
                {
                    MessageBox.Show("TMflow not connected...");
                    return;
                }

                if (!JogStatus)
                {
                    float[] TargetAngle = { 0, 0, 90, 0, 90, 0 };
                    ShellUI.RobotJogProvider.JogByJoint(3f, TargetAngle);
                    ShellUI.RobotJogProvider.HoldPlayKeyToRun(true);
                    JogStatus = true;

                    th_KeepJog = new Thread(_KeepJogging);
                    th_KeepJog.Start();
                    
                    Btn_Jog.Content = "Stop Jogging";
                    Btn_Jog.Background = Brushes.PaleVioletRed;

                }
                else
                {
                    ShellUI.RobotJogProvider.HoldPlayKeyToRun(false);
                    JogStatus = false;

                    th_KeepJog.Join();
                    ShellUI.RobotJogProvider.StopJog();

                    Btn_Jog.Content = "Start Jogging";
                    Btn_Jog.Background = Brushes.LightSeaGreen;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }            
        }

        private void _KeepJogging()
        {
            while (JogStatus)
            {
                if (ShellUI == null || ShellUI.RobotStatusProvider == null)
                {
                    MessageBox.Show("TMflow not connected...");
                    return;
                }

                ShellUI.RobotJogProvider.KeepJogging();
                Thread.Sleep(300); //100 - 500 ms
            }
        }
    }
}
