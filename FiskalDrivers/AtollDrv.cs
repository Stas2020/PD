using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;


namespace FiskalDrivers
{

    class AtollDrviver: IFiskalDriver    
    {
        private int CheckWidth = AtollInteruptDrviver.CheckWidth;
        private bool CheckState(bool CancelCheck, out bool ErrorExit)
        {
            ErrorExit = false;
            if (AtollInteruptDrviver.ResultCode != 0)
            {
                Utils.ToLog("CheckState Error ResultCodeDescription=" + AtollInteruptDrviver.ResultDescription);
                if (AtollInteruptDrviver.ResultCode != -1 && AtollInteruptDrviver.ResultCode != -3807)
                {
                    String OldDescr = AtollInteruptDrviver.ResultDescription;
                    Int32 OldCode = AtollInteruptDrviver.ResultCode;
                    Int32 OldMode = AtollInteruptDrviver.Mode;

                    if (CancelCheck)
                    {
                    AtollInteruptDrviver.CancelCheck();
                    }
                    string Mess = "Ошибка выполнения команды фискальным регистратором." + Environment.NewLine +
                      "Код ошибки: " + OldCode + Environment.NewLine +
                      OldDescr + Environment.NewLine +
                      "устраните ошибку и продолжите печать" + Environment.NewLine +
                      "Режим: " + OldMode.ToString();

                    MF3 Mf = new MF3(Mess);
                    Mf.TopMost = true;


                    Mf.ShowDialog();
                    ErrorExit = Mf.CancelCheck;
                    return false;
                    
                }
                else
                {
                    string Mess = "Ошибка выполнения команды фискальным регистратором."+Environment.NewLine+
                        "Код ошибки: "+AtollInteruptDrviver.ResultCode.ToString()+Environment.NewLine+
                        AtollInteruptDrviver.ResultDescription+Environment.NewLine+
                        "устраните ошибку и продолжите печать"+
                        "Режим: " + AtollInteruptDrviver.Mode.ToString();
                    MF3 Mf = new MF3(Mess);
                    Mf.TopMost = true;
                    
                    
                    Mf.ShowDialog();
                    ErrorExit = Mf.CancelCheck;
                    return false;
                }

                // return false;
            }
            return true;
        }



        public bool PrintPreCheck(FiskalCheck FskChk)
            {

                Utils.ToLog("PrintPreCheck " +FskChk.CheckNum);

                bool Sucsecc = false;
                bool ErrorExit = false;
                AtollInteruptDrviver.DeviceEnabled = true;
                while (!Sucsecc && (!ErrorExit))
                {
                    AtollInteruptDrviver.Password = 30;
                    AtollInteruptDrviver.Mode = 1;
                    AtollInteruptDrviver.SetMode();
                    foreach (String s in FskChk.CaptionInfoStrings)
                    {
                        Utils.ToLog("PrintString " + s);
                        AtollInteruptDrviver.Caption = s;
                        AtollInteruptDrviver.PrintString();
                        if (!CheckState(true, out ErrorExit)) continue;
                        Utils.ToLog("PrintString ok" + s);
                    }


                    AtollInteruptDrviver.Caption = new string(" "[0], CheckWidth);
                    AtollInteruptDrviver.PrintString();
                    if (!CheckState(true, out ErrorExit)) continue;
                    /*
                    AtollInteruptDrviver.Caption = "Блюдо                    Кол-во   Сумма";
                    AtollInteruptDrviver.PrintString();
                    if (!CheckState(true, out ErrorExit)) continue;

                    AtollInteruptDrviver.Caption = new String("-"[0], CheckWidth);
                    AtollInteruptDrviver.PrintString();
                    if (!CheckState(true, out ErrorExit)) continue;

                    foreach (FiskalDish item in FskChk.Dishes)
                    {
                        Utils.ToLog(String.Format("{0} , {1}, {2}", item.Name, item.Price, item.Quantity));
                        if (item.Price == 0)
                        {
                            continue;

                        }
                        AtollInteruptDrviver.Caption = AtollInteruptDrviver.Name.PadRight(CheckWidth-10) + (AtollInteruptDrviver.Quantity.ToString("") + "x" + AtollInteruptDrviver.Price.ToString("0.00")).PadLeft(9);
                        AtollInteruptDrviver.PrintString();
                        
                        if (!CheckState(true, out ErrorExit)) continue;
                    }

                    AtollInteruptDrviver.Caption = new String("-"[0], CheckWidth);
                    AtollInteruptDrviver.PrintString();
                    if (!CheckState(true, out ErrorExit)) continue;
                    */
                    if (FskChk.Discount > 0)
                    {
                        AtollInteruptDrviver.Caption = "Скидка".PadRight(CheckWidth-10)+ FskChk.Discount.ToString("0.00").PadLeft(9);
                        AtollInteruptDrviver.PrintString();
                        if (!CheckState(true, out ErrorExit)) continue;
                    }
                    if (FskChk.Charge > 0)
                    {
                        AtollInteruptDrviver.Caption = "Надбавка".PadRight(CheckWidth-10) + FskChk.Charge.ToString("0.00").PadLeft(9);
                        AtollInteruptDrviver.PrintString();
                        if (!CheckState(true, out ErrorExit)) continue;
                    }
                    AtollInteruptDrviver.Caption = "Оплата                       ";
                    AtollInteruptDrviver.PrintString();
                    if (!CheckState(true, out ErrorExit)) continue;
                    foreach (FiskalPayment P in FskChk.Payments)
                    {
                        AtollInteruptDrviver.Caption = P.Name.PadRight(30) + P.Summ.ToString("0.00").PadLeft(9);
                        AtollInteruptDrviver.PrintString();
                        if (!CheckState(true, out ErrorExit)) continue;
                    }
                    AtollInteruptDrviver.Caption = "ИТОГ".PadRight(CheckWidth - 10) + FskChk.Summ.ToString().PadLeft(9);
                    AtollInteruptDrviver.PrintString();
                    if (!CheckState(true, out ErrorExit)) continue;
                    Sucsecc = true;
                    
                }
                AtollInteruptDrviver.DeviceEnabled = false;
                return Sucsecc && (!ErrorExit);
            }

