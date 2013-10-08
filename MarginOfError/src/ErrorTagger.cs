using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace FourWalledCubicle.MarginOfError
{
    internal sealed class ErrorTagger : ITagger<ErrorGlyphTag>
    {
        private DTE mDTE = null;
        private ITextBuffer mBuffer = null;
        private readonly List<Tuple<SnapshotSpan, ErrorItem>> mErrors = new List<Tuple<SnapshotSpan, ErrorItem>>();
        private BuildEvents mBuildEvents = null;

        internal ErrorTagger(DTE dte, ITextBuffer buffer)
        {
            mDTE = dte;
            mBuffer = buffer;

            mBuildEvents = dte.Events.BuildEvents;
            mBuildEvents.OnBuildDone += (e, p) => UpdateErrorList();

            UpdateErrorList();
        }

        void UpdateErrorList()
        {
            mErrors.Clear();

            ErrorItems errorList = (mDTE as EnvDTE80.DTE2).ToolWindows.ErrorList.ErrorItems;

            try
            {
                ITextDocument textDocument = mBuffer.Properties[typeof(ITextDocument)] as ITextDocument;
                for (int i = 1; i <= errorList.Count; i++)
                {
                    ErrorItem e = errorList.Item(i);

                    if (e.FileName.Equals(textDocument.FilePath))
                    {
                        ITextSnapshotLine line = mBuffer.CurrentSnapshot.GetLineFromLineNumber(e.Line - 1);
                        mErrors.Add(new Tuple<SnapshotSpan, ErrorItem>(new SnapshotSpan(mBuffer.CurrentSnapshot, line.Start, line.Length), e));
                    }
                }
            }
            catch { }

            if (TagsChanged != null)
                TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(mBuffer.CurrentSnapshot, new Span(0, mBuffer.CurrentSnapshot.Length))));
        }

        IEnumerable<ITagSpan<ErrorGlyphTag>> ITagger<ErrorGlyphTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (Tuple<SnapshotSpan, ErrorItem> errorEntry in mErrors)
            {
                TagSpan<ErrorGlyphTag> tagSpan = null;
                
                try
                {
                    tagSpan = new TagSpan<ErrorGlyphTag>(errorEntry.Item1, new ErrorGlyphTag(errorEntry.Item2.Description, errorEntry.Item2.ErrorLevel));
                }
                catch { }

                if (tagSpan != null)
                    yield return tagSpan;
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}
