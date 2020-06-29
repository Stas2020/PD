using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;

namespace PDiscountCard
{
    static  class TrPosXAlohaIntegrator
    {
        private static string SetupPath = @"C:\Aloha\check\trposx\";
        internal static int ECRReceiptNumber = 1;

        internal static bool Init(out string Ex)
        {
            Ex = "";
            try
            {
                string OutStr = "";
                int res = TrPosXClass.TrPosXInit(SetupPath + "setup.txt", out OutStr);
                if ( res== 0)
                {
                    /*
                    TrPosXClass.ShowScreenEvent += new TrPosXClass.ShowScreenDelegate(TrPosXClass_ShowScreenEvent);
                    TrPosXClass.RunOperationAsincComplited += new TrPosXClass.RunOperationAsincDelegate(TrPosXClass_RunOperationAsincComplited);
                     * */
                    return true;
                }
                else
                {
                    Ex = OutStr; 
                    return false;
                }
            }
            catch(Exception e)
            {
                Ex = e.Message;
                return false;
            }
        }

       static  int _PaymentId = 0;
       static  int KKmNum;
       static bool OperInProcess = false;
        internal static bool RunOper(Check Ch, int LastTr, bool AllChecks, FiskInfo mfi, int PaymentId)
        {
            if (TrPosXClass.OperInProcess)
            {
                return false;
            }
            
            _PaymentId = PaymentId;
            KKmNum = MainClass.IsWiFi;
            //ToShtrih.Disconnect();
            string inStr = "";
            decimal Sum = 0;
            if (AllChecks)
            {
                foreach (Check Ch2 in Ch.ChecksOnTable)
                {
                    if (Ch2.AlohaCheckNum != Ch.AlohaCheckNum)
                    {
                        AlohaTSClass.ApplyCardPayment(Ch2.AlohaCheckNum, Ch2.Summ);
                        AlohaTSClass.SetPaymentAttr(Ch2.AlohaCheckNum, LastTr);
                    }
                    Sum += Ch2.Summ;
                }
            }
            else
            {
                Sum = Ch.Summ;
            }


            if (!Ch.Vozvr)
            {
                string TrNumStr = LastTr.ToString().PadLeft(10, "0"[0]);
                inStr = "MessageID=PUR" + Environment.NewLine +
                    //   inStr = "MessageID=SRV" + Environment.NewLine +
                        "ECRReceiptNumber=" + TrNumStr + Environment.NewLine +
                        "ECRNumber=" + KKmNum + Environment.NewLine +
                        "TransactionAmount=" + (Sum * 100).ToString("0") + Environment.NewLine;
            }
            else
            {

                 VF = new VoidFrm(Sum);
                VF.ShowDialog();
                if (VF.Cancel)
                {
                    return false ;
                }
                string chNum = VF.ChNum.ToString().PadLeft(10, "0"[0]);
            
              inStr = "MessageID=VOI" + Environment.NewLine +
                             "ECRReceiptNumber=" + chNum + Environment.NewLine +
                        "ECRNumber=" + KKmNum + Environment.NewLine;
            }

            

            Utils.ToCardLog("Транзакция старт " + inStr);

            
            
           // TrPosXClass.RunOperationAsincComplited += new TrPosXClass.RunOperationAsincDelegate(TrPosXClass_RunOperationAsincComplited);
            if (TrPosXMessageForm != null)
            {
                TrPosXMessageForm.Close();
            }
            TrPosXMessageForm = new FTrposxRunComplited();
            Button1State = 0;
            TrPosXMessageForm.button2.Text = "Свернуть";
            TrPosXMessageForm.button1.Visible = false;
            TrPosXMessageForm.button2.Visible = true ;
            TrPosXMessageForm.button3.Visible = false; 
            TrPosXMessageForm.Init("Операция на терминале пластиковых карт.","");
            TrPosXMessageForm.button1.Click += new EventHandler(button1_Click);
            TrPosXMessageForm.button2.Click += new EventHandler(button2_Click);
            
            TrPosXClass.RunOperationAsinc(inStr);
            TrPosXMessageForm.TopMost = true;
            TrPosXMessageForm.Show();
            
            return true;
        }

        static  VoidFrm VF;

        static void StartVOI(int chNum)
        {
         string  inStr = "MessageID=VOI" + Environment.NewLine +
                                 "ECRReceiptNumber=" + chNum + Environment.NewLine +
                            "ECRNumber=" + KKmNum + Environment.NewLine;
         TrPosXClass.RunOperationAsinc(inStr);

        }

        static void button2_Click(object sender, EventArgs e)
        {
            //TrPosXMessageForm.ToFone();
            ((FTrposxRunComplited)((System.Windows.Forms.Button)sender).Tag ).ToFone();
        }
        static  int Button1State = 0;

