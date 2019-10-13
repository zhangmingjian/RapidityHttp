using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Rapidity.Http.Serializes
{
    public class XmlContentSerializer : IXmlContentSerializer
    {
        private readonly XmlSerializeOption _option;

        public XmlContentSerializer() : this(new XmlSerializeOption()) { }

        public XmlContentSerializer(XmlSerializeOption option)
        {
            _option = option;
        }

        public virtual string Serialize(object obj)
        {
            var serializer = new XmlSerializer(obj.GetType(), new XmlAttributeOverrides());
            var stream = new MemoryStream();
            using (var xmlWriter = _option.XmlWriterSettings != null
                ? XmlWriter.Create(stream, _option.XmlWriterSettings)
                : XmlWriter.Create(stream))
            {
                var nameSpace = new XmlSerializerNamespaces(_option.XmlQualifiedNames.ToArray());
                serializer.Serialize(xmlWriter, obj, nameSpace);
            }
            stream.Seek(0, SeekOrigin.Begin);
            using (StreamReader streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        public virtual object Deserialize(string xml, Type type)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(type, new XmlAttributeOverrides());
            using (var reader = new StringReader(xml))
            using (var xmlReader = _option.XmlReaderSettings != null
                ? XmlReader.Create(reader, _option.XmlReaderSettings)
                : XmlReader.Create(reader))
            {
                return xmlSerializer.Deserialize(xmlReader);
            }
        }
    }

    public class XmlSerializeOption
    {
        public IList<XmlQualifiedName> XmlQualifiedNames { get; set; } = new List<XmlQualifiedName>();

        public XmlReaderSettings XmlReaderSettings { get; set; }

        public XmlWriterSettings XmlWriterSettings { get; set; }

    }
}