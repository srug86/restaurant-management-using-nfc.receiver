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
    /// Lógica de interacción para NewRoomWin.xaml
    /// </summary>
    public partial class NewRoomWin : Window
    {
        public NewRoomWin()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (txtbName.Text != "")
            {
                EditRoomWin editRoom = new EditRoomWin(txtbName.Text, cbbSize.SelectedIndex);
                editRoom.Show();
                this.Visibility = Visibility.Hidden;
            }
        }
    }
}
