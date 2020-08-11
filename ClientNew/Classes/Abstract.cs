using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClientApp
{

    /// <summary>
    ///  Абстрактный класс, связывающий остальные классы и отвечающий за взаимодействие с пользователем
    /// </summary>
    public abstract class Client
    {

        #region Properties

        public WorkManager WorkManager
        {
            get
            { return WorkManager; }

            set
            {

                if (WorkManager == null)
                {
                    WorkManager = value;
                    return;
                }
                throw new System.Exception();

            }
        }

        public Connection Connection
        {
            get
            { return Connection; }

            set
            {

                if (Connection == null)
                {
                    Connection = value;
                    return;
                }
                throw new System.Exception();

            }
        }

        #endregion

        #region Methods

        public Client(WorkManager manager)
        {
            WorkManager = manager;
        }

        #endregion

    }

    /// <summary>
    ///  Абстрактный класс, отвечающий за передачу данных
    /// </summary>
    public abstract class Connection
    {

        #region Properties

        

        #endregion

        #region Methods

        /// <summary>
        ///  Метод, возвращающий состояние соединения
        /// </summary>
        public abstract bool IsOpen();

        /// <summary>
        ///  Метод, возвращающий ответ сервера
        /// </summary>
        public abstract byte[] Receive();

        /// <summary>
        ///  Метод, устанавливающий соединение с сервером
        /// </summary>
        public abstract void Open();

        /// <summary>
        ///  Метод, закрывающий соединение с сервером
        /// </summary>
        public abstract void Close();

        /// <summary>
        ///  Метод, отправляющий сообщение серверу
        /// </summary>
        public abstract void Send(byte[] message);

        #endregion

        #region Classes

        public abstract class ConnectionFactory
        {

            #region Properties

            public static ConnectionRole Role { get; protected set; }

            #endregion

            #region Methods

            public abstract Connection Build(Connection connection);

            #endregion

            #region Classes

            public enum ConnectionRole
            {
                Cryptography,
                Transfer
            }

            #endregion

        }

        #endregion

    }

    /// <summary>
    ///  Абстрактный класс, выполняющий вычисления
    /// </summary>
    public abstract class WorkManager
    {

        #region Properties

        private Dictionary<string, Work> works = new Dictionary<string, Work>();
        public Client Client
        {
            get
            { return Client; }

            set
            {

                if (Client == null)
                {
                    Client = value;
                    return;
                }
                throw new System.Exception();

            }
        }

        #endregion

        #region Methods

        public WorkManager()
        {
            foreach (var path in Directory.EnumerateFiles("/Data/Works", "*.work"))
            {
                using var stream = new FileStream(path, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                if (formatter.Deserialize(stream) is Work work)
                {
                    works.Add(work.Name, work);
                }
            }
        }

        /// <summary>
        ///  Метод, запускающий вычисления
        /// </summary>
        public abstract byte[] ExecuteWork(byte[] workSeed, string path);

        #endregion

    }

    /// <summary>
    ///  Абстрактный класс, преставляющий код вычислений
    /// </summary>
    [Serializable]
    public abstract class Work
    {

        #region Properties

        public readonly string Name;
        public readonly MetaWork MetaWork;
        public readonly Func<byte[], byte[]> Run;

        #endregion

        #region Methods

        public Work(string name, Func<byte[], byte[]> code, MetaWork metaWork)
        {
            Name = name;
            Run = code;
            MetaWork = metaWork;
        }

        #endregion

    }

    [Serializable]
    public abstract class MetaWork
    {

        #region Properties

        public readonly string name;
        public string DisplayName { get; }
        public BitmapFrame ImageSource { get; }
        public string ShortDescription { get; }
        public string FullDescription { get; }
        public int Pay { get; }

        #endregion

        #region Methods

        public MetaWork(string name, string displayName, string iconSource, string shortDescription,
            string fullDescription, int pay)
        {
            this.name = name;
            DisplayName = displayName;
            ImageSource = new JpegBitmapDecoder(new Uri(iconSource, UriKind.RelativeOrAbsolute),
                BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default).Frames[0];
            ShortDescription = shortDescription;
            FullDescription = fullDescription;
            Pay = pay;
        }

        #endregion
    }
}