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
using System.Collections.ObjectModel;

namespace Receiver.presentation
{
    /// <summary>
    /// Lógica de interacción para LoadRoomDialog.xaml
    /// </summary>
    public partial class LoadRoomDialog : Window
    {
        /* Atributos de la clase 'LoadRoomDialog' */
        private Object dad;

        private List<RoomInf> list;

        private bool edit, resetJourney;

        // Método constructor
        public LoadRoomDialog(Object dad, List<RoomInf> list, bool edit, bool resetJourney)
        {
            this.dad = dad;
            this.list = list;
            this.edit = edit;
            this.resetJourney = resetJourney;
            InitializeComponent();
            initializeData();
            showRooms();
        }

        // Inicialización del contenido de la GUI según el tipo de operación
        private void initializeData()
        {
            if (list.Count == 0)    // No hay plantillas en la BD
            {
                lblInstructions.Content = "No hay ningún restaurante almacenado en el servidor.";
                lblLoadMessage.Content = "";
            }
            if (edit)   // Cargar plantilla en el "Editor de plantillas del restaurante"
            {
                this.Title = "MobiCarta - Cargar restaurante";
                lblLoadMessage.Content = "";
            }
            else if (!resetJourney) // Modo "Cargar jornada existente" de la ventana "Gestor de mesas"
            {
                this.Title = "MobiCarta - Cargar una jornada existente.";
                if (list.Count > 0)
                {
                    lblInstructions.Content = "Pulse 'Cargar' para iniciar la jornada con el restaurante actual.";
                    lblLoadMessage.Content = "* Se conservarán los datos almacenados durante la jornada anterior.";
                }
            }
        }

        // El botón "Cargar plantilla" sólo se activa cuando hay alguna plantilla seleccionada
        private void listVRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnLoad.IsEnabled = IsEnabled;
        }

        // Muestra la lista de plantillas del restaurante
        public void showRooms()
        {
            List<RoomItem> collection = new List<RoomItem>();
            foreach (RoomInf room in list)
                if (!room.Name.Equals(""))
                    collection.Add(new RoomItem()
                    {
                        Name = room.Name,
                        Size = room.Height + "x" + room.Width,
                        Tables = Convert.ToString(room.Tables),
                        Capacity = Convert.ToString(room.Capacity)
                    });
            listVRooms.ItemsSource = collection;
        }

        /* Lógica de control de eventos */
        // Click en el botón "Cancelar"
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        // Click en el botón "Cargar plantilla"
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (edit)   // Para el modo "Editor de plantillas del restaurante"
            {
                RoomEditorWin win = (RoomEditorWin)this.dad;
                foreach (RoomInf room in list)
                    if (room.Name.Equals(((RoomItem)listVRooms.SelectedValue).Name))
                        win.loadSelectedRoom(room.Name, room.Height, room.Width);
            }
            else
            {   // Para el modo "Gestor de mesas"
                JourneyManagerWin win = (JourneyManagerWin)this.dad;
                win.loadSelectedRoom(((RoomItem)listVRooms.SelectedValue).Name, resetJourney);
            }
            this.Visibility = Visibility.Hidden;
        }
    }

    /* Clase auxiliar para representar la información de una plantilla de restaurante en una lista */
    public class RoomItem
    {
        public string name, size, tables, capacity;
        // Nombre de la plantilla
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        // Tamaño de la plantilla (Filas x Columnas)
        public string Size
        {
            get { return size; }
            set { size = value; }
        }
        // Número de mesas
        public string Tables
        {
            get { return tables; }
            set { tables = value; }
        }
        // Capacidad total del restaurante
        public string Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }
    }
}
