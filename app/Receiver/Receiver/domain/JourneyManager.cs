﻿using System;
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

        static JourneyManager() { }

        JourneyManager() { }

        public static JourneyManager Instance
        {
            get
            {
                return instance;
            }
        }

        public bool connectToWS(string url)
        {
            return adapter.connect(url);
        }

        public void initBluetoothServer()
        {
            bluetooth.initBluetooth();
        }

        public void closeBluetoothServer()
        {
            bluetooth.closeBluetooth();
        }

        public List<RoomInf> consultingRooms()
        {
            return xmlListOfRooms(adapter.sendMeRooms());
        }

        public void createRoomManager()
        {
            roomManager = new RoomManager();
            clientManager = new ClientManager();
        }

        public void resetCurrentJourney(string room)
        {
            adapter.sendResetJourney(room);
        }

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
                    roomInf.Name = Convert.ToString(room.GetAttribute("name"));
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
