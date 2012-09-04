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
using Receiver.domain;
using System.Windows.Controls.Primitives;
using Receiver.communication;
using System.Windows.Threading;

namespace Receiver.presentation
{
    /// <summary>
    /// Lógica de interacción para JourneyManagerWin.xaml
    /// </summary>
    public partial class JourneyManagerWin : Window, ObserverRE
    {
        private JourneyManager manager = JourneyManager.Instance;

        /* Atributos de la clase 'JourneyManagerWin' */
        // Nombre de la plantilla
        private string roomName;
        public string RoomName
        {
            get { return roomName; }
            set { roomName = value; }
        }

        private int roomHeight, roomWidth;
        // Número de filas de la plantilla
        public int RoomHeight
        {
            get { return roomHeight; }
            set { roomHeight = value; }
        }
        // Número de columnas de la plantilla
        public int RoomWidth
        {
            get { return roomWidth; }
            set { roomWidth = value; }
        }

        // Matriz de casillas de la plantilla
        private Image[, ,] room;
        public Image[, ,] Room
        {
            get { return room; }
            set { room = value; }
        }

        private Grid[] gridsOnMode;

        private UniformGrid[] uGridsRooms;

        // 'Logger'
        private FlowDocument fdEvents;

        // Diccionario que relaciona el estado de una mesa con el color que la representa
        private Dictionary<int, string> colorBox = new Dictionary<int, string> {
            {-1, "/Receiver;component/Images/white.png"},
            {0, "/Receiver;component/Images/green.png"},
            {1, "/Receiver;component/Images/dgreen.png"},
            {2, "/Receiver;component/Images/dgreen.png"},
            {3, "/Receiver;component/Images/orange.png"},
            {4, "/Receiver;component/Images/red.png"},
            {5, "/Receiver;component/Images/yellow.png"},
            {6, "/Receiver;component/Images/pgreen.png"},
        };

        // Método constructor
        public JourneyManagerWin()
        {
            InitializeComponent();
            initGridsOnMode();
            openOffPerspective();
        }

        // Generación de una matriz con los 'grids' de los modos de una jornada
        private void initGridsOnMode()
        {
            gridsOnMode = new Grid[7] { gridORoomView, gridOCome, gridOComeNFC, gridOLeave, 
                gridOLeaveNFC, gridOPayNFC, gridOAllocation };
            foreach (Grid g in gridsOnMode)
                g.Visibility = Visibility.Hidden;
        }

        // Muestra el 'grid' de un modo concreto
        private void showOnModeGrid(Grid grid)
        {
            foreach (Grid g in gridsOnMode)
            {
                if (g.Equals(grid)) g.Visibility = Visibility.Visible;
                else g.Visibility = Visibility.Hidden;
            }
        }

        // 'Setea' los 'grids' para el modo 'OFF' del "gestor de mesas"
        private void openOffPerspective()
        {
            gridInitial.Visibility = Visibility.Visible;
            gridOptions.Visibility = Visibility.Hidden;
            btnOCome.IsEnabled = false;
            btnOLeave.IsEnabled = false;
            btnOView.IsEnabled = false;
        }

        // 'Setea' los 'grids' para el modo 'ON' del "gestor de mesas"
        private void openOnPerspective()
        {
            gridInitial.Visibility = Visibility.Hidden;
            gridOptions.Visibility = Visibility.Visible;
            btnOCome.IsEnabled = true;
            btnOLeave.IsEnabled = true;
            btnOView.IsEnabled = true;
            openViewPerspective();
        }

        // Muestra la perspectiva de "Cliente llega"
        private void openArrivalPerspective()
        {
            txtbIdCome.Text = idBuilder();
            showOnModeGrid(gridOCome);
        }

        // Muestra la perspectiva de "Cliente NFC llega"
        private void openArrivalNFCPerspective()
        {
            showOnModeGrid(gridOComeNFC);
        }

        // Muestra la perspectiva de "Ubicar cliente en mesa"
        private void openAllocationPerspective()
        {
            txtbAllocName.Text = manager.ClientManager.Client.Dni;
            manager.RoomManager.getCandidateTables(manager.ClientManager.Guests);
            showOnModeGrid(gridOAllocation);
        }

        // Muestra la perspectiva de "Cobrar cliente NFC"
        private void openPaymentPerspective()
        {
            showOnModeGrid(gridOPayNFC);
        }

        // Muestra la perspectiva de "Cliente se va"
        private void openExitPerspective()
        {
            manager.RoomManager.getCandidateTables();
            showOnModeGrid(gridOLeave);
        }

        // Muestra la perspectiva de "Cliente NFC se va"
        private void openExitNFCPerspective()
        {
            showOnModeGrid(gridOLeaveNFC);
        }