        static void button1_Click(object sender, EventArgs e)
        {
             if (Button1State == 0)
            {
                //TrPosXMessageForm.Close();
            }
            else if (Button1State == 1)
            {
                PrintSlip(Arp.Receipt);
                //TrPosXMessageForm.Close();
            }
            else if (Button1State == 2)
            {
                //TrPosXMessageForm.Close();
             
            }
            try
            {
                ((FTrposxRunComplited)((System.Windows.Forms.Button)sender).Tag).Close();
                MainClass.PlasticTransactionInProcess = false;
            }
            catch
            {}
        }

        static private  void PrintSlip(string Slip)
        {
            if (iniFile.CreditCardSlipPrintPreCheck)
            {
                try
                {
                    // string[] stringSeparators = new string[] { "\n\r", "\n\n", Environment.NewLine};

                    string[] stringSeparators = new string[] { "\n" };

                    string sres = Slip.Replace("\r", "");

                    AlohaTSClass.PrintCardSlip(sres.Split(stringSeparators, StringSplitOptions.None).ToList());
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("Ошибка печати слипа " + e.Message);
                }
            }
            else
            {
                Slip += Convert.ToChar(31);
                ToShtrih.PrintCardCheck(Slip);
            }
            //ToShtrih.Conn();
          //  ToShtrih.PrintCardCheck(Slip); 
        }

        static  FTrposxRunComplited TrPosXMessageForm;


        delegate void ShowWindowDelegate(TrPosXClass.ScreenParamsOut Params);

        static internal  void TrPosXClass_RunOperationAsincComplited(int res, string resStr, string Response, string Receipt)
        {
         
            if (TrPosXMessageForm != null)
            {
                    //AddResponseParams Arp = new AddResponseParams();
                    Arp.Receipt = Receipt;
                    Arp.res = res;
                    Arp.Response = Response;
                    Arp.resStr = resStr;

                    object[] Params = new object[1];
                    Params[0] = Arp;

                    if (!IsGetJRNCheck)
                    {
                        TrPosXMessageForm.BeginInvoke(new AddResponseToTextBoxDelegate(AddResponseToTextBox), Params);                    
                        if (GetParam("MessageID", Arp.Response) == "PUR")
                        {
                            if (GetParam("Approve", Arp.Response) == "Y")
                            {
                                CloseCheck.InkreasePlastNum();
                            }
                        }
                    }
                    else
                    {
                        if (GetParam("Approve", Response) == "Y")
                        {


                            if (IsGetJRNCheckToVoid)
                            {
                                TrPosXMessageForm.BeginInvoke(new CloseFormDelegate(CloseForm));
                                decimal Sum = Convert.ToDecimal(GetParam("TransactionAmount", Response)) / 100;
                                if (VF != null)
                                {
                                    object[] P1 = new object[2];

                                    int num = Convert.ToInt32(GetParam("ECRReceiptNumber", Arp.Response));
                                    P1[0] = num;
                                    P1[1] = Sum;

                                    VF.BeginInvoke(new EnterrNumberDelegate(VF.EnterrNumber), P1);
                                }

                            }
                            else
                            {
                                TrPosXMessageForm.BeginInvoke(new AddResponseToTextBoxDelegate(AddResponseToTextBox), Params);                                                
                            }

                        }
                        else
                        {
                            TrPosXMessageForm.BeginInvoke(new AddResponseToTextBoxDelegate(AddResponseToTextBox), Params);                    
                        }
                        IsGetJRNCheck = false;
                    }
                    
                    
                    
                    
            }
        }

        delegate void EnterrNumberDelegate(int num , decimal sum);
        static private void EnterrNumberVoid(int num, decimal sum)
        { 
        }
        
