using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.Windows.Media.Imaging;

namespace FourWalledCubicle.MarginOfError
{
    internal sealed class ErrorGlyphFactory : IGlyphFactory
    {
        private const string packURIPrefix = @"pack://application:,,,/MarginOfError;component/";

        private readonly BitmapImage errorIcon = new BitmapImage(new Uri(packURIPrefix + "Resources/ErrorIcon.png"));
        private readonly BitmapImage warningIcon = new BitmapImage(new Uri(packURIPrefix + "Resources/WarningIcon.png"));
        private readonly BitmapImage infoIcon = new BitmapImage(new Uri(packURIPrefix + "Resources/InfoIcon.png"));

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            if (tag == null || !(tag is ErrorGlyphTag))
                return null;

            ErrorGlyphTag errorDetails = tag as ErrorGlyphTag;

            Image glyphIcon = new System.Windows.Controls.Image();
            glyphIcon.Width = 14;
            glyphIcon.Height = 14;

            switch (errorDetails.ErrorLevel)
            {
                case EnvDTE80.vsBuildErrorLevel.vsBuildErrorLevelHigh:
                    glyphIcon.Source = errorIcon;
                    break;
                case EnvDTE80.vsBuildErrorLevel.vsBuildErrorLevelMedium:
                    glyphIcon.Source = warningIcon;
                    break;
                case EnvDTE80.vsBuildErrorLevel.vsBuildErrorLevelLow:
                    glyphIcon.Source = infoIcon;
                    break;
            }

            return glyphIcon;
        }
    }
}
