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
using Newtonsoft.Json;
using System.Windows.Threading;

namespace TMcraftToolbarTestDll
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainPage : UserControl, ITMcraftToolbarEntry
    {
        public MainPage()
        {
            InitializeComponent();
        }
        TMcraftToolbarAPI TMNodeEditor;
        uint lastError = 0;
        public void InitializeToolbar(TMcraftToolbarAPI tMNodeEditor)
        {
            TMNodeEditor = tMNodeEditor;
            if (TMNodeEditor.EndButtonEventProvider != null)
            TMNodeEditor.EndButtonEventProvider.EndButtonClickEvent += EndButtonEventProvider_EndButtonClickEvent;
        }
        DispatcherTimer dispatcherTimerTick = new DispatcherTimer();
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cb_Class.Items.Clear();
            cb_Class.Items.Add("TMcraft");
            cb_Class.Items.Add("IOProvider");
            cb_Class.Items.Add("FreeBotProvider");
            cb_Class.Items.Add("RobotStatusProvider");            
            cb_Class.Items.Add("SystemProvider");
            cb_Class.Items.Add("TCPProvider");
            cb_Class.Items.Add("EndButtonEventProvider");
            cb_Class.Items.Add("VariableProvider");
            cb_Class.SelectedIndex = 0;
            txt_Enum.Text = "";
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string name in Enum.GetNames(typeof(IO_TYPE)))
            {
                stringBuilder.AppendLine("IO_TYPE." + name);
            }
            foreach (string name in Enum.GetNames(typeof(VariableType)))
            {
                stringBuilder.AppendLine("VariableType." + name);
            }
            txt_Enum.Text = stringBuilder.ToString();
            if (TMNodeEditor == null) { MessageBox.Show("TMNodeEditor == null"); }
            CNTitle.Content = string.Format("TMcraft Node Test Tool.(Version = {0})", TMcraftToolbarAPI.Version);
            dispatcherTimerTick.Tick += dispatcherTimer_Tick;
            dispatcherTimerTick.Interval = TimeSpan.FromMilliseconds(500);
        }
        private void EndButtonEventProvider_EndButtonClickEvent(RobotEventType type, object data)
        {
            Dispatcher.Invoke(
                               DispatcherPriority.Background,
                               new Action(delegate ()
                               {
                                   txtRobotEvent.Text = type.ToString() + " " + data.ToString();
                               }));
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                TMNodeEditor.FreeBotProvider.KeepFreeBot();
            }
            catch
            {

            }
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
            Input1.Text = Input1.Text.Replace(" ", string.Empty);
            string[] words = Input1.Text.Split(',');
            try
            {
                switch (className)
                {
                    case "TMcraft":
                        if (target == "GetErrMsg")
                        {
                            uint myErrorNum = 0;
                            if (uint.TryParse(words[0], out myErrorNum))
                            {
                                string tmflowMessage = "";
                                TMcraftErr mcraftErr = TMNodeEditor.GetErrMsg(myErrorNum, out tmflowMessage);
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
                    case "IOProvider":
                        List<DeviceIOInfo> ResultData = new List<DeviceIOInfo>();
                        if (target == "GetAllIOData")
                        {
                            message = TMNodeEditor.IOProvider.GetAllIOData(out ResultData);
                            txt_res_pay.Text = JsonConvert.SerializeObject(ResultData);
                        }
                        else if (target == "WriteDigitOutput")
                        {
                            words[0] = words[0].Replace("IO_TYPE.", "");
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMNodeEditor.IOProvider.WriteDigitOutput(type, int.Parse(words[1]), int.Parse(words[2]), bool.Parse(words[3]));
                            //message = TMNodeEditor.IOProvider.WriteDigitOutput(DEVICE_TYPE.DEVICE_TYPE_EXTRA_IO_DEVICE, 1, 1, true);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "SetCameraLight")
                        {
                            if (words[0].ToLower() == "true")
                            {
                                message = TMNodeEditor.IOProvider.SetCameraLight(true);
                            }
                            else if (words[0].ToLower() == "false")
                            {
                                message = TMNodeEditor.IOProvider.SetCameraLight(false);
                            }
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ReadDigitInput")
                        {
                            bool boolResultData = false;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMNodeEditor.IOProvider.ReadDigitInput(type, int.Parse(words[1]), int.Parse(words[2]), out boolResultData);
                            txt_res_pay.Text = boolResultData.ToString();
                        }
                        else if (target == "ReadDigitOutput")
                        {
                            bool boolResultData = false;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMNodeEditor.IOProvider.ReadDigitOutput(type, int.Parse(words[1]), int.Parse(words[2]), out boolResultData);
                            txt_res_pay.Text = boolResultData.ToString();
                        }
                        else if (target == "ReadAnalogInput")
                        {
                            float floatResultData = 0;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMNodeEditor.IOProvider.ReadAnalogInput(type, int.Parse(words[1]), int.Parse(words[2]), out floatResultData);
                            txt_res_pay.Text = floatResultData.ToString();
                        }
                        else if (target == "ReadAnalogOutput")
                        {
                            float floatResultData = 0;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMNodeEditor.IOProvider.ReadAnalogOutput(type, int.Parse(words[1]), int.Parse(words[2]), out floatResultData);
                            txt_res_pay.Text = floatResultData.ToString();
                        }
                        else if (target == "WriteAnalogOutput")
                        {
                            float floatResultData = 0;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMNodeEditor.IOProvider.WriteAnalogOutput(type, int.Parse(words[1]), int.Parse(words[2]), float.Parse(words[3]));
                            txt_res_pay.Text = floatResultData.ToString();
                        }
                        break;
                    case "RobotStatusProvider":
                        if (target == "GetCurrentPoseByRobotBase")
                        {
                            float[] value = new float[] { };
                            message = TMNodeEditor.RobotStatusProvider.GetCurrentPoseByRobotBase(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetCurrentPoseByCurrentBase")
                        {
                            float[] value = new float[] { };
                            message = TMNodeEditor.RobotStatusProvider.GetCurrentPoseByCurrentBase(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetCurrentPoseByJointAngle")
                        {
                            float[] value = new float[] { };
                            message = TMNodeEditor.RobotStatusProvider.GetCurrentPoseByJointAngle(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetCurrentRobotConfigs")
                        {
                            int[] value = new int[] { };
                            message = TMNodeEditor.RobotStatusProvider.GetCurrentRobotConfigs(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "SetCurrentBase")
                        {
                            message = TMNodeEditor.RobotStatusProvider.SetCurrentBase(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetCurrentBaseName")
                        {
                            string value = "";
                            message = TMNodeEditor.RobotStatusProvider.GetCurrentBaseName(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetCurrentTcp")
                        {
                            string value = "";
                            message = TMNodeEditor.RobotStatusProvider.GetCurrentTcp(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "SetCurrentTcp")
                        {
                            message = TMNodeEditor.RobotStatusProvider.SetCurrentTcp(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "SetCurrentPayload")
                        {
                            message = TMNodeEditor.RobotStatusProvider.SetCurrentPayload(float.Parse(words[0]));
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetCurrentPayload")
                        {
                            float value = 0;
                            message = TMNodeEditor.RobotStatusProvider.GetCurrentPayload(out value);
                            txt_res_pay.Text = value.ToString();
                        }
                        break;
                    case "FreeBotProvider":
                        if (target == "GetFreeBot")
                        {
                            FreeBotInfo freeBot;
                            message = TMNodeEditor.FreeBotProvider.GetFreeBot(out freeBot);
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
                            message = TMNodeEditor.FreeBotProvider.SetFreeBot(freeBot);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "HoldFreeBotKeyToHandGuide")
                        {
                            bool test = bool.Parse(words[0]);
                            if (test == true)
                            {
                                message = TMNodeEditor.FreeBotProvider.HoldFreeBotKeyToHandGuide(test);
                                txt_res_pay.Text = "";
                                dispatcherTimerTick.Start();
                            }
                            else if (test == false)
                            {
                                message = TMNodeEditor.FreeBotProvider.HoldFreeBotKeyToHandGuide(test);
                            txt_res_pay.Text = "";
                                dispatcherTimerTick.Stop();
                            }                            
                        }
                        break;
                    case "SystemProvider":
                        if (target == "GetCurrentLanguageCulture")
                        {
                            string language = "";
                            message = TMNodeEditor.SystemProvider.GetCurrentLanguageCulture(out language);
                            txt_res_pay.Text = JsonConvert.SerializeObject(language);
                        }
                        else if (target == "GetTMflowType")
                        {
                            TMflowType type = TMflowType.Unknown;
                            message = TMNodeEditor.SystemProvider.GetTMflowType(out type);
                            txt_res_pay.Text = type.ToString();
                        }
                        break;
                    case "TCPProvider":
                        if (target == "GetTcpList")
                        {
                            List<TCPInfo> list = new List<TCPInfo>();
                            message = TMNodeEditor.TCPProvider.GetTcpList(out list);
                            txt_res_pay.Text = JsonConvert.SerializeObject(list);
                        }
                        else if (target == "GetProjectVisionTCPList")
                        {
                            List<string> list = new List<string>();
                            message = TMNodeEditor.TCPProvider.GetProjectVisionTCPList(out list);
                            txt_res_pay.Text = JsonConvert.SerializeObject(list);
                        }
                        else if (target == "ChangeTcpPose")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            message = TMNodeEditor.TCPProvider.ChangeTcpPose(words[0], value);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "CreateNewTcp")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            TCPInfo pp = new TCPInfo() { name = words[0], data = value };
                            message = TMNodeEditor.TCPProvider.CreateNewTcp(pp);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetTcpMass")
                        {
                            float value;
                            message = TMNodeEditor.TCPProvider.GetTcpMass(words[0], out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetTcpMassCenter")
                        {
                            float[] value = new float[] { };
                            message = TMNodeEditor.TCPProvider.GetTcpMassCenter(words[0], out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "ChangeTcpMass")
                        {
                            message = TMNodeEditor.TCPProvider.ChangeTcpMass(words[0], float.Parse(words[1]));
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ChangeTcpMassCenter")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            message = TMNodeEditor.TCPProvider.ChangeTcpMassCenter(words[0], value);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetTcpInertia")
                        {
                            float[] value = new float[] { };
                            message = TMNodeEditor.TCPProvider.GetTcpInertia(words[0], out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "ChangeTcpInertia")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]) };
                            message = TMNodeEditor.TCPProvider.ChangeTcpInertia(words[0], value);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "DeleteTcp")
                        {
                            message = TMNodeEditor.TCPProvider.DeleteTcp(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "IsTcpExist")
                        {
                            message = 0;
                            txt_res_pay.Text = TMNodeEditor.TCPProvider.IsTcpExist(words[0]).ToString();
                        }
                        break;
                    case "EndButtonEventProvider":
                        {
                            if (target == "SetEndButtonEventOwnership")
                            {
                                List<VariableInfo> variableInfos = new List<VariableInfo>();
                                message = TMNodeEditor.EndButtonEventProvider.SetEndButtonEventOwnership();
                                txt_res_pay.Text = "";
                            }
                            else if (target == "ReleaseEndButtonEventOwnership")
                            {
                                List<VariableInfo> variableInfos = new List<VariableInfo>();
                                message = TMNodeEditor.EndButtonEventProvider.ReleaseEndButtonEventOwnership();
                                txt_res_pay.Text = "";
                            }
                            else if (target == "IsEndButtonBoardcastMode")
                            {
                                txt_res_pay.Text = TMNodeEditor.EndButtonEventProvider.IsEndButtonBoardcastMode().ToString();
                            }
                            else if (target == "HasEndButtonEventOwnership")
                            {
                                txt_res_pay.Text = TMNodeEditor.EndButtonEventProvider.HasEndButtonEventOwnership().ToString();
                            }
                        }
                        break;
                    case "VariableProvider":
                        {
                            if (target == "GetProjectVariableList")
                            {
                                List<VariableInfo> variableInfos = new List<VariableInfo>();
                                message = TMNodeEditor.VariableProvider.GetProjectVariableList(ref variableInfos);
                                txt_res_pay.Text = JsonConvert.SerializeObject(variableInfos);
                            }
                            if (target == "GetGlobalVariableList")
                            {
                                List<VariableInfo> variableInfos = new List<VariableInfo>();
                                message = TMNodeEditor.VariableProvider.GetGlobalVariableList(ref variableInfos);
                                txt_res_pay.Text = JsonConvert.SerializeObject(variableInfos);
                            }
                            else if (target == "CreateGlobalVariable")
                            {
                                if (words.Length > 3)
                                {
                                    string content = Input1.Text;
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
                                        message = TMNodeEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Boolean, words[2]);
                                        break;
                                    case "VariableType.BooleanArray":
                                        message = TMNodeEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.BooleanArray, words[2]);
                                        break;
                                    case "VariableType.Byte":
                                        message = TMNodeEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Byte, words[2]);
                                        break;
                                    case "VariableType.ByteArray":
                                        message = TMNodeEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.ByteArray, words[2]);
                                        break;
                                    case "VariableType.Double":
                                        message = TMNodeEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Double, words[2]);
                                        break;
                                    case "VariableType.DoubleArray":
                                        message = TMNodeEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.DoubleArray, words[2]);
                                        break;
                                    case "VariableType.Float":
                                        message = TMNodeEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Float, words[2]);
                                        break;
                                    case "VariableType.FloatArray":
                                        message = TMNodeEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.FloatArray, words[2]);
                                        break;
                                    case "VariableType.Integer":
                                        message = TMNodeEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Integer, words[2]);
                                        break;
                                    case "VariableType.IntegrArray":
                                        message = TMNodeEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.IntegrArray, words[2]);
                                        break;
                                    case "VariableType.String":
                                        message = TMNodeEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.String, words[2]);
                                        break;
                                    case "VariableType.StringArray":
                                        message = TMNodeEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.StringArray, words[2]);
                                        break;
                                }
                                txt_res_pay.Text = "";
                            }
                            else if (target == "ChangeProjectVariableValue")
                            {
                                string content = Input1.Text;
                                int start = content.IndexOf('{');
                                int end = content.IndexOf('}');
                                if (start != -1 && end != -1 && start < end)
                                {
                                    words[1] = content.Substring(start, end - start + 1);
                                }
                                List<string[]> value = new List<string[]>();
                                value.Add(new string[] { words[0].Trim(), words[1].Trim() });
                                message = TMNodeEditor.VariableProvider.ChangeProjectVariableValue(value);
                                txt_res_pay.Text = "";
                            }
                            else if (target == "ChangeGlobalVariableValue")
                            {
                                string content = Input1.Text;
                                int start = content.IndexOf('{');
                                int end = content.IndexOf('}');
                                if (start != -1 && end != -1 && start < end)
                                {
                                    words[1] = content.Substring(start, end - start + 1);
                                }
                                List<string[]> value = new List<string[]>();
                                value.Add(new string[] { words[0].Trim(), words[1].Trim() });
                                message = TMNodeEditor.VariableProvider.ChangeGlobalVariableValue(value);
                                txt_res_pay.Text = "";
                            }
                            //else if (target == "DeleteProjectVariable")
                            //{
                            //    message = TMNodeEditor.VariableProvider.DeleteProjectVariable(Input1.Text);
                            //    txt_res_pay.Text = "";
                            //}
                            else if (target == "DeleteGlobalVariable")
                            {
                                message = TMNodeEditor.VariableProvider.DeleteGlobalVariable(Input1.Text);
                                txt_res_pay.Text = "";
                            }
                            else if (target == "IsProjectVariableExist")
                            {
                                message = 0;
                                txt_res_pay.Text = TMNodeEditor.VariableProvider.IsProjectVariableExist(Input1.Text).ToString();
                            }
                            else if (target == "IsGlobalVariableExist")
                            {
                                message = 0;
                                txt_res_pay.Text = TMNodeEditor.VariableProvider.IsGlobalVariableExist(Input1.Text).ToString();
                            }
                            else if (target == "GetVariableRuntimeValue")
                            {
                                string content = Input1.Text.Trim();
                                string value = "";
                                message = TMNodeEditor.VariableProvider.GetVariableRuntimeValue(content, out value);
                                txt_res_pay.Text = value;
                            }
                            //else if (target == "ChangeVariableRuntimeValue")
                            //{
                            //    message = TMNodeEditor.VariableProvider.ChangeVariableRuntimeValue(words[0].Trim(), words[1].Trim());
                            //}
                        }
                        break;
                }
            }
            catch
            {
                message = (uint)TMcraftErr.ExceptionError;
                txt_res_pay.Text = "輸入?�誤";
            }
            txt_res_error.Text = message.ToString();
            if (message != 0)
            {
                lastError = message;
            }
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            if (TMNodeEditor != null) TMNodeEditor.Close();
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
                    Input1.Text = "var_name,value";
                    break;
                //case "DeleteProjectVariable":
                //    Input1.Text = "var_name";
                //    break;
                case "DeleteGlobalVariable":
                    Input1.Text = "var_name";
                    break;
                case "IsProjectVariableExist":
                case "IsGlobalVariableExist":
                case "GetProjectVariableRuntimeValue":
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
                case "JogLineByToolAxes":
                    Input1.Text = "5,0,-252,891,90,0,0";
                    break;
                case "JogLineByCoordinates":
                    Input1.Text = "5,0,-252,891,90,0,0";
                    break;
                case "OpenVisionJob":
                    Input1.Text = "visionJobName";
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
                    case "TMcraft":
                        cb_command.Items.Clear();
                        cb_command.Items.Add("GetErrMsg");
                        break;
                    case "IOProvider":
                        SetIOProvider();
                        break;
                    case "RobotStatusProvider":
                        SetRobotStatusProvider();
                        break;
                    case "FreeBotProvider":
                        SetRobotFreeBotProvider();
                        break;
                    case "SystemProvider":
                        SetSystemProvider();
                        break;
                    case "TCPProvider":
                        SetTCPProvider();
                        break;
                    case "EndButtonEventProvider":
                        SetEndButtonEventProvider();
                        break;
                    case "VariableProvider":
                        SetVariableProvider();
                        break;
                }
                cb_command.SelectedIndex = 0;
            }
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
        private void SetRobotStatusProvider()
        {
            cb_command.Items.Clear();
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
        }
        private void SetRobotFreeBotProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetFreeBot");
            cb_command.Items.Add("SetFreeBot");
            cb_command.Items.Add("HoldFreeBotKeyToHandGuide");
        }
        private void SetSystemProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetCurrentLanguageCulture");
            cb_command.Items.Add("GetTMflowType");
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
        private void SetEndButtonEventProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("SetEndButtonEventOwnership");
            cb_command.Items.Add("ReleaseEndButtonEventOwnership");
            cb_command.Items.Add("IsEndButtonBoardcastMode");
            cb_command.Items.Add("HasEndButtonEventOwnership");
        }
        private void SetVariableProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetProjectVariableList");
            cb_command.Items.Add("GetGlobalVariableList");
            cb_command.Items.Add("CreateProjectVariable");
            cb_command.Items.Add("CreateGlobalVariable");
            cb_command.Items.Add("ChangeProjectVariableValue");
            cb_command.Items.Add("ChangeGlobalVariableValue");
            //cb_command.Items.Add("DeleteProjectVariable");
            cb_command.Items.Add("DeleteGlobalVariable");
            cb_command.Items.Add("IsProjectVariableExist");
            cb_command.Items.Add("IsGlobalVariableExist");
            cb_command.Items.Add("GetVariableRuntimeValue");
        }

        private void btn_Error_Click(object sender, RoutedEventArgs e)
        {
            string words = txt_res_error.Text.Trim();
            if (uint.TryParse(words, out uint myErrorNum))
            {
                string tmflowMessage = "";
                TMcraftErr mcraftErr = TMNodeEditor.GetErrMsg(myErrorNum, out tmflowMessage);
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