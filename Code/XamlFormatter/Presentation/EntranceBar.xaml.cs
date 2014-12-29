using System;
using System.IO;
using System.IO.Packaging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Cvte.CodeResolvers.Xaml;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Cvte.Xaml.Presentation
{
    /// <summary>
    /// EntranceBar.xaml 的交互逻辑
    /// </summary>
    public partial class EntranceBar
    {
        public const string MarginName = "XamlFormatter";
        private readonly IWpfTextView _textView;
        private Storyboard _successStoryboard;
        private Storyboard _errorStoryboard;
        private const string ReadyText = "准备就绪";

        public EntranceBar(IWpfTextView textView)
        {
            InitializeComponent();
            _textView = textView;
        }

        private Storyboard SuccessStoryboard
        {
            get { return _successStoryboard ?? (_successStoryboard = (Storyboard)FindResource("DisplaySuccessStoryboard")); }
        }

        private Storyboard ErrorStoryboard
        {
            get { return _errorStoryboard ?? (_errorStoryboard = (Storyboard)FindResource("DisplayErrorStoryboard")); }
        }

        private void ShowSuccess(string message)
        {
            SuccessStoryboard.Begin();
            MessageButton.Content = message;
        }

        private void ShowError(string message)
        {
            ErrorStoryboard.Begin();
            MessageButton.Content = message;
        }

        private void DisplayStoryboard_Completed(object sender, EventArgs e)
        {
            MessageButton.Content = ReadyText;
        }

        private async void XamlFormatButton_Click(object sender, RoutedEventArgs e)
        {
            XamlFormatButton.IsEnabled = false;
            ITextEdit editor = null;
            try
            {
                // 获取编辑器文本。
                string codeText = _textView.TextSnapshot.GetText();

                // 格式化文本。
                FileInfo templateFile = GetTemplateFile();
                string formattedCode = await FormatCode(codeText, templateFile.Exists ? templateFile : null);

                if (codeText.Equals(formattedCode))
                {
                    // 通知完成。
                    ShowSuccess("此 XAML 已经符合预定的格式，无需格式化。");
                }
                else
                {
                    // 将格式化的文本插入。
                    editor = _textView.TextViewModel.DataModel.DocumentBuffer.CreateEdit();
                    editor.Delete(0, codeText.Length);
                    editor.Insert(0, formattedCode);
                    editor.Apply();

                    // 通知完成。
                    ShowSuccess(String.Format("已使用 {0} 格式化 XAML。",
                        templateFile.Exists ? templateFile.Name : "默认模板"));
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                if (editor != null)
                {
                    editor.Dispose();
                }
            }
            XamlFormatButton.IsEnabled = true;
        }

        private void EntranceBar_Loaded(object sender, RoutedEventArgs e)
        {
            FileInfo file = GetTemplateFile();
            UpdateTemplateFileName(file.Exists ? file.Name : null);
        }

        private void TemplateButton_Click(object sender, RoutedEventArgs e)
        {
            FileInfo file = GetTemplateFile();
            if (!file.Exists)
            {
                using (File.CreateText(file.FullName))
                {
                }
            }
            System.Diagnostics.Process.Start(file.FullName);
            UpdateTemplateFileName(file.Name);
        }

        private FileInfo GetTemplateFile()
        {
            DTE dte = MarginFactory.CurrentDTE;
            FileInfo solutionFile = new FileInfo(dte.Solution.FullName);
            FileInfo templateFile = new FileInfo(solutionFile.FullName + ".xaft");
            return templateFile;
        }

        private async Task<string> FormatCode(string source, FileInfo templateFile)
        {
            Task<string> task = new Task<string>(() =>
                XamlFormatter.Format(source, templateFile));
            task.Start();
            return await task;
        }

        private void UpdateTemplateFileName(string fileName)
        {
            TemplateButton.Content = fileName ?? "未发现 XAML 格式化模板";
        }
    }
}
