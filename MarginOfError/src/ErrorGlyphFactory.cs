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
        private const string packURIPrefix = @"pack://application:,,,/MarginOfError;component/";

        private readonly BitmapImage errorIcon = new BitmapImage(new Uri(packURIPrefix + "Resources/ErrorIcon.png"));
        private readonly BitmapImage warningIcon = new BitmapImage(new Uri(packURIPrefix + "Resources/WarningIcon.png"));
        private readonly BitmapImage infoIcon = new BitmapImage(new Uri(packURIPrefix + "Resources/InfoIcon.png"));

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            ErrorGlyphTag errorDetails = tag as ErrorGlyphTag;

            if (errorDetails == null)
                return null;

            BitmapImage glyphImage = null;

            switch (errorDetails.ErrorLevel)
            {
                case vsBuildErrorLevel.vsBuildErrorLevelHigh:
                    glyphImage = errorIcon;
                    break;
                case vsBuildErrorLevel.vsBuildErrorLevelMedium:
                    glyphImage = warningIcon;
                    break;
                case vsBuildErrorLevel.vsBuildErrorLevelLow:
                    glyphImage = infoIcon;
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
