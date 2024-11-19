using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;
using TMcraft;
using Newtonsoft.Json;
using System.Threading;

namespace TMcraftSellApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        TMcraftShellAPI TMShellEditor;
        string UserSaveData = "";
        bool fgSave = false;
        uint lastError = 0;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cb_Class.Items.Clear();
            cb_Class.Items.Add("TMcraftShell");
            cb_Class.Items.Add("IOProvider");
            cb_Class.Items.Add("ScriptProjectProvider");
            cb_Class.Items.Add("ProjectRunProvider");
            cb_Class.Items.Add("RobotJogProvider");
            cb_Class.Items.Add("RobotStatusProvider");
            cb_Class.Items.Add("RobotStickProvider");
            cb_Class.Items.Add("SystemProvider");
            cb_Class.Items.Add("TCPProvider");
            cb_Class.Items.Add("VariableProvider");
            cb_Class.Items.Add("FreeBotProvider");
            cb_Class.Items.Add("EndButtonEventProvider");
            cb_Class.SelectedIndex = 0;
            txt_Enum.Text = "";
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string name in Enum.GetNames(typeof(VariableType)))
            {
                stringBuilder.AppendLine("VariableType." + name);
            }
            foreach (string name in Enum.GetNames(typeof(VirtualKeyEvent)))
            {
                stringBuilder.AppendLine("VirtualKeyEvent." + name);
            }
            txt_Enum.Text = stringBuilder.ToString();
            if (TMShellEditor == null) 
            {
                TMShellEditor = new TMcraftShellAPI();
                TMShellEditor.InitialTMcraftShell();                
            }
            CNTitle.Content = string.Format("TMcraft Shell Test Tool.(Version = {0})", TMcraftShellAPI.Version);
            if (TMShellEditor == null || TMShellEditor.RobotStatusProvider == null)
            {
                MessageBox.Show("Connection failed");
            }
            else
            {
                TMShellEditor.EndButtonEventProvider.EndButtonClickEvent += RobotStatusProvider_EndButtonClickEvent;
                TMShellEditor.RobotStatusProvider.ErrorEvent += RobotStatusProvider_ErrorEvent;
            }
        }

        private void RobotStatusProvider_ErrorEvent(object data)
        {
            try
            {
                ErrorStatus temp = JsonConvert.DeserializeObject<ErrorStatus>((string)data);
                Dispatcher.BeginInvoke(
                                   DispatcherPriority.Background,
                                   new Action(delegate ()
                                   {
                                       txtRobotEvent.Text = temp.Last_Error_Code.ToString();
                                   }));
            }
            catch { }            
        }

        private void btn_Test_Click(object sender, RoutedEventArgs e)
        {
            uint message = (uint)TMcraftErr.OK;
            if (cb_command.SelectedItem == null || cb_Class.SelectedItem == null)
            {
                return;
            }
            string className = cb_Class.SelectedItem.ToString();
            string target = cb_command.SelectedItem.ToString();
            string Input1Data = Input1.Text.Trim();
            string[] words = Input1Data.Split(',');
            try
            {
                switch (className)
                {
                    case "TMcraftShell":
                        if (target == "GetErrMsg")
                        {
                            uint myErrorNum = 0;
                            if (uint.TryParse(words[0], out myErrorNum))
                            {
                                string tmflowMessage = "";
                                TMcraftErr mcraftErr = TMShellEditor.GetErrMsg(myErrorNum, out tmflowMessage);
                                if (mcraftErr == TMcraftErr.OK)
                                {
                                    txt_res_pay.Text = tmflowMessage;
                                }
                                else
                                {
                                    txt_res_pay.Text = mcraftErr.ToString();
                                }
                            }
                            else
                            {
                                txt_res_pay.Text = TMcraftErr.InvalidParameter.ToString() + " in TMcraft funtion";
                                message = (uint)TMcraftErr.InvalidParameter;
                            }
                        }
                        break;
                    case "ScriptProjectProvider":
                        if (target == "GetScriptProjectList")
                        {
                            List<string[]> list = new List<string[]>();
                            message = TMShellEditor.ScriptProjectProvider.GetScriptProjectList(ref list);
                            txt_res_pay.Text = JsonConvert.SerializeObject(list);
                        }
                        else if (target == "NewScriptProject")
                        {
                            string[] list = new string[] { };
                            message = TMShellEditor.ScriptProjectProvider.NewScriptProject(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "OpenScriptProject")
                        {
                            string[] list = new string[] { };
                            message = TMShellEditor.ScriptProjectProvider.OpenScriptProject(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "SaveScriptProject")
                        {
                            string[] list = new string[] { };
                            message = TMShellEditor.ScriptProjectProvider.SaveScriptProject(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "WriteScriptProjectContent")
                        {
                            message = TMShellEditor.ScriptProjectProvider.WriteScriptProjectContent(txtScriptEnter.Text);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ReadScriptProjectContent")
                        {
                            string textData = "";
                            message = TMShellEditor.ScriptProjectProvider.ReadScriptProjectContent(out textData);
                            txt_res_pay.Text = textData;
                        }
                        else if (target == "ReadScriptProjectRemark")
                        {
                            string textData = "";
                            message = TMShellEditor.ScriptProjectProvider.ReadScriptProjectRemark(words[0], out textData);
                            txt_res_pay.Text = textData;
                        }
                        else if (target == "WriteScriptProjectRemark")
                        {
                            message = TMShellEditor.ScriptProjectProvider.WriteScriptProjectRemark(words[0], txtRemark.Text);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "DeleteScriptProject")
                        {
                            message = TMShellEditor.ScriptProjectProvider.DeleteScriptProject(words[0]);
                            txt_res_pay.Text = "";
                        }
                        break;
                    case "IOProvider":
                        List<DeviceIOInfo> ResultData = new List<DeviceIOInfo>();
                        if (target == "GetAllIOData")
                        {
                            message = TMShellEditor.IOProvider.GetAllIOData(out ResultData);
                            txt_res_pay.Text = JsonConvert.SerializeObject(ResultData);
                        }
                        else if (target == "WriteDigitOutput")
                        {
                            words[0] = words[0].Replace("IO_TYPE.", "");
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMShellEditor.IOProvider.WriteDigitOutput(type, int.Parse(words[1]), int.Parse(words[2]), bool.Parse(words[3]));
                            //message = TMShellEditor.IOProvider.WriteDigitOutput(DEVICE_TYPE.DEVICE_TYPE_EXTRA_IO_DEVICE, 1, 1, true);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "SetCameraLight")
                        {
                            if (words[0].ToLower() == "true")
                            {
                                message = TMShellEditor.IOProvider.SetCameraLight(true);
                            }
                            else if (words[0].ToLower() == "false")
                            {
                                message = TMShellEditor.IOProvider.SetCameraLight(false);
                            }
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ReadDigitInput")
                        {
                            bool boolResultData = false;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMShellEditor.IOProvider.ReadDigitInput(type, int.Parse(words[1]), int.Parse(words[2]), out boolResultData);
                            txt_res_pay.Text = boolResultData.ToString();
                        }
                        else if (target == "ReadDigitOutput")
                        {
                            bool boolResultData = false;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMShellEditor.IOProvider.ReadDigitOutput(type, int.Parse(words[1]), int.Parse(words[2]), out boolResultData);
                            txt_res_pay.Text = boolResultData.ToString();
                        }
                        else if (target == "ReadAnalogInput")
                        {
                            float floatResultData = 0;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMShellEditor.IOProvider.ReadAnalogInput(type, int.Parse(words[1]), int.Parse(words[2]), out floatResultData);
                            txt_res_pay.Text = floatResultData.ToString();
                        }
                        else if (target == "ReadAnalogOutput")
                        {
                            float floatResultData = 0;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMShellEditor.IOProvider.ReadAnalogOutput(type, int.Parse(words[1]), int.Parse(words[2]), out floatResultData);
                            txt_res_pay.Text = floatResultData.ToString();
                        }
                        else if (target == "WriteAnalogOutput")
                        {
                            float floatResultData = 0;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMShellEditor.IOProvider.WriteAnalogOutput(type, int.Parse(words[1]), int.Parse(words[2]), float.Parse(words[3]));
                            txt_res_pay.Text = floatResultData.ToString();
                        }
                        break;
                    case "ProjectRunProvider":
                        try
                        {
                            if (target == "RunProject")
                            {   //0.01F
                                message = TMShellEditor.ProjectRunProvider.RunProject();
                                txt_res_pay.Text = "";
                            }
                            else if (target == "StopProject")
                            {
                                message = TMShellEditor.ProjectRunProvider.StopProject();
                                txt_res_pay.Text = "";
                            }
                            else if (target == "PauseProject")
                            {
                                message = TMShellEditor.ProjectRunProvider.PauseProject();
                                txt_res_pay.Text = "";
                            }
                            else if (target == "SetCurrentProject")
                            {
                                message = TMShellEditor.ProjectRunProvider.SetCurrentProject(words[0]);
                                txt_res_pay.Text = "";
                            }
                            else if (target == "GetDisplayBoardInfo")
                            {
                                string result = "";
                                message = TMShellEditor.ProjectRunProvider.GetDisplayBoardInfo(out result);
                                txt_res_pay.Text = result;
                            }
                            else if (target == "GetCurrentProject")
                            {
                                string data = "";
                                message = TMShellEditor.ProjectRunProvider.GetCurrentProject(out data);
                                txt_res_pay.Text = data;
                            }
                            else if (target == "GetProjectList")
                            {
                                List<string> data = new List<string>();
                                message = TMShellEditor.ProjectRunProvider.GetProjectList(out data);
                                StringBuilder stringBuilder = new StringBuilder();
                                foreach (string item in data) 
                                {
                                    stringBuilder.AppendLine(item);                                    
                                }
                                txt_res_pay.Text = stringBuilder.ToString();
                            }
                        }
                        catch
                        {
                            //message = TMError.InvalidParameter;
                        }
                        break;
                    case "RobotJogProvider":
                        try
                        {
                            if (target == "JogByJoint")
                            {   //0.01F
                                message = TMShellEditor.RobotJogProvider.JogByJoint(float.Parse(words[0]), new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) });
                                txt_res_pay.Text = "";
                            }
                            else if (target == "JogRelativeByTool")
                            {
                                message = TMShellEditor.RobotJogProvider.JogRelativeByTool(float.Parse(words[0]), new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) });
                                txt_res_pay.Text = "";
                            }
                            else if (target == "JogByBase")
                            {
                                message = TMShellEditor.RobotJogProvider.JogByBase(float.Parse(words[0]), new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) });
                                txt_res_pay.Text = "";
                            }
                            else if (target == "StopJog")
                            {
                                message = TMShellEditor.RobotJogProvider.StopJog();
                                txt_res_pay.Text = "";
                            }
                            else if (target == "KeepJogging")
                            {
                                message = TMShellEditor.RobotJogProvider.KeepJogging();
                                txt_res_pay.Text = "";
                            }
                            else if (target == "JogCircle")
                            {
                                message = TMShellEditor.RobotJogProvider.JogCircle(float.Parse(words[0]), new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) }
                                , new float[] { float.Parse(words[7]), float.Parse(words[8]), float.Parse(words[9]), float.Parse(words[10]), float.Parse(words[11]), float.Parse(words[12]) }, int.Parse(words[13]));                                
                                txt_res_pay.Text = "";
                            }
                            else if (target == "JogRelativeByJoint")
                            {
                                message = TMShellEditor.RobotJogProvider.JogRelativeByJoint(float.Parse(words[0]), new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) });
                                txt_res_pay.Text = "";
                            }
                            else if (target == "JogRelativeByBase")
                            {
                                message = TMShellEditor.RobotJogProvider.JogRelativeByBase(float.Parse(words[0]), new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) });
                                txt_res_pay.Text = "";
                            }
                            else if (target == "HoldPlayKeyToRun")
                            {
                                bool press = bool.Parse(words[0]);
                                message = TMShellEditor.RobotJogProvider.HoldPlayKeyToRun(press);
                                txt_res_pay.Text = "";
                            }
                        }
                        catch
                        {
                            //message = TMError.InvalidParameter;
                        }
                        break;
                    case "RobotStatusProvider":
                        if (target == "GetCurrentPoseByRobotBase")
                        {
                            float[] value = new float[] { };
                            message = TMShellEditor.RobotStatusProvider.GetCurrentPoseByRobotBase(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetCurrentPoseByCurrentBase")
                        {
                            float[] value = new float[] { };
                            message = TMShellEditor.RobotStatusProvider.GetCurrentPoseByCurrentBase(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetCurrentPoseByJointAngle")
                        {
                            float[] value = new float[] { };
                            message = TMShellEditor.RobotStatusProvider.GetCurrentPoseByJointAngle(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetCurrentRobotConfigs")
                        {
                            int[] value = new int[] { };
                            message = TMShellEditor.RobotStatusProvider.GetCurrentRobotConfigs(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "SetCurrentBase")
                        {
                            message = TMShellEditor.RobotStatusProvider.SetCurrentBase(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetCurrentBaseName")
                        {
                            string value = "";
                            message = TMShellEditor.RobotStatusProvider.GetCurrentBaseName(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetCurrentTcp")
                        {
                            string value = "";
                            message = TMShellEditor.RobotStatusProvider.GetCurrentTcp(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "SetCurrentTcp")
                        {
                            message = TMShellEditor.RobotStatusProvider.SetCurrentTcp(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "SetCurrentPayload")
                        {
                            message = TMShellEditor.RobotStatusProvider.SetCurrentPayload(float.Parse(words[0]));
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetCurrentPayload")
                        {
                            float value = 0;
                            message = TMShellEditor.RobotStatusProvider.GetCurrentPayload(out value);
                            txt_res_pay.Text = value.ToString();
                        }
                        else if (target == "GetRobotModelType")
                        {
                            string data = "";
                            message = TMShellEditor.RobotStatusProvider.GetRobotModelType(out data);
                            txt_res_pay.Text = data;
                        }
                        else if (target == "GetFlowVersion")
                        {
                            string data = "";
                            message = TMShellEditor.RobotStatusProvider.GetFlowVersion(out data);
                            txt_res_pay.Text = data;
                        }
                        else if (target == "RobotErrorOrNot")
                        {
                            bool data = false;
                            message = TMShellEditor.RobotStatusProvider.RobotErrorOrNot(out data);
                            txt_res_pay.Text = data.ToString();
                        }
                        else if (target == "RobotEstopOrNot")
                        {
                            bool data = false;
                            message = TMShellEditor.RobotStatusProvider.RobotEstopOrNot(out data);
                            txt_res_pay.Text = data.ToString();
                        }
                        else if (target == "ProjectPauseOrNot")
                        {
                            bool data = false;
                            message = TMShellEditor.RobotStatusProvider.ProjectPauseOrNot(out data);
                            txt_res_pay.Text = data.ToString();
                        }
                        else if (target == "ProjectRunOrNot")
                        {
                            bool data = false;
                            message = TMShellEditor.RobotStatusProvider.ProjectRunOrNot(out data);
                            txt_res_pay.Text = data.ToString();
                        }
                        else if (target == "ProjectEditOrNot")
                        {
                            bool data = false;
                            message = TMShellEditor.RobotStatusProvider.ProjectEditOrNot(out data);
                            txt_res_pay.Text = data.ToString();
                        }
                        else if (target == "GetOperationMode")
                        {
                            int data = 0;
                            message = TMShellEditor.RobotStatusProvider.GetOperationMode(out data);
                            txt_res_pay.Text = data.ToString();
                        }
                        else if (target == "GetCurrentSpeedPercentage")
                        {
                            int data = 0;
                            message = TMShellEditor.RobotStatusProvider.GetCurrentSpeedPercentage(out data);
                            txt_res_pay.Text = data.ToString();
                        }
                        else if (target == "GetCurrentToolSpeed")
                        {
                            string data = "";
                            message = TMShellEditor.RobotStatusProvider.GetCurrentToolSpeed(out data);
                            txt_res_pay.Text = data;
                        }
                        else if (target == "GetRobotName")
                        {
                            string data = "";
                            message = TMShellEditor.RobotStatusProvider.GetRobotName(out data);
                            txt_res_pay.Text = data;
                        }
                        break;                        
                    case "RobotStickProvider":
                        if (target == "RobotVirtualStickKeyEvent")
                        {
                            words[0] = words[0].Replace("VirtualKeyEvent.", "");
                            VirtualKeyEvent type = (VirtualKeyEvent)Enum.Parse(typeof(VirtualKeyEvent), words[0]);
                            message = TMShellEditor.RobotStickProvider.RobotVirtualStickKeyEvent(type);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "Example:ChangeMA")
                        {
                            message = TMShellEditor.RobotStickProvider.RobotVirtualStickKeyEvent(VirtualKeyEvent.MALongKey);
                            message = TMShellEditor.RobotStickProvider.RobotVirtualStickKeyEvent(VirtualKeyEvent.PlusKey);
                            message = TMShellEditor.RobotStickProvider.RobotVirtualStickKeyEvent(VirtualKeyEvent.MinusKey);
                            message = TMShellEditor.RobotStickProvider.RobotVirtualStickKeyEvent(VirtualKeyEvent.PlusKey);
                            message = TMShellEditor.RobotStickProvider.RobotVirtualStickKeyEvent(VirtualKeyEvent.PlusKey);
                            message = TMShellEditor.RobotStickProvider.RobotVirtualStickKeyEvent(VirtualKeyEvent.MinusKey);
                            message = TMShellEditor.RobotStickProvider.RobotVirtualStickKeyEvent(VirtualKeyEvent.MAKey);
                            message = TMShellEditor.RobotStickProvider.RobotVirtualStickKeyEvent(VirtualKeyEvent.MAKey);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "RobotStickStatus")
                        {
                            bool fgLocalControl = false;
                            message = TMShellEditor.RobotStickProvider.RobotStickStatus(out fgLocalControl);
                            txt_res_pay.Text = fgLocalControl.ToString();
                        }
                            break;
                    case "SystemProvider":
                        if (target == "GetCurrentLanguageCulture")
                        {
                            string language = "";
                            message = TMShellEditor.SystemProvider.GetCurrentLanguageCulture(out language);
                            txt_res_pay.Text = JsonConvert.SerializeObject(language);
                        }
                        else if (target == "GetTMflowType")
                        {
                            TMflowType type = TMflowType.Unknown;
                            message = TMShellEditor.SystemProvider.GetTMflowType(out type);
                            txt_res_pay.Text = type.ToString();
                        }
                        else if (target == "LogIn")
                        {
                            message = TMShellEditor.SystemProvider.LogIn(words[0], words[1]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "LogOut")
                        {
                            message = TMShellEditor.SystemProvider.LogOut();
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetControl")
                        {
                            bool fgGet = true;
                            if (words.Length > 0)
                            {
                                if (!bool.TryParse(words[0], out fgGet))
                                {
                                    fgGet = true;
                                }
                            }
                            message = TMShellEditor.SystemProvider.GetControl(fgGet);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "Shutdown")
                        {
                            message = TMShellEditor.SystemProvider.Shutdown();
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ShowTMflow")
                        {
                            message = TMShellEditor.SystemProvider.ShowTMflow();
                            txt_res_pay.Text = "";
                        }                        
                        else if (target == "ImportProject")
                        {
                            message = TMShellEditor.SystemProvider.ImportProject(words[0], words[1]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ImportGlobalVariable")
                        {
                            message = TMShellEditor.SystemProvider.ImportGlobalVariable(words[0], words[1]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ImportTCP")
                        {
                            message = TMShellEditor.SystemProvider.ImportTCP(words[0], words[1]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ExportProject")
                        {
                            message = TMShellEditor.SystemProvider.ExportProject(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ExportGlobalVariable")
                        {
                            message = TMShellEditor.SystemProvider.ExportGlobalVariable(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ExportTCP")
                        {
                            message = TMShellEditor.SystemProvider.ExportTCP(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ExportLog")
                        {
                            message = TMShellEditor.SystemProvider.ExportLog(LogExportSetting.Today);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "SetDateTime")
                        {
                            //DateTime test = DateTime.Now;
                            DateTime test = new DateTime(int.Parse(words[0]), int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4]), int.Parse(words[5]));
                            message = TMShellEditor.SystemProvider.SetDateTime(test);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetDateTime")
                        {
                            DateTimeOffset dateTimeOffset;
                            message = TMShellEditor.SystemProvider.GetDateTime(out dateTimeOffset);
                            txt_res_pay.Text = dateTimeOffset.ToString();
                        }
                        else if (target == "GetTimeZone")
                        {
                            string ID = "";
                            bool IsAutoAdjustDST = false;
                            message = TMShellEditor.SystemProvider.GetTimeZone(out ID, out IsAutoAdjustDST);
                            txt_res_pay.Text = "ID = "+ ID + Environment.NewLine + "IsAutoAdjustDST = " + IsAutoAdjustDST;
                        }
                        else if (target == "SetTimeZone")
                        {
                            string ID = words[0];
                            bool IsAutoAdjustDST = bool.Parse(words[1]);
                            message = TMShellEditor.SystemProvider.SetTimeZone(ID, IsAutoAdjustDST);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetSupportTimeZoneList")
                        {
                            List<TimeZoneInfo> timeZoneInfos = new List<TimeZoneInfo>();
                            message = TMShellEditor.SystemProvider.GetSupportTimeZoneList(out timeZoneInfos);
                            StringBuilder test = new StringBuilder();
                            foreach(var timeZoneInfo in timeZoneInfos)
                            {
                                test.AppendLine(timeZoneInfo.Id);
                            }
                            txt_res_pay.Text = test.ToString();
                        }
                        break;
                    case "TCPProvider":
                        if (target == "GetTcpList")
                        {
                            List<TCPInfo> list = new List<TCPInfo>();
                            message = TMShellEditor.TCPProvider.GetTcpList(out list);
                            txt_res_pay.Text = JsonConvert.SerializeObject(list);
                        }
                        else if (target == "GetProjectVisionTCPList")
                        {
                            List<string> list = new List<string>();
                            message = TMShellEditor.TCPProvider.GetProjectVisionTCPList(out list);
                            txt_res_pay.Text = JsonConvert.SerializeObject(list);
                        }
                        else if (target == "ChangeTcpPose")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            message = TMShellEditor.TCPProvider.ChangeTcpPose(words[0], value);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "CreateNewTcp")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            TCPInfo pp = new TCPInfo() { name = words[0], data = value };
                            message = TMShellEditor.TCPProvider.CreateNewTcp(pp);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetTcpMass")
                        {
                            float value;
                            message = TMShellEditor.TCPProvider.GetTcpMass(words[0], out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetTcpMassCenter")
                        {
                            float[] value = new float[] { };
                            message = TMShellEditor.TCPProvider.GetTcpMassCenter(words[0], out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "ChangeTcpMass")
                        {
                            message = TMShellEditor.TCPProvider.ChangeTcpMass(words[0], float.Parse(words[1]));
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ChangeTcpMassCenter")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            message = TMShellEditor.TCPProvider.ChangeTcpMassCenter(words[0], value);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetTcpInertia")
                        {
                            float[] value = new float[] { };
                            message = TMShellEditor.TCPProvider.GetTcpInertia(words[0], out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "ChangeTcpInertia")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]) };
                            message = TMShellEditor.TCPProvider.ChangeTcpInertia(words[0], value);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "DeleteTcp")
                        {
                            message = TMShellEditor.TCPProvider.DeleteTcp(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "IsTcpExist")
                        {
                            message = 0;
                            txt_res_pay.Text = TMShellEditor.TCPProvider.IsTcpExist(words[0]).ToString();
                        }
                        break;
                    case "VariableProvider":
                        {
                            if (target == "GetGlobalVariableList")
                            {
                                List<VariableInfo> variableInfos = new List<VariableInfo>();
                                message = TMShellEditor.VariableProvider.GetGlobalVariableList(ref variableInfos);
                                txt_res_pay.Text = JsonConvert.SerializeObject(variableInfos);
                            }
                            else if (target == "CreateGlobalVariable")
                            {
                                if (words.Length > 3)
                                {
                                    string content = Input1Data;
                                    int start = content.IndexOf('{');
                                    int end = content.IndexOf('}');
                                    if (start != -1 && end != -1 && start < end)
                                    {
                                        words[2] = content.Substring(start, end - start + 1);
                                    }
                                    else
                                    {
                                        //message = TMError.InvalidParameter;
                                    }
                                }
                                switch (words[1])
                                {
                                    case "VariableType.Boolean":
                                        message = TMShellEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Boolean, words[2]);
                                        break;
                                    case "VariableType.BooleanArray":
                                        message = TMShellEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.BooleanArray, words[2]);
                                        break;
                                    case "VariableType.Byte":
                                        message = TMShellEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Byte, words[2]);
                                        break;
                                    case "VariableType.ByteArray":
                                        message = TMShellEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.ByteArray, words[2]);
                                        break;
                                    case "VariableType.Double":
                                        message = TMShellEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Double, words[2]);
                                        break;
                                    case "VariableType.DoubleArray":
                                        message = TMShellEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.DoubleArray, words[2]);
                                        break;
                                    case "VariableType.Float":
                                        message = TMShellEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Float, words[2]);
                                        break;
                                    case "VariableType.FloatArray":
                                        message = TMShellEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.FloatArray, words[2]);
                                        break;
                                    case "VariableType.Integer":
                                        message = TMShellEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Integer, words[2]);
                                        break;
                                    case "VariableType.IntegrArray":
                                        message = TMShellEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.IntegrArray, words[2]);
                                        break;
                                    case "VariableType.String":
                                        message = TMShellEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.String, words[2]);
                                        break;
                                    case "VariableType.StringArray":
                                        message = TMShellEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.StringArray, words[2]);
                                        break;
                                }
                                txt_res_pay.Text = "";
                            }
                            else if (target == "ChangeGlobalVariableValue")
                            {
                                string content = Input1Data;
                                int start = content.IndexOf('{');
                                int end = content.IndexOf('}');
                                if (start != -1 && end != -1 && start < end)
                                {
                                    words[1] = content.Substring(start, end - start + 1);
                                }
                                List<string[]> value = new List<string[]>();
                                value.Add(new string[] { words[0].Trim(), words[1].Trim() });
                                message = TMShellEditor.VariableProvider.ChangeGlobalVariableValue(value);
                                txt_res_pay.Text = "";
                            }
                            else if (target == "DeleteGlobalVariable")
                            {
                                message = TMShellEditor.VariableProvider.DeleteGlobalVariable(Input1Data);
                                txt_res_pay.Text = "";
                            }
                            else if (target == "IsGlobalVariableExist")
                            {
                                message = 0;
                                txt_res_pay.Text = TMShellEditor.VariableProvider.IsGlobalVariableExist(Input1Data).ToString();
                            }
                            else if (target == "GetGlobalVariableValue")
                            {
                                message = 0;
                                //VariableInfo variableInfo = new VariableInfo();
                                string outdata = "";
                                message = TMShellEditor.VariableProvider.GetGlobalVariableValue(words[0], out outdata);
                                txt_res_pay.Text = outdata;// JsonConvert.SerializeObject(variableInfo);
                            }
                            else if (target == "GetVariableRuntimeValue")
                            {
                                string content = Input1.Text.Trim();
                                string value = "";
                                message = TMShellEditor.VariableProvider.GetVariableRuntimeValue(content, out value);
                                txt_res_pay.Text = value;
                            }
                            else if (target == "ChangeVariableRuntimeValue")
                            {
                                message = TMShellEditor.VariableProvider.ChangeVariableRuntimeValue(words[0].Trim(), words[1].Trim());
                            }
                        }
                        break;
                    case "FreeBotProvider":
                        if (target == "GetFreeBot")
                        {
                            FreeBotInfo freeBot;
                            message = TMShellEditor.FreeBotProvider.GetFreeBot(out freeBot);
                            txt_res_pay.Text = JsonConvert.SerializeObject(freeBot);
                        }
                        else if (target == "SetFreeBot")
                        {
                            FreeBotInfo freeBot = new FreeBotInfo();
                            freeBot.Mode = (FreeBotMode)int.Parse(words[0]);
                            freeBot.isBaseMode = bool.Parse(words[1]);
                            freeBot.isFreeX = bool.Parse(words[2]);
                            freeBot.isFreeY = bool.Parse(words[3]);
                            freeBot.isFreeZ = bool.Parse(words[4]);
                            freeBot.isFreeRX = bool.Parse(words[5]);
                            freeBot.isFreeRY = bool.Parse(words[6]);
                            freeBot.isFreeRZ = bool.Parse(words[7]);
                            freeBot.MoveMode = (MoveMode)int.Parse(words[8]);
                            message = TMShellEditor.FreeBotProvider.SetFreeBot(freeBot);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "HoldFreeBotKeyToHandGuide")
                        {
                            message = TMShellEditor.FreeBotProvider.HoldFreeBotKeyToHandGuide(bool.Parse(words[0]));
                            txt_res_pay.Text = "";
                        }
                        else if (target == "KeepFreeBot")
                        {
                            message = TMShellEditor.FreeBotProvider.KeepFreeBot();
                            txt_res_pay.Text = "";
                        }
                        break;
                    case "EndButtonEventProvider":
                        if (target == "SetEndButtonEventOwnership")
                        {
                            message = TMShellEditor.EndButtonEventProvider.SetEndButtonEventOwnership();
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ReleaseEndButtonEventOwnership")
                        {
                            message = TMShellEditor.EndButtonEventProvider.ReleaseEndButtonEventOwnership();
                            txt_res_pay.Text = "";
                        }
                        else if (target == "HasEndButtonEventOwnership")
                        {
                            message = 0;
                            txt_res_pay.Text = TMShellEditor.EndButtonEventProvider.HasEndButtonEventOwnership().ToString();
                        }
                        else if (target == "IsEndButtonBoardcastMode")
                        {
                            message = 0;
                            txt_res_pay.Text = TMShellEditor.EndButtonEventProvider.IsEndButtonBoardcastMode().ToString();
                        }
                        break;

                }
            }
            catch
            {
                message = (uint)TMcraftErr.ExceptionError;
                txt_res_pay.Text = "輸入錯誤";
            }
            txt_res_error.Text = message.ToString();
            if (message != 0)
            {
                lastError = message;
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            UserSaveData = txtScriptEnter.Text;
            fgSave = true;
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {            
            this.Close();
        }

        private void cb_Class_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                ComboBox test = sender as ComboBox;
                string className = test.SelectedItem.ToString();
                if (className == null)
                {
                    cb_command.Items.Clear();
                    return;
                }
                txt_res_error.Text = "";
                switch (className)
                {
                    case "TMcraftShell":
                        cb_command.Items.Clear();
                        cb_command.Items.Add("GetErrMsg");
                        break;
                    case "ScriptProjectProvider":
                        SetProjectProvider();
                        break;
                    case "BaseProvider":
                        SetBaseProvider();
                        break;
                    case "DataStorageProvider":
                        SetDataStorageProvider();
                        break;
                    case "IOProvider":
                        SetIOProvider();
                        break;
                    case "ProjectRunProvider":
                        SetProjectRunProvider();
                        break;
                    case "RobotJogProvider":
                        SetRobotJogProvider();
                        break;
                    case "RobotStatusProvider":
                        SetRobotStatusProvider();
                        break;
                    case "RobotStickProvider":
                        SetRobotStickProvider();
                        break;
                    case "SystemProvider":
                        SetSystemProvider();
                        break;
                    case "TCPProvider":
                        SetTCPProvider();
                        break;
                    case "VariableProvider":
                        SetVariableProvider();
                        break;
                    case "VisionProvider":
                        SetVisionProvider();
                        break;
                    case "FreeBotProvider":
                        SetFreeBotProvider();
                        break;
                    case "EndButtonEventProvider":
                        SetEndButtonEventProvider();
                        break;
                }
                cb_command.SelectedIndex = 0;
            }
        }
        private void SetProjectProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetScriptProjectList");
            cb_command.Items.Add("NewScriptProject");
            cb_command.Items.Add("OpenScriptProject");
            cb_command.Items.Add("SaveScriptProject");
            cb_command.Items.Add("WriteScriptProjectContent");
            cb_command.Items.Add("ReadScriptProjectContent");
            cb_command.Items.Add("ReadScriptProjectRemark");
            cb_command.Items.Add("WriteScriptProjectRemark");
            cb_command.Items.Add("DeleteScriptProject");
        }
        private void SetDataStorageProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetAllData");
            cb_command.Items.Add("SaveData(string)");
        }
        private void SetIOProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetAllIOData");
            cb_command.Items.Add("WriteDigitOutput");
            cb_command.Items.Add("SetCameraLight");
            cb_command.Items.Add("ReadDigitInput");
            cb_command.Items.Add("ReadDigitOutput");
            cb_command.Items.Add("ReadAnalogInput");
            cb_command.Items.Add("ReadAnalogOutput");
            cb_command.Items.Add("WriteAnalogOutput");
        }
        private void SetVariableProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetGlobalVariableList");
            cb_command.Items.Add("CreateGlobalVariable");
            cb_command.Items.Add("ChangeGlobalVariableValue");
            cb_command.Items.Add("DeleteGlobalVariable");
            cb_command.Items.Add("IsGlobalVariableExist");
            cb_command.Items.Add("GetGlobalVariableValue");
            cb_command.Items.Add("GetVariableRuntimeValue");
            cb_command.Items.Add("ChangeVariableRuntimeValue");
        }
        private void SetSystemProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetCurrentLanguageCulture");
            cb_command.Items.Add("GetTMflowType");            
            cb_command.Items.Add("GetControl");
            cb_command.Items.Add("LogIn");
            cb_command.Items.Add("LogOut");
            cb_command.Items.Add("Shutdown");
            cb_command.Items.Add("ShowTMflow");            
            cb_command.Items.Add("ImportProject");
            cb_command.Items.Add("ImportGlobalVariable");
            cb_command.Items.Add("ImportTCP");
            cb_command.Items.Add("ExportProject");
            cb_command.Items.Add("ExportGlobalVariable");
            cb_command.Items.Add("ExportTCP");
            cb_command.Items.Add("ExportLog");
            cb_command.Items.Add("SetDateTime");
            cb_command.Items.Add("GetDateTime");
            cb_command.Items.Add("GetTimeZone");
            cb_command.Items.Add("SetTimeZone");
            cb_command.Items.Add("GetSupportTimeZoneList");
        }
        private void SetBaseProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetBaseList");
            cb_command.Items.Add("CreateNewBase");
            cb_command.Items.Add("ChangeBaseValue");
            cb_command.Items.Add("DeleteBase");
            cb_command.Items.Add("IsBaseExist");
        }
        private void SetProjectRunProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("RunProject");
            cb_command.Items.Add("StopProject");
            cb_command.Items.Add("PauseProject");
            cb_command.Items.Add("SetCurrentProject");
            cb_command.Items.Add("GetDisplayBoardInfo");
            cb_command.Items.Add("GetCurrentProject");
            cb_command.Items.Add("GetProjectList");
        }
        private void SetRobotJogProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("JogByJoint");
            cb_command.Items.Add("JogRelativeByTool");
            cb_command.Items.Add("JogByBase");
            cb_command.Items.Add("StopJog");
            cb_command.Items.Add("KeepJogging");
            cb_command.Items.Add("JogCircle");
            cb_command.Items.Add("JogRelativeByJoint");
            cb_command.Items.Add("JogRelativeByBase");
            cb_command.Items.Add("HoldPlayKeyToRun");
        }
        private void SetTCPProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetTcpList");
            cb_command.Items.Add("GetProjectVisionTCPList");
            cb_command.Items.Add("ChangeTcpPose");
            cb_command.Items.Add("CreateNewTcp");
            cb_command.Items.Add("GetTcpMass");
            cb_command.Items.Add("ChangeTcpMass");
            cb_command.Items.Add("GetTcpInertia");
            cb_command.Items.Add("ChangeTcpInertia");
            cb_command.Items.Add("GetTcpMassCenter");
            cb_command.Items.Add("ChangeTcpMassCenter");
            cb_command.Items.Add("DeleteTcp");
            cb_command.Items.Add("IsTcpExist");
        }
        private void SetVisionProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetVisionJobList");
            cb_command.Items.Add("GetVisionJobInitialPoint");
            cb_command.Items.Add("GetVisionJobInitialBase");
            cb_command.Items.Add("CreateVisionJob");
            cb_command.Items.Add("DeleteVisionJob");
            cb_command.Items.Add("OpenVisionJob");
        }
        private void SetRobotStatusProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetRobotModelType");
            cb_command.Items.Add("GetFlowVersion");
            cb_command.Items.Add("RobotErrorOrNot");
            cb_command.Items.Add("RobotEstopOrNot");
            cb_command.Items.Add("ProjectPauseOrNot");
            cb_command.Items.Add("ProjectRunOrNot");
            cb_command.Items.Add("ProjectEditOrNot");
            cb_command.Items.Add("GetOperationMode");
            cb_command.Items.Add("GetCurrentPoseByRobotBase");
            cb_command.Items.Add("GetCurrentPoseByCurrentBase");
            cb_command.Items.Add("GetCurrentPoseByJointAngle");
            cb_command.Items.Add("GetCurrentRobotConfigs");
            cb_command.Items.Add("SetCurrentBase");
            cb_command.Items.Add("GetCurrentBaseName");
            cb_command.Items.Add("GetCurrentTcp");
            cb_command.Items.Add("SetCurrentTcp");
            cb_command.Items.Add("SetCurrentPayload");
            cb_command.Items.Add("GetCurrentPayload");
            cb_command.Items.Add("GetFreeBot");
            cb_command.Items.Add("SetFreeBot");
            cb_command.Items.Add("GetCurrentSpeedPercentage");
            cb_command.Items.Add("GetCurrentToolSpeed");
            cb_command.Items.Add("GetRobotName");
        }
        private void SetRobotStickProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("RobotVirtualStickKeyEvent");
            cb_command.Items.Add("Example:ChangeMA");
            cb_command.Items.Add("RobotStickStatus");
        }
        private void SetEndButtonEventProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("SetEndButtonEventOwnership");
            cb_command.Items.Add("ReleaseEndButtonEventOwnership");
            cb_command.Items.Add("IsEndButtonBoardcastMode");
            cb_command.Items.Add("HasEndButtonEventOwnership");
        }
        private void SetFreeBotProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetFreeBot");
            cb_command.Items.Add("SetFreeBot");
            cb_command.Items.Add("HoldFreeBotKeyToHandGuide");
            cb_command.Items.Add("KeepFreeBot");
        }
        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            txt_res_error.Text = "";
            txt_res_pay.Text = "";
        }

        private void cb_command_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string define = "";
            txtDes.Visibility = Visibility.Collapsed;
            foreach (var x in e.AddedItems)
            {
                define = (string)x;
            }
            switch (define)
            {
                case "GetErrMsg":
                    Input1.Text = lastError.ToString();
                    break;
                case "CreateNewBase":
                    Input1.Text = "base_name,0,0,0,0,0,0";
                    break;
                case "ChangeBaseValue":
                    Input1.Text = "base_name,0,0,0,0,0,0";
                    break;
                case "DeleteBase":
                    Input1.Text = "base_name";
                    break;
                case "IsBaseExist":
                    Input1.Text = "base_name";
                    break;
                case "SaveData(string)":
                    Input1.Text = "Key,Value";
                    break;
                case "WriteDigitOutput":
                    Input1.Text = "EXT_MODULE,1,1,true";
                    break;
                case "SetCameraLight":
                    Input1.Text = "true";
                    break;
                case "ReadDigitInput":
                    Input1.Text = "EXT_MODULE,1,1";
                    break;
                case "ReadDigitOutput":
                    Input1.Text = "EXT_MODULE,1,1";
                    break;
                case "ReadAnalogInput":
                    Input1.Text = "EXT_MODULE,2,1";
                    break;
                case "ReadAnalogOutput":
                    Input1.Text = "EXT_MODULE,2,1";
                    break;
                case "WriteAnalogOutput":
                    Input1.Text = "EXT_MODULE,2,1,2.1";
                    break;
                case "GetPointRobotConfigs":
                    Input1.Text = "pointName";
                    break;
                case "ChangePointRobotConfigs":
                    Input1.Text = "pointName,0,2,4";
                    break;
                case "CreatePointByFlangeCoordinates":
                    Input1.Text = "pointName,343.34433,-337.1829,622.2805,106.77074,-7.74514,64.56954,0,2,4,RobotBase,NOTOOL";
                    break;
                case "CreatePointByToolCoordinates":
                    Input1.Text = "pointName,343.34433,-337.1829,622.2805,106.77074,-7.74514,64.56954,0,2,4,RobotBase,NOTOOL";
                    break;
                case "CreatePointByJointAngles":
                    Input1.Text = "pointName,20,30,40,50,60,30,RobotBase,NOTOOL";
                    break;
                case "ChangePointToolCoordinates":
                    Input1.Text = "pointName,0,0,0,0,0,0";
                    break;
                case "ChangePointBase":
                    Input1.Text = "pointName,base_name";
                    break;
                case "IsPointExist":
                    Input1.Text = "pointName";
                    break;
                case "CreateProjectVariable":
                    Input1.Text = "nodvar1,VariableType.Integer,123";
                    txtDes.Visibility = Visibility.Visible;
                    txtDes.Content = "e.g.," + Environment.NewLine + "intArray,VariableType.IntegrArray,{0,1,2,3}" + Environment.NewLine + @"nodvar1,VariableType.String,""Test""";
                    break;
                case "CreateGlobalVariable":
                    Input1.Text = "nodvar1,VariableType.Integer,123";
                    txtDes.Visibility = Visibility.Visible;
                    txtDes.Content = "e.g.," + Environment.NewLine + "intArray,VariableType.IntegrArray,{0,1,2,3}" + Environment.NewLine + @"nodvar1,VariableType.String,""Test""";
                    break;
                case "ChangeProjectVariableValue":
                    Input1.Text = "var_name,value";
                    break;
                case "ChangeGlobalVariableValue":
                case "ChangeVariableRuntimeValue":
                    Input1.Text = "var_name,value";
                    break;
                case "DeleteProjectVariable":
                    Input1.Text = "var_name";
                    break;
                case "DeleteGlobalVariable":
                    Input1.Text = "var_name";
                    break;
                case "IsProjectVariableExist":
                case "IsGlobalVariableExist":
                    Input1.Text = "var_name";
                    break;
                case "ChangeTcpPose":
                    Input1.Text = "Tcp_name,0.0,0.0,0.0,0.0,0.0,0.0";
                    break;
                case "CreateNewTcp":
                    Input1.Text = "Tcp_name,0.0,0.0,0.0,0.0,0.0,0.0";
                    break;
                case "ChangeTcpMass":
                    Input1.Text = "Tcp_name,0.0";
                    break;
                case "ChangeTcpMassCenter":
                    Input1.Text = "Tcp_name,0.0,0.0,0.0,0.0,0.0,0.0";
                    break;
                case "GetTcpMass":
                    Input1.Text = "Tcp_name";
                    break;
                case "GetTcpInertia":
                    Input1.Text = "Tcp_name";
                    break;
                case "ChangeTcpInertia":
                    Input1.Text = "Tcp_name,0.0,0.0,0.0";
                    break;
                case "GetTcpMassCenter":
                    Input1.Text = "Tcp_name";
                    break;
                case "DeleteTcp":
                    Input1.Text = "Tcp_name";
                    break;
                case "IsTcpExist":
                    Input1.Text = "Tcp_name";
                    break;
                case "SetCurrentBase":
                    Input1.Text = "base_name";
                    break;
                case "SetCurrentTcp":
                    Input1.Text = "Tcp_name";
                    break;
                case "SetCurrentPayload":
                    Input1.Text = "0.0";
                    break;
                case "SetFreeBot":
                    Input1.Text = "0,False,False,False,False,False,False,False,1";
                    break;
                case "GetVisionJobInitialPoint":
                    Input1.Text = "visionJobName";
                    break;
                case "GetVisionJobInitialBase":
                    Input1.Text = "visionJobName";
                    break;
                case "CreateVisionJob":
                    Input1.Text = "visionJobName";
                    break;
                case "DeleteVisionJob":
                    Input1.Text = "visionJobName";
                    break;
                case "JogJoint":
                    Input1.Text = "3,0,0,0,0,0,0";
                    break;
                case "JogRelativeByTool":
                    Input1.Text = "5,0,-252,891,90,0,0";
                    break;
                case "JogByBase":
                    Input1.Text = "5,0,-252,891,90,0,0";
                    break;
                case "JogRelativeByBase":
                    Input1.Text = "5,0,0,0,0,0,0";
                    break;
                case "JogRelativeByJoint":
                    Input1.Text = "5,0,0,0,0,0,0";
                    break;
                case "OpenVisionJob":
                    Input1.Text = "visionJobName";
                    break;
                case "RobotVirtualStickKeyEvent":
                    Input1.Text = "VirtualKeyEvent.PlayKey";
                    break;
                case "ReadScriptProjectRemark":
                case "WriteScriptProjectRemark":
                    Input1.Text = "projectName";
                    break;
                case "GetControl":
                    Input1.Text = "true";
                    break;
                case "LogIn":
                    Input1.Text = "Administrator,";
                    break;
                case "GetOperationMode":
                    txtDes.Visibility = Visibility.Visible;
                    txtDes.Content = "0:Manual mode, 1:Auto mode";
                    break;
                case "ImportProject":
                    Input1.Text = "YourRobotName,projectName";
                    break;
                case "ImportGlobalVariable":
                    Input1.Text = "YourRobotName,varName";
                    break;
                case "ImportTCP":
                    Input1.Text = "YourRobotName,tcpName";
                    break;
                case "ExportProject":
                    Input1.Text = "projectName";
                    break;
                case "ExportGlobalVariable":
                    Input1.Text = "varName";
                    break;
                case "ExportTCP":
                    Input1.Text = "tcpName";
                    break;
                case "ExportLog":
                    Input1.Text = "";
                    break;
                case "SetDateTime":
                    DateTime test = DateTime.Now;
                    Input1.Text = string.Format("{0},{1},{2},{3},{4},{5}",test.Year, test.Month, test.Day, test.Hour, test.Minute, test.Second);
                    break;
                case "SetTimeZone":
                    Input1.Text = "US Eastern Standard Time,true";
                    break;
                case "HoldPlayKeyToRun":
                    Input1.Text = "true";
                    break;
                default:
                    Input1.Text = "";
                    break;
            }
        }

        private void btnTestBox_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Customized Node Test MessageBox");
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            TMShellEditor.EndButtonEventProvider.EndButtonClickEvent -= RobotStatusProvider_EndButtonClickEvent;
        }
        int testi = 0;
        private void RobotStatusProvider_EndButtonClickEvent(RobotEventType type, object data)
        {
            Dispatcher.BeginInvoke(
                               DispatcherPriority.Background,
                               new Action(delegate ()
                               {
                                   if (type == RobotEventType.EndButtonPointChanged && data.ToString() == "True") 
                                   {
                                       testi++;
                                       txtRobotEvent.Text = string.Format("{0}", testi);
                                   }
                                       
                               }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TMShellEditor != null) TMShellEditor.CloseShellConnection();
        }

        private void btn_Error_Click(object sender, RoutedEventArgs e)
        {
            string words = txt_res_error.Text.Trim();
            if (uint.TryParse(words, out uint myErrorNum))
            {
                string tmflowMessage = "";
                TMcraftErr mcraftErr = TMShellEditor.GetErrMsg(myErrorNum, out tmflowMessage);
                if (mcraftErr == TMcraftErr.OK)
                {
                    txt_res_pay.Text = tmflowMessage;
                }
                else
                {
                    txt_res_pay.Text = mcraftErr.ToString();
                }
            }
            else
            {
                txt_res_pay.Text = "";
            }
        }
    }
}
