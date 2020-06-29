    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Runtime.InteropServices;
using System.Reflection;

namespace PDiscountCard.CreditCardIntegration
{
    public class Arcus3Wrapper
    {

        [DllImport("c:\\aloha\\check\\arcus2\\dll\\Arccom.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateITPos();

        [DllImport("c:\\aloha\\check\\arcus2\\dll\\Arccom.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ITPosClear(IntPtr pClassNameObject);

        [DllImport("c:\\aloha\\check\\arcus2\\dll\\Arccom.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr DeleteITPos(IntPtr pClassNameObject);

        [DllImport("c:\\aloha\\check\\arcus2\\dll\\Arccom.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ITPosSet(IntPtr pClassNameObject, byte[] key, byte[] value, int len);

        [DllImport("c:\\aloha\\check\\arcus2\\dll\\Arccom.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int ITPosGet(IntPtr pClassNameObject, byte[] key, byte[] value, int len);

        [DllImport("c:\\aloha\\check\\arcus2\\dll\\Arccom.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ITPosRun(IntPtr pClassNameObject, int cmd);

        [DllImport("c:\\aloha\\check\\arcus2\\dll\\Arccom.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ITPosRunCmd(IntPtr pClassNameObject, byte[] key, byte[] value, int len);

        /*
        private void button10_Click(object sender, EventArgs e)
        {
            
            Dictionary<string, string> SetParams = new Dictionary<string, string> { };
            SetParams.Add("amount", "100");
            SetParams.Add("currency", "643");
            List<string> getValue = new List<string> {
                "auth_code",
                "result_amount",
                "pan",
                "expiry",
                "slip",
                "aid",
                "card_type",
                "application_label",
                "date",
                "time",
                "terminal_id",
                "response_code",
                "rrn",
                "tvr",
                "received_text_message",
                "text_message",
                "cardholder_name",
                "original_date_time",
            };

            int result = Arcus(ArcusOp.Pay, SetParams, getValue, out Dictionary<string,string> GetParams);
            if(result >= 0)
            {
                textBox1.Text +="Код ответа : " + result.ToString().PadLeft(3,'0') + Environment.NewLine;

                if (GetParams != null)
                {
                    foreach (KeyValuePair<string,string> keyvalue in GetParams)
                    {
                        textBox1.Text += keyvalue.Key + " : " + keyvalue.Value.ToString() + Environment.NewLine;
                    }
                }
                             
            }
            else
            {
                if(GetParams != null)
                {
                    // ошибка
                    if (GetParams.Count(f => f.Key == "Error") > 0)
                    {
                        textBox1.Text += "Error : " + GetParams.First(f => f.Key == "Error").Value + Environment.NewLine;                        
                    }                    
                }                
            }
        }
        */


        public  enum ArcusOp
        {
            Pay = 1,
            CancelLast = 2,
            Refund = 3,
            Cancel = 4,
            XReportFull = 7,
            XReportShort = 8,
            ZReport = 10,
            LastSlipCopy = 60,
            SlipCopy = 62
        }



 public int CallArcusOper(ArcusOp Operation, ArcusRequest inParams, ArcusResponse outParams)
        {
            int Result = -1;
            
            if (Operation > 0)
            {
                IntPtr pos_obj = IntPtr.Zero;
                try
                {
                    pos_obj = CreateITPos();

                    if (pos_obj == IntPtr.Zero)
                    {
                       return -1;
                    }

                    if (inParams != null)
                    {
                        foreach (var p in typeof(ArcusRequest).GetProperties())
                        {
                            if (p.GetArcusNonSerialized())
                            {
                                continue;
                            }
                            if(p.GetValue(inParams,null) != null)
                            {
                                string val = p.GetValue(inParams,null).ToString();

                                if (val.Length > 0 && val != "0")
                                {
                                    ITPosSet(pos_obj, p.GetArcusFieldName().GetASCIIBytes(), p.GetValue(inParams,null).GetASCIIBytes(), -1);
                                }                                
                            }                                                   
                            
                        }
                    } 
                    
                    Result = ITPosRun(pos_obj, (int)Operation);

                    foreach (var p in typeof(ArcusResponse).GetProperties())
                    {
                        object resValue;
                        try
                        {
                            int size = ITPosGet(pos_obj, p.GetArcusFieldName().GetASCIIBytes(), null, -1);
                            byte[] value = new byte[size + 1];

                            ITPosGet(pos_obj, p.GetArcusFieldName().GetASCIIBytes(), value, value.Length);

                            //var resValue = Encoding.ASCII.GetString(value).Trim('\0');
                            resValue = Encoding.ASCII.GetString(value);
                            
                        }
                        catch(Exception e)
                        {
                            Utils.ToCardLog("Error CallArcusOper SetValue prop: " +p.Name+" err: " + e.Message);
                            resValue = e.Message;
                        }
                        p.SetValue(outParams, resValue, null);
                    }

                }
                catch (Exception ex)
                {
                  
                }
                finally
                {
                    ITPosClear(pos_obj);  
                }
                if (pos_obj != IntPtr.Zero)
                {
                    DeleteITPos(pos_obj);
                }
            }

            return Result;
        }


    }

    public class ArcusParams
    {
        
        public string Auth_code { set; get; }
        public string Rnn { set; get; }
        public string Terminal_id { get; set; }

        
        [ArcusFieldAttribute(NonSerialized = true)]
        public DateTime? OriginalDate { set; get; }

        public string Original_date_time
        {
            set
            {
                OriginalDate = value.FromArcusDtFormat();
            }
            get
            {
                return OriginalDate.ToArcusDtFormat();
            }
        }
         
        public override string ToString()
        {
            var s = "";
                        foreach (var p in this.GetType().GetProperties())
            {
                s += Environment.NewLine+p.Name + ": " + p.GetValue(this, null);
            }
            return s;
        }
    }

    public class ArcusResponse : ArcusParams
    {
        public ArcusResponse() { }


        public string Result_amount { get; set; }
        public string Pan { get; set; }
        public string Expiry { get; set; }
        public string Aid { get; set; }
        public string Card_type { get; set; }
        public string Application_label { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

        public string Terminal_serial_number { get; set; }
        public string Tvr { get; set; }

        public string Response_code { get; set; }
        public string Received_text_message { get; set; }
        public string Text_message { get; set; }
        public string Cardholder_name { get; set; }
        public string Slip { get; set; }
        
    }


    public class ArcusRequest : ArcusParams
    {
        public ArcusRequest()
        { }
        
        public string Currency { get; set; }
        public int Amount { set; get; }
        public int Original_amount { set; get; }
        public int Arcus_id { set; get; }
        

    }



    [AttributeUsage(AttributeTargets.Property)]
    public class ArcusFieldAttribute : Attribute
    {
        public ArcusFieldAttribute()
        {


        }

        private string _name = "";
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private bool _nonSerialized = false;
        public bool NonSerialized
        {
            get
            {
                return _nonSerialized;
            }
            set
            {
                _nonSerialized = value;
            }
        }


    }

    public static class ArcusExtensions
    {
        public static bool GetArcusNonSerialized(this PropertyInfo p)
        {
            return (p.GetCustomAttributes(typeof(ArcusFieldAttribute), false).Any() &&
                    ((ArcusFieldAttribute)p.GetCustomAttributes(typeof(ArcusFieldAttribute), false).First()).NonSerialized);
        }

        public static string GetArcusFieldName(this PropertyInfo p)
        {
            var name = p.Name.ToLower();

            if (p.GetCustomAttributes(typeof(ArcusFieldAttribute), false).Any() &&
                ((ArcusFieldAttribute)p.GetCustomAttributes(typeof(ArcusFieldAttribute), false).First()).Name != "")
            {
                name = ((ArcusFieldAttribute)p.GetCustomAttributes(typeof(ArcusFieldAttribute), false).First()).Name;
            }
            return name;
        }


        public static byte[] GetASCIIBytes(this object obj)
        {
            return Encoding.ASCII.GetBytes(obj.ToString());
        }
        public static string ToArcusDtFormat(this DateTime? dt)
        {
            return dt != null ? dt.Value.ToString("yyMMddhhmmss") : "";
        }
        public static DateTime? FromArcusDtFormat(this string dt)
        {
            try
            {
                return new DateTime(Convert.ToInt32(dt.Substring(0, 2)) + 2000, Convert.ToInt32(dt.Substring(2, 2)), Convert.ToInt32(dt.Substring(4, 2)), Convert.ToInt32(dt.Substring(6, 2)),
                    Convert.ToInt32(dt.Substring(8, 2)), Convert.ToInt32(dt.Substring(10, 2)));
            }
            catch
            {
                return null;
            }

        }
    }
}
