using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;




namespace PDiscountCard
{

    public class JmsSender
    {
        public JmsSender()
        {

        }


        private string CreateTmpSpoolMessageFile(string text)
        {
            string FileName = DateTime.Now.ToString("HHmmss_ddMMyy") + ".spl";
            try
            {
                DirectoryInfo di = new DirectoryInfo(Utils.TmpSpoolFilePath);
                if (!di.Exists)
                {
                    di.Create();
                }
                StreamWriter writer = File.AppendText(Utils.TmpSpoolFilePath + FileName);
                writer.WriteLine(text);
                writer.Close();
                return Utils.TmpSpoolFilePath + FileName;
            }
            catch (Exception e)
            {
                Utils.ToLog("[Error] CreateTmpSpoolMessageFile " + e.Message);
                return "";
            }

        }
        /*
                internal void SendTmpFiles()
                {
                    Sonic.Jms.Ext.Connection connect = GetJmsConnection();
                    if (connect != null)
                    {
                        DirectoryInfo di = new DirectoryInfo (Utils.TmpSpoolFilePath);
                        foreach (FileInfo fi in di.GetFiles("*.spl"))
                        {
                            try
                            { 
                        
                               if (SendMsg (File.ReadAllText (fi.FullName),connect )) 
                               {
                                   try
                                   {
                                       File.Delete(fi.FullName);
                                       Utils.ToLog("Удалил временный файл");
                                   }
                                   catch (Exception e)
                                   {
                                       Utils.ToLog("[Error] при удалении временного файла " + e.Message);
                                   } 
                               }
                            }
                            catch
                            { 
                    
                            }
                        }
                    }
                }

                */

        /*
        private Sonic.Jms.Ext.Connection GetJmsConnection()
        { 

                Sonic.Jms.ConnectionFactory factory;
                Utils.ToLog("factory = new Sonic.Jms.Cf.Impl.ConnectionFactory  (iniFile.JMSHost);");
                factory = new Sonic.Jms.Cf.Impl.ConnectionFactory (iniFile.JMSHost);
                Utils.ToLog("Sonic.Jms.Ext.Connection connect = null;");
                Sonic.Jms.Ext.Connection connect = null;
                int TryCount = 0;
                while ((connect == null) && (TryCount < 5))
                {
                    try
                    {
                        Utils.ToLog("connect = (Sonic.Jms.Ext.Connection)factory.createConnection(iniFile.JMSLogin, iniFile.JMSPassword); " + iniFile.JMSLogin + iniFile.JMSPassword);
                        connect = (Sonic.Jms.Ext.Connection)factory.createConnection();
                        Utils.ToLog("connect ");
                        connect = (Sonic.Jms.Ext.Connection)factory.createConnection(iniFile.JMSLogin, iniFile.JMSPassword);
                        Utils.ToLog("connect.setPingInterval(30);");
                        connect.setPingInterval(30);
                        return connect;
                    }
                    catch (Sonic.Jms.JMSException jmse)
                    {
                        Utils.ToLog("[Error] Не могу подсоединится к сессии" + jmse.Message);
                        TryCount++;
                   }
               }
                return null;
        }

        */

        public void TrySendOldJmsAsync()
        {
            Thread th = new Thread(TrySendOldJmsA);
            th.Start();

        }

        private void TrySendOldJmsA()
        {
            TrySendOldJms();
        }

        public bool TrySendOldJms()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(Utils.TmpSpoolFilePath);
                if (!di.Exists)
                {
                    di.Create();
                }
                if (di.GetFiles("*.spl").Length == 0)
                {
                    return true;
                }

