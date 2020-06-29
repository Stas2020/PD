using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics; 
using System.Runtime;
using System.Collections ;
using System.Collections.Generic ;
using System;

namespace PDiscountCard
{
    public class SetWinHook
    {
        private HookProc myCallbackDelegate = null;
        public IntPtr _hookID = IntPtr.Zero;

        public SetWinHook()
        {
            myCallbackDelegate = MyCallbackFunction;

            // setup a keyboard hook


            //using (Process process = Process.GetProcessesByName("iber")[0] )
            using (Process process = Process.GetCurrentProcess())
            using (ProcessModule module = process.MainModule)
            {
                IntPtr hModule = GetModuleHandle(module.ModuleName);

                    _hookID = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, myCallbackDelegate, hModule, 0);
            }
        }

        

        ~SetWinHook()
        
        {
            UnhookWindowsHookEx(_hookID);
        }



        public static  bool StopHook = false;



        private const int WM_KEYDOWN = 0x0100;
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        private delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);
        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }



        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
         static extern IntPtr GetModuleHandle(string lpModuleName);


        [DllImport("user32.dll")]
         static extern IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr hInstance, int threadID);

        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
       
       static  bool StartFirstRoad = false;
       static bool StartSecondRoad = false;
       static int OldvkCode = 0;
       static List<Keys> HookedKeys = new List<Keys>();

        internal void RaiseHookedKeys()
        {
            
            StopHook = true;
            foreach (Keys k in HookedKeys)
            {
                WinApi.keybd_event(k, 0, WinApi.KEYEVENTF_EXTENDEDKEY | WinApi.WM_KEYDOWN, (IntPtr)0);
                WinApi.keybd_event(k, 0, WinApi.KEYEVENTF_EXTENDEDKEY | WinApi.KEYEVENTF_KEYUP, (IntPtr)0);

            }
            HookedKeys.Clear();
            StopHook = false; 

        }
        private static int MyCallbackFunction(int code, IntPtr wParam, IntPtr lParam)
        {
           
            
            if ((code < 0)||(StopHook))
            {
                //you need to call CallNextHookEx without further processing
                //and return the value returned by CallNextHookEx
                return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }

            

            if (code >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                
                int vkCode = Marshal.ReadInt32(lParam);
                HookedKeys.Add((Keys)vkCode);
                Utils.ToLog("Before Нажата клавиша: " + ((Keys)vkCode)); 
                if ((Keys)vkCode == Keys.Oem1)
                {
                    SecondKeysList.Clear();
                    StartSecondRoad = true;
                    if (iniFile.CardEmulate)
                    {
                        OldvkCode = vkCode;
                        return 1;
                    }
                }
                else if (((Keys)vkCode == Keys.D5) && ((Keys)OldvkCode==Keys.LShiftKey))
                {
                    HookedKeys.Clear();
                    HookedKeys.Add(Keys.LShiftKey);
                    HookedKeys.Add(Keys.D5);
                    FirstKeysList.Clear();
                    StartFirstRoad = true;
                    if (iniFile.CardEmulate)
                    {
                        OldvkCode = vkCode;
                        return 1;
                    }
                }
                else if ((Keys)vkCode == Keys.OemQuestion)
                {
                    Utils.ToLog("StartFirstRoad: " + StartFirstRoad); 
                    if (StartFirstRoad)
                    {
                        StartFirstRoad = false;
                        if (iniFile.CardEmulate)
                        {
                            OldvkCode = vkCode;
                            return 1;
                        }
                    }
                    if (StartSecondRoad)
                    {
                        StartSecondRoad = false;
                        if (iniFile.CardEmulate)
                        {
                            
                            CardEmulate c = new CardEmulate();
                            c.ConvertCard(FirstKeysList, SecondKeysList);
                            if (iniFile.CardEmulate)
                            {
                                OldvkCode = vkCode;
                                return 1;
                            }
                        }
                        else
                        {
                            MainClass.SendCard(FirstKeysList, SecondKeysList);
                        }
                        FirstKeysList.Clear();
                        SecondKeysList.Clear();
                    }
                    
                }
                else
                {
                    if ((Keys)vkCode != Keys.LShiftKey)
                    {
                        if (StartFirstRoad)
                        {
                            FirstKeysList.Add(vkCode);
                            if (iniFile.CardEmulate)
                            {
                                OldvkCode = vkCode;
                                return 1;
                            }
                        }
                        else if (StartSecondRoad)
                        {
                            SecondKeysList.Add(vkCode);
                            if (iniFile.CardEmulate)
                            {
                                OldvkCode = vkCode;
                                return 1;
                            }
                        }
                        else
                        {
                            return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
                        }
                    }
                    else
                    {
                        if (iniFile.CardEmulate)
                        {
                            OldvkCode = vkCode;
                            return 1;
                        }
                    }
                    
                    //KeysList.Add(vkCode);
                }
                Utils.ToLog("Нажата клавиша: "+((Keys)vkCode));
                OldvkCode = vkCode;
            }

            /*
            if ((lParam.ToInt32() != 1242064))
            {
                // we can convert the 2nd parameter (the key code) to a System.Windows.Forms.Keys enum constant
                Keys keyPressed = (Keys)wParam.ToInt32();
                Utils.ToLog("Нажата клавиша: " + keyPressed);
                Utils.ToLog("code: " + code);
                Utils.ToLog("wParam: " + wParam);
                Utils.ToLog("lParam: " + lParam);
                Console.WriteLine(keyPressed);

                //return the value returned by CallNextHookEx
                // if (keyPressed==)
            }
             * */
            //return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            
                return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            
        }
        private static  List<int> FirstKeysList = new List<int>();
        private static List<int> SecondKeysList = new List<int>();
    }
}
