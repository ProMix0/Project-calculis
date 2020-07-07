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
    ///  Класс, не шифрующий канал связи
    /// </summary>
    public class NoneProjectCryptography : Connection
    {

        #region Properties

        private bool isOpen;

        private readonly Connection connection;

        #endregion

        #region Methods

        public NoneProjectCryptography(Connection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        ///  Метод, открывающий незашифрованный канал связи
        /// </summary>
        public override void Open()
        {

            this.connection.Open();

        }

        /// <summary>
        ///  Метод, закрывающий незашифрованный канал связи
        /// </summary>
        public override void Close()
        {

            this.connection.Close();

        }

        /// <inheritdoc/>
        public override bool IsOpen()
        {

            return this.isOpen;

        }

        /// <summary>
        ///  Метод, отправляющий сообщение серверу по незашифрованному каналу связи
        /// </summary>
        public override void Send(byte[] message)
        {

            this.connection.Send(message);

        }

        /// <summary>
        ///  Метод, принимающий сообщение серверу по незашифрованному каналу связи
        /// </summary>
        public override byte[] Receive()
        {

            return this.connection.Receive();

        }

        #endregion

    }

    /// <summary>
    ///  Класс, шифрующий канал связи по алгоритму RSA
    /// </summary>
    public class RSAProjectCryptography : Connection
    {

        #region Properties

        private bool isOpen;

        private RSAParameters publicKey;
        private RSAParameters privateKey;

        private KeyGen keyGen;

        private readonly Connection connection;

        #endregion

        #region Methods

        public RSAProjectCryptography(Connection connection)
        {
            
            this.connection = connection;

        }

        /// <inheritdoc/>
        public override void Open()
        {

            // Генерация ключей
            this.privateKey = this.keyGen.GetRSAParameters();

            // Обмен ключами с сервером
            RSAParameters keyToSend = new RSAParameters { Exponent = privateKey.Exponent, Modulus = privateKey.Modulus };
            BinaryFormatter formatter = new BinaryFormatter();
            using Stream stream1 = new MemoryStream();
            formatter.Serialize(stream1, keyToSend);
            using BinaryReader reader = new BinaryReader(stream1);
            this.connection.Send(reader.ReadBytes((int)stream1.Length));

            using Stream stream2 = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(stream2);
            writer.Write(this.connection.Receive());
            this.publicKey = (RSAParameters)formatter.Deserialize(stream2);

            this.isOpen = true;

        }

        /// <inheritdoc/>
        public override void Close()
        {

            this.isOpen = false;

        }

        /// <inheritdoc/>
        public override bool IsOpen()
        {

            return this.isOpen;

        }

        /// <inheritdoc/>
        public override void Send(byte[] message)
        {

            this.connection.Send(Encrypt(message));

        }

        /// <inheritdoc/>
        public override byte[] Receive()
        {

            return Decrypt(this.connection.Receive());

        }

        public RSAProjectCryptography()
        {
            this.keyGen = new KeyGen();
        }

        /// <summary>
        ///  Метод, шифруюший сообщение
        /// </summary>
        private byte[] Encrypt(byte[] message)
        {

            using var rsa = new RSACryptoServiceProvider();

            rsa.ImportParameters(this.publicKey);
            return rsa.Encrypt(message, false);


        }

        /// <summary>
        ///  Метод, дешифрующий сообщение
        /// </summary>
        private byte[] Decrypt(byte[] message)
        {

            using var rsa = new RSACryptoServiceProvider();

            rsa.ImportParameters(this.privateKey);
            return rsa.Decrypt(message, false);


        }

        #endregion

        #region Classes

        /// <summary>
        ///  Класс, отвечающий за асинхронную генерацию ключей
        /// </summary>
        private class KeyGen
        {

            #region Properties

            const int KeysCount = 2;

            private Queue<RSAParameters> Keys;

            #endregion

            #region Methods

            /// <summary>
            ///  Метод, возвращающий ключ
            /// </summary>
            internal RSAParameters GetRSAParameters()
            {
                // Заполнение очереди с ключами до указанного количества
                for (int i = 0; i < KeysCount - this.Keys.Count; i++)
                {
                    this.GenerateKeyToQueue();

                }

                // Ожидание появления ключей в очереди
                while (this.Keys.Count == 0)
                {
                    System.Threading.Thread.Sleep(1000);
                }

                // Извлечение результата
                RSAParameters result;
                lock (this.Keys)
                {
                    result = this.Keys.Dequeue();
                }

                // Генерация ключа в замен использованного
                this.GenerateKeyToQueue();

                return result;

            }

            internal KeyGen()
            {

                this.Keys = new Queue<RSAParameters>();

            }

            /// <summary>
            ///  Метод, генерирующий ключ и добавляющий его в очередь
            /// </summary>
            private void GenerateKeyToQueue()
            {
                // Запуск асинхронной генерации
                Task.Run(() =>
                        {
                            // Генерация
                            using var rsa = new RSACryptoServiceProvider();
                            var result = rsa.ExportParameters(true);

                            // Добавление в очередь
                            lock (this.Keys)
                            {
                                this.Keys.Enqueue(result);
                            }
                        });
            }

            #endregion

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

        /// <inheritdoc/>
        public override bool IsOpen()
        {

            return this.client.Connected;

        }

        /// <summary>
        ///  Метод, при каждом вызове возвращающий адреса разных серверов
        /// </summary>
        private IPEndPoint GetProxyAdress()
        {

            this.adressNumber++;
            if (this.adressNumber >= this.ProxyAdresses.Length)
            {
                this.adressNumber = 0;
            }
            return this.ProxyAdresses[this.adressNumber];

        }

        /// <inheritdoc/>
        public override byte[] Receive()
        {
            return this.reader.ReadBytes((int)this.stream.Length);
        }

        /// <inheritdoc/>
        public override void Open()
        {
            this.client.Connect(this.GetProxyAdress());
            this.stream = client.GetStream();
            this.reader = new BinaryReader(this.stream);
            this.writer = new BinaryWriter(this.stream);
        }

        /// <inheritdoc/>
        public override void Close()
        {
            this.reader.Close();
            this.reader = null;
            this.writer.Close();
            this.writer = null;
            this.stream.Close();
            this.stream = null;
            this.client.Close();
            this.client = null;
        }

        /// <inheritdoc/>
        public override void Send(byte[] message)
        {
            this.writer.Write(message);
            this.writer.Flush();
        }

        /// <inheritdoc/>
        public TCPConnection()
        {

            this.client = new TcpClient();

        }

        #endregion

    }

    /// <inheritdoc/>
    public class ConcreteWorkManager : WorkManager
    {

        #region Properties

        private Work Work;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override byte[] ExecuteWork(byte[] workSeed, string path)
        {
            // Определение пути к файлу с объектом класса Work
            path = Path.Combine(Environment.CurrentDirectory, path);

            // Чтение объекта класса Work в stream
            using var stream = new FileStream(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();

            // Десериализация объекта класса Work из stream
            this.Work = (Work)formatter.Deserialize(stream);

            // Загрузка сборки с кодом вычислений из объекта Work
            Assembly assembly = Assembly.Load(this.Work.WorkCode);

            // Получение метода, выполняющего вычисления
            Type type = assembly.GetType("WorkNamespace.WorkClass", true, true);
            object obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("ExecuteWork");

            // Запуск вычислений и возврат результата
            return (byte[])method.Invoke(obj, new object[] { workSeed });
        }

        #endregion

    }
}