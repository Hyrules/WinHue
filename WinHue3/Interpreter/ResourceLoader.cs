using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace WinHue3
{
    public static class ResourceLoader
    {
        public static IHighlightingDefinition LoadHighlightingDefinition(string resourceName)
        {
            
            var type = typeof(ResourceLoader);
            var fullName = type.Namespace + "." + resourceName;
            using (var stream = type.Assembly.GetManifestResourceStream(fullName))
            using (var reader = new XmlTextReader(stream))
                return HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }

        public static BinaryReader LoadGrammarTables(string grammarname)
        {
            var type = typeof(ResourceLoader);
            var fullName = type.Namespace + "." + grammarname;
            using (var stream = type.Assembly.GetManifestResourceStream(fullName))
                return new BinaryReader(stream);
        }
    }
}