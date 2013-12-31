using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Tagging;
using System.Windows.Threading;
using System.Windows.Shapes;

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
            this.Width = 4;
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
            double markHeight = _textView.ViewportHeight / totalLines;

            this.Children.Clear();

            for (int i = 0; i < totalLines; i++)
            {
                var errorTags = _errorTagAggregator.GetTags(_textView.TextSnapshot.GetLineFromLineNumber(i).Extent);

                EnvDTE80.vsBuildErrorLevel? maxErrorLevel = null;
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
                    errorRect.Height = markHeight;
                    errorRect.Width = this.Width;
                    errorRect.ToolTip = string.Format("Line {0}:\n{1}", i, errorMessage);

                    if (maxErrorLevel == EnvDTE80.vsBuildErrorLevel.vsBuildErrorLevelHigh)
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
                    Canvas.SetTop(errorRect, i * markHeight);
                    this.Children.Add(errorRect);
                }
            }
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
