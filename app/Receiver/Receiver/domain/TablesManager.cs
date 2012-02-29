using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Receiver.presentation;

namespace Receiver.domain
{
    public class TablesManager : SubjectTM
    {
        public List<ObserverTM> observers = new List<ObserverTM>();

        public TablesManager()
        {
            TablesManagerWin window = new TablesManagerWin(this, this);
            window.Show();
        }

        public void registerInterest(ObserverTM obs)
        {
            observers.Add(obs);
        }
    }
}
