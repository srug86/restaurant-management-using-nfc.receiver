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

namespace Receiver.presentation
{
    /// <summary>
    /// Lógica de interacción para NewRoomDialog.xaml
    /// </summary>
    public partial class NewRoomDialog : Window
    {
        private RoomEditorWin editor;

        // Método constructor
        public NewRoomDialog(RoomEditorWin editor)
        {
            this.editor = editor;
            InitializeComponent();
        }

        /* Lógica de control de eventos */
        // Click en el botón "Cancelar"
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Click en el botón "Aceptar"
        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!txtbName.Text.Equals("") && !txtbRows.Text.Equals("") && !txtbColumns.Text.Equals(""))
                {
                    editor.newRoom(txtbName.Text, Convert.ToInt32(txtbRows.Text), Convert.ToInt32(txtbColumns.Text));
                    this.Close();
                }
            }
            catch (FormatException ex) { }
        }
    }
}
