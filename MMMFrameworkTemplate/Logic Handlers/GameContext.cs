using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MMMCommon;
using MultiPointControl_DLL.BasicImplementations;

namespace MMMFrameworkTemplate.Logic_Handlers
{
    public class GameContext
    {
        #region Singleton

        private static GameContext gc = new GameContext();

        private GameContext()
        { 
        
        }

        public static GameContext Instance 
        {
            get { return gc; }
        }

        #endregion

        /// <summary>
        /// Lista de Mouses que son visibles desde toda la aplicación.
        /// </summary>
        public List<BasicMouse> GamingMouses = new List<BasicMouse>();

        /// <summary>
        /// Nodos originales que entrega el sistema
        /// </summary>
        public XmlNodeList nodosIniciales;

        /// <summary>
        /// Curso que esta en juego
        /// </summary>
        public Curso cursoJuego;

        /// <summary>
        /// Ubicación del plugin dentro del computador, termina en backslash
        /// </summary>
        public static string Location = Environment.CurrentDirectory;

        /// <summary>
        /// Evento que permite grabar el estado actual del sistema.
        /// </summary>
        public static event MultiPointControl_DLL.Interfaces.pluginForceSaveDelegate ForzarGrabacion;
    }
}
