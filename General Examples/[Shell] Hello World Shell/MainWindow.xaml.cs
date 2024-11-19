using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
using System.Xml.XPath;
using TMcraft;

namespace HelloWorldShell
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
            if(ShellUI == null)
            {
                ShellUI = new TMcraftShellAPI();
                ShellUI.InitialTMcraftShell();
            }

            if (ShellUI == null || ShellUI.RobotStatusProvider == null)
            {
                MessageBox.Show("Connection failed");
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e) { }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) 
        {
            try
            {
                if(ShellUI != null)
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

        private void Btn_Script_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ShellUI == null || ShellUI.RobotStatusProvider == null)
                {
                    MessageBox.Show("TMflow not connected...");
                    return;
                }

                uint result = 0;
                result = ShellUI.ScriptProjectProvider.NewScriptProject("scriptByShell");
                if (result == 0 || result == 262216) //Successfully create project or project already exists
                {
                    result = 0;
                    result = ShellUI.ScriptProjectProvider.OpenScriptProject("scriptByShell");
                    if (result == 0 || result == 262185) //Successfully open project or project is already opened
                    {
                        string script = "define\r\n{\r\n\r\n}\r\nmain\r\n{\r\n";
                        string str = TextBox_Content.Text;
                        string main_script = "Display(\"Green\", \"White\", \"Hello World\", \"" + str + "\")";
                        script = script + main_script + "\r\n}\r\nclosestop\r\n{\r\n\r\n}\r\nerrorstop\r\n{\r\n\r\n}";

                        MessageBox.Show(script);

                        result = 0;
                        result = ShellUI.ScriptProjectProvider.WriteScriptProjectContent(script);
                        if (result == 0)
                        {
                            MessageBox.Show("Succeed.");
                        }
                    }
                }
                else
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
                MessageBox.Show(ex.ToString());
            }
        }

        
    }
}
