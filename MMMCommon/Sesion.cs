using System;
using System.Xml.Serialization;

namespace MMMCommon
{
    [Serializable]
    public class Sesion
    {
        #region Private Members

        protected int numerosesion = 1;
        protected int sesionpredecesora = -1;
        protected DateTime fechainiciosesion;
        protected DateTime fechafinsesion;
        protected bool guardada = false;
        int sincronizada = -1;

        #endregion

        #region Public Members
        /// <summary>
        /// Fecha de la sesión
        /// </summary>
        [XmlAttribute("Inicio")]
        public DateTime FechaInicioSesion
        {
            get
            {
                return fechainiciosesion;
            }
            set
            {
                fechainiciosesion = value;
            }
        }

        /// <summary>
        /// Numero de la sesión
        /// </summary>
        [XmlAttribute("Num")]
        public int NumeroSesion
        {
            get
            {
                return numerosesion;
            }
            set
            {
                numerosesion = value;
            }
        }

        /// <summary>
        /// Sesión predecesora a esta. -1 si no tiene predecesora
        /// </summary>
        [XmlAttribute("Pred")]
        public int SesionPredecesora
        {
            get
            {
                return sesionpredecesora;
            }
            set
            {
                sesionpredecesora = value;
            }
        }

        /// <summary>
        /// Verdadero si la sesión está guardada en el xml del curso. Falso en otro caso.
        /// </summary>
        [XmlIgnore]
        public bool Guardada
        {
            get
            {
                return guardada;
            }
            set
            {
                guardada = value;
            }
        }

        /// <summary>
        /// Int que representa si la sesión fue sincronizada o no. 
        /// -1 no sincronizada, 0 sincronizada con éxito
        /// </summary>
        [XmlAttribute("sincronizada")]
        public int Sincronizada
        {
            get
            {
                return sincronizada;
            }
            set
            {
                sincronizada = value;
            }
        }

        #endregion

        #region Constructor

        protected Sesion()
        {

        }

        public Sesion(int numero, int predecesora, DateTime fecha, bool guardada)
        {
            numerosesion = numero;
            sesionpredecesora = predecesora;
            fechainiciosesion = fecha;
            this.guardada = guardada;
        }

        #endregion
    }
}
