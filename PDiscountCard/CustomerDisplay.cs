using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard
{
    public static class CCustomerDisplay
    {

        public static bool DisplayInited = false;
        public static void Init()
        {
            CustomerDisplay.MainClass.Init();
            DisplayInited = true;
        }

        internal static void SetCheck(Check Chk)
        {
            if (DisplayInited)
            {
                CustomerDisplay.MainClass.SetCheck(GetCustomerCheck(Chk));
            }
        }

        internal static void WaitForRemoval()
        {
            if (DisplayInited)
            {
                CustomerDisplay.MainClass.SetStatus("Возьмте вашу сдачу");
                CustomerDisplay.MainClass.SetPictureState(2);
            }
            
        }
        internal static void WaitForRemovalReject()
        {
            if (DisplayInited)
            {
                CustomerDisplay.MainClass.SetStatus("Возьмите непринятые банкноты");
                CustomerDisplay.MainClass.SetPictureState(2);
            }

        }
        internal static void EndChange(int Change)
        {
            if (DisplayInited)
            {
                if (Change == 0)
                {
                    CustomerDisplay.MainClass.SetStatus("Спасибо за покупку!");
                }
                else
                {
                    CustomerDisplay.MainClass.SetStatus("В устройстве недостаточно сдачи. Возьмите вашу сдачу у кассира.");

                }

                CustomerDisplay.MainClass.EndPayment();
            }
        }
        internal static void CancelChange(int Change)
        {
            if (DisplayInited)
            {
                CustomerDisplay.MainClass.ChangeCanceled(Change);

                CustomerDisplay.MainClass.EndPayment();
            }
        }


        internal static void StartChange(int Summ)
        {
            if (DisplayInited)
            {
                CustomerDisplay.MainClass.SetStatus("Пожалуйста, внесите денежные средства");
                CustomerDisplay.MainClass.SetTotal(Summ);
                CustomerDisplay.MainClass.SetPictureState(1);
            }
        }
        internal static void UpDateDeposit(int Summ)
        {
            if (DisplayInited)
            {
                CustomerDisplay.MainClass.SetDeposit(Summ);
            }
        }

        private static CustomerDisplay.CustomerCheck GetCustomerCheck(Check Chk)
        {
            CustomerDisplay.CustomerCheck CC = new CustomerDisplay.CustomerCheck()
            {
                AlohId = Chk.AlohaCheckNum,
                Ammount = (int)(Chk.Summ*100)

            };
            foreach (Dish D in Chk.Dishez)
            {
                CustomerDisplay.CustomerDish CD = new CustomerDisplay.CustomerDish()
                {
                    Name = D.LongName,
                    Price = D.Price
                };
                CC.Dishes.Add(CD);
            }
            
            return CC;



        }

    }
}
