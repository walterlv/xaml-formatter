using System;

namespace Cvte.CodeResolvers.Xaml
{
    /// <summary>
    /// 表示一个 <see cref="XamlElement"/> 的属性。
    /// </summary>
    public class XamlProperty
    {
        /// <summary>
        /// 获取属性的命名空间前缀。
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 获取属性除命名空间外的名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取属性包括命名空间的完整名称。
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// 获取属性的类型，如是普通属性还是附加属性。
        /// </summary>
        public XamlPropertyType Type { get; private set; }

        /// <summary>
        /// 获取属性的值。
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 根据属性的键值对创建 <see cref="XamlProperty"/> 的实例。
        /// </summary>
        /// <param name="fullName">属性名。</param>
        /// <param name="value">属性值。</param>
        public XamlProperty(string fullName, string value = null)
        {
            // 解析命名空间和名称。
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

            // 解析属性类型。
            Type = Name.Contains(XamlUtil.PropertyPathSeparator)
                ? XamlPropertyType.AttachedProperty
                : XamlPropertyType.Property;

            // 给属性值赋值。
            Value = value;
        }

        /// <summary>
        /// 将属性模拟成一个 xml 表示方式。
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0}=\"{1}\"", FullName, Value);
        }
    }
}
