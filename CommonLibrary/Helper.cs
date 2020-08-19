using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CommonLibrary
{

    public static class HelperMethods
    {

        #region Properties

        private static readonly List<IPEndPoint> adresses = new List<IPEndPoint>();
        private static int adressNumber;

        #endregion

        #region Methods

        /// <summary>
        ///  Метод, при каждом вызове возвращающий адреса разных серверов
        /// </summary>
        public static IPEndPoint GetServerAdress()
        {
            adressNumber++;
            if (adressNumber >= adresses.Count)
            {
                adressNumber = 0;
            }
            return adresses[adressNumber];
        }

        #endregion

    }

}
