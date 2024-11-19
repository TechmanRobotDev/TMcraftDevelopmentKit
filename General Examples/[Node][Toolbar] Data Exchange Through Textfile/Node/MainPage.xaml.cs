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

namespace DataExchangeDemo_node
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl, ITMcraftNodeEntry
    {
        TMcraftNodeAPI NodeUI;
        const string TargetTextFile = "DataExchangeDemo";
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
            string str = "Data: " + System.Environment.NewLine + TextBox_A.Text + System.Environment.NewLine + TextBox_B.Text + System.Environment.NewLine + TextBox_C.Text;

            string script = "Display(\"" + str + "\")";

            try
            {
                scriptWriter.AppendLine(script);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(NodeUI == null || NodeUI.TextFileProvider == null)
            {
                Label_Msg.Content = "[Error] No Connection with TMflow...";
                return;
            }

            try
            {
                string[] TextFileList;
                uint result = 0;
                bool HasTargetFile = false;

                result = NodeUI.TextFileProvider.GetTextFileList(out TextFileList);
                if (result != 0)
                {
                    string ErrMsg = String.Empty;
                    NodeUI.GetErrMsg(result, out ErrMsg);
                    Label_Msg.Content = "[Error] " + ErrMsg;

                    return;
                }
                
                foreach (string TextFileName in TextFileList)
                {
                    if (TextFileName == TargetTextFile)
                    {
                        HasTargetFile = true;
                        break;
                    }
                }

                if (!HasTargetFile)
                {
                    Label_Msg.Content = "TextFile not Found ...";
                }
                else
                {
                    string[] data;

                    if (getDataFromFile(out data))
                    {
                        TextBox_A.Text = data[0];
                        TextBox_B.Text = data[1];
                        TextBox_C.Text = data[2];
                    }
                    else return;
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }          

        }

        private bool getDataFromFile(out string[] _data)
        {
            string[] fileData = { "", "", "" };
            string str = String.Empty;
            string[] Ary;

            if (NodeUI == null || NodeUI.TextFileProvider == null)
            {
                MessageBox.Show("No Connection with TMflow...");
                _data = fileData;
                return false;
            }

            try
            {
                uint result = 0;

                result = NodeUI.TextFileProvider.ReadTextFile(TargetTextFile, out str);

                if (result != 0)
                {
                    string ErrMsg = String.Empty;
                    NodeUI.GetErrMsg(result, out ErrMsg);
                    Label_Msg.Content = "[Error] " + ErrMsg;

                    _data = fileData;
                    return false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _data = fileData;
                return false;
            }

            string[] Separator = { ";$;" };
            Ary = str.Split(Separator, StringSplitOptions.None);

            if (Ary.Length != 3)
            {
                MessageBox.Show("Invalid Data: " + str);
                _data = fileData;
                return false;
            }
            else
            {
                _data = Ary;
                return true;
            }
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            if(NodeUI == null || NodeUI.RobotStatusProvider == null)
            {
                Environment.Exit(0);
            }
            else
            {
                try
                {
                    NodeUI.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
        }
    }
}