using System;
using System.Collections.Generic;
using System.Text;
/*
namespace PDiscountCard
{


    public class XmlSerializationWriterCardMooverInfo : System.Xml.Serialization.XmlSerializationWriter {

        public void Write3_CardMooverInfo(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"CardMooverInfo", @"");
                return;
            }
            TopLevelElement();
            Write2_CardMooverInfo(@"CardMooverInfo", @"", ((global::PDiscountCard.CardMooverInfo)o), true, false);
        }

        void Write2_CardMooverInfo(string n, string ns, global::PDiscountCard.CardMooverInfo o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::PDiscountCard.CardMooverInfo)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"CardMooverInfo", @"");
            WriteElementString(@"Prefix", @"", ((global::System.String)o.@Prefix));
            WriteElementString(@"CardNum", @"", ((global::System.String)o.@CardNum));
            WriteElementStringRaw(@"CodSh", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@CodSh)));
            WriteElementString(@"CheckNum", @"", ((global::System.String)o.@CheckNum));
            WriteElementStringRaw(@"TermNum", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@TermNum)));
            WriteElementStringRaw(@"CDT", @"", FromDateTime(((global::System.DateTime)o.@CDT)));
            WriteEndElement(o);
        }

        protected override void InitCallbacks() {
        }
    }

    public class XmlSerializationReaderCardMooverInfo : System.Xml.Serialization.XmlSerializationReader {

        public object Read3_CardMooverInfo() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id1_CardMooverInfo && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read2_CardMooverInfo(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":CardMooverInfo");
            }
            return (object)o;
        }

        global::PDiscountCard.CardMooverInfo Read2_CardMooverInfo(bool isNullable, bool checkType) {
            System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType) {
            if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id1_CardMooverInfo && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
            }
            else
                throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::PDiscountCard.CardMooverInfo o;
            o = new global::PDiscountCard.CardMooverInfo();
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute()) {
                if (!IsXmlnsAttribute(Reader.Name)) {
                    UnknownNode((object)o);
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement) {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations0 = 0;
            int readerCount0 = ReaderCount;
            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                    if (!paramsRead[0] && ((object) Reader.LocalName == (object)id3_Prefix && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@Prefix = Reader.ReadElementString();
                        }
                        paramsRead[0] = true;
                    }
                    else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id4_CardNum && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@CardNum = Reader.ReadElementString();
                        }
                        paramsRead[1] = true;
                    }
                    else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id5_CodSh && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@CodSh = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                        }
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[3] && ((object) Reader.LocalName == (object)id6_CheckNum && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@CheckNum = Reader.ReadElementString();
                        }
                        paramsRead[3] = true;
                    }
                    else if (!paramsRead[4] && ((object) Reader.LocalName == (object)id7_TermNum && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@TermNum = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                        }
                        paramsRead[4] = true;
                    }
                    else if (!paramsRead[5] && ((object) Reader.LocalName == (object)id8_CDT && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@CDT = ToDateTime(Reader.ReadElementString());
                        }
                        paramsRead[5] = true;
                    }
                    else {
                        UnknownNode((object)o, @":Prefix, :CardNum, :CodSh, :CheckNum, :TermNum, :CDT");
                    }
                }
                else {
                    UnknownNode((object)o, @":Prefix, :CardNum, :CodSh, :CheckNum, :TermNum, :CDT");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations0, ref readerCount0);
            }
            ReadEndElement();
            return o;
        }

        protected override void InitCallbacks() {
        }

        string id5_CodSh;
        string id6_CheckNum;
        string id1_CardMooverInfo;
        string id4_CardNum;
        string id3_Prefix;
        string id8_CDT;
        string id2_Item;
        string id7_TermNum;

        protected override void InitIDs() {
            id5_CodSh = Reader.NameTable.Add(@"CodSh");
            id6_CheckNum = Reader.NameTable.Add(@"CheckNum");
            id1_CardMooverInfo = Reader.NameTable.Add(@"CardMooverInfo");
            id4_CardNum = Reader.NameTable.Add(@"CardNum");
            id3_Prefix = Reader.NameTable.Add(@"Prefix");
            id8_CDT = Reader.NameTable.Add(@"CDT");
            id2_Item = Reader.NameTable.Add(@"");
            id7_TermNum = Reader.NameTable.Add(@"TermNum");
        }
    }

    public abstract class XmlSerializer1 : System.Xml.Serialization.XmlSerializer {
        protected override System.Xml.Serialization.XmlSerializationReader CreateReader() {
            return new XmlSerializationReaderCardMooverInfo();
        }
        protected override System.Xml.Serialization.XmlSerializationWriter CreateWriter() {
            return new XmlSerializationWriterCardMooverInfo();
        }
    }

    public sealed class CardMooverInfoSerializer : XmlSerializer1 {

        public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
            return xmlReader.IsStartElement(@"CardMooverInfo", @"");
        }

        protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
            ((XmlSerializationWriterCardMooverInfo)writer).Write3_CardMooverInfo(objectToSerialize);
        }

        protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
            return ((XmlSerializationReaderCardMooverInfo)reader).Read3_CardMooverInfo();
        }
    }

    public class XmlSerializerContract : global::System.Xml.Serialization.XmlSerializerImplementation {
        public override global::System.Xml.Serialization.XmlSerializationReader Reader { get { return new XmlSerializationReaderCardMooverInfo(); } }
        public override global::System.Xml.Serialization.XmlSerializationWriter Writer { get { return new XmlSerializationWriterCardMooverInfo(); } }
        System.Collections.Hashtable readMethods = null;
        public override System.Collections.Hashtable ReadMethods {
            get {
                if (readMethods == null) {
                    System.Collections.Hashtable _tmp = new System.Collections.Hashtable();
                    _tmp[@"PDiscountCard.CardMooverInfo::"] = @"Read3_CardMooverInfo";
                    if (readMethods == null) readMethods = _tmp;
                }
                return readMethods;
            }
        }
        System.Collections.Hashtable writeMethods = null;
        public override System.Collections.Hashtable WriteMethods {
            get {
                if (writeMethods == null) {
                    System.Collections.Hashtable _tmp = new System.Collections.Hashtable();
                    _tmp[@"PDiscountCard.CardMooverInfo::"] = @"Write3_CardMooverInfo";
                    if (writeMethods == null) writeMethods = _tmp;
                }
                return writeMethods;
            }
        }
        System.Collections.Hashtable typedSerializers = null;
        public override System.Collections.Hashtable TypedSerializers {
            get {
                if (typedSerializers == null) {
                    System.Collections.Hashtable _tmp = new System.Collections.Hashtable();
                    _tmp.Add(@"PDiscountCard.CardMooverInfo::", new CardMooverInfoSerializer());
                    if (typedSerializers == null) typedSerializers = _tmp;
                }
                return typedSerializers;
            }
        }
        public override System.Boolean CanSerialize(System.Type type) {
            if (type == typeof(global::PDiscountCard.CardMooverInfo)) return true;
            return false;
        }
        public override System.Xml.Serialization.XmlSerializer GetSerializer(System.Type type) {
            if (type == typeof(global::PDiscountCard.CardMooverInfo)) return new CardMooverInfoSerializer();
            return null;
        }
    }



    }
*/
