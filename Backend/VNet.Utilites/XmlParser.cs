using System.Xml;
using System.Xml.Linq;

namespace VNet.Utilites
{
    /// <summary>
    /// XML解析器助手类，专注于XML解析功能
    /// </summary>
    public static class XmlParser
    {
        #region 基本解析方法

        /// <summary>
        /// 从字符串解析XML文档
        /// </summary>
        /// <param name="xmlString">XML字符串</param>
        /// <returns>XDocument对象</returns>
        public static XDocument ParseFromString(string xmlString)
        {
            if (string.IsNullOrWhiteSpace(xmlString))
                throw new ArgumentException("XML字符串不能为空", nameof(xmlString));

            try
            {
                return XDocument.Parse(xmlString);
            }
            catch (XmlException ex)
            {
                throw new XmlException($"解析XML字符串失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 从文件解析XML文档
        /// </summary>
        /// <param name="filePath">XML文件路径</param>
        /// <returns>XDocument对象</returns>
        public static XDocument ParseFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("文件路径不能为空", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"XML文件不存在: {filePath}");

            try
            {
                return XDocument.Load(filePath);
            }
            catch (XmlException ex)
            {
                throw new XmlException($"解析XML文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 从流解析XML文档
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <returns>XDocument对象</returns>
        public static XDocument ParseFromStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            try
            {
                return XDocument.Load(stream);
            }
            catch (XmlException ex)
            {
                throw new XmlException($"从流解析XML失败: {ex.Message}", ex);
            }
        }

        #endregion

        #region 元素解析

        /// <summary>
        /// 获取根元素
        /// </summary>
        /// <param name="document">XML文档</param>
        /// <returns>根元素</returns>
        public static XElement GetRootElement(XDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return document.Root;
        }

        /// <summary>
        /// 根据元素名称查找元素
        /// </summary>
        /// <param name="parent">父元素</param>
        /// <param name="elementName">元素名称</param>
        /// <returns>找到的元素列表</returns>
        public static IEnumerable<XElement> GetElementsByName(XElement parent, string elementName)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            if (string.IsNullOrWhiteSpace(elementName))
                throw new ArgumentException("元素名称不能为空", nameof(elementName));

            return parent.Elements(elementName);
        }

        /// <summary>
        /// 递归查找所有指定名称的元素
        /// </summary>
        /// <param name="parent">父元素</param>
        /// <param name="elementName">元素名称</param>
        /// <returns>找到的元素列表</returns>
        public static IEnumerable<XElement> GetDescendantsByName(XElement parent, string elementName)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            if (string.IsNullOrWhiteSpace(elementName))
                throw new ArgumentException("元素名称不能为空", nameof(elementName));

            return parent.Descendants(elementName);
        }

        /// <summary>
        /// 获取第一个匹配的子元素
        /// </summary>
        /// <param name="parent">父元素</param>
        /// <param name="elementName">元素名称</param>
        /// <returns>找到的元素，不存在返回null</returns>
        public static XElement GetFirstElement(XElement parent, string elementName)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            if (string.IsNullOrWhiteSpace(elementName))
                throw new ArgumentException("元素名称不能为空", nameof(elementName));

            return parent.Element(elementName);
        }

        #endregion

        #region 属性解析

        /// <summary>
        /// 获取元素的属性值
        /// </summary>
        /// <param name="element">XML元素</param>
        /// <param name="attributeName">属性名称</param>
        /// <returns>属性值，不存在返回null</returns>
        public static string GetAttributeValue(XElement element, string attributeName)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentException("属性名称不能为空", nameof(attributeName));

            return element.Attribute(attributeName)?.Value;
        }

        /// <summary>
        /// 获取元素的所有属性
        /// </summary>
        /// <param name="element">XML元素</param>
        /// <returns>属性字典</returns>
        public static Dictionary<string, string> GetAllAttributes(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return element.Attributes()
                         .ToDictionary(attr => attr.Name.LocalName, attr => attr.Value);
        }

        /// <summary>
        /// 检查元素是否包含指定属性
        /// </summary>
        /// <param name="element">XML元素</param>
        /// <param name="attributeName">属性名称</param>
        /// <returns>是否包含该属性</returns>
        public static bool HasAttribute(XElement element, string attributeName)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentException("属性名称不能为空", nameof(attributeName));

            return element.Attribute(attributeName) != null;
        }

        #endregion

        #region 文本内容解析

        /// <summary>
        /// 获取元素的文本内容
        /// </summary>
        /// <param name="element">XML元素</param>
        /// <returns>元素的文本内容</returns>
        public static string GetElementText(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return element.Value;
        }

        /// <summary>
        /// 获取元素的内部XML内容
        /// </summary>
        /// <param name="element">XML元素</param>
        /// <returns>内部XML字符串</returns>
        public static string GetInnerXml(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return string.Concat(element.Nodes().Select(n => n.ToString()));
        }

        /// <summary>
        /// 获取元素及其子元素的完整XML
        /// </summary>
        /// <param name="element">XML元素</param>
        /// <returns>完整的XML字符串</returns>
        public static string GetOuterXml(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return element.ToString();
        }

        #endregion

        #region 类型转换解析

        /// <summary>
        /// 获取元素文本并转换为指定类型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="element">XML元素</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换后的值</returns>
        public static T GetElementValue<T>(XElement element, T defaultValue = default)
        {
            if (element == null)
                return defaultValue;

            try
            {
                var text = element.Value;
                if (string.IsNullOrWhiteSpace(text))
                    return defaultValue;

                return (T)Convert.ChangeType(text, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取属性值并转换为指定类型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="element">XML元素</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换后的值</returns>
        public static T GetAttributeValue<T>(XElement element, string attributeName, T defaultValue = default)
        {
            var attributeValue = GetAttributeValue(element, attributeName);

            if (string.IsNullOrWhiteSpace(attributeValue))
                return defaultValue;

            try
            {
                return (T)Convert.ChangeType(attributeValue, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        #endregion

        #region 验证方法

        /// <summary>
        /// 验证XML字符串格式是否正确
        /// </summary>
        /// <param name="xmlString">XML字符串</param>
        /// <returns>是否为有效XML</returns>
        public static bool IsValidXml(string xmlString)
        {
            if (string.IsNullOrWhiteSpace(xmlString))
                return false;

            try
            {
                XDocument.Parse(xmlString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 验证XML文件格式是否正确
        /// </summary>
        /// <param name="filePath">XML文件路径</param>
        /// <returns>是否为有效XML文件</returns>
        public static bool IsValidXmlFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return false;

            try
            {
                XDocument.Load(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 安全解析方法

        /// <summary>
        /// 安全解析XML（不抛出异常）
        /// </summary>
        /// <param name="xmlString">XML字符串</param>
        /// <param name="document">解析结果</param>
        /// <returns>是否解析成功</returns>
        public static bool TryParseFromString(string xmlString, out XDocument document)
        {
            document = null;

            if (string.IsNullOrWhiteSpace(xmlString))
                return false;

            try
            {
                document = XDocument.Parse(xmlString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 安全从文件解析XML（不抛出异常）
        /// </summary>
        /// <param name="filePath">XML文件路径</param>
        /// <param name="document">解析结果</param>
        /// <returns>是否解析成功</returns>
        public static bool TryParseFromFile(string filePath, out XDocument document)
        {
            document = null;

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return false;

            try
            {
                document = XDocument.Load(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}