﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Receiver.presentation;
using System.IO;
using System.Xml;
using Receiver.communication;

namespace Receiver.domain
{
    public class RoomEditor : SubjectER
    {
        private AdapterWebServices adapter = AdapterWebServices.Instance;

        private List<ObserverER> erObservers = new List<ObserverER>();

        static readonly RoomEditor instance = new RoomEditor();

        /* Atributos de la clase */
        private RoomDef room;

        internal RoomDef Room
        {
            get { return room; }
            set { room = value; }
        }

        private int rowSelected, columnSelected, boxState, mode, nTable;
        // Fila seleccionada
        public int RowSelected  
        {
            get { return rowSelected; }
            set { rowSelected = value; }
        }
        // Columna seleccionada
        public int ColumnSelected   
        {
            get { return columnSelected; }
            set { columnSelected = value; }
        }
        // Estado de la casilla
        public int BoxState     
        {
            get { return boxState; }
            set { boxState = value; }
        }
        // Modo de edición: (0) Empty, (1) Receiver, (2) Bar, (3) Table
        public int Mode
        {
            get { return mode; }
            set {
                if (value == 3)
                    ntableValueHaveChanged();
                mode = value;
            }
        }
        // Identificador de la nueva mesa
        public int NTable
        {
            get { return nTable; }
            set { nTable = value; ntableValueHaveChanged(); }
        }

        private int[,] matrix;
        // Matriz de casillas
        public int[,] Matrix
        {
            get { return matrix; }
            set { matrix = value; }
        }
        // Obtener el estado de una casilla
        public int getBoxStatus(int row, int column)
        {
            return Matrix[row, column];
        }
        // Cambiar el estado de una casilla
        public void setBoxStatus(int row, int column, int state)
        {
            Matrix[row, column] = state;
            RowSelected = row;
            ColumnSelected = column;
            BoxState = state;
            tableValuesHaveChanged();
        }

        /* Implementación de un 'Singleton' para esta clase */
        static RoomEditor() { }

        RoomEditor() { }

        public static RoomEditor Instance
        {
            get
            {
                return instance;
            }
        }

        // 'Seteo' de valores para una nueva plantilla
        public void createNewRoom(String name, int rows, int columns)
        {
            NTable = 1;
            Mode = 0;
            Room = new RoomDef(name, rows, columns);
            initRoom();
        }

        // 'Seteo' del estado de las casillas de la plantilla
        private void initRoom()
        {
            Matrix = new int[Room.Height, Room.Width];
            for (int i = 0; i < Room.Height; i++)
                for (int j = 0; j < Room.Width; j++)
                    setBoxStatus(i, j, 0);
        }

        // Guardar plantilla actual
        public void saveCurrentRoom()
        {
            adapter.sendRoom(Room.Name, xmlRoomBuilder());
        }

        // Cargar plantilla con nombre 'name'
        public void loadExistingRoom(string name)
        {
            xmlDistributionOfRoom(adapter.sendMeRoom(name, false));
            locateObjects();
        }

        // Ubicar los objetos de una plantilla cargada
        public void locateObjects()
        {
            foreach (int[] coordinates in Room.Receiver)
                setBoxStatus(coordinates[0], coordinates[1], 1);
            foreach (int[] coordinates in Room.Bar)
                setBoxStatus(coordinates[0], coordinates[1], 2);
            foreach (TableInf table in Room.Tables)
                foreach (int[] coordinates in table.Place)
                    setBoxStatus(coordinates[0], coordinates[1], 3);
            NTable = Room.Tables.Count + 1;
        }

        // Calcular el estado resultante de una casilla seleccionada
        public Boolean selectedBox(int row, int column)
        {
            switch (Mode)
            {
                case 1: case 2: case 3:
                    if (this.getBoxStatus(row, column) == 0)
                    {
                        this.setBoxStatus(row, column, Mode + Mode * 10);
                        return true;
                    }
                    break;
                default: break;
            }
            return false;
        }

        // Cambio a modo "Ubicar recibidor"
        public void selectedReceiver()
        {
            Mode = 1;
        }

        // Cambio a modo "Ubicar barra"
        public void selectedBar()
        {
            Mode = 2;
        }

        // Cambio a modo "Ubicar mesa"
        public void selectedTable()
        {
            Mode = 3;
        }

        // Confirmar la ubicación de un objeto en la plantilla
        public void acceptOperation(int id, int capacity)
        {
            switch (Mode)
            {
                case 1: confirmBox(Room.Receiver); break;
                case 2: confirmBox(Room.Bar); break;
                case 3: 
                    TableInf table = new TableInf(id, capacity);
                    confirmBox(table.Place);
                    if (table.Place.Count > 0)
                    {
                        Room.Tables.Add(table);
                        NTable = Room.Tables.Count() + 1;
                    }
                    break;
                default: break;
            }
            Mode = 0;
        }

        // Confirmar las casillas ocupadas por el nuevo objeto
        private void confirmBox(List<int[]> list)
        {
            for (int i = 0; i < Room.Height; i++)
                for (int j = 0; j < Room.Width; j++)
                    if (this.getBoxStatus(i, j) == Mode * 10 + Mode)
                    {
                        this.setBoxStatus(i, j, Mode);
                        list.Add(new int[2] { i, j });
                    }
        }

        // 'Reset' de las casillas no confirmadas de un objeto
        public void resetOperation()
        {
            for (int i = 0; i < Room.Height; i++)
                for (int j = 0; j < Room.Width; j++)
                    if (this.getBoxStatus(i, j) > 10)
                        this.setBoxStatus(i, j, 0);
            Mode = 0;
        }

