using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
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
                IMappingTagSpan<ErrorGlyphTag> eSpan = mTagAggregator.GetTags(line.ExtentAsMappingSpan).FirstOrDefault();

                if (eSpan != null)
                    SetTooltip(eSpan.Tag, view.TextSnapshot.CreateTrackingSpan(line.Start, line.Length, SpanTrackingMode.EdgeExclusive));
                else
                    SetTooltip(null, null);
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
                if ((currentTag == null) || (newTag.Description != currentTag.Description))
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
