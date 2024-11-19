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

namespace DataExchangeDemo_toolbar
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl, ITMcraftToolbarEntry
    {
        TMcraftToolbarAPI ToolbarUI;
        const string TargetTextFile = "DataExchangeDemo";
        public MainPage()
        {
            InitializeComponent();
        }

        public void InitializeToolbar(TMcraftToolbarAPI _toolbarUI)
        {
            ToolbarUI = _toolbarUI;
            if (ToolbarUI == null || ToolbarUI.TextFileProvider == null)
            {
                MessageBox.Show("No Connection with TMflow...");
                return;
            }
        }
        private void UserControl_Loaded(object sender, EventArgs e)
        {
            if (ToolbarUI == null || ToolbarUI.TextFileProvider == null)
            {
                MessageBox.Show("No Connection with TMflow...");
                return;
            }

            try
            {
                string[] TextFileList;
                uint result = 0;
                
                result = ToolbarUI.TextFileProvider.GetTextFileList(out TextFileList);
                if (result != 0)
                {
                    string ErrMsg = String.Empty;
                    ToolbarUI.GetErrMsg(result, out ErrMsg);
                    Label_Msg.Content = "[Error] " + ErrMsg;

                    return;
                }

                bool HasTargetFile = false;
                foreach(string TextFileName in TextFileList) 
                {
                    if (TextFileName == TargetTextFile)
                    {
                        HasTargetFile = true;
                        break;
                    }
                }

                if(!HasTargetFile)
                {
                    result = ToolbarUI.TextFileProvider.NewTextFile(TargetTextFile, " ;$; ;$; ");
                    if (result != 0)
                    {
                        string ErrMsg = String.Empty;
                        ToolbarUI.GetErrMsg(result, out ErrMsg);
                        Label_Msg.Content = "[Error] " + ErrMsg;
                    }
                    else
                    {
                        Label_Msg.Content = "TextFile is created.";
                    }
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
                MessageBox.Show(ex.Message);
            }
        }

        private bool getDataFromFile(out string[] _data)
        {
            string[] fileData = { "","","" };
            string str = String.Empty;
            string[] Ary;

            if (ToolbarUI == null || ToolbarUI.TextFileProvider == null)
            {
                MessageBox.Show("No Connection with TMflow...");
                _data = fileData;
                return false;
            }

            try
            {                
                uint result = 0;

                result = ToolbarUI.TextFileProvider.ReadTextFile(TargetTextFile, out str);

                if(result != 0)
                {
                    string ErrMsg = String.Empty;
                    ToolbarUI.GetErrMsg(result, out ErrMsg);
                    Label_Msg.Content = "[Error] " + ErrMsg;

                    _data = fileData;
                    return false;
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                _data = fileData;
                return false;
            }

            string[] Separator = { ";$;" };
            Ary = str.Split(Separator, StringSplitOptions.None);

            if(Ary.Length != 3)
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

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            string fileContent = TextBox_A.Text + ";$;" + TextBox_B.Text + ";$;" + TextBox_C.Text;

            if (ToolbarUI == null || ToolbarUI.TextFileProvider == null)
            {
                Label_Msg.Content = "No Connection with TMflow...";
                return;
            }

            try
            {
                string[] TextFileList;
                uint result = 0;

                ToolbarUI.TextFileProvider.GetTextFileList(out TextFileList);
                result = ToolbarUI.TextFileProvider.GetTextFileList(out TextFileList);
                if (result != 0)
                {
                    string ErrMsg = String.Empty;
                    ToolbarUI.GetErrMsg(result, out ErrMsg);
                    Label_Msg.Content = "[Error] " + ErrMsg;

                    return;
                }

                bool HasTargetFile = false;
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
                    result = ToolbarUI.TextFileProvider.NewTextFile(TargetTextFile, fileContent);
                    if (result != 0)
                    {
                        string ErrMsg = String.Empty;
                        ToolbarUI.GetErrMsg(result, out ErrMsg);
                        Label_Msg.Content = "[Error] " + ErrMsg;
                    }
                    else
                    {
                        Label_Msg.Content = "TextFile is created with data.";
                    }
                }
                else
                {
                    result = ToolbarUI.TextFileProvider.WriteTextFile(TargetTextFile, fileContent);
                    if (result != 0)
                    {
                        string ErrMsg = String.Empty;
                        ToolbarUI.GetErrMsg(result, out ErrMsg);
                        Label_Msg.Content = "[Error] " + ErrMsg;
                    }
                    else
                    {
                        Label_Msg.Content = "TextFile is modified.";
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}