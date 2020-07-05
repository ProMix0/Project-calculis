using CommonClasses;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Client
{

    /// <inheritdoc/>
    public class ConcreteClient : Client
    {

        #region Properties

        #endregion

        #region Methods

        #endregion

    }

    /// <summary>
    ///  Класс, объединяющий функционал общения с сервером
    /// </summary>
    public class ConcreteRemoteProxy : RemoteProxy
    {

        #region Properties

        #endregion

        #region Methods

        #endregion

    }

    /// <summary>
    ///  Класс, не шифрующий канал связи
    /// </summary>
    public class NoneProjectCryptography : ProjectCryptography
    {

        #region Properties

        private bool isOpen;

        #endregion

        #region Methods

        public override void Open()
        {

            this.RemoteProxy.Connection.Open();

        }

        public override void Close()
        {

            this.RemoteProxy.Connection.Close();

        }

        public override bool IsOpen()
        {

            return this.isOpen;

        }

        public override void Send(byte[] message)
        {

            this.RemoteProxy.Connection.Send(message);

        }

        public override byte[] Receive()
        {

            return this.RemoteProxy.Connection.Receive();

        }

        #endregion

    }

    /// <summary>
    ///  Класс, шифрующий канал связи по алгоритму RSA
    /// </summary>
    public class RSAProjectCryptography : ProjectCryptography
    {

        #region Properties

        private bool isOpen;

        private RSACryptoServiceProvider rsa;

        private KeyGenClass KeyGen;

        #endregion

        #region Methods

        public override void Open()
        {

            rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(this.KeyGen.GetRSAParameters());



            this.isOpen = true;

        }

        public override void Close()
        {

            this.rsa.Dispose();
            this.isOpen = false;

        }

        public override bool IsOpen()
        {

            return this.isOpen;

        }

        public override void Send(byte[] message)
        {

            this.RemoteProxy.Connection.Send(Encrypt(message));

        }

        public override byte[] Receive()
        {

            return Decrypt(this.RemoteProxy.Connection.Receive());

        }

        private byte[] Encrypt(byte[] message)
        {



        }

        private byte[] Decrypt(byte[] message)
        {



        }

        public RSAProjectCryptography()
        {
            this.KeyGen = new KeyGenClass();
        }

        #endregion

        #region Classes

        private class KeyGenClass
        {

            const int KeysCount = 2;

            private Queue<RSAParameters> Keys;

            internal RSAParameters GetRSAParameters()
            {
                if (this.Keys.Count < KeysCount)
                {
                    for (int i = 0; i < KeysCount - this.Keys.Count; i++)
                    {
                        Task.Run(() =>
                        {
                            var rsa = new RSACryptoServiceProvider();
                            var result = rsa.ExportParameters(true);
                            lock (this.Keys)
                            {
                                this.Keys.Enqueue(result);
                            }
                            rsa.Dispose();
                        });
                    }
                }

                while (this.Keys.Count == 0)
                {
                    System.Threading.Thread.Sleep(1000);
                }
                return this.Keys.Dequeue();

            }

            internal KeyGenClass()
            {

                this.Keys = new Queue<RSAParameters>();

            }

        }

        #endregion

    }

    /// <summary>
    ///  Класс, отвечающий за передачу данных с помощью протокола TCP
    /// </summary>
    public class TCPConnection : Connection
    {

        #region Properties

        private IPEndPoint[] ProxyAdresses;

        private int adressNumber;
        private TcpClient client;
        private Stream stream;
        private BinaryReader reader;
        private BinaryWriter writer;

        #endregion

        #region Methods

        public override bool IsOpen()
        {

            return this.client.Connected;

        }

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

        public override void Open()
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

        #endregion

    }

    /// <summary>
    ///  Класс, выполняющий вычисления
    /// </summary>
    public class ConcreteWorkManager : WorkManager
    {

        #region Properties

        private Work Work;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override byte[] ExecuteWork(byte[] workSeed, string path)
        {
            path = Path.Combine(Environment.CurrentDirectory, path);
            using (var stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                this.Work = (Work)formatter.Deserialize(stream);
            }
            Assembly assembly = Assembly.Load(this.Work.WorkCode);
            Type type = assembly.GetType("WorkNamespace.WorkClass", true, true);
            object obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("ExecuteWork");
            return (byte[])method.Invoke(obj, new object[] { workSeed });
        }

        #endregion

    }
}