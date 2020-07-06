using System;

namespace Client
{

    /// <summary>
    ///  Абстрактный класс, связывающий остальные классы и отвечающий за протоколы
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

        public ProjectCryptography ProjectCryptography
        {
            get
            { return ProjectCryptography; }

            set
            {

                if (ProjectCryptography == null)
                {
                    ProjectCryptography = value;
                    return;
                }
                throw new System.Exception();

            }
        }

        #endregion

        #region Methods

        #endregion

    }


    /// <summary>
    ///  Абстрактный класс, отвечающий за шифрование канала связи
    /// </summary>
    public abstract class ProjectCryptography
    {

        #region Properties

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
        ///  Метод, возвращающий состояние соединения
        /// </summary>
        public abstract bool IsOpen();

        /// <summary>
        ///  Метод, открывающий зашифрованный канал связи
        /// </summary>
        public abstract void Open();

        /// <summary>
        ///  Метод, закрывающий зашифрованный канал связи
        /// </summary>
        public abstract void Close();

        /// <summary>
        ///  Метод, отправляющий сообщение
        /// </summary>
        public abstract void Send(byte[] message);

        /// <summary>
        ///  Метод, принимающий сообщение
        /// </summary>
        public abstract byte[] Receive();

        #endregion

    }

    /// <summary>
    ///  Абстрактный класс, отвечающий за передачу данных
    /// </summary>
    public abstract class Connection
    {

        #region Properties

        public ProjectCryptography ProjectCryptography
        {
            get
            { return ProjectCryptography; }

            set
            {

                if (ProjectCryptography == null)
                {
                    ProjectCryptography = value;
                    return;
                }
                throw new System.Exception();

            }
        }

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
}