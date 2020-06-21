using System;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;

namespace TestProgramm
{
    class Program
    {

        [Serializable]
        class Example
        {
            public string Field;
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

            Formatter formatter = new Formatter();
            Stream stream = new Stream();
            Example example1 = new Example("ghghh");
            formatter.Serialize(stream, example1);
            Example example2 = (Example)formatter.Deserialize(stream);
            Console.WriteLine(example2.Field);
            Console.ReadKey();
        }
    }
}
