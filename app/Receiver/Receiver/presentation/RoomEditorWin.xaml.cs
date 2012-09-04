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
    /// Lógica de interacción para RoomEditorWin.xaml
    /// </summary>
    public partial class RoomEditorWin : Window, ObserverER
    {
        private JourneyManager manager = JourneyManager.Instance;

        private RoomEditor editor = RoomEditor.Instance;

        /* Atributos de la clase RoomEditorWin */
        private String roomName, origTitle;
        public String RoomName
        {
            get { return roomName; }
            set { roomName = value; }
        }

        public String OrigTitle
        {
            get { return origTitle; }
            set { origTitle = value; }
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

        private Image[,] room;
        public Image[,] Room
        {
            get { return room; }
            set { room = value; }
        }

        // Diccionario que relaciona el estado de una mesa con el color que la representa
        private Dictionary<int, string> colorBox = new Dictionary<int, string>() {
            {0, "/Receiver;component/Images/white.png"},
            {1, "/Receiver;component/Images/red.png"},
            {2, "/Receiver;component/Images/yellow.png"},
            {3, "/Receiver;component/Images/green.png"},
            {11, "/Receiver;component/Images/pred.png"},
            {22, "/Receiver;component/Images/pyellow.png"},
            {33, "/Receiver;component/Images/pgreen.png"},
        };

        // Método constructor
        public RoomEditorWin()
        {
            InitializeComponent();
            OrigTitle = this.Title;
            Changes = false;
        }

        // Cambia la GUI a modo "Nueva plantilla"
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

        // Generar plantilla de restaurante vacía
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

        // Carga una plantilla existente
        public void loadSelectedRoom(string name, int rows, int columns)
        {
            newRoom(name, rows, columns);
            editor.loadExistingRoom(name);
            resetAttibutes();
        }

        // Habilita los botones para la edición de una plantilla
        private void enableButtons()
        {
            btnAccept.IsEnabled = true;
            btnReset.IsEnabled = true;
            btnReceiver.IsEnabled = true;
            btnBar.IsEnabled = true;
            btnTable.IsEnabled = true;
        }

        // Identifica cuando una plantilla ha sido modificada
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

        // Resetea las opciones actuales del editor
        private void resetAttibutes()
        {
            btnBar.Background = btnReceiver.Background = btnTable.Background = Brushes.Gainsboro;
            txtbType.Background = Brushes.White;
            txtbType.Text = "";
            txtbId.Text = "";
            txtbId.IsReadOnly = true;
            txtbCapacity.Text = "";
            txtbCapacity.IsReadOnly = true;
            btnIncrease.IsEnabled = btnDecrease.IsEnabled = false;
        }

        // Deshabilita los atributos no utilizables por los modos: "Insertar barra" y "Insertar recibidor"
        private void disableAttributes()
        {
            txtbId.Text = "0";
            txtbCapacity.Text = "0";
            txtbCapacity.IsReadOnly = true;
        }

        /* Métodos para implementar el patrón 'Observer' de la GUI*/
        private void registerSubjects(SubjectER subject)
        {
            subject.registerInterest(this);
        }

        // Método 'notify' que avisa del cambio de estado de una de las casillas de la plantilla
        public void notifyChangesInABox(int row, int column, int state)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri((String)colorBox[state], UriKind.RelativeOrAbsolute);
            bi.EndInit();
            Room[row, column].Source = bi;
        }

        // Método 'notify' que avisa del nuevo identificador para la nueva mesa a insertar
        public void notifyChangesInNTable(int ntable)
        {
            txtbId.Text = Convert.ToString(ntable);
        }

        /* Lógica de control de eventos */
        // Click en el botón "Nueva plantilla"
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            NewRoomDialog newRoom = new NewRoomDialog(this);
            newRoom.Show();
        }

        // Click en el botón "Cargar plantilla"
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadRoomDialog loadRoom = new LoadRoomDialog(this, manager.consultingRooms(), true, false);
            loadRoom.Show();
        }

        // Click en el botón "Guardar plantilla"
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            editor.saveCurrentRoom();
            modifiedRoom(false);
        }

        // Click en el botón "Insertar recibidor"
        private void btnReceiver_Click(object sender, RoutedEventArgs e)
        {
            BoxSelected = false;
            editor.resetOperation();
            txtbType.Text = "Recibidor";
            btnBar.Background = btnTable.Background = Brushes.Gainsboro;
            txtbType.Background = Brushes.Tomato;
            btnReceiver.Background = Brushes.Tomato;
            btnIncrease.IsEnabled = btnDecrease.IsEnabled = false;
            disableAttributes();
            editor.selectedReceiver();
        }

        // Click en el botón "Insertar barra"
        private void btnBar_Click(object sender, RoutedEventArgs e)
        {
            BoxSelected = false;
            editor.resetOperation();
            txtbType.Text = "Barra";
            btnReceiver.Background = btnTable.Background = Brushes.Gainsboro;
            txtbType.Background = Brushes.Yellow;
            btnBar.Background = Brushes.Yellow;
            btnIncrease.IsEnabled = btnDecrease.IsEnabled = false;
            disableAttributes();
            editor.selectedBar();
        }

        // Click en el botón "Insertar mesa"
        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            BoxSelected = false;
            editor.resetOperation();
            txtbType.Text = "Mesa";
            btnBar.Background = btnReceiver.Background = Brushes.Gainsboro;
            txtbType.Background = Brushes.LimeGreen;
            btnTable.Background = Brushes.LimeGreen;
            btnIncrease.IsEnabled = btnDecrease.IsEnabled = true;
            txtbCapacity.IsReadOnly = false;
            txtbCapacity.Text = "4";
            editor.selectedTable();
        }

        // Click en alguna de las casillas de la plantilla del salón del restaurante
        private void box_Click(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            String box = img.Name.Substring(3);
            String[] coordinates = box.Split('x');
            if (editor.selectedBox(Convert.ToInt32(coordinates[0]), Convert.ToInt32(coordinates[1])))
                BoxSelected = true;
        }

        // Click en el botón "Decrementar capacidad de una mesa"
        private void btnDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (!txtbCapacity.Text.Equals(""))
            {
                int c = Convert.ToInt16(txtbCapacity.Text);
                if (c > 1) txtbCapacity.Text = (--c).ToString();
            }
        }

        // Click en el botón "Incrementar capacidad de una mesa"
        private void btnIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (!txtbCapacity.Text.Equals(""))
            {
                int c = Convert.ToInt16(txtbCapacity.Text);
                txtbCapacity.Text = (++c).ToString();
            }
        }

        // Click en el botón "Resetear casillas del objeto"
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            editor.resetOperation();
            resetAttibutes();
        }

        // Click en el botón "Confirmar casillas del objeto"
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
    }
}
