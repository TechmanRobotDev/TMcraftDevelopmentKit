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
using System.Threading;
using TMcraft;

namespace FreebotByVirtualButton
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl, ITMcraftToolbarEntry
    {
        TMcraftToolbarAPI ToolbarUI;
        FreeBotInfo _freebot;        
        bool FreebotStatus = false;
        CancellationTokenSource cts = new CancellationTokenSource();        
        Thread th_KeepFreebot;
        
        public void InitializeToolbar(TMcraftToolbarAPI _toolbarUI)
        {
            ToolbarUI = _toolbarUI;
        }
         public void FreebotInfo2Str(FreeBotInfo freebot,out string result)
        {
            string str;
            str = "Mode: " + freebot.Mode.ToString() + Environment.NewLine;
            str += "Move Mode: " + freebot.MoveMode.ToString() + Environment.NewLine;
            str += "Is Base Mode: " + freebot.isBaseMode.ToString() + Environment.NewLine;
            str += "Free X:" + freebot.isFreeX.ToString() + Environment.NewLine;
            str += "Free Y:" + freebot.isFreeY.ToString() + Environment.NewLine;
            str += "Free z:" + freebot.isFreeZ.ToString() + Environment.NewLine;
            str += "Free Rx:" + freebot.isFreeRX.ToString() + Environment.NewLine;
            str += "Free Ry:" + freebot.isFreeRY.ToString() + Environment.NewLine;
            str += "Free Rz:" + freebot.isFreeRZ.ToString() + Environment.NewLine;
            result = str;
        }
        public MainPage()
        {
            CancellationToken token = cts.Token;

            th_KeepFreebot = new Thread(() => _KeepFreebot(token));
            th_KeepFreebot.IsBackground = true;
            th_KeepFreebot.Start();
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ToolbarUI == null ||ToolbarUI.FreeBotProvider == null) 
            {
                TextB_Main.Text = "No connection";
                return;
            }

            ToolbarUI.FreeBotProvider.GetFreeBot(out _freebot);
            string str;
            FreebotInfo2Str(_freebot, out str);
            TextB_Main.Text = str;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if(ToolbarUI != null && ToolbarUI.FreeBotProvider != null)
            {
                FreebotStatus = false;
                ToolbarUI.FreeBotProvider.HoldFreeBotKeyToHandGuide(false);
            }

            cts.Cancel();
            th_KeepFreebot.Join();

        }
            private void Btn_FreeAll_Click(object sender, RoutedEventArgs e)
        {
            if (ToolbarUI == null || ToolbarUI.FreeBotProvider == null)
            {
                TextB_Main.Text = "No connection";
                return;
            }
            
            _freebot.Mode = FreeBotMode.All_Joints;
            string msg;
            FreebotInfo2Str(_freebot,out msg);
            //MessageBox.Show(msg);
            uint result = ToolbarUI.FreeBotProvider.SetFreeBot(_freebot);

            if (result == 0)
            {
                Thread.Sleep(100);
                string headline = "Free all joints set." + Environment.NewLine;
                ToolbarUI.FreeBotProvider.GetFreeBot(out _freebot);
                string str;
                FreebotInfo2Str(_freebot, out str);
                TextB_Main.Text = headline + str;
            }
            else
            {
                string temp;
                ToolbarUI.GetErrMsg(result, out temp);
                TextB_Main.Text = temp;
            }
            
            
        }

        private void Btn_FreeXYZ_Click(object sender, RoutedEventArgs e)
        {
            if (ToolbarUI == null || ToolbarUI.FreeBotProvider == null)
            {
                TextB_Main.Text = "No connection";
                return;
            }

            _freebot.Mode = FreeBotMode.XYZ;
            string msg;
            FreebotInfo2Str(_freebot, out msg);
            //MessageBox.Show(msg);
            ToolbarUI.FreeBotProvider.SetFreeBot(_freebot);
            string headline = "Free XYZ set." + Environment.NewLine;

            Thread.Sleep(100);
            ToolbarUI.FreeBotProvider.GetFreeBot(out _freebot);

            string str;
            FreebotInfo2Str(_freebot,out str);
            TextB_Main.Text = headline + str;
        }

        private void Btn_Scara_Click(object sender, RoutedEventArgs e)
        {
            if (ToolbarUI == null || ToolbarUI.FreeBotProvider == null)
            {
                TextB_Main.Text = "No connection";
                return;
            }

            _freebot.Mode = FreeBotMode.SCARA_Like;
            string msg;
            FreebotInfo2Str(_freebot, out msg);
            //MessageBox.Show(msg);
            ToolbarUI.FreeBotProvider.SetFreeBot(_freebot);
            string headline = "SCARA-like set." + Environment.NewLine;
            Thread.Sleep(100);
            ToolbarUI.FreeBotProvider.GetFreeBot(out _freebot);

            string str;
            FreebotInfo2Str(_freebot, out str);
            TextB_Main.Text = headline + str;
        }

        private void Btn_Freebot_Click(object sender, RoutedEventArgs e)
        {
            Btn_Freebot.IsEnabled = false;
            if (ToolbarUI == null || ToolbarUI.FreeBotProvider == null)
            {
                TextB_Main.Text = "No connection";
                Btn_Freebot.IsEnabled = true;
                return;
            }

            try
            {
                if(!FreebotStatus)
                {
                    ToolbarUI.FreeBotProvider.HoldFreeBotKeyToHandGuide(true);
                    FreebotStatus = true;
                    //th_KeepFreebot = new Thread(_KeepFreebot);
                    //th_KeepFreebot.Start();

                    Btn_Freebot.Content = "Press to disable Freebot";
                    Btn_Freebot.Background = Brushes.PaleVioletRed;
                    Btn_Freebot.IsEnabled = true;
                }
                else
                {
                    ToolbarUI.FreeBotProvider.HoldFreeBotKeyToHandGuide(false);
                    FreebotStatus = false;
                    //th_KeepFreebot.Join();

                    Btn_Freebot.Content = "Press and Freebot";
                    Btn_Freebot.Background = Brushes.PaleGoldenrod;
                    Btn_Freebot.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void _KeepFreebot(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (FreebotStatus)
                {
                    ToolbarUI.FreeBotProvider.KeepFreeBot();
                }

                Thread.Sleep(150); //100-500ms                
            }
        }
    }

}
