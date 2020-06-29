using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace PDCardSummChecker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Now;
        }

        List<string> ArcusFiles = new List<string>();
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            textBox1.Text += openFileDialog1.FileName + Environment.NewLine;
            ArcusFiles.Add(openFileDialog1.FileName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ReadXmlChecks();
            ReadArcusSlips();
            Check();

        }


        private void Check()
        {
            foreach (ZRepSrv.XmlCheckRecive RChk in XmlChecks)
            {
                if (ArcusSlips.Where(a => (a.Sum / 100) == RChk.Card && Math.Abs((RChk.Dt.AddHours(-1) - a.HostDt).TotalMinutes) < 20).Count() > 0)
                {
                    PDiscountCard.ArcusSlip Sl = ArcusSlips.Where(a => (a.Sum / 100) == RChk.Card && Math.Abs((RChk.Dt.AddHours(-1) - a.HostDt).TotalMinutes) < 20).First();
                    ArcusSlips.Remove(Sl);
                }
                else
                {
                    textBox3.Text += String.Format("Xml  Unik Chk  Dt {0},  Summ {1} ", RChk.Dt, RChk.Card) + Environment.NewLine; ;
                }
            }
            foreach (PDiscountCard.ArcusSlip Sl in ArcusSlips)
            {
                textBox3.Text += String.Format("Arcus  Unik Chk  Dt {0},  Summ {1}, RRN {2} ", Sl.HostDt, Sl.Sum, Sl.RRN) + Environment.NewLine; ;
                
            
            }
        }

        private List<PDiscountCard.ArcusSlip> ArcusSlips = new List<PDiscountCard.ArcusSlip>();
        private List<ZRepSrv.XmlCheckRecive> XmlChecks = new List<ZRepSrv.XmlCheckRecive>();


        private void ReadXmlChecks()
        {
            XmlChecks.Clear();
            System.ServiceModel.Channels.Binding binding = new System.ServiceModel.BasicHttpBinding();
            ((System.ServiceModel.BasicHttpBinding)binding).MaxReceivedMessageSize = 1024 * 1024;
            System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress("http://s2010:3134/service1.asmx");
            ZRepSrv.Service1SoapClient Cl = new ZRepSrv.Service1SoapClient(binding, remoteAddress);
            Cl.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);
            
            XmlChecks = Cl.GetChecksByDofBandDep(dateTimePicker1.Value.Date, Convert.ToInt32(textBox2.Text)).Where(a=>a.Card!=0).ToList();
            textBox3.Text += String.Format("Xml Пластик {0}", XmlChecks.Sum(a => a.Card)) + Environment.NewLine; 



        }

        private void ReadArcusSlips()
        {
            ArcusSlips.Clear();
            foreach (string s in textBox1.Text.Split(Environment.NewLine.ToCharArray()))
            {
                ArcusSlips.AddRange(GetSlips(s));
            }
            textBox3.Text += String.Format("Arcus Пластик {0}", ArcusSlips.Sum(a => a.Sum)) + Environment.NewLine; 
        }


        private List<PDiscountCard.ArcusSlip> GetSlips(string Fname)
        {
            if (Fname == "") return new List<PDiscountCard.ArcusSlip>();
            XmlReader XR = new XmlTextReader(Fname);
            try
            {
                XmlSerializer XS = new XmlSerializer(typeof(PDiscountCard.ArcusSlips));
                PDiscountCard.ArcusSlips CMI = (PDiscountCard.ArcusSlips)XS.Deserialize(XR);
                XR.Close();
                return CMI.Slips;
            }
            catch
            {
                XR.Close();
                return null;
            }
            
        
        }
    }
}
