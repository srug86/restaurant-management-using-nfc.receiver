using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Receiver.presentation;
using System.IO;
using System.Xml;

namespace Receiver.domain
{
    public class EditRoom : Subject
    {
        private String fileName;

        public String FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private int nRows;          // número de filas de la malla

        public int NRows
        {
            get { return nRows; }
            set { nRows = value; }
        }
        private int nColumns;       // número de columnas de la malla

        public int NColumns
        {
            get { return nColumns; }
            set { nColumns = value; }
        }
        private int rowSelected;    // fila seleccionada

        public int RowSelected
        {
            get { return rowSelected; }
            set { rowSelected = value; }
        }
        private int columnSelected; // columna seleccionada

        public int ColumnSelected
        {
            get { return columnSelected; }
            set { columnSelected = value; }
        }
        private int boxState;       // estado de la casilla

        public int BoxState
        {
            get { return boxState; }
            set { boxState = value; }
        }
        private int mode;           // 0. empty, 1. receiver, 2. bar, 3. table

        public int Mode
        {
            get { return mode; }
            set {
                if (value == 3)
                    ntableValueHaveChanged();
                mode = value;
            }
        }

        private int nTable;

        public int NTable
        {
            get { return nTable; }
            set { nTable = value; ntableValueHaveChanged(); }
        }

        private int[,] matrix;

        public int[,] Matrix
        {
            get { return matrix; }
            set { matrix = value; }
        }

        private List<int[]> receiverPlace;

        private List<int[]> ReceiverPlace
        {
            get { return receiverPlace; }
            set { receiverPlace = value; }
        }

        private List<int[]> barPlace;

        private List<int[]> BarPlace
        {
            get { return barPlace; }
            set { barPlace = value; }
        }

        private List<TableData> tables;

        internal List<TableData> Tables
        {
            get { return tables; }
            set { tables = value; }
        }

        public int getBoxStatus(int row, int column)
        {
            return Matrix[row, column];
        }

        public void setBoxStatus(int row, int column, int state)
        {
            Matrix[row, column] = state;
            RowSelected = row;
            ColumnSelected = column;
            BoxState = state;
            tableValuesHaveChanged();
        }

        public List<Observer> observers = new List<Observer>();

        public EditRoom(int rows, int columns)
        {
            NTable = 1;
            Mode = 0;
            Tables = new List<TableData>();
            ReceiverPlace = new List<int[]>();
            BarPlace = new List<int[]>();
            EditRoomWin window = new EditRoomWin(this, this);
            window.Show();
        }

        public void registerInterest(Observer obs)
        {
            observers.Add(obs);
        }

