using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Receiver.communication;
using System.Xml;

namespace Receiver.domain
{
    class RoomManager : SubjectRE
    {
        private AdapterWebServices adapter = AdapterWebServices.Instance;

        private List<ObserverRE> reObservers = new List<ObserverRE>();

        private RoomDef room;

        internal RoomDef Room
        {
            get { return room; }
            set { room = value; }
        }

        private int mode, selectedTable;

        public int Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        int rowSelected, columnSelected, boxStatus;

        public int RowSelected
        {
            get { return rowSelected; }
            set { rowSelected = value; }
        }

        public int ColumnSelected
        {
            get { return columnSelected; }
            set { columnSelected = value; }
        }

        public int BoxStatus
        {
            get { return boxStatus; }
            set { boxStatus = value; }
        }

        public RoomManager() { }

        public void loadRoom(string name, bool newJourney)
        {
            xmlDistributionOfRoom(adapter.sendMeRoom(name));
        }

        public int selectedBox(int row, int column)
        {
            int tableID = findTable(row, column);
            if ((Mode == 3 || Mode == 4) && tableID != -1)
            {
                Table table = Room.Tables[Room.Tables.IndexOf(new Table(tableID))];
                if (Mode == 3 && table.Status == -1 &&
                    table.Capacity >= JourneyManager.Instance.ClientManager.Guests)
                {
                    if (selectedTable > 0)
                    {
                        Table prevTable = Room.Tables[Room.Tables.IndexOf(new Table(selectedTable))];
                        paintTable(prevTable, -1);
                    }
                    paintTable(table, 6);
                    selectedTable = table.Id;
                    return table.Id;
                }
                else if (Mode == 4 && table.Status == 3)
                {
                    if (selectedTable > 0)
                    {
                        Table prevTable = Room.Tables[Room.Tables.IndexOf(new Table(selectedTable))];
                        paintTable(prevTable, 3);
                    }
                    paintTable(table, 6);
                    selectedTable = table.Id;
                    return table.Id;
                }
                return -1;
            }
            return -1;
        }

        private void paintTable(Table table, int colour)
        {
            foreach (int[] coordinates in table.Place)
            {
                RowSelected = coordinates[0];
                ColumnSelected = coordinates[1];
                BoxStatus = colour;
                boxStatusHasChanged();
            }
        }

        public string getClientsTable(int table)
        {
            return Room.Tables[Room.Tables.IndexOf(new Table(table))].Client;
        }

        public void updateTables()
        {
            xmlTablesStatus(adapter.sendMeTablesStatus());
        }

        public void getCandidateTables()
        {
            updateTables();
            foreach (Table table in Room.Tables)
                foreach (int[] coordinates in table.Place)
                {
                    RowSelected = coordinates[0];
                    ColumnSelected = coordinates[1];
                    if (table.Status == 3)
                        BoxStatus = 3;
                    else
                        BoxStatus = 2;
                    boxStatusHasChanged();
                }
            Mode = 4;
        }

        public void getCandidateTables(int guests)
        {
            updateTables();
            foreach (Table table in Room.Tables)
                foreach (int[] coordinates in table.Place)
                {
                    RowSelected = coordinates[0];
                    ColumnSelected = coordinates[1];
                    if (table.Status == -1 && table.Capacity >= guests)
                        BoxStatus = -1;
                    else
                        BoxStatus = 0;
                    boxStatusHasChanged();
                }
            Mode = 3;
        }

        public void confirmAllocation(int tableID)
        {
            xmlTablesStatus(adapter.sendAllocationTable(JourneyManager.Instance.ClientManager.Client.Dni,
                tableID, JourneyManager.Instance.ClientManager.Guests));
            selectedTable = -1;
        }

        public void confirmDeallocation()
        {
        }

        public void confirmDeallocation(int tableID)
        {
            xmlTablesStatus(adapter.sendDeallocationTable(tableID));
            selectedTable = -1;
        }

