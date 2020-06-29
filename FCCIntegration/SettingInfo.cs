using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;


namespace FCCIntegration
{
    [Serializable]
    public class SettingInfo
    {

        public SettingInfo()
        {

        }
        [XmlElement]
        public string FCCIp = @"http://192.168.77.8/axis2/services/BrueBoxService";
        [XmlElement]
        public string EventLisenterIp = "localhost";
        [XmlElement]
        public int EventLisenterPort = 55561;
        [XmlElement]
        public string LocalIp = "192.168.77.127";

        public List<BarabanMin> BarabanMins { set; get; }



    }

    [Serializable]
    public class BarabanMin
    {
        [XmlAttribute]
        public int fv;
        [XmlAttribute]
        public int MinCount;
    }
}
