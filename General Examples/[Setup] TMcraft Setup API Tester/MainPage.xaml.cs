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

namespace TMcraftSetupTestDll
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainPage : UserControl, ITMcraftSetupEntry
    {        
        TMcraftSetupAPI TMSetupEditor;        
        public void InitializeSetup(TMcraftSetupAPI tMNodeEditor)
        {
            TMSetupEditor = tMNodeEditor;
            if (TMSetupEditor.EndButtonEventProvider != null)
                TMSetupEditor.EndButtonEventProvider.EndButtonClickEvent += EndButtonEventProvider_EndButtonClickEvent;
        }
        public MainPage()
        {
            InitializeComponent();
        }
        uint lastError = 0;
        DispatcherTimer dispatcherTimerTick = new DispatcherTimer();
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cb_Class.Items.Clear();
            cb_Class.Items.Add("TMcraft");
            cb_Class.Items.Add("BaseProvider");
            cb_Class.Items.Add("IOProvider");
            cb_Class.Items.Add("PointProvider");
            cb_Class.Items.Add("RobotStatusProvider");
            cb_Class.Items.Add("ScriptWriteProvider");
            cb_Class.Items.Add("SystemProvider");
            cb_Class.Items.Add("TCPProvider");
            cb_Class.Items.Add("TextFileProvider");
            cb_Class.Items.Add("VariableProvider");
            cb_Class.Items.Add("FreeBotProvider");
            cb_Class.Items.Add("EndButtonEventProvider");
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
            if (TMSetupEditor == null) { MessageBox.Show("TMNodeEditor == null"); }
            CNTitle.Content = string.Format("TMcraft Setup Test Tool.(Version = {0})", TMcraftSetupAPI.Version);
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
                TMSetupEditor.FreeBotProvider.KeepFreeBot();
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
                                TMcraftErr mcraftErr = TMSetupEditor.GetErrMsg(myErrorNum, out tmflowMessage);
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
                    case "BaseProvider":
                        if (target == "GetBaseList")
                        {
                            List<BaseInfo> list = new List<BaseInfo>();
                            message = TMSetupEditor.BaseProvider.GetBaseList(ref list);
                            txt_res_pay.Text = JsonConvert.SerializeObject(list);
                        }
                        else if (target == "CreateNewBase")
                        {
                            //"Cbase1", new float[] { 11, 22, 0, 0, 0, 0 }
                            message = TMSetupEditor.BaseProvider.CreateNewBase(words[0], new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) });
                        }
                        else if (target == "ChangeBaseValue")
                        {
                            message = TMSetupEditor.BaseProvider.ChangeBaseValue(words[0], new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) });
                        }
                        else if (target == "DeleteBase")
                        {
                            message = TMSetupEditor.BaseProvider.DeleteBase(words[0]);
                        }
                        else if (target == "IsBaseExist")
                        {
                            message = 0;
                            txt_res_pay.Text = TMSetupEditor.BaseProvider.IsBaseExist(words[0]).ToString();
                        }
                        break;
                    case "IOProvider":
                        List<DeviceIOInfo> ResultData = new List<DeviceIOInfo>();
                        if (target == "GetAllIOData")
                        {
                            message = TMSetupEditor.IOProvider.GetAllIOData(out ResultData);
                            txt_res_pay.Text = JsonConvert.SerializeObject(ResultData);
                        }
                        else if (target == "WriteDigitOutput")
                        {
                            words[0] = words[0].Replace("IO_TYPE.", "");
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMSetupEditor.IOProvider.WriteDigitOutput(type, int.Parse(words[1]), int.Parse(words[2]), bool.Parse(words[3]));
                            //message = TMNodeEditor.IOProvider.WriteDigitOutput(DEVICE_TYPE.DEVICE_TYPE_EXTRA_IO_DEVICE, 1, 1, true);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "SetCameraLight")
                        {
                            if (words[0].ToLower() == "true")
                            {
                                message = TMSetupEditor.IOProvider.SetCameraLight(true);
                            }
                            else if (words[0].ToLower() == "false")
                            {
                                message = TMSetupEditor.IOProvider.SetCameraLight(false);
                            }
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ReadDigitInput")
                        {
                            bool boolResultData = false;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMSetupEditor.IOProvider.ReadDigitInput(type, int.Parse(words[1]), int.Parse(words[2]), out boolResultData);
                            txt_res_pay.Text = boolResultData.ToString();
                        }
                        else if (target == "ReadDigitOutput")
                        {
                            bool boolResultData = false;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMSetupEditor.IOProvider.ReadDigitOutput(type, int.Parse(words[1]), int.Parse(words[2]), out boolResultData);
                            txt_res_pay.Text = boolResultData.ToString();
                        }
                        else if (target == "ReadAnalogInput")
                        {
                            float floatResultData = 0;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMSetupEditor.IOProvider.ReadAnalogInput(type, int.Parse(words[1]), int.Parse(words[2]), out floatResultData);
                            txt_res_pay.Text = floatResultData.ToString();
                        }
                        else if (target == "ReadAnalogOutput")
                        {
                            float floatResultData = 0;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMSetupEditor.IOProvider.ReadAnalogOutput(type, int.Parse(words[1]), int.Parse(words[2]), out floatResultData);
                            txt_res_pay.Text = floatResultData.ToString();
                        }
                        else if (target == "WriteAnalogOutput")
                        {
                            float floatResultData = 0;
                            IO_TYPE type = (IO_TYPE)Enum.Parse(typeof(IO_TYPE), words[0]);
                            message = TMSetupEditor.IOProvider.WriteAnalogOutput(type, int.Parse(words[1]), int.Parse(words[2]), float.Parse(words[3]));
                            txt_res_pay.Text = floatResultData.ToString();
                        }
                        break;
                    case "PointProvider":
                        if (target == "GetPointList")
                        {
                            List<PointInfo> list = new List<PointInfo>();
                            message = TMSetupEditor.PointProvider.GetPointList(ref list);
                            txt_res_pay.Text = JsonConvert.SerializeObject(list);
                        }
                        else if (target == "GetPointRobotConfigs")
                        {
                            int[] config = new int[] { };
                            message = TMSetupEditor.PointProvider.GetPointRobotConfigs(words[0], ref config);
                            txt_res_pay.Text = JsonConvert.SerializeObject(config);
                        }
                        else if (target == "ChangePointRobotConfigs")
                        {
                            int[] config = new int[] { int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]) };
                            message = TMSetupEditor.PointProvider.ChangePointRobotConfigs(words[0], config);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "CreatePointByFlangeCoordinates")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            int[] configs = new int[] { int.Parse(words[7]), int.Parse(words[8]), int.Parse(words[9]) };
                            message = TMSetupEditor.PointProvider.CreatePointByFlangeCoordinates(words[0],
                                value, configs, words[10], words[11]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "CreatePointByToolCoordinates")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            int[] configs = new int[] { int.Parse(words[7]), int.Parse(words[8]), int.Parse(words[9]) };
                            message = TMSetupEditor.PointProvider.CreatePointByToolCoordinates(words[0], value, configs, words[10], words[11]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "CreatePointByJointAngles")
                        {
                            //float[] joint = new float[] { 10, 0, 2, 0, 0, 3 };
                            float[] joint = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            //message = TMNodeEditor.PointProvider.CreatePointByJointAngles("cpoint1", joint, "RobotBase", "NOTOOL");
                            message = TMSetupEditor.PointProvider.CreatePointByJointAngles(words[0], joint, words[7], words[8]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ChangePointToolCoordinates")
                        {
                            float[] tool = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            message = TMSetupEditor.PointProvider.ChangePointToolCoordinates(words[0], tool);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ChangePointBase")
                        {
                            message = TMSetupEditor.PointProvider.ChangePointBase(words[0], words[1]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "IsPointExist")
                        {
                            message = 0;
                            txt_res_pay.Text = TMSetupEditor.PointProvider.IsPointExist(words[0]).ToString();
                        }
                        break;
                    case "RobotStatusProvider":
                        if (target == "GetCurrentPoseByRobotBase")
                        {
                            float[] value = new float[] { };
                            message = TMSetupEditor.RobotStatusProvider.GetCurrentPoseByRobotBase(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetCurrentPoseByCurrentBase")
                        {
                            float[] value = new float[] { };
                            message = TMSetupEditor.RobotStatusProvider.GetCurrentPoseByCurrentBase(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetCurrentPoseByJointAngle")
                        {
                            float[] value = new float[] { };
                            message = TMSetupEditor.RobotStatusProvider.GetCurrentPoseByJointAngle(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetCurrentRobotConfigs")
                        {
                            int[] value = new int[] { };
                            message = TMSetupEditor.RobotStatusProvider.GetCurrentRobotConfigs(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "SetCurrentBase")
                        {
                            message = TMSetupEditor.RobotStatusProvider.SetCurrentBase(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetCurrentBaseName")
                        {
                            string value = "";
                            message = TMSetupEditor.RobotStatusProvider.GetCurrentBaseName(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetCurrentTcp")
                        {
                            string value = "";
                            message = TMSetupEditor.RobotStatusProvider.GetCurrentTcp(out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "SetCurrentTcp")
                        {
                            message = TMSetupEditor.RobotStatusProvider.SetCurrentTcp(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "SetCurrentPayload")
                        {
                            message = TMSetupEditor.RobotStatusProvider.SetCurrentPayload(float.Parse(words[0]));
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetCurrentPayload")
                        {
                            float value = 0;
                            message = TMSetupEditor.RobotStatusProvider.GetCurrentPayload(out value);
                            txt_res_pay.Text = value.ToString();
                        }
                        break;
                    case "ScriptWriteProvider":
                        if (target == "AppendLineToBuffer")
                        {
                            string language = txtScriptEnter.Text.Trim();
                            TMSetupEditor.ScriptWriteProvider.AppendLineToBuffer(language);
                            message = 0;
                        }
                        else if (target == "AppendScriptToBuffer")
                        {
                            string language = txtScriptEnter.Text.Trim();
                            TMSetupEditor.ScriptWriteProvider.AppendScriptToBuffer(language);
                            message = 0;
                        }
                        else if (target == "GetScriptBuffer")
                        {
                            string language = "";
                            language = TMSetupEditor.ScriptWriteProvider.GetScriptBuffer();
                            txt_res_pay.Text = language;
                            message = 0;
                        }
                        else if (target == "SaveBufferAsScript")
                        {
                            message = TMSetupEditor.ScriptWriteProvider.SaveBufferAsScript();
                        }
                        else if (target == "GetScript")
                        {
                            string language = "";
                            message = TMSetupEditor.ScriptWriteProvider.GetScript(out language);
                            if (message == 0)
                            {
                                txt_res_pay.Text = language;
                            }
                        }
                        break;
                    case "SystemProvider":
                        if (target == "GetCurrentLanguageCulture")
                        {
                            string language = "";
                            message = TMSetupEditor.SystemProvider.GetCurrentLanguageCulture(out language);
                            txt_res_pay.Text = JsonConvert.SerializeObject(language);
                        }
                        else if (target == "GetTMflowType")
                        {
                            TMflowType type = TMflowType.Unknown;
                            message = TMSetupEditor.SystemProvider.GetTMflowType(out type);
                            txt_res_pay.Text = type.ToString();
                        }
                        break;
                    case "TCPProvider":
                        if (target == "GetTcpList")
                        {
                            List<TCPInfo> list = new List<TCPInfo>();
                            message = TMSetupEditor.TCPProvider.GetTcpList(out list);
                            txt_res_pay.Text = JsonConvert.SerializeObject(list);
                        }
                        else if (target == "GetProjectVisionTCPList")
                        {
                            List<string> list = new List<string>();
                            message = TMSetupEditor.TCPProvider.GetProjectVisionTCPList(out list);
                            txt_res_pay.Text = JsonConvert.SerializeObject(list);
                        }
                        else if (target == "ChangeTcpPose")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            message = TMSetupEditor.TCPProvider.ChangeTcpPose(words[0], value);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "CreateNewTcp")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            TCPInfo pp = new TCPInfo() { name = words[0], data = value };
                            message = TMSetupEditor.TCPProvider.CreateNewTcp(pp);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetTcpMass")
                        {
                            float value;
                            message = TMSetupEditor.TCPProvider.GetTcpMass(words[0], out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetTcpMassCenter")
                        {
                            float[] value = new float[] { };
                            message = TMSetupEditor.TCPProvider.GetTcpMassCenter(words[0], out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "ChangeTcpMass")
                        {
                            message = TMSetupEditor.TCPProvider.ChangeTcpMass(words[0], float.Parse(words[1]));
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ChangeTcpMassCenter")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            message = TMSetupEditor.TCPProvider.ChangeTcpMassCenter(words[0], value);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetTcpInertia")
                        {
                            float[] value = new float[] { };
                            message = TMSetupEditor.TCPProvider.GetTcpInertia(words[0], out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "ChangeTcpInertia")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]) };
                            message = TMSetupEditor.TCPProvider.ChangeTcpInertia(words[0], value);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "DeleteTcp")
                        {
                            message = TMSetupEditor.TCPProvider.DeleteTcp(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "IsTcpExist")
                        {
                            message = 0;
                            txt_res_pay.Text = TMSetupEditor.TCPProvider.IsTcpExist(words[0]).ToString();
                        }
                        break;
                    case "TextFileProvider":
                        if (target == "GetTextFileList")
                        {
                            string[] lst;
                            message = TMSetupEditor.TextFileProvider.GetTextFileList(out lst);
                            StringBuilder stringBuilder = new StringBuilder();
                            foreach (string x in lst)
                            {
                                stringBuilder.AppendLine(x);
                            }
                            txt_res_pay.Text = stringBuilder.ToString();
                        }
                        else if (target == "NewTextFile")
                        {
                            message = TMSetupEditor.TextFileProvider.NewTextFile(words[0], txtScriptEnter.Text);
                        }
                        else if (target == "ReadTextFile")
                        {
                            string fileContent = "";
                            message = TMSetupEditor.TextFileProvider.ReadTextFile(words[0], out fileContent);
                            txt_res_pay.Text = fileContent;
                        }
                        else if (target == "WriteTextFile")
                        {
                            message = TMSetupEditor.TextFileProvider.WriteTextFile(words[0], txtScriptEnter.Text);
                        }
                        else if (target == "DeleteTextFile")
                        {
                            message = TMSetupEditor.TextFileProvider.DeleteTextFile(words[0]);
                        }
                        else if (target == "ImportTextFile")
                        {
                            message = TMSetupEditor.TextFileProvider.ImportTextFile(words[0], words[1]);
                        }
                        else if (target == "ExportTextFile")
                        {
                            message = TMSetupEditor.TextFileProvider.ExportTextFile(words[0]);
                        }
                        break;
                    case "VariableProvider":
                        {
                            if (target == "GetProjectVariableList")
                            {
                                List<VariableInfo> variableInfos = new List<VariableInfo>();
                                message = TMSetupEditor.VariableProvider.GetProjectVariableList(ref variableInfos);
                                txt_res_pay.Text = JsonConvert.SerializeObject(variableInfos);
                            }
                            if (target == "GetGlobalVariableList")
                            {
                                List<VariableInfo> variableInfos = new List<VariableInfo>();
                                message = TMSetupEditor.VariableProvider.GetGlobalVariableList(ref variableInfos);
                                txt_res_pay.Text = JsonConvert.SerializeObject(variableInfos);
                            }
                            else if (target == "CreateProjectVariable")
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
                                        message = TMSetupEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.Boolean, words[2]);
                                        break;
                                    case "VariableType.BooleanArray":
                                        message = TMSetupEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.BooleanArray, words[2]);
                                        break;
                                    case "VariableType.Byte":
                                        message = TMSetupEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.Byte, words[2]);
                                        break;
                                    case "VariableType.ByteArray":
                                        message = TMSetupEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.ByteArray, words[2]);
                                        break;
                                    case "VariableType.Double":
                                        message = TMSetupEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.Double, words[2]);
                                        break;
                                    case "VariableType.DoubleArray":
                                        message = TMSetupEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.DoubleArray, words[2]);
                                        break;
                                    case "VariableType.Float":
                                        message = TMSetupEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.Float, words[2]);
                                        break;
                                    case "VariableType.FloatArray":
                                        message = TMSetupEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.FloatArray, words[2]);
                                        break;
                                    case "VariableType.Integer":
                                        message = TMSetupEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.Integer, words[2]);
                                        break;
                                    case "VariableType.IntegrArray":
                                        message = TMSetupEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.IntegrArray, words[2]);
                                        break;
                                    case "VariableType.String":
                                        message = TMSetupEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.String, words[2]);
                                        break;
                                    case "VariableType.StringArray":
                                        message = TMSetupEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.StringArray, words[2]);
                                        break;
                                }
                                txt_res_pay.Text = "";
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
                                        message = TMSetupEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Boolean, words[2]);
                                        break;
                                    case "VariableType.BooleanArray":
                                        message = TMSetupEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.BooleanArray, words[2]);
                                        break;
                                    case "VariableType.Byte":
                                        message = TMSetupEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Byte, words[2]);
                                        break;
                                    case "VariableType.ByteArray":
                                        message = TMSetupEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.ByteArray, words[2]);
                                        break;
                                    case "VariableType.Double":
                                        message = TMSetupEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Double, words[2]);
                                        break;
                                    case "VariableType.DoubleArray":
                                        message = TMSetupEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.DoubleArray, words[2]);
                                        break;
                                    case "VariableType.Float":
                                        message = TMSetupEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Float, words[2]);
                                        break;
                                    case "VariableType.FloatArray":
                                        message = TMSetupEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.FloatArray, words[2]);
                                        break;
                                    case "VariableType.Integer":
                                        message = TMSetupEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.Integer, words[2]);
                                        break;
                                    case "VariableType.IntegrArray":
                                        message = TMSetupEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.IntegrArray, words[2]);
                                        break;
                                    case "VariableType.String":
                                        message = TMSetupEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.String, words[2]);
                                        break;
                                    case "VariableType.StringArray":
                                        message = TMSetupEditor.VariableProvider.CreateGlobalVariable(words[0], VariableType.StringArray, words[2]);
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
                                message = TMSetupEditor.VariableProvider.ChangeProjectVariableValue(value);
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
                                message = TMSetupEditor.VariableProvider.ChangeGlobalVariableValue(value);
                                txt_res_pay.Text = "";
                            }
                            else if (target == "IsProjectVariableExist")
                            {
                                message = 0;
                                txt_res_pay.Text = TMSetupEditor.VariableProvider.IsProjectVariableExist(Input1.Text).ToString();
                            }
                            else if (target == "IsGlobalVariableExist")
                            {
                                message = 0;
                                txt_res_pay.Text = TMSetupEditor.VariableProvider.IsGlobalVariableExist(Input1.Text).ToString();
                            }
                        }
                        break;
                    case "FreeBotProvider":
                        if (target == "GetFreeBot")
                        {
                            FreeBotInfo freeBot;
                            message = TMSetupEditor.FreeBotProvider.GetFreeBot(out freeBot);
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
                            message = TMSetupEditor.FreeBotProvider.SetFreeBot(freeBot);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "HoldFreeBotKeyToHandGuide")
                        {
                            message = TMSetupEditor.FreeBotProvider.HoldFreeBotKeyToHandGuide(bool.Parse(words[0]));
                            txt_res_pay.Text = "";
                        }
                        else if (target == "KeepFreeBot")
                        {
                            message = TMSetupEditor.FreeBotProvider.KeepFreeBot();
                            txt_res_pay.Text = "";
                        }
                        break;
                    case "EndButtonEventProvider":
                        if (target == "SetEndButtonEventOwnership")
                        {
                            message = TMSetupEditor.EndButtonEventProvider.SetEndButtonEventOwnership();
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ReleaseEndButtonEventOwnership")
                        {
                            message = TMSetupEditor.EndButtonEventProvider.ReleaseEndButtonEventOwnership();
                            txt_res_pay.Text = "";
                        }
                        else if (target == "HasEndButtonEventOwnership")
                        {
                            message = 0;
                            txt_res_pay.Text = TMSetupEditor.EndButtonEventProvider.HasEndButtonEventOwnership().ToString();
                        }
                        else if (target == "IsEndButtonBoardcastMode")
                        {
                            message = 0;
                            txt_res_pay.Text = TMSetupEditor.EndButtonEventProvider.IsEndButtonBoardcastMode().ToString();
                        }
                        break;

                }
            }
            catch
            {
                //message = TMError.InvalidParameter;
            }
            txt_res_error.Text = message.ToString();
            if (message != 0)
            {
                lastError = message;
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            //UserSaveData = txtScriptEnter.Text;
            //fgSave = true;
            uint message = TMSetupEditor.ScriptWriteProvider.SaveBufferAsScript();
            txt_res_error.Text = message.ToString();
            if (message != 0)
            {
                lastError = message;
            }
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            if (TMSetupEditor != null) TMSetupEditor.Close();
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
                    case "BaseProvider":
                        SetBaseProvider();
                        break;
                    case "IOProvider":
                        SetIOProvider();
                        break;
                    case "PointProvider":
                        SetPointProvider();
                        break;
                    case "RobotStatusProvider":
                        SetRobotStatusProvider();
                        break;
                    case "ScriptWriteProvider":
                        SetScriptWriteProvider();
                        break;
                    case "SystemProvider":
                        SetSystemProvider();
                        break;
                    case "TCPProvider":
                        SetTCPProvider();
                        break;
                    case "TextFileProvider":
                        SetTextFileProvider();
                        break;
                    case "VariableProvider":
                        SetVariableProvider();
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
        private void SetPointProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetPointList");
            cb_command.Items.Add("GetPointRobotConfigs");
            cb_command.Items.Add("ChangePointRobotConfigs");
            cb_command.Items.Add("CreatePointByFlangeCoordinates");
            cb_command.Items.Add("CreatePointByToolCoordinates");
            cb_command.Items.Add("CreatePointByJointAngles");
            cb_command.Items.Add("ChangePointToolCoordinates");
            cb_command.Items.Add("ChangePointBase");
            cb_command.Items.Add("IsPointExist");
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
            cb_command.Items.Add("IsProjectVariableExist");
            cb_command.Items.Add("IsGlobalVariableExist");
        }
        private void SetScriptWriteProvider() 
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("AppendLineToBuffer");
            cb_command.Items.Add("AppendScriptToBuffer");
            cb_command.Items.Add("GetScriptBuffer");
            cb_command.Items.Add("SaveBufferAsScript");
            cb_command.Items.Add("GetScript");
        }
        private void SetSystemProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetCurrentLanguageCulture");
            cb_command.Items.Add("GetTMflowType");
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
        private void SetTextFileProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("GetTextFileList");
            cb_command.Items.Add("NewTextFile");
            cb_command.Items.Add("ReadTextFile");
            cb_command.Items.Add("WriteTextFile");
            cb_command.Items.Add("DeleteTextFile");
            cb_command.Items.Add("ImportTextFile");
            cb_command.Items.Add("ExportTextFile");
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
                    Input1.Text = "var_name,value";
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
                case "JogLineByToolAxes":
                    Input1.Text = "5,0,-252,891,90,0,0";
                    break;
                case "JogLineByCoordinates":
                    Input1.Text = "5,0,-252,891,90,0,0";
                    break;
                case "OpenVisionJob":
                    Input1.Text = "visionJobName";
                    break;
                case "WriteTextFile":
                case "NewTextFile":
                case "ReadTextFile":
                case "DeleteTextFile":
                case "ExportTextFile":
                    Input1.Text = "txtFileName";
                    break;
                case "ImportTextFile":
                    Input1.Text = "RobotName,txtFileName";
                    break;
                default:
                    Input1.Text = "";
                    break;
            }
        }

        private void btnTestBox_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Customized Setup Test MessageBox");
        }

        private void btn_Error_Click(object sender, RoutedEventArgs e)
        {
            string words = txt_res_error.Text.Trim();
            if (uint.TryParse(words, out uint myErrorNum))
            {
                string tmflowMessage = "";
                TMcraftErr mcraftErr = TMSetupEditor.GetErrMsg(myErrorNum, out tmflowMessage);
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