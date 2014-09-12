using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiPointControl_DLL.Interfaces;
using MultiPointControl_DLL;
using System.Xml;
using MMMCommon;
using Microsoft.MultiPoint.MultiPointSDK;
using MMMFrameworkTemplate.Common_Extension;
using MultiPointControl_DLL.BasicImplementations;

namespace MMMFrameworkTemplate.Logic_Handlers
{
    /// <summary>
    /// Se debe colocar los siguiente 3 valores como string como la MMMPluginInfo:
    ///  - Game Name: Es el nombre del juego que se despliega en la lista de juegos seleccionables.
    ///  - Description: Es la descripción breve del juego, supuestamente para cuando uno pase por arriba del 
    ///  nombre en el menu pero no esta implementado aun.
    ///  - XMLTag: Es el tag que se usa dentro del archivo de grabacion para guardar la informacion de este PlugIn
    /// </summary>
    [MMMPluginInfo("Game Name", "Description", "XMLTag")]
    public class PluginHandler: IMMMPlugin
    {
        #region Events

        /// <summary>
        /// Event to implement the interface which is thrown when  the configuration ends.
        /// </summary>
        public event pluginConfigurationEndedDelegate configEnded;

        /// <summary>
        /// Event which implements the interface, and which activates when the game has finished.
        /// </summary>
        public event pluginGameEndedDelegate gameEnded;

        /// <summary>
        /// Evento que llama en el framework a getFinalData()
        /// </summary>
        public event pluginForceSaveDelegate pluginForceSave;

        #endregion

        /// <summary>
        /// Ventana donde se desarrolla el juego
        /// </summary>
        private MultiPointWindow mpw;

        /// <summary>
        /// Get the instance of MultiPoint SDK class which we will accessable across this
        /// application. It is a singleton class.
        /// </summary>
        public static MultiPointSDK MultiPointObject = MultiPointSDK.GetInstance();

        /// <summary>
        /// 
        /// </summary>
        public PluginHandler()
        {
            GameContext.ForzarGrabacion += new pluginForceSaveDelegate(ForzarGrabacion);
        }

        #region Windows Cerradas

        /// <summary>
        /// Metodo que se ejecuta cuando se cierra la ventana de configuracion, para avisar al framework se ha terminado el setup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetupWindow_Closed(object sender, EventArgs e)
        {
            if (configEnded != null)
            {
                configEnded();
            }
        }

        /// <summary>
        /// Metodo que se ejecuta cuando se cierra la ventana de juego, para avisar al framework se ha terminado el juego.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MultiPointWindow_Closed(object sender, EventArgs e)
        {
            if (gameEnded != null)
            {
                gameEnded();
            }
        }

        #endregion

        #region Miembros de IMMMPlugin

        /// <summary>
        /// Lista de mouses con la informacion de cada uno de ellos y su asociación al niño respectivo.
        /// </summary>
        public List<BasicMouse> GamingMice
        {
            get
            {
                return GameContext.Instance.GamingMouses;
            }
            set
            {
                GameContext.Instance.GamingMouses = value;
            }
        }

        /// <summary>
        /// Ubicación real en el directorio del sistema del plugin cuando se está corriendo
        /// </summary>
        public string Location
        {
            get
            {
                return GameContext.Location;
            }
            set
            {
                GameContext.Location = value;
            }
        }

        /// <summary>
        /// Nos entrega el arbol respectivo asociado al Plug-In para que la applicacion principal escriba en el XML,
        /// de esta forma evitamos que los plug ins defectuosos borren datos de otra app.
        /// </summary>
        /// <returns></returns>
        public XmlNodeList getFinalData()
        {
            XmlNodeList originalNodes = GameContext.Instance.nodosIniciales;
            XmlNode prevNode = originalNodes[0];

            XmlDocument doc = prevNode.OwnerDocument;
            XmlNode sesionNode = doc.CreateNode(XmlNodeType.Element, "Sesion", "");

            CrearAtributo("id", GameContext.Instance.cursoJuego.SesionActual.NumeroSesion.ToString(), doc, sesionNode);
            CrearAtributo("predecesora", GameContext.Instance.cursoJuego.SesionActual.SesionPredecesora.ToString(), doc, sesionNode);
            CrearAtributo("inicio", GameContext.Instance.cursoJuego.SesionActual.FechaInicioSesion.ToString(), doc, sesionNode);
            CrearAtributo("final", DateTime.Now.ToString(), doc, sesionNode);

            /* INSERTAR CODIGO DE GRABADO DE DATOS */

            prevNode.AppendChild(sesionNode);

            return originalNodes;
        }

        /// <summary>
        /// Nos muestra una pequeña foto que se usa durante el proceso de seleccion del Plug-In
        /// </summary>
        /// <returns></returns>
        public Uri getPluginIcon()
        {
            //Obtenemos la imagen por defecto desde la Libreria, notese que:
            // - La primera parte: pack://application:,,,/ es para referirse a un recurso de sistema
            // - La segunda parte: MultiPointControl_DLL;Component/ es porque la assembly donde se encuentra la imagen no es la que esta en compilacion
            // - La tercera parte: Es el path propiamente tal.
            return new Uri("pack://application:,,,/MultiPointControl_DLL;Component/Images/Grandes/Botones/Settings/Boton_pregunta.png");
        }

