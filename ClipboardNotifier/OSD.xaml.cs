using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClipboardNotifier
{
    /// <summary>
    /// Interaction logic for OSD.xaml
    /// </summary>
    public partial class OSD : Window
    {
        public OSD()
        {
            InitializeComponent();
        }

        private HwndSource hWndSource;
        private IntPtr hWndNextViewer;





        




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
                    this.Left = (SystemParameters.PrimaryScreenWidth - 64) / 2;
                    this.Top = SystemParameters.PrimaryScreenHeight - 64 - 100;
                    this.Visibility = Visibility.Visible;
                    Thread thread = new Thread(() =>
                    {
                        Thread.Sleep(1000);
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            this.Visibility = Visibility.Hidden;
                        }));
                    });
                    thread.Start();
                    SendMessage(hWndNextViewer, msg, wParam, lParam);
                    break;
            }
            return IntPtr.Zero;
        }


        public void OpenCBViewer()
        {
            this.Visibility = Visibility.Hidden;
            this.Show();
            WindowInteropHelper wih = new WindowInteropHelper(this);
            hWndSource = HwndSource.FromHwnd(wih.Handle);
            hWndSource.AddHook(this.WinProc);
            hWndNextViewer = SetClipboardViewer(hWndSource.Handle);
        }

        public void CloseCBViewer()
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