        public  bool CloseCheck(FiskalCheck FskChk)
        {
            bool Sucsecc=false;
            bool ErrorExit = false;
            AtollInteruptDrviver.DeviceEnabled = true;
            while (!Sucsecc && (!ErrorExit))
            {
                //AtollInteruptDrviver.setParam(1021, "Кассир Иванов И.");
                AtollInteruptDrviver.Password = 30;
                AtollInteruptDrviver.Mode = 1;
                AtollInteruptDrviver.SetMode();
                AtollInteruptDrviver.NewDocument();
                Utils.ToLog("AtollInteruptDrviver.NewDocument();");
                if (!CheckState(false, out ErrorExit)) continue;

                if (FskChk.IsVoid)
                {
                    AtollInteruptDrviver.CheckType = 2;
                }
                else
                {
                    AtollInteruptDrviver.CheckType = 1;
                }

                AtollInteruptDrviver.OpenCheck();
                Utils.ToLog("AtollInteruptDrviver.OpenCheck();");
                if (!CheckState(true, out ErrorExit)) continue;
                foreach (String s in FskChk.CaptionInfoStrings)
                {

                    AtollInteruptDrviver.Caption = s;
                    AtollInteruptDrviver.PrintString();
                    if (!CheckState(true, out ErrorExit)) continue;
                }
                
                AtollInteruptDrviver.Caption = new string(" "[0], CheckWidth);
                AtollInteruptDrviver.PrintString();
                if (!CheckState(true, out ErrorExit)) continue;

                AtollInteruptDrviver.Caption = "Блюдо                    Кол-во   Сумма";
                AtollInteruptDrviver.PrintString();
                if (!CheckState(true, out ErrorExit)) continue;
                
                AtollInteruptDrviver.Caption = new String("-"[0], CheckWidth);
                AtollInteruptDrviver.PrintString();
                if (!CheckState(true, out ErrorExit)) continue;

                foreach (FiskalDish item in FskChk.Dishes)
                {
                    Utils.ToLog(String.Format("{0} , {1}, {2}",item.Name,item.Price, item.Quantity));
                    if (item.Price == 0)
                    {
                        continue;
                    }
                    AtollInteruptDrviver.Department = 0;
                    AtollInteruptDrviver.AdvancedRegistration = true;
                    AtollInteruptDrviver.Name = item.GetNameAndCountString();
                    AtollInteruptDrviver.Price = item.Price;
                    AtollInteruptDrviver.Quantity = item.Quantity;
                    if (FskChk.IsVoid)
                    {
                        AtollInteruptDrviver.EnableCheckSumm = false;
                        AtollInteruptDrviver.Return();
                    }
                    else
                    {
                        Utils.ToLog("AtollInteruptDrviver.Registration();");
                        AtollInteruptDrviver.Registration();
                    }

                    if (!CheckState(true, out ErrorExit)) continue;
                }

                AtollInteruptDrviver.Caption = new String("-"[0], CheckWidth);
                AtollInteruptDrviver.PrintString();
                if (!CheckState(true, out ErrorExit)) continue;

                if (FskChk.Discount > 0)
                {
                    /*
                    AtollInteruptDrviver.Destination = 0;
                    AtollInteruptDrviver.Summ = FskChk.Discount;
                    Utils.ToLog("AtollInteruptDrviver.SummDiscount() " + FskChk.Discount);
                    AtollInteruptDrviver.SummDiscount();
                     * */

                    string discStr = FskChk.DiscountName;
                    AtollInteruptDrviver.Caption = FskChk.GetDiscountString(CheckWidth);
                    Utils.ToLog("AtollInteruptDrviver.PrintString() Discount" + AtollInteruptDrviver.Caption);
                    AtollInteruptDrviver.PrintString();
                    if (!CheckState(true, out ErrorExit)) continue;
                    
                }
                if (FskChk.Charge > 0)
                {
                    /*
                    AtollInteruptDrviver.Destination = 0;
                    AtollInteruptDrviver.Summ = FskChk.Charge;
                    Utils.ToLog("AtollInteruptDrviver.SummCharge()");
                    AtollInteruptDrviver.SummCharge();
                     * */


                    AtollInteruptDrviver.Department = 0;
                    AtollInteruptDrviver.AdvancedRegistration = true;
                    AtollInteruptDrviver.Name = "Надбавка ";
                    AtollInteruptDrviver.Price = FskChk.Charge;
                    AtollInteruptDrviver.Quantity = 1;
                    
                    Utils.ToLog("AtollInteruptDrviver.Registration() for Charge;");
                    AtollInteruptDrviver.Registration();

                    if (!CheckState(true, out ErrorExit)) continue;
                }


                AtollInteruptDrviver.Caption = new string("-"[0], CheckWidth);
                Utils.ToLog("AtollInteruptDrviver.PrintString()");
                AtollInteruptDrviver.PrintString();
                if (!CheckState(true, out ErrorExit)) continue;

                AtollInteruptDrviver.Caption = new string(" "[0], CheckWidth);
                Utils.ToLog("AtollInteruptDrviver.PrintString()");
                AtollInteruptDrviver.PrintString();
                if (!CheckState(true, out ErrorExit)) continue;


                foreach (FiskalPayment P in FskChk.Payments)
                {
                    if (P.Summ == 0) continue;
                    AtollInteruptDrviver.Caption = P.GetPaymentNameString(CheckWidth);
                    Utils.ToLog("AtollInteruptDrviver.PrintString() Payment " + P.GetPaymentNameString(CheckWidth));
                    AtollInteruptDrviver.PrintString();
                    if (!CheckState(true, out ErrorExit)) continue;

                }

                AtollInteruptDrviver.Caption = new string("-"[0], CheckWidth);
                AtollInteruptDrviver.PrintString();
                if (!CheckState(true, out ErrorExit)) continue;

                foreach (FiskalPayment P in FskChk.Payments)
                {
                    AtollInteruptDrviver.Summ = P.Summ;
                    AtollInteruptDrviver.TypeClose = P.PaymentType;
                    Utils.ToLog("Payment() Type: " + P.PaymentType);
                    AtollInteruptDrviver.Payment();
                    if (!CheckState(true, out ErrorExit)) continue;
                }
                Utils.ToLog("CloseCheck()");
                    AtollInteruptDrviver.CloseCheck();
                if (!CheckState(true, out ErrorExit)) continue;
                Sucsecc = true;

            }
            AtollInteruptDrviver.DeviceEnabled = false;
            return Sucsecc && (!ErrorExit);
        }

