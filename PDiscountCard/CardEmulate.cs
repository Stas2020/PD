using System;
using System.Collections.Generic;
using System.Windows.Forms ; 
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace PDiscountCard
{
    class CardEmulate
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        //private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        public const UInt32 KEYEVENTF_EXTENDEDKEY = 1;
        public const UInt32 KEYEVENTF_KEYUP = 2;



        private string GetStrByKeys(List<int> FRoad)
        {
            string tmp = "";
            foreach (int i in FRoad)
            { 
                
                Keys k = (Keys)i;
                tmp += k.ToString ();
            }
            return tmp;
        }

        public void ConvertCard(List<int> FRoad, List<int> SRoad)
        {
            // if (FRoad[0] == (int)Keys.Z)
            //   {



            string FR = GetStrByKeys(FRoad);
            /*
            if (iniFile.Read("Discounts", Prefix.ToUpper()) != null)
                    {
                        return CardTypes.Discount;
                    }
                    else if (iniFile.Read("PrivilegedKey", Prefix + Num) != null)
                    {
                        return CardTypes.Discount;
                    }

            */
            if (iniFile.Read("Emulator", FR.ToUpper()) != null)
            {
                string GetRestrPrefiks = iniFile.Read("Emulator", FR.ToUpper());
                
                List<int> Tmp = new List<int>();

                foreach (char ch in GetRestrPrefiks)
                {
                    int i = Convert.ToInt32(ch);
                    //Keys k = GetKeyByInt(i);
                    Tmp.Add(i);
                }

               
                /*
                Tmp.Add((int)Keys.D9);
                Tmp.Add((int)Keys.D0);
                Tmp.Add((int)Keys.D6);
                Tmp.Add((int)Keys.D5);
                Tmp.Add((int)Keys.D8);
                 * */
                /*
                Tmp.Add((int)Keys.D0);
                Tmp.Add((int)Keys.D0);
                Tmp.Add((int)Keys.D0);
                Tmp.Add((int)Keys.D1);
                Tmp.Add((int)Keys.D2);
                 * */
                //string SecondRoad = GetStrByKeys(SRoad );
                string SecondRoad = "";
                int C = 0;
                foreach (int ss in SRoad)
                {
                    if (C > 8)
                    {
                        continue;
                    }
                    C++;
                    Keys k = (Keys)ss;

                    SecondRoad += k.ToString()[1];
                }

                if (iniFile.Read("PrivilegedKey", FRoad  + SecondRoad) != null)
                    {
                        GetRestrPrefiks = iniFile.Read("Emulator", "VIP50");
                    }

                for (int i = 9; i > 0;i-- )
                {
                    int k = 0;
                    try
                    {
                        k = SRoad[SRoad.Count - i];
                    }
                    catch
                    { }
                    Tmp.Add(k);
                    
                }
                /*
                    Tmp.Add(SRoad[SRoad.Count - 4]);
                Tmp.Add(SRoad[SRoad.Count - 5]);
                Tmp.Add(SRoad[SRoad.Count - 6]);
                Tmp.Add(SRoad[SRoad.Count - 1]);
                 * */
              // RaiseCard(Tmp);


                int z = ToBase.DoVizitAsink(FR.ToUpper(), SecondRoad, AlohainiFile.DepNum, AlohaTSClass.AlohaCurentState.CheckNum,
        AlohaTSClass.AlohaCurentState.TerminalId, DateTime.Now);
                






                MainClass.AssignMember(GetRestrPrefiks + SecondRoad, GetRestrPrefiks + SecondRoad);
            }
            else
            {
                //MainClass.mSetWinHook.RaiseHookedKeys();   
                MainClass.SendCard(FRoad, SRoad);
            }
        }
        //}


        internal bool  EmulateLoyaltyCard(string Pref, string Num)
        {
            if ((iniFile.Read("Emulator", Pref.ToUpper()) != null) || (Pref.Length == 5))
            {

                AlohaTSClass.ShowMessage("Распознал карту " + Pref  + Num);
                if (AlohaTSClass.AlohaCurentState.CompIsAppled)
                {
                    AlohaTSClass.ShowMessage("Скидка уже применена");
                    return true ;
                }
                if (AlohaTSClass.AlohaCurentState.PredCheckAndNotManager)
                {
                    AlohaTSClass.ShowMessage("Применить скидку по данному чеку может только менеджер.");
                    return true ;
                }

                string GetRestrPrefiks = "";
                string NewNum = "";
                if (Pref.Length == 5)
                {
                    GetRestrPrefiks = Pref;
                    NewNum = Num;
                }
                else
                {
                    Utils.ToLog("Конвертирую карту");
                    GetRestrPrefiks = iniFile.Read("Emulator", Pref.ToUpper());

                    if (iniFile.Read("PrivilegedKey", Pref + Num) == "8")
                    {
                        GetRestrPrefiks = iniFile.Read("Emulator", "VIP50");
                    }

                    if (iniFile.Read("PrivilegedKey", Pref + Num) == "2")
                    {
                        GetRestrPrefiks = iniFile.Read("Emulator", "VIP30");
                    }

                    int z = ToBase.DoVizitAsink(Pref.ToUpper(), Num, AlohainiFile.DepNum, AlohaTSClass.AlohaCurentState.CheckNum,
            AlohaTSClass.AlohaCurentState.TerminalId, DateTime.Now);

                    NewNum = ConvertNumTo9Digits(Num);
                }

                        string OldDisc = AlohaTSClass.GetDiscountAttr((int)AlohaTSClass.AlohaCurentState.CheckId);

                if ((OldDisc != "") &&
                    (OldDisc.Replace(" ", "") != (Pref + Num).Replace(" ", "")) &&
                    (Utils.GetDiscountNumByCardSeries(Convert.ToInt32(GetRestrPrefiks)) != 6))
                {
                    Utils.ToCardLog(String.Format("На чек {0} наложен атрибут {1}", AlohaTSClass.AlohaCurentState.CheckId, OldDisc));

                    AlohaTSClass.ShowMessage("Нельзя применить скидку на чек с зарегистрированным посещением. Совсем нельзя. ");
                    return true;
                }


                MainClass.AssignMember(GetRestrPrefiks + NewNum, GetRestrPrefiks + NewNum);
                return true;
            }
            return false;
        }

        private string ConvertNumTo9Digits(string num)
        {
            string NewNum = "";
            if (num.Length < 9)
            {
                for (int i = num.Length; i < 10; i++)
                {
                    NewNum += "0";
                }
                return NewNum + num;
            }
            else if (num.Length > 9)
            {
                return num.Substring(num.Length - 9, 9);
            }
            return num;
        }

        private  void RaiseCard(List<int> Card)
        {
            SetWinHook.StopHook = true; 
            WinApi.keybd_event(Keys.LShiftKey, 0, KEYEVENTF_EXTENDEDKEY | WM_KEYDOWN, (IntPtr)0);
            WinApi.keybd_event(Keys.D5, 0, KEYEVENTF_EXTENDEDKEY | WM_KEYDOWN, (IntPtr)0);
            WinApi.keybd_event(Keys.D5, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (IntPtr)0);
            WinApi.keybd_event(Keys.LShiftKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (IntPtr)0);
            NextRoad();
            /*
            WinApi.keybd_event(Keys.A, 0, KEYEVENTF_EXTENDEDKEY | WM_KEYDOWN, (IntPtr)0);
            WinApi.keybd_event(Keys.A, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (IntPtr)0);
            WinApi.keybd_event(Keys.E, 0, KEYEVENTF_EXTENDEDKEY | WM_KEYDOWN, (IntPtr)0);
            WinApi.keybd_event(Keys.E, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (IntPtr)0);
             * */
            
            foreach (int k in Card)
            {
               
                    
                RaiseKeyEvent(GetKeyByInt(k));
            }
            
            
            WinApi.keybd_event(Keys.LShiftKey, 0, KEYEVENTF_EXTENDEDKEY | WM_KEYDOWN, (IntPtr)0);
            WinApi.keybd_event(Keys.Oem2, 0, KEYEVENTF_EXTENDEDKEY | WM_KEYDOWN, (IntPtr)0);
            WinApi.keybd_event(Keys.Oem2, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (IntPtr)0);
            WinApi.keybd_event(Keys.LShiftKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (IntPtr)0);
            WinApi.keybd_event(Keys.Return, 0, KEYEVENTF_EXTENDEDKEY | WM_KEYDOWN, (IntPtr)0);
            WinApi.keybd_event(Keys.Return, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (IntPtr)0);
            SetWinHook.StopHook = false; 
        }
     
        private Keys GetKeyByInt(int k)
        
        {
            switch (k)
            {
                case (int)Keys.D0:
                    return Keys.D0;
                case (int)Keys.D1:
                    return Keys.D1;
                case (int)Keys.D2:
                    return Keys.D2;
                case (int)Keys.D3:
                    return Keys.D3;
                case (int)Keys.D4:
                    return Keys.D4;
                case (int)Keys.D5:
                    return Keys.D5;
                case (int)Keys.D6:
                    return Keys.D6;
                case (int)Keys.D7:
                    return Keys.D7;
                case (int)Keys.D8:
                    return Keys.D8;
                case (int)Keys.D9:
                    return Keys.D9;
                default:
                    return Keys.D0; 
                    
            }
        }

        public void RaiseKeyEvent(Keys K)
        {
            
           WinApi.keybd_event(K, 0, KEYEVENTF_EXTENDEDKEY | WM_KEYDOWN, (IntPtr)0);
           WinApi.keybd_event(K, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (IntPtr)0);
        }

        static private void NextRoad()
        {
            WinApi.keybd_event(Keys.LShiftKey, 0, KEYEVENTF_EXTENDEDKEY | WM_KEYDOWN, (IntPtr)0);
            WinApi.keybd_event(Keys.Oem2, 0, KEYEVENTF_EXTENDEDKEY | WM_KEYDOWN, (IntPtr)0);
            WinApi.keybd_event(Keys.Oem2, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (IntPtr)0);
            WinApi.keybd_event(Keys.LShiftKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (IntPtr)0);
            WinApi.keybd_event(Keys.Oem1, 0, KEYEVENTF_EXTENDEDKEY | WM_KEYDOWN, (IntPtr)0);
            WinApi.keybd_event(Keys.Oem1, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (IntPtr)0);
        }

    }
    
}
