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
using Microsoft.Win32;

namespace Receiver.presentation
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class EditRoomWin : Window, ObserverER
    {
        private String roomName;

        public String RoomName
        {
            get { return roomName; }
            set { roomName = value; }
        }
        private int[] size;

        public int[] Size
        {
            get { return size; }
            set { size = value; }
        }
        private EditRoom editRoom;

        public EditRoom EditRoom
        {
            get { return editRoom; }
            set { editRoom = value; }
        }
        private Image[,] room;

        public Image[,] Room
        {
            get { return room; }
            set { room = value; }
        }
        private String origTitle;

        public String OrigTitle
        {
            get { return origTitle; }
            set { origTitle = value; }
        }
        private Boolean boxSelected;

        public Boolean BoxSelected
        {
            get { return boxSelected; }
            set { boxSelected = value; }
        }
        private Boolean changes;

        public Boolean Changes
        {
            get { return changes; }
            set { changes = value; }
        }
        private Dictionary<int, string> colorBox;

        public EditRoomWin(SubjectER subject, EditRoom editRoom)
        {
            InitializeComponent();
            EditRoom = editRoom;
            OrigTitle = this.Title;
            Changes = false;
            register(subject);
        }

        public void newRoom(String name, int size)
        {
            BoxSelected = false;
            RoomName = name;
            Size = new int[2];
            Size[0] = 8 + 4 * size;
            Size[1] = Size[0] + 4;
            initRoom();
            EditRoom.createNewRoom(RoomName, Size[0], Size[1]);
            paintNewRoom();
        }

        private void paintNewRoom()
        {
            switch (Size[0])
            {
                case 8: uniGrid8x12.Visibility = Visibility.Visible; break;
                case 12: uniGrid12x16.Visibility = Visibility.Visible; break;
                case 16: uniGrid16x20.Visibility = Visibility.Visible; break;
                default: break;
            }
            Changes = true;
            this.Title = OrigTitle + " - " + RoomName + "*";
        }

        private void initRoom()
        {
            gridRoom.Visibility = Visibility.Visible;
            uniGrid8x12.Visibility = Visibility.Hidden;
            uniGrid12x16.Visibility = Visibility.Hidden;
            uniGrid16x20.Visibility = Visibility.Hidden;
            switch (Size[0])
            {
                case 8:
                    Room = new Image[8, 12] {
            {imgS1x1, imgS1x2, imgS1x3, imgS1x4, imgS1x5, imgS1x6,
                imgS1x7, imgS1x8, imgS1x9, imgS1x10, imgS1x11, imgS1x12},
            {imgS2x1, imgS2x2, imgS2x3, imgS2x4, imgS2x5, imgS2x6, imgS2x7,
                imgS2x8, imgS2x9, imgS2x10, imgS2x11, imgS2x12},
            {imgS3x1, imgS3x2, imgS3x3, imgS3x4, imgS3x5, imgS3x6, imgS3x7,
                imgS3x8, imgS3x9, imgS3x10, imgS3x11, imgS3x12},
            {imgS4x1, imgS4x2, imgS4x3, imgS4x4, imgS4x5, imgS4x6, imgS4x7,
                imgS4x8, imgS4x9, imgS4x10, imgS4x11, imgS4x12},
            {imgS5x1, imgS5x2, imgS5x3, imgS5x4, imgS5x5, imgS5x6, imgS5x7,
                imgS5x8, imgS5x9, imgS5x10, imgS5x11, imgS5x12},
            {imgS6x1, imgS6x2, imgS6x3, imgS6x4, imgS6x5, imgS6x6, imgS6x7,
                imgS6x8, imgS6x9, imgS6x10, imgS6x11, imgS6x12},
            {imgS7x1, imgS7x2, imgS7x3, imgS7x4, imgS7x5, imgS7x6, imgS7x7,
                imgS7x8, imgS7x9, imgS7x10, imgS7x11, imgS7x12},
            {imgS8x1, imgS8x2, imgS8x3, imgS8x4, imgS8x5, imgS8x6, imgS8x7,
                imgS8x8, imgS8x9, imgS8x10, imgS8x11, imgS8x12}};
                    break;
                case 12:
                    Room = new Image[12, 16] {
            {imgM1x1, imgM1x2, imgM1x3, imgM1x4, imgM1x5, imgM1x6, imgM1x7, imgM1x8, imgM1x9,
                imgM1x10, imgM1x11, imgM1x12, imgM1x13, imgM1x14, imgM1x15, imgM1x16},
            {imgM2x1, imgM2x2, imgM2x3, imgM2x4, imgM2x5, imgM2x6, imgM2x7, imgM2x8, imgM2x9,
                imgM2x10, imgM2x11, imgM2x12, imgM2x13, imgM2x14, imgM2x15, imgM2x16},
            {imgM3x1, imgM3x2, imgM3x3, imgM3x4, imgM3x5, imgM3x6, imgM3x7, imgM3x8, imgM3x9,
                imgM3x10, imgM3x11, imgM3x12, imgM3x13, imgM3x14, imgM3x15, imgM3x16},
            {imgM4x1, imgM4x2, imgM4x3, imgM4x4, imgM4x5, imgM4x6, imgM4x7, imgM4x8, imgM4x9,
                imgM4x10, imgM4x11, imgM4x12, imgM4x13, imgM4x14, imgM4x15, imgM4x16},
            {imgM5x1, imgM5x2, imgM5x3, imgM5x4, imgM5x5, imgM5x6, imgM5x7, imgM5x8, imgM5x9,
                imgM5x10, imgM5x11, imgM5x12, imgM5x13, imgM5x14, imgM5x15, imgM5x16},
            {imgM6x1, imgM6x2, imgM6x3, imgM6x4, imgM6x5, imgM6x6, imgM6x7, imgM6x8, imgM6x9,
                imgM6x10, imgM6x11, imgM6x12, imgM6x13, imgM6x14, imgM6x15, imgM6x16},
            {imgM7x1, imgM7x2, imgM7x3, imgM7x4, imgM7x5, imgM7x6, imgM7x7, imgM7x8, imgM7x9,
                imgM7x10, imgM7x11, imgM7x12, imgM7x13, imgM7x14, imgM7x15, imgM7x16},
            {imgM8x1, imgM8x2, imgM8x3, imgM8x4, imgM8x5, imgM8x6, imgM8x7, imgM8x8, imgM8x9,
                imgM8x10, imgM8x11, imgM8x12, imgM8x13, imgM8x14, imgM8x15, imgM8x16},
            {imgM9x1, imgM9x2, imgM9x3, imgM9x4, imgM9x5, imgM9x6, imgM9x7, imgM9x8, imgM9x9,
                imgM9x10, imgM9x11, imgM9x12, imgM9x13, imgM9x14, imgM9x15, imgM9x16},
            {imgM10x1, imgM10x2, imgM10x3, imgM10x4, imgM10x5, imgM10x6, imgM10x7, imgM10x8, imgM10x9,
                imgM10x10, imgM10x11, imgM10x12, imgM10x13, imgM10x14, imgM10x15, imgM10x16},
            {imgM11x1, imgM11x2, imgM11x3, imgM11x4, imgM11x5, imgM11x6, imgM11x7, imgM11x8, imgM11x9,
                imgM11x10, imgM11x11, imgM11x12, imgM11x13, imgM11x14, imgM11x15, imgM11x16},
            {imgM12x1, imgM12x2, imgM12x3, imgM12x4, imgM12x5, imgM12x6, imgM12x7, imgM12x8, imgM12x9,
                imgM12x10, imgM12x11, imgM12x12, imgM12x13, imgM12x14, imgM12x15, imgM12x16}};
                    break;
                case 16:
                    Room = new Image[16, 20] {
            {imgB1x1, imgB1x2, imgB1x3, imgB1x4, imgB1x5, imgB1x6, imgB1x7, imgB1x8, imgB1x9,
                imgB1x10, imgB1x11, imgB1x12, imgB1x13, imgB1x14, imgB1x15, imgB1x16, imgB1x17,
                imgB1x18, imgB1x19, imgB1x20},
            {imgB2x1, imgB2x2, imgB2x3, imgB2x4, imgB2x5, imgB2x6, imgB2x7, imgB2x8, imgB2x9,
                imgB2x10, imgB2x11, imgB2x12, imgB2x13, imgB2x14, imgB2x15, imgB2x16, imgB2x17,
                imgB2x18, imgB2x19, imgB2x20},
            {imgB3x1, imgB3x2, imgB3x3, imgB3x4, imgB3x5, imgB3x6, imgB3x7, imgB3x8, imgB3x9,
                imgB3x10, imgB3x11, imgB3x12, imgB3x13, imgB3x14, imgB3x15, imgB3x16, imgB3x17,
                imgB3x18, imgB3x19, imgB3x20},
            {imgB4x1, imgB4x2, imgB4x3, imgB4x4, imgB4x5, imgB4x6, imgB4x7, imgB4x8, imgB4x9,
                imgB4x10, imgB4x11, imgB4x12, imgB4x13, imgB4x14, imgB4x15, imgB4x16, imgB4x17,
                imgB4x18, imgB4x19, imgB4x20},
            {imgB5x1, imgB5x2, imgB5x3, imgB5x4, imgB5x5, imgB5x6, imgB5x7, imgB5x8, imgB5x9,
                imgB5x10, imgB5x11, imgB5x12, imgB5x13, imgB5x14, imgB5x15, imgB5x16, imgB5x17,
                imgB5x18, imgB5x19, imgB5x20},
            {imgB6x1, imgB6x2, imgB6x3, imgB6x4, imgB6x5, imgB6x6, imgB6x7, imgB6x8, imgB6x9,
                imgB6x10, imgB6x11, imgB6x12, imgB6x13, imgB6x14, imgB6x15, imgB6x16, imgB6x17,
                imgB6x18, imgB6x19, imgB6x20},
            {imgB7x1, imgB7x2, imgB7x3, imgB7x4, imgB7x5, imgB7x6, imgB7x7, imgB7x8, imgB7x9,
                imgB7x10, imgB7x11, imgB7x12, imgB7x13, imgB7x14, imgB7x15, imgB7x16, imgB7x17,
                imgB7x18, imgB7x19, imgB7x20},
            {imgB8x1, imgB8x2, imgB8x3, imgB8x4, imgB8x5, imgB8x6, imgB8x7, imgB8x8, imgB8x9,
                imgB8x10, imgB8x11, imgB8x12, imgB8x13, imgB8x14, imgB8x15, imgB8x16, imgB8x17,
                imgB8x18, imgB8x19, imgB8x20},
            {imgB9x1, imgB9x2, imgB9x3, imgB9x4, imgB9x5, imgB9x6, imgB9x7, imgB9x8, imgB9x9,
                imgB9x10, imgB9x11, imgB9x12, imgB9x13, imgB9x14, imgB9x15, imgB9x16, imgB9x17,
                imgB9x18, imgB9x19, imgB9x20},
            {imgB10x1, imgB10x2, imgB10x3, imgB10x4, imgB10x5, imgB10x6, imgB10x7, imgB10x8, imgB10x9,
                imgB10x10, imgB10x11, imgB10x12, imgB10x13, imgB10x14, imgB10x15, imgB10x16, imgB10x17,
                imgB10x18, imgB10x19, imgB10x20},
            {imgB11x1, imgB11x2, imgB11x3, imgB11x4, imgB11x5, imgB11x6, imgB11x7, imgB11x8, imgB11x9,
                imgB11x10, imgB11x11, imgB11x12, imgB11x13, imgB11x14, imgB11x15, imgB11x16, imgB11x17,
                imgB11x18, imgB11x19, imgB11x20},
            {imgB12x1, imgB12x2, imgB12x3, imgB12x4, imgB12x5, imgB12x6, imgB12x7, imgB12x8, imgB12x9,
                imgB12x10, imgB12x11, imgB12x12, imgB12x13, imgB12x14, imgB12x15, imgB12x16, imgB12x17,
                imgB12x18, imgB12x19, imgB12x20},
            {imgB13x1, imgB13x2, imgB13x3, imgB13x4, imgB13x5, imgB13x6, imgB13x7, imgB13x8, imgB13x9,
                imgB13x10, imgB13x11, imgB13x12, imgB13x13, imgB13x14, imgB13x15, imgB13x16, imgB13x17,
                imgB13x18, imgB13x19, imgB13x20},
            {imgB14x1, imgB14x2, imgB14x3, imgB14x4, imgB14x5, imgB14x6, imgB14x7, imgB14x8, imgB14x9,
                imgB14x10, imgB14x11, imgB14x12, imgB14x13, imgB14x14, imgB14x15, imgB14x16, imgB14x17,
                imgB14x18, imgB14x19, imgB14x20},
            {imgB15x1, imgB15x2, imgB15x3, imgB15x4, imgB15x5, imgB15x6, imgB15x7, imgB15x8, imgB15x9,
                imgB15x10, imgB15x11, imgB15x12, imgB15x13, imgB15x14, imgB15x15, imgB15x16, imgB15x17,
                imgB15x18, imgB15x19, imgB15x20},
            {imgB16x1, imgB16x2, imgB16x3, imgB16x4, imgB16x5, imgB16x6, imgB16x7, imgB16x8, imgB16x9,
                imgB16x10, imgB16x11, imgB16x12, imgB16x13, imgB16x14, imgB16x15, imgB16x16, imgB16x17,
                imgB16x18, imgB16x19, imgB16x20}};
                    break;
                default: break;
            }
        }

        private void register(SubjectER subject)
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
            Room[row, column].Source = bi;
        }

        public void notify(int ntable)
        {
            txtbId.Text = Convert.ToString(ntable);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            NewRoomDialog newRoom = new NewRoomDialog(this);
            newRoom.Show();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofdXML = new OpenFileDialog();
            ofdXML.Filter = "Archivos XML(*.xml)|*.xml"; // Sólo muestra archivos .xml
            if (ofdXML.ShowDialog().Value)
            {
                EditRoom.xmlLoader(ofdXML.FileName);
                newRoom(EditRoom.FileName, (EditRoom.NRows - 8) / 4);
                EditRoom.updateData();
                resetAttibutes();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Changes)
            {
                SaveFileDialog sfdXML = new SaveFileDialog();
                sfdXML.Filter = "Archivos XML(*.xml)|*.xml"; // Sólo muestra archivos .xml
                sfdXML.FileName = RoomName + ".xml";
                if (sfdXML.ShowDialog().Value)
                {
                    EditRoom.xmlBuilder(sfdXML.FileName);
                    Changes = false;
                    this.Title = this.Title.Substring(0, this.Title.Length - 1);
                }
            }
        }

        private void btnReceiver_Click(object sender, RoutedEventArgs e)
        {
            BoxSelected = false;
            EditRoom.resetOperation();
            txtbType.Text = "Recibidor";
            disableAttributes();
            EditRoom.selectedReceiver();
        }

        private void btnBar_Click(object sender, RoutedEventArgs e)
        {
            BoxSelected = false;
            EditRoom.resetOperation();
            txtbType.Text = "Bar";
            disableAttributes();
            EditRoom.selectedBar();
        }

        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            BoxSelected = false;
            EditRoom.resetOperation();
            txtbType.Text = "Mesa";
            txtbCapacity.IsReadOnly = false;
            txtbCapacity.Text = "4";
            EditRoom.selectedTable();
        }

        private void Box_Click(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            String box = img.Name.Substring(4);
            String [] coordinates = box.Split('x');
            if (EditRoom.selectedBox(Convert.ToInt32(coordinates[0]) - 1, Convert.ToInt32(coordinates[1]) - 1))
                BoxSelected = true;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            EditRoom.resetOperation();
            resetAttibutes();
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (!BoxSelected)
                return;
            if (txtbType.Text == "")
                return;
            if (txtbId.Text == "")
                return;
            if (txtbCapacity.Text == "")
                return;
            EditRoom.acceptOperation(Convert.ToInt32(txtbId.Text), Convert.ToInt32(txtbCapacity.Text));
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
            txtbCapacity.Text = "0";
            txtbCapacity.IsReadOnly = true;
        }
    }
}
