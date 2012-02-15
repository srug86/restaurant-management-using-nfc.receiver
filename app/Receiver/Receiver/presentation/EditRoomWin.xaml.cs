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
using System.Windows.Controls.Primitives;

namespace Receiver.presentation
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class EditRoomWin : Window
    {
        private String name;
        private int size;
        private UniformGrid grid;

        public EditRoomWin(String name, int size)
        {
            this.name = name;
            this.size = size;
            buildGrid();
            InitializeComponent();
        }

        private void buildGrid()
        {
            grid = new UniformGrid();
            switch (size)
            {
                case 0: grid.Rows = 8; grid.Columns = 12; break;
                case 1: grid.Rows = 12; grid.Columns = 16; break;
                case 2: grid.Rows = 16; grid.Columns = 20; break;
                default: grid.Rows = 12; grid.Columns = 16; break;
            }
            /*
             */
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