        private void xmlDistributionOfRoom(string sXml)
        {
            if (sXml != "")
            {
                Room = new RoomDef();
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList _room = xml.GetElementsByTagName("Room");
                Room.Name = Convert.ToString(((XmlElement)_room[0]).GetAttribute("name")).Trim();
                XmlNodeList dimension = ((XmlElement)_room[0]).GetElementsByTagName("Dimension");
                XmlNodeList width = ((XmlElement)dimension[0]).GetElementsByTagName("Width");
                Room.Width = Convert.ToInt16(width[0].InnerText);
                XmlNodeList height = ((XmlElement)dimension[0]).GetElementsByTagName("Height");
                Room.Height = Convert.ToInt16(height[0].InnerText);
                XmlNodeList receiver = ((XmlElement)_room[0]).GetElementsByTagName("Receiver");
                XmlNodeList rBoxes = ((XmlElement)receiver[0]).GetElementsByTagName("Boxes");
                XmlNodeList rBoxesList = ((XmlElement)rBoxes[0]).GetElementsByTagName("Box");
                Room.Receiver = new List<int[]>();
                readBoxes(rBoxesList, room.Receiver);
                XmlNodeList bar = ((XmlElement)_room[0]).GetElementsByTagName("Bar");
                XmlNodeList bBoxes = ((XmlElement)bar[0]).GetElementsByTagName("Boxes");
                XmlNodeList bBoxesList = ((XmlElement)bBoxes[0]).GetElementsByTagName("Box");
                Room.Bar = new List<int[]>();
                readBoxes(bBoxesList, room.Bar);
                XmlNodeList tables = ((XmlElement)_room[0]).GetElementsByTagName("Tables");
                XmlNodeList tList = ((XmlElement)tables[0]).GetElementsByTagName("Table");
                Room.Tables = new List<Table>();
                foreach (XmlElement table in tList)
                {
                    Table td = new Table();
                    td.Id = Convert.ToInt16(table.GetAttribute("id"));
                    td.Capacity = Convert.ToInt16(table.GetAttribute("capacity"));
                    XmlNodeList tBoxes = ((XmlElement)table).GetElementsByTagName("Boxes");
                    XmlNodeList tBoxesList = ((XmlElement)tBoxes[0]).GetElementsByTagName("Box");
                    readBoxes(tBoxesList, td.Place);
                    Room.Tables.Add(td);
                }
            }
        }

        private void readBoxes(XmlNodeList srcList, List<int[]> dstList)
        {
            foreach (XmlElement node in srcList)
            {
                XmlNodeList x = node.GetElementsByTagName("X");
                XmlNodeList y = node.GetElementsByTagName("Y");
                dstList.Add(new int[2] { Convert.ToInt16(y[0].InnerText), Convert.ToInt16(x[0].InnerText) });
            }
        }

        private void xmlTablesStatus(string sXml)
        {
            if (sXml != "")
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList tables = xml.GetElementsByTagName("Tables");
                XmlNodeList tList = ((XmlElement)tables[0]).GetElementsByTagName("Table");
                foreach (XmlElement table in tList)
                {
                    int id = Convert.ToInt16(table.GetAttribute("id"));
                    Table taux = new Table(id);
                    XmlNodeList status = ((XmlElement)table).GetElementsByTagName("Status");
                    Room.Tables[Room.Tables.IndexOf(taux)].Status = Convert.ToInt16(status[0].InnerText);
                    XmlNodeList client = ((XmlElement)table).GetElementsByTagName("Client");
                    Room.Tables[Room.Tables.IndexOf(taux)].Client = Convert.ToString(client[0].InnerText).Trim();
                    XmlNodeList guests = ((XmlElement)table).GetElementsByTagName("Guests");
                    Room.Tables[Room.Tables.IndexOf(taux)].Guests = Convert.ToInt16(guests[0].InnerText);
                }
            }
        }

        public void locateObjects()
        {
            foreach (int[] coordinates in Room.Receiver)
            {
                RowSelected = coordinates[0];
                ColumnSelected = coordinates[1];
                BoxStatus = 4;
                boxStatusHasChanged();
            }
            foreach (int[] coordinates in Room.Bar)
            {
                RowSelected = coordinates[0];
                ColumnSelected = coordinates[1];
                BoxStatus = 5;
                boxStatusHasChanged();
            }
        }

        public void refreshTables()
        {
            foreach (Table table in Room.Tables)
                foreach (int[] coordinates in table.Place)
                {
                    RowSelected = coordinates[0];
                    ColumnSelected = coordinates[1];
                    BoxStatus = table.Status;
                    boxStatusHasChanged();
                }
            Mode = 0;
        }

        private int findTable(int row, int column)
        {
            foreach (Table table in Room.Tables)
                foreach (int[] coordinates in table.Place)
                    if (coordinates[0] == row && coordinates[1] == column)
                        return table.Id;
            return -1;
        }

        private void switchBox()
        {
            for (int i = 0; i < reObservers.Count; i++)
            {
                ObserverRE obs = (ObserverRE)reObservers[i];
                obs.notifyChangesInABox(RowSelected, ColumnSelected, BoxStatus);
            }
        }

        private void boxStatusHasChanged()
        {
            DelegateOfTheBox oSwitchBox = new DelegateOfTheBox();
            DelegateOfTheBox.BoxDelegate oBoxDelegate = new DelegateOfTheBox.BoxDelegate(switchBox);
            oSwitchBox.switchBox += oBoxDelegate;
            oSwitchBox.changeContentsBox = BoxStatus;

            oSwitchBox.switchBox -= oBoxDelegate;
        }

        public void registerInterest(ObserverRE obs)
        {
            reObservers.Add(obs);
        }
    }
}
