using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.SharpDevelop.Editor;
using HueLib2;

namespace WinHue3
{
    public class AnimatorView : View
    {

        private WinHueParser parser;
        private TextMarkerService tms;
        private TextEditor _textEditor;

        //****************** CTOR **********************************

        public AnimatorView(TextEditor editor)
        {
     
            _textEditor = editor;
            parser = new WinHueParser();
            _textEditor.SyntaxHighlighting = ResourceLoader.LoadHighlightingDefinition("Interpreter.Syntax.xshd");
            tms = new TextMarkerService(_textEditor.Document);
            _textEditor.TextArea.TextView.BackgroundRenderers.Add(tms);
            _textEditor.TextArea.TextView.LineTransformers.Add(tms);
        }

        //******************* METHODS ******************************

        private void CheckSyntax()
        {
            bool accepted = parser.Parse(_textEditor.Document.Text);
            if (!(accepted && parser.Error == null))
            {
                //lblMessage.Content = parser.Error.message;

                if (parser.Error == null) return;
                ITextMarker mkr = tms.Create(parser.Error.charpos, 5);
                mkr.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                mkr.MarkerColor = System.Windows.Media.Colors.Red;
                mkr.ToolTip = parser.Error.message;

            }
        }


        //******************** COMMANDS ****************************
        public ICommand CheckSyntaxCommand => new RelayCommand(param => CheckSyntax());


    }
}
