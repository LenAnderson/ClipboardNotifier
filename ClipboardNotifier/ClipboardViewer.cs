using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace ClipboardNotifier
{
    class ClipboardViewer
    {
        private HwndSource hWndSource;
        private IntPtr hWndNextViewer;





        public ClipboardViewer(OSD osd)
        {
            WindowInteropHelper wih = new WindowInteropHelper(osd);
            hWndSource = HwndSource.FromHwnd(wih.Handle);
            hWndSource.AddHook(this.WinProc);
            hWndNextViewer = SetClipboardViewer(hWndSource.Handle);
        }




        private IntPtr WinProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_CHANGECBCHAIN:
                    if (wParam == hWndNextViewer)
                    {
                        hWndNextViewer = lParam;
                    }
                    else if (hWndNextViewer != IntPtr.Zero)
                    {
                        SendMessage(hWndNextViewer, msg, wParam, lParam);
                    }
                    break;

                case WM_DRAWCLIPBOARD:
                    //TODO: raise event
                    SendMessage(hWndNextViewer, msg, wParam, lParam);
                    break;
            }
            return IntPtr.Zero;
        }


        private void CloseCBViewer()
        {
            ChangeClipboardChain(hWndSource.Handle, hWndNextViewer);
            hWndNextViewer = IntPtr.Zero;
            hWndSource.RemoveHook(this.WinProc);
        }




        #region interop
        internal const int WM_DRAWCLIPBOARD = 0x0308;
        internal const int WM_CHANGECBCHAIN = 0x030D;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        #endregion
    }
}
