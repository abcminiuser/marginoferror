using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace FourWalledCubicle.MarginOfError
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(ErrorScrollMargin.MarginName)]
    [Order(Before = PredefinedMarginNames.VerticalScrollBar)]
    [MarginContainer(PredefinedMarginNames.VerticalScrollBarContainer)]
    [ContentType("code")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class ErrorScrollMarginFactory : IWpfTextViewMarginProvider
    {
        [Import]
        internal IViewTagAggregatorFactoryService TagAggregatorFactoryService { get; set; }

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost textViewHost, IWpfTextViewMargin containerMargin)
        {
            return new ErrorScrollMargin(textViewHost.TextView, this);
        }
    }
}