        // Constructor de un XML con la descripción de la plantilla
        public string xmlRoomBuilder()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
            xml += "<Room name=\"" + Room.Name + "\">\n";
            xml += "\t<Dimension>\n";
            xml += "\t\t<Height>" + Room.Height + "</Height>\n";
            xml += "\t\t<Width>" + Room.Width + "</Width>\n";
            xml += "\t\t<NTables>" + Room.getNTables() + "</NTables>\n";
            xml += "\t\t<TCapacity>" + Room.getTCapacity() + "</TCapacity>\n";
            xml += "\t</Dimension>\n";
            xml += "\t<Receiver>\n";
            xml += writeBoxes(Room.Receiver, false);
            xml += "\t</Receiver>\n";
            xml += "\t<Bar>\n";
            xml += writeBoxes(Room.Bar, false);
            xml += "\t</Bar>\n";
            xml += "\t<Tables>\n";
            if (Room.Tables.Count > 0)
                foreach (TableInf table in Room.Tables)
                {
                    xml += "\t\t<Table id=\"" + table.Id + "\" capacity=\"" + table.Capacity + "\">\n";
                    xml += writeBoxes(table.Place, true);
                    xml += "\t\t</Table>\n";
                }
            xml += "\t</Tables>\n";
            xml += "</Room>\n";
            return xml;
        }

        // Constructor de un XML con las casillas que ocupa un objeto en la plantilla del restaurante
        private string writeBoxes(List<int[]> list, Boolean extraTab)
        {
            String tab = extraTab ? "\t" : "";
            string xml = tab + "\t\t<Boxes>\n";
            foreach (int[] coordinates in list)
            {
                xml += tab + "\t\t\t<Box>\n";
                xml += tab + "\t\t\t\t<X>" + coordinates[1] + "</X>\n";
                xml += tab + "\t\t\t\t<Y>" + coordinates[0] + "</Y>\n";
                xml += tab + "\t\t\t</Box>\n";
            }
            xml += tab + "\t\t</Boxes>\n";
            return xml;
        }

        // Decodificador XML de la descripción de una plantilla del restaurante
        private void xmlDistributionOfRoom(string sXml)
        {
            if (sXml != "")
            {
                Room = new RoomDef();
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList _room = xml.GetElementsByTagName("Room");
                Room.Name = Convert.ToString(((XmlElement)_room[0]).GetAttribute("name"));
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
                Room.Tables = new List<TableInf>();
                foreach (XmlElement table in tList)
                {
                    TableInf td = new TableInf(Convert.ToInt16(table.GetAttribute("id")),
                        Convert.ToInt16(table.GetAttribute("capacity")));
                    XmlNodeList tBoxes = ((XmlElement)table).GetElementsByTagName("Boxes");
                    XmlNodeList tBoxesList = ((XmlElement)tBoxes[0]).GetElementsByTagName("Box");
                    readBoxes(tBoxesList, td.Place);
                    Room.Tables.Add(td);
                }
            }
        }

        // Decodificador XML de la ubicación de las casillas de un objeto de la plantilla del restaurante
        private void readBoxes(XmlNodeList srcList, List<int[]> dstList)
        {
            foreach (XmlElement node in srcList)
            {
                XmlNodeList x = node.GetElementsByTagName("X");
                XmlNodeList y = node.GetElementsByTagName("Y");
                dstList.Add(new int[2] { Convert.ToInt16(y[0].InnerText), Convert.ToInt16(x[0].InnerText) });
            }
        }

        /* Métodos que implementan la función 'Observable' */
        // Se notifica a los observadores ante el cambio de estado de una casilla en la plantilla
        private void switchBox()
        {
            for (int i = 0; i < erObservers.Count; i++)
            {
                ObserverER obs = (ObserverER)erObservers[i];
                obs.notifyChangesInABox(RowSelected, ColumnSelected, BoxState);
            }
        }

        // Se notifica a los observadores el identificador para la nueva mesa
        private void switchNTable()
        {
            for (int i = 0; i < erObservers.Count; i++)
            {
                ObserverER obs = (ObserverER)erObservers[i];
                obs.notifyChangesInNTable(NTable);
            }
        }

        public void registerInterest(ObserverER obs)
        {
            erObservers.Add(obs);
        }

        private void ntableValueHaveChanged()
        {
            DelegateOfTheNTable oSwitchNTable = new DelegateOfTheNTable();
            DelegateOfTheNTable.NTableDelegate oNTableDelegate = new DelegateOfTheNTable.NTableDelegate(switchNTable);
            oSwitchNTable.switchNTable += oNTableDelegate;
            oSwitchNTable.changeContentsNTable = NTable;

            oSwitchNTable.switchNTable -= oNTableDelegate;
        }

        private void tableValuesHaveChanged()
        {
            DelegateOfTheBox oSwitchBox = new DelegateOfTheBox();
            DelegateOfTheBox.BoxDelegate oBoxDelegate = new DelegateOfTheBox.BoxDelegate(switchBox);
            oSwitchBox.switchBox += oBoxDelegate;
            oSwitchBox.changeContentsBox = BoxState;

            oSwitchBox.switchBox -= oBoxDelegate;
        }
    }

    public interface SubjectER
    {
        void registerInterest(ObserverER obs);
    }

    public interface ObserverER
    {
        void notifyChangesInABox(int row, int column, int state);
        void notifyChangesInNTable(int nTable);
    }

    class DelegateOfTheBox
    {
        public delegate void BoxDelegate();

        public event BoxDelegate switchBox;

        public object changeContentsBox
        {
            set
            {
                switchBox();
            }
        }
    }

    class DelegateOfTheNTable
    {
        public delegate void NTableDelegate();

        public event NTableDelegate switchNTable;

        public object changeContentsNTable
        {
            set
            {
                switchNTable();
            }
        }
    }
}
