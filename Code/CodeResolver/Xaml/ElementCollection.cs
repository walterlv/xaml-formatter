using System.Collections.ObjectModel;

namespace Cvte.CodeResolvers.Xaml
{
    /// <summary>
    /// 表示 XAML 元素集合。通常作为一个 <see cref="XamlElement"/> 的子元素集合。
    /// </summary>
    public class ElementCollection : Collection<XamlElement>
    {
    }
}