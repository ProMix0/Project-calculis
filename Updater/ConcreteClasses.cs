using CommonLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Updater
{
    class ConcreteClasses
    {
        /// <summary>
        ///  Класс, отвечающий за передачу данных с помощью протокола TCP
        /// </summary>
        public class TCPConnection : Connection
        {

            #region Properties

            private TcpClient client;
            private Stream stream;
            private BinaryReader reader;
            private BinaryWriter writer;

            #endregion

            #region Methods

            /// <inheritdoc/>
            public override bool IsOpen()
            {
                return client.Connected;
            }

            /// <inheritdoc/>
            public override byte[] Receive()
            {
                return reader.ReadBytes((int)stream.Length);
            }

            /// <inheritdoc/>
            public override void Open()
            {
                client.Connect(HelperMethods.GetServerAdress());
                stream = client.GetStream();
                reader = new BinaryReader(stream);
                writer = new BinaryWriter(stream);
            }

            /// <inheritdoc/>
            public override void Close()
            {
                reader.Close();
                reader = null;
                writer.Close();
                writer = null;
                stream.Close();
                stream = null;
                client.Close();
                client = null;
            }

            /// <inheritdoc/>
            public override void Send(byte[] message)
            {
                writer.Write(message);
                writer.Flush();
            }

            /// <inheritdoc/>
            public TCPConnection()
            {

                client = new TcpClient();

            }

            #endregion

            #region Classes

            public class TCPConnectionFactory : ConnectionFactory
            {

                #region Methods

                static TCPConnectionFactory()
                {
                    Role = ConnectionRole.Transfer;
                }

                public override Connection Build(Connection connection)
                {
                    return new TCPConnection();
                }

                #endregion

            }

            #endregion

        }
    }
}
