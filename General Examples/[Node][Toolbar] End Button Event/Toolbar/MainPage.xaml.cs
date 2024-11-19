using System.Text;
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
using TMcraft;

namespace Toolbar_EndButtonEvt_220
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl, ITMcraftToolbarEntry
    {
        TMcraftToolbarAPI toolbarUI;
        bool getOwnership;
        SolidColorBrush Color_Disable = new SolidColorBrush();
        SolidColorBrush Color_Enable = new SolidColorBrush();

        public MainPage()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, EventArgs e)
        {
            try
            {
                if (toolbarUI == null || toolbarUI.RobotStatusProvider == null)
                {
                    MessageBox.Show("No Connection with TMflow...");
                }
                else
                {
                    Color_Disable.Color = Color.FromArgb(70, 53, 160, 192);
                    Color_Enable.Color = Color.FromRgb(141, 192, 53);

                    Bulb_PointButton.Fill = Color_Disable;
                    Bulb_GripperButton.Fill = Color_Disable;
                    Bulb_VisionButton.Fill = Color_Disable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public void InitializeToolbar(TMcraftToolbarAPI _toolbarUI)
        {
            toolbarUI = _toolbarUI;
            if (toolbarUI == null || toolbarUI.EndButtonEventProvider == null)
            {
                MessageBox.Show("No Connection with TMflow...");
            }
            else
            {
                toolbarUI.EndButtonEventProvider.EndButtonClickEvent += EndButtonClickEventReaction;
            }
        }
        private void EndButtonClickEventReaction(RobotEventType type, object data)
        {
            Dispatcher.Invoke(
                DispatcherPriority.Background,
                new Action(delegate ()
                {
                    switch (type.ToString() + " " + data.ToString())
                    {
                        case "EndButtonPointChanged True":
                            Bulb_PointButton.Fill = Color_Enable;
                            break;
                        case "EndButtonPointChanged False":
                            Bulb_PointButton.Fill = Color_Disable;
                            break;

                        case "EndButtonGripperChanged True":
                            Bulb_GripperButton.Fill = Color_Enable;
                            break;
                        case "EndButtonGripperChanged False":
                            Bulb_GripperButton.Fill = Color_Disable;
                            break;

                        case "EndButtonVisionChanged True":
                            Bulb_VisionButton.Fill = Color_Enable;
                            break;
                        case "EndButtonVisionChanged False":
                            Bulb_VisionButton.Fill = Color_Disable;
                            break;

                        default:
                            MessageBox.Show("Invalid RobotEvent: " + type.ToString() + " " + data.ToString());
                            break;
                    }

                }));
        }

        private void Btn_Ownership_Click(object sender, RoutedEventArgs e)
        {
            if (toolbarUI == null || toolbarUI.EndButtonEventProvider == null)
            {
                MessageBox.Show("No connection with TMflow...");
                return;
            }

            if (!getOwnership)
            {
                Btn_Ownership.IsEnabled = false;

                if (!toolbarUI.EndButtonEventProvider.IsEndButtonBoardcastMode() && !toolbarUI.EndButtonEventProvider.HasEndButtonEventOwnership())
                {
                    MessageBox.Show("End Button Event Ownership has been obtained by someone else...");
                    Btn_Ownership.IsEnabled = true;
                    return;
                }

                uint result = toolbarUI.EndButtonEventProvider.SetEndButtonEventOwnership();
                if (result == 0)
                {
                    getOwnership = true;
                    Label_Ownership.Content = "Press To Release End Button Ownership";
                    Btn_Ownership.Background = Brushes.GreenYellow;
                    Btn_Ownership.IsEnabled = true;
                    return;
                }
                else
                {
                    string msg = string.Empty;
                    toolbarUI.GetErrMsg(result, out msg);
                    MessageBox.Show(msg);
                    Btn_Ownership.IsEnabled = true;
                    return;
                }
            }
            else
            {
                Btn_Ownership.IsEnabled = false;

                uint result = toolbarUI.EndButtonEventProvider.ReleaseEndButtonEventOwnership();
                if (result == 0)
                {
                    getOwnership = false;
                    Label_Ownership.Content = "Press To Obtain End Button Ownership";
                    Btn_Ownership.Background = Brushes.SkyBlue;
                    Btn_Ownership.IsEnabled = true;
                    return;
                }
                else
                {
                    string msg = string.Empty;
                    toolbarUI.GetErrMsg(result, out msg);
                    MessageBox.Show(msg);
                    Btn_Ownership.IsEnabled = true;
                    return;
                }
            }
        }
    }

}
