
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard.FRSClientApp
{
    static class GesData
    {
        static public void GetGesData(DateTime DB, int DepNum, out decimal cash, out decimal card)
        {
            cash = 0;
            card = 0;
            try
            {
                Utils.ToCardLog(String.Format("GetGesData DB {0} DepNum {1} ",DB,DepNum));
                Gestory.GestoriCashByDay_T_shopsRow[] shopsRow = new Gestory.GestoriCashByDay_T_shopsRow[1];
                shopsRow[0] = new Gestory.GestoriCashByDay_T_shopsRow();
                shopsRow[0].codShop = DepNum;
                Gestory.GestoriCashByDay_T_cashRow[] cashRow = new Gestory.GestoriCashByDay_T_cashRow[2];
                cashRow[0] = new Gestory.GestoriCashByDay_T_cashRow();

                System.ServiceModel.Channels.Binding binding = new System.ServiceModel.BasicHttpBinding();
                ((System.ServiceModel.BasicHttpBinding)binding).MaxReceivedMessageSize = 1024 * 1024;
                System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(@"http://vfiliasesb0:2580/process/Ges3ServicesProc");
                //System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(iniFile.FRSSRVPath.Trim());


                Gestory.Ges3ServicesObjClient GesCl = new Gestory.Ges3ServicesObjClient(binding, remoteAddress);
                GesCl.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);

                string Res = GesCl.GestoriCashByDay(DB, false, shopsRow, out cashRow);
                cash = cashRow[0].sum_nal.Value;
                card = cashRow[0].sum_plast.Value;
                Utils.ToCardLog("GetGesData cash " +cash);

            }
            catch (Exception e)
            {
                Utils.ToCardLog("Error GetGesData " +e.Message);
                // System.Windows.Forms.MessageBox.Show(e.Message);

            }
        }
    }
}
