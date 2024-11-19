using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace MutliLanguageDemo
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainPage : UserControl, ITMcraftNodeEntry
    {
        public MainPage()
        {
            InitializeComponent();
        }

        TMcraftNodeAPI TMNodeEditor;
        public void InitializeNode(TMcraftNodeAPI tmNodeEditor)
        {
            TMNodeEditor = tmNodeEditor;
        }

        public void InscribeScript(ScriptWriteProvider scriptWriter)
        {
            //This demo does not requires TMscript.
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SetLanguageDictionary();
            }
            catch (Exception ex) { MessageBox.Show("UserControl fails loading: " + ex.Message); }
        }

        private void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();
            string culture = String.Empty;            

            if (TMNodeEditor == null)
            {
                MessageBox.Show("[Set Language] Cannot connect to TMflow...");
                return;
            }
            else 
            {
                uint result = 0;
                string TMErrMsg = string.Empty;

                result = TMNodeEditor.SystemProvider.GetCurrentLanguageCulture(out culture);
                if (result != 0)//not OK
                {
                    TMNodeEditor.GetErrMsg(result, out TMErrMsg);
                    MessageBox.Show("[Set Language] " + TMErrMsg);
                }
            }

            //Must use absoulte directory
            switch (culture)
            {
                case "en-US":
                    dict.Source = new Uri("pack://application:,,,/MutliLanguageDemo;component/Resources/StringResource.en-US.xaml", UriKind.Absolute);
                    break;
                case "zh-TW":
                    dict.Source = new Uri("pack://application:,,,/MutliLanguageDemo;component/Resources/StringResource.zh-TW.xaml", UriKind.Absolute);
                    break;
                default:
                    dict.Source = new Uri("pack://application:,,,/MutliLanguageDemo;component/Resources/StringResource.en-US.xaml", UriKind.Absolute);
                    break;
            }

            this.Resources.MergedDictionaries.Add(dict);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (TMNodeEditor != null)
            {
                TMNodeEditor.Close();
            }
            else
            {
                MessageBox.Show("[Leave] Cannot connect to TMflow...");
            }
        }
    }
}