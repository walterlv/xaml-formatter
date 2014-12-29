namespace Cvte.CodeResolvers.Xaml
{
    /// <summary>
    /// 表示一个 XAML 文档结构。
    /// </summary>
    public class XamlDocument
    {
        /// <summary>
        /// 获取 XAML 文档的根元素。
        /// </summary>
        public XamlElement RootElement { get; private set; }

        /// <summary>
        /// 使用 XAML 根元素创建 <see cref="XamlDocument"/> 实例。
        /// </summary>
        /// <param name="rootElement">XAML 文档根元素。</param>
        public XamlDocument(XamlElement rootElement)
        {
            RootElement = rootElement;
        }
    }
}