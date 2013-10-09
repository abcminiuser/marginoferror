using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace FourWalledCubicle.MarginOfError
{
    internal sealed class ErrorTagInfo
    {
        public string Description { get; set; }
        public vsBuildErrorLevel ErrorLevel { get; set; }
        public SnapshotSpan SpanData { get; set; }
    }

    internal sealed class ErrorTagger : ITagger<ErrorGlyphTag>
    {
        private readonly DTE mDTE = null;
        private readonly ITextBuffer mBuffer = null;
        private readonly Dictionary<int, ErrorTagInfo> mErrors = new Dictionary<int, ErrorTagInfo>();
        private readonly BuildEvents mBuildEvents = null;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public ErrorTagger(DTE dte, ITextBuffer buffer)
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

                        if (mErrors.ContainsKey(e.Line))
                        {
                            ErrorTagInfo errorItem = mErrors[e.Line];
                            errorItem.Description += Environment.NewLine + Environment.NewLine + e.Description;
                            if (errorItem.ErrorLevel < e.ErrorLevel)
                                errorItem.ErrorLevel = e.ErrorLevel;
                        }
                        else
                        {
                            ErrorTagInfo errorItem = new ErrorTagInfo();
                            errorItem.Description = e.Description;
                            errorItem.ErrorLevel = e.ErrorLevel;
                            errorItem.SpanData = new SnapshotSpan(mBuffer.CurrentSnapshot, line.Start, line.Length);

                            mErrors[e.Line] = errorItem;
                        }
                    }
                }
            }
            catch { }

            if (TagsChanged != null)
                TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(mBuffer.CurrentSnapshot, 0, mBuffer.CurrentSnapshot.Length)));
        }

        IEnumerable<ITagSpan<ErrorGlyphTag>> ITagger<ErrorGlyphTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (ErrorTagInfo errorEntry in mErrors.Values)
            {
                TagSpan<ErrorGlyphTag> tagSpan = null;
                
                try
                {
                    tagSpan = new TagSpan<ErrorGlyphTag>(errorEntry.SpanData, new ErrorGlyphTag(errorEntry.Description, errorEntry.ErrorLevel));
                }
                catch { }

                if (tagSpan != null)
                    yield return tagSpan;
            }
        }
    }
}
