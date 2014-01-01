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
        private readonly IWpfTextViewHost _textViewHost;
        private readonly IWpfTextViewMargin _textViewMargin;
        private readonly ITagAggregator<ErrorGlyphTag> _tagAggregator;
        private readonly IToolTipProvider _toolTipProvider;

        private ErrorGlyphTag _currentTag = null;

        public ErrorGlyphMouseProcessor(IWpfTextViewHost textViewHost, IWpfTextViewMargin textViewMargin, ITagAggregator<ErrorGlyphTag> tagAggregator, IToolTipProvider toolTipProvider)
        {
            _textViewHost = textViewHost;
            _textViewMargin = textViewMargin;
            _tagAggregator = tagAggregator;
            _toolTipProvider = toolTipProvider;
        }

        public override void PostprocessMouseMove(MouseEventArgs e)
        {
            ITextView view = _textViewHost.TextView;

            Point mousePosition = Mouse.GetPosition(_textViewHost.TextView.VisualElement);
            mousePosition.Offset(view.ViewportLeft, view.ViewportTop);

            ITextViewLine line = view.TextViewLines.GetTextViewLineContainingYCoordinate(mousePosition.Y);
            if (line != null)
            {
                IMappingTagSpan<ErrorGlyphTag> eSpan = _tagAggregator.GetTags(line.ExtentAsMappingSpan).FirstOrDefault();

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
                if ((_currentTag == null) || (newTag.Description != _currentTag.Description))
                {
                    _toolTipProvider.ShowToolTip(trackingSpan, newTag.Description);
                    _currentTag = newTag;
                }
            }
            else if (_currentTag != null)
            {
                _toolTipProvider.ClearToolTip();
                _currentTag = null;
            }
        }
    }
}
