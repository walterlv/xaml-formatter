namespace Cvte.Xaml.Presentation
{
    /// <summary>
    /// ConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DebugWindow
    {
        private static DebugWindow _current;

        public static DebugWindow Current
        {
            get
            {
                if (_current == null)
                {
                    DebugWindow window = new DebugWindow();
                    window.Show();
                    _current = window;
                }
                return _current;
            }
        }

        public DebugWindow()
        {
            InitializeComponent();
            _current = this;
        }

        private const int MaxLineCount = 200;

        public void Append(string line)
        {
            DebugLineListBox.Items.Insert(0, line);
            if (DebugLineListBox.Items.Count > MaxLineCount)
            {
                DebugLineListBox.Items.RemoveAt(DebugLineListBox.Items.Count - 1);
            }
        }
    }
}