        static internal void TrPosXClass_ShowScreenEvent(object sender, TrPosXClass.ScreenParamsOut Params, bool NeedAnswer)
        {
            //TrPosXClass.ShowScreenEvent -= new TrPosXClass.ShowScreenDelegate(TrPosXClass_ShowScreenEvent);
            if (TrPosXMessageForm != null)
            {
                if (TrPosXMessageForm.Visible)
                {
                    object[] _Params = new object[1];
                    _Params[0] = Params;
                    TrPosXMessageForm.BeginInvoke(new ShowWindowDelegate(ShowWindow), _Params);
                }
            }
        }
      static   private void ShowWindow(TrPosXClass.ScreenParamsOut Params)
        {
            /*
            textBox1.Text += "Main_ShowScreenEvent " + Environment.NewLine;
            textBox1.Text += Params.pTitle + Environment.NewLine;
             * */
          
            TrPosXMessageForm.AddMess(Params.pTitle + Environment.NewLine);
            TrPosXMessageForm.UpdateMess("");
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    
                    if (Params.pStr[i] != "")
                    {
                        TrPosXMessageForm.AddMess(Params.pStr[i] + Environment.NewLine);
                    }
                    
                }
                catch
                { }
            }
            
        }

      private struct AddResponseParams
      {
          public int res;
          public string resStr;
          public string Response;
          public string Receipt;
      }


      static AddResponseParams Arp;

              delegate void CloseFormDelegate();
              static private void CloseForm()
              {
                  TrPosXMessageForm.Close();      
              }

      delegate void AddResponseToTextBoxDelegate(AddResponseParams Params);
        static private void AddResponseToTextBox(AddResponseParams Params)
      {
         // TrPosXMessageForm.ShowDialog();
          AddResponseParams Arp = (AddResponseParams)Params;

          TrPosXMessageForm.UpdateMess(Arp.resStr + Environment.NewLine);
          if (GetParam("Approve", Arp.Response) == "Y")
          {
              TrPosXMessageForm.AddMess("Транзакция одобрена" + Environment.NewLine);
          }
          else
          {
              TrPosXMessageForm.AddMess("Транзакция ОТКЛОНЕНА" + Environment.NewLine);
            
              if (Arp.Response.Length > 2)
              {
                  /*
                  TrPosXMessageForm.AddMess(
                      Encoding.GetEncoding(1251).GetString(
                            Encoding.GetEncoding(866).GetBytes(GetParam("VisualHostResponse", Arp.Response))));
                   * */
                  TrPosXMessageForm.AddMess("Причина: "+
                      GetParam("VisualHostResponse", Arp.Response) + Environment.NewLine);

              }
          }

          if (Arp.Receipt != "")
          {
              Button1State = 1;

              TrPosXMessageForm.button1.Text = "Распечатать слип";
              TrPosXMessageForm.button1.Visible = true;
          }
          else
          {
              Button1State = 2;
              TrPosXMessageForm.AddMess("Нет данных для печати" + Environment.NewLine);
              TrPosXMessageForm.button1.Text = "Закрыть";
              TrPosXMessageForm.button1.Visible = true;
          }
 

            /*
          textBox1.Text += "Main_RunOperationAsincComplited " + Environment.NewLine;
          textBox1.Text += "res: " + Arp.res + Environment.NewLine;
          textBox1.Text += "Response: " + Arp.Response + Environment.NewLine;
          textBox1.Text += "Receipt: " + Arp.Receipt + Environment.NewLine;
             * */
      }

        static public void SverkaWithQ()
        {
            QSverkaForm F1 = new QSverkaForm();
            F1.ShowDialog();
            if (!F1.IsGood)
            {
                return;
            }
            else
            {
                if (TrPosXAlohaIntegrator.Sverka())
                {
                    PDiscountCard.CloseCheck.ZeroPlastNum();
                    FiskInfo fi = PDiscountCard.CloseCheck.ReadFiskInfo();
                    fi.NeedSverka = false;
                    PDiscountCard.CloseCheck.WriteFiskInfo(fi);
                }

            }

        }

        static public bool  Sverka()
        {
            string inStr = "MessageID=STL" + Environment.NewLine +
            "ECRNumber=1" + Environment.NewLine;
            //TrPosXClass.ShowScreenEvent += new TrPosXClass.ShowScreenDelegate(TrPosXClass_ShowScreenEvent);
            
            
            int ress;
            string ResStr;
            string Resp;
            string Receipt;
            TrPosXClass.RunOperation(inStr, out ress, out  ResStr, out  Resp, out  Receipt);
            /*
            TrPosXMessageForm = new FTrposxRunComplited();
            TrPosXMessageForm.button1.Visible = false;
            TrPosXMessageForm.button2.Visible = false;
            TrPosXMessageForm.button3.Visible = false;
            TrPosXMessageForm.Init("Сверка на терминале пластиковых карт.", "");
            TrPosXMessageForm.button1.Click += new EventHandler(button1_Click);
            TrPosXMessageForm.button2.Click += new EventHandler(button2_Click);
            TrPosXMessageForm.TopMost = true;
            AddResponseToTextBox(new AddResponseParams {
                Receipt = Receipt,
                res =ress,
                Response = Resp,
                resStr = ResStr
            });
            */
            if (Receipt != "")
            {
                PrintSlip(Receipt);
            }

            return (GetParam("Approve", Resp) == "Y");
        }
        static public bool FoolReport()
        {
            
        //    TrPosXClass.ShowScreenEvent += new TrPosXClass.ShowScreenDelegate(TrPosXClass_ShowScreenEvent);
            TrPosXMessageForm = new FTrposxRunComplited();
            TrPosXMessageForm.button1.Visible = false;
            TrPosXMessageForm.button2.Visible = false;
            TrPosXMessageForm.button3.Visible = false;
            TrPosXMessageForm.Init("Печать полного отчета с терминала пластиковых карт.", "");
            TrPosXMessageForm.button1.Click += new EventHandler(button1_Click);
            TrPosXMessageForm.button2.Click += new EventHandler(button2_Click);
            TrPosXMessageForm.TopMost = true;
            TrPosXMessageForm.Show();
            TrPosXClass.RunFoolReportAsinc ();
            return true;
        }
        static public bool XReport()
        {

            //    TrPosXClass.ShowScreenEvent += new TrPosXClass.ShowScreenDelegate(TrPosXClass_ShowScreenEvent);
            TrPosXMessageForm = new FTrposxRunComplited();
            TrPosXMessageForm.button1.Visible = false;
            TrPosXMessageForm.button2.Visible = false;
            TrPosXMessageForm.button3.Visible = false;
            TrPosXMessageForm.Init("Печать отчета с терминала пластиковых карт.", "");
            TrPosXMessageForm.button1.Click += new EventHandler(button1_Click);
            TrPosXMessageForm.button2.Click += new EventHandler(button2_Click);
            TrPosXMessageForm.TopMost = true;
            TrPosXMessageForm.Show();
            TrPosXClass.RunXReportAsinc();
            return true;
        }

        static private void CreateTrPosXMessageForm()
        {
            TrPosXMessageForm = new FTrposxRunComplited();
            TrPosXMessageForm.button1.Visible = false;
            TrPosXMessageForm.button2.Visible = false;
            TrPosXMessageForm.button3.Visible = false;
            TrPosXMessageForm.Init("Операция на безналичном терминале", "");
            TrPosXMessageForm.button1.Click += new EventHandler(button1_Click);
            TrPosXMessageForm.button2.Click += new EventHandler(button2_Click);
            TrPosXMessageForm.TopMost = true;
            TrPosXMessageForm.Show();
        }

       //static  Thread FormThread;

        static public void GetSlipCopy()
        {
            CopySlipFrm CSF = new CopySlipFrm();
        
            CSF.ShowDialog();
            
        }

       



        private static  bool IsGetJRNCheck = false ;
        private static bool IsGetJRNCheckToVoid= false;
        internal static string GetJRNCheck(int num)
        {
          return  GetJRNCheck(num, true);
        }
        internal static string GetJRNCheck(int num, bool CheckToVoid)
        {

            IsGetJRNCheckToVoid = CheckToVoid;

            string inStr = "MessageID=JRN" + Environment.NewLine +
                            "ECRReceiptNumber=" + num.ToString().PadLeft(10, "0"[0]) + Environment.NewLine +
                          //  "ECRNumber=" + MainClass.IsWiFi + Environment.NewLine +
                            "ECRNumber=1" + Environment.NewLine +
                            "Flags=802020" + Environment.NewLine;
            int outLen = 0;
            int rcpLen = 0;

            try
            {
                /*
                int res = mTRPOSXLib.Process(inStr, out outLen, out rcpLen);
                string Response = mTRPOSXLib.GetResponse(0, outLen);
                string Rcp = mTRPOSXLib.GetReceipt(0, rcpLen);
                */
                TrPosXMessageForm = new FTrposxRunComplited();
                Button1State = 0;
                TrPosXMessageForm.button2.Text = "Свернуть";
                TrPosXMessageForm.button1.Visible = false;
                TrPosXMessageForm.button2.Visible = true;
                TrPosXMessageForm.button3.Visible = false;
                TrPosXMessageForm.Init("Поиск чека № " + num.ToString()+".", "");
                TrPosXMessageForm.button1.Click += new EventHandler(button1_Click);
                TrPosXMessageForm.button2.Click += new EventHandler(button2_Click);

                IsGetJRNCheck = true;

                TrPosXClass.RunOperationAsinc(inStr);
                TrPosXMessageForm.TopMost = true;
                TrPosXMessageForm.Show();
            
            

                


            }
            catch
            {

            }

            return "";

        }
        internal static string GetParam(string ParamName, string OutParam)
        {
            try
            {
                string[] Str = OutParam.Split(char.ConvertFromUtf32(10)[0]);
                foreach (string str in Str)
                {
                    if (str.Substring(0, str.Length - 1).ToLower().Contains(ParamName.ToLower()))
                    {
                        return str.Substring(ParamName.Length + 1, str.Length - ParamName.Length - 2);
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }

        }
    
    }
}
