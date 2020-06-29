using System.Windows.Forms; 
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime;
using System.Runtime.InteropServices;



namespace PDiscountCard
{
    public  class WinApi
    {
        [Flags]
      public   enum CLSCTX : uint
        {
            CLSCTX_INPROC_SERVER = 0x1,
            CLSCTX_INPROC_HANDLER = 0x2,
            CLSCTX_LOCAL_SERVER = 0x4,
            CLSCTX_INPROC_SERVER16 = 0x8,
            CLSCTX_REMOTE_SERVER = 0x10,
            CLSCTX_INPROC_HANDLER16 = 0x20,
            CLSCTX_RESERVED1 = 0x40,
            CLSCTX_RESERVED2 = 0x80,
            CLSCTX_RESERVED3 = 0x100,
            CLSCTX_RESERVED4 = 0x200,
            CLSCTX_NO_CODE_DOWNLOAD = 0x400,
            CLSCTX_RESERVED5 = 0x800,
            CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
            CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
            CLSCTX_NO_FAILURE_LOG = 0x4000,
            CLSCTX_DISABLE_AAA = 0x8000,
            CLSCTX_ENABLE_AAA = 0x10000,
            CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
            CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
            CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
            CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
        }

        public  const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x0100;
        //private static LowLevelKeyboardProc _proc = HookCallback;
        public static IntPtr _hookID = IntPtr.Zero;
        public const UInt32 KEYEVENTF_EXTENDEDKEY = 1;
        public const UInt32 KEYEVENTF_KEYUP = 2;

        [DllImport("user32.dll", EntryPoint = "keybd_event", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, UInt32 dwFlags, IntPtr dwExtraInfo);

        

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        /// <summary>The GetForegroundWindow function returns a handle to the foreground window.</summary>
        [DllImport("user32.dll")]
        public  static extern IntPtr GetForegroundWindow();


       public  enum ShowWindowCommands : int
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            Hide = 0,
            /// <summary>
            /// Activates and displays a window. If the window is minimized or 
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when displaying the window 
            /// for the first time.
            /// </summary>
            Normal = 1,
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            ShowMinimized = 2,
            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            Maximize = 3, // is this the right value?
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>       
            ShowMaximized = 3,
            /// <summary>
            /// Displays a window in its most recent size and position. This value 
            /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except 
            /// the window is not activated.
            /// </summary>
            ShowNoActivate = 4,
            /// <summary>
            /// Activates the window and displays it in its current size and position. 
            /// </summary>
            Show = 5,
            /// <summary>
            /// Minimizes the specified window and activates the next top-level 
            /// window in the Z order.
            /// </summary>
            Minimize = 6,
            /// <summary>
            /// Displays the window as a minimized window. This value is similar to
            /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the 
            /// window is not activated.
            /// </summary>
            ShowMinNoActive = 7,
            /// <summary>
            /// Displays the window in its current size and position. This value is 
            /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the 
            /// window is not activated.
            /// </summary>
            ShowNA = 8,
            /// <summary>
            /// Activates and displays the window. If the window is minimized or 
            /// maximized, the system restores it to its original size and position. 
            /// An application should specify this flag when restoring a minimized window.
            /// </summary>
            Restore = 9,
            /// <summary>
            /// Sets the show state based on the SW_* value specified in the 
            /// STARTUPINFO structure passed to the CreateProcess function by the 
            /// program that started the application.
            /// </summary>
            ShowDefault = 10,
            /// <summary>
            ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread 
            /// that owns the window is not responding. This flag should only be 
            /// used when minimizing windows from a different thread.
            /// </summary>
            ForceMinimize = 11
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static public  extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);


        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
             int hWnd,           // window handle
             int hWndInsertAfter,    // placement-order handle
             int X,          // horizontal position
             int Y,          // vertical position
             int cx,         // width
             int cy,         // height
             uint uFlags);       // window positioning flags

        private const int SW_SHOWNOACTIVATE = 4;
        private const int HWND_TOPMOST = -1;
        private const uint SWP_NOACTIVATE = 0x0010;
        const UInt32 SWP_SHOWWINDOW = 0x0040;
        private const int SW_NORMAL = 1; 

        static public void ShowTopmost(Form frm, int Top, int Left, int Width, int Height)
        {

            ShowTopmost(frm.Handle, Top, Left, Width, Height);
        }

        static public void ShowTopmost(IntPtr handle, int Top, int Left, int Width, int Height)
        {
            ShowWindow(handle, ShowWindowCommands.Normal);

            SetWindowPos(handle.ToInt32(), HWND_TOPMOST,
            Left, Top, Width, Height,
            SWP_SHOWWINDOW);

        }


        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public  static extern object CoCreateInstance(
           [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
           [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
           CLSCTX dwClsContext,
           [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid);


        [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public  static extern object CoGetClassObject(
           [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
           CLSCTX dwClsContext,
           IntPtr pServerInfo,
           [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid);


        [ComImport]
        [Guid("B196B28F-BAB4-101A-B69C-00AA00341D07")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public  interface IClassFactory2
        {
            /// <summary>
            /// the standard create instance (without licence)
            /// </summary>
            /// <param name="unused">must be set to null</param>
            /// <param name="iid">the Guid of the COM class to create</param>
            /// <returns>an instance of the COM class</returns>
            [return: MarshalAs(UnmanagedType.Interface)]
            Object CreateInstance(
              [In, MarshalAs(UnmanagedType.Interface)] Object unused,
              [In, MarshalAs(UnmanagedType.LPStruct)] Guid iid);

            void LockServer(Int32 fLock);

            IntPtr GetLicInfo(); // TODO : an enum called LICINFO

            [return: MarshalAs(UnmanagedType.BStr)]
            String RequestLicKey(
              [In, MarshalAs(UnmanagedType.U4)] int reserved);

            /// <summary>
            /// create an instance of the COM class thanks to a licence key
            /// </summary>
            /// <param name="pUnkOuter">don't know what it is, set to null</param>
            /// <param name="pUnkReserved">must be set to null</param>
            /// <param name="iid">the Guid of the COM class to create</param>
            /// <param name="bstrKey">the licence key</param>
            /// <returns>an instance of the COM class</returns>
            [return: MarshalAs(UnmanagedType.Interface)]
            Object CreateInstanceLic(
              [In, MarshalAs(UnmanagedType.Interface)] object pUnkOuter,
              [In, MarshalAs(UnmanagedType.Interface)] object pUnkReserved,
              [In, MarshalAs(UnmanagedType.LPStruct)] Guid iid,
              [In, MarshalAs(UnmanagedType.BStr)] string bstrKey);
        }



    }
}
