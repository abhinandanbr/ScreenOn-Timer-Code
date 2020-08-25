using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Shell;
//using Microsoft.Shell;

namespace ScreenOn_Timer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    /*public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var view = new MainWindow();
            view.Show();
        }
    }*/

    
    public partial class App : Application, ISingleInstanceApp
    {
        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        static bool CheckLibrary()
        {
            return LoadLibrary("System.Xaml.dll") == IntPtr.Zero;
        }

        private const string Unique = "ScreenOn_Timer_Abhi";



        [STAThread]
        public static void Main()
        {

            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                ScreenOn_Timer.App app = new ScreenOn_Timer.App();

                try
                {
                    app.InitializeComponent();
                    
                }
                catch (Exception e)
                {
                    MessageBox.Show("Excep", "Message");
                }

                app.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();


            }
        }
        #region ISingleInstanceApp Members
        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // handle command line arguments of second instance
            // ...
            return true;
        }
        #endregion
    }
}
