﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        App()
        {
            InitializeComponent();
            //if (Environment.CommandLine.Contains("run"))
            //{
            //    App app = new App();
            //    MainWindow window = new MainWindow();
            //    app.Run(window);
            //}
            //else
            //{
            //    Process.Start("Updater.exe");
            //}
        }

        private void OnStart(object sender, StartupEventArgs e)
        {
            if (!Environment.CommandLine.Contains("run"))
            {
                //Process.Start("Updater.exe");
                //Shutdown();
            }
        }

        //[STAThread]
        //static void Main()
        //{
        //    ClientApp.App app = new ClientApp.App();
        //    app.InitializeComponent();
        //    app.Run();
        //}
    }
}
