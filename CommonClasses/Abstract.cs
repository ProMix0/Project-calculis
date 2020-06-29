using System;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CommonClasses
{

    [Serializable]
    public abstract class Work
    {

        public readonly string WorkID;

        public readonly byte[] WorkCode;

    }
}