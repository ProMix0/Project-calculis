using CommonClasses;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace Client
{
    public class ConcreteWorkManager:WorkManager
    {

        private Work Work;

        public override byte[] ExecuteWork(byte[] workSeed, string path)
        {
            path= Path.Combine(Environment.CurrentDirectory, path);
            using (var stream= new FileStream(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                this.Work = (Work)formatter.Deserialize(stream);
            }
            Assembly assembly = Assembly.Load(this.Work.WorkCode);
            Type type = assembly.GetType("WorkNamespace.WorkClass", true, true);
            object obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("ExecuteWork");
            return (byte[])method.Invoke(obj, new object[]{workSeed});
        }

    }
}