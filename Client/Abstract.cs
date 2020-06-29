namespace Client
{
    public abstract class Client
    {

        public RemoteProxy RemoteProxy;

        public WorkManager WorkManager;

    }

    public abstract class RemoteProxy
    {

        public readonly Client Client;

        public Security Security;

        public ProjectEncoding ProjectEncoding;

        public Connection Connection;

    }

    public abstract class Security
    {

        public readonly RemoteProxy RemoteProxy;

    }

    public abstract class ProjectEncoding
    {

        public readonly RemoteProxy RemoteProxy;

    }

    public abstract class Connection
    {

        public readonly RemoteProxy RemoteProxy;

        public bool IsOpen{get; private set;}

        public abstract byte[] Receive();

        public abstract void Open(string ip, int port);

        public abstract void Close();

        public abstract void Send(byte[] message);

    }

    public abstract class WorkManager
    {

        public readonly Client Client;

        public abstract byte[] ExecuteWork(byte[] workSeed, string path);

    }
}