                //  Sonic.Jms.Ext.Connection connect = GetJmsConnection();
                Utils.ToCardLog("JMSCOMCLIENTLib.CJMSQueueConnection connect = SonicCom.GetJmsConnection();");
                JMSCOMCLIENTLib.CJMSQueueConnection connect = SonicCom.GetJmsConnection();
                foreach (FileInfo fi in di.GetFiles("*.spl"))
                {

                    try
                    {

                        if (connect != null)
                        {
                            //if (SendMsg(File.ReadAllText(fi.FullName), connect))
                            if (SonicCom.SendMsg(File.ReadAllText(fi.FullName), connect))
                            {
                                try
                                {
                                    File.Delete(fi.FullName);
                                    Utils.ToLog("Удалил временный файл");
                                }
                                catch (Exception e)
                                {
                                    Utils.ToLog("[Error] при удалении временного файла " + e.Message);
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception ee)
                    {
                        Utils.ToLog("[Error] TrySendOldJms " + ee.Message);
                        return true;
                    }


                }
                connect.close();
                return true;

            }
            catch (Exception e)
            {
                Utils.ToLog("[Error] TrySendOldJms " + e.Message);
                return true;
            }
        }

        public void CreateJmsSessionAndSendMsg(string text)
        {

            Utils.ToLog("TrySendOldJms");
            TrySendOldJms();
            Msgtext = text;
            /*
            Thread th = new Thread(mCreateJmsSessionAndSendMsg);
            th.Start();
         */
            mCreateJmsSessionAndSendMsg();
        }

        string Msgtext = "";
        public void mCreateJmsSessionAndSendMsg()
        {

            Utils.ToLog("mCreateJmsSessionAndSendMsg");
            string TmpFileName = CreateTmpSpoolMessageFile(Msgtext);


            //Sonic.Jms.Ext.Connection connect = GetJmsConnection();
            JMSCOMCLIENTLib.CJMSQueueConnection connect = SonicCom.GetJmsConnection();
            if (connect != null)
            {
                //  if (SendMsg(Msgtext, connect))
                if (SonicCom.SendMsg(Msgtext, connect))
                {
                    try
                    {
                        File.Delete(TmpFileName);
                        Utils.ToLog("Отправил сообщение и  Удалил временный файл");
                    }
                    catch (Exception e)
                    {
                        Utils.ToLog("[Error] при удалении временного файла " + e.Message);
                    }

                }
                else
                {
                    Utils.ToLog("Не Отправил сообщение ");
                }
                connect.close();
                
            }

        }


        /*
        private  bool SendMsg(string text, Sonic.Jms.Ext.Connection connect)
        {


            

            try
            {
                
                   
                        Sonic.Jms.Session session = connect.createSession(false, Sonic.Jms.SessionMode.CLIENT_ACKNOWLEDGE);
                        Sonic.Jms.Queue mQueue = session.createQueue(iniFile.JMSQueue);
                        Sonic.Jms.MessageProducer sender = session.createProducer(mQueue);
                        connect.start();


                        Sonic.Jms.TextMessage Tm = session.createTextMessage();
                        Tm.setJMSType("XML");
                        Tm.setText(text);
                        sender.send(Tm);
                        Utils.ToLog("Отправил сообщение в шину " + text,8);
                        sender.close();
                        session.close();
                      //  connect.close();
                        

                        return true;
                    
                }
            
            catch (Exception e)
            {
                Utils.ToLog("[Error] CreateJmsSessionAndSendMsg " + e.Message);
                return false;
            }

        }
        */


    }



/*
    public class mSonic : Sonic.Jms.MessageListener
    {

        public bool Start()
        {
            try
            {
                start(DEFAULT_BROKER_NAME, DEFAULT_USER_NAME, DEFAULT_PASSWORD, DEFAULT_QUEUE, DEFAULT_MODE);
                return true;
            }
            catch (Exception e)
            {
                Utils.ToLog("Error mSonic " + e.Message);
                return false;
            }
        }

        private const String DEFAULT_BROKER_NAME = "tcp://vfiliasesb0:2506";
        private const String DEFAULT_USER_NAME = "Administrator";
        private const String DEFAULT_PASSWORD = "Administrator";
        private const String DEFAULT_QUEUE = "OrderInfo.Entry";
        private const String DEFAULT_MODE = "uppercase";
        private const int UPPERCASE = 0;
        private const int LOWERCASE = 1;

        private Sonic.Jms.Connection connect = null;
        private Sonic.Jms.Session session = null;
        private Sonic.Jms.MessageProducer replier = null;
        private Sonic.Jms.MessageListener Listener = null;

        private int imode;

        private void start(String broker,
                      String username,
                      String password,
                      String rQueue,
                      String mode)
        {
            // Set the operation mode
            imode = (mode.Equals("uppercase")) ? UPPERCASE : LOWERCASE;

            // Create a connection.
            try
            {
                Sonic.Jms.ConnectionFactory factory;
                factory = (new Sonic.Jms.Cf.Impl.ConnectionFactory(broker));
                connect = factory.createConnection(username, password);


            }
            catch (Sonic.Jms.JMSException jmse)
            {
                Utils.ToLog("Failed to connect to broker - " + broker + " : " + jmse.Message);
                exit();
            }

            // Create receivers to application queues as well as a sender
            // to use for JMS replies.
            try
            {

                session = connect.createSession(false, Sonic.Jms.SessionMode.AUTO_ACKNOWLEDGE);
                Sonic.Jms.Queue queue = session.createQueue(rQueue);
                Sonic.Jms.MessageConsumer receiver = session.createConsumer(queue);


                replier = session.createProducer(null); // Queue will be set for each reply
                receiver.setMessageListener(this);
            }
            catch (Sonic.Jms.JMSException jmse)
            {
                Utils.ToLog("Failed to setup sessions, sender and receiver - " + jmse.Message);
                exit();
            }

            // Start connection
            try
            {
                connect.start();
            }
            catch (Sonic.Jms.JMSException jmse)
            {
                Utils.ToLog("Failed to start connection - " + jmse.Message);
                exit();
            }

            // Process user input

        }

        // Cleanup resources and then exit. 
        private void exit()
        {
            try
            {
                if (connect != null)
                {
                    connect.close();
                }
            }
            catch (Exception e)
            {
                Utils.ToLog("Failure in closing connection - " + e.Message);
            }

            Environment.Exit(0);
        }
        public virtual void onMessage(Sonic.Jms.Message aMessage)
        {
            try
            {






                Sonic.Jms.Ext.XMLMessage Xm = (Sonic.Jms.Ext.XMLMessage)aMessage;

                string k = Xm.getText();

                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(k);

                int o = Xm.getJMSDeliveryMode();
                Xm.setJMSDeliveryMode(1);
                OrderInfo Oi = new OrderInfo(Doc);

                Utils.ToLog("Получил заказ: " + Oi.OrderID + " подразделение: " + Oi.OrderDestinationDeptID);
                if (Oi.OrderDestinationDeptID == AlohainiFile.DepNum)
                {

                    MainClass.ProcessRemoteOrder(Oi);
                }

            }
            catch (Exception e)
            {
                Utils.ToLog("Failed to process received message - " + e.Message);
            }
        }
    }
 * */
    public class OrderInfoForAloha
    {
        public int ID = 0;
        public string OrderID = "";
        public int OrderDestinationDeptID = -1;
        public int CustomerID = -1;
        public string CustomerName = "";
        public string CustomerPhone = "";
        public DateTime OrderRegisteredAtTime = new DateTime();
        public DateTime OrderRequiredStartTime = new DateTime();
        public DateTime OrderRequiredEndTime = new DateTime();
        public Item[] Items = null;
    }



