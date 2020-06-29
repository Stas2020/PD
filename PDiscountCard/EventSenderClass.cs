using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace PDiscountCard
{
    static  class EventSenderClass
    {
        static StopListService.Service1 s1; 
        public static void Init()
        {
            try
            {
                /*
                ServiceHost host = new ServiceHost (typeof(StopListService.Service1SoapClient),new Uri( @"http://s2010:3131/Service1.asmx"));
                host.AddServiceEndpoint ( typeof(StopListService.Service1SoapClient),new System.ServiceModel.WebHttpBinding() ,"").Behaviors.Add (new System.ServiceModel.Description.WebHttpBehavior()) ;
                

                host.Open (); 
                 **/

                System.ServiceModel.EndpointAddress adr = new System.ServiceModel.EndpointAddress (@"http://s2010:3131/Service1.asmx")  ;
                s1 = new PDiscountCard.StopListService.Service1();
                //s1.t 
                SendAlohaAsincEvent(PDiscountCard.StopListService.AlohaEventType.TErmInit, "", 0, 0, "", 0, 0,0);
                
            }
            catch(Exception e)
            {
                Utils.ToLog("[Error] EventSenderClass.Init() " + e.Message);
            }
        }
        static StopListService.RemoteEventType mEventType;
        static string mMessage;
        private  static void mSendAsincEvent()
        {
            try
            {
                s1.RemoteEventSend(AlohainiFile.DepNum, Utils.GetTermNum(), Environment.MachineName, mEventType, mMessage);
            }
            catch
            { }
        }
        static Thread t1;
        public static void SendAsincEvent(StopListService.RemoteEventType EventType, string Message)
        {
            try
            {
                if (!iniFile.AsincSenderEventDisabled)
                {
                    mEventType = EventType;
                    mMessage = Message;
                    t1 = new Thread(mSendAsincEvent);
                    t1.Priority = ThreadPriority.Lowest;
                    t1.Start();
                }
            }
            catch
            { }
        
        }

        static  StopListService.AlohaEventType mmEventType; 
         static   string mmMessage; 
        static int mmCode;
        static  int mmCodeDoljn;
           static  string mmName;
        static  int mmToEmployeeId;
        static int mmTableId;
        static int mmCheckId;

        private  static void mmSendAlohaAsincEvent()
        {
            try
            {
                s1.AlohaEventSend2Async(AlohainiFile.DepNum, Utils.GetTermNum(), Environment.MachineName, mmEventType, mmCode, mmCodeDoljn, mmName, mmToEmployeeId, mmTableId, mmCheckId);
            }
            catch
            { }

        }
       static  Thread t2;
        public static void SendAlohaAsincEvent(PDiscountCard.StopListService.AlohaEventType EventType ,
            string Message, int Code, int CodeDoljn, string Name, int ToEmployeeId, int TableId, int CheckId)
        {
            try
            {
                if (!iniFile.AsincSenderEventDisabled)
                {
                    mmEventType = EventType;
                    mmMessage = Message;
                    mmCode = Code;
                    mmCodeDoljn = CodeDoljn;
                    mmName = Name;
                    mmToEmployeeId = ToEmployeeId;
                    mmTableId = TableId;
                    mmCheckId = CheckId;
                    t2 = new Thread(mmSendAlohaAsincEvent);

                    t2.Priority = ThreadPriority.Lowest;
                    t2.Start();
                }
            }
            catch
            { }

        }
    }
}
