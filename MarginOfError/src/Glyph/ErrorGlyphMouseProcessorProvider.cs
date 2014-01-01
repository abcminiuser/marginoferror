using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Adornments;
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
        internal IBufferTagAggregatorFactoryService AggregatorService { get; set; }

        [Import]
        internal IToolTipProviderFactory ToolTipProviderFactory { get; set; }

        public IMouseProcessor GetAssociatedMouseProcessor(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin margin)
        {
            ITagAggregator<ErrorGlyphTag> tagAggregator = AggregatorService.CreateTagAggregator<ErrorGlyphTag>(wpfTextViewHost.TextView.TextBuffer);
            IToolTipProvider tooltipProvider = ToolTipProviderFactory.GetToolTipProvider(wpfTextViewHost.TextView);

            return new ErrorGlyphMouseProcessor(wpfTextViewHost, margin, tagAggregator, tooltipProvider);
        }
    }
}
