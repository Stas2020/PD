using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;

using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace PDiscountCard.FayRetail
{
    public static class XMLSerializer
    {

        public static ResponseData ResponseDeSerializer(string Data)
        {
            ResponseData res = new ResponseData();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ResponseData));
            using (TextReader reader = new StringReader(Data))
            {
                res = (ResponseData)xmlSerializer.Deserialize(reader);
            }
            return res;
        }

        public static String RequestSerializer(RequestData Data)
        {

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(RequestData));
          // XmlWriter Xw = new XmlWriter();
            StringWriter stringWriter = new StringWriter();
            xmlSerializer.Serialize( stringWriter, Data);
            string serializedXML = stringWriter.ToString();
            XDocument xdoc = XDocument.Parse(serializedXML);
            //XDocument xNewDoc = new XDocument();
            //xNewDoc.Add(xdoc.Root);
            //xNewDoc.Root.RemoveNodes();
            if (xdoc.Root.Element("Calculates") != null)
            {
                if (xdoc.Root.Element("Calculates").Element("CalculateRequest") != null)
                {
                    xdoc.Root.Element("Calculates").Element("CalculateRequest").OrderElements("Cheque", "Card", "Pays");
                }
            }
            if (xdoc.Root.Element("Discounts") != null)
            {
                if (xdoc.Root.Element("Discounts").Element("DiscountRequest") != null)
                {
                    xdoc.Root.Element("Discounts").Element("DiscountRequest").OrderElements("Cheque", "Card", "Pays");
                }
            }
            if (xdoc.Root.Element("Payments") != null)
            {
                if (xdoc.Root.Element("Payments").Element("PaymentRequest") != null)
                {
                    xdoc.Root.Element("Payments").Element("PaymentRequest").OrderElements("Cheque", "Card", "Pays");
                }
            }
            return xdoc.Root.ToString();

        }
        public static bool IsCommandType(this Type type)
        {
            if (type == null || type == typeof(string))
                return false;
            return typeof(Command).IsAssignableFrom(type);
        }


    }
    public static class XElementExtensions
    {
        public static void OrderElements(this XElement parent, params string[] orderedLocalNames)
        {
            List<string> order = new List<string>(orderedLocalNames);
            var orderedNodes = parent.Elements().OrderBy(e => order.IndexOf(e.Name.LocalName) >= 0 ? order.IndexOf(e.Name.LocalName) : Int32.MaxValue);
            int i = order.IndexOf(parent.Elements().Last().Name.LocalName);
            parent.ReplaceNodes(orderedNodes);
        }
    }

  
}
