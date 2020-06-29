using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;

namespace PDiscountCard.Scale
{
    public class Scale2
    {

        static PDSystem.AlertModalWindow ScaleWnd;
        static ctrlScaleMessage ctrlMess;
        static IScale myScale = new ScaleCasSWN();
        static bool Running = false;

        public static void GetItmWeight(int Barcode, string DishName)
        {
            if (Running)
            {
                try
                {
                    ScaleWnd.Close();
                }
                catch
                { };
                try
                {
                    myScale.Disconnect();
                }
                catch
                {

                }
            }
            ctrlMess = new ctrlScaleMessage();
            ScaleWnd = PDSystem.ModalWindowsForegraund.GetModalWindow(ctrlMess);
            ctrlMess.SetOwner(ScaleWnd);
            Running = true;
            ctrlMess.SetDish(Barcode, DishName);
            GetWieghtQueueStop = false;
            /*
            try
            {
                myScale.Disconnect();
            }
            catch
            { 
            
            }
            */

            switch (iniFile.ScaleType)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    myScale = new ScaleCasSWN();
                    break;
                default:
                    break;
            }
            if (!myScale.Connect())
            {
                ctrlMess.SetError("Нет соединения с весами. Com " + iniFile.ScalePort.ToString());
            }


            Thread ScaleWorkThread = new Thread(GetWieghtQueue);
            //ShtrihWorkThread.SetApartmentState(ApartmentState.STA);
            ScaleWorkThread.Start();

            ScaleWnd.ShowDialog();
            GetWieghtQueueStop = true;

            myScale.Disconnect();
            Running = false;
        }


        private static bool GetWieghtQueueStop = false;
        private static void GetWieghtQueue()
        {

            while (!GetWieghtQueueStop)
            {
                try
                {
                    bool Stable = false;
                    double ItmWeight = 0;
                    string ErrStr = "";
                    if (!GetWieghtQueueStop)
                    {
                        int res = myScale.GetWeight(out ItmWeight, out Stable, out ErrStr);
                        if (!GetWieghtQueueStop)
                        {
                            if (res == 0)
                            {
                                ctrlMess.SetWeight((int)(ItmWeight * 1000), Stable);
                            }
                            else if (res < 0)
                            {
                                ctrlMess.SetError(ErrStr);
                            }
                        }
                    }
                }
                catch
                {
                    Thread.Sleep(1000);

                }
                //  Thread.Sleep(500);
            }


        }
    }
}
