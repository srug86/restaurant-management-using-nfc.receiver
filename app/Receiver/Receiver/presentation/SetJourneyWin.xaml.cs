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
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Receiver.presentation
{
    /// <summary>
    /// Lógica de interacción para SetJourneyWin.xaml
    /// </summary>
    public partial class SetJourneyWin : Window
    {
        private TablesManagerWin manager;

        public SetJourneyWin(TablesManagerWin manager, Boolean _new)
        {
            this.manager = manager;
            InitializeComponent();
            if (_new) lblMessage.Content = "*Los datos existentes de la sesión anterior se borrarán.";
            else lblMessage.Content = "*Se trabajará con los datos almacenados la última sesión";
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (txtbName.Text != "")
            {
                this.Close();
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofdXML = new OpenFileDialog();
            ofdXML.Filter = "Archivos XML(*.xml)|*.xml"; // Sólo muestra archivos .xml
            if (ofdXML.ShowDialog().Value)
            {
                txtbName.Text = ofdXML.FileName;
                /*EditRoom.xmlLoader(ofdXML.FileName);
                newRoom(EditRoom.FileName, (EditRoom.NRows - 8) / 4);
                EditRoom.updateData();
                resetAttibutes();*/
            }
        }
    }
}
