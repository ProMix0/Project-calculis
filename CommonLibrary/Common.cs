using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Media.Imaging;

namespace CommonLibrary
{

    /// <summary>
    ///  Класс, выполняющий вычисления
    /// </summary>
    public class WorkManager
    {

        #region Properties

        public static List<MetaWork> MetaWorks { get; private set; }
        private readonly Dictionary<string, Work> works = new Dictionary<string, Work>();
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
                throw new Exception();

            }
        }

        #endregion

        #region Methods

        public WorkManager()
        {
            //Connection = new RSAProjectCryptography(new TCPConnection()); //TODO random creating connection chain

            foreach (var path in Directory.EnumerateFiles("/Data/Works", "*.work"))
            {
                using var worksStream = new FileStream(path, FileMode.Open);
                BinaryFormatter worksFormatter = new BinaryFormatter();
                if (worksFormatter.Deserialize(worksStream) is Work work)
                {
                    works.Add(work.Name, work);
                }
            }

            Connection.Open();
            Connection.Send(Encoding.UTF8.GetBytes("Get metadata"));
            byte[] answer = Connection.Receive();
            using Stream stream = new MemoryStream();
            stream.Write(answer);
            BinaryFormatter formatter = new BinaryFormatter();
            MetaWorks = ((SerializableList<MetaWork>)formatter.Deserialize(stream)).list;
            Connection.Close();
        }

        /// <summary>
        ///  Метод, запускающий вычисления
        /// </summary>
        public void ExecuteWork(string workName)
        {
            try
            {
                Connection.Open();
                Connection.Send(Encoding.UTF8.GetBytes($"Get seed{workName}"));
                byte[] seed = Connection.Receive();
                Connection.Close();
                byte[] answer = works[workName].Run(seed);
                Connection.Open();
                Connection.Send(answer);
                Connection.Close();
            }
            finally
            {
                if (Connection.IsOpen())
                    Connection.Close();
            }
        }

        public void DownloadWork(string workName)
        {
            Connection.Open();
            Connection.Send(Encoding.UTF8.GetBytes($"Get work{workName}"));
            using BinaryWriter writer = new BinaryWriter(File.Open(Path.Combine(Directory.GetCurrentDirectory(),
                $"/Data/Works/{workName}.work"), FileMode.OpenOrCreate));
            writer.Write(Connection.Receive());
            writer.Flush();
        }

        #endregion

        #region Classes

        [Serializable]
        public class SerializableList<T>
        {
            public readonly List<T> list;
            public SerializableList(List<T> list)
            {
                this.list = list;
            }
        }

        #endregion

    }

    /// <summary>
    ///  Класс, преставляющий код вычислений
    /// </summary>
    [Serializable]
    public class Work
    {

        #region Properties

        public string Name 
        {
            get => name; 
            set
            {
                if (name!=null)
                    if (value.ToCharArray().All(c=>Char.IsLetter(c)))
                    {
                        name = value;
                    }
            }
        }
        public readonly MetaWork MetaWork;
        public readonly Func<byte[], byte[]> Run;
        private string name;

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
    public class MetaWork
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
