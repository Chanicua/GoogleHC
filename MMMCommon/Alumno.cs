using System;
using System.Xml.Serialization;

namespace MMMCommon
{
    [Serializable]
    public class Alumno: IComparable<Alumno>
    {
        #region Private members

        string nombre;
        string apellido;
        int id;
        string simbolo;
        string dni;
        bool seleccionado = false;

        #endregion

        #region Public members

        /// <summary>
        /// Nombre del alumno
        /// </summary>
        [XmlAttribute("Nombre")]
        public string Nombre
        {
            get
            {
                return nombre;
            }
        }

        /// <summary>
        /// Apellido del alumno
        /// </summary>
        [XmlAttribute("Apellido")]
        public string Apellido
        {
            get
            {
                return apellido;
            }
        }

        /// <summary>
        /// Id del alumno. Puede ser numero de lista o otro identificador numérico
        /// </summary>
        [XmlAttribute("id")]
        public int ID
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// Simbolo que le corresponde al alumno
        /// </summary>
        [XmlAttribute("Simbolo")]
        public string Simbolo
        {
            get
            {
                return simbolo;
            }
            set
            {
                simbolo = value;
            }
        }

        /// <summary>
        /// Nombre completo del alumno de la forma: Apellido, Nombre
        /// </summary>
        [XmlIgnore]
        public string NombreCompleto
        {
            get
            {
                if (!string.IsNullOrEmpty(Apellido) && !string.IsNullOrEmpty(Nombre))
                {
                    return Apellido + ", " + Nombre;
                }
                else if (!string.IsNullOrEmpty(Apellido))
                {
                    return Apellido;
                }
                else if (!string.IsNullOrEmpty(Nombre))
                {
                    return Nombre;
                }
                else
                {
                    return "";
                }

            }

        }

        /// <summary>
        /// Digito nacional de identificacion del alumno
        /// </summary>
        [XmlAttribute("Dni")]
        public string Dni
        {
            get
            {
                return dni;
            }
            set
            {
                dni = value;
            }
        }

        //Tiempo en que el niño aparece en pantalla previo a la liberacion
        [XmlIgnore]
        public DateTime TiempoInicioReconocimiento;

        //Tiempo en que el niño aparece en pantalla previo a la liberacion
        [XmlIgnore]
        public DateTime TiempoFinalReconocimiento;

        //Determina si el alumno esta asociado a un mouse
        [XmlIgnore]
        public bool Seleccionado
        {
            get
            {
                return seleccionado;
            }
            set
            {
                seleccionado = value;
            }
        }

        #endregion

        #region Constructor

        protected Alumno()
        {

        }

        /// <summary>
        /// Constructor de la clase alumno, si es que el simbolo no está asignado.
        /// </summary>
        /// <param name="idAlumno"></param>
        /// <param name="nombreAlumno"></param>
        /// <param name="apellidoAlumno"></param>
        public Alumno(int idAlumno, string nombreAlumno, string apellidoAlumno)
        {
            id = idAlumno;
            nombre = nombreAlumno;
            apellido = apellidoAlumno;
        }

        /// <summary>
        /// Constructor de la clase alumno si el simbolo si está asignado
        /// </summary>
        /// <param name="idAlumno"></param>
        /// <param name="nombreAlumno"></param>
        /// <param name="apellidoAlumno"></param>
        /// <param name="simboloAlumno"></param>
        public Alumno(int idAlumno, string nombreAlumno, string apellidoAlumno, string simboloAlumno)
            : this(idAlumno, nombreAlumno, apellidoAlumno)
        {
            simbolo = simboloAlumno;
        }
        #endregion

        /// <summary>
        /// Entrega el tiempo en milisegundos de cuanto tiempo se demoró el niño en reconocerse
        /// </summary>
        /// <returns>El tiempo en milisegundos</returns>
        public int CalcularTiempoReconocimiento()
        {
            if (TiempoInicioReconocimiento != null && TiempoFinalReconocimiento != null)
            {
                 TimeSpan diferencia = TiempoFinalReconocimiento - TiempoInicioReconocimiento;
                 if (diferencia.TotalMilliseconds > 0) return (int)diferencia.TotalMilliseconds;
            }
                return -1;
        }

        #region IComparable<Alumno> Members

        /// <summary>
        /// Compara alfabeticamente dos alumnos, primero por el apellido y despues por el nombre.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Alumno other)
        {
            return string.Compare(NombreCompleto, other.NombreCompleto, StringComparison.CurrentCultureIgnoreCase);
        }

        #endregion
    }
}
