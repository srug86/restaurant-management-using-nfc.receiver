using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Receiver.domain
{
    class Client
    {
        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        private Table table;

        internal Table Table
        {
            get { return table; }
            set { table = value; }
        }

        private double total;

        public double Total
        {
            get { return total; }
            set { total = value; }
        }
        private Boolean paid;

        public Boolean Paid
        {
            get { return paid; }
            set { paid = value; }
        }
        private List<Product> order;

        internal List<Product> Order
        {
            get { return order; }
            set { order = value; }
        }

        public Client(string id, Table table)
        {
            this.id = id;
            this.table = table;
            this.total = 0.0;
            this.paid = false;
            this.order = new List<Product>();
        }
    }
}
