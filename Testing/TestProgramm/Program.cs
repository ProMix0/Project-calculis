using System;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TestProgramm
{
    public class Program
    {
        /*
        [Serializable]
        class Example
        {
            public byte[] Field;

            public Example(byte[] field)
            {
                this.Field = field;
            }
        }
        */

        [Serializable]
        public class MyXMLSerializableClass
        {
            public string name;

            public List<MyKVPair> data=new List<MyKVPair>();

            public class MyKVPair
            {
                public string name;

                public bool flag;

                public MyKVPair() { Console.WriteLine("MyKVPair default ctor"); }

                public MyKVPair(string name, bool flag)
                {
                    this.name = name;
                    this.flag = flag;
                    Console.WriteLine("MyKVPair parametrised ctor");
                }
            }

            public void Display()
            {
                Console.WriteLine($"Name: {name}");
                foreach(var item in data)
                {
                    Console.WriteLine($"\tName: {item.name}\n\tState: {item.flag}\n");
                }
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
            Console.Read();
            */


            /*
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
            */


            //Console.WriteLine("Creating");
            //var rsa = new RSACryptoServiceProvider();
            //Console.WriteLine("Export");
            //RSAParameters parameters = rsa.ExportParameters(false);
            //Console.WriteLine("After export");
            //Console.ReadKey();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(MyXMLSerializableClass));

            MyXMLSerializableClass toSerialize = new MyXMLSerializableClass
            {
                name = "Unique name"//,
                //data = new List<MyXMLSerializableClass.MyKVPair>()
            };
            toSerialize.data.Add(new MyXMLSerializableClass.MyKVPair("Param1", true));
            toSerialize.data.Add(new MyXMLSerializableClass.MyKVPair("Param2", false));

            using (FileStream fs = new FileStream("Test.xml", FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, toSerialize);
            }

            MyXMLSerializableClass newPerson;
            using (FileStream fs = new FileStream("Test.xml", FileMode.Open))
            {
                newPerson = (MyXMLSerializableClass)xmlSerializer.Deserialize(fs); //InvalidOperationExeption
            }
            newPerson.Display();
            Console.Read();
        }
    }
}
