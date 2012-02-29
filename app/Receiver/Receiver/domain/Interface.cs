using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Receiver.domain
{
    public interface SubjectER
    {
        void registerInterest(ObserverER obs);
    }

    public interface SubjectTM
    {
        void registerInterest(ObserverTM obs);
    }

    public interface ObserverER
    {
        void notify(int row, int column, int state);
        void notify(int nTable);
    }

    public interface ObserverTM
    {
        void notify(int row, int column, int state);
    }
}
