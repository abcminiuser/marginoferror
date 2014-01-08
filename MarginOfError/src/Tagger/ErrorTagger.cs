using System;
using System.Collections.Generic;
using System.Timers;
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
        private readonly DTE _DTE = null;
        private readonly ITextBuffer _textBuffer = null;
        private readonly ITextDocument _textDocument = null;
        private readonly Dictionary<int, ErrorTagInfo> _errors = new Dictionary<int, ErrorTagInfo>();
        private readonly BuildEvents _buildEvents = null;
        private readonly TaskListEvents _taskListEvents = null;
        private readonly Timer _updateTimer = null;
        private readonly ErrorItems _errorList = null;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public ErrorTagger(DTE dte, ITextBuffer buffer)
        {
            _DTE = dte;
            _textBuffer = buffer;
            _textDocument = _textBuffer.Properties[typeof(ITextDocument)] as ITextDocument;
            _errorList = (_DTE as EnvDTE80.DTE2).ToolWindows.ErrorList.ErrorItems;

            _updateTimer = new Timer(500);
            _updateTimer.AutoReset = false;
            _updateTimer.Elapsed += (s, e) => UpdateErrorList();
            _updateTimer.Start();

            buffer.Changed += (i, j) => { _updateTimer.Stop(); _updateTimer.Start(); };

            _taskListEvents = dte.Events.TaskListEvents;
            _taskListEvents.TaskAdded += (i) => { _updateTimer.Stop(); _updateTimer.Start(); };
            _taskListEvents.TaskRemoved += (i) => { _updateTimer.Stop(); _updateTimer.Start(); };

            _buildEvents = dte.Events.BuildEvents;
            _buildEvents.OnBuildDone += (e, p) => { _updateTimer.Stop(); _updateTimer.Start(); };        
        }

        void UpdateErrorList()
        {
            _errors.Clear();

            try
            {
                for (int i = 1; i <= _errorList.Count; i++)
                {
                    ErrorItem e = _errorList.Item(i);

                    if (e.FileName.Equals(_textDocument.FilePath))
                    {
                        ITextSnapshotLine line = _textBuffer.CurrentSnapshot.GetLineFromLineNumber(e.Line - 1);

                        if (_errors.ContainsKey(e.Line))
                        {
                            ErrorTagInfo errorItem = _errors[e.Line];
                            errorItem.Description += Environment.NewLine + Environment.NewLine + e.Description;
                            if (errorItem.ErrorLevel < e.ErrorLevel)
                                errorItem.ErrorLevel = e.ErrorLevel;
                        }
                        else
                        {
                            ErrorTagInfo errorItem = new ErrorTagInfo();
                            errorItem.Description = e.Description;
                            errorItem.ErrorLevel = e.ErrorLevel;
                            errorItem.SpanData = new SnapshotSpan(_textBuffer.CurrentSnapshot, line.Start, line.Length);

                            _errors[e.Line] = errorItem;
                        }
                    }
                }
            }
            catch { }

            if (TagsChanged != null)
                TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(_textBuffer.CurrentSnapshot, 0, _textBuffer.CurrentSnapshot.Length)));
        }

        IEnumerable<ITagSpan<ErrorGlyphTag>> ITagger<ErrorGlyphTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (ErrorTagInfo errorEntry in _errors.Values)
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
