using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Utilities;

namespace ScreenOn_Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private float taskBarHeight = Screen.PrimaryScreen.WorkingArea.Height;

        private double screenTimeTotal = 0;
        private DateTime startTime;

        int iMove,fMove = 0;

        private Thread mousePointerThread;

        TextBlock timerTextUI;
        Window mainWindowUI;

        globalKeyboardHook keyCheckHook;
        init_Common n_init;

        public MainWindow()
        {
            InitializeComponent();

            //Init UI, notify Icon, Keyboard
            n_init = new init_Common(this);
            initAll();
        }

        private void initAll()
        {
            n_init.init_window(this, taskBarHeight);

            //Init Hook
            init_Hook();

            //Set Autostart
            n_init.init_setAutoStartup();

            //Init Thread and run
            mousePointerThread = new Thread(keyChk_thread);
            mousePointerThread.SetApartmentState(ApartmentState.STA);
            mousePointerThread.Start();

            //Initialize time
            startTime = DateTime.Now;
            timerTextUI = timerTxtDisplayUI;
            mainWindowUI = mainTimerWindowUI;

            //Setup notification Tray Icon
            n_init.init_notifyTray(mousePointerThread,keyCheckHook);

        }

        private void init_Hook()
        {
            //Keyboard hook
            //List<Keys> keyList = new List<Keys>();
            //keyList.
            keyCheckHook = new globalKeyboardHook();
            keyCheckHook.HookedKeys.Add(Keys.A);
            foreach (Keys keys in Enum.GetValues(typeof(Keys)))
            {
                if (keys != 0 )
                    /*&&
                    !(keys != Keys.Control || keys != Keys.LControlKey || keys != Keys.RControlKey || keys != Keys.ControlKey
                    || keys != Keys.LWin || keys != Keys.RWin || keys != Keys.RShiftKey || keys != Keys.Shift || keys != Keys.ShiftKey
                    || keys != Keys.LShiftKey || keys != Keys.Alt))*/
                {
                    keyCheckHook.HookedKeys.Add(keys);
                }
            }
            keyCheckHook.KeyUp += new System.Windows.Forms.KeyEventHandler(keyCheck_Up);


            //Mouse hook
        }

        private void keyCheck_Up(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //Console.Write("KeyUp: " +"\n");
            screenTimeTotal += ((TimeSpan)(DateTime.Now - startTime)).TotalMilliseconds;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                timerTextUI.Text = n_init.getDuration(screenTimeTotal);
                if (screenTimeTotal >= 60 * 60 * 1000)
                {
                    if (screenTimeTotal > 3 * 60 * 60 * 1000)
                    {
                        mainWindowUI.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0, 0));
                    }
                    else
                        mainWindowUI.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xF6, 0xA0, 0));
                }
            }));

            //keyCheckHook.unhook();
            //e.Handled = true;
            startTime = DateTime.Now;
        }

        private void Window_PreviewLostKeyboardFocus(object sender, EventArgs e)
        {
            var window = (Window)sender;
            window.Topmost = true;
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Point pnt = e.GetPosition();
            screenTimeTotal+= ((TimeSpan)(DateTime.Now - startTime)).TotalMilliseconds;
            timerTextUI.Text = n_init.getDuration(screenTimeTotal);
            startTime = DateTime.Now;
        }

        private void pollMouse()
        {
            fMove = System.Windows.Forms.Control.MousePosition.X;

            if (fMove != iMove)
            {
                screenTimeTotal += ((TimeSpan)(DateTime.Now - startTime)).TotalMilliseconds;
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    timerTextUI.Text = n_init.getDuration(screenTimeTotal);
                    if (screenTimeTotal >= 60 * 60 * 1000)
                    {
                        if (screenTimeTotal > 3 * 60 * 60 * 1000)
                        {
                            mainWindowUI.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0, 0));
                        }
                        else
                            mainWindowUI.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xF6, 0xA0, 0));
                    }
                }));

                //Console.Write("MouseX: " + fMove + "\n");
                iMove = fMove;
            }

            startTime = DateTime.Now;
        }

        private void keyChk_thread()
        {
            iMove = System.Windows.Forms.Control.MousePosition.X;
            while (true)
            {
                pollMouse();
                Thread.Sleep(TimeSpan.FromSeconds(20));
            }
        }
    }
}