        // Muestra la perspectiva de "Vista general"
        private void openViewPerspective()
        {
            resetProvisionalData();
            manager.RoomManager.refreshTables();
            showOnModeGrid(gridORoomView);
        }

        // Carga una nueva jornada o una jornada existente
        public void loadSelectedRoom(string name, bool reset)
        {
            manager.createRoomManager();
            manager.RoomManager.loadRoom(name, reset);  // Carga la plantilla seleccionada
            generateEmptyRoom(manager.RoomManager.Room.Name,
                manager.RoomManager.Room.Height, manager.RoomManager.Room.Width);
            registerSubjects(manager.RoomManager);      // Se registra como 'Observer'
            manager.ClientManager.setGuiReference(this);
            manager.RoomManager.locateObjects();        // Carga los objetos en la plantilla
            if (reset) manager.resetCurrentJourney(name);   // 'Resetea' la información de la jornada anterior
            else manager.RoomManager.updateTables();        // o carga el estado de las mesas de una jornada existente
            manager.initBluetoothServer();              // Inicializa el servidor Bluetooth
            this.delegateToShowReceiverEvent(0, reset ? "Se inicia de una nueva jornada." : "Se inicia una jornada existente.");
            openOnPerspective();
        }

        // Genera una plantilla de restaurante vacía
        private void generateEmptyRoom(string name, int height, int width)
        {
            RoomName = name;
            RoomHeight = height;
            RoomWidth = width;
            Room = new Image[RoomHeight, RoomWidth, 3];
            fdEvents = new FlowDocument();
            fdEvents.LineHeight = 0.2;
            rtbEvents.Document = fdEvents;
            uGridsRooms = new UniformGrid[] { uGridStatus, uGridAllocation, uGridDeallocation };
            for (int u = 0; u < uGridsRooms.Count(); u++)
            {
                if (uGridsRooms[u].Children.Count > 0) uGridsRooms[u].Children.Clear();
                uGridsRooms[u].Rows = RoomHeight;
                uGridsRooms[u].Columns = RoomWidth;
                for (int i = 0; i < RoomHeight; i++)
                    for (int j = 0; j < RoomWidth; j++)
                    {
                        Image image = new Image();
                        image.Name = "img" + i + "x" + j;
                        image.Stretch = Stretch.Fill;
                        image.MouseUp += new MouseButtonEventHandler(this.box_Click);
                        Room[i, j, u] = image;
                        uGridsRooms[u].Children.Add(image);
                    }
            }
        }

        // 'Resetea' la información residual de la GUI tras un cambio de modo
        private void resetProvisionalData()
        {
            btnAcceptAllocation.IsEnabled = false;
            btnAcceptDeallocation.IsEnabled = false;
            txtbAllocName.Text = txtbAllocTable.Text = txtbCapacityCome.Text =
                txtbDeallocName.Text = txtbDeallocTable.Text = "";
            lblPDNI.Content = lblPName.Content = lblPSurname.Content = lblPTable.Content =
                lblPBill.Content = lblPAmount.Content = "-";
        }

        // Generador de identificadores para un cliente "no NFC"
        private string idBuilder()
        {
            DateTime time = DateTime.Now;
            string month = time.Month.ToString(), day = time.Day.ToString(), hour = time.Hour.ToString(),
                minute = time.Minute.ToString(), second = time.Second.ToString();
            if (time.Month < 10) month = "0" + time.Month.ToString();
            if (time.Day < 10) day = "0" + time.Day.ToString();
            if (time.Hour < 10) hour = "0" + time.Hour.ToString();
            if (time.Minute < 10) minute = "0" + time.Minute.ToString();
            if (time.Second < 10) second = "0" + time.Second.ToString();
            return "C" + time.Year + month + day + hour + minute + second;
        }

        /* Métodos para implementar el patrón 'Observer' de la GUI*/
        private void registerSubjects(SubjectRE subjectRE)
        {
            subjectRE.registerInterest(this);
        }

        // Método 'notify' que avisa del cambio de estado de una de las casillas de la plantilla
        public void notifyChangesInABox(int row, int column, int state)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri((string)colorBox[state], UriKind.RelativeOrAbsolute);
            bi.EndInit();
            for (int i = 0; i < uGridsRooms.Count(); i++)
                Room[row, column, i].Source = bi;
        }

