using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Client client = new ConcreteClient();

            client.WorkManager = new ConcreteWorkManager();
            client.WorkManager.Client = client;

            client.RemoteProxy = new ConcreteRemoteProxy();
            client.RemoteProxy.Client = client;

            client.RemoteProxy.Connection = new TCPConnection();
            client.RemoteProxy.Connection.RemoteProxy = client.RemoteProxy;

            client.RemoteProxy.ProjectCryptography = new RSAProjectCryptography();
            client.RemoteProxy.ProjectCryptography.RemoteProxy = client.RemoteProxy;

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
