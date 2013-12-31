using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Tagging;

namespace FourWalledCubicle.MarginOfError
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(ErrorScrollMargin.MarginName)]
    [Order(Before = PredefinedMarginNames.RightControl)]
    [MarginContainer(PredefinedMarginNames.Right)]
    [ContentType("code")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class ErrorScrollMarginFactory : IWpfTextViewMarginProvider
    {
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost textViewHost, IWpfTextViewMargin containerMargin)
        {
            return new ErrorScrollMargin(textViewHost.TextView, this);
        }

        [Import]
        internal IViewTagAggregatorFactoryService TagAggregatorFactoryService { get; set; }
    }
}
