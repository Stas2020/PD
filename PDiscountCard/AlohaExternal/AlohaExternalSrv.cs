using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.IO;
using System.Threading;

namespace PDiscountCard.AlohaExternal
{
    


    public class AlohaItemInfo : CommandResponse
    {
        public int Barcode { set; get; }
        /// <summary>
        /// Вообще блюдо добавляется с тем именем, что живет в базе Алохе, поэтому данное поле можно не заполнять
        /// </summary>
        public string Name { set; get; }

        public string Comment { set; get; }
        /// <summary>
        /// Для цены по умолчанию Price=-1
        /// </summary>
        public decimal Price { set; get; }
        public List<AlohaItemInfo> Mods = new List<AlohaItemInfo>();
    }


    public class AlohaPaymentInfo : CommandResponse
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public decimal Summ { set; get; }
        public decimal Tip { set; get; }
    }

    public class AlohaDiscountInfo : CommandResponse
    {
        
        public int Id { set; get; }
        public string Name { set; get; }
        public decimal Summ { set; get; }
        public decimal Percent { set; get; }
    }


    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    class AlohaExternal : IAlohaExternal
    {



       private AlohaCheckInfo GetAlohaCheckInfo(Check Chk)
        {
           
            AlohaCheckInfo Tmp = new AlohaCheckInfo()
            {
                AlohaId = Chk.AlohaCheckNum,
                CheckId = Chk.AlohaCheckNum,
                TableId = Chk.TableId,
                TableNum = Chk.TableNumber,
                Summ = Chk.Summ,
                IsClosed =Chk.IsClosed,
                TimeOfOpen = Chk.SystemDateOfOpen,
                TimeOfClose= Chk.SystemDateOfClose2,
                WaiterId = Chk.Waiter,
                WaiterName = AlohaTSClass.GetWaterName(Chk.Waiter),
                DiscountSumm = Chk.Comp,
                NumberInTable = Chk.NumberInTable
            };
            foreach (Dish D in Chk.Dishez)
            {
                if (Chk.Dishez.SelectMany(a => a.CurentModificators).Any(a => a.AlohaNum == D.AlohaNum))
                    {
                    continue;
                }
                Tmp.Dishez.Add(GetAlohaDishInfo(D));
            }
            
            return Tmp;
        }

        private AlohaItemInfo GetAlohaDishInfo(Dish D)
        {
            AlohaItemInfo Tmp = new AlohaItemInfo()
            {
                AlohaId = D.AlohaNum,
                Barcode = D.BarCode,
                Name = D.Name,
                Price = D.Price,
                Mods = new List<AlohaItemInfo>()
            };
            if (D.CurentModificators != null && D.CurentModificators.Count > 0)
            {
                foreach (var m in D.CurentModificators)
                {
                    if (m.BarCode == 999902)
                    {
                        Tmp.Comment += Environment.NewLine+ m.Name;
                    }
                    else
                    {

                        Tmp.Mods.Add(GetAlohaDishInfo(m));
                    }
                }
            }
            return Tmp;
        }



        public DateTime? GetBDPlease()
        {
            try
            {
                return AlohainiFile.BDate;
            }
            catch(Exception e)
            {
                return null;
            }        
        }

        public GetMenuResponse GetMenu()
        {
            Utils.ToCardLog("AlohaExternal GetMenu");
            GetMenuResponse Tmp = new GetMenuResponse();
            Tmp.Mnu = AlohaTSClass.CurentAlohaMnu;
            return Tmp;
        }

        public GetMenuResponse2 GetMenu2()
        {
            Utils.ToCardLog("AlohaExternal GetMenu");
            GetMenuResponse2 Tmp = ConvertMenu(AlohaTSClass.CurentAlohaMnu);
            
            return Tmp;
        }

        private GetMenuResponse2 ConvertMenu(StopListService.AlohaMnu Mnu)
        {
            GetMenuResponse2 Tmp = new GetMenuResponse2();
            AlohaMnuExt MnuExt = new AlohaMnuExt ();
            MnuExt.DepId = Mnu.Dep;
            MnuExt.SubMnus = new List<AlohaSubMnuExt>();
            Tmp.Mnus = new List<AlohaMnuExt>();
            Tmp.Mnus.Add(MnuExt);
            if (Mnu.Smnus == null) return Tmp;
            foreach (StopListService.AlohaSMnu Smnu in Mnu.Smnus)
            {
                AlohaSubMnuExt AlSmnu = new AlohaSubMnuExt ()
                {
                LongName =Smnu.Name,
                Id = Smnu.Id,
                Items = new List<AlohaItemExt> ()
                };
                if (Smnu.Dishes == null) continue;
                foreach (StopListService.AlohaDish ad in Smnu.Dishes)
                {
                AlohaItemExt itm = new AlohaItemExt ()
                {
                    LongName = ad.Name,
                    Id = ad.BarCode,
                    Price = (int)(ad.Price*100),
                    EngName = ad.EngName,
                    ModGroups = new List<AlohaModGroupeExt> ()
                    
                };

                foreach (StopListService.AlohaModGroupe AlModGr in ad.ModGroups)
                {
                    AlohaModGroupeExt NAlohaGr = new AlohaModGroupeExt() {
                        Id = AlModGr.Id,
                        LongName = AlModGr.Name,
                        Mods = new List<AlohaModExt> ()
                    };

                    

                    foreach (StopListService.AlohaMod Nm in AlModGr.Mods)
                    {
                        AlohaModExt NMod = new AlohaModExt() { 
                        Id = Nm.BarCode,
                        LongName = Nm.Name,
                        Price =(int)(Nm.Price*100),
                        };
                        NAlohaGr.Mods.Add(NMod);
                    }



                    itm.ModGroups.Add(NAlohaGr);
                }
                AlSmnu.Items.Add(itm);
                }
               
                MnuExt.SubMnus.Add(AlSmnu);
            }

            return Tmp;
        }

        public bool PrepareCommand(AddEntityRequest Request, CommandResponse Resp)
        {
            Utils.ToCardLog("PrepareCommand ");
            Resp.Success = true;
            if (!UniversalHost.AddRecivedCommand(Resp))
            {
                Utils.ToCardLog("PrepareCommand Command allready recived");
                Resp.Success = false;
                Resp.ErrorMsg = String.Format("Command allready recived", Request.TableNumber);
                Resp.IntegrationErrorCode = -1;
                return false;
            }

            if (Request.AlohaCheckId == 0)
            {
                AlohaTableInfoResponse TResp = new AlohaTableInfoResponse();
                TResp.TNum = Request.TableNumber;
                TResp.Success=true;
                List<Check> Chks = AlohaTSClass.GetChecksOfTableExternal(TResp);
                Resp.AlohaId = TResp.AlohaId;
                Resp.AlohaErrorCode = TResp.AlohaErrorCode;
                Resp.Success = TResp.Success;
                if (Chks != null)
                {
                    if (Chks.Count > 0)
                    {
                        Request.AlohaCheckId = Chks[0].AlohaCheckNum;
                    }
                }
            }
            if (Request.AlohaCheckId == 0)
            {
                Resp.Success = false;
                return false;
            }
            return true;
        }

        public AddItemsResponse AddItems(AddItemsRequest Request)
        {
            Utils.ToCardLog(String.Format("AlohaExternal AddItems Count {0}", Request.Items.Count));
            AddItemsResponse Resp = new AddItemsResponse();
            Resp.RequestId = Request.RequestId;
            
            if (!PrepareCommand(Request,Resp))
            {
                return Resp;
            }
            AlohaTSClass.AddDishFromExternal(Request, Resp);
            UniversalHost.ComplitedRecivedCommand(Resp);
            return Resp;
        }

        public ApplyDiscountsResponse ApplyDiscounts(ApplyDiscountsRequest Request)
        {
            Utils.ToCardLog("AlohaExternal ApplyDiscounts");
            ApplyDiscountsResponse Resp = new ApplyDiscountsResponse();
            Resp.RequestId = Request.RequestId;
            if (!PrepareCommand(Request, Resp))
            {
                return Resp;
            }
            AlohaTSClass.ApplyCompExternal(Request, Resp);
            UniversalHost.ComplitedRecivedCommand(Resp);
            return Resp;
        }

        public AddPaymentsResponse AddPayments(AddPaymentsRequest Request)
        {
            Utils.ToCardLog("AlohaExternal AddPayments");
            AddPaymentsResponse Resp = new AddPaymentsResponse();
            Resp.RequestId = Request.RequestId;
            if (!PrepareCommand(Request, Resp))
            {
                return Resp;
            }
            AlohaTSClass.AddPaymentExternal(Request, Resp);
            UniversalHost.ComplitedRecivedCommand(Resp);
            return Resp;
        }

        public AlohaTableInfoResponse GetTableInfo(int TableNumber)
        {
            Utils.ToCardLog("AlohaExternal GetTableInfo " + TableNumber);
            AlohaTableInfoResponse TInfo = new AlohaTableInfoResponse();
            TInfo.TNum = TableNumber;
            TInfo.Success = true;
            List<Check> Chks = AlohaTSClass.GetChecksOfTableExternal(TInfo);
            if (Chks != null)
            {
                foreach (Check Chk in Chks)
                {
                    TInfo.Checks.Add(GetAlohaCheckInfo(Chk));
                }
                
            }
            return TInfo;
        }

        


        public AlohaCheckInfoResponse GetCheckInfo(int CheckId)
        {
            Utils.ToCardLog("AlohaExternal GetCheckInfo " + CheckId);
            AlohaCheckInfoResponse ChInfo = new AlohaCheckInfoResponse ();
            ChInfo.CheckId = CheckId;
            ChInfo.Success=true;

            var ch = AlohaTSClass.GetCheckByIdExternal(ChInfo);
            if (ch == null)
            {
                ChInfo.Success = false;
                ChInfo.AlohaErrorCode = AlohaErrEnum.ErrCOM_InvalidCheck;
            }
            else
            {
                ChInfo.Check = GetAlohaCheckInfo(ch);
            }


            return ChInfo;
        }
        




        public NewOrderResponse NewOrder(NewOrderRequest Request)
        {
            Utils.ToCardLog("AlohaExternal NewOrder ");
            NewOrderResponse Resp = new NewOrderResponse();
            Resp.RequestId = Request.RequestId;

            if (!PrepareCommand(Request, Resp))
            {
                //return Resp;
            }
            /*
            if (Request.AlohaTableId == 0)
            {
                Request.AlohaTableId = Resp.AlohaId;
            }
            */
            AlohaTSClass.OpenTableFromExternal(Request, Resp);
            UniversalHost.ComplitedRecivedCommand(Resp);
            return Resp;
        }

        public NewOrderResponse NewOrderOnTableRange(NewOrderRequest Request)
        {
            Utils.ToCardLog("AlohaExternal NewOrderOnTableRange ");
            NewOrderResponse Resp = new NewOrderResponse();
            Resp.RequestId = Request.RequestId;

            Resp.Success = true;
            if (!UniversalHost.AddRecivedCommand(Resp))
            {
                Resp.Success = false;
                Resp.ErrorMsg = String.Format("Command allready recived", Request.TableNumber);
                Resp.IntegrationErrorCode = -1;
                return Resp;
            }

            AlohaTSClass.OpenTableFromRangeExternal(Request, Resp);
            UniversalHost.ComplitedRecivedCommand(Resp);
            return Resp;
        }

        public GetStopListResponse GetStopList()
        {
            Utils.ToCardLog("AlohaExternal GetStopList");
            GetStopListResponse Resp = new GetStopListResponse();
            try
            {
                Resp.ItemsBCs = AlohaTSClass.GetStopList2();
            }
            catch(Exception e)
            {
                Resp.Success = false;
                Resp.AlohaErrorCode = CAlohaErrors.GetAlohaErrorVal(e.Message);
                Resp.ErrorMsg = e.Message;
            }
            return Resp;
        }

        public CommandResponse ShowMessage(string Message)
        {
            Utils.ToCardLog("AlohaExternal ShowMessage");
            CommandResponse Resp = new CommandResponse();
            try
            {
                AlohaTSClass.ShowMessageExternal(Message);
            }
            catch (Exception e)
            {
                Resp.Success = false;
                Resp.AlohaErrorCode = CAlohaErrors.GetAlohaErrorVal(e.Message);
                Resp.ErrorMsg = e.Message;
            }
            return Resp;
        }

        public GetCommandStatusResponse GetCommandStatus(Guid CommandId)
        {
            Utils.ToCardLog("AlohaExternal GetCommandStatus " + CommandId);
            GetCommandStatusResponse Resp = new GetCommandStatusResponse();
            PDiscountCard.AlohaExternal.ICommandResponse RecivedCommand ;
            if (UniversalHost.GetRecivedCommand(CommandId, out RecivedCommand))
            {
                Resp.Code = RecivedCommand.Status;
            }
            else
            {
                Resp.IntegrationErrorCode = -2;
                Resp.ErrorMsg = "No command";
            }
            return Resp;
        }

        public GetToGoordersResponse GetToGoOrders()
        {
            Utils.ToCardLog("GetToGoOrdersExternal from ext");

            var res = new GetToGoordersResponse();
            res.Checks = AlohaTSClass.GetToGoOrdersExternal();

            try
            {
                MemoryStream stream1 = new MemoryStream();  
                var ds = new DataContractJsonSerializer(typeof(GetToGoordersResponse));
                ds.WriteObject(stream1, res);
                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                Utils.ToCardLog("JSON res: ");
                string s =sr.ReadToEnd();
                Utils.ToCardLog(s);
            }
            catch(Exception e)
            {
                Utils.ToCardLog("Error GetToGoOrdersExternal show JSON res " +e.Message);
            }

            Utils.ToCardLog("GetToGoOrdersExternal return" );
            return res;
        }

        /*
        public Person GetData(string id)
        {
            throw new NotImplementedException();
        }
         * */

         [STAOperationBehavior]
        public CommandResponse PrintOrder(string  orderId)
        {

            var resp = new CommandResponse() { Success =true};
            try
            {

                Utils.ToCardLog("PrintOrderLabel orderId: " + orderId);
                //MainClass.MainThread.
               // Thread.CurrentThread.SetApartmentState(ApartmentState.STA);


                int ordid = Convert.ToInt32(orderId);
                var respon = new AlohaCheckInfoResponse()
                    {CheckId = ordid}
                    ;
                var chk= AlohaTSClass.GetCheckByIdExternal(respon);
                if (respon.Success)
                {
                    string err="";
                    bool res =  PDiscountCard.PrintOrder.PrintOrder.Instanse.PrintToGoOrderLabels(chk,out err)    ;
                    if (!res)
                    {
                        resp.Success = false;
                        resp.ErrorMsg = err;
                    }
                }
                else
                {
                    resp=respon;
                }
            }
            catch(Exception e)
            {
                resp.Success = false;
                resp.ErrorMsg = e.Message;
                Utils.ToCardLog("Error PrintOrderLabel " + e.Message);
            }
            return resp;    
            

        }

        public GetEmloyeeListResponse GetEmployeeList()
        {
            Utils.ToCardLog("GetEmployeeList from ext");
            var res = new GetEmloyeeListResponse();
            res.Empls = AlohaTSClass.GetEmplListExternal();
            return res;
        }
    }
}
