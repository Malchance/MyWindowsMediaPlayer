using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MyWindowsMediaPlayer.Model
{
    public class FormattedSlider : Slider
    {
        private ToolTip _autoToolTip;
        private string _autoToolTipFormat;
        private ToolTip AutoToolTip
        {
            get
            {
                if (_autoToolTip == null)
                {
                    FieldInfo field = typeof(Slider).GetField("_autoToolTip", BindingFlags.NonPublic | BindingFlags.Instance);
                    _autoToolTip = field.GetValue(this) as ToolTip;
                }
                return (_autoToolTip);
            }
        }

        public string AutoTooltipFormat
        {
            get { return _autoToolTipFormat; }
            set { _autoToolTipFormat = value; }
        }

        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            base.OnThumbDragStarted(e);
            FormatAutoToolTipContent();
        }

        protected override void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            base.OnThumbDragDelta(e);
            this.FormatAutoToolTipContent();
        }

        private void FormatAutoToolTipContent()
        {
            if (string.IsNullOrEmpty(AutoTooltipFormat))
                return;
            object content;
            string text = AutoToolTip.Content as string;
            double number;

            if (double.TryParse(text, out number))
                content = number * 100;
            else
                content = text;
            AutoToolTip.Content = string.Format(AutoTooltipFormat, content);
        }
    }
}
