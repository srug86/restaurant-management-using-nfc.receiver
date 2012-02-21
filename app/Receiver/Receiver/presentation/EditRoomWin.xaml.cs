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
using Receiver.domain;

namespace Receiver.presentation
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class EditRoomWin : Window, Observer
    {
        private String name;
        private int[] size;
        private EditRoom editRoom;
        private Image[,] room;
        private String origTitle;
        private int tableN;
        private Boolean boxSelected;
        private Dictionary<int, string> colorBox;

        public EditRoomWin(Subject subject, EditRoom editRoom)
        {
            InitializeComponent();
            this.editRoom = editRoom;
            this.origTitle = this.Title;
            register(subject);
        }

        public void newRoom(String name, int size)
        {
            this.tableN = 1;
            this.boxSelected = false;
            this.name = name;
            this.size = new int[2];
            switch (size)
            {
                case 0: this.size[0] = 8; this.size[1] = 12; break;
                case 1: this.size[0] = 12; this.size[1] = 16; break;
                case 2: this.size[0] = 16; this.size[1] = 20; break;
                default: this.size[0] = 8; this.size[1] = 12; break;
            }
            this.editRoom.createNewRoom(this.size[0], this.size[1]);
            paintNewRoom();
        }

        private void paintNewRoom()
        {
            initRoom();
            //
            gridRoom.Visibility = Visibility.Visible;
            this.Title = this.origTitle + " - " + this.name;
        }

        private void initRoom()
        {
            this.room = new Image[8, 12] {
            {imgS1x1, imgS1x2, imgS1x3, imgS1x4, imgS1x5, imgS1x6, imgS1x7, imgS1x8, imgS1x9, imgS1x10, imgS1x11, imgS1x12},
            {imgS2x1, imgS2x2, imgS2x3, imgS2x4, imgS2x5, imgS2x6, imgS2x7, imgS2x8, imgS2x9, imgS2x10, imgS2x11, imgS2x12},
            {imgS3x1, imgS3x2, imgS3x3, imgS3x4, imgS3x5, imgS3x6, imgS3x7, imgS3x8, imgS3x9, imgS3x10, imgS3x11, imgS3x12},
            {imgS4x1, imgS4x2, imgS4x3, imgS4x4, imgS4x5, imgS4x6, imgS4x7, imgS4x8, imgS4x9, imgS4x10, imgS4x11, imgS4x12},
            {imgS5x1, imgS5x2, imgS5x3, imgS5x4, imgS5x5, imgS5x6, imgS5x7, imgS5x8, imgS5x9, imgS5x10, imgS5x11, imgS5x12},
            {imgS6x1, imgS6x2, imgS6x3, imgS6x4, imgS6x5, imgS6x6, imgS6x7, imgS6x8, imgS6x9, imgS6x10, imgS6x11, imgS6x12},
            {imgS7x1, imgS7x2, imgS7x3, imgS7x4, imgS7x5, imgS7x6, imgS7x7, imgS7x8, imgS7x9, imgS7x10, imgS7x11, imgS7x12},
            {imgS8x1, imgS8x2, imgS8x3, imgS8x4, imgS8x5, imgS8x6, imgS8x7, imgS8x8, imgS8x9, imgS8x10, imgS8x11, imgS8x12}};
        }

        private void register(Subject subject)
        {
            subject.registerInterest(this);
            colorBox = new Dictionary<int, string>();
            colorBox.Add(0, "/Receiver;component/Resources/white.jpg");
            colorBox.Add(1, "/Receiver;component/Resources/red.jpg");
            colorBox.Add(2, "/Receiver;component/Resources/yellow.jpg");
            colorBox.Add(3, "/Receiver;component/Resources/green.jpg");
            colorBox.Add(11, "/Receiver;component/Resources/pred.jpg");
            colorBox.Add(22, "/Receiver;component/Resources/pyellow.jpg");
            colorBox.Add(33, "/Receiver;component/Resources/pgreen.jpg");
        }

        public void notify(int row, int column, int state)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri((String)colorBox[state], UriKind.RelativeOrAbsolute);
            bi.EndInit();
            this.room[row, column].Source = bi;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            NewRoomWin newRoom = new NewRoomWin(this);
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
            boxSelected = false;
            this.editRoom.resetOperation();
            txtbType.Text = "Recibidor";
            disableAttributes();
            this.editRoom.selectedReceiver();
        }

        private void btnBar_Click(object sender, RoutedEventArgs e)
        {
            boxSelected = false;
            this.editRoom.resetOperation();
            txtbType.Text = "Bar";
            disableAttributes();
            this.editRoom.selectedBar();
        }

        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            boxSelected = false;
            this.editRoom.resetOperation();
            txtbType.Text = "Mesa";
            txtbId.IsReadOnly = false;
            txtbId.Text = Convert.ToString(this.tableN);
            txtbCapacity.IsReadOnly = false;
            txtbCapacity.Text = "4";
            this.editRoom.selectedTable();
        }

        private void Box_Click(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            String box = img.Name.Substring(4);
            String [] coordinates = box.Split('x');
            if (this.editRoom.selectedBox(Convert.ToInt32(coordinates[0]) - 1, Convert.ToInt32(coordinates[1]) - 1))
                boxSelected = true;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            this.editRoom.resetOperation();
            resetAttibutes();
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (!boxSelected)
                return;
            if (txtbType.Text == "")
                return;
            if (txtbId.Text == "")
                return;
            if (txtbCapacity.Text == "")
                return;
            if (this.editRoom.acceptOperation(Convert.ToInt32(txtbId.Text), Convert.ToInt32(txtbCapacity.Text)))
                this.tableN++;
            resetAttibutes();
        }

        private void resetAttibutes()
        {
            txtbType.Text = "";
            txtbId.Text = "";
            txtbId.IsReadOnly = true;
            txtbCapacity.Text = "";
            txtbCapacity.IsReadOnly = true;
        }

        private void disableAttributes()
        {
            txtbId.Text = "0";
            txtbId.IsReadOnly = true;
            txtbCapacity.Text = "0";
            txtbCapacity.IsReadOnly = true;
        }
    }
}
