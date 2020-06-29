using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PDiscountCard
{
   static  class mTRPOSXLib
    {
       static object mTRPOSX;
        static string DllName = "TRPOSX.TRPOSX";

        static Type mTRPOSXType;
        static internal bool CreatemTRPOSX()
        {
            mTRPOSXType = Type.GetTypeFromProgID(DllName);
            mTRPOSX = Activator.CreateInstance(mTRPOSXType);
            if (mTRPOSXType == null)
            {
                return false;
            }
            return true;

        }

        static internal int Init(string Path)
        {
            object[] Params = new object[1];
            Params[0] = Path;
            return Convert.ToInt32 (mTRPOSXType.InvokeMember("Init", BindingFlags.InvokeMethod, null, mTRPOSX, Params));
        }
        static internal void Close()
        {
            mTRPOSXType.InvokeMember("Close", BindingFlags.InvokeMethod, null, mTRPOSX, null );
        }
        static internal int Process(string inStr, out int outLen, out int rcpLen)
        {
            outLen = 0;
            rcpLen = 0;
            object[] Params = new object[3];
            Params[0] = inStr;
            Params[1] = outLen;
            Params[2] = rcpLen;


            ParameterModifier[] pm = new ParameterModifier[1];
            // построить описатель передаваемых параметров 
            // в вызываемый метод: 3 параметра
            pm[0] = new ParameterModifier(3);
            pm[0][0] = false; // by val : ConnectionString
            pm[0][1] = true;  // by ref : RecordsAffected 
            pm[0][2] = true ; 
          //  ct.InvokeMember("Execute", BindingFlags.InvokeMethod, null, cn, pars, pm, null, null);
//            recs = Convert.ToInt32(pars[1]);




          int ret = Convert.ToInt32   (mTRPOSXType.InvokeMember("Process", BindingFlags.InvokeMethod, null, mTRPOSX, Params,pm ,null ,null ));

            outLen = Convert.ToInt32(Params[1]);
            rcpLen = Convert.ToInt32(Params[2]);
            return ret;
        }
        static internal string  GetResponse(int Offset , int Length)
        {
            
            object[] Params = new object[2];
            Params[0] = Offset ;
            Params[1] = Length ;
            object ret = mTRPOSXType.InvokeMember("GetResponse", BindingFlags.InvokeMethod, null, mTRPOSX, Params);
            if (ret != null)
            {
                return ret.ToString();
            }
            else
            {
                return null;
            }
        }

        static internal string GetReceipt(int Offset, int Length)
        {

            object[] Params = new object[2];
            Params[0] = Offset;
            Params[1] = Length;
            object ret = mTRPOSXType.InvokeMember("GetReceipt", BindingFlags.InvokeMethod, null, mTRPOSX, Params);
            if (ret != null)
            {
                return ret.ToString ();
            }
            else
            {
                return null;
            }

        }

       
       

    }
}
