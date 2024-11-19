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
using TMcraft;

namespace HelloWorldNode
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl,ITMcraftNodeEntry
    {
        TMcraftNodeAPI NodeUI;
        string _TMscript = string.Empty;
        bool fgSave = false;
        public MainPage()
        {
            InitializeComponent();
        }       

        public void InitializeNode(TMcraftNodeAPI _nodeUI)
        {
            NodeUI = _nodeUI; //connect TMflow
        }

        public void InscribeScript(ScriptWriteProvider scriptWriter)
        {
            if (fgSave)
            {
                scriptWriter.AppendScript(_TMscript);
                string str = scriptWriter.GetScript();
                MessageBox.Show(str);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try //Get the saved node configuration from its DataStorage and initialize the Node UI
            {

                if (NodeUI == null || NodeUI.RobotStatusProvider == null)
                {
                    MessageBox.Show("[Load] No connection with TMflow...");
                    return;
                }
                else
                {
                    uint result = 0;
                    string TMErrMsg = string.Empty;

                    Dictionary<string, object> NodeConfigs = new Dictionary<string, object>();
                    result = NodeUI.DataStorageProvider.GetAllData(out NodeConfigs);

                    if(result == 0)
                    {
                        if (NodeConfigs.Count == 0)//Node is implement at the first time.
                        {
                            TextB_Main.Text = String.Empty;
                        }
                        else if (NodeConfigs.Count == 1)
                        {
                            string str_Content = string.Empty;
                            result = NodeUI.DataStorageProvider.GetData("Content", out str_Content);
                            if (result != 0)
                            {
                                NodeUI.GetErrMsg(result, out TMErrMsg);
                                MessageBox.Show("GetData Failure: " + TMErrMsg);
                            }
                            else
                            {
                                TextB_Main.Text = str_Content;
                            }
                        }
                    }
                    else
                    {
                        string errMsg = string.Empty;
                        TMcraftErr TMe = NodeUI.GetErrMsg(result, out errMsg);

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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            string str = TextB_Main.Text;
            _TMscript = "Display(\"Green\", \"White\", \"Hello World\", \"" + str + "\")";

            if (NodeUI != null)
            {
                uint result;
                result = NodeUI.DataStorageProvider.SaveData("Content", str);
                string TMErrMsg = string.Empty;

                if (result == 0)
                {
                    fgSave = true;
                    NodeUI.Close();
                }
                else
                {
                    string errMsg = string.Empty;
                    TMcraftErr TMe = NodeUI.GetErrMsg(result, out errMsg);

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
            else
            {
                MessageBox.Show("[Save] No connection with TMflow...");
            }
        }
    }
}