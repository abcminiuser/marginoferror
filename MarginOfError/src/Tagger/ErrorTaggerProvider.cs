using System;
using System.ComponentModel.Composition;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace FourWalledCubicle.MarginOfError
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(ErrorGlyphTag))]
    [ContentType("code")]
    public sealed class ErrorTaggerProvider : ITaggerProvider
    {
        [Import]
        internal SVsServiceProvider ServiceProvider { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            var dte = (DTE)ServiceProvider.GetService(typeof(DTE));

            if (buffer == null)
                throw new ArgumentNullException("buffer");

            Func<ITagger<T>> taggerFunc =
                () => new ErrorTagger(dte, buffer) as ITagger<T>;
            return buffer.Properties.GetOrCreateSingletonProperty<ITagger<T>>(taggerFunc);
        }
    }
}
