using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace FourWalledCubicle.MarginOfError
{
    [Export(typeof(IGlyphMouseProcessorProvider))]
    [Name("MarginOfErrorGlyphIMouseProcessorProvider")]
    [ContentType("code")]
    public sealed class ErrorGlyphMouseProcessorProvider : IGlyphMouseProcessorProvider
    {
        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorService = null;

        public IMouseProcessor GetAssociatedMouseProcessor(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin margin)
        {
            ITagAggregator<ErrorGlyphTag> tagAggregator = aggregatorService.CreateTagAggregator<ErrorGlyphTag>(wpfTextViewHost.TextView.TextBuffer);
            return new ErrorGlyphMouseProcessor(wpfTextViewHost, margin, tagAggregator);
        }
    }
}
