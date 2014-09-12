using System;
using System.Threading;

namespace MMMCommon
{
    public delegate void EstadoConexionCambiaDelegate();

    public class InternetConnectionManager
    {
        /// <summary>
        /// Metodo que setea isConnected a traves de un thread independiente.
        /// </summary>
        /// <returns>True si hay conexión, false si no</returns>
        public static void setConnected()
        {
            Thread th = new Thread(new ThreadStart(buscarConexion));
            th.IsBackground = true;
            th.Start();
        }

        private static void buscarConexion()
        {
            System.Uri Url = urlPing;

            System.Net.WebRequest WebReq;
            System.Net.WebResponse Resp;
            WebReq = System.Net.WebRequest.Create(Url);

            try
            {
                Resp = WebReq.GetResponse();
                Resp.Close();
                WebReq = null;
                isConnected = true;
            }

            catch
            {
                WebReq = null;
                isConnected = false;
            }

            EstadoConexionCambia();
        }

        /// <summary>
        /// True si hay conexion a internet, false si no hay
        /// </summary>
        public static bool isConnected
        {
            get
            {
                return isconnected;
            }
            set
            {
                isconnected = value;
            }
        }

        private static bool isconnected = false;

        public static event EstadoConexionCambiaDelegate EstadoConexionCambia;

        public static Uri urlPing
        {
            get
            {
                return urlping;
            }
            set
            {
                urlping = value;
            }
        }

        private static Uri urlping = new System.Uri("http://www.eduinnova.com");
    }
}
