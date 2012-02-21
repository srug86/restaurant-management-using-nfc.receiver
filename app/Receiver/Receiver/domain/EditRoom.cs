using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Receiver.presentation;

namespace Receiver.domain
{
    public class EditRoom : Subject
    {
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
            set { mode = value; }
        }

        private int[,] matrix;

        public int[,] Matrix
        {
            get { return matrix; }
            set { matrix = value; }
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
            Mode = 0;
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

        private void tableValuesHaveChanged()
        {
            DelegateOfTheBox oSwitchBox = new DelegateOfTheBox();
            DelegateOfTheBox.BoxDelegate oBoxDelegate = new DelegateOfTheBox.BoxDelegate(switchBox);
            oSwitchBox.switchBox += oBoxDelegate;
            oSwitchBox.changeContentsBox = BoxState;

            oSwitchBox.switchBox -= oBoxDelegate;
        }

        public void createNewRoom(int rows, int columns)
        {
            NRows = rows;
            NColumns = columns;
            initRoom();
        }

        private void initRoom()
        {
            Tables = new List<TableData>();
            Matrix = new int[NRows, NColumns];
            for (int i = 0; i < NRows; i++)
                for (int j = 0; j < NColumns; j++)
                    Matrix[i, j] = 0;
        }

        public Boolean selectedBox(int row, int column)
        {
            switch (this.mode)
            {
                case 1: case 2: case 3:
                    if (this.getBoxStatus(row, column) == 0)
                    {
                        this.setBoxStatus(row, column, this.mode + this.mode * 10);
                        return true;
                    }
                    break;
                default: break;
            }
            return false;
        }

        public void selectedReceiver()
        {
            this.mode = 1;
        }

        public void selectedBar()
        {
            this.mode = 2;
        }

        public void selectedTable()
        {
            this.mode = 3;
        }

        public Boolean acceptOperation(int id, int capacity)
        {
            if (this.mode == 3)
            {
                TableData table = new TableData(id, capacity);
                for (int i = 0; i < NRows; i++)
                    for (int j = 0; j < NColumns; j++)
                        if (this.getBoxStatus(i, j) == 33)
                        {
                            this.setBoxStatus(i, j, this.mode);
                            table.Place.Add(new int[2] { i, j });
                        }
                if (table.Place.Count != 0) Tables.Add(table);
                this.mode = 0;
                return true;
            }
            else
            {
                for (int i = 0; i < NRows; i++)
                    for (int j = 0; j < NColumns; j++)
                        if (this.getBoxStatus(i, j) > 10)
                            this.setBoxStatus(i, j, this.mode);
                this.mode = 0;
                return false;
            }
        }

        public void resetOperation()
        {
            for (int i = 0; i < NRows; i++)
                for (int j = 0; j < NColumns; j++)
                    if (this.getBoxStatus(i, j) > 10)
                        this.setBoxStatus(i, j, 0);
            this.mode = 0;
        }
    }
}
