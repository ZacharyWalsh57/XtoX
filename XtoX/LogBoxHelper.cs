using System.IO;
using System.Xml;
using System.Reflection;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Linq;
using System.Diagnostics;

namespace XtoX
{
    public class LogBoxHelper
    {
        private MainWindow Main;
        private TextEditor LogBox;

        public bool ShowLogInfo = false;
        private string XSHDFileName;

        public LogBoxHelper(MainWindow MainView)
        {
            Main = MainView;
            LogBox = Main.ConversionHistoryBox;

            var CurrentAssy = Assembly.GetExecutingAssembly();
            XSHDFileName = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Single(str => str.EndsWith("HilightingRules.xshd"));

            // if (Debugger.IsAttached)
            // {
            RenameXSHDFile();           // Update file format from old to new. 
            LoadHilighterObject();      // Setup the higligher now.
            // }
        }

        /// <summary>
        /// Renames old file/formatting.
        /// </summary>
        private void RenameXSHDFile()
        {
            // Convert old value type here.
            XshdSyntaxDefinition xshd;
            try
            {
                using (XmlTextReader OldXshdRead = new XmlTextReader(XSHDFileName))
                    xshd = HighlightingLoader.LoadXshd(OldXshdRead);
            }
            catch
            {
                // If we failed to get the type of file/file name from the assy.
                string FileName = XSHDFileName.Replace("XtoX.", "");
                using (XmlTextReader OldXshdRead = new XmlTextReader(FileName))
                    xshd = HighlightingLoader.LoadXshd(OldXshdRead);
            }

            // Write the new values out here.
            using (XmlTextWriter XMLWriter = new XmlTextWriter(XSHDFileName, System.Text.Encoding.UTF8))
            {
                XMLWriter.Formatting = Formatting.Indented;
                new SaveXshdVisitor(XMLWriter).WriteDefinition(xshd);
            }
        }

        /// <summary>
        /// Loads the hilighter object here.
        /// </summary>
        private void LoadHilighterObject()
        {
            using (Stream StreamOfFile = Assembly.GetExecutingAssembly().GetManifestResourceStream(XSHDFileName))
            using (XmlTextReader XMLRead = new XmlTextReader(StreamOfFile))
                LogBox.SyntaxHighlighting = HighlightingLoader.Load(XMLRead, HighlightingManager.Instance);
        }

        /// <summary>
        /// Writes info to the textbox.
        /// </summary>
        /// <param name="TextToWrite">Text to write out to the debug box.</param>
        public void WriteToLogBox(string TextToWrite, bool IsDebug = false)
        {
            if (!TextToWrite.EndsWith("\n")) { TextToWrite = TextToWrite + "\n"; }
            if (IsDebug) 
            {
                if (!ShowLogInfo) { return; }
                TextToWrite = "[DEBUG] " + TextToWrite; 
            }

            Main.Dispatcher.Invoke(() =>
            {
                Main.ConversionHistoryBox.Text += TextToWrite;
                Main.ConversionHistoryBox.ScrollToEnd();
            });
        }
    }
}
