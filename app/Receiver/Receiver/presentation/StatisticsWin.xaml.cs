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

namespace Receiver.presentation
{
    /// <summary>
    /// Lógica de interacción para StatisticsWin.xaml
    /// </summary>
    public partial class StatisticsWin : Window
    {
        private RestaurantManager restManager = RestaurantManager.Instance;

        private ClientManager clientManager = ClientManager.Instance;

        public StatisticsWin()
        {
            InitializeComponent();
            initializeData();
        }

        private void initializeData()
        {
            lblAddress.Content = "D\nI\nR\nE\nC\nC\nI\nÓ\nN";
            lblContact.Content = "C\nO\nN\nT\nA\nC\nT\nO";
            lblOthers.Content = "O\nT\nR\nO\nS";
        }

        private void btnLoadR_Click(object sender, RoutedEventArgs e)
        {
            loadRestaurantData(restManager.getRestaurantData());
            btnORestaurant.IsEnabled = IsEnabled;
            btnSaveRestaurant.IsEnabled = IsEnabled;
            gridOptions.Visibility = Visibility.Visible;
            gridORestaurant.Visibility = Visibility.Visible;
            gridInitial.Visibility = Visibility.Hidden;
        }

        private void btnSaveR_Click(object sender, RoutedEventArgs e)
        {
            restManager.setRestaurantData(saveRestaurantData());
        }

        private void btnLoadC_Click(object sender, RoutedEventArgs e)
        {
            loadClientsData(clientManager.getClientsData());
            btnOClients.IsEnabled = IsEnabled;
            gridOptions.Visibility = Visibility.Visible;
            gridOClients.Visibility = Visibility.Visible;
            gridInitial.Visibility = Visibility.Hidden;
        }

        private void btnORestaurant_Click(object sender, RoutedEventArgs e)
        {
            gridORestaurant.Visibility = Visibility.Visible;
            gridOClients.Visibility = Visibility.Hidden;
        }

        private void btnOClients_Click(object sender, RoutedEventArgs e)
        {
            gridOClients.Visibility = Visibility.Visible;
            gridORestaurant.Visibility = Visibility.Hidden;
        }

        private void btnConsult_Click(object sender, RoutedEventArgs e)
        {
            if (listVClients.SelectedIndex != -1)
            {
                ShortClient sc = (ShortClient)((ListViewItem)listVClients.SelectedItem).Content;
                Client client = clientManager.getClientData(sc.Dni);
                ClientDialog clientDialog = new ClientDialog(client, sc.Appearances, sc.Status);
                clientDialog.Show();
            }
        }

        private void loadRestaurantData(List<Object> data)
        {
            Company company = (Company)data[0];
            Address address = (Address)data[1];
            txtbName.Text = company.Name;
            txtbNIF.Text = company.NIF;
            txtbPhone.Text = company.Phone.ToString();
            txtbFax.Text = company.Fax.ToString();
            txtbEmail.Text = company.Email;
            txtbStreet.Text = address.Street;
            txtbNumber.Text = address.Number;
            txtbTown.Text = address.Town;
            txtbState.Text = address.State;
            txtbZipCode.Text = address.ZipCode.ToString();
            txtbIVA.Text = data[2].ToString();
            txtbDiscount.Text = data[3].ToString();
            txtbDiscVisit.Text = data[4].ToString();
        }

        private List<Object> saveRestaurantData()
        {
            List<Object> data = new List<Object>();
            if (!txtbName.Text.Equals("") && !txtbNIF.Equals("") && Convert.ToInt32(txtbPhone.Text) >= 0 &&
                Convert.ToInt32(txtbFax.Text) >= 0 && !txtbEmail.Text.Equals("") && !txtbStreet.Text.Equals("")
                && !txtbNumber.Text.Equals("") && !txtbState.Text.Equals("") && !txtbTown.Text.Equals("") &&
                Convert.ToInt32(txtbZipCode.Text) >= 0 && Convert.ToDouble(txtbIVA.Text) >= 0.0 &&
                Convert.ToDouble(txtbDiscount.Text) >= 0.0 && Convert.ToInt32(txtbDiscVisit.Text) >= 0)
            {
                data.Add(new Company(txtbName.Text, txtbNIF.Text, Convert.ToInt32(txtbPhone.Text),
                    Convert.ToInt32(txtbFax.Text), txtbEmail.Text));
                data.Add(new Address(txtbStreet.Text, txtbNumber.Text, txtbTown.Text,
                    txtbState.Text, Convert.ToInt32(txtbZipCode.Text)));
                data.Add(Convert.ToDouble(txtbIVA.Text));
                data.Add(Convert.ToDouble(txtbDiscount.Text));
                data.Add(Convert.ToInt32(txtbDiscVisit.Text));
            }
            return data;
        }

        private void loadClientsData(List<ShortClient> clients)
        {
            listVClients.Items.Clear();
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Id = i + 1;
                clients[i].Status = (Convert.ToInt32(clients[i].Status) > 0) ? "Activo" : "No activo";
                listVClients.Items.Add(new ListViewItem());
                ((ListViewItem)listVClients.Items[listVClients.Items.Count - 1]).Content = clients[i];
            }
        }
    }

    public class ShortClient
    {
        private string status, dni, name, surname;

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public string Dni
        {
            get { return dni; }
            set { dni = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Surname
        {
            get { return surname; }
            set { surname = value; }
        }

        private int id, appearances;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Appearances
        {
            get { return appearances; }
            set { appearances = value; }
        }

        public ShortClient() { }
    }
}
