using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cvte.CodeResolvers.Xaml
{
    /// <summary>
    /// 包含 XAML 格式化的一般方法。
    /// </summary>
    public static class XamlFormatter
    {
        /// <summary>
        /// 格式化一个 XAML 文本。
        /// </summary>
        /// <param name="source">被格式化的 XAML 源。</param>
        /// <param name="templateFile">用于获取排序方式的配置文件路径。</param>
        /// <returns>返回被格式化后的新字符串。</returns>
        public static string Format(string source, FileInfo templateFile)
        {
            XamlDocument document = XamlResolver.Resolve(source);
            string formattedCode = Format(document, templateFile);
            return formattedCode;
        }

        /// <summary>
        /// 格式化一个 <see cref="XamlDocument"/>。
        /// </summary>
        /// <param name="document">被格式化的 XAML 源。</param>
        /// <param name="templateFile">用于获取排序方式的配置文件路径。</param>
        /// <returns>返回被格式化后的新字符串。</returns>
        private static string Format(XamlDocument document, FileInfo templateFile)
        {
            StringBuilder builder = new StringBuilder();
            BuildFormattedString(templateFile, builder, document.RootElement, 0);
            return builder.ToString();
        }

        /// <summary>
        /// 根据 <see cref="XamlDocument"/> 递归建立格式化的 XAML 文本。
        /// </summary>
        /// <param name="templateFile">用于获取排序方式的配置文件路径。</param>
        /// <param name="builder"><see cref="StringBuilder"/>对象，包含当前 Append 的进度。</param>
        /// <param name="element">当前递归到的 <see cref="XamlElement"/> 节点。</param>
        /// <param name="padElement">当前递归到的节点需要缩进的字符数。</param>
        private static void BuildFormattedString(FileInfo templateFile, StringBuilder builder,
            XamlElement element, int padElement)
        {
            // 将元素名写入目标字符串。
            if (element.Type != XamlElementType.RootElement)
            {
                builder.AppendLine();
            }
            builder.AppendXmlStart(element.FullName, padElement);

            // 计算元素和属性的缩进值。(数字“1”是“<”符号所占的空间。）
            int padAttribute = padElement + element.FullName.Length + 1;
            padElement += XamlUtil.TabSpaceCount;

            // 排序属性。（此处使用 var 是因为可能返回任何形式的枚举。）
            var sortedProperties = XamlPropertySorter.Sort(element, templateFile);

            // 将属性写入目标字符串。
            bool isFirstLine = true;
            bool isMultiLine = false;
            foreach (IEnumerable<XamlProperty> line in sortedProperties)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                }
                else
                {
                    isMultiLine = true;
                    builder.AppendLine();
                    builder.AppendWhitespaces(padAttribute);
                }
                foreach (XamlProperty attribute in line)
                {
                    builder.AppendXmlAttribute(attribute.FullName, attribute.Value);
                }
            }

            // 将子元素写入目标字符串。
            if (element.InnerElements.Any())
            {
                builder.AppendXmlHeadEnd();
                bool isInnerElementMultiLine = true;
                foreach (XamlElement innerElement in element.InnerElements)
                {
                    if (innerElement.Type == XamlElementType.Comment)
                    {
                        // 写入注释元素。
                        builder.AppendLine().AppendLine();
                        builder.AppendComment(innerElement.Value, padElement);
                    }
                    else if (innerElement.Type == XamlElementType.Text)
                    {
                        // 写入文本元素。
                        if (isMultiLine)
                        {
                            builder.AppendLine();
                            builder.AppendWhitespaces(padElement);
                            builder.Append(innerElement.Value);
                        }
                        else
                        {
                            isInnerElementMultiLine = false;
                            builder.Append(innerElement.Value);
                        }
                    }
                    else
                    {
                        // 写入普通元素。
                        BuildFormattedString(templateFile, builder, innerElement, padElement);
                    }
                }
                // 写入结束符。
                if (isInnerElementMultiLine)
                {
                    builder.AppendLine();
                    builder.AppendXmlEnd(element.FullName, padElement - XamlUtil.TabSpaceCount);
                }
                else
                {
                    builder.AppendXmlEnd(element.FullName);
                }
            }
            else
            {
                builder.AppendXmlEnd();
            }
        }
    }
}
