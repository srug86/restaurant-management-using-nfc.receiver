using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Receiver.communication;
using System.Xml;

namespace Receiver.domain
{
    class JourneyManager
    {
        private AdapterWebServices adapter = AdapterWebServices.Instance;

        private BluetoothServer bluetooth = BluetoothServer.Instance;

        static readonly JourneyManager instance = new JourneyManager();

        RoomManager roomManager;
        internal RoomManager RoomManager
        {
            get { return roomManager; }
            set { roomManager = value; }
        }

        ClientManager clientManager;
        internal ClientManager ClientManager
        {
            get { return clientManager; }
            set { clientManager = value; }
        }

        /* Implementación de un 'Singleton' para esta clase */
        static JourneyManager() { }

        JourneyManager() { }

        public static JourneyManager Instance
        {
            get
            {
                return instance;
            }
        }

        // Inicia el servidor Bluetooth
        public void initBluetoothServer()
        {
            bluetooth.initBluetooth();
        }

        // Cierra el servidor Bluetooth
        public void closeBluetoothServer()
        {
            bluetooth.closeBluetooth();
        }

        // Devuelve la lista de plantillas para el restaurante
        public List<RoomInf> consultingRooms()
        {
            return xmlListOfRooms(adapter.sendMeRooms());
        }

        // Devuelve el nombre de la plantilla de la jornada actual
        public List<RoomInf> consultingCurrentRoom()
        {
            return xmlListOfRooms(adapter.sendMeCurrentRoom());
        }

        // Crea una instancia del gestor de clientes
        public void createRoomManager()
        {
            roomManager = new RoomManager();
            clientManager = ClientManager.Instance;
        }

        // 'Resetea' los datos de la jornada actual al crearse una "nueva jornada"
        public void resetCurrentJourney(string room)
        {
            adapter.sendResetJourney(room);
        }

        // Decodifica XML con la lista de plantillas disponibles para el restaurante
        private List<RoomInf> xmlListOfRooms(string sXml)
        {
            List<RoomInf> listOfRooms = new List<RoomInf>();
            if (sXml != "")
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList rooms = xml.GetElementsByTagName("Rooms");
                XmlNodeList rList = ((XmlElement)rooms[0]).GetElementsByTagName("Room");
                foreach (XmlElement room in rList)
                {
                    RoomInf roomInf = new RoomInf();
                    roomInf.Name = Convert.ToString(room.GetAttribute("name")).Trim();
                    XmlNodeList width = ((XmlElement)room).GetElementsByTagName("Width");
                    roomInf.Width = Convert.ToInt16(width[0].InnerText);
                    XmlNodeList height = ((XmlElement)room).GetElementsByTagName("Height");
                    roomInf.Height = Convert.ToInt16(height[0].InnerText);
                    XmlNodeList nTables = ((XmlElement)room).GetElementsByTagName("Tables");
                    roomInf.Tables = Convert.ToInt16(nTables[0].InnerText);
                    XmlNodeList tCapacity = ((XmlElement)room).GetElementsByTagName("Capacity");
                    roomInf.Capacity = Convert.ToInt16(tCapacity[0].InnerText);
                    listOfRooms.Add(roomInf);
                }
            }
            return listOfRooms;
        }
    }
}
