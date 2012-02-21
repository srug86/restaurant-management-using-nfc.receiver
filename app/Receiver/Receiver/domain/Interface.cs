using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Receiver.domain
{
    public interface Subject
    {
        void registerInterest(Observer obs);
    }

    public interface Observer
    {
        void notify(int row, int column, int state);
    }
}
