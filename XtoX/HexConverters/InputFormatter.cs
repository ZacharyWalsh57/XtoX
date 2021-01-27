using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace XtoX.HexConverters
{
    public class OutputTextBoxes
    {
        private Grid OutputValuesGrid;
        private TextEditor HistoryLog;

        public TextBox StringOutBox;
        public TextBox HexBase16Box;
        public TextBox IntBase32Box;
        public TextBox BinOutputBox;
        public TextBox DoubleOutBox;

        public OutputTextBoxes(Grid OutputGrid, TextEditor HistoryLogBox)
        {
            // Set the main grid and the log box items.
            OutputValuesGrid = OutputGrid;
            HistoryLog = HistoryLogBox;

            // Loop items in the child objects and let them get stored as a name entry.
            foreach (var ChildObject in OutputValuesGrid.Children)
            {
                // If not a textbox continue on.
                if (ChildObject.GetType() != typeof(TextBox)) { continue; }

                TextBox ThisBoxItem = (TextBox)ChildObject;
                var RowIndex = Grid.GetRow(ThisBoxItem);

                switch (RowIndex)
                {
                    case 0:
                        StringOutBox = ThisBoxItem;
                        // MainWindow.Logger.WriteToLogBox("SETUP STRING TEXTBOX", true);
                        break;

                    case 1:
                        HexBase16Box = ThisBoxItem;
                        // MainWindow.Logger.WriteToLogBox("SETUP HEX 16 TEXTBOX", true);
                        break;

                    case 2:
                        IntBase32Box = ThisBoxItem;
                        // MainWindow.Logger.WriteToLogBox("SETUP INT32 TEXTBOX", true);
                        break;

                    case 3:
                        BinOutputBox = ThisBoxItem;
                        // MainWindow.Logger.WriteToLogBox("SETUP BIN TEXTBOX", true);
                        break;

                    case 4:
                        DoubleOutBox = ThisBoxItem;
                        // MainWindow.Logger.WriteToLogBox("SETUP DOUBLE TEXTBOX", true);
                        break;
                }
            }
        }
    }


    public class InputFormatter
    {
        // Used to setup the grid for all output values.    
        public OutputTextBoxes OutputUpdater;

        public string OutputValueString;
        public string TypeOfInputString;
        public int TypeOfInputInt;

        public string InputStringNow;

        public InputFormatter(Grid OutputGrid, TextEditor HistoryLogBox)
        {
            OutputUpdater = new OutputTextBoxes(OutputGrid, HistoryLogBox);

            OutputValueString = "";
            TypeOfInputInt = 0;
            TypeOfInputString = "STRING";

            InputStringNow = "N/A";
        }

        /// <summary>
        /// Gets the type of input text based on the values passed in.
        /// </summary>
        /// <param name="InputText">String to pass in for conversion.</param>
        /// <returns>Int Value showing what kind of input we have.</returns>
        public int DetermineTypeOfInput(string InputText)
        {
            // Split log info.
            MainWindow.Logger.WriteToLogBox("----------------------------------------");

            // Check for int force or hex split string.
            bool ForceInt = InputText.EndsWith(".");
            if (InputText.Contains(" "))
            {
                string[] InputSplit = InputText.Split(' ');
                for (int Index = 0; Index < InputSplit.Length; Index++)
                {
                    if (InputSplit[Index].Length != 2) { break; }

                    InputSplit[Index] = "0x" + InputSplit[Index];
                    continue;
                }

                InputText = string.Join(" ", InputSplit);
                InputStringNow = InputText;
            }

            if (int.TryParse(InputText, out int OutputInt) || ForceInt)
            {
                foreach (var part in InputText.ToCharArray())
                {
                    if (part.ToString() == ".") { continue; }
                    if (int.Parse(part.ToString()) > 1 || ForceInt)
                    {
                        MainWindow.Logger.WriteToLogBox("INPUT TYPE:  INTERGER");
                        MainWindow.Logger.WriteToLogBox("VALUE IN:    " + InputText);

                        if (ForceInt) { OutputInt = int.Parse(InputText.Replace(".", String.Empty)); }
                        OutputValueString = OutputInt.ToString();

                        TypeOfInputInt = 3;
                        TypeOfInputString = "INTERGER";
                        InputStringNow = InputText;

                        string NoDecimal = InputText;
                        if (NoDecimal.EndsWith(".")) { NoDecimal = NoDecimal.Replace(".", String.Empty); }

                        OutputUpdater.IntBase32Box.Text = NoDecimal;
                        OutputUpdater.DoubleOutBox.Text = double.Parse(InputText).ToString("F5");
                        OutputUpdater.HexBase16Box.Text = int.Parse(NoDecimal).ToString("X");
                        OutputUpdater.BinOutputBox.Text = Convert.ToString(int.Parse(NoDecimal), 2);
                        OutputUpdater.StringOutBox.Text = InputText;

                        MainWindow.Logger.WriteToLogBox("\\__ STRING:      " + NoDecimal);
                        MainWindow.Logger.WriteToLogBox("\\__ INTERGER:    " + NoDecimal);
                        MainWindow.Logger.WriteToLogBox("\\__ DOUBLE:      " + OutputUpdater.DoubleOutBox.Text);
                        MainWindow.Logger.WriteToLogBox("\\__ HEXDECIMAL:  " + OutputUpdater.HexBase16Box.Text);
                        MainWindow.Logger.WriteToLogBox("\\__ BINARY:      " + OutputUpdater.BinOutputBox.Text);

                        return 3;
                    }
                }

                MainWindow.Logger.WriteToLogBox("INPUT TYPE:  BINARY");
                MainWindow.Logger.WriteToLogBox("VALUE IN:    " + InputText);

                OutputValueString = OutputInt.ToString();
                TypeOfInputInt = 1;
                TypeOfInputString = "BINARY";
                InputStringNow = InputText;

                int IntValue = Convert.ToInt32(InputText, 2);
                OutputUpdater.IntBase32Box.Text = IntValue.ToString();
                OutputUpdater.DoubleOutBox.Text = double.Parse(IntValue.ToString()).ToString("F5");
                OutputUpdater.HexBase16Box.Text = int.Parse(IntValue.ToString()).ToString("X");
                OutputUpdater.StringOutBox.Text = InputText;
                OutputUpdater.BinOutputBox.Text = InputText;

                MainWindow.Logger.WriteToLogBox("\\__ STRING:      " + InputText);
                MainWindow.Logger.WriteToLogBox("\\__ INTERGER:    " + IntValue.ToString());
                MainWindow.Logger.WriteToLogBox("\\__ DOUBLE:      " + OutputUpdater.DoubleOutBox.Text);
                MainWindow.Logger.WriteToLogBox("\\__ HEXDECIMAL:  " + OutputUpdater.HexBase16Box.Text);
                MainWindow.Logger.WriteToLogBox("\\__ BINARY:      " + OutputUpdater.BinOutputBox.Text);

                return 1;
            }
            if (double.TryParse(InputText, out double OutputDouble))
            {
                MainWindow.Logger.WriteToLogBox("INPUT TYPE:  DOUBLE");
                MainWindow.Logger.WriteToLogBox("VALUE IN:    " + InputText);

                OutputValueString = OutputDouble.ToString();

                string IntString = ((int)double.Parse(InputText)).ToString();
                OutputUpdater.IntBase32Box.Text = IntString;
                OutputUpdater.DoubleOutBox.Text = double.Parse(InputText).ToString("F5");
                OutputUpdater.HexBase16Box.Text = int.Parse(IntString).ToString("X");
                OutputUpdater.StringOutBox.Text = InputText;
                OutputUpdater.BinOutputBox.Text = Convert.ToString(int.Parse(IntString), 2);

                MainWindow.Logger.WriteToLogBox("\\__ STRING:      " + InputText);
                MainWindow.Logger.WriteToLogBox("\\__ INTERGER:    " + IntString);
                MainWindow.Logger.WriteToLogBox("\\__ DOUBLE:      " + OutputUpdater.DoubleOutBox.Text);
                MainWindow.Logger.WriteToLogBox("\\__ HEXDECIMAL:  " + OutputUpdater.HexBase16Box.Text);
                MainWindow.Logger.WriteToLogBox("\\__ BINARY:      " + OutputUpdater.BinOutputBox.Text);

                TypeOfInputInt = 2;
                TypeOfInputString = "DOUBLE";
                InputStringNow = InputText;

                return 2;
            }
            try
            {
                int HexValue = 0;

                MainWindow.Logger.WriteToLogBox("INPUT TYPE:  HEXDECIMAL");
                MainWindow.Logger.WriteToLogBox("VALUE IN:    " + InputText);

                if (!InputText.Contains(" ")) { HexValue = Convert.ToInt32(InputText, 16); }
                else
                {
                    string[] SplitHexValues = InputText.Split(' ');
                    foreach (string PartOfString in SplitHexValues)
                    {
                        try { HexValue += Convert.ToInt32(PartOfString, 16); }
                        catch 
                        {
                            OutputValueString = "N/A";
                            InputStringNow = "INVALID VALUE IN!";

                            OutputUpdater.IntBase32Box.Text = "ERROR";
                            OutputUpdater.DoubleOutBox.Text = "ERROR";
                            OutputUpdater.HexBase16Box.Text = "ERROR";
                            OutputUpdater.StringOutBox.Text = "ERROR";
                            OutputUpdater.BinOutputBox.Text = "ERROR";

                            MainWindow.Logger.WriteToLogBox("\\__ STRING:      ERROR");
                            MainWindow.Logger.WriteToLogBox("\\__ INTERGER:    ERROR");
                            MainWindow.Logger.WriteToLogBox("\\__ DOUBLE:      ERROR");
                            MainWindow.Logger.WriteToLogBox("\\__ HEXDECIMAL:  ERROR");
                            MainWindow.Logger.WriteToLogBox("\\__ BINARY:      ERROR");

                           return 0; 
                        }
                    }
                }

                string Converted = "";
                string No0xAndSpaces = InputText.Replace("0x", "").Replace(" ", "");
                StringBuilder Builder = new StringBuilder();
                for (int i = 0; i < No0xAndSpaces.Length; i += 2)
                {
                    string hs = No0xAndSpaces.Substring(i, 2);
                    Builder.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
                }
                Converted = Builder.ToString();

                OutputValueString = HexValue.ToString();
                OutputUpdater.IntBase32Box.Text = HexValue.ToString();
                OutputUpdater.DoubleOutBox.Text = ((double)HexValue).ToString("F5");
                OutputUpdater.HexBase16Box.Text = InputText;
                OutputUpdater.StringOutBox.Text = Converted;
                OutputUpdater.BinOutputBox.Text = Convert.ToString(HexValue, 2);

                MainWindow.Logger.WriteToLogBox("\\__ STRING:      " + OutputUpdater.StringOutBox.Text);
                MainWindow.Logger.WriteToLogBox("\\__ INTERGER:    " + HexValue.ToString());
                MainWindow.Logger.WriteToLogBox("\\__ DOUBLE:      " + OutputUpdater.DoubleOutBox.Text);
                MainWindow.Logger.WriteToLogBox("\\__ HEXDECIMAL:  " + InputText);
                MainWindow.Logger.WriteToLogBox("\\__ BINARY:      " + OutputUpdater.BinOutputBox.Text);

                TypeOfInputInt = 4;
                TypeOfInputString = "HEXDECIMAL";
                InputStringNow = InputText;

                return 4;
            }
            catch { }

            // Input was string.
            MainWindow.Logger.WriteToLogBox("INPUT TYPE:  STRING");
            MainWindow.Logger.WriteToLogBox("VALUE IN:    " + InputText);

            MainWindow.Logger.WriteToLogBox("\\__ STRING:      " + InputText);
            MainWindow.Logger.WriteToLogBox("\\__ INTERGER:    " + "N/A");
            MainWindow.Logger.WriteToLogBox("\\__ DOUBLE:      " + "N/A");
            MainWindow.Logger.WriteToLogBox("\\__ HEXDECIMAL:  " + "N/A");
            MainWindow.Logger.WriteToLogBox("\\__ BINARY:      " + "N/A");

            OutputValueString = "N/A";
            InputStringNow = InputText;

            return 0;
        }
    }
}
