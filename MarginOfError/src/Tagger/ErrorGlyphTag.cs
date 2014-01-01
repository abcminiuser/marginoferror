using EnvDTE80;
using Microsoft.VisualStudio.Text.Editor;

namespace FourWalledCubicle.MarginOfError
{
    internal class ErrorGlyphTag : IGlyphTag
    {
        public string Description { get; private set;}
        public vsBuildErrorLevel ErrorLevel { get; private set; }

        public ErrorGlyphTag(string errorDescription, vsBuildErrorLevel errorLevel)
        {
            Description = errorDescription;
            ErrorLevel = errorLevel;
        }
    }
}
