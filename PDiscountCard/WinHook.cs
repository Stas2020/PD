using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.RibbonHelpers
{
    /*
    /// <summary>
    /// Provides Windows Operative System specific functionallity.
    /// </summary>
    public static class WinApi
    {
        #region Constants


        public const int WM_MOUSEFIRST = 0x0200;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_RBUTTONDBLCLK = 0x0206;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MBUTTONDBLCLK = 0x0209;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_XBUTTONDOWN = 0x020B;
        public const int WM_XBUTTONUP = 0x020C;
        public const int WM_XBUTTONDBLCLK = 0x020D;
        public const int WM_MOUSELAST = 0x020D;

        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;

        public const byte VK_SHIFT = 0x10;
        public const byte VK_CAPITAL = 0x14;
        public const byte VK_NUMLOCK = 0x90;

        private const int DTT_COMPOSITED = (int)(1UL << 13);
        private const int DTT_GLOWSIZE = (int)(1UL << 11);

        private const int DT_SINGLELINE = 0x00000020;
        private const int DT_CENTER = 0x00000001;
        private const int DT_VCENTER = 0x00000004;
        private const int DT_NOPREFIX = 0x00000800;

        /// <summary>
        /// Enables the drop shadow effect on a window
        /// </summary>
        public const int CS_DROPSHADOW = 0x00020000;

        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level mouse input events.
        /// </summary>
        public const int WH_MOUSE_LL = 14;

        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level keyboard  input events.
        /// </summary>
        public const int WH_KEYBOARD_LL = 13;

        /// <summary>
        /// Installs a hook procedure that monitors mouse messages.
        /// </summary>
        public const int WH_MOUSE = 7;

        /// <summary>
        /// Installs a hook procedure that monitors keystroke messages.
        /// </summary>
        public const int WH_KEYBOARD = 2;

        /// <summary>
        /// The WM_NCLBUTTONUP message is posted when the user releases the left mouse button while the cursor is within the nonclient area of a window. 
        /// </summary>
        public const int WM_NCLBUTTONUP = 0x00A2;

        /// <summary>
        /// The WM_SIZE message is sent to a window after its size has changed.
        /// </summary>
        public const int WM_SIZE = 0x5;

        /// <summary>
        /// The WM_ERASEBKGND message is sent when the window background must be erased (for example, when a window is resized).
        /// </summary>
        public const int WM_ERASEBKGND = 0x14;

        /// <summary>
        /// The WM_NCCALCSIZE message is sent when the size and position of a window's client area must be calculated.
        /// </summary>
        public const int WM_NCCALCSIZE = 0x83;

        /// <summary>
        /// The WM_NCHITTEST message is sent to a window when the cursor moves, or when a mouse button is pressed or released.
        /// </summary>
        public const int WM_NCHITTEST = 0x84;

        /// <summary>
        /// The WM_NCMOUSEMOVE message is posted to a window when the cursor is moved within the nonclient area of the window. 
        /// </summary>
        public const int WM_NCMOUSEMOVE = 0x00A0;

        /// <summary>
        /// The WM_NCMOUSELEAVE message is posted to a window when the cursor leaves the nonclient area of the window specified in a prior call to TrackMouseEvent.
        /// </summary>
        public const int WM_NCMOUSELEAVE = 0x2a2;

        /// <summary>
        /// An uncompressed format.
        /// </summary>
        public const int BI_RGB = 0;

        /// <summary>
        /// The BITMAPINFO structure contains an array of literal RGB values.
        /// </summary>
        public const int DIB_RGB_COLORS = 0;

        /// <summary>
        /// Copies the source rectangle directly to the destination rectangle.
        /// </summary>
        public const int SRCCOPY = 0x00CC0020;

        #endregion

        #region Dll Imports

        [DllImport("user32")]
        internal static extern bool GetCursorPos(out POINT lpPoint);

        /// <summary>
        /// The ToAscii function translates the specified virtual-key code and keyboard state to the corresponding character or characters.
        /// </summary>
        /// <param name="uVirtKey"></param>
        /// <param name="uScanCode"></param>
        /// <param name="lpbKeyState"></param>
        /// <param name="lpwTransKey"></param>
        /// <param name="fuState"></param>
        /// <returns></returns>
        [DllImport("user32")]
        internal static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        /// <summary>
        /// The GetKeyboardState function copies the status of the 256 virtual keys to the specified buffer.
        /// </summary>
        /// <param name="pbKeyState"></param>
        /// <returns></returns>
        [DllImport("user32")]
        internal static extern int GetKeyboardState(byte[] pbKeyState);

        /// <summary>
        /// This function retrieves the status of the specified virtual key. The status specifies whether the key is up, down, or toggled on or off — alternating each time the key is pressed. 
        /// </summary>
        /// <param name="vKey"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern short GetKeyState(int vKey);

        /// <summary>
        /// Installs an application-defined hook procedure into a hook chain
        /// </summary>
        /// <param name="idHook"></param>
        /// <param name="lpfn"></param>
        /// <param name="hInstance"></param>
        /// <param name="threadId"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        internal static extern int SetWindowsHookEx(int idHook, GlobalHook.HookProcCallBack lpfn, IntPtr hInstance, int threadId);

        /// <summary>
        /// Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function. 
        /// </summary>
        /// <param name="idHook"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        internal static extern bool UnhookWindowsHookEx(int idHook);

        /// <summary>
        /// Passes the hook information to the next hook procedure in the current hook chain
        /// </summary>
        /// <param name="idHook"></param>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        internal static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// This function retrieves a handle to a display device context (DC) for the client area of the specified window.
        /// </summary>
        /// <param name="hdc"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr hdc);

        /// <summary>
        /// The SaveDC function saves the current state of the specified device context (DC) by copying data describing selected objects and graphic modes
        /// </summary>
        /// <param name="hdc"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        internal static extern int SaveDC(IntPtr hdc);

        /// <summary>
        /// This function releases a device context (DC), freeing it for use by other applications. The effect of ReleaseDC depends on the type of device context.
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        internal static extern int ReleaseDC(IntPtr hdc, int state);

        /// <summary>
        /// Draws text using the color and font defined by the visual style. Extends DrawThemeText by allowing additional text format options.
        /// </summary>
        /// <param name="hTheme"></param>
        /// <param name="hdc"></param>
        /// <param name="iPartId"></param>
        /// <param name="iStateId"></param>
        /// <param name="text"></param>
        /// <param name="iCharCount"></param>
        /// <param name="dwFlags"></param>
        /// <param name="pRect"></param>
        /// <param name="pOptions"></param>
        /// <returns></returns>
        [DllImport("UxTheme.dll", CharSet = CharSet.Unicode)]
        private static extern int DrawThemeTextEx(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref RECT pRect, ref DTTOPTS pOptions);

        /// <summary>
        /// Draws text using the color and font defined by the visual style.
        /// </summary>
        /// <param name="hTheme"></param>
        /// <param name="hdc"></param>
        /// <param name="iPartId"></param>
        /// <param name="iStateId"></param>
        /// <param name="text"></param>
        /// <param name="iCharCount"></param>
        /// <param name="dwFlags1"></param>
        /// <param name="dwFlags2"></param>
        /// <param name="pRect"></param>
        /// <returns></returns>
        [DllImport("UxTheme.dll")]
        internal static extern int DrawThemeText(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags1, int dwFlags2, ref RECT pRect);

        /// <summary>
        /// The CreateDIBSection function creates a DIB that applications can write to directly.
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="pbmi"></param>
        /// <param name="iUsage"></param>
        /// <param name="ppvBits"></param>
        /// <param name="hSection"></param>
        /// <param name="dwOffset"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint iUsage, int ppvBits, IntPtr hSection, uint dwOffset);

        /// <summary>
        /// This function transfers pixels from a specified source rectangle to a specified destination rectangle, altering the pixels according to the selected raster operation (ROP) code.
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="nXDest"></param>
        /// <param name="nYDest"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <param name="hdcSrc"></param>
        /// <param name="nXSrc"></param>
        /// <param name="nYSrc"></param>
        /// <param name="dwRop"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        internal static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        /// <summary>
        /// This function creates a memory device context (DC) compatible with the specified device.
        /// </summary>
        /// <param name="hDC"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        /// <summary>
        /// This function selects an object into a specified device context. The new object replaces the previous object of the same type.
        /// </summary>
        /// <param name="hDC"></param>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        internal static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        /// <summary>
        /// The DeleteObject function deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object. After the object is deleted, the specified handle is no longer valid.
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// The DeleteDC function deletes the specified device context (DC).
        /// </summary>
        /// <param name="hdc"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        internal static extern bool DeleteDC(IntPtr hdc);

        /// <summary>
        /// Extends the window frame behind the client area.
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="marInset"></param>
        /// <returns></returns>
        [DllImport("dwmapi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hdc, ref MARGINS marInset);

        /// <summary>
        /// Default window procedure for Desktop Window Manager (DWM) hit-testing within the non-client area.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [DllImport("dwmapi.dll")]
        internal static extern int DwmDefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, out IntPtr result);

        /// <summary>
        /// Obtains a value that indicates whether Desktop Window Manager (DWM) composition is enabled.
        /// </summary>
        /// <param name="pfEnabled"></param>
        /// <returns></returns>
        [DllImport("dwmapi.dll")]
        internal static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        /// <summary>
        /// Sends the specified message to a window or windows
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region Structs

        /// <summary>
        /// Contains information about a mouse event passed to a WH_MOUSE hook procedure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal class MouseLLHookStruct
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public int extraInfo;
        }

        /// <summary>
        /// Contains information about a low-level keyboard input event.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal class KeyboardLLHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        /// <summary>
        /// Contains information about a mouse event passed to a WH_MOUSE hook procedure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        /// <summary>
        /// Represents a point
        /// </summary>
        internal struct POINT
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// Defines the options for the DrawThemeTextEx function.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct DTTOPTS
        {
            public uint dwSize;
            public uint dwFlags;
            public uint crText;
            public uint crBorder;
            public uint crShadow;
            public int iTextShadowType;
            public POINT ptShadowOffset;
            public int iBorderSize;
            public int iFontPropId;
            public int iColorPropId;
            public int iStateId;
            public int fApplyOverlay;
            public int iGlowSize;
            public IntPtr pfnDrawTextCallback;
            public int lParam;
        }

        /// <summary>
        /// This structure describes a color consisting of relative intensities of red, green, and blue.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }


        /// <summary>
        /// This structure contains information about the dimensions and color format of a device-independent bitmap (DIB).
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        /// <summary>
        /// This structure defines the dimensions and color information of a Windows-based device-independent bitmap (DIB).
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public RGBQUAD bmiColors;
        }

        /// <summary>
        /// Describes the width, height, and location of a rectangle.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        /// <summary>
        /// The NCCALCSIZE_PARAMS structure contains information that an application can use 
        /// while processing the WM_NCCALCSIZE message to calculate the size, position, and 
        /// valid contents of the client area of a window. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NCCALCSIZE_PARAMS
        {
            public RECT rect0, rect1, rect2;                    // Can't use an array here so simulate one
            public IntPtr lppos;
        }

        /// <summary>
        /// Used to specify margins of a window
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;

            public MARGINS(int Left, int Right, int Top, int Bottom)
            {
                this.cxLeftWidth = Left;
                this.cxRightWidth = Right;
                this.cyTopHeight = Top;
                this.cyBottomHeight = Bottom;
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets if the current OS is Windows NT or later
        /// </summary>
        public static bool IsWindows
        {
            get { return Environment.OSVersion.Platform == PlatformID.Win32NT; }
        }

        /// <summary>
        /// Gets a value indicating if operating system is vista or higher
        /// </summary>
        public static bool IsVista
        {
            get { return IsWindows && Environment.OSVersion.Version.Major >= 6; }
        }

        /// <summary>
        /// Gets a value indicating if operating system is xp or higher
        /// </summary>
        public static bool IsXP
        {
            get { return IsWindows && Environment.OSVersion.Version.Major >= 5; }
        }

        /// <summary>
        /// Gets if computer is glass capable and enabled
        /// </summary>
        public static bool IsGlassEnabled
        {
            get
            {
                //Check is windows vista
                if (IsVista)
                {
                    //Check what DWM says about composition
                    int enabled = 0;
                    int response = DwmIsCompositionEnabled(ref enabled);

                    return enabled > 0;
                }

                return false;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Equivalent to the HiWord C Macro
        /// </summary>
        /// <param name="dwValue"></param>
        /// <returns></returns>
        public static int HiWord(int dwValue)
        {
            return (dwValue >> 16) & 0xFFFF;
        }

        /// <summary>
        /// Equivalent to the LoWord C Macro
        /// </summary>
        /// <param name="dwValue"></param>
        /// <returns></returns>
        public static int LoWord(int dwValue)
        {
            return dwValue & 0xFFFF;
        }

        /// <summary>
        /// Equivalent to the MakeLParam C Macro
        /// </summary>
        /// <param name="LoWord"></param>
        /// <param name="HiWord"></param>
        /// <returns></returns>
        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return new IntPtr((HiWord << 16) | (LoWord & 0xffff));
        }

        /// <summary>
        /// Fills an area for glass rendering
        /// </summary>
        /// <param name="gph"></param>
        /// <param name="rgn"></param>
        public static void FillForGlass(Graphics g, Rectangle r)
        {
            RECT rc = new RECT();
            rc.Left = r.Left;
            rc.Right = r.Right;
            rc.Top = r.Top;
            rc.Bottom = r.Bottom;

            IntPtr destdc = g.GetHdc();    //hwnd must be the handle of form,not control
            IntPtr Memdc = CreateCompatibleDC(destdc);
            IntPtr bitmap;
            IntPtr bitmapOld = IntPtr.Zero;

            BITMAPINFO dib = new BITMAPINFO();
            dib.bmiHeader.biHeight = -(rc.Bottom - rc.Top);
            dib.bmiHeader.biWidth = rc.Right - rc.Left;
            dib.bmiHeader.biPlanes = 1;
            dib.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            dib.bmiHeader.biBitCount = 32;
            dib.bmiHeader.biCompression = BI_RGB;
            if (!(SaveDC(Memdc) == 0))
            {
                bitmap = CreateDIBSection(Memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
                if (!(bitmap == IntPtr.Zero))
                {
                    bitmapOld = SelectObject(Memdc, bitmap);
                    BitBlt(destdc, rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top, Memdc, 0, 0, SRCCOPY);

                }

                //Remember to clean up
                SelectObject(Memdc, bitmapOld);

                DeleteObject(bitmap);

                ReleaseDC(Memdc, -1);
                DeleteDC(Memdc);


            }
            g.ReleaseHdc();

        }

        /// <summary>
        /// Draws theme text on glass.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="ctlrct"></param>
        /// <param name="iglowSize"></param>
        /// <remarks>This method is courtesy of 版权所有 (I hope the name's right)</remarks>
        public static void DrawTextOnGlass(IntPtr hwnd, String text, Font font, Rectangle ctlrct, int iglowSize)
        {
            if (IsGlassEnabled)
            {
                RECT rc = new RECT();
                RECT rc2 = new RECT();

                rc.Left = ctlrct.Left;
                rc.Right = ctlrct.Right;// +2 * iglowSize;  //make it larger to contain the glow effect
                rc.Top = ctlrct.Top;
                rc.Bottom = ctlrct.Bottom;// +2 * iglowSize;

                //Just the same rect with rc,but (0,0) at the lefttop
                rc2.Left = 0;
                rc2.Top = 0;
                rc2.Right = rc.Right - rc.Left;
                rc2.Bottom = rc.Bottom - rc.Top;

                IntPtr destdc = GetDC(hwnd);    //hwnd must be the handle of form,not control
                IntPtr Memdc = CreateCompatibleDC(destdc);   // Set up a memory DC where we'll draw the text.
                IntPtr bitmap;
                IntPtr bitmapOld = IntPtr.Zero;
                IntPtr logfnotOld;

                int uFormat = DT_SINGLELINE | DT_CENTER | DT_VCENTER | DT_NOPREFIX;   //text format

                BITMAPINFO dib = new BITMAPINFO();
                dib.bmiHeader.biHeight = -(rc.Bottom - rc.Top);         // negative because DrawThemeTextEx() uses a top-down DIB
                dib.bmiHeader.biWidth = rc.Right - rc.Left;
                dib.bmiHeader.biPlanes = 1;
                dib.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                dib.bmiHeader.biBitCount = 32;
                dib.bmiHeader.biCompression = BI_RGB;
                if (!(SaveDC(Memdc) == 0))
                {
                    bitmap = CreateDIBSection(Memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);   // Create a 32-bit bmp for use in offscreen drawing when glass is on
                    if (!(bitmap == IntPtr.Zero))
                    {
                        bitmapOld = SelectObject(Memdc, bitmap);
                        IntPtr hFont = font.ToHfont();
                        logfnotOld = SelectObject(Memdc, hFont);
                        try
                        {

                            VisualStyleRenderer renderer = new VisualStyleRenderer(VisualStyleElement.Window.Caption.Active);

                            DTTOPTS dttOpts = new DTTOPTS();

                            dttOpts.dwSize = (uint)Marshal.SizeOf(typeof(DTTOPTS));

                            dttOpts.dwFlags = DTT_COMPOSITED | DTT_GLOWSIZE;


                            dttOpts.iGlowSize = iglowSize;

                            int dtter = DrawThemeTextEx(renderer.Handle, Memdc, 0, 0, text, -1, uFormat, ref rc2, ref dttOpts);

                            bool bbr = BitBlt(destdc, rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top, Memdc, 0, 0, SRCCOPY);

                            if (!bbr)
                            {
                                //throw new Exception("???");
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            //throw new Exception("???");
                        }


                        //Remember to clean up
                        SelectObject(Memdc, bitmapOld);
                        SelectObject(Memdc, logfnotOld);
                        DeleteObject(bitmap);
                        DeleteObject(hFont);

                        ReleaseDC(Memdc, -1);
                        DeleteDC(Memdc);



                    }
                    else
                    {
                        //throw new Exception("???");
                    }
                }
                else
                {
                    //throw new Exception("???");
                }



            }

        }
        #endregion
    }
     * */
}

