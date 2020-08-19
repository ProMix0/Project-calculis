using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Updater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        App(){}

        [STAThread]
        static void Main()
        {
            //TODO Receiving updates


            // Run client app
            Process.Start("Client.exe", "run");
        }
    }
}
