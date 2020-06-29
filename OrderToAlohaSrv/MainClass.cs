using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace OrderToAlohaSrv
{
  public   static class MainClass
    {
        static  Thread  ThreadAddOders;
        static RemoteLisenter RL;
        public static void Init(int port, string SqlServerName)
        {
            try
            {
                Utils.ToLog("AlohaGalerySrvInit port " + port + "SqlServerName " + SqlServerName);
                ToSql.SetConnStr(SqlServerName);

                ThreadAddOders = new System.Threading.Thread(SendFromNetToAloha.AddOrdQ);
                ThreadAddOders.Start();
                RL = new RemoteLisenter();
                RL.Init(port);
            }
            catch(Exception e)
            {
                Utils.ToLog("Error AlohaGalerySrvInit port " + port + "SqlServerName " + SqlServerName +"  " + e.Message);
            }


        }
         //public delegate void NeedCloseCheck(object sender);
         //public event 
    }
        }


