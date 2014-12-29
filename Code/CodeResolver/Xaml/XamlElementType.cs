namespace Cvte.CodeResolvers.Xaml
{
    /// <summary>
    /// 表示一个 XAML 元素的类型。
    /// </summary>
    public enum XamlElementType
    {
        /// <summary>
        /// 根元素。
        /// </summary>
        RootElement,

        /// <summary>
        /// 子元素。
        /// </summary>
        Element,

        /// <summary>
        /// 用元素形式表示的属性。
        /// </summary>
        Property,

        /// <summary>
        /// 纯文本。
        /// </summary>
        Text,

        /// <summary>
        /// 注释块。
        /// </summary>
        Comment,
    }

    /// <summary>
    /// 表示一个 <see cref="XamlElement"/> 的创建方式。
    /// </summary>
    public enum XamlElementCreateOption
    {
        /// <summary>
        /// 常规创建。
        /// </summary>
        Normal,

        /// <summary>
        /// 作为根元素创建。
        /// </summary>
        Root,

        /// <summary>
        /// 作为纯文本创建。
        /// </summary>
        Text,

        /// <summary>
        /// 作为注释创建。
        /// </summary>
        Comment,
    }
}