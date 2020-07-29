using System;
using System.Collections.Generic;
using System.Net;
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

        /// <summary>
        ///  Метод, при каждом вызове возвращающий адреса разных серверов
        /// </summary>
        internal abstract IPEndPoint GetServerAdress();

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

    }

    /// <summary>
    ///  Абстрактный класс, выполняющий вычисления
    /// </summary>
    public abstract class WorkManager
    {

        #region Properties

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

        /// <summary>
        ///  Метод, запускающий вычисления
        /// </summary>
        public abstract byte[] ExecuteWork(byte[] workSeed, string path);

        #endregion

    }

    /// <summary>
    ///  Абстрактный класс, преставляющий код вычислений
    /// </summary>
    public abstract class Work
    {

        #region Properties

        public readonly string Name;

        public readonly byte[] WorkCode;

        #endregion

        #region Methods

        public Work(string name, byte[] code)
        {
            Name = name;
            WorkCode = code;
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
        public string delLoadButtonText;

        #endregion

        #region Methods

        public MetaWork(string name, string displayName, string iconSource, string shortDescription,
            string fullDescription)
        {
            this.name = name;
            this.DisplayName = displayName;
            this.ImageSource = new JpegBitmapDecoder(new Uri(iconSource, UriKind.RelativeOrAbsolute),
                BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default).Frames[0];
            this.ShortDescription = shortDescription;
            this.FullDescription = fullDescription;
        }

        #endregion
    }
}