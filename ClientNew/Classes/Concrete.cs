using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ClientApp
{

    /// <summary>
    ///  Класс, связывающий остальные классы и отвечающий за взаимодействие с пользователем
    /// </summary>
    public class ConcreteClient : Client
    {

        #region Properties

        private readonly List<IPEndPoint> adresses = new List<IPEndPoint>();
        private int adressNumber;

        #endregion

        #region Methods

        /// <inheritdoc/>
        internal override IPEndPoint GetServerAdress()
        {
            adressNumber++;
            if (adressNumber >= adresses.Count)
            {
                adressNumber = 0;
            }
            return adresses[adressNumber];
        }

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

        /// <inheritdoc/>
        public override void Open()
        {

            this.connection.Open();
            isOpen = true;

        }

        /// <inheritdoc/>
        public override void Close()
        {

            this.connection.Close();
            isOpen = false;

        }

        /// <inheritdoc/>
        public override bool IsOpen()
        {

            return this.isOpen;

        }

        /// <inheritdoc/>
        public override void Send(byte[] message)
        {

            if (!isOpen) throw new Exception("Channel is close");
            this.connection.Send(message);

        }

        /// <inheritdoc/>
        public override byte[] Receive()
        {

            if (!isOpen) throw new Exception("Channel is close");
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

        private readonly KeyGen keyGen;

        private readonly Connection connection;

        #endregion

        #region Methods

        public RSAProjectCryptography(Connection connection)
        {
            
            this.connection = connection;
            keyGen = new KeyGen();

        }

        /// <inheritdoc/>
        public override void Open()
        {

            // Открытие дочернего канала
            connection.Open();

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

            connection.Close();
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

            if (!isOpen) throw new Exception("Channel is close");
            this.connection.Send(Encrypt(message));

        }

        /// <inheritdoc/>
        public override byte[] Receive()
        {

            if (!isOpen) throw new Exception("Channel is close");
            return Decrypt(this.connection.Receive());

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

            private readonly int keysCount;

            private readonly Queue<RSAParameters> keys = new Queue<RSAParameters>();

            #endregion

            #region Methods

            /// <summary>
            ///  Метод, возвращающий ключ
            /// </summary>
            internal RSAParameters GetRSAParameters()
            {
                // Заполнение очереди с ключами до указанного количества +1
                for (int i = 0; i <= keysCount - keys.Count; i++)
                {
                    this.GenerateKeyToQueue();

                }

                // Ожидание появления ключей в очереди
                while (keys.Count == 0)
                {
                    System.Threading.Thread.Sleep(100);
                }

                // Извлечение результата
                RSAParameters result;
                lock (keys)
                {
                    result = keys.Dequeue();
                }

                return result;

            }

            internal KeyGen(int keysCount)
            {
                this.keysCount = keysCount;
            }

            internal KeyGen()
                :this(2)
            {

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
                            lock (keys)
                            {
                                keys.Enqueue(result);
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

        private readonly List<IPEndPoint> proxyAdresses = new List<IPEndPoint>();

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

            return client.Connected;

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

        /// <summary>
        ///  Метод, при каждом вызове возвращающий адреса разных серверов
        /// </summary>
        private IPEndPoint GetProxyAdress()
        {

            adressNumber++;
            if (adressNumber >= proxyAdresses.Count)
            {
                adressNumber = 0;
            }
            return proxyAdresses[adressNumber];

        }

        #endregion

    }


    /// <summary>
    ///  Класс, выполняющий вычисления
    /// </summary>
    public class ConcreteWorkManager : WorkManager
    {

        #region Properties

        private Work work;

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
            this.work = (Work)formatter.Deserialize(stream);

            // Загрузка сборки с кодом вычислений из объекта Work
            Assembly assembly = Assembly.Load(this.work.WorkCode);

            // Получение метода, выполняющего вычисления
            Type type = assembly.GetType("WorkNamespace.WorkClass", true, true);
            object obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("ExecuteWork");

            // Запуск вычислений и возврат результата
            return (byte[])method.Invoke(obj, new object[] { workSeed });
        }

        #endregion

    }

    /// <summary>
    ///  Класс, преставляющий код вычислений
    /// </summary>
    public class ConcreteWork:Work
    {

        #region Properties



        #endregion

        #region Methods

        public ConcreteWork(string name, byte[] code)
            :base(name, code)
        { }

        #endregion
    }

    public class ConcreteMetaWork : MetaWork
    {

        #region Properties



        #endregion

        #region Methods

        public ConcreteMetaWork(string name, string displayName, string iconSource, string shortDescription,
            string fullDescription, int pay)
            : base(name, displayName, iconSource, shortDescription, fullDescription, pay)
        {

        }

        #endregion

    }
}