using CommonClasses;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace Client
{
    public class ConcreteWorkManager:WorkManager
    {

        private Work Work;

        public override byte[] ExecuteCurrentWork(byte[] workSeed, string path)
        {
            path= Path.Combine(Environment.CurrentDirectory, path);
            using (var stream= new FileStream(path, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    //Formatter.
                }
            }
        }

    }
}