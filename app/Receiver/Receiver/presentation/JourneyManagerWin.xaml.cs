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

        private BluetoothServer bluetoothServer = BluetoothServer.Instance;

        private string roomName;

        public string RoomName
        {
            get { return roomName; }
            set { roomName = value; }
        }

        private int roomHeight, roomWidth;

        public int RoomHeight
        {
            get { return roomHeight; }
            set { roomHeight = value; }
        }

        public int RoomWidth
        {
            get { return roomWidth; }
            set { roomWidth = value; }
        }

        private Image[, ,] room;

        public Image[, ,] Room
        {
            get { return room; }
            set { room = value; }
        }

        private Grid[] gridsOnMode;

        private UniformGrid[] uGridsRooms;

        private FlowDocument fdEvents;

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

        public JourneyManagerWin()
        {
            InitializeComponent();
            initGridsOnMode();
            openOffPerspective();
        }

        private void initGridsOnMode()
        {
            gridsOnMode = new Grid[7] { gridORoomView, gridOCome, gridOComeNFC, gridOLeave, gridOLeaveNFC, gridOPayNFC, gridOAllocation };
            foreach (Grid g in gridsOnMode)
                g.Visibility = Visibility.Hidden;
        }

        private void showOnModeGrid(Grid grid)
        {
            foreach (Grid g in gridsOnMode)
            {
                if (g.Equals(grid)) g.Visibility = Visibility.Visible;
                else g.Visibility = Visibility.Hidden;
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            LoadRoomDialog roomDialog = new LoadRoomDialog(this, manager.consultingRooms(), false, true);
            roomDialog.Show();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadRoomDialog roomDialog = new LoadRoomDialog(this, manager.consultingCurrentRoom(), false, false);
            roomDialog.Show();
        }

        private void btnCM_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            String number = btn.Name.Substring(5);
            if (number == "Delete")
            {
                if (txtbCapacityCome.Text != "")
                    txtbCapacityCome.Text = txtbCapacityCome.Text.Substring(1, txtbCapacityCome.Text.Length - 1);
            }
            else
                txtbCapacityCome.Text += number;
        }

        private void btnCN_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            String number = btn.Name.Substring(5);
            if (number == "Delete")
            {
                if (txtbCapacityComeNFC.Text != "")
                    txtbCapacityComeNFC.Text = txtbCapacityComeNFC.Text.Substring(1, txtbCapacityComeNFC.Text.Length - 1);
            }
            else
                txtbCapacityComeNFC.Text += number;
        }

        private void btnOCome_Click(object sender, RoutedEventArgs e)
        {
            openArrivalPerspective();
        }

        private void btnOPay_Click(object sender, RoutedEventArgs e)
        {
            openPaymentPerspective();
        }

        private void btnOLeave_Click(object sender, RoutedEventArgs e)
        {
            openExitPerspective();
        }

        private void btnOView_Click(object sender, RoutedEventArgs e)
        {
            manager.RoomManager.updateTables();
            openViewPerspective();
        }

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

        private void openOffPerspective()
        {
            gridInitial.Visibility = Visibility.Visible;
            gridOptions.Visibility = Visibility.Hidden;
            btnOCome.IsEnabled = false;
            btnOLeave.IsEnabled = false;
            btnOView.IsEnabled = false;
        }

        private void openOnPerspective()
        {
            gridInitial.Visibility = Visibility.Hidden;
            gridOptions.Visibility = Visibility.Visible;
            btnOCome.IsEnabled = true;
            btnOLeave.IsEnabled = true;
            btnOView.IsEnabled = true;
            openViewPerspective();
        }

        private void openArrivalPerspective()
        {
            txtbIdCome.Text = idBuilder();
            showOnModeGrid(gridOCome);
        }

        private void openArrivalNFCPerspective()
        {
            showOnModeGrid(gridOComeNFC);
        }

        private void openAllocationPerspective()
        {
            txtbAllocName.Text = manager.ClientManager.Client.Dni;
            manager.RoomManager.getCandidateTables(manager.ClientManager.Guests);
            showOnModeGrid(gridOAllocation);
        }

        private void openPaymentPerspective()
        {
            showOnModeGrid(gridOPayNFC);
        }

        private void openExitPerspective()
        {
            manager.RoomManager.getCandidateTables();
            showOnModeGrid(gridOLeave);
        }

        private void openExitNFCPerspective()
        {
            showOnModeGrid(gridOLeaveNFC);
        }

        private void openViewPerspective()
        {
            resetProvisionalData();
            manager.RoomManager.refreshTables();
            showOnModeGrid(gridORoomView);
        }

        public void loadSelectedRoom(string name, bool reset)
        {
            manager.createRoomManager();
            manager.RoomManager.loadRoom(name, reset);
            generateEmptyRoom(manager.RoomManager.Room.Name,
                manager.RoomManager.Room.Height, manager.RoomManager.Room.Width);
            registerSubjects(manager.RoomManager);
            manager.ClientManager.setGuiReference(this);
            manager.RoomManager.locateObjects();
            if (reset) manager.resetCurrentJourney(name);
            else manager.RoomManager.updateTables();
            manager.initBluetoothServer();
            this.delegateToShowReceiverEvent(0, reset ? "Se inicia de una nueva jornada." : "Se inicia una jornada existente.");
            openOnPerspective();
        }

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

        private void registerSubjects(SubjectRE subjectRE)
        {
            subjectRE.registerInterest(this);
        }

        private void resetProvisionalData()
        {
            btnAcceptAllocation.IsEnabled = false;
            btnAcceptDeallocation.IsEnabled = false;
            txtbAllocName.Text = txtbAllocTable.Text = txtbCapacityCome.Text =
                txtbDeallocName.Text = txtbDeallocTable.Text = "";
            lblPDNI.Content = lblPName.Content = lblPSurname.Content = lblPTable.Content =
                lblPBill.Content = lblPAmount.Content = "-";
            //btnAcceptPayNFC.IsEnabled = true;
            //btnAcceptPayNFC.Content = "PAGADA";
        }

        public void notifyChangesInABox(int row, int column, int state)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri((string)colorBox[state], UriKind.RelativeOrAbsolute);
            bi.EndInit();
            for (int i = 0; i < uGridsRooms.Count(); i++)
                Room[row, column, i].Source = bi;
        }

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
                case 1: p.Foreground = Brushes.DarkGreen; break;   // Llega cliente
                case 2: p.Foreground = Brushes.DarkOrange; break;   // Paga cliente NFC
                case 3: p.Foreground = Brushes.DarkRed; break;      // Se va cliente
                default: break;
            }
            p.Inlines.Add(new Bold(new Run("(" + DateTime.Now + ")\t" + message)));
            if (fdEvents.Blocks.Count > 0)
                fdEvents.Blocks.InsertBefore(fdEvents.Blocks.FirstBlock, p);
            else fdEvents.Blocks.Add(p);
        }

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

        private void btnCancelCome_Click(object sender, RoutedEventArgs e)
        {
            openViewPerspective();
        }

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

        private void btnCancelAllocation_Click(object sender, RoutedEventArgs e)
        {
            if (txtbAllocName.Text.Substring(0, 1) == "C")
                openArrivalPerspective();
            else
                openArrivalNFCPerspective();
            btnAcceptAllocation.IsEnabled = false;
            txtbAllocName.Text = txtbAllocTable.Text = "";
        }

        private void btnAcceptAllocation_Click(object sender, RoutedEventArgs e)
        {
            if (txtbAllocTable.Text != "")
            {
                if (manager.ClientManager.Client.Dni.Substring(0, 1) == "C")
                    manager.ClientManager.newStandardClient();
                manager.RoomManager.confirmAllocation(Convert.ToInt16(txtbAllocTable.Text));
                this.delegateToShowReceiverEvent(1, "El cliente " + txtbAllocName.Text + " llega al restaurante y ocupa la mesa " + txtbAllocTable.Text + ".");
                openViewPerspective();
            }
        }

        private void btnCancelDeallocation_Click(object sender, RoutedEventArgs e)
        {
            btnAcceptDeallocation.IsEnabled = false;
            openViewPerspective();
        }

        private void btnAcceptDeallocation_Click(object sender, RoutedEventArgs e)
        {
            if (txtbDeallocName.Text != "")
            {
                manager.RoomManager.confirmDeallocation(Convert.ToInt16(txtbDeallocTable.Text));
                this.delegateToShowReceiverEvent(3, "El cliente " + txtbDeallocName.Text + " deja libre la mesa " + txtbDeallocTable.Text + " y abandona el restaurante.");
                openViewPerspective();
            }
        }

        private void btnAcceptComeNFC_Click(object sender, RoutedEventArgs e)
        {
            if (txtbCapacityComeNFC.Text != "")
            {
                manager.ClientManager.Guests = Convert.ToInt16(txtbCapacityComeNFC.Text);
                openAllocationPerspective();
                txtbCapacityComeNFC.Text = "";
            }
        }

        private void btnCancelComeNFC_Click(object sender, RoutedEventArgs e)
        {
            openViewPerspective();
        }

        private void btnAcceptLeaveNFC_Click(object sender, RoutedEventArgs e)
        {
            manager.RoomManager.confirmDeallocation(Convert.ToInt16(lblLTable.Content));
            this.delegateToShowReceiverEvent(3, "El cliente " + lblLDNI.Content + " deja libre la mesa " + lblLTable.Content + " y abandona el restaurante.");
            openViewPerspective();
        }

        private void btnCancelLeaveNFC_Click(object sender, RoutedEventArgs e)
        {
            openViewPerspective();
        }

        /*private void btnAcceptPayNFC_Click(object sender, RoutedEventArgs e)
        {
            manager.ClientManager.payBill(Convert.ToInt16(lblPBill.Content), 2);
            manager.RoomManager.confirmDeallocation(Convert.ToInt16(lblPTable.Content));
            this.delegateToShowReceiverEvent(2, "El cliente " + lblPDNI.Content + " paga su cuenta por NFC.");
            this.delegateToShowReceiverEvent(3, "El cliente " + lblPDNI.Content + " deja libre la mesa " + lblPTable.Content + " y abandona el restaurante.");
            btnAcceptPayNFC.IsEnabled = false;
            btnCancelPayNFC.Content = "Salir";
            btnAcceptPayNFC.Content = "COBRADA";
        }*/

        private void btnCancelPayNFC_Click(object sender, RoutedEventArgs e)
        {
            openViewPerspective();
        }

        private void txtbAllocTable_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnAcceptAllocation.IsEnabled = true;
        }

        private void txtbDeallocTable_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnAcceptDeallocation.IsEnabled = true;
        }
    }
}