        public  bool Create()
        {
            return AtollInteruptDrviver.CreateAtollInteruptDrviver();
        }

        public bool Connect(int LUNumber)
        {
          

            try
            {
                AtollInteruptDrviver.CurrentDeviceNumber = LUNumber;
                AtollInteruptDrviver.DeviceEnabled = true;
                AtollInteruptDrviver.DeviceEnabled = false;
                
                if (AtollInteruptDrviver.ResultCode != 0)
                {
                    Utils.ToLog("Биксилон принтер LUNumber: " + LUNumber + " не подключен ResultCode= " + AtollInteruptDrviver.ResultCode);
                    Utils.ToLog("ResultCodeDescription=" + AtollInteruptDrviver.ResultDescription);
                    return false;
                }
                else
                {
                   // Utils.ToLog("ResCode=" + ResultCodeDescription);
                    Utils.ToLog("Нашел принтер скорость: " + AtollInteruptDrviver.BaudRate + " порт: " + AtollInteruptDrviver.PortNumber);


                    return true;
                }
            }
            catch (Exception e)
            {


           
                return false;
            }
        
        }


        public void PrintString(string Str)
        {
            AtollInteruptDrviver.Caption = Str;
            AtollInteruptDrviver.PrintString();
        
        }

        public void PrintXReport()
        {

           bool Sucsecc=false;
            bool ErrorExit = false;
            AtollInteruptDrviver.DeviceEnabled = true;
            while (!Sucsecc && (!ErrorExit))
            {
                
                AtollInteruptDrviver.Mode = 2;
                AtollInteruptDrviver.SetMode();
                if (!CheckState(true, out ErrorExit)) continue;
                AtollInteruptDrviver.ReportType = 2;
                AtollInteruptDrviver.Report();
                if (!CheckState(true, out ErrorExit)) continue;
                Sucsecc = true;
            }
            AtollInteruptDrviver.DeviceEnabled = false;
        }
        public void PrintZReport()
        {

            bool Sucsecc = false;
            bool ErrorExit = false;
            AtollInteruptDrviver.DeviceEnabled = true;
            while (!Sucsecc && (!ErrorExit))
            {
                AtollInteruptDrviver.Mode = 3;
                AtollInteruptDrviver.SetMode();
                if (!CheckState(true, out ErrorExit)) continue;
                AtollInteruptDrviver.ReportType = 1;
                AtollInteruptDrviver.Report();
                if (!CheckState(true, out ErrorExit)) continue;
                Sucsecc = true;
            }
            AtollInteruptDrviver.DeviceEnabled = false;
        }
      
    }



