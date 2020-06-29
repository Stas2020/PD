using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OrderToAlohaSrv
{
    static public  class ToSql
    {

        //public static string SrvName = @"Aloha1\SQLExpress";

        private static string ConnectionString = "";

        static public void SetConnStr(string SrvName)
        {
            ConnectionString = @"Data Source=" + SrvName + ";Initial Catalog=AlohaGallery;User ID=AlohaOper;Password=Qdrjnty^pp1223";
             db = new OrdersDbDataContext(ConnectionString);
        }

      static  OrdersDbDataContext db = null;
        //private static string ConnectionString = @"Data Source=(local)\SQLEXPRESS;  Initial Catalog=AlohaGallery;  Integrated Security=True";
        static public  Order GetNonCloseCheck()
        {
           // Utils.ToLog("GetNonCloseCheck: " );
            

            try
            {
                Order res = (from o in db.Orders where (o.Closed.Value && !o.FiscalClose.Value) select o).FirstOrDefault();

             //   db.Connection.Close();
             //   db.Dispose();
                return res;
            }
            catch(Exception e)
            {
                Utils.ToLog("Error GetNonCloseCheck Conn: " + ConnectionString+" mess" + e.Message);
                return null;
            }

        }


        static public Order GetNeedPrecheck()
        {
            // Utils.ToLog("GetNonCloseCheck: " );


            try
            {
                Order res = (from o in db.Orders where (o.Closed.Value && !o.PreCheck) select o).FirstOrDefault();

                //   db.Connection.Close();
                //   db.Dispose();
                return res;
            }
            catch (Exception e)
            {
                Utils.ToLog("Error GetNonCloseCheck Conn: " + ConnectionString + " mess" + e.Message);
                return null;
            }

        }

        static public void UpdateCloseCheck(int StNum)
        {
            OrdersDbDataContext db = new OrdersDbDataContext(ConnectionString);

            try
            {
                Order res = (from o in db.Orders where (o.StNum==StNum) select o).FirstOrDefault();
                res.FiscalClose = true;

                db.SubmitChanges();
            }
            catch
            {
                
            }

        }


        static public void UpdatePrintPreCheck(int StNum)
        {
            OrdersDbDataContext db = new OrdersDbDataContext(ConnectionString);

            try
            {
                Order res = (from o in db.Orders where (o.StNum == StNum) select o).FirstOrDefault();
                res.PreCheck = true;

                db.SubmitChanges();
            }
            catch
            {

            }

        }
        static internal void AddItmToBase(int BarCode, string Name)
        {
            OrdersDbDataContext db = new OrdersDbDataContext(ConnectionString);

            
            var res = (from o in db.Itms where (o.BarCode == BarCode) select o).FirstOrDefault();
            if (res != null)
            {
                if (Name != "")
                {
                    res.Name = Name;
                    db.SubmitChanges();
                }
            }
            else
            {
                res = new Itm()
                {
                    BarCode = BarCode,
                    Name = Name,
                    
                };
                db.Itms.InsertOnSubmit(res);
                db.SubmitChanges();
            }
            
        }



        static internal void AddOrder(int OrderId, List<OrderToAloha.Item> Items, int CompanyId, string CompanyName, string BortName, int DiscountId, int Margin, DateTime TimeOfShipping, decimal FreeDisc, int AlohaCheckId, int AlohaTableNum)
        {
            try
            {
                Utils.ToLog("ConnectionString = " + ConnectionString);
            OrdersDbDataContext db = new OrdersDbDataContext(ConnectionString);
            if (DiscountId == null)
           {
               DiscountId = 0;
            }

            if (Margin == null)
            {
                Margin = 0;
            }

            foreach (OrderToAloha.Item itm in Items)
            {
                if (itm.Name == null)
                {
                    //Utils.ToLog("itm.Name == null " + itm.BarCode);
                }
                else
                {
                    //Utils.ToLog("Add itm.Name " + itm.BarCode);
                    //AddItmToBase(itm.BarCode, itm.Name);
                    AddItmToBase(itm.AlohaBarCode, itm.Name);
                }
            }

            Order order;
            var res = (from o in db.Orders where (o.StNum == OrderId) select o).FirstOrDefault();
            Utils.ToLog("FreeDisc: " + FreeDisc.ToString());
            if (res != null)
            {

                if (res.Closed.Value)
                {
                    Utils.ToLog("Заказ закрыт");
                    return;
                }
                order = res;
                Utils.ToLog("Изменение старого заказа");
                try
                {
                    //TableNumId = Achk.TableNumber;
                    //TId = OldTId;

                    AlohaCheck Achk = GetChk(OrderId);
                    List<OrderToAloha.Item> AddItms = new List<OrderToAloha.Item>();
                    List<OrderToAloha.Item> DelItms = new List<OrderToAloha.Item>();
                    order.TimeofShipping = TimeOfShipping;

                    if (order.Summ != Items.Sum(a => a.Price))
                    {
                        order.OrderItems.Clear();
                        foreach (OrderToAloha.Item it in Items)
                        {
                            OrderItem OI = new OrderItem()
                            {
                                Barcode = it.AlohaBarCode,
                                Deleted = false,
                                Price = it.Price,
                                DiscPrice = it.Price,
                                Quantity = it.Count,
                                Alk =(it.SourceBase==1)
                            };


                            if (DiscountId == 1)
                            {
                                OI.DiscPrice = OI.Price * (decimal)0.95;

                            }
                            else if (DiscountId == 2)
                            {
                                OI.DiscPrice = OI.Price * (decimal)0.9;

                            }
                            //OI.DiscPrice = OI.Price -FreeDisc;

                            order.OrderItems.Add(OI);
                        }
                    }
                  
                    order.Summ = order.OrderItems.Where(a=>!a.Deleted.GetValueOrDefault()).Sum(a => a.Price * (decimal)a.Quantity);
                    if (FreeDisc == null)
                    {
                        FreeDisc = 0;
                    }
                    if (FreeDisc == 0)
                    {
                        if (DiscountId == 1)
                        {
                            order.StDiscount = order.Summ.Value * (decimal)0.05;

                        }
                        else if (DiscountId == 2)
                        {
                            order.StDiscount = order.Summ.Value * (decimal)0.1;
                        }
                        else
                        {
                            order.StDiscount = FreeDisc;
                        }
                    }
                    else
                    {
                        order.StDiscount = FreeDisc;
                    }


                    order.MarginId = (Margin == 0 ? 0 : 1);
                    order.Margin = order.Summ.Value * ((decimal)Margin) / 100;
                    /*
                    if (Margin == 2)
                    {
                        order.Margin = order.Summ.Value * (decimal)0.1;
                    }
                    else
                    {
                        order.Margin = 0;
                    }
                    */
                    //AlohaFuncs.SelectAllEntriesOnCheck(Properties.Settings.Default.TerminalNumber, ChId);
                    //  AlohaFuncs.ManagerVoidSelectedItems(Properties.Settings.Default.TerminalNumber, Properties.Settings.Default.ManagerNumber, ChId, Properties.Settings.Default.VoidReason);
                }
                catch (Exception e)
                {
                    //AlohaException ae = new AlohaException("SQL Ошибка удаления заказа № " + OrderId, e);
                    //   ae.AlohaErrorDescription = (new CAlohaErrors(e.Message)).ValStr;
                    //throw ae;
                    Utils.ToLog("SQL Ошибка удаления заказа № " + OrderId);
                }

            }
            else
            {
                
                    Utils.ToLog("Новый заказ");
                    order = new Order();
                    db.Orders.InsertOnSubmit(order);
                    Utils.ToLog("1");

                    order.AlohaNumber = AlohaCheckId;
                    order.AlohaTable = AlohaTableNum.ToString();
                    order.ComplexDisc = 0;
                    order.OpenTime = DateTime.Now;
                    order.StNum = OrderId;
                    Utils.ToLog("2");    
                order.TimeofShipping = TimeOfShipping;
                    order.Closed = false;
                    order.PreCheck = false;
                    order.FiscalClose = false;
                    order.OrderItems = new System.Data.Linq.EntitySet<OrderItem>();

                    foreach (OrderToAloha.Item it in Items)
                    {
                        OrderItem OI = new OrderItem()
                        {
                            Barcode = it.AlohaBarCode,
                            Deleted = false,
                            Price = it.Price,
                            DiscPrice = it.Price,
                            Quantity = it.Count,
                            Alk = (it.SourceBase == 1)
                        };
                        Utils.ToLog("3");
                       
                        if (DiscountId == 1)
                        {
                            OI.DiscPrice = OI.Price * (decimal)0.95;

                        }
                        else if (DiscountId == 2)
                        {
                            OI.DiscPrice = OI.Price * (decimal)0.9;

                        }
                        //OI.DiscPrice = OI.Price -FreeDisc;

                        order.OrderItems.Add(OI);
                    }



                }
            Utils.ToLog("4");
                order.BortName = BortName;
                order.CompanyId = CompanyId;
                order.CompanyName = CompanyName;
                order.StDiscId = DiscountId;
             //   order.MarginId = Margin;

                Utils.ToLog("5");

                order.Summ = order.OrderItems.Where(a => !a.Deleted.GetValueOrDefault()).Sum(a => a.Price * (decimal)a.Quantity);
                if (FreeDisc == null)
                {
                    FreeDisc = 0;
                }
                if (FreeDisc == 0)
                {
                    if (DiscountId == 1)
                    {
                        order.StDiscount = order.Summ.Value * (decimal)0.05;

                    }
                    else if (DiscountId == 2)
                    {
                        order.StDiscount = order.Summ.Value * (decimal)0.1;
                    }
                    else
                    {
                        order.StDiscount = FreeDisc;
                    }
                }
                else
                {
                    order.StDiscount = FreeDisc;
                }
                Utils.ToLog("6");
                /*
                if (Margin == 2)
                {
                    order.Margin = order.Summ.Value * (decimal)0.1;
                }
                else
                {
                    order.Margin = 0;
                }
                 * */
                order.MarginId = (Margin == 0 ? 0 : 1);
                order.Margin = order.Summ.Value * ((decimal)Margin)/100;
                db.SubmitChanges();
            }
            catch(Exception e)
            {
                Utils.ToLog("AddOrder Error " + e.Message);
            }

        }
        public static string SQLGetDishName(int Barcode)
        {
            Dictionary<int, string> Tmp = new Dictionary<int, string>();
            OrdersDbDataContext db = new OrdersDbDataContext(ConnectionString);
            try
            {
                string n = (from o in db.Itms where o.BarCode == Barcode select o.Name).First();
                return n;
            }
            catch
            {
                return "Открытое блюдо";
            }

            
        }

        

        internal static AlohaCheck GetChk(int StOrderId)
        {
            OrdersDbDataContext db = new OrdersDbDataContext(ConnectionString);

            Order order;

            
            var res = (from o in db.Orders where (o.StNum == StOrderId) select o).FirstOrDefault();

            if (res == null)
            {
                return null;

            }
            else
            {
                AlohaCheck AChk = new AlohaCheck()
                {
                    AlohaId = res.AlohaNumber.GetValueOrDefault(),
                    DiscId = res.OrderDiscId.GetValueOrDefault(),
                    DiscTypeId = res.OrderDiscId.GetValueOrDefault(),
                    STId = res.StNum.GetValueOrDefault(),
                    Summ = res.Summ.GetValueOrDefault(),
                    TableNumber = int.Parse(res.AlohaTable)

                };

                AChk.items = new List<OrderToAloha.Item> ();
                foreach (OrderItem Oi in res.OrderItems.Where(a=>!a.Deleted.Value))
                {
                    OrderToAloha.Item it = new OrderToAloha.Item()
                    {
                        BarCode = Oi.Barcode.GetValueOrDefault(),
                        Count = Oi.Quantity.GetValueOrDefault(),
                        EntryId = Oi.EntryId.GetValueOrDefault(),
                        Price = Oi.Price.GetValueOrDefault(),
                    };
                    AChk.items.Add(it);
                }
                

                return AChk;
            }
            
        }


    }
}
