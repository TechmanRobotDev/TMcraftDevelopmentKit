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

namespace HelloWorldToolbar
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl, ITMcraftToolbarEntry
    {
        TMcraftToolbarAPI ToolbarUI;
        bool status_CDO0 = false;
        bool status_EDO0 = false;
        
        public MainPage()
        {
            InitializeComponent();
        }

        public void InitializeToolbar(TMcraftToolbarAPI _toolbarUI)
        {
            ToolbarUI = _toolbarUI;
        }

        private void UserControl_Loaded(object sender, EventArgs e)
        {
            try
            {
                if (ToolbarUI == null || ToolbarUI.RobotStatusProvider == null)
                {
                    MessageBox.Show("No Connection with TMflow...");
                }
                else
                {
                   ToolbarUI.IOProvider.ReadDigitOutput(IO_TYPE.CONTROL_BOX, 0, 0, out status_CDO0);
                    if (status_CDO0)
                    {
                        Btn_CtrlDO0.Background = Brushes.GreenYellow;
                    }
                    else
                    {
                        Btn_CtrlDO0.Background = Brushes.White;
                    }

                    ToolbarUI.IOProvider.ReadDigitOutput(IO_TYPE.END_MODULE, 0, 0, out status_EDO0);
                    if (status_EDO0)
                    {
                        Btn_EndDO0.Background = Brushes.GreenYellow;
                    }
                    else
                    {
                        Btn_EndDO0.Background = Brushes.White;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void Btn_CtrlDO0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ToolbarUI == null || ToolbarUI.RobotStatusProvider == null)
                {
                    MessageBox.Show("No Connection with TMflow...");
                    return;
                }

                if (status_CDO0)
                {
                    ToolbarUI.IOProvider.WriteDigitOutput(IO_TYPE.CONTROL_BOX, 0, 0, false);
                    status_CDO0 = false;
                    Btn_CtrlDO0.Background = Brushes.White;
                }
                else
                {
                    ToolbarUI.IOProvider.WriteDigitOutput(IO_TYPE.CONTROL_BOX, 0, 0, true);
                    status_CDO0 = true;
                    Btn_CtrlDO0.Background = Brushes.GreenYellow;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Btn_EndDO0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ToolbarUI == null || ToolbarUI.RobotStatusProvider == null)
                {
                    MessageBox.Show("No Connection with TMflow...");
                    return;
                }

                if (status_EDO0)
                {
                    ToolbarUI.IOProvider.WriteDigitOutput(IO_TYPE.END_MODULE, 0, 0, false);
                    status_EDO0 = false;
                    Btn_EndDO0.Background = Brushes.White;
                }
                else
                {
                    ToolbarUI.IOProvider.WriteDigitOutput(IO_TYPE.END_MODULE, 0, 0, true);
                    status_EDO0 = true;
                    Btn_EndDO0.Background = Brushes.GreenYellow;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }    
}