    public class OrderInfo
    {

        public OrderInfo()
        {
        }

        public OrderInfo(XmlDocument Doc)
        {
            //Xr.ReadStartElement();

            XmlNode Xn = Doc.GetElementsByTagName("OrderInfo")[0];

            OrderID = Doc.GetElementsByTagName("OrderID")[0].FirstChild.Value;
            OrderDestinationDeptID = Convert.ToInt32(Doc.GetElementsByTagName("OrderDestinationDeptID")[0].FirstChild.Value);
            CustomerID = Convert.ToInt32(Doc.GetElementsByTagName("CustomerID")[0].FirstChild.Value);
            CustomerName = Doc.GetElementsByTagName("CustomerName")[0].FirstChild.Value;
            CustomerPhone = Doc.GetElementsByTagName("CustomerPhone")[0].FirstChild.Value;
            OrderRegisteredAtTime = DateTime.Parse(Doc.GetElementsByTagName("OrderRegisteredAtTime")[0].FirstChild.Value);
            OrderRequiredStartTime = DateTime.Parse(Doc.GetElementsByTagName("OrderRequiredStartTime")[0].FirstChild.Value);
            OrderRequiredEndTime = DateTime.Parse(Doc.GetElementsByTagName("OrderRequiredEndTime")[0].FirstChild.Value);

            Items = new Item[Doc.GetElementsByTagName("Items")[0].ChildNodes.Count];
            int i = 0;
            foreach (XmlNode ItemNode in Doc.GetElementsByTagName("Items")[0].ChildNodes)
            {
                Items.SetValue(new Item(ItemNode), i);
                i++;

            }






        }



