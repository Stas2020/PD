using System;
using System.Collections.Generic;
using System.Text;

namespace PDiscountCard
{
    static class SonicCom
    {
        static internal JMSCOMCLIENTLib.CJMSQueueConnection GetJmsConnection()
        {

            JMSCOMCLIENTLib.CJMSQueueConnectionFactory factory;

            factory = new JMSCOMCLIENTLib.CJMSQueueConnectionFactory();
            factory.initialize2(iniFile.JMSHost, iniFile.JMSPort, "tcp", iniFile.JMSLogin, iniFile.JMSPassword);
            JMSCOMCLIENTLib.CJMSQueueConnection connect = null;
            int TryCount = 0;
            while ((connect == null) && (TryCount < 5))
            {
                try
                {

                    connect = factory.createQueueConnection();
                    Utils.ToCardLog("connect = factory.createQueueConnection();");
                    connect.setPingInterval(30);
                    return connect;

                }
                //catch (Sonic.Jms.JMSException jmse)
                catch (Exception jmse)
                {
                    Utils.ToLog("[Error] Не могу подсоединится к сессии" + jmse.Message);
                    TryCount++;
                }
            }
            return null;
        }
        static internal bool SendMsg(string text, JMSCOMCLIENTLib.CJMSQueueConnection connect)
        {
            try
            {
                JMSCOMCLIENTLib.CJMSQueueSession session = connect.createQueueSession(0, 2);
                JMSCOMCLIENTLib.CJMSQueue mQueue = session.createQueue(iniFile.JMSQueue);
                //Sonic.Jms.Queue mQueue = session.createQueue(iniFile.JMSQueue);
                //Sonic.Jms.MessageProducer sender = session.createProducer(mQueue);
                JMSCOMCLIENTLib.CJMSQueueSender sender = session.createSender(mQueue);
                connect.start();

                JMSCOMCLIENTLib.CJMSTextMessage Tm = session.createTextMessage();
                //Sonic.Jms.TextMessage Tm = session.createTextMessage();

                Tm.setJMSType("XML");
                Tm.setText(text);

                sender.send((JMSCOMCLIENTLib.CJMSMessage)Tm);

                Utils.ToLog("Отправил сообщение в шину " + text, 8);
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

    }
}
