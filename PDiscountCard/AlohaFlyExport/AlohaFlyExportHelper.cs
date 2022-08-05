using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;


namespace PDiscountCard.AlohaFlyExport
{
    public class AlohaFlyExportHelper
    {
        Thread Th;
        public void SendOrderFlightAsync(Check chk)
        {
             Th = new Thread(
                () =>
                {
                    Utils.ToLog("[SendOrderFlightAsync] " );
                    try
                    {
                        string SendOrderFlightSavePath = @"C:\Aloha\check\Discount\Tmp\Flight\";
                        var dir = new DirectoryInfo(SendOrderFlightSavePath);
                        if (!Directory.Exists(SendOrderFlightSavePath))
                        {
                            Directory.CreateDirectory(SendOrderFlightSavePath);
                        }
                        if (chk != null)
                        {
                            string fileName = SendOrderFlightSavePath + chk.GuidId + "Flight.xml";
                            if (!File.Exists(fileName))
                            {
                                CloseCheck.SerializeCheck(fileName, chk);
                                if (DBProvider.SendOrder(GetOrderFlightFromAlohaCheck(chk)))
                                {
                                    File.Delete(fileName);
                                    Utils.ToLog("[SendOrderFlightAsync]  File.Delete(fileName);" + fileName);
                                }
                            }
                        }
                        foreach (var d in dir.GetFiles("*Flight.xml"))
                        {
                            try
                            {
                                Utils.ToLog("[SendOrderFlightAsync] Find File" + d.Name);
                                var chk2 = CloseCheck.ReadCheckFromTmp(d.FullName);
                                if (DBProvider.SendOrder(GetOrderFlightFromAlohaCheck(chk2)))
                                {
                                    File.Delete(d.FullName);
                                }
                            }
                            catch(Exception ee)
                            {
                                Utils.ToLog("[SendOrderFlightAsync] error " + ee.Message);
                            }
                        }

                        //var XChk = GetOrderFlightFromAlohaCheck(chk);
                        //DBProvider.SendOrder(XChk);
                    }
                    catch (Exception e)
                    {
                        Utils.ToLog("[Error] SendOrderFlightAsync " + e.Message);
                    }

                }
                );
            Th.Start();
        }





        public static AlohaService.OrderFlight GetOrderFlightFromAlohaCheck(Check chk)
        {
            var of = new AlohaService.OrderFlight();
            if (chk.Tenders.Count == 0)
            {
                Utils.ToLog("[GetOrderFlightFromAlohaCheck] no tenders");
                return null;
            }
            of.AlohaGuidId = chk.GuidId;
            int Tndrid = chk.Tenders[0].AlohaTenderId;

            Utils.ToLog("GetOrderFlightFromAlohaCheck Tndrid:" + Tndrid.ToString());

            if (Tndrid < 100)
            {
                if (Tndrid == 1)
                {
                    of.AirCompanyId =  iniFile.AlohaFlyCompanyIdCash;
                }
                else if (Tndrid == 20)
                {
                    of.AirCompanyId = iniFile.AlohaFlyCompanyIdCash;
                }
                else
                {
                    of.AirCompanyId = iniFile.AlohaFlyCompanyIdOver;
                }
                
                of.Closed = true;
                of.PreCheckPrinted = (Tndrid == 2);
                of.FRPrinted = (Tndrid != 2);
                of.OrderStatus = AlohaService.OrderStatus._16;

                Utils.ToLog("Чек отправлен как закрытый Tndrid:" + Tndrid.ToString());
            }
            else
            {
                of.AirCompanyId = Tndrid - 100;
                of.Closed = false;
                of.OrderStatus = AlohaService.OrderStatus._4;
                Utils.ToLog("Чек отправлен как открытый Tndrid:" + Tndrid.ToString());
            }

            of.CreatedById = iniFile.AlohaFlyExportUserId;
            of.SendById = iniFile.AlohaFlyExportUserId;
            of.CreationDate = chk.SystemDateOfOpen;
            of.ReadyTime = chk.SystemDateOfOpen;
            of.ExportTime = chk.SystemDateOfOpen;
            of.DeliveryPlaceId = iniFile.AlohaFlyExportPlaceId;
            //of.DishPackages = new AlohaService.DishPackageFlightOrder();
            of.FlightNumber = GetFightNumber(chk);
            of.IsSHSent = false;
            of.NumberOfBoxes = 1;
            of.DeliveryDate = chk.SystemDate;

            var dList = new List<AlohaService.DishPackageFlightOrder>();
            foreach (var d in chk.ConSolidateDishez)
            {
                if (d.BarCode == iniFile.AlohaFlyExportFlightNumberDishId) { continue; }
                var pd = new AlohaService.DishPackageFlightOrder();
                pd.Amount = d.Count * d.QtyQUANTITY;
                pd.Code = d.BarCode;
                pd.DishName = d.Name;
                pd.PositionInOrder = dList.Count + 1;
                pd.TotalPrice = (decimal)d.Priceone;
                dList.Add(pd);

            }
            of.DishPackages = dList.ToArray();
            return of;
        }


        public static string GetFightNumber(Check chk)
        {
            if (chk.Dishez.Any(a => a.BarCode == iniFile.AlohaFlyExportFlightNumberDishId))
            {
                return chk.Dishez.FirstOrDefault(a => a.BarCode == iniFile.AlohaFlyExportFlightNumberDishId).Name;
            }
            return "";
        }
    }
}
