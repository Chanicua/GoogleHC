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
using MultiPointControl_DLL.Controles;
using Microsoft.MultiPoint.MultiPointSDK;
using Microsoft.MultiPoint.MultiPointMousePlugIn;
using Microsoft.MultiPoint.MultiPointCommonTypes;
using System.Diagnostics;
using MultiPointControl_DLL;
using System.IO;

using MMMFrameworkTemplate.Logic_Handlers;
using MultiPointControl_DLL.Controls.BasicControls;


namespace MMMFrameworkTemplate
{
    /// <summary>
    /// Esta es la ventana en la cual se mostrará lo que se desee hacer como parte de la configuración del juego.
    /// </summary>
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ChoiceSetup_Loaded);
        }

        private void SuscribeMultipoint()
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

        #region Event handlers

        private void close_MultiPointMouseDownEvent(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChoiceSetup_Loaded(object sender, RoutedEventArgs e)
        {
            SuscribeMultipoint(); 
        }

        #endregion
        
        #region Bound Methods

        private void close_MultiPointMouseEnterEvent(object sender, RoutedEventArgs e)
        {
            ApplyTransform(false, (MultiPointLabel)sender);
        }

        private void close_MultiPointMouseLeaveEvent(object sender, RoutedEventArgs e)
        {
            ApplyTransform(true, (MultiPointLabel)sender);
        }

        private void ApplyTransform(bool minimize,MultiPointLabel mpl)
        {
            ScaleTransform st;

            if (minimize)
            {
                st = new ScaleTransform(1, 1);
            }
            else
            {
                st = new ScaleTransform(1.2, 1.2);
            }

            st.CenterX = mpl.ActualWidth / 2;
            st.CenterY = mpl.ActualHeight / 2;

            mpl.RenderTransform = st;
        }

        #endregion

    }
}
