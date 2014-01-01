using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using EnvDTE80;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace FourWalledCubicle.MarginOfError
{
    internal sealed class ErrorGlyphFactory : IGlyphFactory
    {
        private const string PackURIPrefix = @"pack://application:,,,/MarginOfError;component/";

        private readonly BitmapImage _errorIcon = new BitmapImage(new Uri(PackURIPrefix + "Resources/ErrorIcon.png"));
        private readonly BitmapImage _warningIcon = new BitmapImage(new Uri(PackURIPrefix + "Resources/WarningIcon.png"));
        private readonly BitmapImage _infoIcon = new BitmapImage(new Uri(PackURIPrefix + "Resources/InfoIcon.png"));

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            ErrorGlyphTag errorDetails = tag as ErrorGlyphTag;

            if (errorDetails == null)
                return null;

            BitmapImage glyphImage = null;

            switch (errorDetails.ErrorLevel)
            {
                case vsBuildErrorLevel.vsBuildErrorLevelHigh:
                    glyphImage = _errorIcon;
                    break;
                case vsBuildErrorLevel.vsBuildErrorLevelMedium:
                    glyphImage = _warningIcon;
                    break;
                case vsBuildErrorLevel.vsBuildErrorLevelLow:
                    glyphImage = _infoIcon;
                    break;
            }

            Image glyphIcon = new Image();
            glyphIcon.Width = 16;
            glyphIcon.Height = 16;
            glyphIcon.Source = glyphImage;

            return glyphIcon;
        }
    }
}
