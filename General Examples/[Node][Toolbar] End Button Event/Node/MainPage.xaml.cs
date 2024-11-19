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

namespace Node_EndButtonEvt_220
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl, ITMcraftNodeEntry
    {
        TMcraftNodeAPI nodeUI;
        SolidColorBrush Color_Disable = new SolidColorBrush();
        SolidColorBrush Color_Enable = new SolidColorBrush();
        bool getOwnership = false;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (nodeUI == null) { MessageBox.Show("[UserControl Load] Cannot connect to TMflow..."); }

                //Initialize the UI.
                Color_Disable.Color = Color.FromArgb(70, 53, 160, 192);
                Color_Enable.Color = Color.FromRgb(141, 192, 53);

                Bulb_PointButton.Fill = Color_Disable;
                Bulb_GripperButton.Fill = Color_Disable;
                Bulb_VisionButton.Fill = Color_Disable;
            }
            catch (Exception ex) { MessageBox.Show("UserControl fails loading: " + ex.Message); }
        }
        public void InitializeNode(TMcraftNodeAPI _nodeUI)
        {
            nodeUI = _nodeUI;
            if(nodeUI != null && nodeUI.EndButtonEventProvider != null)
            {
                nodeUI.EndButtonEventProvider.EndButtonClickEvent += EndButtonClickEventReaction;
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
        public void InscribeScript(ScriptWriteProvider scriptWriter)
        {
            //This demo does not requires TMscript.
        }

        private void UserControl_UnLoaded(object sender, RoutedEventArgs e)
        {
            if(nodeUI == null || nodeUI.EndButtonEventProvider == null)
            {
                return;
            }
            else
            {
                nodeUI.EndButtonEventProvider.ReleaseEndButtonEventOwnership();
                nodeUI.Close();
                return;
            }
            
        }
        public MainPage()
        {
            InitializeComponent();
        }

        private void Btn_Ownership_Click(object sender, RoutedEventArgs e)
        {
            if (nodeUI == null || nodeUI.EndButtonEventProvider == null)
            {
                MessageBox.Show("No connection with TMflow...");
                return;
            }

            if(!getOwnership)
            {
                Btn_Ownership.IsEnabled = false;

                if(!nodeUI.EndButtonEventProvider.IsEndButtonBoardcastMode() && !nodeUI.EndButtonEventProvider.HasEndButtonEventOwnership())
                {
                    MessageBox.Show("End Button Event Ownership has been obtained by someone else...");
                    Btn_Ownership.IsEnabled = true;
                    return;
                }

                uint result = nodeUI.EndButtonEventProvider.SetEndButtonEventOwnership();
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
                    nodeUI.GetErrMsg(result, out msg);
                    MessageBox.Show(msg);
                    Btn_Ownership.IsEnabled = true;
                    return;


                }               
            }
            else
            {
                Btn_Ownership.IsEnabled = false ;

                uint result = nodeUI.EndButtonEventProvider.ReleaseEndButtonEventOwnership();
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
                    nodeUI.GetErrMsg(result, out msg);
                    MessageBox.Show(msg);
                    Btn_Ownership.IsEnabled = true;
                    return;
                }
            }
        }
    }

}
