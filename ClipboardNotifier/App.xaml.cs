using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ClipboardNotifier
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;
        private OSD osd;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            osd = new OSD();
            osd.OpenCBViewer();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            osd.CloseCBViewer();
            notifyIcon.Dispose();
            base.OnExit(e);
        }
    }
}
