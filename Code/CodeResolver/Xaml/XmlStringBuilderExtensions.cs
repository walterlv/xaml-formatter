using System;
using System.Text;

namespace Cvte.CodeResolvers.Xaml
{
    /// <summary>
    /// 包含组合 XAML 格式化字符串的一系列扩展方法。
    /// </summary>
    internal static class XmlStringBuilderExtensions
    {
        /// <summary>
        /// 附加“&lt;<paramref name="name"/>”。
        /// </summary>
        internal static void AppendXmlStart(this StringBuilder builder, string name, int whitespaceCount)
        {
            builder.AppendWhitespaces(whitespaceCount);
            builder.Append(String.Format("<{0}", name));
        }

        /// <summary>
        /// 附加“&gt;”。
        /// </summary>
        internal static void AppendXmlHeadEnd(this StringBuilder builder)
        {
            builder.Append(">");
        }

        /// <summary>
        /// 附加“/&gt;”。
        /// </summary>
        internal static void AppendXmlEnd(this StringBuilder builder)
        {
            builder.Append(" />");
        }

        /// <summary>
        /// 附加“&lt;/<paramref name="name"/>&gt;”。
        /// </summary>
        internal static void AppendXmlEnd(this StringBuilder builder, string name)
        {
            builder.Append(String.Format("</{0}>", name));
        }

        /// <summary>
        /// 附加“&lt;/<paramref name="name"/>&gt;”。
        /// </summary>
        internal static void AppendXmlEnd(this StringBuilder builder, string name, int whitespaceCount)
        {
            builder.AppendWhitespaces(whitespaceCount);
            builder.Append(String.Format("</{0}>", name));
        }

        /// <summary>
        /// 附加“<paramref name="property"/>="<paramref name="value"/>"”。
        /// </summary>
        internal static void AppendXmlAttribute(this StringBuilder builder, string property, string value)
        {
            builder.Append(String.Format(" {0}=\"{1}\"", property, value));
        }

        /// <summary>
        /// 附加“<paramref name="property"/>="<paramref name="value"/>"”。
        /// </summary>
        internal static void AppendXmlAttribute(this StringBuilder builder, string property, string value, int whitespaceCount)
        {
            builder.AppendWhitespaces(whitespaceCount);
            builder.Append(String.Format(" {0}=\"{1}\"", property, value));
        }

        internal static void AppendComment(this StringBuilder builder, string text)
        {
            builder.Append(String.Format("<!-- {0} -->", text));
        }

        internal static void AppendComment(this StringBuilder builder, string text, int whitespaceCount)
        {
            builder.AppendWhitespaces(whitespaceCount);
            builder.Append(String.Format("<!-- {0} -->", text));
        }

        /// <summary>
        /// 附加 <paramref name="count"/> 个空格。
        /// </summary>
        internal static void AppendWhitespaces(this StringBuilder builder, int count)
        {
            for (int i = 0; i < count; i++)
            {
                builder.Append(' ');
            }
        }
    }
}