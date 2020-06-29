using System;
using System.Collections.Generic;
using System.Text;

namespace PDiscountCard
{
    static public   class OrderDivider
    {
        private static  List<int> KitchenGroups = new List<int>();
        static     OrderDivider()
        {
            KitchenGroups = new List<int> { 6, 19, 1, 70, 3, 2, 4, 7, 8, 23, 10, 16, 27, 28, 30, 31,22 };
        }




        public static void HideWindow()
        {

            IntPtr Hwnd = WinApi.GetForegroundWindow();
            //IntPtr Hwnd2 = WinApi.FindWindowEx(Hwnd, IntPtr.Zero, "", "");
            IntPtr Hwnd3 = WinApi.FindWindowEx(Hwnd, IntPtr.Zero, "Button", "Заказ");
            WinApi.ShowWindow(Hwnd3, WinApi.ShowWindowCommands.Hide);

        
        }

        public static void OrderItems(bool All)
        {
            AlohaTSClass.CheckWindow();
            List<OrderDish> OdList = new List<OrderDish>();
            
                OdList = AlohaTSClass.GetSelectedItems(All);
            
            List<OrderDish> OdListKitchen = new List<OrderDish>();
            List<OrderDish> OdListOther = new List<OrderDish>();
            foreach (OrderDish Od in OdList)
            {
                if (KitchenGroups.Contains(Od.Vrouting))
                {
                    OdListKitchen.Add(Od);
                }
                else
                {
                    OdListOther.Add(Od); 
                }
            }
            AlohaTSClass.OrderListDish(OdListKitchen);
            AlohaTSClass.OrderListDish(OdListOther);
            AlohaTSClass.RefreshCheckDisplay(); 
        }
    }
}
