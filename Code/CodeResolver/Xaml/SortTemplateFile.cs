using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cvte.CodeResolvers.Xaml
{
    internal class SortTemplateFile
    {
        private readonly FileInfo _templateFile;

        internal string FullName { get { return _templateFile.FullName; } }

        internal SortTemplateFile(FileInfo fileInfo)
        {
            _templateFile = fileInfo;
        }

        internal SortTemplate GetTemplate(string sectionName)
        {
            SortTemplate template = GetTemplateInner(_templateFile.FullName, sectionName, false);
            return template ?? GetTemplateInner(_templateFile.FullName, "Default", false);
        }

        /// <summary>
        /// 在此类内部调用，用于递归获取 <see cref="SortTemplate"/> 对象。递归的目的是处理文件外包含。
        /// </summary>
        /// <param name="fileName">解析此文件。</param>
        /// <param name="sectionName">解析此元素名。</param>
        /// <param name="isInRecursion">如果是递归内部调用，则不传入值；如果是首次调用，则传入 true。</param>
        /// <returns>解析得到的 <see cref="SortTemplate"/>。</returns>
        private SortTemplate GetTemplateInner(string fileName, string sectionName, bool isInRecursion = true)
        {
            // 如果配置文件的外包含项出现了循环包含，则直接返回。
            if (isInRecursion)
            {
                if (_templateFile.FullName.Equals(fileName))
                {
                    return null;
                }
            }

            // 读取整个文件，并寻找需要的 section。
            string[] lines = File.ReadAllLines(fileName);
            int startLine = -1, endLine = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(String.Format("[{0}]", sectionName)))
                {
                    startLine = i;
                }
                else if (startLine >= 0)
                {
                    if (lines[i].Contains('[') && lines[i].Contains(']'))
                    {
                        endLine = i;
                        break;
                    }
                }
            }

            // 将需要的 section 包装成字符串数组。
            //  - 如果尾行比首行只多 1，说明此区域并未存有意义的值。
            if (endLine <= startLine + 1 || startLine < 0)
            {
                return null;
            }
            string[] section = new string[endLine - startLine];
            for (int i = 0; i < section.Length; i++)
            {
                section[i] = lines[i + startLine];
            }

            // 对外包含的判断。如果外包含，则递归读取外部文件；如果没有外包含，则直接返回。
            SortTemplate sortTemplate = new SortTemplate(section);
            if (!String.IsNullOrEmpty(sortTemplate.ExternalFile))
            {
                if (File.Exists(sortTemplate.ExternalFile))
                {
                    FileInfo externalFileInfo = new FileInfo(sortTemplate.ExternalFile);
                    return GetTemplateInner(externalFileInfo.FullName, sectionName);
                }
                return null;
            }
            return sortTemplate;
        }
    }

    internal class SortTemplate
    {
        /// <summary>
        /// 获取被排序的元素名。
        /// </summary>
        internal string ElementName { get; private set; }

        /// <summary>
        /// 获取一个整数。元素名长度超过这个整数时，其第一个属性也会换行。
        /// </summary>
        internal int NameLengthToReturn { get; private set; }

        /// <summary>
        /// 获取一个整数。属性长度超过这个整数时，其同一行的下一个属性也会换行。
        /// </summary>
        internal int PropertyLengthToReturn { get; private set; }

        /// <summary>
        /// 获取该排序目标使用的外部引用文件。如果没有引用外部文件，则为 null。
        /// </summary>
        public string ExternalFile { get; private set; }

        /// <summary>
        /// 获取所有属性排序的二维数组。
        /// </summary>
        internal string[][] Properties { get; private set; }

        /// <summary>
        /// 根据配置文件行数组创建排序模板。
        /// </summary>
        /// <param name="sectionLines">项行数组。</param>
        internal SortTemplate(IEnumerable<string> sectionLines)
        {
            List<string[]> properties = new List<string[]>();
            foreach (string line in sectionLines)
            {
                if (line.Contains('[') && line.Contains(']'))
                {
                    string[] words = line.Split(new[] { '=' });
                    if (words.Any())
                    {
                        ElementName = words[0].Trim();
                    }
                }
                else if (line.Contains('='))
                {
                    string[] words = line.Split(new[] {'='});
                    if (words.Length >= 2)
                    {
                        string key = words[0].Trim();
                        string value = words[1].Trim();
                        switch (key)
                        {
                            case "NameLengthToReturn":
                                NameLengthToReturn = ParseInt32(value);
                                break;
                            case "PropertyLengthToReturn":
                                PropertyLengthToReturn = ParseInt32(value);
                                break;
                            case "ExternalFile":
                                ExternalFile = value;
                                break;
                        }
                    }
                }
                else if (line.Contains(','))
                {
                    string[] words = line.Split(new[] {','}).Where(s => s.Trim().Any()).ToArray();
                    if (words.Any())
                    {
                        properties.Add(words.Select(s => s.Trim()).ToArray());
                    }
                }
                else if (line.Contains('#') || line.Contains('/') || line.Contains(';'))
                {
                }
                else
                {
                    properties.Add(new[] {line.Trim()});
                }
            }
            Properties = properties.ToArray();
        }

        /// <summary>
        /// 转换字符串为整数。如果无法转换，则返回 0。
        /// </summary>
        private static int ParseInt32(string s)
        {
            int value;
            Int32.TryParse(s, out value);
            return value;
        }
    }
}
