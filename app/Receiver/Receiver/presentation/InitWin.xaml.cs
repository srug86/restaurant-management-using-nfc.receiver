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
using System.Diagnostics;
using Receiver.domain;

namespace Receiver.presentation
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class InitWin : Window
    {
        public InitWin()
        {
            InitializeComponent();
        }

        private void btnRoom_Click(object sender, RoutedEventArgs e)
        {
            EditRoom editRoom = new EditRoom(-1, -1);
        }

        private void Close_Program(object sender, EventArgs e)
        {
            App.Current.Shutdown();
            foreach (Process p in Process.GetProcesses())
                if (p.ProcessName == "Receiver.vshost") p.Kill();
        }
    }
}
