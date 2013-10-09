using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Adornments;

namespace FourWalledCubicle.MarginOfError
{
    internal sealed class ErrorGlyphMouseProcessor : MouseProcessorBase
    {
        private readonly IWpfTextViewHost mTextViewHost;
        private readonly IWpfTextViewMargin mTextViewMargin;
        private readonly ITagAggregator<ErrorGlyphTag> mTagAggregator;
        private readonly IToolTipProvider mToolTipProvider;

        private ErrorGlyphTag currentTag = null;

        public ErrorGlyphMouseProcessor(IWpfTextViewHost textViewHost, IWpfTextViewMargin textViewMargin, ITagAggregator<ErrorGlyphTag> tagAggregator, IToolTipProvider toolTipProvider)
        {
            mTextViewHost = textViewHost;
            mTextViewMargin = textViewMargin;
            mTagAggregator = tagAggregator;
            mToolTipProvider = toolTipProvider;
        }

        public override void PostprocessMouseMove(MouseEventArgs e)
        {
            ITextView view = mTextViewHost.TextView;

            Point mousePosition = Mouse.GetPosition(mTextViewHost.TextView.VisualElement);
            mousePosition.Offset(view.ViewportLeft, view.ViewportTop);

            ITextViewLine line = view.TextViewLines.GetTextViewLineContainingYCoordinate(mousePosition.Y);
            if (line != null)
            {
                ErrorGlyphTag newTag = null;

                foreach (IMappingTagSpan<ErrorGlyphTag> eSpan in mTagAggregator.GetTags(new SnapshotSpan(line.Start, line.End)))
                    newTag = eSpan.Tag;

                SetTooltip(newTag, view.TextSnapshot.CreateTrackingSpan(line.Start, line.Length, SpanTrackingMode.EdgeExclusive));
            }
        }

        public override void PostprocessMouseLeave(MouseEventArgs e)
        {
            SetTooltip(null, null);
        }

        private void SetTooltip(ErrorGlyphTag newTag, ITrackingSpan trackingSpan)
        {
            if (newTag != null)
            {
                if (newTag != currentTag)
                {
                    mToolTipProvider.ShowToolTip(trackingSpan, newTag.Description);
                    currentTag = newTag;
                }
            }
            else if (currentTag != null)
            {
                mToolTipProvider.ClearToolTip();
                currentTag = null;
            }
        }
    }
}
