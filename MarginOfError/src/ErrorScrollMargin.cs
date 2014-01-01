using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using EnvDTE80;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;

namespace FourWalledCubicle.MarginOfError
{
    [Name(ErrorScrollMargin.MarginName)]
    class ErrorScrollMargin : Canvas, IWpfTextViewMargin
    {
        public const string MarginName = "Error Scrollbar Overview";

        private readonly IWpfTextView _textView;
        private readonly ITagAggregator<ErrorGlyphTag> _errorTagAggregator;

        private bool _isDisposed = false;

        public ErrorScrollMargin(IWpfTextView textView, ErrorScrollMarginFactory factory)
        {
            _textView = textView;

            _errorTagAggregator = factory.TagAggregatorFactoryService.CreateTagAggregator<ErrorGlyphTag>(textView);

            this.ClipToBounds = true;
            this.Width = 6;
            this.Background = Brushes.White;

            _textView.TextBuffer.ChangedLowPriority += (s, e) => { UpdateDisplay(); };
            _textView.ViewportHeightChanged += (s, e) => { UpdateDisplay(); };
            _errorTagAggregator.TagsChanged += (s, e) => { UpdateDisplay(); };
        }

        private void UpdateDisplay()
        {
            this.Dispatcher.BeginInvoke(new Action(Render), DispatcherPriority.Render);
        }

        private void Render()
        {
            if (_textView.IsClosed)
                return;

            int totalLines = _textView.TextSnapshot.LineCount;
            double relLineHeight = _textView.ViewportHeight / totalLines;
            double markerHeight = Math.Max(relLineHeight, 10);
            double currMarkerOffset = (relLineHeight - markerHeight) / 2;

            this.Children.Clear();

            for (int i = 0; i < totalLines; i++)
            {
                var errorTags = _errorTagAggregator.GetTags(_textView.TextSnapshot.GetLineFromLineNumber(i).Extent);

                vsBuildErrorLevel? maxErrorLevel = null;
                string errorMessage = string.Empty;

                foreach (var x in errorTags)
                {
                    if (!maxErrorLevel.HasValue || (maxErrorLevel < x.Tag.ErrorLevel))
                    {
                        maxErrorLevel = x.Tag.ErrorLevel;
                        errorMessage = x.Tag.Description;
                    }
                }

                if (maxErrorLevel.HasValue)
                {
                    Rectangle errorRect = new Rectangle();
                    errorRect.Height = markerHeight;
                    errorRect.Width = this.Width;
                    errorRect.ToolTip = string.Format("Line {0}:\n\n{1}", i, errorMessage);
                    errorRect.Tag = i;
                    errorRect.MouseDown += errorRect_MouseDown;

                    if (maxErrorLevel == vsBuildErrorLevel.vsBuildErrorLevelHigh)
                    {
                        errorRect.Stroke = Brushes.DarkRed;
                        errorRect.Fill = Brushes.Red;
                    }
                    else
                    {
                        errorRect.Stroke = Brushes.Goldenrod;
                        errorRect.Fill = Brushes.Yellow;
                    }

                    Canvas.SetLeft(errorRect, 0);
                    Canvas.SetTop(errorRect, currMarkerOffset);
                    this.Children.Add(errorRect);
                }

                currMarkerOffset += relLineHeight;
            }
        }

        void errorRect_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int currLineNumber = (int)(sender as FrameworkElement).Tag;
            var currLine = _textView.TextSnapshot.GetLineFromLineNumber(currLineNumber);

            _textView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(currLine.Start, 0), EnsureSpanVisibleOptions.AlwaysCenter);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(MarginName);
        }

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                return this.Width;
            }
        }

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return (marginName == ErrorScrollMargin.MarginName) ? (IWpfTextViewMargin)this : null;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }
    }
}
