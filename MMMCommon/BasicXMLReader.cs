using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;

namespace MMMCommon
{
    public class BasicXMLReader
    {
        #region Private members
        string xmlpath;

        #endregion

        #region Public members
        
        /// <summary>
        /// Path del xml con que se está trabajando.
        /// </summary>
        public string XMLPath
        {
            get
            {
                return xmlpath;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la clase. Recibe el path del xml a trabajar.
        /// </summary>
        /// <param name="path"></param>
        public BasicXMLReader(string path)
        {
            xmlpath = path;
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Lee la lista de un curso determinado y devuelve un curso.
        /// </summary>
        /// <param name="nivelcurso"></param>
        /// <param name="letracurso"></param>
        /// <returns>Lista del curso. Null si no encuentra el curso.</returns>
        public Curso LeerListaCurso(int nivelcurso, string letracurso)
        {
            //TODO: Hacer esto con XELEMENT y LINQ para mayor velocidad

            //XmlReader reader = new XmlTextReader(XMLPath);
            //reader.MoveToContent();
            Curso curso = null;

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(XMLPath);

                XElement xcurso = XElement.Parse(doc.OuterXml);

                int existecurso = xcurso.Elements("Curso")
                                        .Where(x => Convert.ToInt32(x.Attribute("nivel").Value, CultureInfo.InvariantCulture) == nivelcurso)
                                        .Where(x => letracurso.Equals(x.Attribute("letra").Value))
                                        .Count();

                if (existecurso > 0)
                {
                    //Se carga el objeto de la ultima sesion
                    curso = xcurso.Elements("Curso")
                                            .Where(x => Convert.ToInt32(x.Attribute("nivel").Value, CultureInfo.InvariantCulture) == nivelcurso)
                                            .Where(x => letracurso.Equals(x.Attribute("letra").Value))
                                            .Select(x => new Curso(Convert.ToInt32(x.Attribute("nivel").Value, CultureInfo.InvariantCulture), x.Attribute("letra").Value)
                                            {
                                                idInstitucion = x.Attribute("idInstitucion") != null ? Convert.ToInt32(x.Attribute("idInstitucion").Value, CultureInfo.InvariantCulture) : 0,
                                                Id = x.Attribute("id") != null ? Convert.ToInt32(x.Attribute("id").Value, CultureInfo.InvariantCulture) : 0,
                                                AcademicPeriodId = x.Attribute("academicPeriodId") != null ? Convert.ToInt32(x.Attribute("academicPeriodId").Value, CultureInfo.InvariantCulture) : 0,
                                            })
                                            .First();

                    List<Alumno> alumnoscurso = xcurso.Elements("Curso").Elements("Alumno")
                                            .OrderBy(a => a.Attribute("apellido").Value)
                                            .Select(a => new Alumno(Convert.ToInt32(a.Attribute("id").Value, CultureInfo.InvariantCulture),
                                                                    a.Attribute("nombre").Value,
                                                                    a.Attribute("apellido").Value,
                                                                    a.Attribute("simbolo").Value) { })
                                            .ToList();

                    curso.SetearAlumnos(alumnoscurso);
                }
            }
            catch
            {
            }

            return curso;
        }

        /// <summary>
        /// Lee los alumnos de un xml de un curso.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="curso"></param>
        private void LeerAlumnosCurso(XmlReader reader, Curso curso)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "Alumno")
                    {
                        int id = int.Parse(reader.GetAttribute("id"), CultureInfo.InvariantCulture);
                        string nombre = reader.GetAttribute("nombre");
                        string apellidop = reader.GetAttribute("apellido");
                        string simbolo = reader.GetAttribute("simbolo");

                        Alumno alumno = new Alumno(id, nombre, apellidop, simbolo);
                        curso.AgregarAlumno(alumno);
                    }

                }

                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name != "Alumno")
                {
                    break;
                }
            }
        }

        public void GuardarCurso(Curso c, List<Alumno> alumnos)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.Encoding = Encoding.UTF8;
            
            XmlWriter writer = XmlWriter.Create(xmlpath, xws);

            c.PathArchivoCurso = xmlpath;

            writer.WriteStartElement("root");

            writer.WriteStartElement("Curso");

            //atributos de curso
            writer.WriteAttributeString("nivel", c.Nivel.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("letra", c.Letra);
            writer.WriteAttributeString("id", c.Id.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("idInstitucion", c.idInstitucion.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("academicPeriodId", c.AcademicPeriodId.ToString(CultureInfo.InvariantCulture));

            BaseTools.SymbolIdentificator = 1;

            //alumnos
            foreach (Alumno a in alumnos)
            {
                if (string.IsNullOrEmpty(a.Simbolo))
                {
                    a.Simbolo = "Icono" + BaseTools.GetSymbolIdentificator() + ".png";
                }
                
                c.Alumnos.Add(a);

                writer.WriteStartElement("Alumno");
                writer.WriteAttributeString("id", a.ID.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("nombre", a.Nombre);
                writer.WriteAttributeString("apellido", a.Apellido);
                writer.WriteAttributeString("Dni", a.Dni);
                writer.WriteAttributeString("simbolo", a.Simbolo);
                writer.WriteEndElement();
            }

            //end para curso
            writer.WriteEndElement();

            //Inicio Plugins
            writer.WriteStartElement("Plugins");

            //end para Plugins
            writer.WriteEndElement();

            //end para root
            writer.WriteEndElement();

            writer.Close();
        }

        #endregion
    }
}
