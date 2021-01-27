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
using XtoX.HexConverters;

namespace XtoX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Used to show or hide debug info in the log box.
        public static LogBoxHelper Logger;

        public MainWindow()
        {
            InitializeComponent();

            // Init the log helper.
            Logger = new LogBoxHelper(this);
        }

        /// <summary>
        /// Convert the info/values in the textblock for the value inputs here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConvertTextBoxContents(object sender, RoutedEventArgs e)
        {
            string InputContents = InputTextValues.Text;
            if (InputContents.Length == 0) { return; }

            var FormatterAndOutput = new InputFormatter(OutputGridValues, ConversionHistoryBox);
            FormatterAndOutput.DetermineTypeOfInput(InputContents);
            Logger.WriteToLogBox("----------------------------------------\n\n");

            InputContents = FormatterAndOutput.InputStringNow;
            InputTextValues.Text = InputContents;
        }

        /// <summary>
        /// Toggle Logging info.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConversionHistoryBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Toggle Logging bool.
            Logger.ShowLogInfo = !Logger.ShowLogInfo;
            Logger.WriteToLogBox("TOGGLED SHOW/HIDE DEBUG INFO OK!", true);

            // Pull all text from the log box and if we dont wanna show the debugger, remove it.
            string[] AllText = ConversionHistoryBox.Text.Split('\n');
            if (!Logger.ShowLogInfo)
            {
                for (int Count = 0; Count < AllText.Length; Count++)
                {
                    if (!AllText[Count].StartsWith("[DEBUG]")) { continue; }
                    AllText[Count] = "";
                }

                // Now loop the changed text values and see what needs to be pulled out.
                string NewTextString = "";
                foreach (var stringItem in AllText)
                {
                    if (stringItem == "") { continue; }
                    NewTextString += stringItem.Trim() + "\n";
                }

                ConversionHistoryBox.Text = NewTextString;
            }
            ConversionHistoryBox.ScrollToEnd();
        }
    }
}
