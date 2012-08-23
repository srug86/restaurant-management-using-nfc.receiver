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
    /// Lógica de interacción para ClientDialog.xaml
    /// </summary>
    public partial class ClientDialog : Window
    {
        private Client client;

        private int appearances;

        private string status;

        public ClientDialog(Client client, int appearances, string status)
        {
            this.client = client;
            this.appearances = appearances;
            this.status = status;
            InitializeComponent();
            initializeData();
        }

        private void initializeData()
        {
            txtbAppearances.Text = this.appearances.ToString();
            txtbStatus.Text = this.status;
            txtbDni.Text = client.Dni;
            txtbName.Text = client.Name;
            txtbSurname.Text = client.Surname;
            txtbStreet.Text = client.Address.Street;
            txtbNumber.Text = client.Address.Number;
            txtbTown.Text = client.Address.Town;
            txtbState.Text = client.Address.State;
            txtbZipCode.Text = (client.Address.ZipCode > 0) ? client.Address.ZipCode.ToString() : "";
            if (!client.Dni.Substring(0, 1).Equals("C"))
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("/Receiver;component/Images/nfcClient.png", UriKind.RelativeOrAbsolute);
                bi.EndInit();
                imgAvatar.Source = bi;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
