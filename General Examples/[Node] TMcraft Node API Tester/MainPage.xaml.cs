﻿using System;
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
//using TMcraftNodeType;

namespace TMCraftNodeTestDll
{
    /// <summary>
    /// UserControl1.xaml 的互動邏輯
    /// </summary>
    public partial class MainPage : UserControl, ITMcraftNodeEntry//, ITMcraftNodeOutputTypeEntry
    {
        public MainPage()
        {
            InitializeComponent();
        }
        /*
        /// <summary>
        /// Multi Node 宣告
        /// </summary>        
        public BinaryNodeTemplate DefineBinaryNodeTemplate { get; set; }
        public NodeOutputTypeTemplate DefineNodeOutputType { get; set; }
        public List<string> DefineCaseNodes { get; set; }
        public void InitializeNodeOutputType()
        {
            //初始化定義            
            DefineNodeOutputType = NodeOutputTypeTemplate.Binary;
            DefineBinaryNodeTemplate = BinaryNodeTemplate.Yes_No;
            DefineCaseNodes = new List<string>();
            DefineNodeOutputType = NodeOutputTypeTemplate.Multi;
            DefineCaseNodes = new List<string>
            {
                "One",
                "Two",
                "Three",
                "Final"
            };
            DefineNodeOutputType = NodeOutputTypeTemplate.Single;
        }
        */
        TMcraftNodeAPI TMNodeEditor;
        string UserSaveData = "";
        bool fgSave = false;
        uint lastError = 0;
        public void InitializeNode(TMcraftNodeAPI tMNodeEditor)
        {
            TMNodeEditor = tMNodeEditor;
            TMNodeEditor.EndButtonEventProvider.EndButtonClickEvent += RobotStatusProvider_EndButtonClickEvent;            
        }

        private void RobotStatusProvider_EndButtonClickEvent(RobotEventType type, object data)
        {
            Dispatcher.Invoke(
                               DispatcherPriority.Background,
                               new Action(delegate ()
                               {
                                   txtRobotEvent.Text = type.ToString() + " " + data.ToString();
                               }));            
        }

