using CommonClasses;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Net.Sockets;
using System.Net;

namespace Client
{

    public class ConcreteClient:Client
    {
        
        public ConcreteClient(RemoteProxy proxy, WorkManager workManager, ProtocolController protocolController)
            :base(proxy, workManager, protocolController)
            {}

    }

    public class TCPRemoteProxy:RemoteProxy
    {

        public TCPRemoteProxy(ProjectEncoding projectEncoding, Connection connection)
            :base(projectEncoding, connection)
            {}

    }

    public class ConcreteProtocolController:ProtocolController
    {

    }

    public class RSAProjectEncoding:ProjectEncoding
    {

        

    }

    public class TCPConnection:Connection
    {

        private IPEndPoint[] ProxyAdresses;

        private int adressNumber;
        private TcpClient client;
        private Stream stream;
        private BinaryReader reader;
        private BinaryWriter writer;

        private IPEndPoint GetProxyAdress()
        {

            this.adressNumber++;
            if (this.adressNumber >= this.ProxyAdresses.Length)
            {
                this.adressNumber = 0;
            }
            return this.ProxyAdresses[this.adressNumber];

        }

        public override byte[] Receive()
        {
            return this.reader.ReadBytes((int)this.stream.Length);
        }
        
        public override void ConnectProxy()
        {
            this.client.Connect(this.GetProxyAdress());
            this.stream = client.GetStream();
            this.reader = new BinaryReader(this.stream);
            this.writer = new BinaryWriter(this.stream);
        }

        public override void Close()
        {
            this.client.Close();
            this.client = null;
            this.stream.Close();
            this.stream = null;
            this.reader.Close();
            this.reader = null;
            this.writer.Close();
            this.writer = null;
        }

        public override void Send(byte[] message)
        {
            this.writer.Write(message);
            this.writer.Flush();
        }

        public TCPConnection()
        {

            this.client = new TcpClient();

        }

    }

    public class ConcreteWorkManager:WorkManager
    {

        private Work Work;

        public override byte[] ExecuteWork(byte[] workSeed, string path)
        {
            path= Path.Combine(Environment.CurrentDirectory, path);
            using (var stream= new FileStream(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                this.Work = (Work)formatter.Deserialize(stream);
            }
            Assembly assembly = Assembly.Load(this.Work.WorkCode);
            Type type = assembly.GetType("WorkNamespace.WorkClass", true, true);
            object obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("ExecuteWork");
            return (byte[])method.Invoke(obj, new object[]{workSeed});
        }

    }
}