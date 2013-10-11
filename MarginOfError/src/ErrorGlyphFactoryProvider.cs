using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace FourWalledCubicle.MarginOfError
{
    [Export(typeof(IGlyphFactoryProvider))]
    [Name("MarginOfErrorGlyphFactoryProvder")]
    [ContentType("code")]
    [Order(After="VsTextMarker")]
    [TagType(typeof(ErrorGlyphTag))]
    public sealed class ErrorGlyphFactoryProvider : IGlyphFactoryProvider
    {
        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
        {
            return new ErrorGlyphFactory();
        }
    }
}
