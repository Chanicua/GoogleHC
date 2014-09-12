using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.MultiPoint.MultiPointSDK;
using Microsoft.MultiPoint.MultiPointCommonTypes;
using System.Collections;
using MultiPointControl_DLL;
using System.Threading;
using MultiPointControl_DLL.Interfaces;
using System.Xml;
using MMMCommon;
using MMMFrameworkTemplate.Logic_Handlers;
using MultiPointControl_DLL.Controles;
using MultiPointControl_DLL.BasicImplementations;
using MultiPointControl_DLL.Controls;


namespace MMMFrameworkTemplate
{
    /// <summary>
    /// Interaction logic for MultiPointWindow.xaml
    /// </summary>
    public partial class MultiPointWindow : System.Windows.Window
    {
        #region Miembros Privados

        /*
         * Nota: La llave primaria de las 2 HashTables de abajo son el DeviceName de la DeviceInfo del mouse, ¿Porque?
         * Primero, cada dispositivo no tienen ningun tipo de identificador unico por lo que resulta imposible trackearlos
         * pero la relación puerto USB - Mouse es unica y genera el DeviceName. CONCLUSION: Solo se puede reconectar un mouse caido
         * en su mismo puerto!
         */

        /// <summary>
        /// HashTable que va a mantener la informacion de los controles que se pierden, donde la llave es el DeviceName del mouse
        /// </summary>
        private Hashtable devicesLost = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Hashtable with the feedback bars
        /// </summary>
        private Hashtable feedbackbarsLost = Hashtable.Synchronized(new Hashtable());

        #endregion

        /// <summary>
        /// Lista con las cajas de los alumnos
        /// </summary>
        private List<KidBox> kidBoxList = new List<KidBox>();

        /// <summary>
        /// Event which implements the interface, and which activates when the game has finished.
        /// </summary>
        public event pluginGameEndedDelegate gameEnded;

        /// <summary>
        /// Constructor de la clase Multipoint Window
        /// </summary>
        public MultiPointWindow()
        {
            InitializeComponent();

            // Add handler to window loaded event where all multipoint 
            // initialization will happen
            this.Loaded += new RoutedEventHandler(MultiPointWindow_Loaded);
            this.Closed += new EventHandler(MultiPointWindow_Closed);

            // Note: Handlers can be added here to trap MultiPoint control events 
        }

        void MultiPointWindow_Closed(object sender, EventArgs e)
        {
            if (gameEnded != null)
            {
                gameEnded();
            }
        }

        /// <summary>
        /// This is event handler for window "Loaded" Event. 
        /// All MultiPoint initialization should happen on this event.
        /// </summary>
        void MultiPointWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SuscribirMultiPoint();
            DrawGameZone();
            barContainer.DibujarContenedor(GameContext.Instance.GamingMouses);

            foreach (KidBox kb in kidBoxList)
            { 
                kb.controlAnsweredEvent += new GanarControlDelegate(barContainer.ControlGanado);
            }

            PluginHandler.MultiPointObject.DeviceArrivalEvent += new EventHandler<DeviceNotifyEventArgs>(Mouse_Conectado);
            PluginHandler.MultiPointObject.DeviceRemoveCompleteEvent += new EventHandler<DeviceNotifyEventArgs>(Mouse_Desconectado);
        }
        
        /// <summary>
        /// Llama a métodos necesarios para inicializar MultiPoint en esta ventana.
        /// </summary>
        public void SuscribirMultiPoint()
        {
            // Park system cursor at a particular location
            MultiPointSDK.SystemCursorPosition = new Point(this.Left + 10, this.Top + 10);

            //Hide the system cursor
            MultiPointSDK.HideSystemCursor();

            // Set current window before the device visual is  
            // drawn as this is parent window for the visuals
            PluginHandler.MultiPointObject.CurrentWindow = this;

            // Register to receive mouse inputs
            PluginHandler.MultiPointObject.RegisterMouseDevice();

            // Draw device visual for mice 
            PluginHandler.MultiPointObject.DrawMouseDevices();
        }

        #region Drawing Methods

