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
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class EditRoomWin : Window
    {
        public EditRoomWin()
        {
            InitializeComponent();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            NewRoomWin newRoom = new NewRoomWin();
            newRoom.Show();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReceiver_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTable_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
