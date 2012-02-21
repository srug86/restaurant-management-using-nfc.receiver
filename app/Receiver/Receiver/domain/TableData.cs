using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Receiver.domain
{
    class TableData
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private int capacity;

        public int Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }
        private List<int[]> place;

        public List<int[]> Place
        {
            get { return place; }
            set { place = value; }
        }

        public TableData(int id, int capacity)
        {
            Id = id;
            Capacity = capacity;
            Place = new List<int[]>();
        }
    }
}
