using System.Net;

namespace Client
{
    public abstract class Client
    {

        public RemoteProxy RemoteProxy;

        public WorkManager WorkManager;

        public ProtocolController ProtocolController;

        public Client(RemoteProxy proxy, WorkManager workManager, ProtocolController protocolController)
        {

            this.RemoteProxy = proxy;
            this.RemoteProxy.Client = this;
            this.WorkManager = workManager;
            this.WorkManager.Client = this;
            this.ProtocolController = protocolController;
            this.ProtocolController.Client = this;

        }

    }

    public abstract class RemoteProxy
    {

        public Client Client;

        public ProjectEncoding ProjectEncoding;

        public Connection Connection;

        public RemoteProxy(ProjectEncoding projectEncoding, Connection connection)
        {

            this.ProjectEncoding = projectEncoding;
            this.ProjectEncoding.RemoteProxy = this;
            this.Connection = connection;
            this.Connection.RemoteProxy = this;

        }

    }

    public abstract class ProtocolController
    {

        public Client Client;

    }

    public abstract class ProjectEncoding
    {

        public RemoteProxy RemoteProxy;

    }

    public abstract class Connection
    {

        public RemoteProxy RemoteProxy;

        public bool IsOpen{get; private set;}

        public abstract byte[] Receive();

        public abstract void ConnectProxy();

        public abstract void Close();

        public abstract void Send(byte[] message);

    }

    public abstract class WorkManager
    {

        public Client Client;

        public abstract byte[] ExecuteWork(byte[] workSeed, string path);

    }
}