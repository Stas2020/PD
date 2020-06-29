using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;

namespace PDiscountCard.Hamster
{
    public static class HamsterWorker
    {
        public static string ChecksPath = @"C:\aloha\check\discount\tmp\check";

        static Thread SendOldHamsterThread;
        public static void SendOldHamster()
        {
            SendOldHamsterThread = new Thread(SendOldProcess);
            SendOldHamsterThread.Name = "SendOldHamsterThread";
            SendOldHamsterThread.Priority = ThreadPriority.Lowest;
            SendOldHamsterThread.Start();

        }

        public static void SendOldProcess()
        {
            if (iniFile.FRSEnabled)
            {
                try
                {
                    if (Directory.GetFiles(ChecksPath).Where(a => a.Contains("hamster")).Count() == 0)
                    {
                        return;
                    }
                    DirectoryInfo Di = new DirectoryInfo(ChecksPath);
                    foreach (FileInfo fi in Di.GetFiles())
                    {
                        if (fi.Name.Contains("hamster"))
                        {
                            AllChecks myAllChecks2 = CloseCheck.ReadAllChecks(fi.FullName);
                            Utils.ToCardLog("SendOldHamster " + fi.FullName);
                            //ZRSrv.Service1 srv1 = new ZRSrv.Service1();

                            bool Ok = true;
                            int UndepNum = 0;
                            try
                            {
                                UndepNum = Convert.ToInt32(AlohainiFile.DepNum.ToString() + AlohaTSClass.GetTermNum().ToString());
                            }
                            catch
                            {
                                UndepNum = 9984;
                            }
                            foreach (Check Chk in myAllChecks2.Checks)
                            {

                                try
                                {
                                    Utils.ToCardLog("SendOldHamster " + Chk.BusinessDate);
                                    XmlSerializer xs = new XmlSerializer(typeof(Check));
                                    StringWriter xout = new StringWriter();
                                    xs.Serialize(xout, Chk);

                                    string res = "";
                                    try
                                    {
                                        ZRepSrv.Service1SoapClient Cl = GetZrepClient();
                                        res = Cl.AddHamsterCheck(xout.ToString(), UndepNum, AlohainiFile.DepNum, ChecksPath + @"\hamster.xml");
                                        Cl.Close();
                                    }
                                    catch (Exception e)
                                    {
                                        Utils.ToCardLog("Error send oldChk " + e.Message);
                                    }

                                    if (!((res == "Ok") || (res == "exist")))
                                    {
                                        if (!Directory.Exists(CloseCheck.BugChecksPath))
                                        {
                                            Directory.CreateDirectory(CloseCheck.BugChecksPath);
                                        }
                                        using (XmlWriter Xwr = new XmlTextWriter(CloseCheck.BugChecksPath + @"\" + Chk.GuidId.ToString() + ".xml", System.Text.Encoding.UTF8))
                                        {
                                            xs.Serialize(Xwr, Chk);
                                            Utils.ToCardLog("Save oldChk ");
                                        }
                                        //Utils.ToCardLog("Move SendHamsterChk" + XmlFileName);
                                    }



                                    xout.Close();
                                    xout.Dispose();

                                    Utils.ToCardLog("SendOldHamster End res = " + res);
                                }
                                catch (Exception e)
                                {

                                    Utils.ToCardLog("SendOldHamster error= " + e.Message);
                                }
                            }
                            fi.Delete();
                        }
                    }
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("Error SendOldProcess" + e.Message);
                }
            }
        }


        public static ZRepSrv.Service1SoapClient GetZrepClient()
        {
            try
            {
                Utils.ToCardLog("Init ZRep ");

                System.ServiceModel.Channels.Binding binding = new System.ServiceModel.BasicHttpBinding();
                ((System.ServiceModel.BasicHttpBinding)binding).MaxReceivedMessageSize = 1024 * 1024;
                //System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(@"http://192.168.77.7:3838/FRSService/RemoteData");
                System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(@"http://s2010:3134/service1.asmx");
                ZRepSrv.Service1SoapClient FClient = new ZRepSrv.Service1SoapClient(binding, remoteAddress);
                return FClient;
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error Init ZRep " + e.Message);
                return null;
            }
        }

        public static void MoveHamster()
        {
            string XmlFileName = CloseCheck.ChecksPath + @"\hamster.xml";
            try
            {
                if (!File.Exists(XmlFileName))
                { return; }
                if (!Directory.Exists(CloseCheck.BugChecksPath))
                {
                    Directory.CreateDirectory(CloseCheck.BugChecksPath);
                }
                File.Move(XmlFileName, CloseCheck.BugChecksPath + @"\" + Guid.NewGuid() + Path.GetFileName(XmlFileName));
                Utils.ToCardLog("Move SendHamsterChk" + XmlFileName);
            }
            catch(Exception e)
            {
                Utils.ToCardLog("Errro Move SendHamsterChk " + e.Message);
            }

        }

        public static void SendHamsterChk(string XmlFileName)
        {
            if (!File.Exists(XmlFileName))
            { return; }
            try
            {
                Utils.ToCardLog("SendHamsterChk " + XmlFileName);
                XmlDocument Xd = new XmlDocument();
                Xd.Load(XmlFileName);
                ZRSrv.Service1 srv1 = new ZRSrv.Service1();
                int UndepNum = Convert.ToInt32(AlohainiFile.DepNum.ToString() + AlohaTSClass.GetTermNum().ToString());



                ZRepSrv.Service1SoapClient Cl = GetZrepClient();
                //   string res = Cl.AddHamsterCheck((XmlElement)x.ChildNodes[1], UndepNum, AlohainiFile.DepNum, ChecksPath + @"\hamster.xml");
                string res = Cl.AddHamsterCheck(Xd.OuterXml, UndepNum, AlohainiFile.DepNum, ChecksPath + @"\hamster.xml");

                Utils.ToCardLog("res = " + res);
                //      string res =   srv1.AddHamsterCheck(Xd.ChildNodes[1].ToString(), UndepNum, AlohainiFile.DepNum, XmlFileName);
                if ((res == "Ok") || (res == "exist"))
                {
                    File.Delete(XmlFileName);
                    Utils.ToCardLog("File.Delete(XmlFileName);");
                }
                else
                {
                    if (!Directory.Exists(CloseCheck.BugChecksPath))
                    {
                        Directory.CreateDirectory(CloseCheck.BugChecksPath);
                    }
                    File.Move(XmlFileName, CloseCheck.BugChecksPath + @"\" + Path.GetFileName(XmlFileName));
                    Utils.ToCardLog("Move SendHamsterChk" + XmlFileName);
                }

                Utils.ToCardLog("SendHamsterChk Ok" + XmlFileName);
            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error SendHamsterChk " + e.Message);
            }

        }

    }
}