        /// <summary>
        /// Metodo encargado de hacer que la grilla de juego
        /// </summary>
        private void DrawGameZone()
        {
            int mouseNumber = 0;

            foreach (BasicMouse mc in GameContext.Instance.GamingMouses)
            {
                if (!mc.TeacherMouse)
                {
                    mouseNumber++;
                }
            }

            #region Diseño de grilla

            int side = (int)Math.Ceiling(Math.Sqrt(mouseNumber));

            gameGrid.ColumnDefinitions.Clear();
            gameGrid.RowDefinitions.Clear();

            for (int i = 0; i < side; i++)
            {
                gameGrid.ColumnDefinitions.Add(new ColumnDefinition());
                gameGrid.RowDefinitions.Add(new RowDefinition());
            }

            if (mouseNumber + side <= side * side && side > 0)
            {
                gameGrid.RowDefinitions.Remove(gameGrid.RowDefinitions[0]);
            }

            #endregion

            FillKidBoxes(side, mouseNumber);
        }

        private void FillKidBoxes(int side, int mouseNumber)
        {
            int i = 0;
            int j = 0;

            foreach (BasicMouse m in GameContext.Instance.GamingMouses)
            {
                if (!m.TeacherMouse)
                {
                    if (mouseNumber >= j + side * i)
                    {
                        KidBox kidBox = new KidBox(m);
                        AddKidBox(kidBox, i, j);

                        if (j >= side - 1)
                        {
                            j = 0;
                            i++;
                        }
                        else
                        {
                            j++;
                        }
                    }
                }
            }
        }

        private void AddKidBox(KidBox kb, int i, int j)
        {
            kidBoxList.Add(kb);

            Grid.SetRow(kb, i);
            Grid.SetColumn(kb, j);
            gameGrid.Children.Add(kb);
        }

        #endregion

        #region Eventos de Conexion//Desconexion

        void Mouse_Conectado(object sender, DeviceNotifyEventArgs e)
        {
            BasicMouse newMouse = null;
            KidBox kb = null;
            Barra fb = null;

            bool reconnected = false;

            foreach (object key in devicesLost.Keys)
            {
                string deviceName = (string)key;

                if (deviceName.Equals(e.DeviceInfo.DeviceName))
                {
                    kb = (KidBox)devicesLost[deviceName];
                    newMouse = new BasicMouse(e.DeviceInfo);
                    newMouse.AlumnoMouse = kb.alumnoMouse;
                    newMouse.Foto_Identificacion = kb.FotoMouse;

                    GameContext.Instance.GamingMouses.Add(newMouse);
                    fb = (Barra)feedbackbarsLost[deviceName];
                    fb.KidID = e.DeviceInfo.ID;

                    fb.Mouse_Barra = newMouse;

                    devicesLost.Remove(key);
                    feedbackbarsLost.Remove(key);

                    reconnected = true;

                    break;
                }
            }

            fb.Mouse_Barra = newMouse;

            barContainer.barras.Add(fb);
            barContainer.RefrescarBarras();

            kb.ConnectBox(newMouse);
        }

        void Mouse_Desconectado(object sender, DeviceNotifyEventArgs e)
        {
            foreach (BasicMouse m in GameContext.Instance.GamingMouses)
            {
                if (m.deviceInfo.DeviceName == e.DeviceInfo.DeviceName)
                {
                    //We remove the mouse
                    GameContext.Instance.GamingMouses.Remove(m);

                    KidBox disconectedKidBox = null;

                    foreach (KidBox kb in this.kidBoxList)
                    {
                        if (kb.MouseControl != null && kb.MouseControl == m)
                        {
                            disconectedKidBox = kb;
                            disconectedKidBox.DisconnectBox();
                            break;
                        }
                    }

                    devicesLost.Add(m.deviceInfo.DeviceName, disconectedKidBox);

                    //we remove the feedbackbar
                    foreach (Barra b in barContainer.barras)
                    {
                        if (m.deviceInfo.ID == b.KidID)
                        {
                            feedbackbarsLost.Add(m.deviceInfo.DeviceName, b);

                            barContainer.barras.Remove(b);
                            barContainer.RefrescarBarras();

                            break;
                        }
                    }

                    break;
                }
            }
        }

        #endregion

    }
}