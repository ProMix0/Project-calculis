using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Serialization;

namespace Client
{
    [Serializable]
    public class ConfigurationManager
    {
        public readonly List<Setting> settings = new List<Setting>();

        public ConfigurationManager()
        { }

        public void Add(string name, bool value)
        {
            if (settings.Where(s => s.Name == name).Count() == 0)
            {
                settings.Add(new Setting(name, value));
            }
        }

        public void Save()
        {
            using FileStream fs = new FileStream("Settings.xml", FileMode.Create);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConfigurationManager));
            xmlSerializer.Serialize(fs, this);

            //string str="";
            //foreach (var item in settings)
            //{
            //    str += '\n' + item.ToString();
            //}
            //MessageBox.Show(str);
        }

        public static ConfigurationManager Load()
        {
            try
            {
                using FileStream fs = new FileStream("Settings.xml", FileMode.Open);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConfigurationManager));
                return (ConfigurationManager)xmlSerializer.Deserialize(fs);
            }
            catch
            {
                ConfigurationManager defaultValue = new ConfigurationManager();

                defaultValue.Add("Name", false);

                return defaultValue;
            }
        }

        public class Setting
        {
            public string Name { get; set; }

            public bool Value { set; get; }

            public override string ToString()
            {
                return $"Name: {Name}, value: {Value}";
            }

            public Setting() { }

            public Setting(string name, bool value)
            {
                this.Name = name;
                Value = value;
            }
        }
    }
}
