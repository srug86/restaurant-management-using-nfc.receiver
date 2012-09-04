using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Receiver.domain
{
    // 'TableDef' define las características de una mesa
    class TableDef
    {
        /* Atributos del objeto */
        private int id, capacity;
        // Identificador
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        // Capacidad total
        public int Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }
        // Casillas que ocupa en la plantilla
        private List<int[]> place;

        public List<int[]> Place
        {
            get { return place; }
            set { place = value; }
        }
        // Método constructor
        public TableDef(int id, int capacity)
        {
            Id = id;
            Capacity = capacity;
            Place = new List<int[]>();
        }
    }
}
