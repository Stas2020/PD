using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime;
using System.Threading;
using System.Runtime.InteropServices;
namespace PDiscountCard
{
    static  class TrPosXClass
    {

        public delegate int TRGUI_ScreenShowDelegate(IntPtr pScreenParams);

        static private ScreenParamsOut ScreenParamsConvert(TrposxImport.ScreenParams tr)
        {

            ScreenParamsOut mScreenParamsOut = new ScreenParamsOut();

            mScreenParamsOut.format = (int)tr.format;
            mScreenParamsOut.pButton0 = GetStrFromPtr(tr.pButton0);
            mScreenParamsOut.pButton1 = GetStrFromPtr(tr.pButton1);
            mScreenParamsOut.pInitStr = GetStrFromPtr(tr.pInitStr);
            mScreenParamsOut.pStr = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                string nn = GetStrFromPtr(tr.pStr[i]);
                mScreenParamsOut.pStr.Add(nn);
                mScreenParamsOut.AllStrs += nn + Environment.NewLine;
            }
            
            
            mScreenParamsOut.pTitle = GetStrFromPtr(tr.pTitle);
            mScreenParamsOut.pInitStr = GetStrFromPtr(tr.pInitStr);
            mScreenParamsOut.screenID = tr.screenID;

            return mScreenParamsOut;

        }


        static private String GetStrFromPtr(IntPtr Ptr)
        {
            if (Ptr != IntPtr.Zero)
            {
                StringBuilder strBuilder = new StringBuilder();
                TrposxImport.OemToChar(Ptr, strBuilder);
                return strBuilder.ToString();
            }
            return "";
        }

        static public void  ScreenShow(IntPtr ScPar)
        {
            int l2 = Marshal.SizeOf(ScPar);
            object kk = Marshal.PtrToStructure(ScPar, typeof(TrposxImport.ScreenParams));
            TrposxImport.ScreenParams tr = (TrposxImport.ScreenParams)kk;
            Marshal.StructureToPtr(tr, ScPar, true);
            ScreenParamsOut ScOut = ScreenParamsConvert(tr);
            Utils.ToCardLog("ScreenShow IDЖ " + ScOut.screenID + " AllStrs " + ScOut.AllStrs);
            if (tr.screenID == 3)
            {
                QFromTrposxForm TrPosXMessageForm = new QFromTrposxForm(ScOut.pTitle, ScOut.AllStrs);
                TrPosXMessageForm.button1.Text = ScOut.pButton0;
                TrPosXMessageForm.button2.Text = ScOut.pButton1;
                TrPosXMessageForm.button1.Visible = true;
                TrPosXMessageForm.button2.Visible = true;
                TrPosXMessageForm.button3.Visible = false;
                //TrPosXMessageForm.Init("Операция на терминале пластиковых карт.", "");
                //TrPosXMessageForm.button1.Click += new EventHandler(button1_Click);
                //TrPosXMessageForm.button2.Click += new EventHandler(button2_Click);

                //TrPosXClass.RunOperationAsinc(inStr);
                TrPosXMessageForm.TopMost = true;
                TrPosXMessageForm.ShowDialog();
                if (TrPosXMessageForm.Answ)
                {
                    //Marshal.WriteInt32( tr.eventKey, 0x30);
                    TrposxImport.SetEventKey(0x30);
                }
                else
                {
                    TrposxImport.SetEventKey(0x31);
                    //Marshal.WriteInt32(tr.eventKey, 0x31);
                }
                TrPosXMessageForm.Dispose();
            }
            else
            {
                ShowScreenVoid(ScOut, tr.maxInp == 0);
            }

            if (tr.screenID == 5)
            {
                Thread.Sleep(2000);
            }

            //Thread.CurrentThread.Join();
            //return 0;
        }


        public static void ScreenClose(int k)
        {
           // string k = "";
        }
        public static TrposxImport.mCbDelegate ScShow = new TrposxImport.mCbDelegate(ScreenShow);
        public static TrposxImport.mCbDelegate2 ScClose = new TrposxImport.mCbDelegate2(ScreenClose);


        public delegate void ShowScreenDelegate(object sender, ScreenParamsOut Params, bool NeedAnswer);

