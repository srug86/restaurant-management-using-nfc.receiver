using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Receiver.domain
{
    class DiningRoom
    {
        private List<Table> tables;

        internal List<Table> Tables
        {
            get { return tables; }
            set { tables = value; }
        }
        
        public DiningRoom()
        {
        }
    }
}