        /* Delegados de la GUI */
        // Delegado que registra los eventos producidos en el 'logger' del modo "Vista general"
        private delegate void ReceiverEvent(int type, string message);
        public void delegateToShowReceiverEvent(int type, string message)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new ReceiverEvent(this.rcvEvent), type, message);
        }

        public void rcvEvent(int type, string message)
        {
            Paragraph p = new Paragraph();
            switch (type)
            {
                case 0: p.Foreground = Brushes.DarkBlue; break;     // Inicio de jornada
                case 1: p.Foreground = Brushes.DarkGreen; break;    // Llega cliente
                case 2: p.Foreground = Brushes.DarkOrange; break;   // Paga cliente NFC
                case 3: p.Foreground = Brushes.DarkRed; break;      // Se va cliente
                default: break;
            }
            p.Inlines.Add(new Bold(new Run("(" + DateTime.Now + ")\t" + message)));
            if (fdEvents.Blocks.Count > 0)
                fdEvents.Blocks.InsertBefore(fdEvents.Blocks.FirstBlock, p);
            else fdEvents.Blocks.Add(p);
        }

        // Delegado que informa de la llegada de un cliente NFC
        private delegate void EntryNFCClient(Client client);
        public void delegateToNFCClientHasArrived(Client client)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new EntryNFCClient(this.nfcEntry), client);
        }

        public void nfcEntry(Client client)
        {
            lblCDNI.Content = client.Dni;
            lblCName.Content = client.Name;
            lblCSurname.Content = client.Surname;
            openArrivalNFCPerspective();
        }

        // Delegado que informa de la salida de un cliente NFC
        private delegate void ExitNFCClient(Client client, int table);
        public void delegateToNFCClientHasLeft(Client client, int table)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new ExitNFCClient(this.nfcExit), client, table);
        }

        public void nfcExit(Client client, int table)
        {
            lblLDNI.Content = client.Dni;
            lblLName.Content = client.Name;
            lblLSurname.Content = client.Surname;
            lblLTable.Content = table;
            openExitNFCPerspective();
        }

        // Delegado que informa del pago de un cliente NFC
        private delegate void PayNFCClient(Client client, int table, int billID, double amount);
        public void delegateToNFCClientHasPaid(Client client, int table, int billID, double amount)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new PayNFCClient(this.nfcPayment), client, table, billID, amount);
        }

        public void nfcPayment(Client client, int table, int billID, double amount)
        {
            lblPDNI.Content = client.Dni;
            lblPName.Content = client.Name;
            lblPSurname.Content = client.Surname;
            lblPTable.Content = table;
            lblPBill.Content = billID;
            lblPAmount.Content = amount + " €";
            openPaymentPerspective();
        }

        /* Lógica de control de eventos */
        // Click en el botón "Nueva jornada"
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            LoadRoomDialog roomDialog = new LoadRoomDialog(this, manager.consultingRooms(), false, true);
            roomDialog.Show();
        }

        // Click en el botón "Cargar jornada existente"
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadRoomDialog roomDialog = new LoadRoomDialog(this, manager.consultingCurrentRoom(), false, false);
            roomDialog.Show();
        }

        // Click en un botón del teclado numérico del modo "Cliente llega"
        private void btnCM_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            String number = btn.Name.Substring(5);
            if (number == "Delete" && txtbCapacityCome.Text != "")  // Botón 'Delete' (<--)
                    txtbCapacityCome.Text = txtbCapacityCome.Text.Substring(1, txtbCapacityCome.Text.Length - 1);
            else txtbCapacityCome.Text += number;                   // Botón numérico (0-9)
        }

        // Click en un botón del teclado numérico del modo "Cliente NFC llega"
        private void btnCN_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            String number = btn.Name.Substring(5);
            if (number == "Delete" && txtbCapacityComeNFC.Text != "")   // Botón 'Delete' (<--)
                    txtbCapacityComeNFC.Text = txtbCapacityComeNFC.Text.Substring(1, txtbCapacityComeNFC.Text.Length - 1);
            else txtbCapacityComeNFC.Text += number;                    // Botón numérico (0-9)
        }

        // Click en el botón del modo "Cliente llega"
        private void btnOCome_Click(object sender, RoutedEventArgs e)
        {
            openArrivalPerspective();
        }

        // Click en el botón del modo "Cliente se va"
        private void btnOLeave_Click(object sender, RoutedEventArgs e)
        {
            openExitPerspective();
        }

        // Click en el botón del modo "Vista general"
        private void btnOView_Click(object sender, RoutedEventArgs e)
        {
            manager.RoomManager.updateTables();
            openViewPerspective();
        }

        // Click en alguna de las casillas de la plantilla del restaurante
        private void box_Click(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            string box = img.Name.Substring(3);
            string[] coordinates = box.Split('x');
            int selectedTable = manager.RoomManager.selectedBox(Convert.ToInt16(coordinates[0]),
                Convert.ToInt16(coordinates[1]));
            if (gridOAllocation.IsVisible && selectedTable != -1)
                txtbAllocTable.Text = Convert.ToString(selectedTable);
            else if (gridOLeave.IsVisible && selectedTable != -1)
            {
                txtbDeallocTable.Text = Convert.ToString(selectedTable);
                txtbDeallocName.Text = manager.RoomManager.getClientsTable(selectedTable);
            }
        }

        // Click en el botón "Aceptar" en el modo "Cliente llega"
        private void btnAcceptCome_Click(object sender, RoutedEventArgs e)
        {
            if (txtbIdCome.Text != "" && txtbCapacityCome.Text != "")
            {
                manager.ClientManager.Client = new Client(txtbIdCome.Text, "", "", new Address());
                manager.ClientManager.Guests = Convert.ToInt16(txtbCapacityCome.Text);
                openAllocationPerspective();
                txtbCapacityCome.Text = "";
            }
        }

        // Click en el botón "Cancelar" en el modo "Cliente llega"
        private void btnCancelCome_Click(object sender, RoutedEventArgs e)
        {
            openViewPerspective();
        }

        // Click en el botón "Asignar mesa" en el modo "Ubicar cliente en mesa"
        private void btnAcceptAllocation_Click(object sender, RoutedEventArgs e)
        {
            if (txtbAllocTable.Text != "")
            {
                if (manager.ClientManager.Client.Dni.Substring(0, 1) == "C")    // Para clientes "no NFC"
                    manager.ClientManager.newStandardClient();
                manager.RoomManager.confirmAllocation(Convert.ToInt16(txtbAllocTable.Text));
                this.delegateToShowReceiverEvent(1, "El cliente " + txtbAllocName.Text + " llega al restaurante y ocupa la mesa " + txtbAllocTable.Text + ".");
                openViewPerspective();
            }
        }

        // Click en el botón "Cancelar" en el modo "Ubicar cliente en mesa"
        private void btnCancelAllocation_Click(object sender, RoutedEventArgs e)
        {
            if (txtbAllocName.Text.Substring(0, 1) == "C")  // Para clientes "no NFC"
                openArrivalPerspective();
            else
                openArrivalNFCPerspective();                // Para clientes NFC
            btnAcceptAllocation.IsEnabled = false;
            txtbAllocName.Text = txtbAllocTable.Text = "";
        }

        // Click en el botón "Aceptar" en el modo "Desubicar cliente de mesa"
        private void btnAcceptDeallocation_Click(object sender, RoutedEventArgs e)
        {
            if (txtbDeallocName.Text != "")
            {
                manager.RoomManager.confirmDeallocation(Convert.ToInt16(txtbDeallocTable.Text));
                this.delegateToShowReceiverEvent(3, "El cliente " + txtbDeallocName.Text + " deja libre la mesa " + txtbDeallocTable.Text + " y abandona el restaurante.");
                openViewPerspective();
            }
        }

        // Click en el botón "Cancelar" en el modo "Desubicar cliente de mesa"
        private void btnCancelDeallocation_Click(object sender, RoutedEventArgs e)
        {
            btnAcceptDeallocation.IsEnabled = false;
            openViewPerspective();
        }

        // Click en el botón "Aceptar" en el modo "Cliente NFC llega"
        private void btnAcceptComeNFC_Click(object sender, RoutedEventArgs e)
        {
            if (txtbCapacityComeNFC.Text != "")
            {
                manager.ClientManager.Guests = Convert.ToInt16(txtbCapacityComeNFC.Text);
                openAllocationPerspective();
                txtbCapacityComeNFC.Text = "";
            }
        }

        // Click en el botón "Cancelar" en el modo "Cliente NFC llega"
        private void btnCancelComeNFC_Click(object sender, RoutedEventArgs e)
        {
            openViewPerspective();
        }

        // Click en el botón "Aceptar" en el modo "Cliente NFC se va"
        private void btnAcceptLeaveNFC_Click(object sender, RoutedEventArgs e)
        {
            manager.RoomManager.confirmDeallocation(Convert.ToInt16(lblLTable.Content));
            this.delegateToShowReceiverEvent(3, "El cliente " + lblLDNI.Content + " deja libre la mesa " + lblLTable.Content + " y abandona el restaurante.");
            openViewPerspective();
        }

        // Click en el botón "Cancelar" en el modo "Cliente NFC se va"
        private void btnCancelLeaveNFC_Click(object sender, RoutedEventArgs e)
        {
            openViewPerspective();
        }

        // Click en el botón "Cerrar" en el modo "Cliente NFC paga"
        private void btnClosePayNFC_Click(object sender, RoutedEventArgs e)
        {
            openViewPerspective();
        }

        // Cambio de contenido en el texto del identificador de mesa en el modo "Ubicar cliente en mesa"
        private void txtbAllocTable_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnAcceptAllocation.IsEnabled = true;
        }

        // Cambio de contenido en el texto del identificador de mesa en el modo "Desubicar cliente de mesa"
        private void txtbDeallocTable_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnAcceptDeallocation.IsEnabled = true;
        }
    }
}