    public static class AtollInteruptDrviver
    {
        static internal int CheckWidth = 40;
        static object mShtrih;
        static string ProgID = "AddIn.FPrnM45";


        static internal Type _ShtrihType;
        static internal Type ShtrihType
        {
            get
            {
                return _ShtrihType;
            }
        }
        static internal bool CreateAtollInteruptDrviver()
        {

            if (_ShtrihType != null)
            {
                try
                {
                    Disconnect();

                }
                catch
                { }
            }
            _ShtrihType = Type.GetTypeFromProgID(ProgID);
            mShtrih = Activator.CreateInstance(_ShtrihType);
            if (_ShtrihType == null)
            {
                Utils.ToLog("[Error] не смог создать драйвер Атолл", 6);
                return false;
            }
            return true;
        }


        //private static int LUNumber = 2;
       

       


        




        static internal int CurrentDeviceIndex
        {
            get
            {
                return (int)ShtrihType.InvokeMember("CurrentDeviceIndex", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("CurrentDeviceIndex", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }


        static internal int CurrentDeviceNumber
        {
            get
            {
                return (int)ShtrihType.InvokeMember("CurrentDeviceNumber", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("CurrentDeviceNumber", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }


        static internal bool DeviceEnabled
        {
            get
            {
                return (bool)ShtrihType.InvokeMember("DeviceEnabled", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("DeviceEnabled", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }

        static internal bool AdvancedRegistration
        {
            get
            {
                return (bool)ShtrihType.InvokeMember("AdvancedRegistration", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("AdvancedRegistration", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal bool EnableCheckSumm
        {
            get
            {
                return (bool)ShtrihType.InvokeMember("EnableCheckSumm", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("EnableCheckSumm", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal int Timeout
        {
            get
            {
                return (int)ShtrihType.InvokeMember("Timeout", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("Timeout", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal int ResultCode
        {
            get
            {
                return (int)ShtrihType.InvokeMember("ResultCode", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("ResultCode", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal int BaudRate
        {
            get
            {
                return (int)ShtrihType.InvokeMember("BaudRate", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("BaudRate", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal int PortNumber
        {
            get
            {
                return (int)ShtrihType.InvokeMember("PortNumber", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("PortNumber", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal int Password
        {
            get
            {
                return (int)ShtrihType.InvokeMember("Password", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                //AddProp("Password", value);
                ShtrihType.InvokeMember("Password", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }

        static internal int Department
        {
            get
            {
                return (int)ShtrihType.InvokeMember("Department", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {

                ShtrihType.InvokeMember("Department", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal string ResultDescription
        {
            get
            {
                return (string)ShtrihType.InvokeMember("ResultDescription", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("ResultDescription", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }


        static internal string Caption
        {
            get
            {
                return (string)ShtrihType.InvokeMember("Caption", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                ShtrihType.InvokeMember("Caption", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }

        static internal int Mode
        {
            get
            {
                return (int)ShtrihType.InvokeMember("Mode", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                //AddProp("Password", value);
                ShtrihType.InvokeMember("Mode", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal int ReportType
        {
            get
            {
                return (int)ShtrihType.InvokeMember("ReportType", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                //AddProp("Password", value);
                ShtrihType.InvokeMember("ReportType", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }

        static internal int CheckType
        {
            get
            {
                return (int)ShtrihType.InvokeMember("CheckType", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                //AddProp("Password", value);
                ShtrihType.InvokeMember("CheckType", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal  void SetMode()
        {
            ShtrihType.InvokeMember("SetMode", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
       
        static internal  void Connect()
        {
            ShtrihType.InvokeMember("Connect", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal void Report()
        {
            ShtrihType.InvokeMember("Report", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal  void Disconnect()
        {
            ShtrihType.InvokeMember("Disconnect", BindingFlags.InvokeMethod, null, mShtrih, null);
        }

        static internal  void NewDocument()
        {
            ShtrihType.InvokeMember("NewDocument", BindingFlags.InvokeMethod, null, mShtrih, null);
        }

        static internal  void PrintString()
        {
            if (Caption.Length > CheckWidth)
            {
                Caption = Caption.Substring(0,CheckWidth);
            }
            ShtrihType.InvokeMember("PrintString", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal  void OpenCheck()
        {
            ShtrihType.InvokeMember("OpenCheck", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal  void Registration()
        {
            ShtrihType.InvokeMember("Registration", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal  void Annulate()
        {
            ShtrihType.InvokeMember("Annulate", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal  void Return()
        {
            ShtrihType.InvokeMember("Return", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal  void Storno()
        {
            ShtrihType.InvokeMember("Storno", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal  void PercentsCharge()
        {
            ShtrihType.InvokeMember("PercentsCharge", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal  void PercentsDiscount()
        {
            ShtrihType.InvokeMember("PercentsDiscount", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal  void SummCharge()
        {
            ShtrihType.InvokeMember("SummCharge", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal  void SummDiscount()
        {
            ShtrihType.InvokeMember("SummDiscount", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal  void Payment()
        {
            ShtrihType.InvokeMember("Payment", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal  void CancelCheck()
        {
            ShtrihType.InvokeMember("CancelCheck", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal  void CloseCheck()
        {
            ShtrihType.InvokeMember("CloseCheck", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal void Delivery()
        {
            ShtrihType.InvokeMember("Delivery", BindingFlags.InvokeMethod, null, mShtrih, null);
        }
        static internal double Percents
        {
            get
            {
                return (double)ShtrihType.InvokeMember("Percents", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                //AddProp("Password", value);
                ShtrihType.InvokeMember("Percents", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal double Summ
        {
            get
            {
                return (double)ShtrihType.InvokeMember("Summ", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                //AddProp("Password", value);
                ShtrihType.InvokeMember("Summ", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal double Quantity
        {
            get
            {
                return (double)ShtrihType.InvokeMember("Quantity", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                //AddProp("Password", value);
                ShtrihType.InvokeMember("Quantity", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal double Price
        {
            get
            {
                return (double)ShtrihType.InvokeMember("Price", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                //AddProp("Password", value);
                ShtrihType.InvokeMember("Price", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }



        static internal int Destination
        {
            get
            {
                return (int)ShtrihType.InvokeMember("Destination", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                //AddProp("Password", value);
                ShtrihType.InvokeMember("Destination", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal int TypeClose
        {
            get
            {
                return (int)ShtrihType.InvokeMember("TypeClose", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                //AddProp("Password", value);
                ShtrihType.InvokeMember("TypeClose", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal bool TestMode
        {
            get
            {
                return (bool)ShtrihType.InvokeMember("TestMode", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                //AddProp("Password", value);
                ShtrihType.InvokeMember("TestMode", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
        static internal string Name
        {
            get
            {
                return (string)ShtrihType.InvokeMember("Name", BindingFlags.GetProperty, null, mShtrih, null);
            }
            set
            {
                //AddProp("Password", value);
                ShtrihType.InvokeMember("Name", BindingFlags.SetProperty, null, mShtrih, new object[] { value });
            }
        }
    }

}