        public static event ShowScreenDelegate ShowScreenEvent;

        static private void ShowScreenVoid(ScreenParamsOut Params, bool NeedAnswer)
        {
            TrPosXAlohaIntegrator.TrPosXClass_ShowScreenEvent(null, Params, NeedAnswer); 
            /*
            if (ShowScreenEvent != null)
            {
                ShowScreenEvent(null, Params, NeedAnswer);
            }
             * */
        }

        public struct ScreenParamsOut
        {
            public int screenID; // [in]
            public int format; // [in]
            public string pTitle; // [in]
            public List<string> pStr; // [in]
            public string pInitStr; // [in]
            public string pButton0; // [in]
            public string pButton1; // [in]
            //public CurrencyParams CurParam; // [in]
            //public Int32 eventKey; // [out]
            public string pBuf; // [in]
            public string AllStrs;
        };

        static private  string GetResStr(int Res)
        {
            string strResp = "";
            switch (Res)
            {
                case 0:
                    strResp = "Операция завершена";
                    break;
                case 1:
                    strResp = "Невозможно открыть конфигурационный файл";
                    break;
                case 2:
                    strResp = "Ошибка обработки входных параметров (файла)";
                    break;
                case 3:
                    strResp = "Ошибка создания выходных параметров (файла)";
                    break;
                case 4:
                    strResp = "Ошибка создания образа карт-чека";
                    break;
                case 5:
                    strResp = "Невозможно открыть порт к POS-терминалу";
                    break;
                case 6:
                    strResp = "Ошибка в параметрах вызова";
                    break;
                case 7:
                    strResp = "Ошибка во входных данных";
                    break;
                case 8:
                    strResp = "Системная ошибка";
                    break;

                default:
                    strResp = "Сработал таймаут. Биты 0-6 содержат код диагностики. " + Res;

                    break;
                    
            }
            return  strResp ;
        }

        static public int TrPosXInit(string Path, out string strResp)
        {
            strResp = "";
            //int i = TrposxImport.TRPOSX_Init(Path, ScShow, ScClose);
            int i = TrposxImport.CreateTrposxDriver(Path, ScShow, ScClose);
            strResp = GetResStr(i);
        
            return i;
        }

        public delegate void RunOperationAsincDelegate(int res, string resStr, string Response, string Receipt);

        //public static event RunOperationAsincDelegate RunOperationAsincComplited;

        static private void RunOperationAsincComplitedVoid(int res, string resStr, string Response, string Receipt)
        {
            TrPosXAlohaIntegrator.TrPosXClass_RunOperationAsincComplited(res, resStr, Response, Receipt);
            /*
            if (RunOperationAsincComplited != null)
            {
                RunOperationAsincComplited(res, resStr, Response, Receipt);
            }
             * */
        }

        private static String OperString = "";
        private static int SlipNum = 0;
        public static bool OperInProcess = false;
        public static void RunOperationAsinc(string mOperString)
        {
            OperString = mOperString;
            if (!OperInProcess)
            {
                OperInProcess = true;
                Thread TrPosXThread = new Thread(mRunOperation);
                TrPosXThread.Name = "Поток для mRunOperation";
                TrPosXThread.Start();
            }
        }
        public static void RunFoolReportAsinc()
        {
            OperString = "MessageID=SVR" + Environment.NewLine +
   "ECRReceiptNumber=0000000100" + Environment.NewLine +
    "ECRNumber=01";
            if (!OperInProcess)
            {
                OperInProcess = true;
                Thread TrPosXThread = new Thread(mRunFoolReport);
                TrPosXThread.Name = "Поток для mRunFoolReport";
                TrPosXThread.Start();
            }
        }
        public static void RunXReportAsinc()
        {
            if (!OperInProcess)
            {
                OperInProcess = true;
                Thread TrPosXThread = new Thread(mRunXReport);
                TrPosXThread.Name = "Поток для mRunFoolReport";
                TrPosXThread.Start();
            }
        }
        public static void RunCopySlipAsinc(int mSlipNum)
        {
            if (!OperInProcess)
            {
                SlipNum = mSlipNum;
                OperInProcess = true;
                Thread TrPosXThread = new Thread(mRunCopySlipReport);
                TrPosXThread.Name = "Поток для mRunCopySlipReport";
                TrPosXThread.Start();
            }
        }


