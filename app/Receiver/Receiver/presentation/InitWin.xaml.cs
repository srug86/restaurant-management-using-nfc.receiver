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
    /// L�gica de interacci�n para InitWin.xaml
    /// </summary>
    public partial class InitWin : Window
    {
        // M�todo constructor
        public InitWin()
        {
            InitializeComponent();
        }

        // M�todo que cierra los subprocesos de la aplicaci�n tras el cierre de esta
        private void Close_Program(object sender, EventArgs e)
        {
            App.Current.Shutdown();
            foreach (Process p in Process.GetProcesses())
                if (p.ProcessName == "Receiver.vshost") p.Kill();
        }

        /* L�gica de control de eventos */
        // Click en el bot�n "Iniciar jornada"
        private void btnBegin_Click(object sender, RoutedEventArgs e)
        {
            JourneyManagerWin manager = new JourneyManagerWin();
            manager.Show();
        }

        // Click en el bot�n "Editar sal�n"
        private void btnRoom_Click(object sender, RoutedEventArgs e)
        {
            RoomEditorWin editor = new RoomEditorWin();
            editor.Show();
        }

        // Click en el bot�n "Restaurante y clientes"
        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            StatisticsWin statistics = new StatisticsWin();
            statistics.Show();
        }
    }
}
