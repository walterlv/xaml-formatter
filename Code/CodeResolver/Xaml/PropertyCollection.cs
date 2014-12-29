using System.Collections.ObjectModel;

namespace Cvte.CodeResolvers.Xaml
{
    /// <summary>
    /// 表示 XAML 元素属性集合。通常作为一个 <see cref="XamlElement"/> 的属性集合。
    /// </summary>
    public class PropertyCollection : Collection<XamlProperty>
    {
    }
}