using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace FourWalledCubicle.MarginOfError
{
    internal sealed class ErrorGlyphFactory : IGlyphFactory
    {
        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            if (tag == null || !(tag is ErrorGlyphTag))
                return null;

            ErrorGlyphTag errorDetails = tag as ErrorGlyphTag;

            Ellipse ellipse = new Ellipse();

            switch (errorDetails.ErrorLevel)
            {
                case EnvDTE80.vsBuildErrorLevel.vsBuildErrorLevelHigh:
                    ellipse.Stroke = Brushes.DarkRed;
                    ellipse.Fill = Brushes.IndianRed;
                    break;
                case EnvDTE80.vsBuildErrorLevel.vsBuildErrorLevelMedium:
                    ellipse.Stroke = Brushes.DarkGoldenrod;
                    ellipse.Fill = Brushes.LightGoldenrodYellow;
                    break;
                case EnvDTE80.vsBuildErrorLevel.vsBuildErrorLevelLow:
                    ellipse.Stroke = Brushes.DarkBlue;
                    ellipse.Fill = Brushes.DarkGray;
                    break;
            }

            ellipse.StrokeThickness = 2;
            ellipse.Height = 12;
            ellipse.Width = 12;

            return ellipse;
        }
    }
}
