using System.Windows.Input;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;

namespace FourWalledCubicle.MarginOfError
{
    internal sealed class ErrorGlyphMouseProcessor : MouseProcessorBase
    {
        private IWpfTextViewHost mTextViewHost;
        private IWpfTextViewMargin mTextViewMargin;
        private ITagAggregator<ErrorGlyphTag> mTagAggregator;

        public ErrorGlyphMouseProcessor(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin margin, ITagAggregator<ErrorGlyphTag> tagAggregator)
        {
            mTextViewHost = wpfTextViewHost;
            mTextViewMargin = margin;
            mTagAggregator = tagAggregator;
        }

        public override void PostprocessMouseEnter(MouseEventArgs e)
        {
            ITextView view = mTextViewHost.TextView;

            Point mousePosition = Mouse.GetPosition(mTextViewHost.TextView.VisualElement);
            mousePosition.Offset(view.ViewportLeft, view.ViewportTop);

            ITextViewLine line = view.TextViewLines.GetTextViewLineContainingYCoordinate(mousePosition.Y);

            foreach (IMappingTagSpan<ErrorGlyphTag> eSpan in mTagAggregator.GetTags(new SnapshotSpan(line.Start, line.End)))
                mTextViewMargin.VisualElement.ToolTip = eSpan.Tag.Description;
        }
    }
}
