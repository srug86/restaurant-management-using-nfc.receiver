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
    public partial class RoomEditorWin : Window, ObserverER
    {
        private JourneyManager manager = JourneyManager.Instance;

        private RoomEditor editor = RoomEditor.Instance;

        private String roomName;

        public String RoomName
        {
            get { return roomName; }
            set { roomName = value; }
        }

        private int rows, columns;

        public int Rows
        {
          get { return rows; }
          set { rows = value; }
        }

        public int Columns
        {
          get { return columns; }
          set { columns = value; }
        }

        private Image[,] room;

        public Image[,] Room
        {
            get { return room; }
            set { room = value; }
        }

        private Dictionary<int, string> colorBox = new Dictionary<int, string>() {
            {0, "/Receiver;component/Resources/white.jpg"},
            {1, "/Receiver;component/Resources/red.jpg"},
            {2, "/Receiver;component/Resources/yellow.jpg"},
            {3, "/Receiver;component/Resources/green.jpg"},
            {11, "/Receiver;component/Resources/pred.jpg"},
            {22, "/Receiver;component/Resources/pyellow.jpg"},
            {33, "/Receiver;component/Resources/pgreen.jpg"},
        };

        private String origTitle;

        public String OrigTitle
        {
            get { return origTitle; }
            set { origTitle = value; }
        }

        private bool boxSelected, changes;

        public bool BoxSelected
        {
            get { return boxSelected; }
            set { boxSelected = value; }
        }

        public bool Changes
        {
            get { return changes; }
            set { changes = value; }
        }

        public RoomEditorWin()
        {
            InitializeComponent();
            OrigTitle = this.Title;
            Changes = false;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            NewRoomDialog newRoom = new NewRoomDialog(this);
            newRoom.Show();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadRoomDialog loadRoom = new LoadRoomDialog(this, manager.consultingRooms(), true, false);
            loadRoom.Show();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            editor.saveCurrentRoom();
            modifiedRoom(false);
        }

        private void btnReceiver_Click(object sender, RoutedEventArgs e)
        {
            BoxSelected = false;
            editor.resetOperation();
            txtbType.Text = "Recibidor";
            btnBar.Background = btnTable.Background = Brushes.Gainsboro;
            btnReceiver.Background = Brushes.Tomato;
            disableAttributes();
            editor.selectedReceiver();
        }

        private void btnBar_Click(object sender, RoutedEventArgs e)
        {
            BoxSelected = false;
            editor.resetOperation();
            txtbType.Text = "Bar";
            btnReceiver.Background = btnTable.Background = Brushes.Gainsboro;
            btnBar.Background = Brushes.Yellow;
            disableAttributes();
            editor.selectedBar();
        }

        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            BoxSelected = false;
            editor.resetOperation();
            txtbType.Text = "Mesa";
            btnBar.Background = btnReceiver.Background = Brushes.Gainsboro;
            btnTable.Background = Brushes.LimeGreen;
            txtbCapacity.IsReadOnly = false;
            txtbCapacity.Text = "4";
            editor.selectedTable();
        }

        private void box_Click(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            String box = img.Name.Substring(3);
            String [] coordinates = box.Split('x');
            if (editor.selectedBox(Convert.ToInt32(coordinates[0]), Convert.ToInt32(coordinates[1])))
                BoxSelected = true;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            editor.resetOperation();
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
            editor.acceptOperation(Convert.ToInt32(txtbId.Text), Convert.ToInt32(txtbCapacity.Text));
            resetAttibutes();
            modifiedRoom(true);
        }

        public void newRoom(String name, int rows, int columns)
        {
            RoomName = name;
            Rows = rows;
            Columns = columns;
            this.Title = OrigTitle + " - " + RoomName;
            BoxSelected = false;
            Changes = true;
            generateEmptyRoom(name, rows, columns);
            registerSubjects(editor);
            editor.createNewRoom(RoomName, Rows, Columns);
            enableButtons();
            resetAttibutes();
        }

        private void generateEmptyRoom(String name, int rows, int columns)
        {
            Room = new Image[Rows, Columns];
            uGridRoom.Children.Clear();
            uGridRoom.Rows = Rows;
            uGridRoom.Columns = Columns;
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                {
                    Image image = new Image();
                    image.Name = "img" + i + "x" + j;
                    image.Stretch = Stretch.Fill;
                    image.MouseUp += new MouseButtonEventHandler(this.box_Click);
                    Room[i, j] = image;
                    uGridRoom.Children.Add(image);
                }
        }

        public void loadSelectedRoom(string name, int rows, int columns)
        {
            newRoom(name, rows, columns);
            editor.loadExistingRoom(name);
            resetAttibutes();
        }

        private void enableButtons()
        {
            btnAccept.IsEnabled = true;
            btnReset.IsEnabled = true;
            btnReceiver.IsEnabled = true;
            btnBar.IsEnabled = true;
            btnTable.IsEnabled = true;
        }

        private void modifiedRoom(bool modify)
        {
            if (modify && !this.Title.Substring(this.Title.Length - 1).Equals("*"))
            {
                this.Title += "*";
                btnSave.IsEnabled = true;
            }
            else if (!modify && this.Title.Substring(this.Title.Length - 1).Equals("*"))
            {
                this.Title = this.Title.Substring(0, this.Title.Length - 1);
                btnSave.IsEnabled = false;
            }
        }

        private void resetAttibutes()
        {
            btnBar.Background = btnReceiver.Background = btnTable.Background = Brushes.Gainsboro;
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

        private void registerSubjects(SubjectER subject)
        {
            subject.registerInterest(this);
        }

        public void notifyChangesInABox(int row, int column, int state)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri((String)colorBox[state], UriKind.RelativeOrAbsolute);
            bi.EndInit();
            Room[row, column].Source = bi;
        }

        public void notifyChangesInNTable(int ntable)
        {
            txtbId.Text = Convert.ToString(ntable);
        }
    }
}
