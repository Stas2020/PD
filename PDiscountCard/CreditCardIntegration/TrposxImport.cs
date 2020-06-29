using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime;
using System.Runtime.InteropServices;

namespace PDiscountCard
{
    [ComVisible(true)]
    static public class TrposxImport
    {
        const string trposxPath = @"C:\aloha\check\trposx\trposx.dll";
        const string trposxPath2 = @"C:\aloha\check\trposx\TrPosXTest.dll";

        [DllImport("user32.dll")]
        static public extern bool OemToCharA(char[] lpszSrc, [Out] StringBuilder lpszDst);

        [DllImport("user32.dll")]
        static public extern bool OemToChar(IntPtr lpszSrc, [Out] StringBuilder lpszDst);

        public delegate void mCbDelegate(IntPtr i);
        public delegate void mCbDelegate2(int i);
        public delegate int TRGUI_ScreenShowDelegate(IntPtr pScreenParams);


        public delegate void TRGUI_ScreenCloseDelegate();

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class ScreenParams
        {

            public Int32 len; // [in]
            public Int32 screenID; // [in]
            public Int32 maxInp; // [in]
            public Int32 minInp; // [in]
            public UInt32 format; // [in]
            public IntPtr pTitle; // [in]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public IntPtr[] pStr; // [in]
            public IntPtr pInitStr; // [in]
            public IntPtr pButton0; // [in]
            public IntPtr pButton1; // [in]
            public CurrencyParams CurParam; // [in]
            public Int32 eventKey; // [out]
            public IntPtr pBuf; // [in]
        };
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct CurrencyParams
        {
            public IntPtr CurAlpha; // [in] символьный код валюты.
            public byte nDecPoint; // [in] позиция десятичной точки при отображении валюты.
        };

        /*
        [DllImport(trposxPath, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int TRPOSX_Init(string path_to_cfg,
           [In, Out]  TRGUI_ScreenShowDelegate ScreenShow,
           [In, Out]  TRGUI_ScreenCloseDelegate ScreenClose);

        [DllImport(trposxPath, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int TRPOSX_GetRsp(
            [In, Out]  IntPtr out_params,
            [In, Out]  IntPtr receipt);

          [DllImport(trposxPath, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int TRPOSX_Proc(string in_params, IntPtr len_out_params, IntPtr len_receipt);
        */

        [DllImport(trposxPath2, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        static public extern int CreateTrposxDriver(string Path, mCbDelegate mCb, mCbDelegate2 mCb2);
        [DllImport(trposxPath2, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        static public extern int SetEventKey(int EventKey);

        [DllImport(trposxPath2, CallingConvention = CallingConvention.Cdecl, SetLastError = false, CharSet = CharSet.None)]
        static public extern int TrposxDriverRunOper(string InParams, [Out] StringBuilder OutParams, [Out] StringBuilder reciept, [In, Out] IntPtr icodeGetResp, [In] int OperType, [In] int SlipNum);

        /*
        static public int TrposxDriverRunOper(string InParams, [Out] StringBuilder OutParams, [Out] StringBuilder reciept, [In, Out] IntPtr icodeGetResp, [In] int OperType)
        {
            return TrposxDriverRunOper(InParams, OutParams, reciept, icodeGetResp, OperType, 0);
        }
         * */





        
    }
}