        public void InscribeScript(ScriptWriteProvider scriptWriter)
        {
            if (fgSave)
            {
                //Write script code to TMFlow.
                scriptWriter.AppendLine(UserSaveData);
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cb_Class.Items.Clear();
            cb_Class.Items.Add("TMcraft");
            cb_Class.Items.Add("BaseProvider");
            cb_Class.Items.Add("DataStorageProvider");
            cb_Class.Items.Add("IOProvider");
            cb_Class.Items.Add("PointProvider");
            cb_Class.Items.Add("RobotJogProvider");
            cb_Class.Items.Add("RobotStatusProvider");
            cb_Class.Items.Add("SystemProvider");
            cb_Class.Items.Add("TCPProvider");
            cb_Class.Items.Add("TextFileProvider");
            cb_Class.Items.Add("VariableProvider");
            cb_Class.Items.Add("VisionProvider");
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
            if (TMNodeEditor == null) { MessageBox.Show("TMNodeEditor == null"); }
            CNTitle.Content = string.Format("TMcraft Node Test Tool.(Version = {0})",TMcraftNodeAPI.Version);            
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
                    case "BaseProvider":
                        if (target == "GetBaseList")
                        {
                            List<BaseInfo> list = new List<BaseInfo>();
                            message = TMNodeEditor.BaseProvider.GetBaseList(ref list);
                            txt_res_pay.Text = JsonConvert.SerializeObject(list);
                        }
                        else if (target == "CreateNewBase")
                        {
                            //"Cbase1", new float[] { 11, 22, 0, 0, 0, 0 }
                            message = TMNodeEditor.BaseProvider.CreateNewBase(words[0], new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) });
                        }
                        else if (target == "ChangeBaseValue")
                        {
                            message = TMNodeEditor.BaseProvider.ChangeBaseValue(words[0], new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) });                            
                        }
                        else if (target == "DeleteBase")
                        {
                            message = TMNodeEditor.BaseProvider.DeleteBase(words[0]);                            
                        }
                        else if (target == "IsBaseExist")
                        {
                            message = 0;
                            txt_res_pay.Text = TMNodeEditor.BaseProvider.IsBaseExist(words[0]).ToString();
                        }
                        break;
                    case "DataStorageProvider":
                        if (target == "SaveData(string)")
                        {
                            Dictionary<string, string> pay = new Dictionary<string, string>();
                            //pay.Add("k1", "value1");
                            //pay.Add("k2", "value2");
                            //pay.Add("k3", "value3");
                            pay.Add(words[0], words[1]);
                            message = TMNodeEditor.DataStorageProvider.SaveData(pay);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "GetAllData")
                        {
                            Dictionary<string, object> pay;
                            message = TMNodeEditor.DataStorageProvider.GetAllData(out pay);
                            txt_res_pay.Text = JsonConvert.SerializeObject(pay);
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
                            words[0] = words[0].Replace("IO_TYPE.","");
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
                    case "PointProvider":
                        if (target == "GetPointList")
                        {
                            List<PointInfo> list = new List<PointInfo>();
                            message = TMNodeEditor.PointProvider.GetPointList(ref list);
                            txt_res_pay.Text = JsonConvert.SerializeObject(list);
                        }
                        else if (target == "GetPointRobotConfigs")
                        {
                            int[] config = new int[] { };
                            message = TMNodeEditor.PointProvider.GetPointRobotConfigs(words[0], ref config);
                            txt_res_pay.Text = JsonConvert.SerializeObject(config);
                        }
                        else if (target == "ChangePointRobotConfigs")
                        {
                            int[] config = new int[] { int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]) };
                            message = TMNodeEditor.PointProvider.ChangePointRobotConfigs(words[0], config);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "CreatePointByFlangeCoordinates")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            int[] configs = new int[] { int.Parse(words[7]), int.Parse(words[8]), int.Parse(words[9]) };
                            message = TMNodeEditor.PointProvider.CreatePointByFlangeCoordinates(words[0],
                                value, configs, words[10], words[11]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "CreatePointByToolCoordinates")
                        {
                            float[] value = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            int[] configs = new int[] {int.Parse(words[7]), int.Parse(words[8]), int.Parse(words[9]) };
                            message = TMNodeEditor.PointProvider.CreatePointByToolCoordinates(words[0], value, configs, words[10], words[11]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "CreatePointByJointAngles")
                        {
                            //float[] joint = new float[] { 10, 0, 2, 0, 0, 3 };
                            float[] joint = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            //message = TMNodeEditor.PointProvider.CreatePointByJointAngles("cpoint1", joint, "RobotBase", "NOTOOL");
                            message = TMNodeEditor.PointProvider.CreatePointByJointAngles(words[0], joint, words[7], words[8]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ChangePointToolCoordinates")
                        {
                            float[] tool = new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) };
                            message = TMNodeEditor.PointProvider.ChangePointToolCoordinates(words[0], tool);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ChangePointBase")
                        {
                            message = TMNodeEditor.PointProvider.ChangePointBase(words[0], words[1]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "IsPointExist")
                        {
                            message = 0;
                            txt_res_pay.Text = TMNodeEditor.PointProvider.IsPointExist(words[0]).ToString();
                        }
                        break;
                    case "RobotJogProvider":
                        try
                        {
                            if (target == "JogJoint")
                            {   //0.01F
                                message = TMNodeEditor.RobotJogProvider.JogByJoint(float.Parse(words[0]), new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) });
                                txt_res_pay.Text = "";
                            }
                            else if (target == "JogLineByToolAxes")
                            {
                                message = TMNodeEditor.RobotJogProvider.JogRelativeByTool(float.Parse(words[0]), new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) });
                                txt_res_pay.Text = "";
                            }
                            else if (target == "JogLineByCoordinates")
                            {
                                message = TMNodeEditor.RobotJogProvider.JogByBase(float.Parse(words[0]), new float[] { float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]), float.Parse(words[5]), float.Parse(words[6]) });
                                txt_res_pay.Text = "";
                            }
                            else if (target == "StopJog")
                            {
                                message = TMNodeEditor.RobotJogProvider.StopJog();
                                txt_res_pay.Text = "";
                            }
                            else if (target == "OpenControllerPanel")
                            {
                                message = TMNodeEditor.RobotJogProvider.OpenControllerPanel();
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
                            TCPInfo pp = new TCPInfo() {  name = words[0], data = value };
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
                            message = TMNodeEditor.TCPProvider.GetTcpMassCenter(words[0] ,out value);
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
                            message = TMNodeEditor.TCPProvider.GetTcpInertia(words[0],out value);
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
                    case "TextFileProvider":
                        if (target == "GetTextFileList")
                        {
                            string[] lst ;
                            message = TMNodeEditor.TextFileProvider.GetTextFileList(out lst);
                            StringBuilder stringBuilder = new StringBuilder();
                            foreach(string x in lst)
                            {
                                stringBuilder.AppendLine(x);
                            }
                            txt_res_pay.Text = stringBuilder.ToString();
                        }
                        else if (target == "NewTextFile")
                        {
                            message = TMNodeEditor.TextFileProvider.NewTextFile(words[0], txtScriptEnter.Text);
                        }
                        else if (target == "ReadTextFile")
                        {
                            string fileContent = "";
                            message = TMNodeEditor.TextFileProvider.ReadTextFile(words[0], out fileContent);
                            txt_res_pay.Text = fileContent;
                        }
                        else if (target == "WriteTextFile")
                        {
                            message = TMNodeEditor.TextFileProvider.WriteTextFile(words[0], txtScriptEnter.Text);
                        }
                        else if (target == "DeleteTextFile")
                        {
                            message = TMNodeEditor.TextFileProvider.DeleteTextFile(words[0]);
                        }
                        else if (target == "ImportTextFile")
                        {
                            message = TMNodeEditor.TextFileProvider.ImportTextFile(words[0], words[1]);
                        }
                        else if (target == "ExportTextFile")
                        {
                            message = TMNodeEditor.TextFileProvider.ExportTextFile(words[0]);
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
                                        message = TMNodeEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.Boolean, words[2]);
                                        break;
                                    case "VariableType.BooleanArray":
                                        message = TMNodeEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.BooleanArray, words[2]);
                                        break;
                                    case "VariableType.Byte":
                                        message = TMNodeEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.Byte, words[2]);
                                        break;
                                    case "VariableType.ByteArray":
                                        message = TMNodeEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.ByteArray, words[2]);
                                        break;
                                    case "VariableType.Double":
                                        message = TMNodeEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.Double, words[2]);
                                        break;
                                    case "VariableType.DoubleArray":
                                        message = TMNodeEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.DoubleArray, words[2]);
                                        break;
                                    case "VariableType.Float":
                                        message = TMNodeEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.Float, words[2]);
                                        break;
                                    case "VariableType.FloatArray":
                                        message = TMNodeEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.FloatArray, words[2]);
                                        break;
                                    case "VariableType.Integer":
                                        message = TMNodeEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.Integer, words[2]);
                                        break;
                                    case "VariableType.IntegrArray":
                                        message = TMNodeEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.IntegrArray, words[2]);
                                        break;
                                    case "VariableType.String":
                                        message = TMNodeEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.String, words[2]);
                                        break;
                                    case "VariableType.StringArray":
                                        message = TMNodeEditor.VariableProvider.CreateProjectVariable(words[0], VariableType.StringArray, words[2]);
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
                                    if (start != -1 && end != -1 && start < end )
                                    {
                                        words[2] = content.Substring(start, end - start +1);
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
                            else if (target == "DeleteProjectVariable")
                            {
                                message = TMNodeEditor.VariableProvider.DeleteProjectVariable(Input1.Text);
                                txt_res_pay.Text = "";
                            }
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
                        }
                        break;
                    case "VisionProvider":
                        if (target == "GetVisionJobList")
                        {
                            List<string> list;
                            message = TMNodeEditor.VisionProvider.GetVisionJobList(out list);
                            txt_res_pay.Text = JsonConvert.SerializeObject(list);
                        }
                        else if (target== "GetVisionJobInitialPoint")
                        {
                            float[] value = new float[] { };
                            message = TMNodeEditor.VisionProvider.GetVisionJobInitialPoint(words[0], out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "GetVisionJobInitialBase")
                        {
                            string value = "";
                            message = TMNodeEditor.VisionProvider.GetVisionJobInitialBase(words[0], out value);
                            txt_res_pay.Text = JsonConvert.SerializeObject(value);
                        }
                        else if (target == "CreateVisionJob")
                        {
                            message = TMNodeEditor.VisionProvider.CreateVisionJob(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "DeleteVisionJob")
                        {
                            message = TMNodeEditor.VisionProvider.DeleteVisionJob(words[0]);
                            txt_res_pay.Text = "";
                        }
                        else if (target == "OpenVisionJob")
                        {
                            message = TMNodeEditor.VisionProvider.OpenVisionJob(words[0]);
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
                            message = TMNodeEditor.FreeBotProvider.HoldFreeBotKeyToHandGuide(bool.Parse(words[0]));
                            txt_res_pay.Text = "";
                        }
                        else if (target == "KeepFreeBot")
                        {
                            message = TMNodeEditor.FreeBotProvider.KeepFreeBot();
                            txt_res_pay.Text = "";
                        }
                        break;
                    case "EndButtonEventProvider":
                        if (target == "SetEndButtonEventOwnership")
                        {
                            message = TMNodeEditor.EndButtonEventProvider.SetEndButtonEventOwnership();
                            txt_res_pay.Text = "";
                        }
                        else if (target == "ReleaseEndButtonEventOwnership")
                        {
                            message = TMNodeEditor.EndButtonEventProvider.ReleaseEndButtonEventOwnership();
                            txt_res_pay.Text = "";
                        }
                        else if (target == "HasEndButtonEventOwnership")
                        {
                            message = 0;
                            txt_res_pay.Text = TMNodeEditor.EndButtonEventProvider.HasEndButtonEventOwnership().ToString();
                        }
                        else if (target == "IsEndButtonBoardcastMode")
                        {
                            message = 0;
                            txt_res_pay.Text = TMNodeEditor.EndButtonEventProvider.IsEndButtonBoardcastMode().ToString();
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
            UserSaveData = txtScriptEnter.Text;
            fgSave = true;
            if (TMNodeEditor != null) TMNodeEditor.Close();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            if (TMNodeEditor != null) TMNodeEditor.Close();
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
                    case "DataStorageProvider":
                        SetDataStorageProvider();
                        break;
                    case "IOProvider":
                        SetIOProvider();
                        break;
                    case "PointProvider":
                        SetPointProvider();
                        break;
                    case "RobotJogProvider":
                        SetRobotJogProvider();
                        break;
                    case "RobotStatusProvider":
                        SetRobotStatusProvider();
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
            cb_command.Items.Add("DeleteProjectVariable");
            cb_command.Items.Add("DeleteGlobalVariable");
            cb_command.Items.Add("IsProjectVariableExist");
            cb_command.Items.Add("IsGlobalVariableExist");
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
        private void SetRobotJogProvider()
        {
            cb_command.Items.Clear();
            cb_command.Items.Add("JogJoint");
            cb_command.Items.Add("JogLineByToolAxes");
            cb_command.Items.Add("JogLineByCoordinates");
            cb_command.Items.Add("StopJog");
            cb_command.Items.Add("OpenControllerPanel");
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
            switch(define)
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
                    Input1.Text = "txtFileName";
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