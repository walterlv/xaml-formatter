using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Cvte.CodeResolvers.Xaml
{
    /// <summary>
    /// 包含 XAML 解析的一般方法。
    /// </summary>
    public static class XamlResolver
    {
        public static XamlDocument Resolve(string xaml)
        {
            // 以 xml 的形式加载 xaml。
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xaml);
            MemoryStream stream = new MemoryStream();
            xmlDocument.Save(stream);
            stream.Position = 0;

            // 准备读取 xml，生成根节点。
            XmlTextReader reader = new XmlTextReader(stream);
            reader.Read();
            XamlElement rootElement = new XamlElement(reader.Name, XamlElementCreateOption.Root);
            ReadAttributes(reader, rootElement);

            // 循环利用中间变量。
            Stack<XamlElement> elementStack = new Stack<XamlElement>();
            elementStack.Push(rootElement);

            // 遍历读取 xml，并翻译成 XamlDocument。
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        RectifyElementStack(elementStack, reader.Depth);
                        XamlElement tempElement = new XamlElement(reader.Name);
                        ReadAttributes(reader, tempElement);
                        XamlElement parentElement = elementStack.Peek();
                        parentElement.InnerElements.Add(tempElement);
                        elementStack.Push(tempElement);
                        break;
                    case XmlNodeType.Comment:
                        RectifyElementStack(elementStack, reader.Depth);
                        XamlElement commentElement = new XamlElement(reader.Value, XamlElementCreateOption.Comment);
                        XamlElement commentParentElement = elementStack.Peek();
                        commentParentElement.InnerElements.Add(commentElement);
                        break;
                    case XmlNodeType.Text:
                        XamlElement textHostElement = elementStack.Peek();
                        textHostElement.InnerElements.Add(new XamlElement(reader.Value, XamlElementCreateOption.Text));
                        break;
                    case XmlNodeType.EndElement:
                        elementStack.Pop();
                        break;
                }
            }

            // 结束并返回。
            reader.Dispose();
            return new XamlDocument(rootElement);
        }

        /// <summary>
        /// 从一个 xml 中读取全部的特性并添加到 <see cref="XamlElement"/> 的属性集合中。
        /// </summary>
        /// <param name="reader">xml 当前的读取元素。</param>
        /// <param name="element">xaml 当前的生成元素。</param>
        private static void ReadAttributes(XmlTextReader reader, XamlElement element)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                XamlProperty property = new XamlProperty(reader.Name, reader.Value);
                element.Properties.Add(property);
            }
        }

        /// <summary>
        /// 为了解决 &lt;Element Property="Value"/&gt; 这种元素在读取时不能判断结尾的问题，需要修正遍历栈。
        /// </summary>
        /// <param name="elementStack">被修正的遍历栈。</param>
        /// <param name="depth">遍历深度。</param>
        /// <exception cref="InvalidDataException">只有在 xml 文件出现元素开始和结尾不匹配时才会抛出此异常。</exception>
        private static void RectifyElementStack(Stack<XamlElement> elementStack, int depth)
        {
            if (elementStack.Count == depth) return;
            if (elementStack.Count > depth)
            {
                for (int i = 0; i < elementStack.Count - depth; i++)
                {
                    elementStack.Pop();
                }
                return;
            }
            throw new InvalidDataException("Xaml resolved error.");
        }
    }
}
