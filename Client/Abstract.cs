namespace Client
{
    public abstract class Client
    {

        public readonly RemoteProxy RemoteProxy;

        public readonly WorkManager WorkManager;

    }

    public abstract class RemoteProxy
    {

        public readonly Client Client;

        public readonly Security Security;

        public readonly ProjectEncoding ProjectEncoding;

        public readonly Connection Connection;

    }

    public abstract class Security
    {

        public readonly Client Client;

        public readonly RemoteProxy RemoteProxy;

    }

    public abstract class ProjectEncoding
    {

        public readonly Client Client;

        public readonly RemoteProxy RemoteProxy;

    }

    public abstract class Connection
    {

        public readonly Client Client;

        public readonly RemoteProxy RemoteProxy;

        public bool IsOpen{get; private set;}

        public byte[] Received {get; private set;}

        public abstract bool Open(string ip, int port);

        public abstract void Close();

    }

    public abstract class WorkManager
    {

        public readonly Client Client;

        public abstract byte[] ExecuteCurrentWork(byte[] workSeed, string path);

    }
}