using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Tagging;

namespace FourWalledCubicle.MarginOfError
{
    [Export(typeof(IGlyphMouseProcessorProvider))]
    [Name("MarginOfErrorGlyphIMouseProcessorProvider")]
    [ContentType("code")]
    public sealed class ErrorGlyphMouseProcessorProvider : IGlyphMouseProcessorProvider
    {
        [Import]
        IBufferTagAggregatorFactoryService aggregatorService = null;

        public IMouseProcessor GetAssociatedMouseProcessor(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin margin)
        {
            ITagAggregator<ErrorGlyphTag> tagAggregator = aggregatorService.CreateTagAggregator<ErrorGlyphTag>(wpfTextViewHost.TextView.TextBuffer);
            return new ErrorGlyphMouseProcessor(wpfTextViewHost, margin, tagAggregator);
        }
    }
}
