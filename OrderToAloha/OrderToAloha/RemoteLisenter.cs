using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace OrderToAloha
{
    class RemoteLisenter
    {
        static RemoteConnection RC;

        internal RemoteLisenter(int port)
        { 
         Init(port);
        }

        void Init(int port)
        {
            try
            {
                RC = new RemoteConnection();

                RC.StartServerLisenter(port);

                RC.DataRecive += new RemoteConnection.DataReciveDelegate(RC_DataRecive);
            }
            catch (Exception e)
            {
                
            }
        }

        internal void Stop()
        {
            try
            {
                RC.StopServerLisenter();
                RC.DataRecive -= new RemoteConnection.DataReciveDelegate(RC_DataRecive);
            }
            catch (Exception e)
            {

            }
        }

        internal delegate void ResponseEventHandler(DataReciver.AlohaResponse e);

        
        internal event ResponseEventHandler ResponseEvent;

        protected virtual void RaiseResponseEvent(DataReciver.AlohaResponse e)
        {
        
            if (ResponseEvent != null)
                ResponseEvent(e);
        }


         void RC_DataRecive(object Fi)
        {
            try
            {

                
                DataReciver.STCommand InCommand = (DataReciver.STCommand)Fi;


                //Utils.ToCardLog("RC_DataRecive тип " + InCommand.CommandType);
                if (InCommand.Ansver)
                {
                    DataReciver.AlohaResponse resp = new DataReciver.AlohaResponse();
                    resp.OrderId = InCommand.sendOrderToAlohaRequest.OrderId;
                    resp.CommandType = InCommand.CommandType;
                    resp.Err = InCommand.ExeptionMessage;
                    resp.port = InCommand.SenderPort;
                    resp.ResultId = InCommand.ResultId;
                    if (InCommand.CommandType==DataReciver.STCommandType.AddOrder)
                    {
                        
                        resp.AlohaCheckId = InCommand.sendOrderToAlohaRequest.AlohaCheckId;
                        resp.AlohaTableNum = InCommand.sendOrderToAlohaRequest.AlohaTableNum;
                        
                    }
                    else if (InCommand.CommandType == DataReciver.STCommandType.DeleteOrder)
                    { 
                    
                    }



                    RaiseResponseEvent(resp);
                }
                else
                {
                 
                }

            }
            catch(Exception e)
            {
                Utils.ToLog(e.Message);
               
            }
        }

    }
}
