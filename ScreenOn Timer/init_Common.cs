using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Forms;
using Utilities;

namespace ScreenOn_Timer
{
    class init_Common
    {
        private double screenTimeTotal_notify;
        private System.Windows.Forms.NotifyIcon notifyIcon = null;
        private bool flagMoveEnabled = true;
        Window myWindow;
        Thread mousePointerThread;
        globalKeyboardHook keyCheckHook_temp;
        ContextMenuStrip notifyContext1;
        ToolStripMenuItem menuItem1, menuItem2;

        public init_Common(Window window)
        {
            myWindow = window;
            return;
        }

        public void init_window(Window window,float taskBar_height)
        {
            //Setup window
            
            myWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            myWindow.Left = SystemParameters.PrimaryScreenWidth - window.Width;
            myWindow.Top = taskBar_height - window.Height;
        }

        public void init_setAutoStartup()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            //System.AppDomain.CurrentDomain.BaseDirectory;
            rk.SetValue(System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name,  System.Windows.Forms.Application.ExecutablePath);

        }

        private void notifyTray_ContextMenu()
        {

            notifyContext1 = new ContextMenuStrip();

            menuItem1 = new ToolStripMenuItem("Exit");
            menuItem2 = new ToolStripMenuItem("Minimize");

            notifyContext1.ItemClicked += new ToolStripItemClickedEventHandler(menuItem_Click);
            notifyContext1.Name = "notifyContextMenu";

            notifyContext1.Items.Add(menuItem1);
            notifyContext1.Items.Add(menuItem2);
        }
        
        public void init_notifyTray(Thread thread, globalKeyboardHook keyCheckHookPass)
        {
            keyCheckHook_temp = keyCheckHookPass;
            mousePointerThread = thread;
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            //notifyIcon.Click += new System.EventHandler(this.notifyIcon_Click);
            notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            notifyIcon.Icon =
                System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name);
            notifyIcon.Visible = true;
            notifyIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseMove);
            notifyIcon.ShowBalloonTip(2000, "ScreenOnTimer", "Usage Start", ToolTipIcon.Info);
            notifyIcon.BalloonTipShown += new EventHandler(notifyIcon_BalloonTipShown);
            notifyIcon.BalloonTipClosed += new EventHandler(notifyIcon_BalloonTipClosed);
            notifyTray_ContextMenu();
            notifyIcon.ContextMenuStrip = notifyContext1;
        }

        void menuItem_Click(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem menuItem = e.ClickedItem;

            if (menuItem == menuItem1)
            {
                if (mousePointerThread != null)
                {
                    mousePointerThread.Abort();
                }
                keyCheckHook_temp.unhook();
                notifyIcon.Dispose();
                myWindow.Close();
                System.Windows.Application.Current.Shutdown();

            }
            else if (menuItem == menuItem2)
            {        
                // Minimize or Restore the Window
                if (myWindow.WindowState == System.Windows.WindowState.Normal)
                {
                    myWindow.WindowState = System.Windows.WindowState.Minimized;
                    myWindow.Visibility = System.Windows.Visibility.Hidden;
                    menuItem2.Text = "Restore";
                }
                else
                {
                    myWindow.Show();
                    myWindow.WindowState = System.Windows.WindowState.Normal;
                    menuItem2.Text = "Minimize";

                }
            }
        }

        /*private void notifyIcon_Click(object Sender, EventArgs e)
        {
            // Minimize or Restore the Window
            if (myWindow.WindowState == System.Windows.WindowState.Normal)
            {
                myWindow.WindowState = System.Windows.WindowState.Minimized;
                myWindow.Visibility = System.Windows.Visibility.Hidden;
                menuItem2.Text = "Restore";
            }
            else
            {
                myWindow.Show();
                myWindow.WindowState = System.Windows.WindowState.Normal;
                menuItem2.Text = "Minimize";

            }
        }*/

        private void notifyIcon_DoubleClick(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            if (mousePointerThread != null)
            {
                mousePointerThread.Abort();
            }
            keyCheckHook_temp.unhook();
            notifyIcon.Dispose();
            myWindow.Close();
            System.Windows.Application.Current.Shutdown();
        }

        private void notifyIcon_BalloonTipShown(object Sender, EventArgs e)
        {
            // Disable mouse detection
            flagMoveEnabled = false;
        }

        private void notifyIcon_BalloonTipClosed(object Sender, EventArgs e)
        {
            // Enable mouse detection
            flagMoveEnabled = true;
        }

        private void notifyIcon_MouseMove(object Sender, EventArgs e)
        {
            // Show Usage info
            if (flagMoveEnabled)
                notifyIcon.ShowBalloonTip(2000, "ScreenOnTimer", "Screen On: " + getDuration(screenTimeTotal_notify), ToolTipIcon.Info);
        }

        public string getDuration(double timein_ms)
        {
            screenTimeTotal_notify = timein_ms;

            TimeSpan t = TimeSpan.FromMilliseconds(timein_ms);
            string answer = "";

            if (timein_ms >= 60 * 60 * 1000) //GE than 1 hour.
            {
                if (timein_ms >= 24 * 60 * 60 * 1000) //GE than 1 Day.
                {
                    answer = string.Format("{0:D2} Days: {1:D2} Hour: {2:D2} Min",
                    t.Days,
                    t.Hours,
                    t.Minutes);
                }
                else
                {
                    answer = string.Format("{0:D2} Hour: {1:D2} Min",
                    t.Hours,
                    t.Minutes);

                }
            }
            else
            {
                answer = string.Format("{0:D2} Min: {1:D2} Sec",
                    t.Minutes,
                    t.Seconds);
            }
            return answer;
        }
    }
}
