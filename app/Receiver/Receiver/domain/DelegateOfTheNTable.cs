using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Receiver.domain
{
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
