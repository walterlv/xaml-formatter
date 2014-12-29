using System;

namespace Cvte.CodeResolvers.Xaml
{
    /// <summary>
    /// 表示一个 XAML 的元素。
    /// </summary>
    public class XamlElement
    {
        /// <summary>
        /// 获取元素的命名空间前缀。
        /// </summary>
        public string Prefix { get; private set; }

        /// <summary>
        /// 获取元素的除命名空间外的名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取元素包括命名空间的完整名称。
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// 获取元素的值。通常是纯文本或注释内文本。
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// 获取元素的类型，如是元素还是属性。
        /// </summary>
        public XamlElementType Type { get; private set; }

        /// <summary>
        /// 获取元素的属性集合。
        /// </summary>
        public PropertyCollection Properties { get; private set; }

        /// <summary>
        /// 获取元素的子元素集合。
        /// </summary>
        public ElementCollection InnerElements { get; private set; }


        /// <summary>
        /// 使用指定的创建方式创建 XAML 元素。
        /// </summary>
        /// <param name="fullName">XAML 元素名。</param>
        /// <param name="option">元素创建方式。</param>
        public XamlElement(string fullName, XamlElementCreateOption option = XamlElementCreateOption.Normal)
        {
            // 创建文本类型的元素。
            if (option == XamlElementCreateOption.Text)
            {
                Type = XamlElementType.Text;
                FullName = fullName;
                Value = Name = fullName.Trim();
            }
            else if (option == XamlElementCreateOption.Comment)
            {
                Type = XamlElementType.Comment;
                Value = Name = FullName = fullName.Trim();
            }
            else
            {
                // 解析名称和命名空间。
                if (fullName.Contains(XamlUtil.NamespaceSeparator))
                {
                    string[] parts = fullName.Split(XamlUtil.NamespaceSeparateChars);
                    Prefix = parts[0];
                    Name = parts[1];
                }
                else
                {
                    Prefix = null;
                    Name = fullName;
                }
                FullName = fullName;

                // 解析元素类型。
                if (option == XamlElementCreateOption.Root)
                {
                    Type = XamlElementType.RootElement;
                }
                else if (Name.Contains(XamlUtil.PropertyPathSeparator))
                {
                    Type = XamlElementType.Property;
                }
                else
                {
                    Type = XamlElementType.Element;
                }
            }

            // 初始化属性和子元素集合。
            Properties = new PropertyCollection();
            InnerElements = new ElementCollection();
        }

        /// <summary>
        /// 将元素模拟成一个 xml 表示方式。
        /// </summary>
        public override string ToString()
        {
            return String.Format("<{0} PropertyCount={1}><InnerElement Count={2}/></{0}>",
                FullName,
                Properties.Count,
                InnerElements.Count);
        }
    }
}
