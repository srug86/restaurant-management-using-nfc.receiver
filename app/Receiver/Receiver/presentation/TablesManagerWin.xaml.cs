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
    /// Lógica de interacción para TablesManagerWin.xaml
    /// </summary>
    public partial class TablesManagerWin : Window
    {
        private TablesManager manager;

        public TablesManager Manager
        {
            get { return manager; }
            set { manager = value; }
        }

        public TablesManagerWin(SubjectTM subject, TablesManager tablesManager)
        {
            InitializeComponent();
            Manager = tablesManager;
            //register(subject);
        }

        public void notify(int row, int column, int state)
        {
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            SetJourneyDialog setJourney = new SetJourneyDialog(this, true);
            setJourney.Show();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            SetJourneyDialog setJourney = new SetJourneyDialog(this, false);
            setJourney.Show();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnCN_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            String number = btn.Name.Substring(4);
            if (number == "Delete")
            {
                if (txtbCapacityCome.Text != "")
                    txtbCapacityCome.Text = txtbCapacityCome.Text.Substring(1, txtbCapacityCome.Text.Length - 1);
            }
            else
                txtbCapacityCome.Text += number;
        }

        private void btnOCome_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOPay_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOLeave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOView_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
