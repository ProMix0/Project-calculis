using CommonClasses;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Net.Sockets;

namespace Client
{

    public class TCPConnection:Connection
    {

        private TcpClient client;
        private Stream stream;
        private BinaryReader reader;
        private BinaryWriter writer;

        public override byte[] Receive()
        {
            byte[] result = new byte[0];
            byte[] buffer = new byte[256];
            int readed;
            int index = 0;
            while ((readed = this.reader.Read(buffer, index, buffer.Length)) == 256)
            {
                index+=buffer.Length;
                Array.Resize(ref result, result.Length + buffer.Length);
                for(int i = 0;i<buffer.Length;i++)
                {
                    result[i+result.Length-buffer.Length] = buffer[i];
                }
            }
            return result;
        }
        
        public override void Open(string ip, int port)
        {//TODO Connection to some count of proxy
            this.client = new TcpClient(ip, port);
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