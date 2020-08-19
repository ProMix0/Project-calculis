using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Media.Imaging;

namespace CommonLibrary
{

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

}