        private void switchBox()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                Observer obs = (Observer)observers[i];
                obs.notify(RowSelected, ColumnSelected, BoxState);
            }
        }

        private void switchNTable()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                Observer obs = (Observer)observers[i];
                obs.notify(NTable);
            }
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

        public void createNewRoom(String name, int rows, int columns)
        {
            FileName = name;
            NRows = rows;
            NColumns = columns;
            initRoom();
        }

        private void initRoom()
        {
            Matrix = new int[NRows, NColumns];
            for (int i = 0; i < NRows; i++)
                for (int j = 0; j < NColumns; j++)
                    setBoxStatus(i, j, 0);
        }

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

        public void selectedReceiver()
        {
            Mode = 1;
        }

        public void selectedBar()
        {
            Mode = 2;
        }

        public void selectedTable()
        {
            Mode = 3;
        }

        public void acceptOperation(int id, int capacity)
        {
            switch (Mode)
            {
                case 1: confirmBox(ReceiverPlace); break;
                case 2: confirmBox(BarPlace); break;
                case 3: 
                    TableData table = new TableData(id, capacity);
                    confirmBox(table.Place);
                    if (table.Place.Count != 0)
                    {
                        Tables.Add(table);
                        NTable = Tables.Count() + 1;
                    }
                    break;
                default: break;
            }
            Mode = 0;
        }

        private void confirmBox(List<int[]> list)
        {
            for (int i = 0; i < NRows; i++)
                for (int j = 0; j < NColumns; j++)
                    if (this.getBoxStatus(i, j) == Mode * 10 + Mode)
                    {
                        this.setBoxStatus(i, j, Mode);
                        list.Add(new int[2] { i, j });
                    }
        }

        public void resetOperation()
        {
            for (int i = 0; i < NRows; i++)
                for (int j = 0; j < NColumns; j++)
                    if (this.getBoxStatus(i, j) > 10)
                        this.setBoxStatus(i, j, 0);
            Mode = 0;
        }

        public void xmlBuilder(String path)
        {
            StreamWriter writer = new StreamWriter(path);
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            //writer.WriteLine("<!DOCTYPE Room SYSTEM \"Room.dtd\">");
            writer.WriteLine("<Room name=\"" + FileName + "\">");
            writer.WriteLine("\t<Dimension>");
            writer.WriteLine("\t\t<Height>" + NRows + "</Height>");
            writer.WriteLine("\t\t<Width>" + NColumns + "</Width>");
            writer.WriteLine("\t</Dimension>");
            writer.WriteLine("\t<Receiver>");
            writeBoxes(writer, ReceiverPlace, false);
            writer.WriteLine("\t</Receiver>");
            writer.WriteLine("\t<Bar>");
            writeBoxes(writer, BarPlace, false);
            writer.WriteLine("\t</Bar>");
            writer.WriteLine("\t<Tables>");
            if (Tables.Count != 0)
                foreach (TableData table in Tables)
                {
                    writer.WriteLine("\t\t<Table id=\"" + table.Id + "\" capacity=\"" + table.Capacity + "\">");
                    writeBoxes(writer, table.Place, true);
                    writer.WriteLine("\t\t</Table>");
                }
            writer.WriteLine("\t</Tables>");
            writer.WriteLine("</Room>");
            writer.Close();
        }

        private void writeBoxes(StreamWriter writer, List<int[]> list, Boolean extraTab)
        {
            String tab = "";
            if (extraTab) tab = "\t";
            writer.WriteLine(tab + "\t\t<Boxes>");
            foreach (int[] coordinates in list)
            {
                writer.WriteLine(tab + "\t\t\t<Box>");
                writer.WriteLine(tab + "\t\t\t\t<X>" + coordinates[1] + "</X>");
                writer.WriteLine(tab + "\t\t\t\t<Y>" + coordinates[0] + "</Y>");
                writer.WriteLine(tab + "\t\t\t</Box>");
            }
            writer.WriteLine(tab + "\t\t</Boxes>");
        }

        public void xmlLoader(String path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList room = doc.GetElementsByTagName("Room");
            foreach (XmlElement node in room)
                FileName = node.GetAttribute("name");
            XmlNodeList dimension = ((XmlElement)room[0]).GetElementsByTagName("Dimension");
            XmlNodeList height = ((XmlElement)dimension[0]).GetElementsByTagName("Height");
            NRows = Convert.ToInt32(height[0].InnerText);
            XmlNodeList width = ((XmlElement)dimension[0]).GetElementsByTagName("Width");
            NColumns = Convert.ToInt32(width[0].InnerText);
            XmlNodeList receiver = ((XmlElement)room[0]).GetElementsByTagName("Receiver");
            XmlNodeList rBoxes = ((XmlElement)receiver[0]).GetElementsByTagName("Boxes");
            XmlNodeList rBoxesList = ((XmlElement)rBoxes[0]).GetElementsByTagName("Box");
            ReceiverPlace = new List<int[]>();
            readBoxes(rBoxesList, ReceiverPlace);
            XmlNodeList bar = ((XmlElement)room[0]).GetElementsByTagName("Bar");
            XmlNodeList bBoxes = ((XmlElement)bar[0]).GetElementsByTagName("Boxes");
            XmlNodeList bBoxesList = ((XmlElement)bBoxes[0]).GetElementsByTagName("Box");
            BarPlace = new List<int[]>();
            readBoxes(bBoxesList, BarPlace);
            XmlNodeList tables = ((XmlElement)room[0]).GetElementsByTagName("Tables");
            XmlNodeList tList = ((XmlElement)tables[0]).GetElementsByTagName("Table");
            Tables = new List<TableData>();
            foreach (XmlElement table in tList)
            {
                TableData td = new TableData(-1, -1);
                td.Id = Convert.ToInt32(table.GetAttribute("id"));
                td.Capacity = Convert.ToInt32(table.GetAttribute("capacity"));
                XmlNodeList tBoxes = ((XmlElement)table).GetElementsByTagName("Boxes");
                XmlNodeList tBoxesList = ((XmlElement)tBoxes[0]).GetElementsByTagName("Box");
                readBoxes(tBoxesList, td.Place);
                Tables.Add(td);
            }
        }

        private void readBoxes(XmlNodeList srcList, List<int[]> dstList)
        {
            foreach (XmlElement node in srcList)
            {
                XmlNodeList x = node.GetElementsByTagName("X");
                XmlNodeList y = node.GetElementsByTagName("Y");
                dstList.Add(new int[2] { Convert.ToInt32(y[0].InnerText), Convert.ToInt32(x[0].InnerText) });
            }
        }

        public void updateData()
        {
            NTable = Tables.Count + 1;
            foreach (int[] coordinates in ReceiverPlace)
                setBoxStatus(coordinates[0], coordinates[1], 1);
            foreach (int[] coordinates in BarPlace)
                setBoxStatus(coordinates[0], coordinates[1], 2);
            foreach (TableData table in Tables)
                foreach (int[] coordinates in table.Place)
                    setBoxStatus(coordinates[0], coordinates[1], 3);
        }
    }
}
