using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;

namespace MMMCommon
{
    /// <summary>
    /// Clase que representa a un curso
    /// </summary>
    [Serializable]
    public class Curso
    {
        #region Protected members

        protected string letra;
        protected int nivel;
        protected List<Alumno> alumnos = new List<Alumno>();
        protected List<Sesion> sesiones = new List<Sesion>();

        protected Sesion sesionactual;

        string patharchivocurso = "";

        protected int idinstitucion;
        protected int id;

        protected int academicPeriodId;

        #endregion

        #region Public members

        /// <summary>
        /// Letra del curso (A, B, C, D, ...)
        /// </summary>
        [XmlAttribute("Letra")]
        public string Letra
        {
            get
            {
                return letra;
            }
        }

        /// <summary>
        /// Nivel del curso (1°, 2°, 3°, ...)
        /// </summary>
        [XmlAttribute("Nivel")]
        public int Nivel
        {
            get
            {
                return nivel;
            }
        }

        /// <summary>
        /// Lista de alumnos de un curso
        /// </summary>
        [XmlArray("Alumnos")]
        public List<Alumno> Alumnos
        {
            get
            {
                return alumnos;
            }
        }

        /// <summary>
        /// Ruta del archivo donde se encuentra la lista del curso
        /// </summary>
        [XmlIgnore]
        public string PathArchivoCurso
        {
            get
            {
                return patharchivocurso;
            }
            set
            {
                patharchivocurso = value;
            }
        }

        /// <summary>
        /// Lista de las sesiones que ha jugado el curso
        /// </summary>
        [XmlIgnore]
        public List<Sesion> Sesiones
        {
            get
            {
                return sesiones;
            }
            set
            {
                sesiones = value;
            }
        }

        /// <summary>
        /// Sesion actual que se está jugando
        /// </summary>
        [XmlIgnore]
        public Sesion SesionActual
        {
            get { return sesionactual; }
            set { sesionactual = value; }
        }

        /// <summary>
        /// Id del curso
        /// </summary>
        [XmlAttribute("Id")]
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
            

        /// <summary>
        /// Id de la institucion a la que pertenece el curso
        /// </summary>
        [XmlAttribute("InstitutionId")]
        public int idInstitucion
        {
            get
            {
                return idinstitucion;
            }
            set
            {
                idinstitucion = value;
            }
        }

        /// <summary>
        /// Id del período académico del curso
        /// </summary>
        [XmlAttribute("AcademicPeriodId")]
        public int AcademicPeriodId
        {
            get
            {
                return academicPeriodId;
            }
            set
            {
                academicPeriodId = value;
            }
        }

        #endregion

        #region Constructor

        protected Curso()
        {

        }

        /// <summary>
        /// Constructor de clase curso.
        /// </summary>
        /// <param name="nivelCurso"></param>
        /// <param name="letraCurso"></param>
        public Curso(int nivelCurso, string letraCurso)
        {
            nivel = nivelCurso;
            letra = letraCurso;
        }

        public Curso(int nivelCurso, string letraCurso, int id, int idInstitucion)
            :this(nivelCurso, letraCurso)
        {
            this.Id = id;
            this.idInstitucion = idInstitucion;
        }

        #endregion

        #region Manejo de Alumnos

        internal void SetearAlumnos(List<Alumno> lista)
        {
            alumnos = lista;
        }

        /// <summary>
        /// Agrega un alumno a un curso y ordena la lista alfabéticamente
        /// </summary>
        /// <param name="nuevo"></param>
        public void AgregarAlumno(Alumno nuevo)
        {
            alumnos.Add(nuevo);
            alumnos.Sort();
        }

        /// <summary>
        /// Busca un alumno por su Id
        /// </summary>
        /// <param name="id">Id del alumno buscado</param>
        /// <returns>Alumno buscado. Null en caso de no encontrarse</returns>
        public Alumno BuscarAlumnoPorId(int id)
        {
            Alumno buscado = null;

            foreach (Alumno a in alumnos)
            {
                if (a.ID == id)
                {
                    buscado = a;
                    break;
                }
            }

            return buscado;
        }

        #endregion

        #region Manejo de Sesiones

        /// <summary>
        /// Crea una nueva sesión para el curso.
        /// </summary>
        /// <param name="SesionPredecesora"></param>
        public void CrearSesion(int SesionPredecesora)
        {
            int numerosesion = 1;
            
            if (sesiones.Count > 0)
            {
                numerosesion = getUltimaSesion().NumeroSesion;
            }

            sesionactual = new Sesion(numerosesion + 1, SesionPredecesora, DateTime.Now, false); 

            sesiones.Add(sesionactual);
        }

        /// <summary>
        /// Devuelve la última sesión del curso.
        /// </summary>
        /// <returns></returns>
        public Sesion getUltimaSesion()
        {
            return Sesiones.OrderBy(x => x.FechaInicioSesion).LastOrDefault();

        }

        /// <summary>
        /// Devuelve una sesión predecesora. Null si no la encuentra o predecesora es -1
        /// </summary>
        /// <param name="sesion">Sesión de la cual se quiere la predecesora</param>
        /// <returns>Sesión predecesora. Null si no la encuentra o predecesora es -1</returns>
        public Sesion getSesionPredecesora(Sesion sesion)
        {
            if (sesion.SesionPredecesora > 0 && sesiones.Count > 0)
            {
                foreach (Sesion s in sesiones)
                {
                    if (s.NumeroSesion == sesion.SesionPredecesora)
                    {
                        return s;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
