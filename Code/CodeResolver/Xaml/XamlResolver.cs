using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Cvte.CodeResolvers.Xaml
{
    /// <summary>
    /// ���� XAML ������һ�㷽����
    /// </summary>
    public static class XamlResolver
    {
        public static XamlDocument Resolve(string xaml)
        {
            // �� xml ����ʽ���� xaml��
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xaml);
            MemoryStream stream = new MemoryStream();
            xmlDocument.Save(stream);
            stream.Position = 0;

            // ׼����ȡ xml�����ɸ��ڵ㡣
            XmlTextReader reader = new XmlTextReader(stream);
            reader.Read();
            XamlElement rootElement = new XamlElement(reader.Name, XamlElementCreateOption.Root);
            ReadAttributes(reader, rootElement);

            // ѭ�������м������
            Stack<XamlElement> elementStack = new Stack<XamlElement>();
            elementStack.Push(rootElement);

            // ������ȡ xml��������� XamlDocument��
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

            // ���������ء�
            reader.Dispose();
            return new XamlDocument(rootElement);
        }

        /// <summary>
        /// ��һ�� xml �ж�ȡȫ�������Բ���ӵ� <see cref="XamlElement"/> �����Լ����С�
        /// </summary>
        /// <param name="reader">xml ��ǰ�Ķ�ȡԪ�ء�</param>
        /// <param name="element">xaml ��ǰ������Ԫ�ء�</param>
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
        /// Ϊ�˽�� &lt;Element Property="Value"/&gt; ����Ԫ���ڶ�ȡʱ�����жϽ�β�����⣬��Ҫ��������ջ��
        /// </summary>
        /// <param name="elementStack">�������ı���ջ��</param>
        /// <param name="depth">������ȡ�</param>
        /// <exception cref="InvalidDataException">ֻ���� xml �ļ�����Ԫ�ؿ�ʼ�ͽ�β��ƥ��ʱ�Ż��׳����쳣��</exception>
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