        /// <summary>
        /// Se entregan los datos del arbol XML respectivo para poder cargar los datos del juego.
        /// </summary>
        /// <param name="nodos"></param>
        public void loadData(Curso c, XmlNodeList nodos)
        {
            //Guardamos los nodos originales
            GameContext.Instance.nodosIniciales = nodos;

            //Transformamos los alumnos del curso
            TransformarAlumnosCurso(c);

            //Guardamos una instancia del curso actual para poder ver datos posteriores.
            GameContext.Instance.cursoJuego = c;

            foreach (XmlNode node in nodos[0])
            {
                if (node.NodeType == XmlNodeType.Comment) continue;

                #region Cargado de Sesion

                int id = Convert.ToInt32(node.Attributes["id"].Value);
                int id_pred = Convert.ToInt32(node.Attributes["predecesora"].Value);

                DateTime inicio = DateTime.Now;
                DateTime final = inicio;

                if (node.Attributes["inicio"].Value != "")
                {
                    inicio = DateTime.Parse(node.Attributes["inicio"].Value);
                }
                if (node.Attributes["final"].Value != "")
                {
                    final = DateTime.Parse(node.Attributes["final"].Value);
                }

                SesionTemplate sc = new SesionTemplate(id, id_pred, inicio, true);

                c.Sesiones.Add((Sesion)sc);

                #endregion

                /* INSERTAR CODIGO DE LECTURA DE DATOS */
            }

            #region Creacion de nueva Sesion

            Sesion s = c.getUltimaSesion();
            SesionTemplate nuevaSesion;

            if (s != null)
            {
                nuevaSesion = new SesionTemplate(s.NumeroSesion + 1, s.NumeroSesion, DateTime.Now, false);
            }
            else
            {
                nuevaSesion = new SesionTemplate(1, -1, DateTime.Now, false);
            }

            c.SesionActual = nuevaSesion;
            c.Sesiones.Add((Sesion)nuevaSesion);

            #endregion

        }

        /// <summary>
        /// Evento que se ejecuta cuando termina la configuracion del Plug-In
        /// </summary>
        public event pluginConfigurationEndedDelegate pluginConfigurationEnded
        {
            add
            {
                configEnded += value;
            }
            remove
            {
                configEnded -= value;
            }
        }

        /// <summary>
        /// Evento que se ejecuta cuando termina el juego del Plug-In. Este juego incluye la visualizacion de resultados.
        /// </summary>
        public event pluginGameEndedDelegate pluginGameEnded
        {
            add
            {
                gameEnded += value;
            }
            remove
            {
                gameEnded -= value;
            }
        }

        /// <summary>
        /// Registra los dispositivos en MultiPointSDK para dicha ventana.
        /// </summary>
        public void registerMultipointMouse()
        {
            if (mpw != null)
            {
                mpw.SuscribirMultiPoint();
            }
        }

        /// <summary>
        /// Setea el lenguaje asociado en su carpeta respectiva de lenguajes. Se usan siglas como en para ingles y es para español
        /// </summary>
        /// <param name="language"></param>
        public void setLanguage(string language)
        {
            //No está listo en el Framework
        }

        /// <summary>
        /// Inicia la configuración del PlugIn, esta debe ser una nueva ventana.
        /// </summary>
        public void startPluginConfiguration()
        {
            SetupWindow sw = new SetupWindow();
            sw.Closed += new EventHandler(SetupWindow_Closed);
            sw.Show();
        }

        /// <summary>
        /// Inicia el juego y se le entrega control de los mouses.
        /// </summary>
        public void startPluginGame()
        {
            mpw = new MultiPointWindow();
            mpw.Closed += new EventHandler(MultiPointWindow_Closed);
            mpw.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ForzarGrabacion()
        {
            pluginForceSave();
        }

        #endregion

        #region Utilities

        //Transforma los alumnos del curso en alumnos de esta instancia especifica.
        private void TransformarAlumnosCurso(Curso c)
        {
            List<AlumnoTemplate> listAlumnosTemporales = new List<AlumnoTemplate>();

            foreach (Alumno al in c.Alumnos)
            {
                AlumnoTemplate alch = new AlumnoTemplate(al);
                listAlumnosTemporales.Add(alch);
            }

            c.Alumnos.Clear();

            foreach (AlumnoTemplate ac in listAlumnosTemporales)
            {
                c.Alumnos.Add(ac);
            }
        }

        //Crea un atributo asociado a un nodo
        private void CrearAtributo(string nombreAtributo, string valor, XmlDocument document, XmlNode nodeToAppend)
        {
            XmlNode atrib = document.CreateNode(XmlNodeType.Attribute, nombreAtributo, "");
            atrib.Value = valor;
            nodeToAppend.Attributes.SetNamedItem(atrib);
        }

        #endregion
    }
}
