using System.Net;

namespace Client
{

    /// <summary>
    ///  Абстрактный класс, связывающий остальные классы
    /// </summary>
    public abstract class Client
    {

        public RemoteProxy RemoteProxy 
        {
            get
            {return RemoteProxy;} 

            set
            {

                if (RemoteProxy == null)
                {
                    RemoteProxy = value;
                    return;
                }
                throw new System.Exception();

            }
        }

        public WorkManager WorkManager 
        {
            get
            {return WorkManager;} 

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

    }

    /// <summary>
    ///  Абстрактный класс, объединяющий функционал общения с сервером
    /// </summary>
    public abstract class RemoteProxy
    {

        public  Client Client 
        {
            get
            {return Client;} 

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

        public  ProjectEncoding ProjectEncoding 
        {
            get
            {return ProjectEncoding;} 

            set
            {

                if (ProjectEncoding == null)
                {
                    ProjectEncoding = value;
                    return;
                }
                throw new System.Exception();

            }
        }

        public  Connection Connection 
        {
            get
            {return Connection;} 

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

    }

    /// <summary>
    ///  Абстрактный класс, отвечающий за шифрование канала связи
    /// </summary>
    public abstract class ProjectEncoding
    {

        public RemoteProxy RemoteProxy 
        {
            get
            {return RemoteProxy;} 

            set
            {

                if (RemoteProxy == null)
                {
                    RemoteProxy = value;
                    return;
                }
                throw new System.Exception();

            }
        }

    }

    /// <summary>
    ///  Абстрактный класс, отвечающий за передачу данных
    /// </summary>
    public abstract class Connection
    {

        public RemoteProxy RemoteProxy 
        {
            get
            {return RemoteProxy;} 

            set
            {

                if (RemoteProxy == null)
                {
                    RemoteProxy = value;
                    return;
                }
                throw new System.Exception();

            }
        }

        public bool IsOpen{get; private set;}

        public abstract byte[] Receive();

        public abstract void ConnectProxy();

        public abstract void Close();

        public abstract void Send(byte[] message);

    }

    /// <summary>
    ///  Абстрактный класс, выполняющий вычисления
    /// </summary>
    public abstract class WorkManager
    {

        public  Client Client 
        {
            get
            {return Client;} 

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

        public abstract byte[] ExecuteWork(byte[] workSeed, string path);

    }
}