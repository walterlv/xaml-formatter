using System.IO;
using System.Windows;
using Cvte.CodeResolvers.Xaml;
using Cvte.Xaml.Properties;

namespace Cvte.Xaml.Presentation
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void XamlFormatButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = Settings.Default.TemplateDefinitionFile;
            FileInfo file = File.Exists(fileName) ? new FileInfo(fileName) : null;
            string codeText = CodeTextBox.Text;
            string result = XamlFormatter.Format(codeText, file);
            CodeTextBox.Text = result;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
