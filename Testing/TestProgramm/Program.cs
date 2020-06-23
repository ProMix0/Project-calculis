using System;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TestProgramm
{
    class Program
    {

        [Serializable]
        class Example
        {
            public byte[] Field;

            public Example(byte[] field)
            {
                this.Field = field;
            }
        }

        static void Main(string[] args)
        {/*
            Assembly asm;

            using (BinaryReader reader = new BinaryReader(File.Open(@"C:\Users\Миша\source\repos\Project-calculis\TestLibrary\bin\Debug\netcoreapp3.1\TestLibrary.dll", FileMode.Open)))
            {
                asm = Assembly.Load(reader.ReadBytes((int)reader.BaseStream.Length));
            }

            

            Type type = asm.GetType("TestLibrary.TestClass", true, true);

            // создаем экземпляр класса Program
            object obj = Activator.CreateInstance(type);

            // получаем метод GetResult
            MethodInfo method = type.GetMethod("X2");

            // вызываем метод, передаем ему значения для параметров и получаем результат
            object result = method.Invoke(obj, new object[] { 2 });
            Console.WriteLine((result));
            Console.Read();*/

            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            Example example1 = new Example(new byte[2]);
            formatter.Serialize(stream, example1);
            example1=null;
            GC.Collect();
            stream.Position = 0;
            Console.WriteLine(stream.Length);
            Example example2 = (Example)formatter.Deserialize(stream);
            Console.WriteLine(example2.Field);
            Console.ReadKey();
        }
    }
}
