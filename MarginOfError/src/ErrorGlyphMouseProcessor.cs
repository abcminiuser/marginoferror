using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;

namespace FourWalledCubicle.MarginOfError
{
    internal sealed class ErrorGlyphMouseProcessor : MouseProcessorBase
    {
        private readonly IWpfTextViewHost mTextViewHost;
        private readonly IWpfTextViewMargin mTextViewMargin;
        private readonly ITagAggregator<ErrorGlyphTag> mTagAggregator;

        private bool isToolTipOwner = false;

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
            if (line != null)
            {
                foreach (IMappingTagSpan<ErrorGlyphTag> eSpan in mTagAggregator.GetTags(new SnapshotSpan(line.Start, line.End)))
                {
                    mTextViewMargin.VisualElement.ToolTip = eSpan.Tag.Description;
                    isToolTipOwner = true;
                }
            }
        }

        public override void PostprocessMouseLeave(MouseEventArgs e)
        {
            if (isToolTipOwner == true)
            {
                mTextViewMargin.VisualElement.ToolTip = null;
                isToolTipOwner = false;
            }
        }
    }
}
