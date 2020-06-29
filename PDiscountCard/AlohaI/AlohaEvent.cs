using System;
using System.Collections.Generic;
using System.Text;


namespace PDiscountCard
{
    /*
    static  public  class AlohaEvent
    {
        public delegate void AddDishDelegate(object sender, string Name, string Price);

        public static event AddDishDelegate AddDishEvent;
      //  static string CName = "v-pc";
        static string CName = Environment.MachineName;
        static int CPort = 25252;




      
        static internal  void AddDishVoid( string Name, string Price)
        {
            if (!iniFile.RemoteEventEnabled)
            {
                return;
            }
           

            RemoteConnection Rc = new RemoteConnection();
            //Rc.SendData(Environment.MachineName, 25252, Name);
            object[] ob = new object[3];
            ob[0] = Name;
            ob[1] = Price ;
            ob[2] = 1;
            Rc.SendData(CName ,CPort  , ob);
            

        }
        static internal void NewOrderVoid()
        {
            if (!iniFile.RemoteEventEnabled)
            {
                return;
            }
            RemoteConnection Rc = new RemoteConnection();
            object[] ob = new object[3];
            ob[0] = "";
            ob[1] = "";
            ob[2] = 0;
            Rc.SendData(CName, CPort, ob);
        }
        static internal void ShowTotalVoid(string Price)
        {
            if (!iniFile.RemoteEventEnabled)
            {
                return;
            }
            RemoteConnection Rc = new RemoteConnection();
            object[] ob = new object[3];
            ob[0] = "Итого: ";
            ob[1] = Price ;
            ob[2] = 1;
            Rc.SendData(CName, CPort, ob);
        }

    }
     * */
}