        public string OrderID = "";
        public int OrderDestinationDeptID = -1;
        public int CustomerID = -1;
        public string CustomerName = "";
        public string CustomerPhone = "";
        public DateTime OrderRegisteredAtTime = new DateTime();
        public DateTime OrderRequiredStartTime = new DateTime();
        public DateTime OrderRequiredEndTime = new DateTime();
        public Item[] Items = null;
    }

    public class Item
    {
        public Item()
        {
        }

        public Item(XmlNode ItemNode)
        {
            foreach (XmlNode ItemParam in ItemNode.ChildNodes)
            {
                switch (ItemParam.Name)
                {
                    case "ItemID":
                        ItemID = Convert.ToInt32(ItemParam.FirstChild.Value);
                        break;
                    case "ItemName":
                        ItemName = ItemParam.FirstChild.Value;
                        break;
                    case "ItemQuantity":
                        ItemQuantity = Convert.ToInt32(ItemParam.FirstChild.Value);
                        break;
                    case "ItemSpecialMessage":
                        ItemSpecialMessage = ItemParam.FirstChild.Value;
                        break;
                    case "Modifiers":
                        Modifiers = new Modifier[ItemParam.ChildNodes.Count];
                        int i = 0;
                        foreach (XmlNode ModifiersNode in ItemParam.ChildNodes)
                        {
                            Modifiers.SetValue(new Modifier(ModifiersNode), i);
                            i++;

                        }
                        break;

                    default:
                        break;
                }
            }
        }
        public int ItemID = -1;
        public string ItemName = "";
        public int ItemQuantity = 1;
        public string ItemSpecialMessage = "";
        public Modifier[] Modifiers = null;
    }
    public class Modifier
    {
        public Modifier()
        {

        }
        public Modifier(XmlNode ModifiersNode)
        {
            foreach (XmlNode ModiferParam in ModifiersNode.ChildNodes)
            {
                switch (ModiferParam.Name)
                {
                    case "ItemModifierID":
                        ItemModifierID = Convert.ToInt32(ModiferParam.FirstChild.Value);
                        break;
                    case "ItemModifierName":
                        ItemModifierName = ModiferParam.FirstChild.Value;
                        break;
                    case "ItemModifierQuantity":
                        ItemModifierQuantity = Convert.ToInt32(ModiferParam.FirstChild.Value);
                        break;
                    case "ItemModifierSpecialMessage":
                        ItemModifierSpecialMessage = ModiferParam.FirstChild.Value;
                        break;


                    default:
                        break;
                }
            }
        }

        public int ItemModifierID = -1;
        public string ItemModifierName = "";
        public int ItemModifierQuantity = 1;
        public string ItemModifierSpecialMessage = "";
    }

}
