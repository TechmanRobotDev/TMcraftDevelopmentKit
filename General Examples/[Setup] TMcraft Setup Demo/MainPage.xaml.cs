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

namespace TMcraftSetupDemo
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl, ITMcraftSetupEntry
    {
        TMcraftSetupAPI setupUI;
        bool DO_0 = false; bool DO_1 = false; bool DO_2 = false;
        bool DO_3 = false; bool DO_4 = false; bool DO_5 = false;

        string scriptTemplate = "IO[\"ControlBox\"].DO[0] = DO0\r\nIO[\"ControlBox\"].DO[1] = DO1\r\nIO[\"ControlBox\"].DO[2] = DO2\r\nIO[\"ControlBox\"].DO[3] = DO3\r\nIO[\"ControlBox\"].DO[4] = DO4\r\nIO[\"ControlBox\"].DO[5] = DO5\r\nDisplay(\"Initialization Complete\")";

        SolidColorBrush Color_Disable = new SolidColorBrush();
        SolidColorBrush Color_Enable = new SolidColorBrush();        

        public MainPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, EventArgs e)
        {
            //Color_Disable.Color = Color.FromArgb(70, 53, 160, 192);
            Color_Disable.Color = Color.FromRgb(211, 211, 211);
            Color_Enable.Color = Color.FromRgb(141, 192, 53);

            try
            {
                string str_Version = TMcraftSetupAPI.Version;
                Label_Version.Content = str_Version;

                if (setupUI == null || setupUI.RobotStatusProvider == null)
                {
                    //MessageBox.Show("No Connection with TMflow...");
                    TextBlock_MsgBox.Text = "No Connection with TMflow...";
                    return;
                }

                InitializeConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        
        public void InitializeSetup(TMcraftSetupAPI _setupUI)
        {
            try
            {
                setupUI = _setupUI;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }         
            
        }
        
        public void EmptyConfig()
        {
            Btn_DO_0.Background = Color_Disable;
            DO_0 = false;
            Btn_DO_1.Background = Color_Disable;
            DO_1 = false;
            Btn_DO_2.Background = Color_Disable;
            DO_2 = false;
            Btn_DO_3.Background = Color_Disable;
            DO_3 = false;
            Btn_DO_4.Background = Color_Disable;
            DO_4 = false;
            Btn_DO_5.Background = Color_Disable;
            DO_5 = false;
        }

        public void SetConfig(Dictionary<string, object> setupConfig)
        {
            object Config_0; object Config_1; object Config_2;
            object Config_3; object Config_4; object Config_5;

            try
            {
                if (setupConfig.ContainsKey("DO_0"))
                {
                    setupConfig.TryGetValue("DO_0", out Config_0);
                    switch (Config_0.ToString())
                    {
                        case "0":
                            Btn_DO_0.Background = Color_Disable;
                            DO_0 = false;
                            break;
                        case "1":
                            Btn_DO_0.Background = Color_Enable;
                            DO_0 = true;
                            break;
                        default:
                            TextBlock_MsgBox.Text = "Invalid DO_0 value: " + Config_0.ToString();
                            return;
                    }
                }

                if (setupConfig.ContainsKey("DO_1"))
                {
                    setupConfig.TryGetValue("DO_1", out Config_1);
                    switch (Config_1.ToString())
                    {
                        case "0":
                            Btn_DO_1.Background = Color_Disable;
                            DO_1 = false;
                            break;
                        case "1":
                            Btn_DO_1.Background = Color_Enable;
                            DO_1 = true;
                            break;
                        default:
                            TextBlock_MsgBox.Text = "Invalid DO_0 value: " + Config_1.ToString();
                            return;
                    }
                }

                if (setupConfig.ContainsKey("DO_2"))
                {
                    setupConfig.TryGetValue("DO_2", out Config_2);
                    switch (Config_2.ToString())
                    {
                        case "0":
                            Btn_DO_2.Background = Color_Disable;
                            DO_2 = false;
                            break;
                        case "1":
                            Btn_DO_2.Background = Color_Enable;
                            DO_2 = true;
                            break;
                        default:
                            TextBlock_MsgBox.Text = "Invalid DO_0 value: " + Config_2.ToString();
                            return;
                    }
                }

                if (setupConfig.ContainsKey("DO_3"))
                {
                    setupConfig.TryGetValue("DO_3", out Config_3);
                    switch (Config_3.ToString())
                    {
                        case "0":
                            Btn_DO_3.Background = Color_Disable;
                            DO_3 = false;
                            break;
                        case "1":
                            Btn_DO_3.Background = Color_Enable;
                            DO_3 = true;
                            break;
                        default:
                            TextBlock_MsgBox.Text = "Invalid DO_0 value: " + Config_3.ToString();
                            return;
                    }
                }

                if (setupConfig.ContainsKey("DO_4"))
                {
                    setupConfig.TryGetValue("DO_4", out Config_4);
                    switch (Config_4.ToString())
                    {
                        case "0":
                            Btn_DO_4.Background = Color_Disable;
                            DO_4 = false;
                            break;
                        case "1":
                            Btn_DO_4.Background = Color_Enable;
                            DO_4 = true;
                            break;
                        default:
                            TextBlock_MsgBox.Text = "Invalid DO_0 value: " + Config_4.ToString();
                            return;
                    }
                }

                if (setupConfig.ContainsKey("DO_5"))
                {
                    setupConfig.TryGetValue("DO_5", out Config_5);
                    switch (Config_5.ToString())
                    {
                        case "0":
                            Btn_DO_5.Background = Color_Disable;
                            DO_5 = false;
                            break;
                        case "1":
                            Btn_DO_5.Background = Color_Enable;
                            DO_5 = true;
                            break;
                        default:
                            TextBlock_MsgBox.Text = "Invalid DO_0 value: " + Config_5.ToString();
                            return;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        public void InitializeConfig()
        {
            try
            {
                if(setupUI == null || setupUI.DataStorageProvider == null)
                {
                    TextBlock_MsgBox.Text = "TMflow not connected...";
                    return;
                }
                else
                {
                    Dictionary<string, object> setupConfig;
                    setupUI.DataStorageProvider.GetAllData(out setupConfig);
                    
                    switch(setupConfig.Count)
                    {
                        case 0:
                            TextBlock_MsgBox.Text = "No intialization script yet.";
                            EmptyConfig();
                            return;
                        case 6:
                            SetConfig(setupConfig);
                            return;
                        default:
                            TextBlock_MsgBox.Text = "Invalid Number of data: " + setupConfig.ToString();
                            return;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Btn_DO_0_Click(object sender, RoutedEventArgs e)
        {
            if (DO_0)
            {
                Btn_DO_0.Background = Color_Disable;
                DO_0 = false;
            }
            else
            {
                Btn_DO_0.Background = Color_Enable;
                DO_0 = true;
            }
        }

        private void Btn_DO_1_Click(object sender, RoutedEventArgs e)
        {
            if (DO_1)
            {
                Btn_DO_1.Background = Color_Disable;
                DO_1 = false;
            }
            else
            {
                Btn_DO_1.Background = Color_Enable;
                DO_1 = true;
            }
        }

        private void Btn_DO_2_Click(object sender, RoutedEventArgs e)
        {
            if (DO_2)
            {
                Btn_DO_2.Background = Color_Disable;
                DO_2 = false;
            }
            else
            {
                Btn_DO_2.Background = Color_Enable;
                DO_2 = true;
            }
        }

        private void Btn_DO_3_Click(object sender, RoutedEventArgs e)
        {
            if (DO_3)
            {
                Btn_DO_3.Background = Color_Disable;
                DO_3 = false;
            }
            else
            {
                Btn_DO_3.Background = Color_Enable;
                DO_3 = true;
            }
        }

        private void Btn_DO_4_Click(object sender, RoutedEventArgs e)
        {
            if (DO_4)
            {
                Btn_DO_4.Background = Color_Disable;
                DO_4 = false;
            }
            else
            {
                Btn_DO_4.Background = Color_Enable;
                DO_4 = true;
            }
        }

        private void Btn_DO_5_Click(object sender, RoutedEventArgs e)
        {
            if (DO_5)
            {
                Btn_DO_5.Background = Color_Disable;
                DO_5 = false;
            }
            else
            {
                Btn_DO_5.Background = Color_Enable;
                DO_5 = true;
            }
        }               

        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            EmptyConfig();

            try
            {
                if (setupUI == null || setupUI.ScriptWriteProvider == null)
                {
                    MessageBox.Show("No connection with TMflow...");
                    return;
                }
                else
                {
                    setupUI.ScriptWriteProvider.ClearBuffer();
                    Thread.Sleep(50);
                    setupUI.ScriptWriteProvider.SaveBufferAsScript();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return;
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            string script;
            Dictionary<string, string> setupConfig = new Dictionary<string, string>();
            generateScript(out script, out setupConfig);//generate script according to the setting
            TextBlock_MsgBox.Text = script;           

            try
            {
                if(setupUI == null || setupUI.ScriptWriteProvider == null || setupUI.DataStorageProvider == null)
                {
                    MessageBox.Show("No connection with TMflow...");
                    return;
                }
                else
                {
                    //Inscribe initialization script
                    setupUI.ScriptWriteProvider.ClearBuffer();
                    setupUI.ScriptWriteProvider.AppendScriptToBuffer(script);
                    Thread.Sleep(50);
                    setupUI.ScriptWriteProvider.SaveBufferAsScript();

                    //Save the setting configuraiton
                    setupUI.DataStorageProvider.SaveData(setupConfig);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void generateScript(out string _str_Script, out Dictionary<string, string> _setupConfig)
        {
            string str_Script = string.Empty;
            Dictionary<string,string> setupConfig = new Dictionary<string, string>();            

            if (DO_0)
            {
                str_Script += "byte DO0 = 1" + Environment.NewLine;
                setupConfig.Add("DO_0", "1");
            }
            else
            {
                str_Script += "byte DO0 = 0" + Environment.NewLine;
                setupConfig.Add("DO_0", "0");
            }

            if (DO_1)
            {
                str_Script += "byte DO1 = 1" + Environment.NewLine;
                setupConfig.Add("DO_1", "1");
            }
            else
            {
                str_Script += "byte DO1 = 0" + Environment.NewLine;
                setupConfig.Add("DO_1", "0");
            }

            if (DO_2)
            {
                str_Script += "byte DO2 = 1" + Environment.NewLine;
                setupConfig.Add("DO_2", "1");
            }
            else
            {
                str_Script += "byte DO2 = 0" + Environment.NewLine;
                setupConfig.Add("DO_2", "0");
            }

            if (DO_3)
            {
                str_Script += "byte DO3 = 1" + Environment.NewLine;
                setupConfig.Add("DO_3", "1");
            }
            else
            {
                str_Script += "byte DO3 = 0" + Environment.NewLine;
                setupConfig.Add("DO_3", "0");
            }

            if (DO_4)
            {
                str_Script += "byte DO4 = 1" + Environment.NewLine;
                setupConfig.Add("DO_4", "1");
            }
            else
            {
                str_Script += "byte DO4 = 0" + Environment.NewLine;
                setupConfig.Add("DO_4", "0");
            }

            if (DO_5)
            {
                str_Script += "byte DO5 = 1" + Environment.NewLine;
                setupConfig.Add("DO_5", "1");
            }
            else
            {
                str_Script += "byte DO5 = 0" + Environment.NewLine;
                setupConfig.Add("DO_5", "0");
            }

            str_Script += scriptTemplate + Environment.NewLine;

            _str_Script = str_Script;
            _setupConfig = setupConfig;            
        }

        private void Btn_script_Click(object sender, RoutedEventArgs e)
        {
            string script = string.Empty;

            try
            {
                if (setupUI == null || setupUI.RobotStatusProvider == null)
                {
                    TextBlock_MsgBox.Text = "No connection with TMflow...";
                    
                    return;
                }
                else
                {
                    setupUI.ScriptWriteProvider.GetScript(out script);
                    TextBlock_MsgBox.Text = script;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}