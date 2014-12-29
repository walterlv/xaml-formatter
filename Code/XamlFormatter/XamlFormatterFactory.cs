using System.ComponentModel.Composition;
using Cvte.Xaml.Presentation;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Cvte.Xaml
{

    #region XamlFormatter Factory

    /// <summary>
    /// 导出一个 <see cref="IWpfTextViewMarginProvider"/>，以获取文本编辑器可供使用的一个页边区实例。
    /// </summary>
    [Export(typeof (IWpfTextViewMarginProvider))]
    [Name(EntranceBar.MarginName)] // 指定实例的名字。
    [Order(After = PredefinedMarginNames.HorizontalScrollBar)] // 使实例显示到水平滚动条以下。
    [MarginContainer(PredefinedMarginNames.Bottom)] // 使实例显示在页边区的底部。
    [ContentType("text")] // 为所有文本类型的控件添加此实例。
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class MarginFactory : IWpfTextViewMarginProvider
    {
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost textViewHost, IWpfTextViewMargin containerMargin)
        {
            if (CurrentDTE == null)
            {
                CurrentDTE = (DTE) ServiceProvider.GetService(typeof (DTE));
            }
            if (textViewHost.TextView.TextSnapshot.ContentType.TypeName.Equals("XAML"))
            {
                return new EntranceBar(textViewHost.TextView);
            }
            return null;
        }

        [Import] internal SVsServiceProvider ServiceProvider = null;

        internal static DTE CurrentDTE { get; private set; }
    }

    #endregion
}
