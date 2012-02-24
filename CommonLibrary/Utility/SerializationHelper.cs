using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml.Serialization.Advanced;
using System.Collections;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Reflection;
using System.IO;

namespace CommonLibrary.Utility
{
    public class DataTableSchemaImporterExtension : SchemaImporterExtension
    {
        Hashtable importedTypes = new Hashtable();
        public override string ImportSchemaType(string name, string schemaNamespace, XmlSchemaObject context, XmlSchemas schemas, XmlSchemaImporter importer, CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeGenerationOptions options, CodeDomProvider codeProvider)
        {
            IList values = schemas.GetSchemas(schemaNamespace);
            if (values.Count != 1)
            {
                return null;
            }
            XmlSchema schema = values[0] as XmlSchema;
            if (schema == null)
                return null;
            XmlSchemaType type = (XmlSchemaType)schema.SchemaTypes[new XmlQualifiedName(name, schemaNamespace)];
            return ImportSchemaType(type, context, schemas, importer, compileUnit, mainNamespace, options, codeProvider);
        }

        public override string ImportSchemaType(XmlSchemaType type, XmlSchemaObject context, XmlSchemas schemas, XmlSchemaImporter importer, CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeGenerationOptions options, CodeDomProvider codeProvider)
        {
            if (type == null)
            {
                return null;
            }
            if (importedTypes[type] != null)
            {
                mainNamespace.Imports.Add(new CodeNamespaceImport(typeof(DataSet).Namespace));
                compileUnit.ReferencedAssemblies.Add("System.Data.dll");
                return (string)importedTypes[type];
            }
            if (!(context is XmlSchemaElement))
                return null;
            if (type is XmlSchemaComplexType)
            {
                XmlSchemaComplexType ct = (XmlSchemaComplexType)type;
                if (ct.Particle is XmlSchemaSequence)
                {
                    XmlSchemaObjectCollection items = ((XmlSchemaSequence)ct.Particle).Items;
                    if (items.Count == 2 && items[0] is XmlSchemaAny && items[1] is XmlSchemaAny)
                    {
                        XmlSchemaAny any0 = (XmlSchemaAny)items[0];
                        XmlSchemaAny any1 = (XmlSchemaAny)items[1];
                        if (any0.Namespace == XmlSchema.Namespace && any1.Namespace == "urn:schemas-microsoft-com:xml-diffgram-v1")
                        {
                            string typeName = typeof(DataTable).FullName;
                            importedTypes.Add(type, typeName);
                            mainNamespace.Imports.Add(new CodeNamespaceImport(typeof(DataTable).Namespace));
                            compileUnit.ReferencedAssemblies.Add("System.Data.dll");
                            return typeName;
                        }
                    }
                }
            }
            return null;
        }
    }

    public class SerializationHelper
    {
        public static string SerializeToXml<T>(Type objType, T t) where T : class, new()
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            XmlSerializer serializer = new XmlSerializer(objType);
            serializer.Serialize(writer, t);
            writer.Close();
            return sb.ToString();

        }
        public static T DeserializeObject<T>(Type objType, string objXml) where T : class, new()
        {
            System.IO.StringReader strReader = new System.IO.StringReader(objXml);
            XmlReader xmlReader = XmlReader.Create(strReader);
            XmlSerializer serializer = new XmlSerializer(objType);
            return serializer.Deserialize(xmlReader) as T;
        }
        public static PropertyInfo[] GetPropertyInfos<T>() where T : class, new()
        {
            List<PropertyInfo> propertyinfo_list = new List<PropertyInfo>();
            foreach (PropertyInfo pi in typeof(T).GetProperties())
            {
                if (!typeof(IComparable).IsAssignableFrom(pi.PropertyType))
                    continue;

                propertyinfo_list.Add(pi);
            }
            return propertyinfo_list.ToArray();
        }

        public static string SerializeToXml(object obj)
        {
            //XmlSerializer xs = new XmlSerializer(obj.GetType());
            //StringWriter sw = new StringWriter();
            //xs.Serialize(sw, obj);
            //sw.Close();
            //return sw.ToString();
            if (obj == null) return string.Empty;
            System.Xml.Serialization.XmlSerializer xs = new XmlSerializer(obj.GetType());
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            xs.Serialize(stream, obj);
            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }
        public static string SerializeToXml(object obj, string defaultNamespaces, XmlSerializerNamespaces namespaces)
        {
            if (obj == null) return string.Empty;
            System.Xml.Serialization.XmlSerializer xs = new XmlSerializer(obj.GetType(), defaultNamespaces);
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            xs.Serialize(stream, obj, namespaces);
            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }

        public static T FromXmlProcess<T>(string xml) where T : class, new()
        {
            T t = new T();
            XmlSerializer xs = new XmlSerializer(t.GetType());
            StringReader sr = new StringReader(xml);
            object o = xs.Deserialize(sr);
            if (o != null) return (T)o;
            return null;
        }

        public static T FromXml<T>(string xml) where T : class, new()
        {
            T t = null;
            try
            {
                t = FromXmlProcess<T>(xml);
            }
            catch
            {
                xml = StringHelper.FixSerializationString(xml);
                t = FromXmlProcess<T>(xml);
            }
            return t;
        }

        public static T FromXml<T>(string xml, string defaultNamespaces) where T : class, new()
        {
            T t = new T();
            XmlSerializer xs = new XmlSerializer(t.GetType(), defaultNamespaces);
            StringReader sr = new StringReader(xml);
            object o = xs.Deserialize(sr);
            if (o != null) return (T)o;
            return null;
        }

        public static string SerializeDataTableXml(DataTable dt)
        {
            System.Xml.Serialization.XmlSerializer xs = new XmlSerializer(typeof(DataTable));
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            xs.Serialize(stream, dt);
            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
