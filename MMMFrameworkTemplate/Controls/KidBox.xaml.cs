using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.MultiPoint.MultiPointMousePlugIn;
using System.Collections;
using MMMCommon;
using MultiPointControl_DLL;
using Microsoft.MultiPoint.MultiPointCommonTypes;
using MultiPointControl_DLL.BasicImplementations;
using MMMFrameworkTemplate.Common_Extension;

namespace MMMFrameworkTemplate
{
    public enum EstadoFeedback { Desconectado, Ganado, Perdido, Vaciar}

    /// <summary>
    /// Lógica de interacción para KidBox.xaml
    /// </summary>
    public partial class KidBox : UserControl
    {
        #region Variables y Propiedades

        //Booleano que define si la caja esta o no conectada.
        private bool BoxConectada = true;

        /// <summary>
        /// Mouse that belongs to this box
        /// </summary>
        private BasicMouse mouseControl;
        public BasicMouse MouseControl
        {
            get { return mouseControl; }
        }

        /// <summary>
        /// Devuelve la ultima imagen de mouse que tiene
        /// </summary>
        private BitmapImage fotoMouse;
        public BitmapImage FotoMouse
        {
            get { return fotoMouse; }
        }

        /// <summary>
        /// Returns the alumno asociated to this mouse
        /// </summary>
        public AlumnoTemplate alumnoMouse
        {
            get { return (AlumnoTemplate)mouseControl.AlumnoMouse; }
        }

        /// <summary>
        /// Events that triggers when the answer of a question has been selected and is ready.
        /// </summary>
        public event MultiPointControl_DLL.Controls.GanarControlDelegate controlAnsweredEvent;        

        #endregion

        public KidBox(BasicMouse mc)
        {
            InitializeComponent();
            SetearMouse(mc);

            this.Loaded += new RoutedEventHandler(KidBox_Loaded);
            this.SizeChanged += new SizeChangedEventHandler(ChangeMouseBounds);
        }

        private void SetearMouse(BasicMouse mc)
        {            
            mouseControl = mc;
            fotoMouse = mouseControl.Foto_Identificacion;            
        }

        private void KidBox_Loaded(object sender, RoutedEventArgs e)
        {
            //Ejemplo
            Image im = new Image();
            im.Source = new BitmapImage(new Uri("pack://application:,,,/MultiPointControl_DLL;Component/Images/Grandes/Botones/Cajitas/play_click.png"));
            buttonExample.Content = im;
            buttonExample.MultiPointMouseDownEvent += new RoutedEventHandler(botonEjemploClickeado);
            controlAnsweredEvent += new MultiPointControl_DLL.Controls.GanarControlDelegate(ganarBox);
            //Ejemplo
            
            ChangeMouseBounds();
        }

        //Ejemplo
        void ganarBox(BasicMouse bm, int points)
        {
            DrawFeedback(EstadoFeedback.Ganado);
        }

        //Ejemplo
        void botonEjemploClickeado(object sender, RoutedEventArgs e)
        {
            controlAnsweredEvent(this.mouseControl, 2);
        }

        #region Drawing Methods

        /// <summary>
        /// Dibuja el feedbac
        /// </summary>
        /// <param name="ef"></param>
        private void DrawFeedback(EstadoFeedback ef)
        {
            if (!BoxConectada) return;

            switch (ef)
            {
                case EstadoFeedback.Vaciar:
                    {
                        feedbackImage.Source = null;
                        break;
                    }
                case EstadoFeedback.Desconectado:
                    {
                        feedbackImage.Source = new BitmapImage(new Uri("pack://application:,,,/MultiPointControl_DLL;Component/Images/Grandes/Feedbacks/Feedback_disconnected.png"));
                        break;
                    }
                case EstadoFeedback.Ganado:
                    {
                        feedbackImage.Source = new BitmapImage(new Uri("pack://application:,,,/MultiPointControl_DLL;Component/Images/Grandes/Feedbacks/Feedback_bien.png"));
                        break;
                    }
                case EstadoFeedback.Perdido:
                    {
                        feedbackImage.Source = new BitmapImage(new Uri("pack://application:,,,/MultiPointControl_DLL;Component/Images/Grandes/Feedbacks/Feedback_error.png"));
                        break;
                    }
            }
        }

        #endregion

        #region Bound methods

        private void KidBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeMouseBounds();
        }

        /// <summary>
        /// Changes the border area of the Mouse
        /// </summary>
        private void ChangeMouseBounds()
        {
            if (mouseControl != null)
            {                
                MouseInputFilter.ChangeMouseBounds(this.mouseControl.deviceInfo.DeviceID, this);
                mouseControl.Position = this.PointToScreen(new Point(10, 10));
            }
        }

        /// <summary>
        /// Method that wraps the mouse to the control through the MousePlugIn
        /// </summary>
        private void ChangeMouseBounds(object sender, RoutedEventArgs e)
        {
            ChangeMouseBounds();
        }

        #endregion

        #region Connectable//Disconnectable

        public void ConnectBox(BasicMouse bm)
        {
            SetearMouse(bm);
            MouseInputFilter.ChangeMouseBounds(bm.deviceInfo.DeviceID, this);
            //No Borrar por ridicula que parezca! Refresca el cursor con su nuevos Bounds.
            bm.Position = bm.Position;

            BoxConectada = true;
            DrawFeedback(EstadoFeedback.Vaciar);
            foreach (UIElement uie in this.mainGrid.Children)
            {
                uie.Opacity = 1;
            }
        }

        public void DisconnectBox()
        {
            DrawFeedback(EstadoFeedback.Desconectado);
            BoxConectada = false;

            foreach (UIElement uie in this.mainGrid.Children)
            {
                //OJO: Esto solo funciona con las imagenes
                if (!(uie is Image))
                {
                    uie.Opacity = 0.4;
                }

            }
        }

        #endregion
    }
}
