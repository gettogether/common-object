using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using System.Collections;

namespace CommonLibrary.Utility.Xml
{
    public class XmlHelper
    {
        public XmlHelper()
        {
        }

        public static string addTag(string tag, string val)
        {
            if (val.Length > 0)
                return string.Format("<{0}>{1}</{0}>", new object[] { tag, val });
            else
                return string.Format("<{0}/>", new object[] { tag });
        }

        public static XmlDocument GetDoc(string xmlString)
        {
            XmlDocument ret = new XmlDocument();
            ret.LoadXml(xmlString);

            return ret;
        }

        public static XmlNodeList GetNodeList(XmlDocument xmlDoc, string TagName)
        {
            if (xmlDoc != null)
            {
                XmlNodeList nodeList = xmlDoc.GetElementsByTagName(TagName);
                return nodeList;
            }
            return null;
        }

        public static string GetValueFromChild(XmlNode node, string tag)
        {
            XmlElement n = node[tag];

            if (n == null)
                return "";
            else
                return n.InnerText;
        }

        public static string GetValueFromAttribute(XmlNode node, string tag)
        {
            string ret = node.Attributes[tag].InnerText;
            return ret;
        }
        public static string GetValueFromChildNodeAttribute(XmlNode node, string nodename, string att)
        {
            XmlElement n = node[nodename];

            if (n == null)
                return "";
            else
                return n.Attributes[att].InnerText;
        }

        public static string SerializeDataSet(DataSet[] dss)
        {
            string resultXml = "";
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            foreach (DataSet ds in dss)
            {
                if (ds != null)
                    ds.WriteXml(stream);
            }
            resultXml = System.Text.UTF8Encoding.UTF8.GetString(stream.ToArray());

            return resultXml;
        }

        public static string SerializeObj(object obj)
        {
            if (obj == null)
                return "";
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            xs.Serialize(stream, obj);
            string resultXml = System.Text.UTF8Encoding.UTF8.GetString(stream.ToArray());
            return resultXml;
        }

        public static string ProcessString(string localhost, string xmlstring, string xslt, Hashtable htParams, Hashtable htExtObjs)
        {
            System.Xml.Xsl.XslCompiledTransform xsl = new System.Xml.Xsl.XslCompiledTransform();
            xsl.Load(localhost + xslt);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xmlstring);
            StringWriter sw = new StringWriter();
            System.Xml.Xsl.XsltArgumentList arg = new System.Xml.Xsl.XsltArgumentList();
            if (htParams != null)
            {
                foreach (string k in htParams.Keys) arg.AddParam(k, "", htParams[k]);
            }
            if (htExtObjs != null)
            {
                foreach (string k in htExtObjs.Keys) arg.AddExtensionObject(k, htExtObjs[k]);
            }
            xsl.Transform(xmldoc, arg, sw);
            return System.Web.HttpUtility.HtmlDecode(sw.ToString());

        }
    }
}
