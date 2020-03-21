using System;
using System.Xml;
using System.Collections.Generic;
using Msdfa.Core.Tools.Extensions;

namespace Msdfa.Core.Tools
{
    public class QuickXML
    {
        XmlDocument xmlDocument = new XmlDocument();
        string documentPath;


        public QuickXML()
        {
        }
        
        public QuickXML(string documentPath)
        {
            this.documentPath = documentPath;
            try { xmlDocument.Load(documentPath); }
            catch { xmlDocument.LoadXml("<settings></settings>"); }
        }

        public void LoadText(string textXML)
        {
            xmlDocument.LoadXml(textXML);
        }
        
        public int GetSetting(string xPath, int defaultValue)
        {
            return Convert.ToInt16(GetSetting(xPath, Convert.ToString(defaultValue)));
        }

        public void PutSetting(string xPath, int value)
        {
            PutSetting(xPath, Convert.ToString(value));
        }

        public string GetSetting(string xPath, string defaultValue = "")
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode != null) { return xmlNode.InnerText; }
            else { return defaultValue; }
        }

        public string GetSettingRequired(string xPath, string defaultValue = "")
        {
            var xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode == null) throw new Exception($"[{xPath}] Brak wymaganego ustawienia w pliku xml");
            return xmlNode.InnerText;
        }

        public XmlNodeList GetChild(string xPath)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode != null) { return xmlNode.ChildNodes; }

            return null;
        }
        public XmlNodeList GetChildList(string xPath)
        {
            var xmlNodeList = xmlDocument.DocumentElement.SelectNodes("/settings/" + xPath);
            return xmlNodeList;
        }

        public string GetSettingBase64(string xPath, string defaultValue = "")
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode != null) { return StringExtension.DecodeFromByte64(xmlNode.InnerText); }
            else { return StringExtension.DecodeFromByte64(defaultValue); }
        }

        public void PutSetting(string xPath, string value)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode == null) { xmlNode = createMissingNode("settings/" + xPath); }
            xmlNode.InnerText = value;
            xmlDocument.Save(documentPath);
        }

        public void PutSettingBase64(string xPath, string value)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode == null) { xmlNode = createMissingNode("settings/" + xPath); }
            xmlNode.InnerText = StringExtension.EncodeToByte64(value);
            xmlDocument.Save(documentPath);
        }

        public List<string> GetChildrenNames()
        {
            List<string> temp = new List<string>();
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings");
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                temp.Add(node.Name);
            }
            return temp;
        }

        public List<string> GetChildrenNames(string xPath)
        {
            List<string> temp = new List<string>();
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                temp.Add(node.Name);
            }
            return temp;
        }

        public void RemoveSetting(string xPath)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);

            if (xmlNode == null)
                return;

            xmlNode.ParentNode.RemoveChild(xmlNode);
            
            xmlDocument.Save(documentPath);
        }

        private XmlNode createMissingNode(string xPath)
        {
            string[] xPathSections = xPath.Split('/');
            string currentXPath = "";
            XmlNode testNode = null;
            XmlNode currentNode = xmlDocument.SelectSingleNode("settings");
            foreach (string xPathSection in xPathSections)
            {
                currentXPath += xPathSection;
                testNode = xmlDocument.SelectSingleNode(currentXPath);
                if (testNode == null)
                {
                    currentNode.InnerXml += "<" +
                                    xPathSection + "></" +
                                    xPathSection + ">";
                }
                currentNode = xmlDocument.SelectSingleNode(currentXPath);
                currentXPath += "/";
            }
            return currentNode;
        }
    }
}