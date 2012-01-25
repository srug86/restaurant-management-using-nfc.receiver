using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Receiver.domain
{
    class Table: Object
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private int chairs;

        public int Chairs
        {
            get { return chairs; }
            set { chairs = value; }
        }
        private int status;

        public int Status
        {
            get { return status; }
            set { status = value; }
        }
        private Client client;

        internal Client Client
        {
            get { return client; }
            set { client = value; }
        }

        public Table(int id, int chairs)
        {
            this.id = id;
            this.chairs = chairs;
            this.status = 0;
            this.client = null;
        }
    }
}
