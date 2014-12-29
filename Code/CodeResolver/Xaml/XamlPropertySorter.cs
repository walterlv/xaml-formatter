using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cvte.CodeResolvers.Internel;

namespace Cvte.CodeResolvers.Xaml
{
    /// <summary>
    /// 包含 XAML 元素属性排序的一般方法。
    /// </summary>
    public static class XamlPropertySorter
    {
        /// <summary>
        /// 获取或设置最近被使用的一个有效排序配置文件。
        /// </summary>
        private static SortTemplateFile _sortTemplateFile;

        /// <summary>
        /// 对 XAML 的属性排序。
        /// </summary>
        /// <param name="element">被排序的 <see cref="XamlElement"/> 对象。</param>
        /// <param name="templateFile">用于获取排序方式的配置文件路径。</param>
        /// <returns>返回排序后的二维枚举值。</returns>
        public static XamlProperty[][] Sort(XamlElement element, FileInfo templateFile)
        {
            // 默认情况下一行一个属性。
            if (templateFile == null)
            {
                return SortLineByLine(element);
            }

            // 如果有配置文件，则读取配置文件。
            if (_sortTemplateFile == null || _sortTemplateFile.FullName.Equals(templateFile.FullName))
            {
                _sortTemplateFile = new SortTemplateFile(templateFile);
            }

            // 如果配置文件读取失败，则返回一行一个属性。
            SortTemplate template = _sortTemplateFile.GetTemplate(element.Name);
            if (template == null)
            {
                return SortLineByLine(element);
            }

            // 解析顺序并排序。
            // - 属性的位置字典。
            Dictionary<Position, XamlProperty> xamlPositionDictionary = new Dictionary<Position, XamlProperty>();
            // - 已定义顺序的属性。
            List<XamlProperty[]> sortedProperties = new List<XamlProperty[]>();
            // - 未定义顺序的属性。
            List<XamlProperty> unsortedProperties = new List<XamlProperty>();
            SortedDictionary<string, XamlProperty> xmlnsProperties = new SortedDictionary<string, XamlProperty>();
            // - 给位置字典和未定义顺序的属性列表（字典）赋值。
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
            // - 给已定义顺序的属性列表组赋值。
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
            // - 拼接已定义和未定义顺序的列表组。
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
        /// 返回一行一个属性的排序。
        /// </summary>
        /// <param name="element">被排序的 <see cref="XamlElement"/> 对象。</param>
        /// <returns>返回排序后的二维枚举值。</returns>
        private static XamlProperty[][] SortLineByLine(XamlElement element)
        {
            return element.Properties.Select(property => new[] { property }).ToArray();
        }

        /// <summary>
        /// 获取一个属性（名字）在模板中的位置。如果模板中不存在此属性，则返回 <see cref="Position.NotDefined"/>。
        /// </summary>
        /// <param name="name">属性名。</param>
        /// <param name="template">排序模板。</param>
        /// <returns>属性在排序模板中的位置。</returns>
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
