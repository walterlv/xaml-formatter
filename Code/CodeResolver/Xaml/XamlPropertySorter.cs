using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cvte.CodeResolvers.Internel;

namespace Cvte.CodeResolvers.Xaml
{
    /// <summary>
    /// ���� XAML Ԫ�����������һ�㷽����
    /// </summary>
    public static class XamlPropertySorter
    {
        /// <summary>
        /// ��ȡ�����������ʹ�õ�һ����Ч���������ļ���
        /// </summary>
        private static SortTemplateFile _sortTemplateFile;

        /// <summary>
        /// �� XAML ����������
        /// </summary>
        /// <param name="element">������� <see cref="XamlElement"/> ����</param>
        /// <param name="templateFile">���ڻ�ȡ����ʽ�������ļ�·����</param>
        /// <returns>���������Ķ�άö��ֵ��</returns>
        public static XamlProperty[][] Sort(XamlElement element, FileInfo templateFile)
        {
            // Ĭ�������һ��һ�����ԡ�
            if (templateFile == null)
            {
                return SortLineByLine(element);
            }

            // ����������ļ������ȡ�����ļ���
            if (_sortTemplateFile == null || _sortTemplateFile.FullName.Equals(templateFile.FullName))
            {
                _sortTemplateFile = new SortTemplateFile(templateFile);
            }

            // ��������ļ���ȡʧ�ܣ��򷵻�һ��һ�����ԡ�
            SortTemplate template = _sortTemplateFile.GetTemplate(element.Name);
            if (template == null)
            {
                return SortLineByLine(element);
            }

            // ����˳������
            // - ���Ե�λ���ֵ䡣
            Dictionary<Position, XamlProperty> xamlPositionDictionary = new Dictionary<Position, XamlProperty>();
            // - �Ѷ���˳������ԡ�
            List<XamlProperty[]> sortedProperties = new List<XamlProperty[]>();
            // - δ����˳������ԡ�
            List<XamlProperty> unsortedProperties = new List<XamlProperty>();
            SortedDictionary<string, XamlProperty> xmlnsProperties = new SortedDictionary<string, XamlProperty>();
            // - ��λ���ֵ��δ����˳��������б��ֵ䣩��ֵ��
            int xmlnsRow = -1;
            bool isXmlnsAdded = false;
            foreach (XamlProperty property in element.Properties)
            {
                Position position = GetPositionInTemplate(property.FullName, template);
                if (position == Position.NotDefined)
                {
                    unsortedProperties.Add(property);
                }
                else
                {
                    if (("xmlns".Equals(property.Prefix) || "xmlns".Equals(property.Name)))
                    {
                        if (xmlnsRow < 0)
                        {
                            xmlnsRow = position.X;
                        }
                        xmlnsProperties.Add(property.FullName, property);
                        if (!isXmlnsAdded)
                        {
                            isXmlnsAdded = true;
                            xamlPositionDictionary.Add(position, property);
                        }
                    }
                    else
                    {
                        xamlPositionDictionary.Add(position, property);
                    }
                }
            }
            // - ���Ѷ���˳��������б��鸳ֵ��
            isXmlnsAdded = false;
            for (int i = 0; i < template.Properties.Length; i++)
            {
                string[] line = template.Properties[i];
                List<XamlProperty> lineProperties = null;
                for (int j = 0; j < line.Length; j++)
                {
                    Position current = new Position(i, j);
                    if (xamlPositionDictionary.ContainsKey(current))
                    {
                        XamlProperty property = xamlPositionDictionary[current];
                        if (property.FullName.Contains("xmlns"))
                        {
                            if (!isXmlnsAdded)
                            {
                                isXmlnsAdded = true;
                                sortedProperties.AddRange(xmlnsProperties.Select(pair => new[] {pair.Value}));
                            }
                            continue;
                        }
                        if (template.PropertyLengthToReturn > 0 &&
                            property.ToString().Length > template.PropertyLengthToReturn)
                        {
                            sortedProperties.Add(new[] {property});
                        }
                        else
                        {
                            if (lineProperties == null)
                            {
                                lineProperties = new List<XamlProperty>();
                            }
                            lineProperties.Add(property);
                        }
                    }
                }
                if (lineProperties != null)
                {
                    sortedProperties.Add(lineProperties.ToArray());
                }
            }
            // - ƴ���Ѷ����δ����˳����б��顣
            foreach (XamlProperty property in unsortedProperties)
            {
                //if (("xmlns".Equals(property.Prefix) || "xmlns".Equals(property.Name)) && xmlnsRow >= 0)
                //{
                //    if (property.FullName.Equals("xmlns"))
                //    {
                //        sortedProperties.Insert(xmlnsRow, new[] {property});
                //    }
                //    else
                //    {
                //        sortedProperties.Insert(xmlnsRow + 1, new[] { property });
                //    }
                //}
                //else
                //{
                    sortedProperties.Add(new[] {property});
                //}
            }
            return sortedProperties.ToArray();
        }

        /// <summary>
        /// ����һ��һ�����Ե�����
        /// </summary>
        /// <param name="element">������� <see cref="XamlElement"/> ����</param>
        /// <returns>���������Ķ�άö��ֵ��</returns>
        private static XamlProperty[][] SortLineByLine(XamlElement element)
        {
            return element.Properties.Select(property => new[] { property }).ToArray();
        }

        /// <summary>
        /// ��ȡһ�����ԣ����֣���ģ���е�λ�á����ģ���в����ڴ����ԣ��򷵻� <see cref="Position.NotDefined"/>��
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="template">����ģ�塣</param>
        /// <returns>����������ģ���е�λ�á�</returns>
        private static Position GetPositionInTemplate(string name, SortTemplate template)
        {
            for (int i = 0; i < template.Properties.Length; i++)
            {
                string[] line = template.Properties[i];
                for (int j = 0; j < line.Length; j++)
                {
                    if (name.Equals(line[j]))
                    {
                        return new Position(i, j);
                    }
                    if (name.StartsWith("xmlns") && line[j].StartsWith("xmlns"))
                    {
                        return new Position(i, 0);
                    }
                }
            }
            return Position.NotDefined;
        }
    }
}