        public static void RunOperation(string mOperString, out int ress, out string ResStr, out string Resp, out string Receipt)
        {
            OperString = mOperString;
            ress = 0;
            Resp = "";
            Receipt ="";
            
            if (!OperInProcess)
            {
                OperInProcess = true;
                IntPtr _pInt_buffer1 = Marshal.AllocHGlobal(4);
                IntPtr _pInt_buffer2 = Marshal.AllocHGlobal(4);
                StringBuilder bufferForResp = new StringBuilder(100000);
                StringBuilder bufferForReceipt = new StringBuilder(100000);
                IntPtr bufferForICode2 = Marshal.AllocHGlobal(4);
                 ress = TrposxImport.TrposxDriverRunOper(OperString, bufferForResp, bufferForReceipt, bufferForICode2, 0,0);
                Resp = bufferForResp.ToString();
                Receipt = bufferForReceipt.ToString();
                
                OperInProcess = false;
                //RunOperationAsincComplitedVoid(res, GetResStr(res), Resp, Receipt);

            }
            ResStr = GetResStr(ress);
        }

        private static void mRunFoolReport()
        {
            
            StringBuilder bufferForResp = new StringBuilder(100000);
            StringBuilder bufferForReceipt = new StringBuilder(100000);
            
            IntPtr bufferForICode2 = Marshal.AllocHGlobal(4);
            Utils.ToCardLog("TrposxImport.TrposxDriverRunOper OperString: " + OperString + " ");
            int res = TrposxImport.TrposxDriverRunOper(OperString, bufferForResp, bufferForReceipt, bufferForICode2, 1,0);
            
            
            string Resp = bufferForResp.ToString();
            string Receipt = bufferForReceipt.ToString();
            OperInProcess = false;

            RunOperationAsincComplitedVoid(res, GetResStr(res), Resp, Receipt);

        }

        private static void mRunXReport()
        {

            StringBuilder bufferForResp = new StringBuilder(100000);
            StringBuilder bufferForReceipt = new StringBuilder(100000);

            IntPtr bufferForICode2 = Marshal.AllocHGlobal(4);
            Utils.ToCardLog("TrposxImport.Xreport OperString: " );
            int res = TrposxImport.TrposxDriverRunOper(OperString, bufferForResp, bufferForReceipt, bufferForICode2, 2, 0);


            string Resp = bufferForResp.ToString();
            string Receipt = bufferForReceipt.ToString();
            OperInProcess = false;

            RunOperationAsincComplitedVoid(res, GetResStr(res), Resp, Receipt);

        }
        private  static void mRunCopySlipReport()
        {

            StringBuilder bufferForResp = new StringBuilder(100000);
            StringBuilder bufferForReceipt = new StringBuilder(100000);

            IntPtr bufferForICode2 = Marshal.AllocHGlobal(4);
            int res = TrposxImport.TrposxDriverRunOper("", bufferForResp, bufferForReceipt, bufferForICode2, 1,SlipNum );


            string Resp = bufferForResp.ToString();
            string Receipt = bufferForReceipt.ToString();
            OperInProcess = false;

            RunOperationAsincComplitedVoid(res, GetResStr(res), Resp, Receipt);

        }


        private static void mRunOperation()
        {
            IntPtr _pInt_buffer1 = Marshal.AllocHGlobal(4);
            IntPtr _pInt_buffer2 = Marshal.AllocHGlobal(4);
            StringBuilder bufferForResp = new StringBuilder(100000);
            StringBuilder bufferForReceipt = new StringBuilder(100000);
            IntPtr bufferForICode2 = Marshal.AllocHGlobal(4);
            int res= TrposxImport.TrposxDriverRunOper(OperString, bufferForResp, bufferForReceipt, bufferForICode2, 0,0);
            string Resp = bufferForResp.ToString ();
            string Receipt = bufferForReceipt.ToString ();
            OperInProcess = false;
            RunOperationAsincComplitedVoid(res, GetResStr(res), Resp, Receipt);

        }
    }
    

}
